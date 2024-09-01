using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Creature : BaseObject // ---> Abstract가 나을 것 같긴해.
    {
        public Data.CreatureData CreatureData { get; private set; } = null;
        public CreatureAI CreatureAI { get; protected set; } = null;
        [field: SerializeField] public ECreatureRarity CreatureRarity { get; protected set; } = ECreatureRarity.None;
        public SkillComponent CreatureSkill { get; protected set; } = null; // Skills
        //public EffectComponent CreatureEffect { get; protected set; } = null; // Effects --> Move To BaseEffect
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

        public bool CanSkill
        {
            get
            {
                if (this.IsValid() == false)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                if (CanEnterSkillAState == false)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                if (CanEnterSkillBState == false)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                if (Target.IsValid() == false)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                // --- 스킬이 2개 이상일 경우, 
                // --- CanSkill에서 B가 선택이 되었는데
                // --- DO SKILL에서 C가 선택이 될 위험이 있다.
                SkillBase currentSkill = CreatureSkill.CurrentSkill;
                if (currentSkill == null || currentSkill.RemainCoolTime > 0f)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                Vector3Int worldToCell = Managers.Map.WorldToCell(transform.position);
                Vector3Int targetWorldToCell = Managers.Map.WorldToCell(Target.transform.position);
                int dx = Mathf.Abs(worldToCell.x - targetWorldToCell.x);
                int dy = Mathf.Abs(worldToCell.y - targetWorldToCell.y);
                int invokeRange = currentSkill.InvokeRange;
                if (Target.ObjectType != EObjectType.Env)
                {
                    if (dx <= invokeRange && dy <= invokeRange)
                    {
                        CreatureAnim.ReadySkill = true;
                        return true;
                    }
                }

                CreatureAnim.ReadySkill = false;
                return false;
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

        public bool IsValid(BaseObject bo) => bo.IsValid();

        public EFindPathResult FindPathAndMoveToCellPos(Vector3 destPos, int maxDepth, EObjectType ignoreObjectType = EObjectType.None)
        {
            Vector3Int destCellPos = Managers.Map.WorldToCell(destPos);
            return FindPathAndMoveToCellPos(destCellPos, maxDepth, ignoreObjectType);
        }

        // public bool IsAtCenter(Vector3Int destPos)
        // {
        //     Vector3 center = Managers.Map.GetCenterWorld(destPos);
        //     Vector3 dir = center - transform.position;

        //     float threshold = 0.1f;
        //     if (dir.sqrMagnitude < threshold * threshold)
        //     {
        //         transform.position = center;
        //         return true;
        //     }

        //     return false;
        // }

        public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destPos, int maxDepth, EObjectType ignoreObjectType = EObjectType.None)
        {
            if (IsForceMovingPingPongObject)
                return EFindPathResult.Fail_ForceMove;

            // 움직임 진행중..
            // if (LerpToCellPosCompleted == false)
            //     return EFindPathResult.Success;

            // if (CreatureAIState == ECreatureAIState.Idle)
            //     return EFindPathResult.Fail_LerpCell;

            // if (_coLerpToCellPos == null)
            // {
            //     // Managers.Map.RemoveObject(this);
            //     // Managers.Map.AddObject(this, CellPos);
            //     return EFindPathResult.Fail_LerpCell;
            // }

            // // 움직임 진행중
            // if (LerpToCellPosCompleted == false)
            // {
            //     return EFindPathResult.Fail_LerpCell;
            // }

            // A*
            List<Vector3Int> path = Managers.Map.FindPath(startCellPos: CellPos, destPos, maxDepth, ignoreObjectType);
            if (path.Count == 1)
            {
                if (IsOnTheCellCenter == false)
                {
                    NextCellPos = path[0];
                    LerpToCellPosCompleted = false;
                    return EFindPathResult.Success;
                }
                else if (LerpToCellPosCompleted) // --- 무조건 가운데까지 간다.
                    return EFindPathResult.Fail_LerpCell;
            }

            else if (path.Count > 1)
            {
                Vector3Int dirCellPos = path[1] - CellPos;
                Vector3Int nextPos = CellPos + dirCellPos;

                if (Managers.Map.TryMove(creature: this, cellPos: nextPos, ignoreObjectType: ignoreObjectType) == false)
                    return EFindPathResult.Fail_MoveTo;

                NextCellPos = nextPos;
                LerpToCellPosCompleted = false;
            }

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
        public void Dead()
        {
            // CreatureAnim.ReleaseAllAnimStates();
            CreatureAnim.Dead();
        }

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

        public virtual Vector3 GetFirePosition() => CenterPosition;

        public void MoveToCellCenter()
        {
            Vector3 center = Managers.Map.CellToCenteredWorld(CellPos);
            Vector3 dir = center - transform.position;

            float threshold = 0.1f;
            if (dir.sqrMagnitude < threshold * threshold)
            {
                LerpToCellPosCompleted = true;
                transform.position = center;
                Moving = false;
            }
            else if (Target.IsValid())
                LookAtValidTarget();
            else if (dir.x < 0f)
                LookAtDir = ELookAtDirection.Left;
            else if (dir.x > 0f)
                LookAtDir = ELookAtDirection.Right;

            transform.position += dir.normalized * MovementSpeed * Time.deltaTime;
            LerpToCellPosCompleted = false;

            // --- CellPos로 가도록 변경해야함. 어차피 CellPos는 실시간으로 업데이트중.
            // Vector3Int nearCell = Managers.Map.WorldToCell(transform.position);
            // Vector3 center = Managers.Map.GetCenterWorld(nearCell);
            // Vector3 dir = center - transform.position;

            // float threshold = 0.1f;
            // if (dir.sqrMagnitude < threshold * threshold)
            // {
            //     transform.position = center;
            //     Moving = false;
            //     LerpToCellPosCompleted = true;
            // }
            // else
            // {
            //     if (dir.x < 0f)
            //         LookAtDir = ELookAtDirection.Left;
            //     else if (dir.x > 0f)
            //         LookAtDir = ELookAtDirection.Right;

            //     transform.position += dir.normalized * MovementSpeed * Time.deltaTime;
            //     LerpToCellPosCompleted = false;
            // }
        }

        private bool CanEnterSkillAState
            => CreatureAnim.CanEnterAnimState(ECreatureAnimState.Upper_SkillA);
        private bool CanEnterSkillBState
            => CreatureAnim.CanEnterAnimState(ECreatureAnimState.Upper_SkillB);
        public void CancelPlayAnimations() 
            => CreatureAnim.CancelPlayAnimations();

        #region Init Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            CreatureBody = BaseBody as CreatureBody;
            CreatureAnim = BaseAnim as CreatureAnimation;
            CreatureAnimCallback = CreatureAnim.GetComponent<CreatureAnimationCallback>();
            
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            if (ObjectType == EObjectType.Hero)
                CreatureData = Managers.Data.HeroDataDict[dataID];
            else if (ObjectType == EObjectType.Monster)
                CreatureData = Managers.Data.MonsterDataDict[dataID];

            CreatureRarity = CreatureData.CreatureRarity;
            Type aiClassType = Util.GetTypeFromName(CreatureData.AIClassName);
            CreatureAI = gameObject.AddComponent(aiClassType) as CreatureAI;
            CreatureAI.InitialSetInfo(this);

            Collider.radius = CreatureData.ColliderRadius;
            Collider.offset = CreatureData.ColliderOffset;
            
            CreatureSkill = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkill.InitialSetInfo(owner: this, creatureData: CreatureData);
            CreatureAnimCallback.InitialSetInfo(this);

            // --> Change to BaseEffect
            // CreatureEffect = gameObject.GetOrAddComponent<EffectComponent>();
            // CreatureEffect.InitialSetInfo(this);
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            base.EnterInGame(spawnPos);
            RigidBody.simulated = false;
            Targets.Clear();
            StartCoWait(waitCondition: () => BaseAnim.IsPlay() == false,
                      callbackWaitCompleted: () =>
                      {
                          //CreatureBody.ResetMaterialsAndColors();
                          CreatureAI.EnterInGame();
                          //CreatureAnim.AddAnimClipEvents();
                          //CreatureAnim.AddAnimStateEvents();
                          CreatureAnim.EnterInGame();
                          StartCoUpdateAI();
                          StartCoLerpToCellPos();
                          RigidBody.simulated = true;
                      });
        }
        #endregion Init Core

        private void LateUpdate()
            => UpdateCellPos();

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            if (CreatureAIState == ECreatureAIState.Dead)
                return;

            base.OnDamaged(attacker, skillFromAttacker);
            // if (skillFromAttacker.SkillData.EffectIDs != null)
            //     CreatureEffect.GenerateEffect(skillFromAttacker.SkillData.EffectIDs, EEffectSpawnType.Skill, this);
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            Moving = false;
            StopCoLerpToCellPos(); // --- Stop Path Finding
            StopCoUpdateAI(); // --- Stop AI Tick
            CreatureAIState = ECreatureAIState.Dead;
            attacker.Targets.Remove(this);
            base.OnDead(attacker, skillFromAttacker);
            CreatureBody.StartCoFadeOutEffect(
                startCallback: () => 
                {
                    Managers.Object.SpawnBaseObject<EffectBase>(
                        objectType: EObjectType.Effect,
                        spawnPos: CenterPosition,
                        dataID: ReadOnly.DataAndPoolingID.DNPID_Effect_OnDeadSkull
                    );
                },
                endCallback: () => OnDeadFadeOutCompleted()
            );
        }

        protected override void OnDisable() { } // --- TEMP

        public bool IsRunningAITick => _coUpdateAI != null;
        protected Coroutine _coUpdateAI = null;
        protected IEnumerator CoUpdateAI()
        {
            if (ObjectType == EObjectType.Monster)
                yield break;

            // while (true)
            // {
            //     if (CreatureAI.ForceWaitCompleted == false)
            //     {
            //         yield return null;
            //         continue;
            //     }

            //     switch (CreatureAIState)
            //     {
            //         case ECreatureAIState.Idle:
            //             CreatureAI.UpdateIdle();
            //             break;

            //         case ECreatureAIState.Move:
            //             CreatureAI.UpdateMove();
            //             break;
            //     }

            //     yield return null;
            // }
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
        private IEnumerator CoWait(Func<bool> waitCondition, Action waitCompleted = null)
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

        protected void StartCoWait(Func<bool> waitCondition, Action callbackWaitCompleted = null)
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
                if (IsForceMovingPingPongObject || CreatureAIState == ECreatureAIState.Idle)
                {
                    //LerpToCellPosCompleted = true;
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
                Vector3 destPos = Managers.Map.CellToCenteredWorld(nextPos);
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
            // UpdateCellPos();
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
