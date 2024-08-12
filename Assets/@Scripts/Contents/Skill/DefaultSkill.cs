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
        private float _angleDiagonal = 45f;
        private float _angleVerticalMin = 70f;
        private float _angleVerticalMax = 90f;
        public float angleTreshhold = 10f;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        // --- 등록된 타겟 스킬 적용
        public override void OnSkillCallback()
        {
            if (Owner.IsValidOwner == false)
                return;

            if (_skillTargets.Count == 0)
                return;

            // Do Projectile Later
            for (int i = 0; i < _skillTargets.Count; ++i)
            {
                if (_skillTargets[i].IsValidOwner == false)
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
                        //spawnPos: Util.GetCellRandomQuadPosition(Managers.Map.GetCenterWorld(target.CellPos)),
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

        // --- 미리 타겟 예약(등록)
        public override void OnSkillStateEnter() 
        {
            if (Owner.IsValidOwner == false)
            {
                _skillTargets.Clear();
                return;
            }

            if (Owner.Targets.Count == 0)
            {
                _skillTargets.Clear();
                return;
            }

            for (int i = 0; i < Owner.Targets.Count; ++i)
            {
                if (Owner.Targets[i].IsValidOwner == false)
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
                        GatherHalfTarget(target);
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

        protected override void GatherHalfTarget(BaseObject target)
        {
            Vector3 nLookDir = new Vector3((int)Owner.LookAtDir, 0, 0);
            Vector3 nTargetDir = (target.transform.position - Owner.transform.position).normalized;
            float dot = Vector3.Dot(nLookDir, nTargetDir);
            if (dot < 0) // --- 둔각일때는 종료
                return;

            int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            if (dx <= TargetDistance && dy <= TargetDistance)
            {
                if (nTargetDir.y >= 0 || nTargetDir.y <= 0)
                {
                    _skillTargets.Add(target);
                    // target.OnDamaged(attacker: Owner, skillByAttacker: this);
                    // if (SkillData.EffectIDs.Length != 0)
                    // {
                    //     List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                    //         effectIDs: SkillData.EffectIDs,
                    //         spawnPos: Util.GetRandomQuadPosition(target.CenterPosition),
                    //         //spawnPos: Util.GetCellRandomQuadPosition(Managers.Map.GetCenterWorld(target.CellPos)),
                    //         startCallback: null
                    //     );
                    // }
                }
            }
        }

        protected override void DoSelfTarget(BaseObject target)  {  }
        protected override void DoSingleTarget(BaseObject target) 
        {
            if (target.IsValid() == false)
                return;

            Vector3 nLookDir = new Vector3((int)Owner.LookAtDir, 0, 0);
            Vector3 nTargetDir = (target.transform.position - Owner.transform.position).normalized;
            float dot = Vector3.Dot(nLookDir, nTargetDir);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            if (dx <= TargetDistance && dy <= TargetDistance)
            {
                if (IsLockTargetDir(ETargetDirection.Horizontal) == false)
                {
                    if (target.CellPos.y == Owner.CellPos.y)
                    {
                        if (angle < angleTreshhold)
                        {
                            Debug.Log("<color=white>Horizontal</color>");
                            LockTargetDir(ETargetDirection.Vertical);
                            LockTargetDir(ETargetDirection.Diagonal);;
                            target.OnDamaged(attacker: Owner, skillByAttacker: this);
                            if (SkillData.EffectIDs.Length != 0)
                            {
                                List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                                    effectIDs: SkillData.EffectIDs,
                                    source: target,
                                    startCallback: null
                                );
                            }
                        }
                    }
                }

                if (IsLockTargetDir(ETargetDirection.Vertical) == false)
                {
                    if (target.CellPos.x == Owner.CellPos.x)
                    {
                        if (angle >= _angleVerticalMin && angle <= _angleVerticalMax)
                        {
                            Debug.Log("<color=white>Vertical</color>");
                            LockTargetDir(ETargetDirection.Horizontal);
                            LockTargetDir(ETargetDirection.Diagonal);
                            target.OnDamaged(attacker: Owner, skillByAttacker: this);
                            if (SkillData.EffectIDs.Length != 0)
                            {
                                List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                                    effectIDs: SkillData.EffectIDs,
                                    source: target,
                                    startCallback: null
                                );
                            }
                        }
                    }
                }

                if (IsLockTargetDir(ETargetDirection.Diagonal) == false)
                {
                    if (Mathf.Abs(angle - _angleDiagonal) < angleTreshhold)
                    {
                        Debug.Log("<color=white>Diagonal</color>");
                        LockTargetDir(ETargetDirection.Horizontal);
                        LockTargetDir(ETargetDirection.Vertical);
                        target.OnDamaged(attacker: Owner, skillByAttacker: this);
                        if (SkillData.EffectIDs.Length != 0)
                        {
                            List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                                effectIDs: SkillData.EffectIDs,
                                source: target,
                                startCallback: null
                            );
                        }
                    }
                }
            }
        }
        protected override void DoHalfTarget(BaseObject target)
        {
            Vector3 nLookDir = new Vector3((int)Owner.LookAtDir, 0, 0);
            Vector3 nTargetDir = (target.transform.position - Owner.transform.position).normalized;
            float dot = Vector3.Dot(nLookDir, nTargetDir);
            if (dot < 0) // --- 둔각일때는 종료
                return;

            int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            if (dx <= TargetDistance && dy <= TargetDistance)
            {
                if (nTargetDir.y >= 0 || nTargetDir.y <= 0)
                {
                    target.OnDamaged(attacker: Owner, skillByAttacker: this);
                    if (SkillData.EffectIDs.Length != 0)
                    {
                        List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                            effectIDs: SkillData.EffectIDs,
                            spawnPos: Util.GetRandomQuadPosition(target.CenterPosition),
                            //spawnPos: Util.GetCellRandomQuadPosition(Managers.Map.GetCenterWorld(target.CellPos)),
                            startCallback: null
                        );
                    }
                }
            }
        }
        protected override void DoAroundTarget(BaseObject target)
        {
            int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            if (dx <= TargetDistance && dy <= TargetDistance)
            {
                target.OnDamaged(attacker: Owner, skillByAttacker: this);
                if (SkillData.EffectIDs.Length != 0)
                {
                    List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                        effectIDs: SkillData.EffectIDs,
                            spawnPos: Util.GetRandomQuadPosition(target.CenterPosition),
                        //spawnPos: Util.GetCellRandomQuadPosition(Managers.Map.GetCenterWorld(target.CellPos)),
                        startCallback: null
                    );
                }
            }
        }
    }
}
