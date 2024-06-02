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
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BaseObject : InitBase
    {
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

        protected float DistanceToTargetSQR
        {
            get
            {
                if (Target.IsValid() == false)
                    return 0.0f;

                Vector3 toTargetDir = Target.transform.position - transform.position;
                return UnityEngine.Mathf.Max(0.0f, toTargetDir.sqrMagnitude); // ??? 의미 없는데 어차피 무조건 양수 나오는데
            }
        }

        protected float AttackDistance // TEMP
        {
            get
            {
                float threshold = 2.2f;
                if (Target.IsValid() && Target.ObjectType == EObjectType.Env)
                    return UnityEngine.Mathf.Max(threshold, Collider.radius + Target.Collider.radius);

                return AtkRange + Collider.radius + Target.ColliderRadius;
            }
        }

        #region Stat
        public Data.StatData StatData { get; private set; } = null;
        private int _levelCount = -1;
        private int _levelValue = -1;
        private int _maxLevelValue = -1;
        public int Level
        {
            get => (_levelValue % DataTemplateID) + 1;
            private set
            {
                if (_levelValue == _maxLevelValue)
                    return;

                _levelValue++;
                _levelValue = UnityEngine.Mathf.Clamp(_levelValue, DataTemplateID, _maxLevelValue);
                if (_levelValue == _maxLevelValue)
                {
                    // TODO : DO SOMETHING WHEN GOT MAX LEVEL STATE (EX)COMMON -> ELITE
                }
                else
                {
                    // TODO : DO SOMETHING WHEN LEVEL UP (EX)EFFECT 
                }
            }
        }

        protected void LevelUp() => Level++;

        public float Hp { get; set; } = 0f;
        protected Stat _maxHp = null;
        public float MaxHp
        {
            get => _maxHp.BaseValue;
            protected set
            {
                _maxHp = _maxHp == null ? new Stat(value) : _maxHp;
            }
        }

        protected Stat _atk = null;
        public float Atk
        {
            get => _atk.BaseValue;
            protected set
            {
                _atk = _atk == null ? new Stat(value) : _atk;
            }
        }

        protected Stat _atkRange = null;
        public float AtkRange
        {
            get => _atkRange.BaseValue;
            protected set
            {
                _atkRange = _atkRange == null ? new Stat(value) : _atkRange;
            }
        }

        protected Stat _movementSpeed = null;
        public float MovementSpeed
        {
            get => _movementSpeed.BaseValue;
            protected set
            {
                _movementSpeed = _movementSpeed == null ? new Stat(value) : _movementSpeed;
            }
        }
        #endregion

        [field: SerializeField] public BaseObject Target { get; set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BaseAnim = Util.FindChild<BaseAnimation>(gameObject, name: ReadOnly.String.AnimationBody, recursive: false);
            Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            RigidBody.gravityScale = 0f;
            SortingGroup = gameObject.GetOrAddComponent<SortingGroup>();
            SortingGroup.sortingOrder = ReadOnly.Numeric.SortingLayer_Base;

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
            RigidBody.drag = 0f; // TEMP
            SetStat(dataID);

            OnDeadFadeOutEndHandler -= this.OnDeadFadeOutEnded;
            OnDeadFadeOutEndHandler += this.OnDeadFadeOutEnded;

            return true;
        }

        protected override void EnterInGame(int dataID)
        {
            if (ObjectType == EObjectType.Projectile)
                return;

            // Reset Stat (TEMP)
            MaxHp = StatData.MaxHp;
            Hp = MaxHp;
            Atk = StatData.Atk;
            AtkRange = StatData.AtkRange;
            MovementSpeed = StatData.MovementSpeed;
        }

        private void SetStat(int dataID)
        {
            if (Managers.Data.StatDataDict.TryGetValue(dataID, out Data.StatData statData) == false)
                return;

            StatData = statData;
            _levelCount = Managers.Data.StatDataDict.Count;
            _levelValue = dataID;
            _maxLevelValue = _levelValue + _levelCount - 1;

            Hp = statData.MaxHp;
            MaxHp = statData.MaxHp;
            Atk = statData.Atk;
            AtkRange = statData.AtkRange;
            MovementSpeed = statData.MovementSpeed;
        }

        public void LookAtValidTarget() // TEMP
        {
            if (Target.IsValid() == false) // 방어
                return;

            Vector3 toTargetDir = Target.transform.position - transform.position;
            if (toTargetDir.x < 0)
                LookAtDir = ELookAtDirection.Left;
            else
                LookAtDir = ELookAtDirection.Right;
        }

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

        protected virtual IEnumerator CoDeadFadeOut(Action endFadeOutCallback = null)
        {
            if (isActiveAndEnabled == false)
                yield break;

            yield return new WaitForSeconds(ReadOnly.Numeric.StartDeadFadeOutTime);

            float delta = 0f;
            float percent = 1f;
            AnimationCurve curve = Managers.Animation.Curve(EAnimationCurveType.Ease_In);
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

            Debug.Log($"{gameObject.name} is dead.");
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
