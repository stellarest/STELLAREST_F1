using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MeleeAttack : SkillBase
    {
        // public override float RemainCoolTime 
        // { 
        //     get => base.RemainCoolTime; 
        //     set
        //     {
        //         base.RemainCoolTime = value;
        //         if (base.RemainCoolTime == 0f)
        //             Owner.CreatureAnim.CanSkillAttack = true;
        //         else
        //             Owner.CreatureAnim.CanSkillAttack = false;
        //     }
        // }

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

        #region Anim Clip Callback
        public override void OnSkillCallback()
        {
            if (Owner.IsValid() == false)
                return;

            if (Owner.Target.IsValid() == false)
                return;

            if (Owner.Target.ObjectType != EObjectType.Monster)
                return;

            Owner.Target.OnDamaged(Owner, this);
        }
        #endregion

        #region Anim State Events
        public override void OnSkillStateEnter() 
        {
            // Owner.LookAtValidTarget();
        }

        public override void OnSkillStateUpdate()
        {
            // if (Owner.IsValid() == false)
            //     return;

            // Owner.LookAtValidTarget();
        }

        public override void OnSkillStateExit() 
        {             
            // Owner.CreatureAnim.ReleaseAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A);
            // Owner.CreatureAnim.ReleaseAnimState(ECreatureAnimState.Upper_Move_To_Skill_A);
            // Debug.Log($"<color=white>{nameof(OnSkillStateExit)}</color>");
        }
        #endregion
    }
}
