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
        public event System.Action<ECreatureState> OnMonsterAnimationComplatedHandler = null;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _monsterAnim = _monsterAnim == null ? animator.GetComponent<MonsterAnimation>() : _monsterAnim;
            _owner = _owner == null ? _monsterAnim.GetOwner<Monster>() : _owner;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _monsterAnim?.GetHash(ECreatureState.Attack))
                OnMonsterAnimationComplatedHandler?.Invoke(ECreatureState.Attack);
        }
    }
}
