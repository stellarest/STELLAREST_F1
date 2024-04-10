using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroStateMachine : StateMachineBehaviour
    {
        private Hero _owner = null;
        private HeroAnimation _heroAnim = null;
        private bool _canGiveDamageFlag = false;

        public event System.Action<ECreatureState> OnHeroAnimUpdateHandler = null;
        public event System.Action<ECreatureState> OnHeroAnimComplatedHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _heroAnim = _heroAnim == null ? animator.GetComponent<HeroAnimation>() : _heroAnim;
            _owner = _owner == null ? _heroAnim.GetOwner<Hero>() : _owner;

            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Attack))
                _canGiveDamageFlag = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Attack) && _canGiveDamageFlag)
            {
                float endThreshold = 0.5f;
                if (stateInfo.normalizedTime >= endThreshold)
                {
                    OnHeroAnimUpdateHandler?.Invoke(ECreatureState.Attack);
                    _canGiveDamageFlag = false;
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_owner?.HeroMoveState == EHeroMoveState.ForceMove)
                return;

            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Attack))
            {
                // Target의 Valid 체크를 하지 않으면, Target이 죽고 다시 한 번 이벤트가 발동되어서 Idle 애니메이션이 한번 더 실행됨
                if (_owner.Target.IsValid())
                    OnHeroAnimComplatedHandler?.Invoke(ECreatureState.Attack);

                _canGiveDamageFlag = false; // TEMP
            }
        }
    }
}
