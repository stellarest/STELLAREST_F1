using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroStateMachine : StateMachineBehaviour
    {
        private Hero _owner = null;
        private HeroAnimation _heroAnim = null;
        private bool _canGiveDamageFlag = false;
        private bool _canCollectEnvFlag = false;

        public event System.Action<ECreatureState> OnHeroAnimUpdateHandler = null;
        public event System.Action<ECreatureState> OnHeroAnimCompletedHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _heroAnim = _heroAnim == null ? animator.GetComponent<HeroAnimation>() : _heroAnim;
            _owner = _owner == null ? _heroAnim.GetOwner<Hero>() : _owner;

            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Skill_Attack))
                _canGiveDamageFlag = true;

            // if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.CollectEnv))
            //     _canCollectEnvFlag = true;
        }


        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float currentPercentage = stateInfo.normalizedTime % 1.0f;
            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Skill_Attack) && _canGiveDamageFlag)
            {
                float endThresholdPercentage = 0.5f;
                if (currentPercentage >= endThresholdPercentage)
                {
                    OnHeroAnimUpdateHandler?.Invoke(ECreatureState.Skill_Attack);
                    _canGiveDamageFlag = false;
                }
                return;
            }

            // ECreatureState.CollectEnv : 트렌지션 전환이 없는 단일 애니메이션
            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.CollectEnv))
            {
                float endThresholdPercentage = 0.65f;
                if (currentPercentage > endThresholdPercentage && _canCollectEnvFlag == false)
                {
                    _canCollectEnvFlag = true;
                    OnHeroAnimUpdateHandler?.Invoke(ECreatureState.CollectEnv);
                }
                else if (currentPercentage < endThresholdPercentage && _canCollectEnvFlag)
                    _canCollectEnvFlag = false;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_owner?.CreatureMoveState == ECreatureMoveState.ForceMove)
                return;

            // Skill_Attack -> Idle (Has Exit Time)
            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Skill_Attack))
            {
                OnHeroAnimCompletedHandler?.Invoke(ECreatureState.Skill_Attack);
                _canGiveDamageFlag = false;
            }

            if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.CollectEnv))
                _canCollectEnvFlag = false;
        }
    }
}
