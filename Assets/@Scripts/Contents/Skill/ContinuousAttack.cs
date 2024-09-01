using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class ContinuousAttack : SkillBase
    {
        private Vector3 _enteredStartPos = Vector3.zero;
        private Vector3 _enteredDir = Vector3.zero;
        private int _enteredSignX = 0;
        private int _enteredSignY = 0;

        #region Events
        public override bool OnSkillStateEnter()
        {
            if (base.OnSkillStateEnter() == false)
                return false;

            _enteredStartPos = Owner.Target.CenterPosition;
            _enteredDir = Owner.Target.CellPos - Owner.CellPos;
            _enteredSignX = (Owner.LookAtDir == Define.ELookAtDirection.Left) ? 1 : 0;

            Owner.LookAtValidTarget();
            if (SkillData.ProjectileID < 0)
                GatherMeleeTargets();
            else
                SetFirstTargetPos(Owner.Target.CenterPosition);

            return true;
        }

        public override void OnSkillCallback()
        {
            if (Owner.CreatureSkill.CurrentSkillType != SkillType)
            {
                Debug.Log("No ContinuousAttack.");
                return;
            }

            // Debug.Log($"OnSkill: {SkillType}");
            if (SkillData.EnterEffectIDs.Length != 0)
            {
                List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                    effectIDs: SkillData.EnterEffectIDs,
                    spawnPos: Owner.CenterPosition,
                    startCallback: () => Managers.Contents.ActivateEffects(Owner.BaseEffect, _enteredStartPos, _enteredDir, _enteredSignX)
                );
            }

            // --- Handle: Ranged Targets
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
            // --- Handle: Melee Targets
            else
            {
                for (int i = 0; i < _skillTargets.Count; ++i)
                {
                    if (_skillTargets[i].IsValid() == false)
                        continue;

                    BaseObject target = _skillTargets[i];
                    target.OnDamaged(attacker: Owner, skillByAttacker: this);
                    {
                        // --- Effect
                        if (SkillData.HitEffectIDs.Length != 0)
                        {
                            List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                                effectIDs: SkillData.HitEffectIDs,
                                spawnPos: Util.GetRandomQuadPosition(target.CenterPosition),
                                startCallback: null
                            );
                        }
                    }
                }
            }
        }

        public override void OnSkillStateExit()
            => base.OnSkillStateExit();
        #endregion Events
    }
}
