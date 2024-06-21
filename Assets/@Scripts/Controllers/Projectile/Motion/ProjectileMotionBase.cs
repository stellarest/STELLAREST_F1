using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public abstract class ProjectileMotionBase : InitBase
    {
        public int DataTemplateID { get; set; } = -1;
        public Data.ProjectileData ProjectileData { get; private set; } = null;

        public Vector3 StartPosition { get; protected set; } = Vector3.zero; // TEMP
        public Vector3 TargetPosition { get; private set; } = Vector3.zero;

        public EProjectileMotionType MotionType { get; private set; } = EProjectileMotionType.None;
        public EAnimationCurveType AnimCurveType { get; private set; } = EAnimationCurveType.None;


        private bool _rotateToTarget = false;
        public bool RotateToTarget
        {
            get => _rotateToTarget;
            private set => _rotateToTarget = value;
        }

        protected float _movementSpeed = 0.0f;
        protected float _atkRange = 0.0f;
        
        protected Action _endCallback = null;
        private Coroutine _coLaunchProjectile = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override bool SetInfo(BaseObject owner, int dataID)
        {
            DataTemplateID = dataID;
            ProjectileData = Managers.Data.ProjectileDataDict[dataID];
            MotionType = Util.GetEnumFromString<EProjectileMotionType>(ProjectileData.Type);
            AnimCurveType = Util.GetEnumFromString<EAnimationCurveType>(ProjectileData.AnimationCurveType);
            RotateToTarget = ProjectileData.RotateToTarget;
            _movementSpeed = ProjectileData.MovementSpeed;

            StartPosition = transform.position;
            // TargetPosition = owner.Target.CenterPosition; // 이동하면서 중간에 사정거리에서 아웃되면 null crash
            TargetPosition = owner.TargetPosition;

            if (_coLaunchProjectile != null)
                StopCoroutine(_coLaunchProjectile);

            ReadyToLaunch();
            _coLaunchProjectile = StartCoroutine(CoLaunchProjectile());

            return true;
        }

        public void SetEndCallback(Action endCallback)
        {
            _endCallback -= endCallback;
            _endCallback += endCallback;
        }

        protected void Rotation2D(Vector2 targetDir)
        {
            targetDir = targetDir.normalized;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg);
        }

        protected abstract void ReadyToLaunch();

        protected abstract IEnumerator CoLaunchProjectile(); 
    }
}
