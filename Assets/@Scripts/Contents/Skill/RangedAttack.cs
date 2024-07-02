using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // Projectile 투사체가 있을 수도 있고, 없을 수도 있음. (ex)그브 산탄. 애니메이션 발동시 즉시 시전(투사체 오브젝트 없이)
    public class RangedAttack : SkillBase
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

            return true;
        }

        protected override void EnterInGame(BaseObject owner, int dataID)
        {
            base.EnterInGame(owner, dataID);
        }

        public override void OnSkillCallback()
        {
            if (Owner.IsValid() == false)
                return;

            Projectile projectile = GenerateProjectile(Owner, GetSpawnPos());
            projectile.SetInfo(Owner, SkillData.ProjectileID);

            if (Owner.Target.IsValid() == false)
                Owner.CreatureAIState = ECreatureAIState.Move;
        }

        public override void OnSkillStateEnter() { }
        public override void OnSkillStateUpdate() { }
        public override void OnSkillStateExit() { }
    }
}

