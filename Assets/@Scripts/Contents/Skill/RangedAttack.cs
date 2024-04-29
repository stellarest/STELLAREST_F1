using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // Projectile 투사체가 있을 수도 있고, 없을 수도 있음. (ex)그브 산탄. 애니메이션 발동시 즉시 시전(투사체 오브젝트 없이)
    public class RangedAttack : SkillBase
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

            // Add ProjectileMotion
            

            return true;
        }

        protected override void EnterInGame()
        {
            base.EnterInGame();
        }

        public override void OnSkillStateEnter()
        {
            // if (Owner.Target.IsValid() == false)
            //     return;
        }

        public override void OnSkillStateUpdate()
        {
            GenerateProjectile(Owner, Owner.CenterPosition);
        }

        public override void OnSkillStateEnd()
        {
            if (Owner.IsValid() == false)
                return;

            Owner.CreatureState = ECreatureState.Idle;
        }
    }
}

