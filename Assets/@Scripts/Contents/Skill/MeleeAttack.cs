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

        public override bool SetInfo(Creature owner, int dataID)
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

        // Called From UpdateSkill
        public override void DoSkill()
        {
            base.DoSkill();
            Owner.LookAtTarget(Owner.Target);
        }

        public override void OnSkillAnimationEnter()
        {
        }

        public override void OnSkillAnimationUpdate()
        {
            if (Owner.Target.IsValid() == false)
                return;

            Owner.Target.OnDamaged(Owner, this);
        }

        public override void OnSkillAnimationCompleted()
        {
            if (Owner.IsValid() == false)
                return;

            Owner.CreatureState = ECreatureState.Idle;
            Debug.Log("I'M IDLE...");
        }
    }
}
