using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Creature : BaseObject
    {
        #region Background
        public CreatureAI CreatureAI { get; protected set; } = null;
        public ECreatureRarity CreatureRarity { get; protected set; } = ECreatureRarity.None;
        public SkillComponent CreatureSkill { get; protected set; } = null; // Skills
        public EffectComponent CreatureEffect { get; protected set; } = null; // Effects
        public CreatureBody CreatureBody { get; protected set; } = null;
        public CreatureAnimation CreatureAnim { get; private set; } = null;
        public CreatureAnimationCallback CreatureAnimCallback { get; private set; } = null;

        // --- NO SETTER
        public virtual Vector3Int ChaseCellPos 
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
        [field: SerializeField] public bool ForceMove { get; set; } = false;

        public bool CanSkill
        {
            get
            {
                if (this.IsValid() == false)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                if (Target.IsValid() == false)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                if (CreatureAnim.CanEnterAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A) == false ||
                    CreatureAnim.CanEnterAnimState(ECreatureAnimState.Upper_Move_To_Skill_A) == false)
                {
                    return false;
                }

                SkillBase currentSkill = CreatureSkill.CurrentSkill;
                if (currentSkill.RemainCoolTime > 0f)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                // 가로 범위, 새로 범위, 대각선 범위로 바꿔야할지
                int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
                int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);
                int invokeRange = currentSkill.InvokeRange;
                if (Target.ObjectType == EObjectType.Monster || Target.ObjectType == EObjectType.Hero)
                {
                    if (dx <= invokeRange && dy <= invokeRange)
                    {
                        // --- For HeroAI, MonsterAI, 자기 자신이 아직 Cell 중앙까지 오지 않은 상황이라면 리턴
                        if (NextCenteredCellPos.HasValue)
                        {
                            if ((transform.position - NextCenteredCellPos.Value).sqrMagnitude > 0.01f)
                                return false;
                        }
                        // --- For Leader Hero (ex. 리더같은 경우에는 치킨이 중앙에 왔을 때 때려야함)
                        else
                        {
                            if ((Target.transform.position - Target.NextCenteredCellPos.Value).sqrMagnitude > 0.01f)
                                return false;
                        }                        

                        CreatureAnim.ReadySkill = true;
                        return true;
                    }
                }

                CreatureAnim.ReadySkill = false;
                return false;
            }
        }

        public virtual bool CanCollectEnv
        {
            get
            {
                if (this.IsValid() == false)
                    return false;

                if (Target.IsValid() && Target.ObjectType != EObjectType.Env)
                    return false;

                if (Moving || CreatureAnim.CanEnterAnimState(ECreatureAnimState.Upper_Idle_To_CollectEnv) == false)
                    return false;

                int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
                int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);
                if (dx <= 1 && dy <= 1)
                    return true;

                return true;
            }
        }

        [SerializeField] private ECreatureAIState _creatureAIState = ECreatureAIState.Idle;
        public virtual ECreatureAIState CreatureAIState
        {
            get => _creatureAIState;
            set
            {
                if (_creatureAIState != value)
                {
                    _creatureAIState = value;
                    if (value == ECreatureAIState.Dead)
                    {
                        Dead();
                        CreatureAI.OnDead();
                    }
                }
            }
        }

        // protected override void InitStat(int dataID)
        // {
        //     base.InitStat(dataID);
        //     for (int i = DataTemplateID; i < DataTemplateID + 10;)
        //     {
        //         if (Managers.Data.StatDataDict.ContainsKey(i) == false)
        //             break;

        //         _maxLevel = i++;
        //     }
        // }

        public bool IsValid(BaseObject bo) => bo.IsValid();

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
            // if (CreatureAIState == ECreatureAIState.Idle)
            //     return EFindPathResult.Fail_LerpCell;

            if (_coLerpToCellPos == null)
            {
                Managers.Map.RemoveObject(this);
                Managers.Map.AddObject(this, CellPos);
                return EFindPathResult.Fail_LerpCell;
            }

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
            NextCenteredCellPos = Managers.Map.CenteredCellToWorld(nextPos);

            if (Managers.Map.MoveTo(creature: this, cellPos: nextPos, ignoreObjectType: ignoreObjectType) == false)
                return EFindPathResult.Fail_MoveTo;

            return EFindPathResult.Success;
        }

        public virtual bool Moving
        {
            get => CreatureAnim.Moving;
            set => CreatureAnim.Moving = value;
        }

        public bool ReadySkill
        {
            get => CreatureAnim.ReadySkill;
            set => CreatureAnim.ReadySkill = value;
        }

        public void Skill(ESkillType skillType) => CreatureAnim.Skill(skillType);
        public void CollectEnv() => CreatureAnim.CollectEnv();
        public void Dead() => CreatureAnim.Dead();

        public bool IsInTheNearestTarget
        {
            get
            {
                if (Target.IsValid() == false)
                    return false;

                int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
                int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);
                if (Target.ObjectType == EObjectType.Monster || Target.ObjectType == EObjectType.Hero)
                {
                    float bestShortRange = float.MaxValue;
                    for (int i = 0; i < (int)ESkillType.Max; ++i)
                    {
                        SkillBase skill = CreatureSkill.SkillArray[i];
                        if (skill == null)
                            continue;

                        if (skill.InvokeRange < bestShortRange)
                            bestShortRange = skill.InvokeRange;
                    }

                    if (dx <= bestShortRange && dy <= bestShortRange)
                        return true;
                }
                else if (Target.ObjectType == EObjectType.Env)
                {
                    if (dx <= 1 && dy <= 1)
                        return true;
                }

                return false;
            }
        }

        public float TheShortestSkillInvokeRange
        {
            get
            {
                float bestShortRange = float.MaxValue;
                for (int i = 0; i < (int)ESkillType.Max; ++i)
                {
                    SkillBase skill = CreatureSkill.SkillArray[i];
                    if (skill == null)
                        continue;

                    if (skill.InvokeRange < bestShortRange)
                        bestShortRange = skill.InvokeRange;
                }

                return bestShortRange;
            }
        }
        #endregion

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            CreatureBody = GetComponent<CreatureBody>();
            CreatureAnim = BaseAnim as CreatureAnimation;
            CreatureAnimCallback = CreatureAnim.GetComponent<CreatureAnimationCallback>();
            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame();
                return false;
            }

            CreatureAnimCallback.SetInfo(this);
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            CreatureEffect = gameObject.GetOrAddComponent<EffectComponent>();
            CreatureEffect.SetInfo(this);
        }

        protected override void EnterInGame()
        {
            base.EnterInGame();
            RigidBody.simulated = false;
            Target = null;

            ShowBody(false);
            CreatureAI.EnterInGame();
            StartCoWait(waitCondition: () => BaseAnim.IsPlay() == false,
                      callbackWaitCompleted: () =>
                      {
                          ShowBody(true);
                          RigidBody.simulated = true;
                          Target = null;
                #region Events
                          CreatureAnim.AddAnimClipEvents();
                          CreatureAnim.AddAnimStateEvents();
                #endregion
                          //StartCoroutine(CoUpdateAI());
                          StartCoUpdateAI();
                          StartCoLerpToCellPos();
                      });
        }

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

            // if (skillFromAttacker.SkillData.EffectIDs != null)
            //     CreatureEffect.GenerateEffect(skillFromAttacker.SkillData.EffectIDs, EEffectSpawnType.Skill);
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            StopCoLerpToCellPos(); // 길찾기 움직임 중이었다면 멈춘다.
            CreatureAIState = ECreatureAIState.Dead;
            base.OnDead(attacker, skillFromAttacker);
        }

        protected override void OnDisable() { } // --- TEMP
        #endregion

        #region Coroutines
        protected Coroutine _coUpdateAI = null;
        #endregion
        protected IEnumerator CoUpdateAI()
        {
            while (true)
            {
                switch (CreatureAIState)
                {
                    case ECreatureAIState.Idle:
                        CreatureAI.UpdateIdle();
                        break;

                    case ECreatureAIState.Move:
                        CreatureAI.UpdateMove();
                        break;
                }

                //UpdateCellPos();
                yield return null;
            }
        }

        public void StartCoUpdateAI()
        {
            StopCoUpdateAI();
            _coUpdateAI = StartCoroutine(CoUpdateAI());
        }

        public void StopCoUpdateAI()
        {
            if (_coUpdateAI != null)
                StopCoroutine(_coUpdateAI);
            
            _coUpdateAI = null;
        }

        protected Coroutine _coWait = null;
        private IEnumerator CoWait(System.Func<bool> waitCondition, System.Action waitCompleted = null)
        {
            yield return new WaitUntil(waitCondition);
            _coWait = null;
            waitCompleted?.Invoke();
        }
        private IEnumerator CoWait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _coWait = null;
        }

        protected void StartCoWait(System.Func<bool> waitCondition, System.Action callbackWaitCompleted = null)
        {
            StopCoWait();
            _coWait = StartCoroutine(CoWait(waitCondition, callbackWaitCompleted));
        }
        protected void StartCoWait(float seconds)
        {
            StopCoWait();
            _coWait = StartCoroutine(CoWait(seconds));
        }

        protected void StopCoWait()
        {
            if (_coWait != null)
                StopCoroutine(_coWait);
            _coWait = null;
        }

        // --- In Creature.cs temporary
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
                        maxDistanceSQR: ReadOnly.Util.HeroDefaultScanRange * ReadOnly.Util.HeroDefaultScanRange
                    );

                    LerpToCellPos(movementSpeed);
                }
                else //--- Monster
                {
                    LerpToCellPos(MovementSpeed);
                }

                yield return null;
            }
        }

        public void StartCoLerpToCellPos()
        {
            StopCoLerpToCellPos();
            _coLerpToCellPos = StartCoroutine(CoLerpToCellPos());
        }

        public void StopCoLerpToCellPos()
        {
            if (_coLerpToCellPos != null)
                StopCoroutine(_coLerpToCellPos);
            _coLerpToCellPos = null;
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
        [ PREV REF ]
        public virtual bool IsMoving
        {
            get => CreatureAnim.IsMoving;
            set => CreatureAnim.IsMoving = value;
        }

        public bool IsValidTargetInAttackRange()
        {
            if (Target.IsValid() == false)
            {
                CreatureAnim.IsInAttackRange = false;
                return false;
            }

            int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
            int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);

            if (Target.ObjectType == EObjectType.Monster || Target.ObjectType == EObjectType.Hero)
            {
                if (dx <= AtkRange && dy <= AtkRange)
                {
                    CreatureAnim.IsInAttackRange = true;
                    return true;
                }
            }
            else if (Target.ObjectType == EObjectType.Env)
            {
                if (dx <= 1 && dy <= 1)
                    return true;
            }

            CreatureAnim.IsInAttackRange = false;
            return false;
        }

        private T SearchClosestInRange<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null,
                                        System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            T firstTarget = null;
            T secondTarget = null;
            float bestDistSQR = float.MaxValue;
            float scanRangeSQR = scanRange * scanRange;

            // --- Set Hero Leader Scan Range (half)
            // --- 없어도 될 것 같은데
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
                firstTarget = obj;
            }

            // --- 일반적인 Searching 또는 AutoTarget이 켜져있을 때
            if (allTargetsCondition == null || allTargetsCondition.Invoke() == false)
            {
                if (firstTarget != null)
                    return firstTarget;
                else if (firstTarget == null && secondTargets != null)
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
                        secondTarget = obj;
                    }
                }

                return secondTarget;
            }
            // --- AutoTarget과 상관 없이 리더이고 ForceMove가 True일 때.
            else if (allTargetsCondition != null && allTargetsCondition.Invoke())
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
                    secondTarget = obj;
                }

                if (func?.Invoke(firstTarget) == false)
                    return secondTarget;
                else if (func?.Invoke(secondTarget) == false)
                    return firstTarget;
                else
                {
                    float fDistSQR = (firstTarget.CellPos - CellPos).sqrMagnitude;
                    float sDistSQR = (secondTarget.CellPos - CellPos).sqrMagnitude;
                    if (fDistSQR < sDistSQR)
                        return firstTarget;
                    else
                        return secondTarget;
                }

                // // 두 개 모두 있는지부터 가정
                // if (firstTarget != null && secondTarget != null)
                // {
                //     float fDistSQR = (firstTarget.CellPos - CellPos).sqrMagnitude;
                //     float sDistSQR = (secondTarget.CellPos - CellPos).sqrMagnitude;
                //     if (fDistSQR < sDistSQR)
                //         return firstTarget;

                //     return secondTarget;
                // }
                // else if (firstTarget != null)
                //     return firstTarget;
                // else
                //     return secondTarget; // null이 나올수도 있음
            }

            return null;
        }

        // --- Creature -> BaseObject
        //public bool PauseSearchTarget { get; protected set; } = false;

        private Coroutine _coSearchTarget = null;
        private IEnumerator CoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            float tick = ReadOnly.Numeric.SearchFindTargetTick;
            while (true)
            {
                if (this.IsValid() == false)
                    yield break;

                yield return new WaitForSeconds(tick);
                if (PauseSearchTarget)
                {
                    Target = null; // --- DEFENSE
                    yield return null;
                    continue;
                }

                // if (_coWaitSearchTarget != null) // 이것도 필요할지...
                // {
                //     yield return null;
                //     continue;
                // }
                Target = SearchClosestInRange(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition);

                // if (Target.IsValid() == false)
                // {
                //     Debug.Log("<color=red>!@#!@#!@#!@#!@#!@#!@#</color>");
                // }

                // CreatureAnim.IsTargetValid = Target != null ? true : false;
                // if (Target.IsValid())
                //     CreatureMoveState = ECreatureMoveState.MoveToTarget;
                // --- Target이 존재하지 않을 때, MoveToTarget 해제는 Creature AI에서 해결    
            }
        }

        protected void StartCoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            // if (_coSearchTarget != null)
            //     return;
            StopCoSearchTarget();
            _coSearchTarget = StartCoroutine(CoSearchTarget<T>(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition));
        }

        protected void StopCoSearchTarget()
        {
            Target = null;
            if (_coSearchTarget != null)
            {
                StopCoroutine(_coSearchTarget);
                _coSearchTarget = null;
            }
        }private T SearchClosestInRange<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null,
                                        System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            T firstTarget = null;
            T secondTarget = null;
            float bestDistSQR = float.MaxValue;
            float scanRangeSQR = scanRange * scanRange;

            // --- Set Hero Leader Scan Range (half)
            // --- 없어도 될 것 같은데
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
                firstTarget = obj;
            }

            // --- 일반적인 Searching 또는 AutoTarget이 켜져있을 때
            if (allTargetsCondition == null || allTargetsCondition.Invoke() == false)
            {
                if (firstTarget != null)
                    return firstTarget;
                else if (firstTarget == null && secondTargets != null)
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
                        secondTarget = obj;
                    }
                }

                return secondTarget;
            }
            // --- AutoTarget과 상관 없이 리더이고 ForceMove가 True일 때.
            else if (allTargetsCondition != null && allTargetsCondition.Invoke())
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
                    secondTarget = obj;
                }

                if (func?.Invoke(firstTarget) == false)
                    return secondTarget;
                else if (func?.Invoke(secondTarget) == false)
                    return firstTarget;
                else
                {
                    float fDistSQR = (firstTarget.CellPos - CellPos).sqrMagnitude;
                    float sDistSQR = (secondTarget.CellPos - CellPos).sqrMagnitude;
                    if (fDistSQR < sDistSQR)
                        return firstTarget;
                    else
                        return secondTarget;
                }

                // // 두 개 모두 있는지부터 가정
                // if (firstTarget != null && secondTarget != null)
                // {
                //     float fDistSQR = (firstTarget.CellPos - CellPos).sqrMagnitude;
                //     float sDistSQR = (secondTarget.CellPos - CellPos).sqrMagnitude;
                //     if (fDistSQR < sDistSQR)
                //         return firstTarget;

                //     return secondTarget;
                // }
                // else if (firstTarget != null)
                //     return firstTarget;
                // else
                //     return secondTarget; // null이 나올수도 있음
            }

            return null;
        }

        // --- Creature -> BaseObject
        //public bool PauseSearchTarget { get; protected set; } = false;

        private Coroutine _coSearchTarget = null;
        private IEnumerator CoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            float tick = ReadOnly.Numeric.SearchFindTargetTick;
            while (true)
            {
                if (this.IsValid() == false)
                    yield break;

                yield return new WaitForSeconds(tick);
                if (PauseSearchTarget)
                {
                    Target = null; // --- DEFENSE
                    yield return null;
                    continue;
                }

                // if (_coWaitSearchTarget != null) // 이것도 필요할지...
                // {
                //     yield return null;
                //     continue;
                // }
                Target = SearchClosestInRange(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition);

                // if (Target.IsValid() == false)
                // {
                //     Debug.Log("<color=red>!@#!@#!@#!@#!@#!@#!@#</color>");
                // }

                // CreatureAnim.IsTargetValid = Target != null ? true : false;
                // if (Target.IsValid())
                //     CreatureMoveState = ECreatureMoveState.MoveToTarget;
                // --- Target이 존재하지 않을 때, MoveToTarget 해제는 Creature AI에서 해결    
            }
        }

        protected void StartCoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            // if (_coSearchTarget != null)
            //     return;
            StopCoSearchTarget();
            _coSearchTarget = StartCoroutine(CoSearchTarget<T>(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition));
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

          // --- WILL DEPRECIATE
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

                // 제거예정
        public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
        {
            if (LerpToCellPosCompleted == false)
                return false;

            return Managers.Map.MoveTo(this, cellPos: destCellPos);
        }

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