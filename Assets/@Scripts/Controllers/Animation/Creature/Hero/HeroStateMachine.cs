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
        public event System.Action<ECreatureState> OnHeroAnimationComplatedHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _heroAnim = _heroAnim == null ? animator.GetComponent<HeroAnimation>() : _heroAnim;
            _owner = _owner == null ? _heroAnim.GetOwner<Hero>() : _owner;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_owner?.HeroMoveState == EHeroMoveState.ForceMove)
                return;

            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Idle))
                OnHeroAnimationComplatedHandler?.Invoke(ECreatureState.Idle);

            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Move))
                OnHeroAnimationComplatedHandler?.Invoke(ECreatureState.Move);

            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Attack))
                OnHeroAnimationComplatedHandler?.Invoke(ECreatureState.Attack);
        }
    }
}
