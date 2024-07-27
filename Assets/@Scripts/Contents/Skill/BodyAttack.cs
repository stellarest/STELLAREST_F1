using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BodyAttack : SkillBase
    {
        private float _delta = 0f;
        private float _desiredTimeToReach = 0.35f;
        private float _desiredTimeToReturn = 0.65f;

        private Vector3 _startPoint = Vector3.zero;
        private Vector3 _targetPoint = Vector3.zero;

        public override void OnSkillCallback()
        {
            if (Owner.IsValid() == false)
            {
                StopCoBodyAttack();
                return;
            }

            if (Owner.Target.IsValid() == false)
            {
                StopCoBodyAttack();
                return;
            }

            StopCoBodyAttack();
            _coBodyAttack = StartCoroutine(CoBodyAttack());
        }
        public override void OnSkillStateEnter() { }
        public override void OnSkillStateExit() { }
        protected override void DoSelfTarget(BaseObject target) 
        { 
            
        }
        protected override void DoSingleTarget(BaseObject target) 
        { 

        }
        protected override void DoHalfTarget(BaseObject target) 
        { 

        }
        protected override void DoAroundTarget(BaseObject target) 
        { 

        }

        private Coroutine _coBodyAttack = null;
        private IEnumerator CoBodyAttack()
        {
            Vector3 originLocalPos = Owner.CreatureAnim.transform.localPosition;
            _delta = 0f;
            // _startPoint = Owner.transform.position;
            // _targetPoint = Owner.Target.CenterPosition;
            _startPoint = Managers.Map.GetCenterWorld(Owner.CellPos);
            _targetPoint = Managers.Map.GetCenterWorld(Owner.Target.CellPos);
            yield return new WaitUntil(() => IsReachedPoint(_startPoint, _targetPoint, _desiredTimeToReach, EAnimationCurveType.Linear));
            // if (Owner.IsValid() == false || Owner.Target.IsValid() == false)
            // {
            //     Owner.CreatureAIState = ECreatureAIState.Idle;
            //     yield break;
            // }

            // if ((Owner.CenterPosition - Owner.Target.CenterPosition).sqrMagnitude < 1.5f)
            //     Owner.Target.OnDamaged(Owner, this);
            // Debug.Log($"sqrMag: {(_startPoint - _targetPoint).sqrMagnitude}");
            if ((_startPoint - _targetPoint).sqrMagnitude <= 3f * 3f)
            {
                if (Owner.Target.IsValid())
                    Owner.Target.OnDamaged(Owner, this);
            }

            _targetPoint = _startPoint;
            _startPoint = Owner.transform.position;
            yield return new WaitUntil(() => IsReachedPoint(_startPoint, _targetPoint, _desiredTimeToReturn, EAnimationCurveType.Ease_Out));
            Owner.CreatureAIState = ECreatureAIState.Idle;
        }

        private void StopCoBodyAttack()
        {
            if (_coBodyAttack != null)
                StopCoroutine(_coBodyAttack);
            
            _coBodyAttack = null;
        }

        private bool IsReachedPoint(Vector3 startPoint, Vector3 targetPoint, float desiredTime, EAnimationCurveType animCurveType)
        {
            if (Owner.IsValid() == false || Owner.Target.IsValid() == false)
                return true;

            _delta += Time.deltaTime;
            float percent = _delta / desiredTime * Owner.MovementSpeed;
            AnimationCurve curve = MonoContentsManager.Instance.Curve(animCurveType);
            Owner.transform.position = Vector3.Lerp(startPoint, targetPoint, curve.Evaluate(percent));
            if (percent >= 1f)
            {
                _delta = 0f;
                Owner.transform.position = targetPoint;
                return true;
            }

            return false;
        }
    }
}
