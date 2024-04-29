using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace STELLAREST_F1
{
    public class StraightMotion : ProjectileMotionBase
    {
        protected override IEnumerator CoLaunchProjectile()
        {
            float distance = (StartPosition - TargetPosition).magnitude;
            float totalMovementTime = distance / _movementSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < totalMovementTime)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / totalMovementTime;
                transform.position = Vector3.Lerp(StartPosition, TargetPosition, normalizedTime);

                if (RotateToTarget)
                    Rotation(TargetPosition - transform.position);                

                yield return null;
            }

            transform.position = TargetPosition;
            _endCallback?.Invoke();
        }
    }
}
