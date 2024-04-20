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
        public int DataTemplateID { get; private set; } = -1;
        public EObjectType ObjectType { get; protected set; } = EObjectType.None;
        public EObjectRarity ObjectRarity { get; protected set; } = EObjectRarity.Common;
        public BaseAnimation BaseAnim { get; private set; } = null;
        public CircleCollider2D Collider { get; private set; } = null;
        public Rigidbody2D RigidBody { get; private set; } = null;
        public SortingGroup SortingGroup { get; private set; } = null;
        public float ColliderRadius { get => Collider != null ? Collider.radius : 0.0f; }
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
            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            RigidBody.gravityScale = 0f;
            SortingGroup = gameObject.GetOrAddComponent<SortingGroup>();

            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
                return false;

            DataTemplateID = dataID;
            ObjectRarity = EObjectRarity.Common;

            if (ObjectType == EObjectType.Hero || ObjectType == EObjectType.Monster)
                SetStat(dataID);

            return true;
        }

        private void SetStat(int dataID)
        {
            if (Managers.Data.StatDataDict.TryGetValue(dataID, out Data.StatData statData) == false)
            {
                Debug.LogError($"{nameof(BaseObject)}, {nameof(SetStat)}, Input : \"{dataID}\"");
                return;
            }

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

            if (velocity == Vector2.zero)
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

        protected void ShowBody(bool show)
        {
            foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>())
            {
                spr.enabled = show;
                if (show)
                {
                    spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
                    spr.gameObject.SetActive(true);
                }
            }

            if (show)
                RigidBody.simulated = true;
        }
    }
}
