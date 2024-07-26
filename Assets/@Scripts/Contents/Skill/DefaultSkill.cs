using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class DefaultSkill : SkillBase
    {
        public float angleTreshhold = 10f;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void OnSkillCallback()
        {
            if (Owner.IsValid() == false)
                return;

            // --- Generate Projectile
            if (SkillData.ProjectileID != -1)
            {
                // Projectile projectile = GenerateProjectile(Owner, GetSpawnPos());
                // projectile.SetInfo(SkillData.ProjectileID, Owner);
                Projectile projectile = Managers.Object.SpawnBaseObject<Projectile>
                    (objectType: EObjectType.Env, spawnPos: GetSpawnPos(), dataID: SkillData.ProjectileID, owner: Owner);
                projectile.Owner = Owner;

                if (Owner.Targets.Count == 0)
                    Owner.CreatureAIState = ECreatureAIState.Move;

                if (Owner.Target.IsValid() == false)
                    Owner.CreatureAIState = ECreatureAIState.Move;

                return;
            }

            bool lockHorizontal = false;
            bool lockVertical = false;
            bool lockDiagonal = false;
            for (int i = 0; i < Owner.Targets.Count; ++i)
            {
                BaseObject target = Owner.Targets[i];
                if (target.IsValid() == false)
                    continue;

                if (target.ObjectType == EObjectType.Env)
                    continue;

                Vector3 nLookDir = new Vector3((int)Owner.LookAtDir, 0, 0);
                Vector3 nTargetDir = (target.transform.position - Owner.transform.position).normalized;
                float dot = Vector3.Dot(nLookDir, nTargetDir);
                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                float diagonal = 45f;

                int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
                int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
                switch (TargetRange)
                {
                    case ESkillTargetRange.None:
                    case ESkillTargetRange.Single:
                        {
                            if (dx <= SkillDistance && dy <= SkillDistance)
                            {
                                if (lockHorizontal == false)
                                {
                                    if (target.CellPos.y == Owner.CellPos.y)
                                    {
                                        if (angle < angleTreshhold)
                                        {
                                            Debug.Log("<color=white>Horizontal</color>");
                                            lockVertical = true;
                                            lockDiagonal = true;
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
                                
                                if (lockVertical == false)
                                {
                                    if (target.CellPos.x == Owner.CellPos.x)
                                    {
                                        if (angle >= 70f && angle <= 90f)
                                        {
                                            Debug.Log("<color=white>Vertical</color>");
                                            lockHorizontal = true;
                                            lockDiagonal = true;
                                            target.OnDamaged(attacker: Owner, skillByAttacker: this);
                                            if (SkillData.EffectIDs.Length != 0)
                                            {
                                                List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                                                    effectIDs: SkillData.EffectIDs,
                                                    source: target,
                                                    startCallback: null
                                                );
                                            }

                                            // --- Prev
                                            // if (SkillData.EffectIDs.Length != 0)
                                            //     Owner.BaseEffect.GenerateEffects(SkillData.EffectIDs, EEffectSpawnType.Internal, target);
                                        }
                                    }
                                }

                                if (lockDiagonal == false)
                                {
                                    if (Mathf.Abs(angle - diagonal) < angleTreshhold)
                                    {
                                        Debug.Log("<color=white>Diagonal</color>");
                                        lockHorizontal = true;
                                        lockVertical = true;
                                        target.OnDamaged(attacker: Owner, skillByAttacker: this);
                                        if (SkillData.EffectIDs.Length != 0)
                                        {
                                            List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                                                effectIDs: SkillData.EffectIDs,
                                                source: target,
                                                startCallback: null
                                            );
                                        }

                                        // if (SkillData.EffectIDs.Length != 0)
                                        //     Owner.BaseEffect.GenerateEffects(SkillData.EffectIDs, EEffectSpawnType.Internal, target);
                                    }
                                }

                                // else if (CanEnterDir(EGameDir.Vertical) && Owner.CellPos.x == Owner.Targets[i].CellPos.x)
                                // {
                                //     LockEnterDir(EGameDir.Horizontal);
                                //     LockEnterDir(EGameDir.Diagonal);
                                //     Owner.Targets[i].OnDamaged(attacker: Owner, skillFromAttacker: this);
                                //     if (SkillData.EffectIDs.Length != 0)
                                //         Owner.CreatureEffect.GenerateEffect(SkillData.EffectIDs, EEffectSpawnType.Skill, Owner.Targets[i]);
                                // }
                                // else if (CanEnterDir(EGameDir.Diagonal))
                                // {
                                //     LockEnterDir(EGameDir.Horizontal);
                                //     LockEnterDir(EGameDir.Vertical);
                                //     float angle = Mathf.Acos(Vector3.Dot(nLookDir, nTargetDir)) * Mathf.Rad2Deg;
                                //     float diagonal = 45f;
                                //     float angleTreshhold = 22.5f; 
                                //     if (Mathf.Abs(angle - diagonal) < angleTreshhold)
                                //     {
                                //         Owner.Targets[i].OnDamaged(attacker: Owner, skillFromAttacker: this);
                                //         if (SkillData.EffectIDs.Length != 0)
                                //             Owner.CreatureEffect.GenerateEffect(SkillData.EffectIDs, EEffectSpawnType.Skill, Owner.Targets[i]);
                                //     }
                                // }
                            }
                        }
                        break;

                    case ESkillTargetRange.Half:
                        {
                            Debug.Log($"DOT: {Vector3.Dot(nLookDir, nTargetDir)}");
                            if (Vector3.Dot(nLookDir, nTargetDir) >= 0f)
                            {
                                // --- 1사분면, 4사분면에 들어오는 타겟
                                if (nTargetDir.y >= 0 || nTargetDir.y <= 0)
                                {
                                    target.OnDamaged(attacker: Owner, skillByAttacker: this);
                                    if (SkillData.EffectIDs.Length != 0)
                                    {
                                        List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                                            effectIDs: SkillData.EffectIDs,
                                            spawnPos: Util.GetRandomCellQuadPosition(Managers.Map.GetCenterWorld(target.CellPos)),
                                            startCallback: null
                                        );
                                    }
                                }
                                else
                                {
                                    Debug.Log("11");
                                }
                            }
                            else
                            {
                                Debug.Log("22");
                            }
                        }
                        break;
                }

                // --- Prev
                // --- 간헐적으로 한번 더 공격할 때가 있음. 나중에 발생하면 한번 더 체크
                if (Owner.Target.IsValid() == false)
                    Owner.CreatureAIState = ECreatureAIState.Move;
            }
        }

        public override void OnSkillStateEnter() { }
        public override void OnSkillStateUpdate() { }
        public override void OnSkillStateExit() { }
    }
}
