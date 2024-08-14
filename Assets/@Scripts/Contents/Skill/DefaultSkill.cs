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
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        #region Events
        public override void OnSkillCallback()
        {
            if (Owner.IsValid() == false)
                return;

            if (_skillTargets.Count == 0)
                return;

            // --- Do Projectile Later
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

        // --- Reverse Targets
        public override void OnSkillStateEnter() 
        {
            if (Owner.IsValid() == false)
                return;

            if (Owner.Target.IsValid() == false)
                return;
            
            _skillTargets.Clear();
            Owner.LookAtValidTarget();
            for (int i = 0; i < Owner.Targets.Count; ++i)
            {
                if (Owner.Targets[i].IsValid() == false)
                    continue;

                BaseObject target = Owner.Targets[i];
                if (target.ObjectType == EObjectType.Env)
                    continue;

                switch (TargetRange)
                {
                    case ESkillTargetRange.Self:
                        break;

                    case ESkillTargetRange.Single:
                        break;

                    case ESkillTargetRange.Half:
                        ReserveHalfTargets(target);
                        break;

                    case ESkillTargetRange.Around:
                        break;
                }
            }

            ReleaseAllTargetDirs();
        }

        public override void OnSkillStateExit() 
        {  
            _skillTargets.Clear();
        }
        #endregion Events
    }
}
