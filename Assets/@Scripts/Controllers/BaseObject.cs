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
        public EObjectType ObjectType { get; protected set; } = EObjectType.None;
        public CircleCollider2D Collider { get; private set; } = null;
        public BaseAnimation BaseAnim { get; private set; } = null;
        public Rigidbody2D RigidBody { get; private set; } = null;

        public float ColliderRadius { get => Collider != null ? Collider.radius : 0.0f; }
        public float ColliderRadius2 { get => Collider?.radius ?? 0.0f; }

        private bool _lookLeft = true;
        public bool LookLeft
        {
            get => _lookLeft;
            set
            {
                _lookLeft = value;
                Flip(!value); // 뭐 굳이 이것까진 따라하지 않아도 됨.
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            BaseAnim = Util.FindChild<BaseAnimation>(gameObject, name = ReadOnly.String.AnimBody, recursive: false);
            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            RigidBody.gravityScale = 0f;

            return true;
        }

        public void TranslateEx(Vector3 dir)
        {
            transform.Translate(dir);
            if (dir.x < 0)
                LookLeft = true;
            else if (dir.x > 0)
                LookLeft = false;
        }

        #region Animation
        protected virtual void UpdateAnimation()
        {
        }

        public void PlayAnimation(string AnimName)
        {
            if (BaseAnim == null)
                return;
        }

        public void Flip(bool flag)
        {
            if (BaseAnim == null)
                return;
        }
        #endregion
    }
}
