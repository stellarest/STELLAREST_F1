using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.UI;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    /// <summary>
    /// Creatures(Hero, Monster, Summon), Env
    /// </summary>
    public class BaseCellObject : BaseObject
    {
        public BaseBody BaseBody { get; private set; } = null;
        public BaseAnimation BaseAnim { get; private set; } = null;
        public BaseStat BaseStat { get; private set; } = null; // 일단은 Env도 들고 있게. 헷갈림.
        public EffectComponent BaseEffect { get; private set; } = null;

        [SerializeField] private ELookAtDirection _lookAtDir = ELookAtDirection.Right;
        public virtual ELookAtDirection LookAtDir
        {
            get => _lookAtDir;
            set
            {
                if (_lookAtDir != value)
                {
                    _lookAtDir = value;
                    BaseAnim.Flip(value);
                }
            }
        }

        [field: SerializeField] public Vector3Int SpawnedCellPos { get; protected set; } = Vector3Int.zero;
        [field: SerializeField] public Vector3Int CellPos { get; protected set; } = Vector3Int.zero;
        public bool IsOnTheCellCenter
        {
            get
            {
                Vector3 center = Managers.Map.CellToCenteredWorld(CellPos);
                float threshold = 0.1f;
                if ((center - transform.position).sqrMagnitude < threshold * threshold)
                    return true;

                return false;
            }
        }
        public virtual Vector3Int ChaseCellPos
            => Target.IsValid() ? Target.CellPos : CellPos;

        [field: SerializeField] public Vector3Int NextCellPos { get; protected set; } = Vector3Int.one;
        [field: SerializeField] public bool LerpToCellPosCompleted { get; protected set; } = false;

        // --- TEMP
        // [field: SerializeField] public List<BaseObject> Allies { get; set; } = new List<BaseObject>();
        [field: SerializeField] public List<BaseCellObject> Targets { get; set; } = new List<BaseCellObject>();
        public virtual BaseCellObject Target
        {
            get
            {
                if (Targets.Count > 0)
                {
                    if (Targets[0].IsValid())
                        return Targets[0];
                }

                return null;
            }
        }

        /*  [Stat Data]

            public float MaxHealth;
            public float MinAttack;
            public float MaxAttack;
            public float CriticalRate;
            public float DamageReductionRate;
	        public float DodgeRate;
            public float MovementSpeed;


	        public float Luck;                  
	        public int InvincibleCount;
        */

        // ***** Stat Prev *****
        // --- Stat: Level
        // [SerializeField] protected int _levelID = 0;
        // public int Level => (_levelID % DataTemplateID) + 1;

        // [SerializeField] protected int _maxLevelID = 0;
        // public int MaxLevel => (_maxLevelID % DataTemplateID) + 1;
        // protected bool IsMaxLevel => _levelID == _maxLevelID;

        // // --- Stat
        // [SerializeField] private float _health = 0.0f;
        // public float Health
        // {
        //     get => _health;
        //     protected set => _health = value;
        // }

        // [field: SerializeField] public float BonusHealth { get; set; } = 0.0f;

        // [field: SerializeField] public float MaxHealth { get; set; } = 0.0f;
        // public float MaxHealthBase { get; protected set; } = 0.0f;


        // public float MinAttackBase { get; protected set; } = 0.0f;
        // public float MaxAttackBase { get; protected set; } = 0.0f;
        // public float MovementSpeedRateBase { get; protected set; } = 0.0f;
        
        // public float MaxHpBase { get; set; } = 0.0f;
        // public float MinAtkBase { get; set; } = 0.0f;
        // public float MaxAtkBase { get; set; } = 0.0f;
        // public float CriticalRateBase { get; set; } = 0.0f;
        // public float DodgeRateBase { get; set; } = 0.0f;
        // public float MovementSpeedBase { get; set; } = 0.0f;

        // [field: SerializeField] public float MaxHp { get; set; } = 0.0f;
        // [field: SerializeField] public float MinAtk { get; set; } = 0.0f;
        // [field: SerializeField] public float MaxAtk { get; set; } = 0.0f;
        // [field: SerializeField] public float CriticalRate { get; set; } = 0.0f;
        // [field: SerializeField] public float DodgeRate { get; set; } = 0.0f;
        // [field: SerializeField] public float MovementSpeed { get; set; } = 0.0f;


        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BaseBody = gameObject.GetOrAddComponent<BaseBody>();
            BaseAnim = Util.FindChild<BaseAnimation>(gameObject, name: ReadOnly.Util.AnimationBody, recursive: false);
            BaseStat = gameObject.GetOrAddComponent<BaseStat>();
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            BaseBody.InitialSetInfo(dataID, this);
            BaseAnim.InitialSetInfo(dataID, this);
            BaseStat.InitialSetInfo(dataID, this);            

            BaseEffect = gameObject.GetOrAddComponent<EffectComponent>();
            BaseEffect.InitialSetInfo(this);
            // _levelID = dataID;
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            base.EnterInGame(spawnPos);
            Targets.Clear();

            //SetStat(_levelID);
            BaseStat.SetBaseStat();

            BaseBody.ResetMaterialsAndColors();
            BaseBody.StartCoFadeInEffect();
            SpawnedCellPos = Managers.Map.WorldToCell(spawnPos);
            Managers.Map.ForceMove(cellObj: this, cellPos: SpawnedCellPos, ignoreCellObjType: EObjectType.None);
        }

        public virtual void OnDamaged(BaseCellObject attacker, SkillBase skillByAttacker) { }

        //public virtual void OnDamaged(BaseCellObject attacker, SkillBase skillByAttacker) { }
        public virtual void OnDead(BaseCellObject attacker, SkillBase skillFromAttacker)
        {
            RigidBody.simulated = false;
            // BaseBody.StartCoFadeOutEffect(() => OnDeadFadeOutCompleted());
        }
        #endregion
        
        #region Background
        public virtual void ApplyStat()
        {
            BonusHealth = ApplyFinalStat(baseValue: MaxHealthBase, applyStatType: EApplyStatType.BonusHealth);
            //DamageReductinoRate = ApplyFinalStat(baseValue: DamageReductinoRate,)

            // MaxHpBase
            // AtkBase
            // ...
            // MovementSpeedBase
            float prevMaxHealth = MaxHealth;

            // ... Apply MaxHp = MaxHpBase,,,
            if (prevMaxHealth != MaxHealthBase)
            {
                // 현재의 hp를 증가된 MaxHp만큼의 비율로 조정한다.
                Health = MaxHealth * (Health / prevMaxHealth);

                // Final Min, Max Check
                Health = Mathf.Clamp(value: Health, min: 0.0f, max: MaxHealth);
            }

            float ratio = Health / MaxHealth;
            // HpBar.Refresh(ratio); -- LATER TODO
        }

        protected virtual float ApplyFinalStat(float baseValue, EApplyStatType applyStatType)
            => baseValue;

        public EFindPathResult FindPathAndMoveToCellPos(Vector3 destPos, int maxDepth, EObjectType ignoreCellObjType = EObjectType.None)
            => FindPathAndMoveToCellPos(Managers.Map.WorldToCell(destPos), maxDepth, ignoreCellObjType);

        public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destPos, int maxDepth, EObjectType ignoreCellObjType = EObjectType.None)
        {
            if (IsForceMovingPingPongObject)
                return EFindPathResult.Fail_ForceMove;

            // ---A*
            List<Vector3Int> path = Managers.Map.FindPath(startCellPos: CellPos, destPos, maxDepth, ignoreCellObjType);
            if (path.Count == 1)
            {
                if (IsOnTheCellCenter == false)
                {
                    NextCellPos = path[0];
                    LerpToCellPosCompleted = false;
                    return EFindPathResult.Success;
                }
                else if (LerpToCellPosCompleted) // --- 무조건 가운데까지 간다.
                {
                    return EFindPathResult.Fail_LerpCell;
                }
            }

            else if (path.Count > 1)
            {
                Vector3Int dirCellPos = path[1] - CellPos;
                Vector3Int nextCellPos = CellPos + dirCellPos;

                if (Managers.Map.TryMove(moveToCellPos: nextCellPos, ignoreCellObjType: ignoreCellObjType) == false)
                    return EFindPathResult.Fail_MoveTo;

                NextCellPos = nextCellPos;
                LerpToCellPosCompleted = false;
            }

            return EFindPathResult.Success;
        }

        public void MoveToCellCenter()
        {
            Vector3 center = Managers.Map.CellToCenteredWorld(CellPos);
            Vector3 dir = center - transform.position;

            float threshold = 0.1f;
            if (dir.sqrMagnitude < threshold * threshold)
            {
                LerpToCellPosCompleted = true;
                transform.position = center;
                // Moving = false; --- 안먹히는듯
            }
            else if (Target.IsValid())
                LookAtValidTarget();
            else if (dir.x < 0f)
                LookAtDir = ELookAtDirection.Left;
            else if (dir.x > 0f)
                LookAtDir = ELookAtDirection.Right;

            transform.position += dir.normalized * MovementSpeed * Time.deltaTime;
            LerpToCellPosCompleted = false;
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
                Vector3 dir = destPos - transform.position;
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

        protected Coroutine _coLerpToCellPos = null;
        protected IEnumerator CoLerpToCellPos()
        {
            while (true)
            {
                if (IsForceMovingPingPongObject /* || CreatureAIState == ECreatureAIState.Idle */ )
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

        public void SetCellPos(Vector3 worldPos) 
            => SetCellPos(Managers.Map.WorldToCell(worldPos));
        public void SetCellPos(Vector3Int cellPos)
        {
            Managers.Map.RemoveCellObject(this);
            Managers.Map.Cells[cellPos] = this;
            CellPos = cellPos;
            NextCellPos = cellPos;
            transform.position = Managers.Map.CellToCenteredWorld(cellPos);
            LerpToCellPosCompleted = true;
        }

        public void UpdateCellPos()
        {
            Vector3Int currentCellPos = Managers.Map.WorldToCell(transform.position);
            if (Managers.Map.AddCellObject(this, currentCellPos))
                CellPos = currentCellPos;
        }

        public virtual void LerpToCellPos(float movementSpeed)
        {
            if (LerpToCellPosCompleted)
                return;

            Vector3 destPos = Managers.Map.CellToCenteredWorld(NextCellPos);
            Vector3 dir = destPos - transform.position;
            if (dir.x < 0f)
                LookAtDir = ELookAtDirection.Left;
            else if (dir.x > 0f)
                LookAtDir = ELookAtDirection.Right;

            float threshold = 0.1f;
            if (dir.sqrMagnitude < threshold * threshold)
            {
                LerpToCellPosCompleted = true;
                transform.position = destPos;
                return;
            }

            float moveDist = Mathf.Min(dir.magnitude, movementSpeed * Time.deltaTime);
            transform.position += dir.normalized * moveDist;
        }

        public void LookAtValidTarget()
        {
            if (Target.IsValid() == false)
                return;

            Vector3 toTargetDir = Target.transform.position - transform.position;
            if (toTargetDir.x < 0)
                LookAtDir = ELookAtDirection.Left;
            else
                LookAtDir = ELookAtDirection.Right;
        }

        // --- PREV
        // protected void SetStat(int levelID)
        // {
        //     StatData statData = null;
        //     switch (ObjectType)
        //     {
        //         case EObjectType.Hero:
        //             statData = Managers.Data.HeroStatDataDict[levelID];
        //             break;

        //         case EObjectType.Monster:
        //             statData = Managers.Data.MonsterStatDataDict[levelID];
        //             break;

        //         case EObjectType.Env:
        //             {
        //                 EnvData envData = Managers.Data.EnvDataDict[levelID];
                        
        //                 Health = envData.MaxHealth;
        //                 MaxHp = MaxHpBase = envData.MaxHealth;
        //                 return;
        //             }
        //     }

        //     Health = statData.MaxHealth;
        //     MaxHp = MaxHpBase = statData.MaxHealth;
        //     MinAtk = MinAtkBase = statData.MinAttack;
        //     MaxAtk = MaxAtkBase = statData.MaxAttack;
        //     CriticalRate = CriticalRateBase = statData.CriticalRate;
        //     DodgeRate = DodgeRateBase = statData.DodgeRate;
        //     MovementSpeed = MovementSpeedBase = statData.MovementSpeed;
        // }
        #endregion

        #region Stat Util
        // --- Main Stat
        public float Health { get => BaseStat.Health; set => BaseStat.Health = value; }
        public float MaxHealth { get => BaseStat.MaxHealth; set => BaseStat.MaxHealth = value; }
        public float MaxHealthBase => BaseStat.MaxHealthBase;

        public float MinAttack { get => BaseStat.MinAttack; set => BaseStat.MinAttack = value; }
        public float MinAttackBase => BaseStat.MinAttackBase;

        public float MaxAttack { get => BaseStat.MaxAttack; set => BaseStat.MaxAttack = value; }
        public float MaxAttackBase => BaseStat.MaxAttackBase;

        public float CriticalRate { get => BaseStat.CriticalRate; set => BaseStat.CriticalRate = value; }
        public float CriticalRateBase => BaseStat.CriticalRateBase;

        public float DodgeRate { get => BaseStat.DodgeRate; set => BaseStat.DodgeRate = value; }
        public float DodgeRateBase => BaseStat.DodgeRateBase;

        public float MovementSpeed { get => BaseStat.MovementSpeed; set => BaseStat.MovementSpeed = value; }
        public float MovementSpeedBase => BaseStat.MovementSpeedBase;

        public float Luck { get => BaseStat.Luck; set => BaseStat.Luck = value; }
        public float LuckBase => BaseStat.LuckBase;

        // --- Sub Stat
        public float BonusHealth { get => BaseStat.BonusHealth; set => BaseStat.BonusHealth = value; }
        public float FixedBonusAttackAmount { get => BaseStat.FixedBonusAttackAmount; set => BaseStat.FixedBonusAttackAmount = value; }
        public float DamageReductinoRate { get => BaseStat.DamageReductinoRate; set => BaseStat.DamageReductinoRate = value; }
        public float DebuffResistanceRate { get => BaseStat.DebuffResistanceRate; set => BaseStat.DebuffResistanceRate = value; }
        public int InvincibleCountPerWave { get => BaseStat.InvincibleCountPerWave; set => BaseStat.InvincibleCountPerWave = value; }

        // --- Level
        public int Level => BaseStat.Level;
        public int MaxLevel => BaseStat.MaxLevel;
        public bool IsMaxLevel => BaseStat.IsMaxLevel;
        #endregion
    }
}
