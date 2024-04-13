using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
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

        public Vector3 MoveDir { get; protected set; } = Vector2.zero;


        #region Stat
        public float Hp { get; set; } = 0f;
        
        protected Stat _maxHp = null; 
        public float MaxHp
        {
            get => _maxHp.BaseValue;
            protected set
            {
                // DO SOMETHING
            }
        }
        
        protected Stat _atk = null;
        public float Atk
        {
            get => _atk.BaseValue;
            protected set
            {
                // DO SOMETHING
            }
        }

        protected Stat _atkRange = null;
        public float AtkRange
        {
            get => _atkRange.BaseValue;
            protected set
            {
                // DO SOMETHING
            }
        }

        protected Stat _movementSpeed = null;
        public float MovementSpeed
        {
            get => _movementSpeed.BaseValue;
            protected set
            {
                // DO SOMETHING
            }
        }
        #endregion

        public BaseObject Target { get; set; } = null;
        protected float AttackDistance
        {
            get
            {
                float env = 2.2f;
                if (Target != null && Target.ObjectType == EObjectType.Env)
                    return UnityEngine.Mathf.Max(env, this.Collider.radius + Target.Collider.radius + 0.1f);

                return AtkRange;
            }
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
            SortingGroup = gameObject.GetOrAddComponent<SortingGroup>();

            return true;
        }

        private bool _initialSet = false;
        public virtual bool SetInfo(int dataID)
        {
            if (_initialSet)
                return false;

            _initialSet = true;
            DataTemplateID = dataID;
            return true;
        }

        protected virtual void EnterInGame()
        {
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
        public virtual void OnDamaged(BaseObject attacker)
        {
        }

        public virtual void OnDead(BaseObject attacker)
        {
        }
        #endregion
    }
}
