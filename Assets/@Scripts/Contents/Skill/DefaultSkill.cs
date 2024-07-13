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
        float AngleThreshHold = 22.5f;
        public override void OnSkillCallback()
        {
            if (Owner.IsValid() == false)
                return;

            if (SkillData.ProjectileID != -1)
            {
                Projectile projectile = GenerateProjectile(Owner, GetSpawnPos());
                projectile.SetInfo(SkillData.ProjectileID, Owner);

                if (Owner.Targets.Count == 0)
                    Owner.CreatureAIState = ECreatureAIState.Move;

                if (Owner.Target.IsValid() == false)
                    Owner.CreatureAIState = ECreatureAIState.Move;

                return;
            }

            for (int i = 0; i < Owner.Targets.Count; ++i)
            {
                if (Owner.Targets[i].IsValid() == false)
                    continue;

                Vector3 nLookDir = new Vector3((int)Owner.LookAtDir, 0, 0);
                Vector3 nTargetDir = (Owner.Targets[i].transform.position - Owner.transform.position).normalized;
                switch (TargetRange)
                {
                    case ESkillTargetRange.None:
                    case ESkillTargetRange.Single:
                        {
                            List<BaseObject> singleTargets = new List<BaseObject>();

                            float dot = Vector3.Dot(nLookDir, nTargetDir);
                            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                            float diagonal = 45f;

                            int dx = Mathf.Abs(Owner.Targets[i].CellPos.x - Owner.CellPos.x);
                            int dy = Mathf.Abs(Owner.Targets[i].CellPos.y - Owner.CellPos.y);

                            bool grabbedStraightTarget = false;
                            bool grabbedDiagonalTarget = false;

                            if (dx <= InvokeRange || dy <= InvokeRange)
                            {
                                if (angle < AngleThreshHold && grabbedDiagonalTarget == false)
                                {
                                    grabbedStraightTarget = true;
                                    singleTargets.Add(Owner.Targets[i]);
                                }
                                // else if (Mathf.Abs(angle - diagonal) < AngleThreshHold && grabbedStraightTarget == false)
                                // {
                                //     grabbedDiagonalTarget = true;
                                //     singleTargets.Add(Owner.Targets[i]);
                                // }
                            }

                            if (dx <= InvokeRange && dy <= InvokeRange)
                            {
                            }
                            
                            // if (dx <= SkillDistance || dy <= SkillDistance)
                            // {
                            //     if (angle < AngleThreshHold)
                            //     {
                            //         Owner.Targets[i].OnDamaged(attacker: Owner, skillFromAttacker: this);
                            //         if (SkillData.EffectIDs.Length != 0)
                            //             Owner.CreatureEffect.GenerateEffect(SkillData.EffectIDs, EEffectSpawnType.Skill, Owner.Targets[i]);
                                    
                            //         break;
                            //     }
                            // }
                            // // ---  대각선
                            // else if (dx <= SkillDistance && dy <= SkillDistance)
                            // {
                            //     if (Mathf.Abs(angle - diagonal) < AngleThreshHold)
                            //     {
                            //         Owner.Targets[i].OnDamaged(attacker: Owner, skillFromAttacker: this);
                            //         if (SkillData.EffectIDs.Length != 0)
                            //             Owner.CreatureEffect.GenerateEffect(SkillData.EffectIDs, EEffectSpawnType.Skill, Owner.Targets[i]);
                            //     }
                            // }
                        }
                        break;

                    case ESkillTargetRange.Half:
                        {
                            if (Vector3.Dot(nLookDir, nTargetDir) >= 0f)
                            {
                                if (nTargetDir.y >= 0 || nTargetDir.y <= 0)
                                {
                                    int dx = Mathf.Abs(Owner.Targets[i].CellPos.x - Owner.CellPos.x);
                                    int dy = Mathf.Abs(Owner.Targets[i].CellPos.y - Owner.CellPos.y);
                                    if (dx <= SkillDistance && dy <= SkillDistance)
                                    {
                                        Owner.Targets[i].OnDamaged(attacker: Owner, skillFromAttacker: this);
                                        if (SkillData.EffectIDs.Length != 0)
                                            Owner.CreatureEffect.GenerateEffect(SkillData.EffectIDs, EEffectSpawnType.Skill, Owner.Targets[i]);
                                    }
                                }
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
        public override void OnSkillStateExit() { }
        public override void OnSkillStateUpdate() { }
    }
}
