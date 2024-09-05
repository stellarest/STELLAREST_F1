using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.UI;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    /// <summary>
    /// Creatures(Hero, Monster), Env
    /// </summary>
    public class BaseCellObject : BaseObject
    {
        public BaseBody BaseBody { get; private set; } = null;
        public BaseAnimation BaseAnim { get; private set; } = null;

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

        [field: SerializeField] public Vector3Int NextCellPos { get; set; } = Vector3Int.one;
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

        // --- Stat
        [SerializeField] protected int _levelID = 0;
        public int Level => (_levelID % DataTemplateID) + 1;

        [SerializeField] protected int _maxLevelID = 0;
        public int MaxLevel => (_maxLevelID % DataTemplateID) + 1;
        protected bool IsMaxLevel => _levelID == _maxLevelID;

        [SerializeField] private float _hp = 0f;
        public float Hp
        {
            get => _hp;
            protected set => _hp = value;
        }
        public float MaxHpBase { get; set; } = 0f;
        public float MinAtkBase { get; set; } = 0f;
        public float MaxAtkBase { get; set; } = 0f;
        public float CriticalRateBase { get; set; } = 0f;
        public float DodgeRateBase { get; set; } = 0f;
        public float MovementSpeedBase { get; set; } = 0f;

        [field: SerializeField] public float MaxHp { get; set; } = 0f;
        [field: SerializeField] public float MinAtk { get; set; } = 0f;
        [field: SerializeField] public float MaxAtk { get; set; } = 0f;
        [field: SerializeField] public float CriticalRate { get; set; } = 0f;
        [field: SerializeField] public float DodgeRate { get; set; } = 0f;
        [field: SerializeField] public float MovementSpeed { get; set; } = 0f;

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BaseBody = gameObject.GetOrAddComponent<BaseBody>();
            BaseAnim = Util.FindChild<BaseAnimation>(gameObject, name: ReadOnly.Util.AnimationBody, recursive: false);
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            BaseBody.InitialSetInfo(dataID, this);
            BaseAnim.InitialSetInfo(dataID, this);
            _levelID = dataID;
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            base.EnterInGame(spawnPos);
            Targets.Clear();
            SetStat(_levelID);
            BaseBody.ResetMaterialsAndColors();
            BaseBody.StartCoFadeInEffect();
            SpawnedCellPos = Managers.Map.WorldToCell(spawnPos);
            Managers.Map.ForceMove(cellObj: this, cellPos: SpawnedCellPos, ignoreCellObjType: EObjectType.None);
        }

        public virtual void OnDamaged(BaseCellObject attacker, SkillBase skillByAttacker) { }
        public virtual void OnDead(BaseCellObject attacker, SkillBase skillFromAttacker)
        {
            RigidBody.simulated = false;
            // BaseBody.StartCoFadeOutEffect(() => OnDeadFadeOutCompleted());
        }
        #endregion

        #region Background
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

        protected void SetStat(int levelID)
        {
            StatData statData = null;
            switch (ObjectType)
            {
                case EObjectType.Hero:
                    statData = Managers.Data.HeroStatDataDict[levelID];
                    break;

                case EObjectType.Monster:
                    statData = Managers.Data.MonsterStatDataDict[levelID];
                    break;

                case EObjectType.Env:
                    {
                        EnvData envData = Managers.Data.EnvDataDict[levelID];
                        Hp = envData.MaxHp;
                        MaxHp = MaxHpBase = envData.MaxHp;
                        return;
                    }
            }

            Hp = statData.MaxHp;
            MaxHp = MaxHpBase = statData.MaxHp;
            MinAtk = MinAtkBase = statData.MinAtk;
            MaxAtk = MaxAtkBase = statData.MaxAtk;
            CriticalRate = CriticalRateBase = statData.CriticalRate;
            DodgeRate = DodgeRateBase = statData.DodgeRate;
            MovementSpeed = MovementSpeedBase = statData.MovementSpeed;
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
    }
    #endregion
}
