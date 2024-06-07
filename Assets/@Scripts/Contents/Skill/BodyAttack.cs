using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class BodyAttack : SkillBase
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override bool SetInfo(BaseObject owner, int dataID)
        {
            if (base.SetInfo(owner, dataID) == false)
            {
                EnterInGame(owner, dataID);
                return false;
            }

            EnterInGame(owner, dataID);
            return true;
        }

        protected override void EnterInGame(BaseObject owner, int dataID) // dataID 왜 받았지??
        {
            base.EnterInGame(owner, dataID);
        }

        #region CreatureStateMachine
        public override void OnSkillStateEnter()
        {
        }

        public override void OnSkillStateUpdate()
        {
            // --- DEFENSE
            if (Owner.IsValid() == false)
                return;

            // --- DEFENSE
            if (Owner.Target.IsValid() == false)
                return;

            if (_coBodyAttack != null)
            {
                StopCoroutine(_coBodyAttack);
                _coBodyAttack = null;
            }

            _coBodyAttack = StartCoroutine(CoBodyAttack());
        }

        public override void OnSkillStateEnd()
        {
        }
        #endregion

        #region Coroutines
        private float _delta = 0f;
        private float _desiredTimeToReach = 0.25f;
        private float _desiredTimeToReturn = 0.35f;
        private Vector3 _startPoint = Vector3.zero;
        private Vector3 _targetPoint = Vector3.zero;
        private Coroutine _coBodyAttack = null;
        private IEnumerator CoBodyAttack(System.Action endCallback = null)
        {
            _delta = 0f;
            _startPoint = Owner.transform.position;
            _targetPoint = Owner.Target.CenterPosition;
            yield return new WaitUntil(() => IsReachedPoint(_startPoint, _targetPoint, _desiredTimeToReach));
            
            if (Owner.IsValid() == false || Owner.Target.IsValid() == false)
                yield break;

            if ((Owner.CenterPosition - Owner.Target.CenterPosition).sqrMagnitude < 1f)
                Owner.Target.OnDamaged(Owner, this);

            _targetPoint = _startPoint;
            _startPoint = Owner.transform.position;
            yield return new WaitUntil(() => IsReachedPoint(_startPoint, _targetPoint, _desiredTimeToReturn));
        }

        private bool IsReachedPoint(Vector3 startPoint, Vector3 targetPoint, float desiredTime)
        {
            if (Owner.IsValid() == false || Owner.Target.IsValid() == false)
                return true;

            _delta += Time.deltaTime;
            float percent = _delta / desiredTime * Owner.MovementSpeed;
            Owner.transform.position = Vector3.Lerp(startPoint, targetPoint, percent); // Curve 필요?
            if (percent >= 1f)
            {
                _delta = 0f;
                Owner.transform.position = targetPoint;
                return true;
            }

            return false;
        }

        #endregion
    }
}
