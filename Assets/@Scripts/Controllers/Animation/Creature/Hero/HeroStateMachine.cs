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
        private CreatureAnimation _creatureAnim = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_creatureAnim == null)
            {
                _creatureAnim = animator.GetComponent<CreatureAnimation>();
                _owner = _creatureAnim.GetOwner<Hero>();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float endThreshold = 0.9f;
            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Attack))
            {
                if (stateInfo.normalizedTime >= endThreshold)
                    _owner.CreatureState = ECreatureState.Idle;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}

/*
            CreatureAnimation creatureAnim = animator.GetComponent<CreatureAnimation>();
            if (stateInfo.shortNameHash == creatureAnim.GetHash(ECreatureState.Attack))
                Debug.Log("ENTER");
*/
