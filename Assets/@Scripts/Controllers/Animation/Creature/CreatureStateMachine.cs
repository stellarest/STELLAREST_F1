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
        private bool _canSkillAttackFlag = false;
        private bool _canCollectEnvFlag = false;

        public event System.Action<ECreatureState> OnStateEnterHandler = null;
        public event System.Action<ECreatureState> OnStateUpdateHandler = null;
        public event System.Action<ECreatureState> OnStateEndHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_creatureAnim == null)
            {
                _creatureAnim = animator.GetComponent<CreatureAnimation>();
                _owner = _creatureAnim.GetOwner<Creature>();
            }

            // CreatureState, Animation State 동기화
            if (stateInfo.shortNameHash != _creatureAnim?.GetHash(_owner.CreatureState))
            {
                _creatureAnim.UpdateAnimation();
                return;
            }

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Skill_Attack))
            {
                _canSkillAttackFlag = true;
                OnStateEnterHandler?.Invoke(ECreatureState.Skill_Attack);
            }
            else if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.CollectEnv))
            {
                _canCollectEnvFlag = true;
                OnStateEnterHandler?.Invoke(ECreatureState.CollectEnv);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float currentPercentage = stateInfo.normalizedTime % 1.0f;
            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Skill_Attack) && _canSkillAttackFlag)
            {
                float invokeRatio = _owner.CreatureSkill.GetInvokeRatio(ECreatureState.Skill_Attack);
                if (currentPercentage >= invokeRatio)
                {
                    OnStateUpdateHandler?.Invoke(ECreatureState.Skill_Attack);
                    _canSkillAttackFlag = false;
                }
                return;
            }
            
            // 가독성은 이게 더 좋긴함.
            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Skill_Attack))
            {
                // Transition duration에서 뺀 값: 0.90f
                if (currentPercentage >= 0.90f)
                {
                    // 강제로 빠져나오기
                    _creatureAnim.AnimationEnd(ECreatureState.Skill_Attack);
                    return;
                }
            }

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.CollectEnv))
            {
                float endThresholdPercentage = 0.65f;
                if (currentPercentage > endThresholdPercentage && _canCollectEnvFlag == false)
                {
                    _canCollectEnvFlag = true;
                    OnStateUpdateHandler?.Invoke(ECreatureState.CollectEnv);
                }
                else if (currentPercentage < endThresholdPercentage && _canCollectEnvFlag)
                    _canCollectEnvFlag = false;

                return;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_owner?.CreatureMoveState == ECreatureMoveState.ForceMove)
                return;

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Skill_Attack))
            {
                OnStateEndHandler?.Invoke(ECreatureState.Skill_Attack);
                _canSkillAttackFlag = false;
                return;
            }

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.CollectEnv))
            {
                _canCollectEnvFlag = false;
                return;
            }
        }
    }
}
