using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public abstract class ProjectileMotionBase : InitBase
    {
        public EProjectileMotionType MotionType { get; set; } = EProjectileMotionType.None;
        public EAnimationCurveType CurveType { get; set; } = EAnimationCurveType.None;
        
        private Coroutine _coLaunchProjectile = null;
        public Vector3 StartPosition { get; private set; } = Vector3.zero;
        public Vector3 TargetPosition { get; private set; } = Vector3.zero;
        public bool RotateToTarget { get; private set; } = false;
        public Data.ProjectileData ProjectileData { get; private set; } = null;
        protected System.Action _endCallback = null;
        protected float _movementSpeed = 0.0f;
        public bool EndMotion { get; protected set; } = false;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public void SetMotion(int dataID, Vector3 spawnPosition, Vector3 targetPosition, System.Action endCallback = null)
        {
            EndMotion = false;
            _movementSpeed = 5.0f;
            if (dataID != -1)
            {
                ProjectileData = Managers.Data.ProjectileDataDict[dataID];
                _movementSpeed = ProjectileData.MovementSpeed;
            }

            StartPosition = spawnPosition;
            TargetPosition = targetPosition;
            _endCallback = endCallback;

            RotateToTarget = ProjectileData.RotateToTarget;

            if (_coLaunchProjectile != null)
                StopCoroutine(_coLaunchProjectile);

            _coLaunchProjectile = StartCoroutine(CoLaunchProjectile());
        }

        protected void Rotation(Vector2 toTargetDir)
        {
            toTargetDir = toTargetDir.normalized;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg);
        }

        protected abstract IEnumerator CoLaunchProjectile(); 
    }
}
