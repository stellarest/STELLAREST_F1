using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.Scripting;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public abstract class ProjectileMotionBase : MonoBehaviour
    {
        protected Vector3 _startPos = Vector3.zero;
        protected Vector3 _targetPos = Vector3.zero;
        protected Projectile _projectile = null;
        // protected Action _endCallback = null; // event keyworld 임시로 빼둠
        public Vector3 LaunchingDir { get; protected set; } = Vector3.zero;

        public void ReadyToLaunch(Vector3 startPos, Vector3 targetPos, Projectile projectile)
        {
            _startPos = startPos;
            _targetPos = targetPos;
            _projectile = projectile;

            // _endCallback -= AddEndCallback;
            // _endCallback += AddEndCallback;

            StartCoroutine(CoLaunchProjectile());
        }

        protected virtual void AddEndCallback()
            => Managers.Object.Despawn(_projectile, _projectile.DataTemplateID);

        protected void Rotate2D(Vector2 nDir)
            => transform.rotation = Quaternion.Euler(0f, 0, Mathf.Atan2(nDir.y, nDir.x) * Mathf.Rad2Deg);

        protected abstract IEnumerator CoLaunchProjectile();

        //     public int DataTemplateID { get; set; } = -1;
        //     public Data.ProjectileData ProjectileData { get; private set; } = null;

        //     public Vector3 StartPosition { get; protected set; } = Vector3.zero;
        //     public Vector3 TargetPosition { get; private set; } = Vector3.zero;

        //     public EProjectileMotionType MotionType { get; private set; } = EProjectileMotionType.None;
        //     public EAnimationCurveType AnimCurveType { get; private set; } = EAnimationCurveType.None;


        //     private bool _isRotateToTarget = false;
        //     public bool IsRotateToTarget
        //     {
        //         get => _isRotateToTarget;
        //         private set => _isRotateToTarget = value;
        //     }

        //     protected float _movementSpeed = 0.0f;
        //     protected float _atkRange = 0.0f;

        //     protected Action _endCallback = null;
        //     private Coroutine _coLaunchProjectile = null;

        //     public override bool Init()
        //     {
        //         if (base.Init() == false)
        //             return false;

        //         return true;
        //     }

        //     // 움직임과 관련된 부분만 받아올 것. 나머지 부분은 프로젝타일 또는 스킬 부분에서 처리하는게 맞다고 봄.
        //     public void InitialSetInfo(ProjectileData projectileData)
        //     {
        //         MotionType = projectileData.MotionType;
        //         AnimCurveType = Util.GetEnumFromString<EAnimationCurveType>(ProjectileData.AnimationCurveType);
        //         IsRotateToTarget = projectileData.IsRotateToTarget;
        //         _movementSpeed = ProjectileData.MovementSpeed;
        //     }

        //     public void EnterInGame(Vector3 startPos, Vector3 targetPos)
        //     {
        //         StartPosition = startPos;
        //         TargetPosition = targetPos;

        //         if (_coLaunchProjectile != null)
        //             StopCoroutine(_coLaunchProjectile);

        //         ReadyToLaunch();
        //         _coLaunchProjectile = StartCoroutine(CoLaunchProjectile());
        //     }

        //     // public override bool SetInfo(int dataID, BaseObject owner)
        //     // {
        //     //     DataTemplateID = dataID;
        //     //     ProjectileData = Managers.Data.ProjectileDataDict[dataID];
        //     //     MotionType = ProjectileData.MotionType;
        //     //     AnimCurveType = Util.GetEnumFromString<EAnimationCurveType>(ProjectileData.AnimationCurveType);
        //     //     RotateToTarget = ProjectileData.RotateToTarget;
        //     //     _movementSpeed = ProjectileData.MovementSpeed;
        //     //     StartPosition = transform.position;
        //     //     return true;
        //     // }

        //     public void SetEndCallback(Action endCallback)
        //     {
        //         _endCallback -= endCallback;
        //         _endCallback += endCallback;
        //     }

        //     protected void Rotation2D(Vector2 targetDir)
        //     {
        //         targetDir = targetDir.normalized;
        //         transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg);
        //     }

        //     protected abstract void ReadyToLaunch();

        //     protected abstract IEnumerator CoLaunchProjectile(); 
    }
}
