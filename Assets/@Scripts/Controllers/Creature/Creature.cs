using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Creature : BaseObject
    {
        public SkillComponent CreatureSkill { get; protected set; } = null;
        public CreatureBody CreatureBody { get; protected set; } = null;
        public CreatureAnimation CreatureAnim { get; private set; } = null;

        [SerializeField] private ECreatureState _creatureState =  ECreatureState.Idle;
        public virtual ECreatureState CreatureState
        {
            get => _creatureState;
            set
            {
                if (_creatureState != value)
                {
                    _creatureState = value;
                    UpdateAnimation();
                }
            }
        }
        public void SetCreatureState(ECreatureState creatureState)
            => _creatureState = creatureState;
        [SerializeField] private ECreatureMoveState _creatureMoveState = ECreatureMoveState.None;
        public virtual ECreatureMoveState CreatureMoveState
        {
            get => _creatureMoveState;
            set => _creatureMoveState = value;
        }
        [SerializeField] private bool _collectEnv = false;
        public virtual bool CollectEnv
        {
            get => _collectEnv;
            set => _collectEnv = value;
        }
        public virtual Vector3Int ChaseCellPos // *** NO SETTER !! ***
        {
            get
            {
                if (Target.IsValid())
                    return Target.CellPos;
                else
                    return CellPos;
            }
        }
        

        [SerializeField] protected EFindPathResult _findPathResult = EFindPathResult.None;
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            // 최초 Animator Entry State는 Idle이기 때문에 최초에는 이와 동기화 시켜준다.
            // _creatureState = ECreatureState.Idle;
            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame(dataID);
                return false;
            }

            CreatureAnim = BaseAnim as CreatureAnimation;

            return true;
        }

        protected override void SetStat(int dataID)
        {
            base.SetStat(dataID);
            for (int i = DataTemplateID; i < DataTemplateID + 10;)
            {
                if (Managers.Data.StatDataDict.ContainsKey(i) == false)
                    break;

                _maxLevel = i++;
            }
        }

        protected override void EnterInGame(int dataID)
        {
            base.EnterInGame(dataID);
            RigidBody.simulated = false;
            CollectEnv = false;
            Target = null;

            ShowBody(false);
            StartWait(waitCondition: () => BaseAnim.IsPlay() == false,
                      callbackWaitCompleted: () => {
                          ShowBody(true);
                          RigidBody.simulated = true;
                          Target = null;
                          CancelWait();
                          AddAnimationEvents();
                          //CreatureState = ECreatureState.Idle; --> 각 클래스에서 관리
                          CreatureMoveState = ECreatureMoveState.None;
                          StartCoroutine(CoUpdateAI()); // --> Leader도 AI 돌고 있어야함.
                          StartCoLerpToCellPos();
                          // StartCoroutine(CoLerpToCellPos()); // Map
                      });

            // StartWait(waitCondition: () => BaseAnim.IsPlay() == false,
            //         delegate()
            //         {
            //         });
        }

        private void AddAnimationEvents()
        {
            CreatureStateMachine[] creatureStateMachines = CreatureAnim.Animator.GetBehaviours<CreatureStateMachine>();
            for (int i = 0; i < creatureStateMachines.Length; ++i)
            {
                creatureStateMachines[i].OnStateEnterHandler -= OnStateEnter;
                creatureStateMachines[i].OnStateEnterHandler += OnStateEnter;

                creatureStateMachines[i].OnStateUpdateHandler -= OnStateUpdate;
                creatureStateMachines[i].OnStateUpdateHandler += OnStateUpdate;

                creatureStateMachines[i].OnStateEndHandler -= OnStateEnd;
                creatureStateMachines[i].OnStateEndHandler += OnStateEnd;
            }
        }

        private void ReleaseAnimationEvents()
        {
            if (CreatureAnim == null)
                return;

            CreatureStateMachine[] creatureStateMachines = CreatureAnim.Animator.GetBehaviours<CreatureStateMachine>();
            for (int i = 0; i < creatureStateMachines.Length; ++i)
            {
                creatureStateMachines[i].OnStateEnterHandler -= OnStateEnter;
                creatureStateMachines[i].OnStateUpdateHandler -= OnStateUpdate;
                creatureStateMachines[i].OnStateEndHandler -= OnStateEnd;
            }
        }

        protected IEnumerator CoUpdateAI()
        {
            while (true)
            {
                switch (CreatureState)
                {
                    case ECreatureState.Idle:
                        UpdateIdle();
                        break;

                    case ECreatureState.Move:
                        UpdateMove();
                        break;

                    case ECreatureState.Skill_Attack:
                    case ECreatureState.Skill_A:
                    case ECreatureState.Skill_B:
                        UpdateSkill();
                        break;

                    case ECreatureState.CollectEnv:
                        UpdateCollectEnv();
                        break;

                    case ECreatureState.Dead:
                        UpdateDead();
                        break;
                }
                
                yield return null;
            }
        }

        protected virtual void UpdateIdle() { }
        protected virtual void UpdateMove() { }
        protected virtual void UpdateSkill() { }
        protected virtual void UpdateCollectEnv() { }
        protected virtual void UpdateDead()
        {
            // SetRigidBodyVelocity(Vector2.zero); - DELETED
            CancelWait();
        }

        #region Coroutines
        // Co Wait 없앨준비하라네
        protected Coroutine _coWait = null;
        protected void StartWait(float seconds)
        {
            CancelWait();
            _coWait = StartCoroutine(CoWait(seconds));
        }
        private IEnumerator CoWait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _coWait = null;
        }

        protected void StartWait(System.Func<bool> waitCondition, System.Action callbackWaitCompleted = null)
        {
            CancelWait();
            _coWait = StartCoroutine(CoWait(waitCondition, callbackWaitCompleted));
        }
        private IEnumerator CoWait(System.Func<bool> waitCondition, System.Action waitCompleted = null)
        {
            yield return new WaitUntil(waitCondition);
            _coWait = null;
            waitCompleted?.Invoke();
        }

        protected void CancelWait()
        {
            if (_coWait != null)
                StopCoroutine(_coWait);
            _coWait = null;
        }
        #endregion Coroutines

        #region Events
        protected void OnStateEnter(ECreatureState enterState)
        {
            switch (enterState)
            {
                case ECreatureState.Skill_Attack:
                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    CreatureSkill?.PassOnSkillStateEnter(enterState);
                    break;

                case ECreatureState.CollectEnv:
                    OnCollectEnvStateEnter();
                    break;
            }
        }
        protected virtual void OnCollectEnvStateEnter()
        {
             CollectEnv = true;
        }

        protected void OnStateUpdate(ECreatureState updateState)
        {
            switch (updateState)
            {
                case ECreatureState.Skill_Attack:
                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    // 스킬을 사용하는 주체가 크리처이기 때문에 여기서 이벤트 등록, 삭제하고 호출
                    CreatureSkill.PassOnSkillStateUpdate(updateState);
                    break;

                case ECreatureState.CollectEnv:
                    OnCollectEnvStateUpdate();
                    break;
            }
        }

        protected virtual void OnCollectEnvStateUpdate() { }

        protected void OnStateEnd(ECreatureState endState)
        {
            switch (endState)
            {
                case ECreatureState.Skill_Attack:
                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    CreatureSkill?.PassOnSkillStateEnd(endState);
                    break;
            }
        }
        #endregion State Machine Events 

        protected BaseObject FindClosestInRange(float scaneRange, IEnumerable<BaseObject> objs, System.Func<BaseObject, bool> func = null)
        {
            BaseObject target = null;
            float bestDistSQR = float.MaxValue;
            float scanRangeSQR = scaneRange * scaneRange;

            foreach (BaseObject obj in objs)
            {
                //Vector3 dir = obj.transform.position - transform.position;
                // 5칸인지 테스트
                Vector3Int dir = obj.CellPos - CellPos;
                float distToTargetSQR = dir.sqrMagnitude;

                if (scanRangeSQR < distToTargetSQR)
                    continue;

                if (bestDistSQR < distToTargetSQR)
                    continue;

                // 추가 조건 (ex)객체가 살아있는가? 람다 기반으로 넘겨줄수도 있으니까 뭐, 편함 이런게 있으면.
                // 살아있는 객체만 찾아서, 살아있는 객체에게만 Target을 잡을 수 있도록. 이런식으로 가능.
                if (func?.Invoke(obj) == false)
                    continue;

                bestDistSQR = distToTargetSQR;
                target = obj;
            }

            return target;
        }

        #region Battle
        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            Creature creature = attacker as Creature;
            if (creature.IsValid() == false)
                return;

            float finalDamage = creature.Atk;
            Hp = UnityEngine.Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
            Managers.Object.ShowDamageFont(position: this.CenterPosition, damage: finalDamage, isCritical: false);
            //Debug.Log($"<color=white>{gameObject.name}: ({Hp}/{MaxHp})</color>");

            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillFromAttacker);
            }
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            StopCoLerpToCellPos(); // 길찾기 움직임 중이었다면 멈춘다.
            StopCoSearchTarget(); // 크리처에서 돌고있는 모든 코루틴을 Dictionary로 관리하면 어떨까.
            CreatureMoveState = ECreatureMoveState.None;
            CreatureState = ECreatureState.Dead;
            base.OnDead(attacker, skillFromAttacker);
        }

        #endregion Battle
        protected bool IsInAttackRange()
        {
            if (Target.IsValid() == false)
                return false;

            int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
            int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);

            if (Target.ObjectType == EObjectType.Monster)
            {
                if (dx <= AtkRange && dy <= AtkRange)
                    return true;
            }
            else if (Target.ObjectType == EObjectType.Env)
            {
                if (dx <= 1 && dy <= 1)
                    return true;
            }

            return false;
        }

        public bool CanAttackOrChase()
        {
            if (Target.IsValid() == false) // --- DEFENSE
                return false;

            if (IsInAttackRange())
                return true;

            return false;
        }


        // protected void ChaseOrAttackTarget_Prev_Temp(float chaseRange, float atkRange)
        // {
        //     //Vector3 toTargetDir = Target.transform.position - transform.position;
        //     if (DistanceToTargetSQR <= atkRange * atkRange)
        //     {
        //         if (Target.IsValid() && Target.ObjectType == EObjectType.Env)
        //             CreatureState = ECreatureState.CollectEnv;
        //         else
        //             CreatureSkill?.CurrentSkill.DoSkill();
        //     }
        //     else
        //     {
        //         _findPathResult = FindPathAndMoveToCellPos(Target.transform.position, ReadOnly.Numeric.HeroMaxMoveDepth);
        //         float searchDistSQR = chaseRange * chaseRange;
        //         if (DistanceToTargetSQR > searchDistSQR)
        //         {
        //             Target = null;
        //             CreatureState = ECreatureState.Move;
        //         }
        //     }
        // }

        // Util로 빼야할듯
        protected float CalculateMovementSpeed(float distanceToTargetSQR)
        {
            float maxDistance = ReadOnly.Numeric.MaxDistanceForMovementSpeed;
            float maxMovementSpeed = MovementSpeed * ReadOnly.Numeric.MaxMovementSpeedMultiplier;
            float movementSpeed = Mathf.Lerp(MovementSpeed, 
                                            maxMovementSpeed, 
                                            Mathf.Log(distanceToTargetSQR + 1.0f) / Mathf.Log(maxDistance * maxDistance + 1.0f));

            return movementSpeed;
        }

        protected override void OnDisable()
        {
            //Debug.Log("Creature::OnDisable");
            ReleaseAnimationEvents();
        }

        #region Misc
        protected bool IsValid(BaseObject bo) => bo.IsValid();
        public bool IsSkillState
            => CreatureState == ECreatureState.Skill_Attack || CreatureState == ECreatureState.Skill_A || CreatureState == ECreatureState.Skill_B;
        #endregion Misc

        public EFindPathResult FindPathAndMoveToCellPos(Vector3 destPos, int maxDepth, EObjectType ignoreObjectType = EObjectType.None)
        {
            Vector3Int destCellPos = Managers.Map.WorldToCell(destPos);
            return FindPathAndMoveToCellPos(destCellPos, maxDepth, ignoreObjectType);
        }

        public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destPos, int maxDepth, EObjectType ignoreObjectType = EObjectType.None)
        {
            if (IsForceMovingPingPongObject)
                return EFindPathResult.Fail_ForceMovePingPongObject;

            // ***** 이미 스킬(공격)중이면 길찾기 금지 *****
            if (CreatureState == ECreatureState.Skill_Attack)
                return EFindPathResult.Fail_LerpCell;

            // 움직임 진행중
            if (LerpToCellPosCompleted == false)
            {
                return EFindPathResult.Fail_LerpCell;
            }

            // A*
            List<Vector3Int> path = Managers.Map.FindPath(startCellPos: CellPos, destPos, maxDepth, ignoreObjectType);
            if (path.Count < 2)
                return EFindPathResult.Fail_NoPath;

            Vector3Int dirCellPos = path[1] - CellPos;
            Vector3Int nextPos = CellPos + dirCellPos;

            if (Managers.Map.MoveTo(creature: this, nextPos, ignoreObjectType: ignoreObjectType) == false)
                return EFindPathResult.Fail_MoveTo;

            return EFindPathResult.Success;
        }

        // 제거예정
        public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
        {
            if (LerpToCellPosCompleted == false)
                return false;

            return Managers.Map.MoveTo(this, cellPos: destCellPos);
        }


        protected void StartCoLerpToCellPos()
        {
            StopCoLerpToCellPos();
            _coLerpToCellPos = StartCoroutine(CoLerpToCellPos());
        }

        protected void StopCoLerpToCellPos()
        {
            if (_coLerpToCellPos != null)
            {
                StopCoroutine(_coLerpToCellPos);
                _coLerpToCellPos = null;
            }
        }

        // ### Creature: O, Projectile: X
        protected Coroutine _coLerpToCellPos = null;
        protected IEnumerator CoLerpToCellPos()
        {
            while (true)
            {
                if (IsForceMovingPingPongObject)
                {
                    yield return null;
                    continue;
                }

                Hero hero = this as Hero;
                if (hero.IsValid())
                {
                    // 1. 리더의 MovementSpeed가 느리면 멤버들의 Movement Speed도 Leader에게 맞춰진다.
                    // --> 리더 주변으로 Chase해야하기 때문
                    // 2. 1번이 어색하게 느껴지면 MovementSpeed는 통합 Stat으로 관리
                    float movementSpeed = Util.CalculateValueFromDistance(
                        value: Managers.Object.HeroLeaderController.Leader.MovementSpeed,
                        maxValue: Managers.Object.HeroLeaderController.Leader.MovementSpeed * 2f,
                        distanceToTargetSQR: (CellPos - Managers.Object.HeroLeaderController.Leader.CellPos).sqrMagnitude,
                        maxDistanceSQR: ReadOnly.Numeric.HeroDefaultScanRange * ReadOnly.Numeric.HeroDefaultScanRange
                    );

                    LerpToCellPos(movementSpeed);
                }
                else // Monster
                    LerpToCellPos(MovementSpeed);

                yield return null;
            }
        }

        private T SearchClosestInRange<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, 
                                        System.Func<T, bool> func = null) where T : BaseObject
        {
            T target = null;
            float bestDistSQR = float.MaxValue;
            float scanRangeSQR = scanRange * scanRange;

            // --- Set Hero Leader Scan Range (half)
            Hero hero = this as Hero;
            if (hero.IsValid() && hero.IsLeader)
                scanRangeSQR *= 0.8f;

            foreach (T obj in firstTargets)
            {
                Vector3Int dir = obj.CellPos - CellPos;
                float distToTargetSQR = dir.sqrMagnitude;
                if (scanRangeSQR < distToTargetSQR)
                    continue;

                if (bestDistSQR < distToTargetSQR)
                    continue;

                if (func?.Invoke(obj) == false)
                    continue;

                bestDistSQR = distToTargetSQR;
                target = obj;
            }

            if (target != null) // First Target
                return target;
            else if (target == null && secondTargets != null) // Second Targets
            {
                foreach (T obj in secondTargets)
                {
                    Vector3Int dir = obj.CellPos - CellPos;
                    float distToTargetSQR = dir.sqrMagnitude;
                    if (scanRangeSQR < distToTargetSQR)
                        continue;

                    if (bestDistSQR < distToTargetSQR)
                        continue;

                    if (func?.Invoke(obj) == false)
                        continue;

                    bestDistSQR = distToTargetSQR;
                    target = obj;
                }
            }

            return target;
        }

        protected bool _pauseSearchTarget = false;
        private Coroutine _coSearchTarget = null;
        private IEnumerator CoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null) where T : BaseObject
        {
            float tick = ReadOnly.Numeric.SearchFindTargetTick;
            while (true)
            {
                if (this.IsValid() == false) // 방어
                    yield break;

                yield return new WaitForSeconds(tick);
                if (_pauseSearchTarget)
                {
                    Target = null;
                    yield return null;
                    continue;
                }

                if (CreatureMoveState != ECreatureMoveState.ForceMove)
                {
                    if (_coWaitSearchTarget != null) // search tick이후, 이미 Wait이 진행중이었다면.. 
                    {
                        yield return null;
                        continue;
                    }

                    Target = SearchClosestInRange(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func);
                    if (Target.IsValid())
                        CreatureMoveState = ECreatureMoveState.MoveToTarget;
                    // --- Target이 존재하지 않을 때, MoveToTarget 해제는 Creature AI에서 해결    
                }
            }
        }

        protected void StartCoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null) where T : BaseObject
        {
            if (_coSearchTarget != null)
                return;

            _coSearchTarget = StartCoroutine(CoSearchTarget<T>(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func));
        }

        protected void StopCoSearchTarget()
        {
            Target = null;
            if (_coSearchTarget != null)
            {
                StopCoroutine(_coSearchTarget);
                _coSearchTarget = null;
            }
        }

        protected Coroutine _coWaitSearchTarget = null;
        private IEnumerator CoWaitSearchTarget(float waitSeconds)
        {
            // --- 확인됨
            //Debug.Log($"<color=white>{nameof(CoWaitSearchTarget)}</color>");
            yield return new WaitForSeconds(waitSeconds);
            StopCoWaitSearchTarget();
        }

        protected void StartCoWaitSearchTarget(float waitSeconds)
        {
            if (_coWaitSearchTarget != null)
                return;

            _coWaitSearchTarget = StartCoroutine(CoWaitSearchTarget(waitSeconds));
        }

        protected void StopCoWaitSearchTarget() // PRIVATE
        {
            if (_coWaitSearchTarget != null)
            {
                StopCoroutine(_coWaitSearchTarget);
                _coWaitSearchTarget = null;
            }

            // --- 확인됨
            // Debug.Log($"<color=white>{nameof(StopCoWaitSearchTarget)}</color>");
        }

        /*
            1 - 큐에 A 추가: [A]
            2 - 큐에 B 추가: [A, B]
            3 - 큐에 A 추가: [A, B, A]
            4 - 큐에 B 추가: [A, B, A, B]
            5 - 1 (검사)
            5 - 2 Dequeue: [B, A, B]
            5 - 3 큐에 A 추가: [B, A, B, A]
            6 - 1 (검사)
            6 - 2 Dequeue: [A, B, A]
            6 - 3 큐에 B 추가: [A, B, A, B]
            7 - 1 (검사)
            ...

            이게 정상이긴한데 위치가 정확하게 입력되지 않음.
            그래서 큐의 4개의 요소 안에 A가 2개, B가 2개가 있는지만 확인.
            나중에 몬스터에서도 필요하면 Creature로 옮겨주면 됨
        */
        private Queue<Vector3Int> _cantMoveCheckQueue = new Queue<Vector3Int>();
        [SerializeField] protected int _currentPingPongCantMoveCount = 0;
        private int maxCantMoveCheckCount = 4; // 2칸에 대해 왔다 갔다만 조사하는 것이라 4로 설정
        protected bool IsPingPongAndCantMoveToDest(Vector3Int cellPos)
        {
            if (_cantMoveCheckQueue.Count >= maxCantMoveCheckCount)
                _cantMoveCheckQueue.Dequeue();

            _cantMoveCheckQueue.Enqueue(cellPos);
            if (_cantMoveCheckQueue.Count == maxCantMoveCheckCount)
            {
                Vector3Int[] cellArr = _cantMoveCheckQueue.ToArray();
                HashSet<Vector3Int> uniqueCellPos = new HashSet<Vector3Int>(cellArr);
                if (uniqueCellPos.Count == 2)
                {
                    Dictionary<Vector3Int, int> checkCellPosCountDict = new Dictionary<Vector3Int, int>();
                    foreach (var pos in _cantMoveCheckQueue)
                    {
                        if (checkCellPosCountDict.ContainsKey(pos))
                            checkCellPosCountDict[pos]++;
                        else
                            checkCellPosCountDict[pos] = 1;
                    }

                    foreach (var count in checkCellPosCountDict.Values)
                    {
                        if (count != 2)
                            return false;
                    }

                    return true;
                }
            }

            return false;
        }

        // ***** Force Move Ping Pong Object Coroutine *****
        private Coroutine _coForceMovePingPongObject = null;
        protected bool IsForceMovingPingPongObject => _coForceMovePingPongObject != null;
        private IEnumerator CoForceMovePingPongObject(Vector3Int currentCellPos, Vector3Int destCellPos, System.Action endCallback = null)
        {
            List<Vector3Int> path = Managers.Map.FindPath(currentCellPos, destCellPos);

            Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
            for (int i = 0; i < path.Count; ++i)
                pathQueue.Enqueue(path[i]);
            pathQueue.Dequeue();

            Vector3Int nextPos = pathQueue.Dequeue();
            Vector3 currentWorldPos = Managers.Map.CellToWorld(currentCellPos);
            while (pathQueue.Count != 0)
            {
                Vector3 destPos = Managers.Map.CenteredCellToWorld(nextPos);
                Vector3 dir = destPos - transform.position; // 왜 이걸로하면 안되지
                if (dir.x < 0f)
                    LookAtDir = ELookAtDirection.Left;
                else if (dir.x > 0f)
                    LookAtDir = ELookAtDirection.Right;
                
                if (dir.sqrMagnitude < 0.01f)
                {
                    transform.position = destPos;
                    currentWorldPos = transform.position;
                    nextPos = pathQueue.Dequeue();
                }
                else
                {
                    float moveDist = Mathf.Min(dir.magnitude, MovementSpeed * Time.deltaTime);
                    transform.position += dir.normalized * moveDist; // Movement per frame.
                }

                yield return null;
            }

            endCallback?.Invoke();
            UpdateCellPos();
        }

        protected void CoStartForceMovePingPongObject(Vector3Int currentCellPos, Vector3Int destCellPos, System.Action endCallback = null)
        {
            if (_coForceMovePingPongObject != null)
                return;

            _coForceMovePingPongObject = StartCoroutine(CoForceMovePingPongObject(currentCellPos, destCellPos, endCallback));
        }

        protected void CoStopForceMovePingPongObject()
        {
            if (_coForceMovePingPongObject != null)
            {
                StopCoroutine(_coForceMovePingPongObject);
                _coForceMovePingPongObject = null;
            }
        }
    }
}

/*
        [ PREV CODEs ]
        protected IEnumerator CoLerpToCellPos()
        {
            while (true)
            {
                Hero hero = this as Hero;
                // if (hero.IsLeader)
                // {
                //     Debug.Log("BREAK LEADER AI.");
                //     yield break;
                // }

                // 여기도 고쳐야할듯
                if (hero != null)
                {
                    //float divOffsetSQR = 5f * 5f;
                    // pointerCellPos : 중요하진않음. 그냥 이속조절을 위한 용도 뿐
                    // ***** Managers.Object.Camp.Pointer.position ---> Leader로 변경 예정 *****
                    //Vector3Int pointerCellPos = Managers.Map.WorldToCell(Managers.Object.Camp.Pointer.position);
                    // Vector3Int pointerCellPos = Managers.Map.WorldToCell(Managers.Object.LeaderController.Leader.transform.position);
                    //float ratio = Mathf.Max(1, (CellPos - pointerCellPos).sqrMagnitude / divOffsetSQR); // --> 로그로 변경 필요
                    //LerpToCellPos(MovementSpeed * ratio);
                    LerpToCellPos(MovementSpeed);
                }
                else
                    LerpToCellPos(MovementSpeed);

                yield return null;
            }
        }

        public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destPos, int maxDepth, bool forceMoveCloser = false)
        {
            // 움직임 진행중
            if (LerpToCellPosCompleted == false)
            {
                return EFindPathResult.Fail_LerpCell;
            }

            // A*
            List<Vector3Int> path = Managers.Map.FindPath(startCellPos: CellPos, destPos, maxDepth);
            if (path.Count < 2)
                return EFindPathResult.Fail_NoPath;

            #region TEST
            // 다른 오브젝트가 길막해서 와리가리할수있다는데, 그럴때 diff1, diff2의 점수 계산을 해서 안가게끔 막는 것이라고 함.
            // 근데 아주 예외적인 케이스라고함.
            // if (forceMoveCloser)
            // {
            //     Vector3Int diff1 = CellPos - destPos;
            //     Vector3Int diff2 = path[1] - destPos;
            //     if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
            //         return EFindPathResult.Fail_NoPath;
            // }
            #endregion

            Vector3Int dirCellPos = path[1] - CellPos;
            Vector3Int nextPos = CellPos + dirCellPos;

            if (Managers.Map.MoveTo(creature: this, nextPos) == false)
                return EFindPathResult.Fail_MoveTo;

            return EFindPathResult.Success;
        }
*/