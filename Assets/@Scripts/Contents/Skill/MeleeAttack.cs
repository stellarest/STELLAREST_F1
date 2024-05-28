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

        public override void OnSkillStateEnter() 
        { 
            if (Owner.Target.IsValid())
            {
                Owner.LookAtTarget();
            }
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

            // *****
            // 애니메이션 트렌지션을 통해 빠져나오므로, 여기서 Idle로 세팅해주어야한다.
            // (여전히 CreatuteState는 Skill_Attack이기 떄문이다.)
            // 그러나 애니메이션 트렌지션이 씹힐때도 있는 것 같다. 그래서 UpdateSkillState 상태에서도 예외를 주어야할듯.
            // --- DEFENSE (CreatureAnimation::ForceExitState)
            if (Owner.CreatureState != ECreatureState.Idle)
            {
                Owner.CreatureMoveState = ECreatureMoveState.None;
                Owner.CreatureState = ECreatureState.Idle;
            }
        }
    }
}
