using System.Collections;
using System.Collections.Generic;
// using Cinemachine; --> 제거함
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CameraController : InitBase
    {
        // Ease-In-Out(TEMP)
        [SerializeField] private AnimationCurve _curve = null;
        private Coroutine _coSetTarget = null;
        [SerializeField] private bool _processTargeting = true;
        private BaseObject _target = null;
        public BaseObject Target
        {
            get => _target;
            set
            {
                // ***** VIRTUAL CAM (DELETED) *****
                // SetTarget(value.transform);
                // ***********************
                if (_target == null)
                {
                    _target = value;
                    _coSetTarget = StartCoroutine(CoMoveToTarget()); // 최초에는 No Coroutine
                }
                else if (_target != value)
                {
                    if (_coSetTarget != null)
                        StopCoroutine(_coSetTarget);

                    _target = value;
                    _coSetTarget = StartCoroutine(CoMoveToTarget()); ;
                }
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Camera.main.orthographicSize = ReadOnly.Numeric.CamOrthoSize + 3f;
            return true;
        }

        private void LateUpdate()
        {
            if (_processTargeting)
                return;
            else if (_target != null)
            {
                Vector3 targetPos = new Vector3(_target.CenterPosition.x, _target.CenterPosition.y, -10f);
                transform.position = targetPos;
            }
        }

        private IEnumerator CoMoveToTarget()
        {
            _processTargeting = true;
            float delta = 0f;
            float percent = 0f;
            //AnimationCurve curve = Managers.Animation.Curve(EAnimationCurveType.Ease_In_Out); 
            Vector3 startPos = transform.position;
            Vector3 targetPos = new Vector3(_target.CenterPosition.x, _target.CenterPosition.y, -10f);
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = Mathf.Clamp01(delta / ReadOnly.Numeric.CamDesiredMoveToTargetTime);
                // Debug.Log($"Percent: {percent}"); // 별도로 속도를 곱하지않고 하면 제대로 동작함
                targetPos = new Vector3(_target.CenterPosition.x, _target.CenterPosition.y, -10f);
                //Vector3 targetPos = new Vector3(_target.CenterPosition.x, _target.CenterPosition.y, -10f);
                transform.position = Vector3.Lerp(startPos, targetPos, _curve.Evaluate(percent));
                yield return null;
            }

            transform.position = new Vector3(_target.CenterPosition.x, _target.CenterPosition.y, -10f);
            _processTargeting = false;
        }
    }
}

/*
        // Cinemachine cam Note - DELETED
        // 버츄어 카메라 방식 (DELETED, but 참고)
        // Soft Zone Width 만땅(2)
        // Soft Zone Height 만땅(2)로 설정하면 히어로들이 서로 멀리 있을 때 시점 전환해도 부드럽게 됨.
        // 100% 맞음 0으로 하면 그냥 팍팍 전환됨.
        // 근데 일단 없애고 코드 베이스로 해보기. 이쁘긴한데 좀 과하긴함.
        // 코드 베이스로 다 만들면 Cinemachine Remove하면 됨

        // private void SetTarget(Transform target) // Cinemachine Virtual Cam (DELETED)
        // {
        //     GetComponent<CinemachineVirtualCamera>().Follow = target;
        //     //this.Target = target;
        // }
*/