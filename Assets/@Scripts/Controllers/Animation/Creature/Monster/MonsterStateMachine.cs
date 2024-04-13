using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // Obsolute
    public class MonsterStateMachine : StateMachineBehaviour
    {
        private Monster _owner = null;
        private MonsterAnimation _monsterAnim = null;
        private bool _canGiveDamageFlag = false;

        public event System.Action<ECreatureState> OnMonsterAnimUpdateHandler = null;
        public event System.Action<ECreatureState> OnMonsterAnimCompletedHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _monsterAnim = _monsterAnim == null ? animator.GetComponent<MonsterAnimation>() : _monsterAnim;
            _owner = _owner == null ? _monsterAnim.GetOwner<Monster>() : _owner;

            if (stateInfo.shortNameHash == _monsterAnim?.GetHash(ECreatureState.Skill_Attack))
                _canGiveDamageFlag = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float currentPercentage = stateInfo.normalizedTime % 1.0f;
            if (stateInfo.shortNameHash == _monsterAnim?.GetHash(ECreatureState.Skill_Attack) && _canGiveDamageFlag)
            {
                float endThresholdPercentage = 0.5f;
                if (currentPercentage >= endThresholdPercentage)
                {
                    OnMonsterAnimUpdateHandler?.Invoke(ECreatureState.Skill_Attack);
                    _canGiveDamageFlag = false;
                    return;
                }
            }

            if (stateInfo.shortNameHash == _monsterAnim?.GetHash(ECreatureState.Dead))
            {
                float endThresholdPercentage = 0.9f;
                if (currentPercentage >= endThresholdPercentage)
                    OnMonsterAnimUpdateHandler?.Invoke(ECreatureState.Dead);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Skill_Attack -> Idle (Has Exit Time)
            if (stateInfo.shortNameHash == _monsterAnim?.GetHash(ECreatureState.Skill_Attack))
            {
                OnMonsterAnimCompletedHandler?.Invoke(ECreatureState.Skill_Attack);
                _canGiveDamageFlag = false; // temp
            }
        }
    }
}
