using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class DynamicHeroAI : HeroAI
    {
        // #region Background
        // private bool _forceBackStepMovement = false;
        // public override Vector3Int ChaseCellPos
        // {
        //     get
        //     {
        //         if (Owner.ForceMove == false && Owner.Target.IsValid())
        //         {
        //             if (_forceBackStepMovement)
        //             {
        //                 Vector3 nDir = (Owner.transform.position - Owner.Target.transform.position).normalized;
        //                 return GetWorldToCellDest(nDir, Owner.AtkRange);
        //             }
        //             else
        //                 return Owner.Target.CellPos;
        //         }
                
        //         HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
        //         switch (leaderController.HeroMemberFormationMode)
        //         {
        //             case EHeroMemberFormationMode.FollowLeaderClosely:
        //                 return Managers.Map.WorldToCell(leaderController.Leader.transform.position); ;

        //             case EHeroMemberFormationMode.NarrowFormation:
        //             case EHeroMemberFormationMode.WideFormation:
        //                 return leaderController.RequestFormationCellPos(Owner); ;

        //             case EHeroMemberFormationMode.RandomFormation:
        //                 return leaderController.RequestRandomFormationCellPos(Owner);

        //             case EHeroMemberFormationMode.ForceStop:
        //                 return PrevCellPosForForceStop;
        //         }

        //         return Vector3Int.zero;
        //     }
        // }
        // #endregion

        // #region Core
        // public override bool Init()
        // {
        //     if (base.Init() == false)
        //         return false;

        //     return true;
        // }
        // public override void SetInfo(Creature owner) => base.SetInfo(owner);
        // public override void EnterInGame() => base.EnterInGame();
        
        // public override void UpdateIdle()
        // {
        //     if (Owner.IsLeader)
        //         return;

        //     if (Owner.IsValid() == false)
        //         return;

        //     Owner.LookAtValidTarget();
        //     if (Owner.ForceMove == false && Owner.IsValidTargetInAttackRange())
        //     {
        //         if (Owner.Target.ObjectType == EObjectType.Monster)
        //         {
        //             if (Owner.CanSkill)
        //                 Owner.CreatureSkill.CurrentSkill.DoSkill();

        //             Monster target = Owner.Target as Monster;
        //             if (target.IsValidTargetInAttackRange())
        //             {
        //                 _forceBackStepMovement = true;
        //                 Owner.CreatureAIState = ECreatureAIState.Move;
        //                 return;
        //             }
        //         }
        //         else if (Owner.Target.ObjectType == EObjectType.Env)
        //         {
        //             if (Owner.CanCollectEnv)
        //                 Owner.CollectEnv();
        //         }
        //     }
        //     else
        //     {
        //         List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: Owner.CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
        //         if (idlePathFind.Count > 1)
        //         {
        //             if (Managers.Map.CanMove(idlePathFind[1]))
        //             {
        //                 Owner.CreatureAIState = ECreatureAIState.Move;
        //                 return;
        //             }
        //         }
        //     }
        // }

        // public override void UpdateMove()
        // {
        //     if (Owner.IsLeader)
        //         return;

        //     if (Owner.Target.IsValid() && Owner.ForceMove == false)
        //     {
        //         if ((Owner.transform.position - Owner.Target.transform.position).magnitude > Owner.AtkRange)
        //         {
        //             _forceBackStepMovement = false;
        //             Owner.CreatureAIState = ECreatureAIState.Idle;
        //             return;
        //         }
        //     }

        //     EFindPathResult result = Owner.FindPathAndMoveToCellPos(destPos: ChaseCellPos,
        //         maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

        //     if (result == EFindPathResult.Fail_NoPath)
        //     {
        //         Hero leader = Managers.Object.HeroLeaderController.Leader;
        //         if (leader.IsMoving == false)
        //         {
        //             Owner.CreatureAIState = ECreatureAIState.Idle;
        //         }

        //         return;
        //     }

        //     EvadePingPongMovement();
        // }

        // // --- Overwrite Look At Valid Target
        // private void LateUpdate()
        // {
        //     Owner.LookAtValidTarget();
        //     if (Owner.IsValidTargetInAttackRange() && Owner.CanSkill)
        //     {
        //         Owner.CreatureSkill.CurrentSkill.DoSkill();
        //     }
        // }
        // #endregion
    }
}
