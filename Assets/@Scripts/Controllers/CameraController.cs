using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CameraController : InitBase
    {
        //[SerializeField] private AnimationCurve _curveEaseOut = null;
        private Coroutine _coSetTarget = null;
        [SerializeField] private bool _processTargeting = true;
        private BaseObject _target = null;
        public BaseObject Target
        {
            get => _target;
            set
            {
                // 버츄어 카메라 방식
                // Soft Zone Width 만땅(2)
                // Soft Zone Height 만땅(2)로 설정하면 히어로들이 서로 멀리 있을 때 시점 전환해도 부드럽게 됨.
                // 100% 맞음 0으로 하면 그냥 팍팍 전환됨.
                // 근데 일단 없애고 코드 베이스로 해보기. 이쁘긴한데 좀 과하긴함.
                // 코드 베이스로 다 만들면 Cinemachine Remove하면 됨
                SetTarget(value.transform); 

                // if (_target == null)
                // {
                //     _target = value;
                //     // _coSetTarget = StartCoroutine(CoMoveToTarget()); 최초에는 No Coroutine
                // }
                // else if (_target != value)
                // {
                //     if (_coSetTarget != null)
                //         StopCoroutine(_coSetTarget);

                //     _target = value;
                //     _coSetTarget = StartCoroutine(CoMoveToTarget()); ;
                // }
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Camera.main.orthographicSize = ReadOnly.Numeric.CamOrthoSize;
            return true;
        }

        private void SetTarget(Transform target)
        {
            GetComponent<CinemachineVirtualCamera>().Follow = target;
            //this.Target = target;
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

            // if (_target == null)
            //     return;

            // // TEMP
            // //Vector3 toTargetPos = new Vector3(_target.transform.position.x,  _target.transform.position.y + 1.3f, -10f);
            // //Vector3 toTargetPos = new Vector3(_target.transform.position.x,  _target.transform.position.y, -10f);
            // Vector3 toTargetPos = new Vector3(_target.CenterPosition.x,  _target.CenterPosition.y, -10f);
            // transform.position = toTargetPos;
            
            // if (_processingChange)
            // {
            //     Vector3 toTargetPos = new Vector3(_target.CenterPosition.x,  _target.CenterPosition.y, -10f);
            //     transform.position = toTargetPos;
            // }
        }

        private IEnumerator CoMoveToTarget()
        {
            _processTargeting = true;
            float delta = 0f;
            float percent = 0f;
            AnimationCurve curve = Managers.Animation.Curve(EAnimationCurveType.Ease_Out); 

            Vector3 startPos = transform.position;
            Vector3 targetPos = new Vector3(_target.CenterPosition.x, _target.CenterPosition.y, -10f);
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = Mathf.Clamp01(delta / ReadOnly.Numeric.CamDesiredMoveToTargetTime);
                Debug.Log($"Percent: {percent}");
                //Vector3 targetPos = new Vector3(_target.CenterPosition.x, _target.CenterPosition.y, -10f);
                transform.position = Vector3.Lerp(startPos, targetPos, curve.Evaluate(percent));
                yield return null;
            }
            
            transform.position = new Vector3(_target.CenterPosition.x, _target.CenterPosition.y, -10f);
            _processTargeting = false;
        }
    }
}

