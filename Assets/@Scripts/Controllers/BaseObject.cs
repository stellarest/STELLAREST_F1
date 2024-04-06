using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BaseObject : InitBase
    {
        public int DataTemplateID { get; private set; } = -1;
        public EObjectType ObjectType { get; protected set; } = EObjectType.None;
        public BaseAnimation BaseAnim { get; private set; } = null;
        public CircleCollider2D Collider { get; private set; } = null;
        public Rigidbody2D RigidBody { get; private set; } = null;
        public float ColliderRadius { get => Collider != null ? Collider.radius : 0.0f; }
        //public float ColliderRadius2 { get => Collider?.radius ?? 0.0f; }
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
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BaseAnim = Util.FindChild<BaseAnimation>(gameObject, name: ReadOnly.String.AnimationBody, recursive: false);
            Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            RigidBody.gravityScale = 0f;

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

        protected virtual void Refresh()
        {
        }

        public void TranslateEx(Vector3 dir)
        {
            transform.Translate(dir);
            if (dir.x > 0)
            {
                LookAtDir = ELookAtDirection.Right;
            }
            else if (dir.x < 0)
            {
                LookAtDir = ELookAtDirection.Left;
            }
        }

        #region Animation
        public void UpdateAnimation()
            => BaseAnim.UpdateAnimation();

        // public void Flip(LookAtDirection lookAtDir)
        //     => BaseAnim.Flip(lookAtDir);
        #endregion
    }
}
