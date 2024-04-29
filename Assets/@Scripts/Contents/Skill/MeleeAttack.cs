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
                EnterInGame();
                return false;
            }

            EnterInGame();
            return true;
        }

        protected override void EnterInGame()
        {
            base.EnterInGame();
        }

        public override void OnSkillStateEnter() { }

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

            Owner.CreatureState = ECreatureState.Idle;
        }
    }
}