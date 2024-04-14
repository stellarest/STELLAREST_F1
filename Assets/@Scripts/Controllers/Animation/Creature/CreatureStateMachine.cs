using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureStateMachine : StateMachineBehaviour
    {
        private Creature _owner = null;
        private CreatureAnimation _creatureAnim = null;
        private bool _canGiveDamageFlag = false;
        private bool _canCollectEnvFlag = false;

        public event System.Action<ECreatureState> OnAnimUpdateHandler = null;
        public event System.Action<ECreatureState> OnAnimCompletedHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _creatureAnim = _creatureAnim == null ? animator.GetComponent<CreatureAnimation>() : _creatureAnim;
            _owner = _owner == null ? _creatureAnim.GetOwner<Creature>() : _owner;

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Skill_Attack))
                _canGiveDamageFlag = true;

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.CollectEnv))
                _canCollectEnvFlag = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float currentPercentage = stateInfo.normalizedTime % 1.0f;
            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Skill_Attack) && _canGiveDamageFlag)
            {
                float endThresholdPercentage = 0.5f;
                if (currentPercentage >= endThresholdPercentage)
                {
                    OnAnimUpdateHandler?.Invoke(ECreatureState.Skill_Attack);
                    _canGiveDamageFlag = false;
                }
                return;
            }

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.CollectEnv))
            {
                float endThresholdPercentage = 0.65f;
                if (currentPercentage > endThresholdPercentage && _canCollectEnvFlag == false)
                {
                    _canCollectEnvFlag = true;
                    OnAnimUpdateHandler?.Invoke(ECreatureState.CollectEnv);
                }
                else if (currentPercentage < endThresholdPercentage && _canCollectEnvFlag)
                    _canCollectEnvFlag = false;
            }

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Dead))
            {
                float endThresholdPercentage = 0.9f;
                if (currentPercentage >= endThresholdPercentage)
                    OnAnimUpdateHandler?.Invoke(ECreatureState.Dead);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_owner?.CreatureMoveState == ECreatureMoveState.ForceMove)
                return;

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Skill_Attack))
            {
                OnAnimCompletedHandler?.Invoke(ECreatureState.Skill_Attack);
                _canGiveDamageFlag = false;
            }

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.CollectEnv))
                _canCollectEnvFlag = false;
        }
    }
}