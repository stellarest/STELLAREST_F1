using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BodyAttack : ActiveSkillBase
    {
        private float _delta = 0f;
        private float _desiredTimeToReach = 0.35f;
        private float _desiredTimeToReturn = 0.65f;
        private float _sqrTargetDist = 0f;

        public override void InitialSetInfo(int dataID, BaseCellObject owner)
        {
            base.InitialSetInfo(dataID, owner);
            _sqrTargetDist = TargetDistance * 3f;
        }

        #region Events
        public override bool OnSkillEnter()
            => base.OnSkillEnter();

        public override bool OnSkillCallback()
        {
            if (base.OnSkillCallback() == false)
                return false;

            if (IsValidOwner == false || IsValidTarget == false)
            {
                StopCoBodyAttack();
                return true;
            }

            StopCoBodyAttack();
            _coBodyAttack = StartCoroutine(CoBodyAttack());
            return true;
        }

        public override void OnSkillExit() { }
        #endregion Events

        private Vector3 _startPoint = Vector3.zero;
        private Vector3 _targetPoint = Vector3.zero;
        private Coroutine _coBodyAttack = null;
        private IEnumerator CoBodyAttack()
        {
            //_startPoint = Managers.Map.GetCenterWorld(Owner.CellPos);
            _startPoint = Owner.transform.position;
            _targetPoint = Managers.Map.CellToCenteredWorld(Owner.Target.CellPos);
            yield return new WaitUntil(() => IsReachedPoint(_startPoint, _targetPoint, _desiredTimeToReach, EAnimationCurveType.Linear));
            // if (Owner.IsValid() == false || Owner.Target.IsValid() == false)
            // {
            //     Owner.CreatureAIState = ECreatureAIState.Idle;
            //     yield break;
            // }

            // if ((Owner.CenterPosition - Owner.Target.CenterPosition).sqrMagnitude < 1.5f)
            //     Owner.Target.OnDamaged(Owner, this);
            // Debug.Log($"sqrMag: {(_startPoint - _targetPoint).sqrMagnitude}");

            if ((_startPoint - _targetPoint).sqrMagnitude <= _sqrTargetDist * _sqrTargetDist)
            {
                if (IsValidTarget)
                    Owner.Target.OnDamaged(Owner, this);
            }

            _targetPoint = _startPoint;
            _startPoint = Owner.transform.position;
            
            //_startPoint = Managers.Map.GetCenterWorld(Owner.CellPos);
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
            if (IsValidOwner == false || IsValidTarget == false)
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
