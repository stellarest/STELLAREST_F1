using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureStateMachine : StateMachineBehaviour
    {
        private CreatureAnimation _creatureAnim = null;

        public event System.Action<ECreatureAnimState> OnAnimStateEnterHandler = null;
        public event System.Action<ECreatureAnimState> OnAnimStateExitHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_creatureAnim == null)
                _creatureAnim = animator.GetComponent<CreatureAnimation>();

            // --- New
            if (stateInfo.shortNameHash == _creatureAnim.Upper_Idle)
                OnAnimStateEnterHandler?.Invoke(ECreatureAnimState.Upper_Idle);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_Move)
                OnAnimStateEnterHandler?.Invoke(ECreatureAnimState.Upper_Move);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_SkillA)
                OnAnimStateEnterHandler?.Invoke(ECreatureAnimState.Upper_SkillA);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_SkillB)
                OnAnimStateEnterHandler?.Invoke(ECreatureAnimState.Upper_SkillB);

            if (stateInfo.shortNameHash == _creatureAnim.Upper_SkillC)
                OnAnimStateEnterHandler?.Invoke(ECreatureAnimState.Upper_SkillC);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_CollectEnv)
                OnAnimStateEnterHandler?.Invoke(ECreatureAnimState.Upper_CollectEnv);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_Dead)
                OnAnimStateEnterHandler?.Invoke(ECreatureAnimState.Upper_Dead);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_creatureAnim == null)
                _creatureAnim = animator.GetComponent<CreatureAnimation>();

            // --- New
            if (stateInfo.shortNameHash == _creatureAnim.Upper_Idle)
                OnAnimStateExitHandler?.Invoke(ECreatureAnimState.Upper_Idle);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_Move)
                OnAnimStateExitHandler?.Invoke(ECreatureAnimState.Upper_Move);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_SkillA)
                OnAnimStateExitHandler?.Invoke(ECreatureAnimState.Upper_SkillA);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_SkillB)
                OnAnimStateExitHandler?.Invoke(ECreatureAnimState.Upper_SkillB);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_SkillC)
                OnAnimStateExitHandler?.Invoke(ECreatureAnimState.Upper_SkillC);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_CollectEnv)
                OnAnimStateExitHandler?.Invoke(ECreatureAnimState.Upper_CollectEnv);

            else if (stateInfo.shortNameHash == _creatureAnim.Upper_Dead)
                OnAnimStateExitHandler?.Invoke(ECreatureAnimState.Upper_Dead);
        }
    }
}

/*
[ Prev ]
public class CreatureStateMachine : StateMachineBehaviour
    {
        // private Creature _owner = null;
        // private CreatureAnimation _creatureAnim = null;

        // public event System.Action<ECreatureAIState> OnStateEnterHandler = null;
        // public event System.Action<ECreatureAIState> OnStateUpdateHandler = null;
        // public event System.Action<ECreatureAIState> OnStateExitHandler = null;

        public event System.Action<ECreatureAnimState> OnAnimStateEnterHandler = null;
        public event System.Action<ECreatureAnimState> OnAnimStateUpdateHandler = null;
        public event System.Action<ECreatureAnimState> OnAnimStateExitHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == animator.GetComponent<CreatureAnimation>()?.Upper_Idle_To_CollectEnv)
                OnAnimStateEnterHandler?.Invoke(ECreatureAnimState.Upper_Idle_To_CollectEnv);

            // if (_creatureAnim == null)
            // {
            //     _creatureAnim = animator.GetComponent<CreatureAnimation>();
            //     _owner = _creatureAnim.GetOwner<Creature>();
            // }

            // if (stateInfo.shortNameHash == _creatureAnim?.Upper_Idle_To_CollectEnv)
            // {
            //     OnStateEnterHandler?.Invoke(ECreatureAIState.CollectEnv);
            // }



            // if (stateInfo.shortNameHash == _creatureAnim?.IdleHash)
            //     OnStateEnterHandler?.Invoke(ECreatureAIState.Idle);

            // if (stateInfo.shortNameHash == _creatureAnim?.MoveHash)
            // {
            //     if (_owner.CollectEnv)
            //     {
            //         _owner.StopCoActivateCollectEnv();
            //         Debug.Log("<color=cyan>Move State Enter</color>");
            //     }
            // }

            // if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureUpperAnimState.UA_Idle))
            // {
            //     // if (_creatureAnim.IsSkill == (int)ESkillType.Skill_Attack)
            //     // {
            //     //     _creatureAnim.IsSkill = -1; // 트리거로 바꿀까...
            //     //     return;
            //     // }
            // }

            //  if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureUpperAnimState.UA_Move))
            // {
            //     // if (_creatureAnim.IsSkill == (int)ESkillType.Skill_Attack)
            //     // {
            //     //     _creatureAnim.IsSkill = -1; // 트리거로 바꿀까...
            //     //     return;
            //     // }
            // }

            // // --- Idle 또는 Move 상태로 들어오게 되므로 AI State는 자동 동기화
            // if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureUpperAnimState.UA_Idle))
            // {
            //     _owner.CreatureAIState = ECreatureAIState.Idle;
            //     //_owner.CreatureUpperAnimState = ECreatureUpperAnimState.UA_Idle;
            // }
            // else if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureUpperAnimState.UA_Move))
            // {
            //     // 여기 떄문에... 표정이 변하고 있었다.
            //     _owner.CreatureAIState = ECreatureAIState.Move;
            //     //_owner.CreatureUpperAnimState = ECreatureUpperAnimState.UA_Move;
            // }

            // // --- CreatureState, Animation State 동기화
            // if (stateInfo.shortNameHash != _creatureAnim?.GetHash(_owner.CreatureState))
            // {
            //     _creatureAnim.UpdateAnimation();
            //     return;
            // }

            // // Body Att 끊김. Gunner 애니메이션 자연스러움
            // if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureAIState.Idle))
            // {
            //     // Debug.Log($"<color=white>{_owner.gameObject.name}, Set Idle.</color>");
            //     _owner.CreatureState = ECreatureAIState.Idle;
            //     return;
            // }

            // if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureAIState.Skill_Attack))
            // {
            //     OnStateEnterHandler?.Invoke(ECreatureAIState.Skill_Attack); // LookAt Target
            // }
            // else if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureAIState.CollectEnv))
            // {
            //     _canCollectEnvFlag = true;
            //     OnStateEnterHandler?.Invoke(ECreatureAIState.CollectEnv);
            // }
        }

        // --- PREV ON STATE ENTER
        // public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     if (_creatureAnim == null)
        //     {
        //         _creatureAnim = animator.GetComponent<CreatureAnimation>();
        //         _owner = _creatureAnim.GetOwner<Creature>();
        //     }

        //     // --- CreatureState, Animation State 동기화
        //     if (stateInfo.shortNameHash != _creatureAnim?.GetHash(_owner.CreatureState))
        //     {
        //         _creatureAnim.UpdateAnimation();
        //         return;
        //     }

        //     // Body Att 끊김. Gunner 애니메이션 자연스러움
        //     if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureAIState.Idle))
        //     {
        //         // Debug.Log($"<color=white>{_owner.gameObject.name}, Set Idle.</color>");
        //         _owner.CreatureState = ECreatureAIState.Idle;
        //         return;
        //     }

        //     if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureAIState.Skill_Attack))
        //     {
        //         OnStateEnterHandler?.Invoke(ECreatureAIState.Skill_Attack); // LookAt Target
        //     }
        //     else if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureAIState.CollectEnv))
        //     {
        //         _canCollectEnvFlag = true;
        //         OnStateEnterHandler?.Invoke(ECreatureAIState.CollectEnv);
        //     }
        // }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // float currentPercentage = stateInfo.normalizedTime % 1.0f;
            // Skill_Attack Out - 가독성은 이게 더 좋긴함. - 제거 예정
            // if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureState.Skill_Attack))
            // {
            //     // Transition duration에서 뺀 값: 0.90f
            //     if (currentPercentage >= 0.90f)
            //     {
            //         // --- 강제로 빠져나오기
            //         _creatureAnim.AnimationEnd(ECreatureState.Skill_Attack);
            //         return;
            //     }
            // }

            // if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureAIState.CollectEnv))
            // {
            //     float endThresholdPercentage = 0.65f;
            //     if (currentPercentage > endThresholdPercentage && _canCollectEnvFlag == false)
            //     {
            //         _canCollectEnvFlag = true;
            //         OnStateUpdateHandler?.Invoke(ECreatureAIState.CollectEnv);
            //     }
            //     else if (currentPercentage < endThresholdPercentage && _canCollectEnvFlag)
            //         _canCollectEnvFlag = false;
            // }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // --- 필요없을것같기도한데
            // if (stateInfo.shortNameHash == _creatureAnim?.CollectEnvHash)
            //     OnStateExitHandler?.Invoke(ECreatureAIState.CollectEnv);

            // if (_owner?.CreatureMoveState == ECreatureMoveState.ForceMove)
            //     return;

            // if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureAIState.Skill_Attack))
            // {
            //     OnStateEndHandler?.Invoke(ECreatureAIState.Skill_Attack);
            //     return;
            // }

            // if (stateInfo.shortNameHash == _creatureAnim?.GetHash(ECreatureAIState.CollectEnv))
            // {
            //     _canCollectEnvFlag = false;
            //     return;
            // }
        }
    }
*/