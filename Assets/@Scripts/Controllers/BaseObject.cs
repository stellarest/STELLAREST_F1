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

        public Vector3 MoveDir { get; protected set; } = Vector2.zero;


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
            Collider.isTrigger = true; // ##### TEMP #####
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

        public void SetRigidBodyVelocity(Vector2 velocity)
        {
            if (RigidBody == null)
                return;

            RigidBody.velocity = velocity;
            if (velocity == Vector2.zero) // DO NOT FLIP.
                return;

            if (velocity.x < 0)
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
        public bool LerpToCellPosCompleted { get; protected set; } = false;

        private Vector3Int _cellPos = Vector3Int.zero;
        public Vector3Int CellPos // *** CORE ***
        {
            // 이제부터, CellPosition이 진짜 정보이고, transform.position은 랜더링용 정보가 된다.
            // 그래서 만약에 오브젝트의 CellPosition과 transform.position이 일치하지 않을 경우에는
            // 어떻게든 오브젝트의 이동을 스르르륵 보정을 해줘서 물체가 해당 칸으로 오게끔 유도를 해줘야한다.
            // 이거와 관련된 부분은 LerpToCellPos(float) 참조
            get => _cellPos;
            protected set
            {
                _cellPos = value;
                LerpToCellPosCompleted = false; // ### ??? ###
            }
        }

        public void SetCellPos(Vector3Int cellPos, bool forceMove = false)
        {
            CellPos = cellPos;
            LerpToCellPosCompleted = false;

            if (forceMove) // 순간 이동
            {
                transform.position = Managers.Map.CellToWorld(cellPos);
                LerpToCellPosCompleted = true;
            }
        }

        // CellPos를 넣으면 그거에 대한 WorldPos를 반환해서 거기로 이동시킴
        // 즉, 정확하게 Cell위치에 있지 않은 오브젝트를 CellPos로 이동시키는 것임.
        // 일단 CellPos로 이동시키는 것이긴한데.
        public void LerpToCellPos(float movementSpeed)
        {
            if (LerpToCellPosCompleted)
                return;

            Vector3 destPos = Managers.Map.CellToWorld(CellPos);
            Vector3 dir = destPos - transform.position;
            if (dir.x < 0f)
                LookAtDir = ELookAtDirection.Left;
            else
                LookAtDir = ELookAtDirection.Right;

            if (dir.magnitude < Mathf.Epsilon)
            {
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
