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

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_owner == null && _heroAnim == null)
            {
                _heroAnim = animator.GetComponent<HeroAnimation>();
                _owner = _heroAnim.GetOwner<Hero>();
            }
        }

        // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     // float endThreshold = 0.9f;
        //     // if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Attack))
        //     // {
        //     //     if (stateInfo.normalizedTime >= endThreshold)
        //     //         _owner.CreatureState = ECreatureState.Idle;
        //     // }
        // }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Idle 자동
            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Attack))
            {
                _owner.CreatureState = ECreatureState.Idle;
                Debug.Log("ATTACK TO IDLE.");
            }
        }
    }
}

/*
            CreatureAnimation creatureAnim = animator.GetComponent<CreatureAnimation>();
            if (stateInfo.shortNameHash == creatureAnim.GetHash(ECreatureState.Attack))
                Debug.Log("ENTER");
*/
