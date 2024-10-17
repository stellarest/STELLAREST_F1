using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class DefaultSkill : SkillBase
    {
        #region Events
        public override bool OnSkillStateEnter()
        {
            if (base.OnSkillStateEnter() == false)
                return false;

            Owner.LookAtValidTarget();
            if (SkillData.ProjectileID < 0)
                GatherMeleeTargets();
            else
                SetFirstTargetPos(Owner.Target.CenterPosition);

            return true;
        }

        public override bool OnSkillCallback()
        {
            if (base.OnSkillCallback() == false)
                return false;

            // --- Handle Ranged Targets
            if (SkillData.ProjectileID >= 0)
            {
                Projectile projectile = GenerateProjectile(Owner, GetSpawnPos());
                if (projectile == null)
                {
                    // --- 프로젝타일 객체가 없는 즉발성 원거리 스킬
                    // --- TODO: Target이 존재하지 않을 경우, Target이 있었던 CellPos에 Effect를 남기면 될지?
                    Owner.Target.OnDamaged(Owner, this);
                    // --- Generate Effect
                }
            }
            // --- Handle Melee Targets
            else
            {
                for (int i = 0; i < _skillTargets.Count; ++i)
                {
                    if (_skillTargets[i].IsValid() == false)
                        continue;

                    BaseCellObject target = _skillTargets[i];
                    target?.OnDamaged(attacker: Owner, skillByAttacker: this);
                }
            }

            return true;
        }

        public override void OnSkillStateExit()
            => base.OnSkillStateExit();
        #endregion Events

    }
}
