using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BodyAttack : SkillBase
    {
        // public override float RemainCoolTime
        // {
        //     get => base.RemainCoolTime;
        //     set
        //     {
        //         base.RemainCoolTime = value;
        //         if (base.RemainCoolTime == 0f)
        //             Owner.CreatureAnim.CanSkillAttack = true;
        //         else
        //             Owner.CreatureAnim.CanSkillAttack = false;
        //     }
        // }

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

        #region Coroutines
        private float _delta = 0f;
        // private float _desiredTimeToReach = 0.25f;
        private float _desiredTimeToReach = 0.35f;
        // private float _desiredTimeToReach = 1.25f; // -- InAtkRange Test

        private float _desiredTimeToReturn = 0.35f;
        //private float _desiredTimeToReturn = 1.35f;

        private Vector3 _startPoint = Vector3.zero;
        private Vector3 _targetPoint = Vector3.zero;

        private Coroutine _coBodyAttack = null;
        private IEnumerator CoBodyAttack(System.Action endCallback = null)
        {
            _delta = 0f;
            _startPoint = Owner.transform.position;
            _targetPoint = Owner.Target.CenterPosition;
            yield return new WaitUntil(() => IsReachedPoint(_startPoint, _targetPoint, _desiredTimeToReach, EAnimationCurveType.Linear));
            
            if (Owner.IsValid() == false || Owner.Target.IsValid() == false)
                yield break;

            if ((Owner.CenterPosition - Owner.Target.CenterPosition).sqrMagnitude < 1f)
                Owner.Target.OnDamaged(Owner, this);

            _targetPoint = _startPoint;
            _startPoint = Owner.transform.position;
            yield return new WaitUntil(() => IsReachedPoint(_startPoint, _targetPoint, _desiredTimeToReturn, EAnimationCurveType.Ease_Out));
            Owner.CreatureAIState = ECreatureAIState.Idle;
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
        #endregion

        #region Anim Clip Callback
        public override void OnSkillCallback()
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

            //Owner.UpdateCellPos();
            //Debug.Log($"<color=magenta>Called from {Owner.gameObject.name}</color>");
            _coBodyAttack = StartCoroutine(CoBodyAttack());
        }
        #endregion

        #region Anim State Events
        public override void OnSkillStateEnter()
        {
            Owner.StopCoLerpToCellPos(); // Body Attack은 이게 맞는듯
            Owner.UpdateCellPos(); // --- DEFENSE
        }

        public override void OnSkillStateUpdate() { }
        public override void OnSkillStateExit() { }
        #endregion
    }
}
