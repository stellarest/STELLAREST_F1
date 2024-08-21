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
        private const float MaxHeight = 2F;
        public float HeightArc { get; private set; } = MaxHeight;
        private const float FalldownSpeed = 2F;
        private const float FalldownRotSpeed = 10F;

        protected override IEnumerator CoLaunchProjectile()
        {
            float startTime = Time.time;
            float journeyLength = Vector2.Distance(_startPos, _targetPos);
            float totalTime = journeyLength / _projectile.ProjectileSpeed; // 거리를 속도로 나누면 시간

            AnimationCurve curve = Managers.Contents.Curve(_projectile.ProjectileCurveType);
            Vector3 nextPos = Vector3.zero;
            while (Time.time - startTime < totalTime)
            {
                float normalizedTime = (Time.time - startTime) / totalTime;
                float baseX = Mathf.Lerp(_startPos.x, _targetPos.x, curve.Evaluate(normalizedTime));
                float baseY = Mathf.Lerp(_startPos.y, _targetPos.y, curve.Evaluate(normalizedTime));

                float arc = HeightArc * Mathf.Sin(normalizedTime * Mathf.PI);
                float arcY = baseY + arc;

                nextPos = new Vector3(baseX, arcY);
                if (_projectile.RotateToTarget)
                    Rotate2D(nextPos - transform.position);

                transform.position = nextPos;
                yield return null;
            }

            // --- Falldown
            while (true)
            {
                // --- 로컬 오른쪽 방향으로 이동
                Vector3 localRight = transform.TransformDirection(Vector3.right) * _projectile.ProjectileSpeed * Time.deltaTime;

                // --- 하강 속도를 따로 적용하여 서서히 하강
                Vector3 downwardMovement = Vector3.down * FalldownSpeed * Time.deltaTime;

                // --- 다음 위치 계산
                nextPos = transform.position + localRight + downwardMovement;

                // --- 방향을 향해 부드럽게 회전
                Vector3 rotDir = (nextPos - transform.position).normalized;
                float angle = Mathf.Atan2(rotDir.y, rotDir.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

                // --- 위치, 회전 업데이트
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * FalldownRotSpeed);
                transform.position = nextPos;

                yield return null;
            }
        }

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