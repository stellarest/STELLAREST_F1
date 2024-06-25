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

            // Add ProjectileMotion
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

            // 애니메이션에서 발사하면 무조건 생성하는게 자연스러움.
            Projectile projectile = GenerateProjectile(Owner, GetSpawnPos());
            if (Owner.Target.IsValid() == false)
                Owner.CreatureAIState = ECreatureAIState.Move;
            //projectile.TargetPosition = Owner.Target.CenterPosition;
        }

        public override void OnSkillStateEnter()
        {
            // if (Owner.Target.IsValid() == false)
            //     return;
        }

        public override void OnSkillStateUpdate()
        {
            // if (Owner.IsValid() == false)
            //     return;
            // if (Owner.Target.IsValid() == false)
            //     return;
            // if (Owner.Target.ObjectType == EObjectType.Env)
            //     return;

            // GenerateProjectile(Owner, GetSpawnPos());
        }

        public override void OnSkillStateExit()
        {
            if (Owner.IsValid() == false)
                return;

            //Owner.CreatureAIState = ECreatureAIState.Idle;
        }
    }
}

