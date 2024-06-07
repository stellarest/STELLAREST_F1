using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BaseObject : InitBase
    {
        private void Update()
        {
            if (Input.GetKeyDown("5"))
            {
                LevelUp();
            }    
        }

        public int DataTemplateID { get; protected set; } = -1;
        public EObjectType ObjectType { get; protected set; } = EObjectType.None;
        public EObjectRarity ObjectRarity { get; protected set; } = EObjectRarity.Common;
        public BaseAnimation BaseAnim { get; private set; } = null;
        public CircleCollider2D Collider { get; private set; } = null;
        public Rigidbody2D RigidBody { get; private set; } = null;
        public SortingGroup SortingGroup { get; private set; } = null;
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
        protected event System.Action OnDeadFadeOutEndHandler = null;

        #region Stat
        public Data.StatData StatData { get; private set; } = null;

        [SerializeField] private int _level = 0;
        public int Level => (_level % DataTemplateID) + 1;

        [SerializeField] protected int _maxLevel = 0;
        public int MaxLevel => (_maxLevel % DataTemplateID) + 1;
        public bool IsMaxLevel => _level == _maxLevel;
        protected void LevelUp()
        {
            if (_level < _maxLevel)
            {
                _level = Mathf.Clamp(_level + 1, DataTemplateID, _maxLevel);
                if (Managers.Data.StatDataDict.TryGetValue(_level, out Data.StatData statData))
                    StatData = statData; // Refresh
            }

            // Debug.Log($"===== {gameObject.name} =====");
            // Debug.Log($"LevelText: {StatData.LevelText}");
            // Debug.Log($"MaxHp: {StatData.MaxHp}");
            // Debug.Log($"Atk: {StatData.Atk}");
            // Debug.Log($"AtkRange: {StatData.AtkRange}");
            // Debug.Log($"CriticalRate: {StatData.CriticalRate}");
            // Debug.Log($"DodgeRate: {StatData.DodgeRate}");
            // Debug.Log($"MovementSpeed: {StatData.MovementSpeed}");
        }

        private float _hp = 0f;
        public float Hp
        {
            get => _hp;
            protected set => _hp = value;
        }

        public float MaxHpBase { get; set; } = 0f;
        public float AtkBase { get; set; } = 0f;
        public float AtkRangeBase { get; set; } = 0f;
        public float CriticalRateBase { get; set; } = 0f;
        public float DodgeRateBase { get; set; } = 0f;
        public float MovementSpeedBase { get; set; } = 0f;
        
        [field: SerializeField] public float MaxHp { get; set; } = 0f;
        [field: SerializeField] public float Atk { get; set; } = 0f;
        [field: SerializeField] public float AtkRange { get; set; } = 0f;
        [field: SerializeField] public float CriticalRate { get; set; } = 0f;
        [field: SerializeField] public float DodgeRate { get; set; } = 0f;
        [field: SerializeField] public float MovementSpeed { get; set; } = 0f;

        // Modifier를 밖에다가 빼기
        private bool _statDirtyFlag = false;
        private void RecalculateStat()
        {
            if (_statDirtyFlag == false)
                return;

            _statDirtyFlag = true;
            // TODO Recalc
            // ITEM
            // BUFFS
            // ...
        }
        #endregion

        [field: SerializeField] private BaseObject _target = null;
         public virtual BaseObject Target 
         { 
            get => _target;
            set => _target = value;
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BaseAnim = Util.FindChild<BaseAnimation>(gameObject, name: ReadOnly.String.AnimationBody, recursive: false);
            Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            RigidBody.gravityScale = 0f;
            RigidBody.mass = 0f;
            RigidBody.drag = 0f;

            // RigidbodyType2D - Dynamic: 물리 완전 제어, 높은 비용, 충돌 감지
            // RigidbodyType2D - Kinematic: 물리 회전, 위치를 업데이트 하지 않음, 비교적 낮은 비용, 충돌 감지
            // RigidbodyType2D - Static: 절대적으로 움직이지 않는 상태에서만 충돌 감지.
            RigidBody.bodyType = RigidbodyType2D.Kinematic;
            SortingGroup = gameObject.GetOrAddComponent<SortingGroup>();
            SortingGroup.sortingLayerName = ReadOnly.SortingLayers.SLName_BaseObject;
            SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_BaseObject;

            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame(dataID);
                return false;
            }

            DataTemplateID = dataID;
            ObjectRarity = EObjectRarity.Common; // TEMP
            SetStat(dataID);

            OnDeadFadeOutEndHandler -= this.OnDeadFadeOutEnded;
            OnDeadFadeOutEndHandler += this.OnDeadFadeOutEnded;

            return true;
        }

        protected override void EnterInGame(int dataID)
        {
            if (ObjectType == EObjectType.Projectile)
                return;

            // Reset Stat
            Hp = StatData.MaxHp;
        }

        protected virtual void SetStat(int dataID)
        {
            if (Managers.Data.StatDataDict.TryGetValue(dataID, out Data.StatData statData) == false)
                return;

            StatData = statData;

            _level = DataTemplateID;
            Hp = StatData.MaxHp;

            MaxHp = MaxHpBase = StatData.MaxHp;
            Atk = AtkBase = StatData.Atk;
            AtkRange = AtkRangeBase = StatData.AtkRange;
            CriticalRate = CriticalRateBase = StatData.CriticalRate;
            DodgeRate = DodgeRateBase = StatData.DodgeRate;
            MovementSpeed = MovementSpeedBase = StatData.MovementSpeed;
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

        #region Animation
        public void UpdateAnimation()
            => BaseAnim.UpdateAnimation();
        #endregion

        #region Events
        protected virtual void OnDeadFadeOutEnded()
        {
            Managers.Object.Despawn(this, DataTemplateID);
            _initialSpawnedCellPos = null;
        }
        #endregion

        #region Battle
        public virtual void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker) { }
        public virtual void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            RigidBody.simulated = false; // RigidBody 제거 예정
            StartCoroutine(CoDeadFadeOut(this.OnDeadFadeOutEndHandler));
        }

        // 오~ 되는구나
        protected virtual void OnDisable()
        {
            // Debug.Log("BaseObject::OnDisable!!");
        }

        protected virtual IEnumerator CoDeadFadeOut(Action endFadeOutCallback = null)
        {
            if (isActiveAndEnabled == false)
                yield break;

            yield return new WaitForSeconds(ReadOnly.Numeric.StartDeadFadeOutTime);

            float delta = 0f;
            float percent = 1f;
            AnimationCurve curve = Managers.Contents.Curve(EAnimationCurveType.Ease_In);
            while (percent > 0f)
            {
                // Debug.Log($"{gameObject.name}, {percent}");
                delta += Time.deltaTime;
                percent = 1f - (delta / ReadOnly.Numeric.DesiredDeadFadeOutEndTime);
                foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>())
                {
                    float current = Mathf.Lerp(0f, 1f, curve.Evaluate(percent));
                    spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, current);
                }

                yield return null;
            }

            //Debug.Log($"{gameObject.name} is dead.");
            endFadeOutCallback?.Invoke();

            Managers.Object.Despawn(this, DataTemplateID);
            _initialSpawnedCellPos = null;
        }
        #endregion

        // protected void ShowBody(bool show)
        // {
        //     // includeInactive: true (임시, 나중에 개선 필요)
        //     foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>(includeInactive: true))
        //     {
        //         spr.enabled = show;
        //         if (show)
        //         {
        //             spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
        //             spr.gameObject.SetActive(true);
        //         }
        //     }
        // }

        public void ShowBody(bool show)
        {
            if (show == false)
                Collider.enabled = false;

            // includeInactive: true (임시, 나중에 개선 필요)
            foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>(includeInactive: true))
            {
                spr.enabled = show;
                if (show)
                {
                    spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
                    spr.gameObject.SetActive(true);
                }
            }

            if (show)
                Collider.enabled = true;
        }

        #region Map
        [field: SerializeField] public bool LerpToCellPosCompleted { get; protected set; } = false;


        // [SerializeField] private Vector3Int _cellPos = Vector3Int.zero;
        // public Vector3Int CellPos // ### CORE
        // {
        //     get => _cellPos;
        //     protected set
        //     {
        //         _cellPos = value;
        //         LerpToCellPosCompleted = false;
        //     }
        // }

        [field: SerializeField] public Vector3Int CellPos { get; protected set; } = Vector3Int.zero;

        private Vector3Int? _initialSpawnedCellPos = null;
        public Vector3Int? InitialSpawnedCellPos
        {
            get => _initialSpawnedCellPos.HasValue ? _initialSpawnedCellPos.Value : null;
            set
            {
                if (_initialSpawnedCellPos.HasValue == false)
                {
                    _initialSpawnedCellPos = value;
                }
                else
                {
                    Debug.LogWarning($"Already it has initial spawned cell pos: {_initialSpawnedCellPos.Value}");
                }
            }
        }

        public void SetCellPos(Vector3 position, bool forceMove = false)
            => SetCellPos(Managers.Map.WorldToCell(position), forceMove);

        public void UpdateCellPos()
        {
            Vector3Int currentCellPos = Managers.Map.WorldToCell(transform.position);
            Managers.Map.RemoveObject(this);
            Managers.Map.AddObject(this, currentCellPos);
            CellPos = currentCellPos;
        }

        // 이건 애초에 Add를 안함. 제거 예정?
        public void SetCellPos(Vector3Int cellPos, bool stopLerpToCell = false, bool forceMove = false)
        {
            CellPos = cellPos;

            if (stopLerpToCell == false) // 이녀석 때문임 !!!
                LerpToCellPosCompleted = false;

            if (forceMove)
            {
                transform.position = Managers.Map.CenteredCellToWorld(cellPos); // 이동은 셀 가운데로
                LerpToCellPosCompleted = true;
            }
        }

        public void LerpToCellPos(float movementSpeed) // Coroutine every tick
        {
            if (LerpToCellPosCompleted)
            {
                return;
            }

            Vector3 destPos = Managers.Map.CenteredCellToWorld(CellPos); // 이동은 가운데로.
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
        #endregion

        #region MISC
        #endregion
    }
}

/*
        Collider.includeLayers = 1 << (int)ELayer.Obstacle;
        Collider.excludeLayers = 1 << (int)ELayer.Monster | (1 << (int)ELayer.Hero);

        // protected float DistanceToTargetSQR
        // {
        //     get
        //     {
        //         if (Target.IsValid() == false)
        //             return 0.0f;

        //         Vector3 toTargetDir = Target.transform.position - transform.position;
        //         return UnityEngine.Mathf.Max(0.0f, toTargetDir.sqrMagnitude); // ??? 의미 없는데 어차피 무조건 양수 나오는데
        //     }
        // }

        // protected float AttackDistance // TEMP
        // {
        //     get
        //     {
        //         float threshold = 2.2f;
        //         if (Target.IsValid() && Target.ObjectType == EObjectType.Env)
        //             return UnityEngine.Mathf.Max(threshold, Collider.radius + Target.Collider.radius);

        //         return AtkRange + Collider.radius + Target.ColliderRadius;
        //     }
        // }
*/