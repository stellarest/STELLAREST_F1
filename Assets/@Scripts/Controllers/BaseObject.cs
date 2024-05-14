using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
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
        // Creature 및 다른 오브젝트의 경우, 발바닥 부분이 피벗임
        
        public Vector3 CenterPosition { get => transform.position + Vector3.up * ColliderRadius; }

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

        protected float DistanceToTargetSQR
        {
            get
            {
                if (Target.IsValid() == false)
                    return 0.0f;

                Vector3 toTargetDir = Target.transform.position - transform.position;
                return UnityEngine.Mathf.Max(0.0f, toTargetDir.sqrMagnitude);
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

        public BaseObject Target { get; set; } = null;

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

        public void LookAtTarget(BaseObject target) // TEMP
        {
            if (Target.IsValid() == false)
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

        #region Battle
        public virtual void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker) { }
        public virtual void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            RigidBody.simulated = false;
            StartCoroutine(CoDeadFadeOut(() => Managers.Object.Despawn(this, DataTemplateID)));
        }

        protected virtual IEnumerator CoDeadFadeOut(System.Action callback = null)
        {
            if (this.isActiveAndEnabled == false)
                yield break;

            yield return new WaitForSeconds(ReadOnly.Numeric.StartDeadFadeOutTime);

            float delta = 0f;
            float percent = 1f;
            AnimationCurve curve = Managers.Animation.Curve(EAnimationCurveType.Ease_In);
            while (percent > 0f)
            {
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
            callback?.Invoke();
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

        [SerializeField] private Vector3Int _cellPos = Vector3Int.zero;
        public Vector3Int CellPos // ### CORE
        {
            get => _cellPos;
            protected set
            {
                _cellPos = value;
                LerpToCellPosCompleted = false;
            }
        }

        public void SetCellPos(Vector3 position, bool forceMove = false)
            => SetCellPos(Managers.Map.WorldToCell(position), forceMove);

        public void SetCellPos(Vector3Int cellPos, bool forceMove = false)
        {
            CellPos = cellPos;
            LerpToCellPosCompleted = false;
            if (forceMove) // 순간 이동
            {
                transform.position = Managers.Map.CenteredCellToWorld(cellPos);
                LerpToCellPosCompleted = true;
            }
        }

        // LerpToCellPosComplated: false
        public void LerpToCellPos(float movementSpeed) // Coroutine every tick
        {
            if (LerpToCellPosCompleted)
                return;

            Vector3 destPos = Managers.Map.CenteredCellToWorld(CellPos);
            Vector3 dir = destPos - transform.position;
    
            if (dir.x < 0f)
                LookAtDir = ELookAtDirection.Left;
            else if (dir.x > 0f)
                LookAtDir = ELookAtDirection.Right;

            // dir.sqrMagnitude < Mathf.Epsilon : 애초에 불가능
            // Origin : 0.01f (버그는 해결되는데 좀 딱딱해보임)
            // Mathf.Approximately(dir.sqrMagnitude, Mathf.Epsilon)
            // 고쳐야될수도 있음. 메모장에 A* 뻑났을 때 예외상황 참고.
            // ---> 조금 널널하게 값을 주면 제자리 걸음은 고치게됨. 도착으로 인식하게 되어서.
            // 그러나 와리가리는 안고쳐짐. (Fail_LerpCell) // 0.001f : OK, But ReplaceHeroes not okay
            if (dir.sqrMagnitude < 0.001f) // 0.001f
            {
                // 일단 도착점이 나온다는건 알았음. 근데 왜 일로감?
                Debug.Log("############## MOVEMENT COMPLETED ####################");
                transform.position = destPos;
                LerpToCellPosCompleted = true; 
                return;
            }

            float moveDist = Mathf.Min(dir.magnitude, movementSpeed * Time.deltaTime);
            transform.position += dir.normalized * moveDist;
        }
        #endregion
    }
}
