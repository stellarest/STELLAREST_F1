using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class ParabolaMotion : ProjectileMotionBase
    {
        // 개선 필요
        // transform.position += (nextPos - transform.position).normalized * 3f;
        private const float MinHeight = 1F;
        private const float MaxHeight = 2F;
        public float HeightArc { get; private set; } = MaxHeight;
        private bool changeStraightMotion = false; // *****
        private const float MaxDistanceSQR = 144.0F;

        // protected override void ReadyToLaunch()
        // {
        //     float firstTick = Time.deltaTime;
        //     float baseX = Mathf.Lerp(StartPosition.x, TargetPosition.x, firstTick);
        //     float baseY = Mathf.Lerp(StartPosition.y, TargetPosition.y, firstTick);

        //     HeightArc = Mathf.Lerp(MinHeight, MaxHeight, Util.Distance(StartPosition, TargetPosition, isSQR: true)/ MaxDistanceSQR);
        //     float arc = HeightArc * Mathf.Sin(firstTick * Mathf.PI);
        //     float arcY = baseY + arc;

        //     Vector3 nextPos = new Vector3(baseX, arcY);
        //     transform.position += (nextPos - transform.position).normalized * 2f;
        //     Rotation2D(transform.position - nextPos);
        //     StartPosition = transform.position;
        // }

        // // 파라볼라 모션도 지금 고장나있는게 가까이 붙어 있으면 못맞춤
        // protected override IEnumerator CoLaunchProjectile()
        // {
        //     //Debug.Log($"### Launch::ParabolaMotion ###");
        //     float startTime = Time.time;
        //     float journeyLength = Vector2.Distance(StartPosition, TargetPosition);
        //     float totalTime = journeyLength / _movementSpeed;

        //     AnimationCurve curve = Managers.Contents.Curve(AnimCurveType);
        //     while (Time.time - startTime < totalTime)
        //     {
        //         float normalizedTime = (Time.time - startTime) / totalTime;
        //         // float baseX = Mathf.Lerp(StartPosition.x, TargetPosition.x, normalizedTime);
        //         // float baseY = Mathf.Lerp(StartPosition.y, TargetPosition.y, normalizedTime);
        //         float baseX = Mathf.Lerp(StartPosition.x, TargetPosition.x, curve.Evaluate(normalizedTime));
        //         float baseY = Mathf.Lerp(StartPosition.y, TargetPosition.y, curve.Evaluate(normalizedTime));

        //         float arc = HeightArc * Mathf.Sin(normalizedTime * Mathf.PI);
        //         float arcY = baseY + arc;

        //         Vector3 nextPos = new Vector3(baseX, arcY);
        //         if (IsRotateToTarget)
        //             Rotation2D(nextPos - transform.position);

        //         transform.position = nextPos;
        //         yield return null;
        //     }

        //     _endCallback?.Invoke();
        // }
    }
}


/*
    [ Super Latest2 ]
    protected override IEnumerator LaunchProjectile( )
    {
        float startTime = Time.time;
        float journeyLength = Vector2.Distance(_startPos, _endPos);
        float totalTime = journeyLength / _speed;

        while (Time.time - startTime < totalTime)
        {
            float normalizedTime = (Time.time - startTime) / totalTime;

            // 포물선 모양으로 이동
            float x = Mathf.Lerp(_startPos.x, _endPos.x, normalizedTime);
            float baseY = Mathf.Lerp(_startPos.y, _endPos.y, normalizedTime);
            float arc = _heightArc * Mathf.Sin(normalizedTime * Mathf.PI);

            float y = baseY + arc;

            var nextPos = new Vector3(x, y);
            if (IsRotation)
                transform.rotation = LookAt2D(nextPos - (Vector3)transform.position);
            transform.position = nextPos;

            yield return null;
        }

        EndCallback?.Invoke();
    }

    [ Super Latest1 ]
    protected override IEnumerator CoLaunchProjectile()
	{
		float journeyLength = Vector2.Distance(StartPosition, TargetPosition);
		float totalTime = journeyLength / _speed;
		float elapsedTime = 0;

		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;

			float normalizedTime = elapsedTime / totalTime;

			// 포물선 모양으로 이동
			float x = Mathf.Lerp(StartPosition.x, TargetPosition.x, normalizedTime);
			float baseY = Mathf.Lerp(StartPosition.y, TargetPosition.y, normalizedTime);
			float arc = HeightArc * Mathf.Sin(normalizedTime * Mathf.PI);

			float y = baseY + arc;

			var nextPos = new Vector3(x, y);

			if (LookAtTarget)
				LookAt2D(nextPos - transform.position);

			transform.position = nextPos;

			yield return null;
		}

		EndCallback?.Invoke();
	}

                   if (Projectile.Show == false && normalizedTime > 0.2f)
                {
                    // 일단 정리가 되었다고 가정
                    // "InvokeRatioOnUpdate" : 0.62 (오리지널)
                    // --->> 0.42로 미리 땅겨놓고. 안보여주다가 20% 지나면 보여줌.
                    // 어쨋든 평생 과제로 개선 필요함. 무조건 화살의 중앙에서 나오도록.
                    // 내 생각엔 Arrow의 Center Child를 정해서 Center를 회전시키면 한방에 해결될 것 같긴 한데.
                    Debug.Log("<color=cyan>SHOW BODY PROJECTILE</color>");
                    //Projectile.ShowBody(true);
                    //Projectile.Show = true;
                }
*/