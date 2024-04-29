using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class ParabolaMotion : ProjectileMotionBase
    {
        // 개선 필요
        public float HeightArc { get; protected set; } = 2.0f; 
        protected override IEnumerator CoLaunchProjectile()
        {
            float distance = (StartPosition - TargetPosition).magnitude;
            float reachToTargetTime = distance / _movementSpeed; 
            float elapsedTime = 0.0f;

            AnimationCurve curve = Managers.Animation.Curve(EAnimationCurveType.Ease_In);
            while (elapsedTime < reachToTargetTime)
            {
                elapsedTime += Time.deltaTime;

                float normalizedTime = elapsedTime / reachToTargetTime;
                float baseX = Mathf.Lerp(StartPosition.x, TargetPosition.x, curve.Evaluate(normalizedTime));
                float baseY = Mathf.Lerp(StartPosition.y, TargetPosition.y, curve.Evaluate(normalizedTime));
                float arc = HeightArc * Mathf.Sin(normalizedTime * Mathf.PI);
                float arcY = baseY + arc;

                Vector3 nextPos = new Vector3(baseX, arcY);
                if (RotateToTarget)
                    Rotation(nextPos - transform.position);

                transform.position = nextPos;
                yield return null;
            }
            
            EndMotion = true;
            _endCallback?.Invoke();
        }
    }
}

