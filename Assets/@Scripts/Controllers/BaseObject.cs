using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BaseObject : InitBase
    {
        [field: SerializeField] public int DataTemplateID { get; protected set; } = -1;
        public EObjectType ObjectType { get; protected set; } = EObjectType.None;
        public SortingGroup SortingGroup { get; private set; } = null;
        public CircleCollider2D Collider { get; private set; } = null;
        public Rigidbody2D RigidBody { get; private set; } = null;
        public float ColliderRadius { get => Collider != null ? Collider.radius : 0.0f; }
        public Vector3 CenterPosition { get => transform.position + Vector3.up * ColliderRadius; }
        public Vector3 CenterLocalPosition => Vector3.up * ColliderRadius;
        [field: SerializeField] public Vector3 SpawnedPos { get; protected set; } = Vector3.zero;

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            Collider.isTrigger = true;

            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            RigidBody.gravityScale = 0f;
            RigidBody.mass = 0f;
            RigidBody.drag = 0f;
            RigidBody.bodyType = RigidbodyType2D.Kinematic;
            RigidBody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
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
        }

        protected virtual void EnterInGame(Vector3 spawnPos)
        {
            RigidBody.simulated = true;
            SpawnedPos = spawnPos;
        }

        // --- TEMP
        protected virtual void OnDisable() { }
        #endregion

        #region Background
        public static Vector3 GetLookAtRotation(Vector3 dir)
            => new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg);

        protected virtual void OnDeadFadeOutCompleted()
        {
            Managers.Object.Despawn(this, DataTemplateID);
            StopAllCoroutines(); // --- DEFENSE
        }

        protected void HitShakeMovement(float duration, float power, int vibrato)
        {
            Vector3 startPos = transform.position;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOShakePosition(duration, new Vector3(power, 0f, 0f), vibrato).SetEase(Ease.InBounce))
               .OnComplete(() =>
               {
                   if (this.IsValid())
                       transform.position = startPos;
               });
        }
        #endregion
    }
}
