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
        public int DataTemplateID { get; set; } = -1;
        public EObjectType ObjectType { get; protected set; } = EObjectType.None;
        public BaseAnimation BaseAnim { get; private set; } = null;
        public CircleCollider2D Collider { get; private set; } = null;
        public Rigidbody2D RigidBody { get; private set; } = null;
        public float ColliderRadius { get => Collider != null ? Collider.radius : 0.0f; }
        public float ColliderRadius2 { get => Collider?.radius ?? 0.0f; }

        [SerializeField] private LookAtDirection _lookAtDir = LookAtDirection.Right;
        public LookAtDirection LookAtDir
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

        public virtual void SetInfo(int dataID)
        {
            DataTemplateID = dataID;
        }

        public void TranslateEx(Vector3 dir)
        {
            transform.Translate(dir);
            if (dir.x > 0)
                LookAtDir = LookAtDirection.Right;
            else if (dir.x < 0)
                LookAtDir = LookAtDirection.Left;
        }

        #region Animation
        public void UpdateAnimation()
            => BaseAnim.UpdateAnimation();

        public void Flip(LookAtDirection lookAtDir)
            => BaseAnim.Flip(lookAtDir);
        #endregion
    }
}
