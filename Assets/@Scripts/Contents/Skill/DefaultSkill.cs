using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class DefaultSkill : SkillBase
    {
        #region Events
        public override void OnSkillStateEnter()
        {
            if (Owner.IsValid() == false)
                return;

            Owner.LookAtValidTarget();
            if (SkillData.ProjectileID == -1)
                GatherMeleeTargets();
            else
                SetRangedFirstTarget(Owner.Target);
        }

        public override void OnSkillStateExit()
            => _skillTargets.Clear();

        public override void OnSkillCallback()
        {
            if (Owner.IsValid() == false)
                return;

            // --- Handle Projectile
            if (SkillData.ProjectileID > -1)
            {
                // --- 여기까지 들어왔다는건, Target이 존재한다는 의미(또는 존재했었다는 의미)
                Projectile projectile = GenerateProjectile(Owner, GetSpawnPos());
                return;
            }
            // --- Handle Melee Targets
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
                        if (SkillData.EffectIDs.Length != 0)
                        {
                            List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                            effectIDs: SkillData.EffectIDs,
                            spawnPos: Util.GetRandomQuadPosition(target.CenterPosition),
                            startCallback: null
                        );
                        }
                    }
                }
            }
        }
        #endregion Events

        // --- How to gather Allies Targets?
        private void GatherMeleeTargets()
        {
            _skillTargets.Clear();
            ELookAtDirection enteredLookAtDir = Owner.LookAtDir;
            for (int i = 0; i < Owner.Targets.Count; ++i)
            {
                if (Owner.Targets[i].IsValid() == false)
                    continue;

                BaseObject target = Owner.Targets[i];
                if (target.ObjectType == EObjectType.Env)
                    continue;

                switch (TargetRange)
                {
                    case ESkillTargetRange.Single:
                        ReserveSingleTargets(enteredLookAtDir, target);
                        break;

                    case ESkillTargetRange.Half:
                        ReserveHalfTargets(enteredLookAtDir, target);
                        break;

                    case ESkillTargetRange.Around:
                        ReserveAroundTargets(target);
                        break;
                }
            }

            ReleaseAllTargetDirs();
        }

        private void SetRangedFirstTarget(BaseObject target)
            => _skillTargets.Add(target);

        // --- Prev
        // public override void OnSkillCallback()
        // {
        //     if (Owner.IsValid() == false)
        //         return;

        //     if (_skillTargets.Count == 0)
        //         return;

        //     // --- Prev
        //     // --- Generate Projectile
        //     if (SkillData.ProjectileID != -1)
        //     {
        //         // Projectile projectile = GenerateProjectile(Owner, GetSpawnPos());
        //         // projectile.SetInfo(SkillData.ProjectileID, Owner);
        //         Projectile projectile = Managers.Object.SpawnBaseObject<Projectile>
        //             (objectType: EObjectType.Env, spawnPos: GetSpawnPos(), dataID: SkillData.ProjectileID, owner: Owner);
        //         projectile.Owner = Owner;

        //         if (Owner.Targets.Count == 0)
        //             Owner.CreatureAIState = ECreatureAIState.Move;

        //         if (Owner.Target.IsValid() == false)
        //             Owner.CreatureAIState = ECreatureAIState.Move;

        //         return;
        //     }

        //     for (int i = 0; i < Owner.Targets.Count; ++i)
        //     {
        //         BaseObject target = Owner.Targets[i];
        //         if (target.IsValid() == false)
        //             continue;

        //         if (target.ObjectType == EObjectType.Env)
        //             continue;

        //         switch (TargetRange)
        //         {
        //             case ESkillTargetRange.Self:
        //                 DoSelfTarget(target);
        //                 break;

        //             case ESkillTargetRange.Single:
        //                 DoSingleTarget(target);
        //                 break;

        //             case ESkillTargetRange.Half:
        //                 DoHalfTarget(target);
        //                 break;

        //             case ESkillTargetRange.Around:
        //                 DoAroundTarget(target);
        //                 break;
        //         }
        //     }

        //     ReleaseAllTargetDirs();
        // }
    }
}
