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
            // Refresh Animation (한 프레임 내에 CreatureState가 변경될 때 애니메이션 갱신이 안될수도 있음)
            // --> ex) 한 프레임 내에 Animator.Play(Move) -> Animator.Play(Idle) 전환이 안되는 경우도 있어서 체크
            if (stateInfo.shortNameHash != _creatureAnim?.GetHash(_owner.CreatureState))
            {
                _creatureAnim.UpdateAnimation();
                return;
            }

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
            }

            if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.CollectEnv))
                _canCollectEnvFlag = false;
        }
    }
}
