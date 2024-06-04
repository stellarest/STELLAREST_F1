using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace STELLAREST_F1
{
    public class StraightMotion : ProjectileMotionBase
    {
        protected override void ReadyToLaunch()
        {
        }
        
        protected override IEnumerator CoLaunchProjectile()
        {
            // Debug.Log($"### Launch::StraightMotion ###");
            // ########## TEST ##########
            // StartPosition = Owner.WeaponLWorldPosition + ((TargetPosition - Owner.WeaponLWorldPosition).normalized * 2.25f);
            // ##############################
            float distance = (StartPosition - TargetPosition).magnitude;
            float totalMovementTime = distance / _movementSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < totalMovementTime)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / totalMovementTime;
                transform.position = Vector3.Lerp(StartPosition, TargetPosition, normalizedTime);

                if (RotateToTarget)
                    Rotation2D(TargetPosition - transform.position);                

                yield return null;
            }

            transform.position = TargetPosition;
            _endCallback?.Invoke();
            yield break;
        }
    }
}
