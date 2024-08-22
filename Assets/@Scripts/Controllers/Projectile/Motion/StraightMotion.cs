using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace STELLAREST_F1
{
    public class StraightMotion : ProjectileMotionBase
    {
        protected override IEnumerator CoLaunchProjectile()
        {
            AnimationCurve curve = Managers.Contents.Curve(_projectile.ProjectileCurveType);
            LaunchingDir = _targetPos - _startPos;
            Vector3 nDir = (_targetPos - _startPos).normalized;
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(nDir.y, nDir.x) * Mathf.Rad2Deg);
            float delta = 0f;
            float calculatedSpeed = 0f;

            while (delta < 1f)
            {
                float prevDelta = delta;
                delta += Time.deltaTime;

                if (delta >= 1f)
                {
                    // delta가 1f를 초과했을 때, 정확히 1f 시점의 위치와 속도를 계산
                    delta = 1f;
                    Vector3 posAt1 = Vector3.Lerp(_startPos, _startPos + nDir * _projectile.ProjectileSpeed, curve.Evaluate(1f));
                    Vector3 posBefore1 = Vector3.Lerp(_startPos, _startPos + nDir * _projectile.ProjectileSpeed, curve.Evaluate(prevDelta));

                    float timeStep = delta - prevDelta;
                    calculatedSpeed = Vector3.Distance(posAt1, posBefore1) / timeStep;
                    // Debug.Log($"Calculated Speed at delta = 1: {calculatedSpeed}");
                    transform.position = posAt1;
                    break;
                }

                Vector3 nextPos = Vector3.Lerp(_startPos, _startPos + nDir * _projectile.ProjectileSpeed, curve.Evaluate(delta));
                transform.position = nextPos;
                yield return null;
            }

            // 계속해서 투사체 이동 (calculatedSpeed 사용)
            while (true)
            {
                delta += Time.deltaTime;
                Vector3 nextPos = transform.position + nDir * calculatedSpeed * Time.deltaTime;
                transform.position = nextPos;
                yield return null;
            }
        }
    }
}
