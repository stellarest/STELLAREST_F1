using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class ArcherAI : HeroAI
    {
        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfo(Creature owner) => base.SetInfo(owner);

        public override void EnterInGame() => base.EnterInGame();

        public override void UpdateIdle()
        {
            if (Owner.IsLeader)
                return;

            if (Owner.IsValid() == false)
                return;

            Owner.LookAtValidTarget();
            if (Owner.ForceMove == false && Owner.IsInTargetAttackRange())
            {
                if (Owner.Target.ObjectType == EObjectType.Monster)
                {
                    if (Owner.CanSkill(ESkillType.Skill_Attack))
                        Owner.CreatureSkill.CurrentSkill.DoSkill();
                }
                else if (Owner.Target.ObjectType == EObjectType.Env)
                {
                    if (Owner.CanCollectEnv)
                        Owner.CollectEnv();
                }
            }
            else
            {
                // Hero leader = Managers.Object.HeroLeaderController.Leader;
                // if ((Owner.transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
                // {
                //     Debug.Log("111");
                //     Owner.CreatureAIState = ECreatureAIState.Move;
                //     return;
                // }

                List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: Owner.CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
                if (idlePathFind.Count > 1)
                {
                    if (Managers.Map.CanMove(idlePathFind[1]))
                    {
                        Debug.Log("222");
                        Owner.CreatureAIState = ECreatureAIState.Move;
                        return;
                    }
                }
            }
        }

        public override void UpdateMove()
        {
            if (Owner.IsLeader)
                return;

            if (Owner.IsValid() == false)
                return;

            if (Owner.ForceMove == false && Owner.IsInTargetAttackRange())
            {
                Owner.CreatureAIState = ECreatureAIState.Idle;
                return;
            }

            EFindPathResult result = Owner.FindPathAndMoveToCellPos(destPos: ChaseCellPos,
                maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

            if (result == EFindPathResult.Fail_NoPath)
            {
                Hero leader = Managers.Object.HeroLeaderController.Leader;
                if (leader.IsMoving == false)
                {
                    Debug.Log("111-111");
                    Owner.CreatureAIState = ECreatureAIState.Idle;
                }

                return;
            }
        }
        #endregion
    }
}
