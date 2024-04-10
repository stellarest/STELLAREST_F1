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
        public event System.Action<ECreatureState> OnMonsterAnimComplatedHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _monsterAnim = _monsterAnim == null ? animator.GetComponent<MonsterAnimation>() : _monsterAnim;
            _owner = _owner == null ? _monsterAnim.GetOwner<Monster>() : _owner;

            if (stateInfo.shortNameHash == _monsterAnim?.GetHash(ECreatureState.Attack))
                _canGiveDamageFlag = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _monsterAnim?.GetHash(ECreatureState.Attack) && _canGiveDamageFlag)
            {
                float endThreshold = 0.5f;
                if (stateInfo.normalizedTime >= endThreshold)
                {
                    OnMonsterAnimUpdateHandler?.Invoke(ECreatureState.Attack);
                    _canGiveDamageFlag = false;
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _monsterAnim?.GetHash(ECreatureState.Attack))
            {
                if (_owner.Target.IsValid())
                    OnMonsterAnimComplatedHandler?.Invoke(ECreatureState.Attack);

                _canGiveDamageFlag = false; // temp
            }
        }
    }
}
