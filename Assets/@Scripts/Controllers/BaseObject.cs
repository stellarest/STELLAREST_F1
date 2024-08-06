using System;
using System.Collections;
using System.Linq;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using static STELLAREST_F1.Define;
using System.Collections.Generic;

namespace STELLAREST_F1
{
    public class BaseObject : InitBase
    {
        public int DataTemplateID { get; protected set; } = -1;
        public EObjectType ObjectType { get; protected set; } = EObjectType.None;
        public BaseBody BaseBody { get; protected set; } = null;
        public BaseAnimation BaseAnim { get; private set; } = null;
        public CircleCollider2D Collider { get; private set; } = null;
        public Rigidbody2D RigidBody { get; private set; } = null;
        public SortingGroup SortingGroup { get; private set; } = null;
        //public AnimationClipCallback AnimCallback { get; private set; } = null;

        public EffectComponent BaseEffect { get; private set; } = null;
        public float ColliderRadius { get => Collider != null ? Collider.radius : 0.0f; }
        public Vector3 CenterPosition { get => transform.position + Vector3.up * ColliderRadius; } // Creature 및 다른 오브젝트의 경우, 발바닥 부분이 피벗임
        public Vector3 CenterLocalPosition => Vector3.up * ColliderRadius;

        [SerializeField] private ELookAtDirection _lookAtDir = ELookAtDirection.Right;
        public ELookAtDirection LookAtDir
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

        public StatData StatData { get; private set; } = null;

        [SerializeField] protected int _level = 0; // ---> _levelID로 변경해야함
        public int Level => (_level % DataTemplateID) + 1;

        [SerializeField] protected int _maxLevel = 0; // ---> _maxLevelID로 변경해야함
        public int MaxLevel => (_maxLevel % DataTemplateID) + 1;
        protected bool IsMaxLevel => _level == _maxLevel;

        protected void SetStat(StatData statData)
        {
            StatData = statData; // Refresh
            MaxHp = statData.MaxHp;
            Hp = statData.MaxHp;
            MinAtk = MinAtkBase = statData.MinAtk;
            CriticalRate = CriticalRateBase = statData.CriticalRate;
            DodgeRate = DodgeRateBase = statData.DodgeRate;
            MovementSpeed = MovementSpeedBase = statData.MovementSpeed;
        }

        [SerializeField] private float _hp = 0f;
        public float Hp
        {
            get => _hp;
            protected set => _hp = value;
        }

        public float MaxHpBase { get; set; } = 0f;
        public float MinAtkBase { get; set; } = 0f;
        public float MaxAtkBase { get; set; } = 0f;
        public int AtkRangeBase { get; set; } = 0;
        public float CriticalRateBase { get; set; } = 0f;
        public float DodgeRateBase { get; set; } = 0f;
        public float MovementSpeedBase { get; set; } = 0f;

        [field: SerializeField] public float MaxHp { get; set; } = 0f;
        [field: SerializeField] public float MinAtk { get; set; } = 0f;
        [field: SerializeField] public float MaxAtk { get; set; } = 0f;
        [field: SerializeField] public int AtkRange { get; set; } = 0;
        [field: SerializeField] public float CriticalRate { get; set; } = 0f;
        [field: SerializeField] public float DodgeRate { get; set; } = 0f;
        [field: SerializeField] public float MovementSpeed { get; set; } = 0f;

        [field: SerializeField] public List<BaseObject> Targets { get; set; } = new List<BaseObject>();
        public virtual BaseObject Target
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

        // public bool IsAtCenter(float threshold = 0.1f)
        // {
        //     Vector3 center = Managers.Map.GetCenterWorld(CellPos);
        //     Vector3 dir = center - transform.position;

        //     if (dir.sqrMagnitude < threshold * threshold)
        //     {
        //         // transform.position = center;
        //         return true;
        //     }

        //     return false;
        // }

        public bool IsAtCenter
        {
            get
            {
                Vector3 center = Managers.Map.GetCenterWorld(CellPos);
                float threshold = 0.1f;
                if ((center - transform.position).sqrMagnitude < threshold * threshold)
                    return true;

                return false;
            }
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

        public static Vector3 GetLookAtRotation(Vector3 dir)
            => new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg);

        protected virtual void OnDeadFadeOutCompleted()
        {
            Managers.Object.Despawn(this, DataTemplateID);
            StopAllCoroutines(); // --- DEFENSE
        }

        [field: SerializeField] public bool LerpToCellPosCompleted { get; protected set; } = false;
        [field: SerializeField] public Vector3Int CellPos { get; protected set; } = Vector3Int.zero;
        public void SetCellPos(Vector3Int cellPos) => CellPos = cellPos;

        private Vector3Int _nextCellPos = Vector3Int.zero;
        public Vector3Int NextCellPos
        {
            get => _nextCellPos;
            set
            {
                _nextCellPos = value;
                LerpToCellPosCompleted = false;
            }
        }

        [field: SerializeField] public Vector3 SpawnedPos { get; protected set; } = Vector3.zero;
        [field: SerializeField] public Vector3Int SpawnedCellPos { get; protected set; } = Vector3Int.zero;

        public void UpdateCellPos()
        {
            Vector3Int currentCellPos = Managers.Map.WorldToCell(transform.position);
            Managers.Map.RemoveObject(this);
            Managers.Map.AddObject(this, currentCellPos);
            CellPos = currentCellPos;
        }

        public void UpdateCellPos(Vector3 worldPos)
        {
            Vector3Int currentCellPos = Managers.Map.WorldToCell(worldPos);
            Managers.Map.RemoveObject(this);
            Managers.Map.AddObject(this, currentCellPos);
            CellPos = currentCellPos;
        }

        public void SetCellPos(Vector3 position, bool forceMove = false)
            => SetCellPos(cellPos: Managers.Map.WorldToCell(position), forceMove: forceMove);

        public void SetCellPos(Vector3Int cellPos, bool stopLerpToCell = false, bool forceMove = false)
        {
            CellPos = cellPos;

            if (stopLerpToCell == false) // 이녀석 때문임 !!!
                LerpToCellPosCompleted = false;

            if (forceMove)
            {
                transform.position = Managers.Map.GetCenterWorld(cellPos); // 이동은 셀 가운데로
                LerpToCellPosCompleted = true;
            }
        }

        public virtual void LerpToCellPos(float movementSpeed)
        {
            if (LerpToCellPosCompleted)
                return;

            //Vector3 destPos = Managers.Map.GetCenterWorld(CellPos);
            Vector3 destPos = Managers.Map.GetCenterWorld(NextCellPos);

            Vector3 dir = destPos - transform.position;
            if (dir.x < 0f)
                LookAtDir = ELookAtDirection.Left;
            else if (dir.x > 0f)
                LookAtDir = ELookAtDirection.Right;

            if (dir.sqrMagnitude < 0.001f)
            {
                transform.position = destPos;
                LerpToCellPosCompleted = true;
                return;
            }

            float moveDist = Mathf.Min(dir.magnitude, movementSpeed * Time.deltaTime);
            transform.position += dir.normalized * moveDist;
        }

        protected void HitShakeMovement(float duration, float power, int vibrato)
        {
            Vector3 startPos = transform.position;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOShakePosition(duration, new Vector3(power, 0f, 0f), vibrato).SetEase(Ease.InBounce))
               .OnComplete(() => {
                if (this.IsValid())
                       transform.position = startPos;
               });
        }

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BaseBody = GetComponent<BaseBody>();
            BaseAnim = Util.FindChild<BaseAnimation>(gameObject, name: ReadOnly.Util.AnimationBody, recursive: false);

            Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            Collider.isTrigger = true;

            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            RigidBody.gravityScale = 0f;
            RigidBody.mass = 0f;
            RigidBody.drag = 0f;
            RigidBody.bodyType = RigidbodyType2D.Kinematic;
            RigidBody.simulated = false;

            SortingGroup = gameObject.GetOrAddComponent<SortingGroup>();
            SortingGroup.sortingLayerName = ReadOnly.SortingLayers.SLName_BaseObject;
            SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_BaseObject;

            return true;
        }

        private bool _initCompleted = false;
        public virtual bool SetInfo(int dataID, Vector3 spawnPos)
        {
            if (_initCompleted)
            {
                EnterInGame(spawnPos);
                return false;
            }

            _initCompleted = true;
            InitialSetInfo(dataID);
            EnterInGame(spawnPos);
            return true;
        }

        protected virtual void InitialSetInfo(int dataID)
        {
            DataTemplateID = dataID;
            BaseBody.InitialSetInfo(dataID, this);
            BaseAnim.InitialSetInfo(dataID, this);
            BaseEffect = gameObject.GetOrAddComponent<EffectComponent>();
            BaseEffect.InitialSetInfo(this);

            // --- InitStat
            if (Managers.Data.StatDataDict.TryGetValue(dataID, out StatData statData) == false)
                return;

            StatData = statData;

            _level = DataTemplateID;
            Hp = StatData.MaxHp;

            MaxHp = MaxHpBase = StatData.MaxHp;
            MinAtk = MinAtkBase = StatData.MinAtk;
            MaxAtk = MaxAtkBase = StatData.MaxAtk;
            CriticalRate = CriticalRateBase = StatData.CriticalRate;
            DodgeRate = DodgeRateBase = StatData.DodgeRate;
            MovementSpeed = MovementSpeedBase = StatData.MovementSpeed;
        }

        protected virtual void EnterInGame(Vector3 spawnPos)
        {
            // StopAllCoroutines(); // --- TEST
            BaseBody.ResetMaterialsAndColors();
            // BaseBody.StartCoFadeInEffect();
            SpawnedPos = spawnPos;
            SpawnedCellPos = Managers.Map.WorldToCell(spawnPos);
            MaxHp = StatData.MaxHp;
            Hp = StatData.MaxHp;
            Debug.Log($"<color=white>{gameObject.name}, {nameof(EnterInGame)}</color>");
        }

        public virtual void OnDamaged(BaseObject attacker, SkillBase skillByAttacker)
        {
            if (attacker.IsValid() == false)
                return;

            //float finalDamage = attacker.MinAtk;
            float damage = UnityEngine.Random.Range(attacker.MinAtk, attacker.MaxAtk);
            float finalDamage = Mathf.FloorToInt(damage);

            Hp = Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
            bool isCritical = false;
            Managers.Object.ShowDamageFont(position: this.CenterPosition, damage: finalDamage, isCritical: isCritical);
            // if (UnityEngine.Random.Range(0f, 100f) >= 50f)
            //     isCritical = true;

            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillByAttacker);
            }
            else
            {
                BaseBody.StartCoHurtFlashEffect(isCritical: isCritical);
            }
        }

        public virtual void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            RigidBody.simulated = false;
            //BaseBody.StartCoFadeOutEffect(() => OnDeadFadeOutCompleted());
        }

        protected virtual void OnDisable() { }
        #endregion
    }
}