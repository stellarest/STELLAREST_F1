using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MeleeAttack : SkillBase
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override bool SetInfo(BaseObject owner, int dataID)
        {
            if (base.SetInfo(owner, dataID) == false)
            {
                EnterInGame(owner, dataID);
                return false;
            }

            EnterInGame(owner, dataID);
            return true;
        }

        protected override void EnterInGame(BaseObject owner, int dataID)
        {
            base.EnterInGame(owner, dataID);
        }

        #region CreatureStateMachine
        public override void OnSkillStateEnter() 
        { 
            if (Owner.Target.IsValid())
                Owner.LookAtValidTarget();
        }

        public override void OnSkillStateUpdate()
        {
            if (Owner.Target.IsValid() == false)
                return;

            Owner.Target.OnDamaged(Owner, this);
        }

        public override void OnSkillStateEnd()
        {
            if (Owner.IsValid() == false)
                return;

            /*
                - 공격 애니메이션은 Idle Transition으로 연결되어 있어서 크리처 상태 업데이트도 해줘야함.
                - 그러나 CreatureStateMachine에서 ForceExitState로 인해 이미 None, Idle로 처리하는 것으로 변경.
                - 그래서, 만약 알 수 없는 이유로 인해 ForceExitState를 호출하지 못했을 때 여기서 수행.
                - DEFENSE CODE
            */
            if (Owner.CreatureState != ECreatureState.Idle) // --- DEFENSE
            {
                Owner.CreatureMoveState = ECreatureMoveState.MoveToTarget;
                Owner.CreatureState = ECreatureState.Idle;
            }
        }
        #endregion
    }
}
