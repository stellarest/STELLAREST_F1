using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class RangedHeroAI : HeroAI
    {
        #region Background
        [SerializeField] private bool _tryBackStep = false;
        public override Vector3Int ChaseCellPos
        {
            get
            {
                // --- Owner.ForceMove == false는 계속 false로 잠그고 있어야함.
                // --- 기본적으로 Leader의 Position을 쫓는다.
                if (Owner.ForceMove == false && Owner.Target.IsValid())
                {
                    if (_tryBackStep)
                    {
                        int dx = Mathf.Abs(_backStepDestPos.x - Owner.CellPos.x);
                        int dy = Mathf.Abs(_backStepDestPos.y - Owner.CellPos.y);
                        if (dx <= 1 && dy <= 1)
                        {
                            _tryBackStep = false;
                            Owner.CreatureAIState = ECreatureAIState.Idle;
                        }

                        return _backStepDestPos;
                    }
                    else
                        return Owner.Target.CellPos;
                }

                HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
                switch (leaderController.HeroMemberFormationMode)
                {
                    case EHeroMemberFormationMode.FollowLeaderClosely:
                        return Managers.Map.WorldToCell(leaderController.Leader.transform.position); ;

                    case EHeroMemberFormationMode.NarrowFormation:
                    case EHeroMemberFormationMode.WideFormation:
                        return leaderController.RequestFormationCellPos(Owner); ;

                    case EHeroMemberFormationMode.RandomFormation:
                        return leaderController.RequestRandomFormationCellPos(Owner);

                    case EHeroMemberFormationMode.ForceStop:
                        return PrevCellPosForForceStop;
                }

                return Vector3Int.zero;
            }
        }

        private Vector3Int GetBackStepRandomCellPos(Vector3 dir, float dist)
        {
            Vector3 dest = dir * dist;
            float minRot = -5f;
            float maxRot = 5f;
            Quaternion randRot = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(minRot, maxRot));
            Vector3Int destCellPos = Managers.Map.WorldToCell(randRot * dest);
            int attemptCount = 0;
            while (Managers.Map.CanMove(destCellPos) == false)
            {
                if (attemptCount++ > 100)
                {
                    destCellPos = Owner.CellPos;
                    break;
                }

                dest = dir * UnityEngine.Random.Range(dist, dist++);
                randRot = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(minRot--, maxRot++));
                destCellPos = Managers.Map.WorldToCell(randRot * dest);
            }

            return Managers.Map.CenteredCellPos(destCellPos);
        }
        private Vector3Int _backStepDestPos = Vector3Int.zero;
        #endregion

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
            if (Owner.IsValid() == false)
                return;

            // --- Try ForceMove
            if (TryForceMove())
            {
                _tryBackStep = false;
                Owner.CreatureAIState = ECreatureAIState.Move;
                return;
            }
            else if (Owner.Target.IsValid())
            {
                // --- ForceMove가 아니고, Target이 탐지 범위에만 들어간 상태라면 타겟에게 이동을 시작한다.
                if (Owner.IsInTheNearestTarget == false)
                {
                    Owner.CreatureAIState = ECreatureAIState.Move;
                    return;
                }

                // --- Try BackStep
                Monster monster = Owner.Target as Monster;
                if (monster.Target == Owner && monster.IsInTheNearestTarget)
                {
                    _tryBackStep = true;
                    Vector3 nDir = (Owner.transform.position - Owner.Target.transform.position).normalized;
                    _backStepDestPos = GetBackStepRandomCellPos(nDir, Owner.TheShortestSkillInvokeRange);
                    Owner.CreatureAIState = ECreatureAIState.Move;
                    return;
                }
            }
            else
                _tryBackStep = false;

            // --- Skill, Env는 Move -> Idle로 부터 실행
            if (Owner.CanSkill)
                Owner.CreatureSkill.CurrentSkill.DoSkill();
            else if (Owner.CanCollectEnv)
                Owner.CollectEnv();
        }

        public override void UpdateMove()
        {
            if (Owner.IsValid() == false)
                return;

            EFindPathResult result = Owner.FindPathAndMoveToCellPos(destPos: ChaseCellPos,
               maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

            if (Owner.CanSkill || Owner.CanCollectEnv || result == EFindPathResult.Fail_NoPath)
            {
                Hero leader = Managers.Object.HeroLeaderController.Leader;
                if (leader.Moving == false)
                {
                    Owner.CreatureAIState = ECreatureAIState.Idle;
                    return;
                }
                // --- 움직이고 있을 때, Skill이 가능하면 Moving Shot을 한다.
                else if (leader.Moving && Owner.CanSkill)
                {
                    Owner.CreatureSkill.CurrentSkill.DoSkill();
                    return;
                }
            }

            {
                EvadePingPongMovement();
            }
        }

        public override void OnDead() => base.OnDead();
        #endregion
    }
}

/*
    [PREV REF]
    // public class DynamicHeroAI : HeroAI
    // {
    //     // #region Background
    //     // private bool _forceBackStepMovement = false;
    //     // public override Vector3Int ChaseCellPos
    //     // {
    //     //     get
    //     //     {
    //     //         if (Owner.ForceMove == false && Owner.Target.IsValid())
    //     //         {
    //     //             if (_forceBackStepMovement)
    //     //             {
    //     //                 Vector3 nDir = (Owner.transform.position - Owner.Target.transform.position).normalized;
    //     //                 return GetWorldToCellDest(nDir, Owner.AtkRange);
    //     //             }
    //     //             else
    //     //                 return Owner.Target.CellPos;
    //     //         }
                
    //     //         HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
    //     //         switch (leaderController.HeroMemberFormationMode)
    //     //         {
    //     //             case EHeroMemberFormationMode.FollowLeaderClosely:
    //     //                 return Managers.Map.WorldToCell(leaderController.Leader.transform.position); ;

    //     //             case EHeroMemberFormationMode.NarrowFormation:
    //     //             case EHeroMemberFormationMode.WideFormation:
    //     //                 return leaderController.RequestFormationCellPos(Owner); ;

    //     //             case EHeroMemberFormationMode.RandomFormation:
    //     //                 return leaderController.RequestRandomFormationCellPos(Owner);

    //     //             case EHeroMemberFormationMode.ForceStop:
    //     //                 return PrevCellPosForForceStop;
    //     //         }

    //     //         return Vector3Int.zero;
    //     //     }
    //     // }
    //     // #endregion

    //     // #region Core
    //     // public override bool Init()
    //     // {
    //     //     if (base.Init() == false)
    //     //         return false;

    //     //     return true;
    //     // }
    //     // public override void SetInfo(Creature owner) => base.SetInfo(owner);
    //     // public override void EnterInGame() => base.EnterInGame();
        
    //     // public override void UpdateIdle()
    //     // {
    //     //     if (Owner.IsLeader)
    //     //         return;

    //     //     if (Owner.IsValid() == false)
    //     //         return;

    //     //     Owner.LookAtValidTarget();
    //     //     if (Owner.ForceMove == false && Owner.IsValidTargetInAttackRange())
    //     //     {
    //     //         if (Owner.Target.ObjectType == EObjectType.Monster)
    //     //         {
    //     //             if (Owner.CanSkill)
    //     //                 Owner.CreatureSkill.CurrentSkill.DoSkill();

    //     //             Monster target = Owner.Target as Monster;
    //     //             if (target.IsValidTargetInAttackRange())
    //     //             {
    //     //                 _forceBackStepMovement = true;
    //     //                 Owner.CreatureAIState = ECreatureAIState.Move;
    //     //                 return;
    //     //             }
    //     //         }
    //     //         else if (Owner.Target.ObjectType == EObjectType.Env)
    //     //         {
    //     //             if (Owner.CanCollectEnv)
    //     //                 Owner.CollectEnv();
    //     //         }
    //     //     }
    //     //     else
    //     //     {
    //     //         List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: Owner.CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
    //     //         if (idlePathFind.Count > 1)
    //     //         {
    //     //             if (Managers.Map.CanMove(idlePathFind[1]))
    //     //             {
    //     //                 Owner.CreatureAIState = ECreatureAIState.Move;
    //     //                 return;
    //     //             }
    //     //         }
    //     //     }
    //     // }

    //     // public override void UpdateMove()
    //     // {
    //     //     if (Owner.IsLeader)
    //     //         return;

    //     //     if (Owner.Target.IsValid() && Owner.ForceMove == false)
    //     //     {
    //     //         if ((Owner.transform.position - Owner.Target.transform.position).magnitude > Owner.AtkRange)
    //     //         {
    //     //             _forceBackStepMovement = false;
    //     //             Owner.CreatureAIState = ECreatureAIState.Idle;
    //     //             return;
    //     //         }
    //     //     }

    //     //     EFindPathResult result = Owner.FindPathAndMoveToCellPos(destPos: ChaseCellPos,
    //     //         maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

    //     //     if (result == EFindPathResult.Fail_NoPath)
    //     //     {
    //     //         Hero leader = Managers.Object.HeroLeaderController.Leader;
    //     //         if (leader.IsMoving == false)
    //     //         {
    //     //             Owner.CreatureAIState = ECreatureAIState.Idle;
    //     //         }

    //     //         return;
    //     //     }

    //     //     EvadePingPongMovement();
    //     // }

    //     // // --- Overwrite Look At Valid Target
    //     // private void LateUpdate()
    //     // {
    //     //     Owner.LookAtValidTarget();
    //     //     if (Owner.IsValidTargetInAttackRange() && Owner.CanSkill)
    //     //     {
    //     //         Owner.CreatureSkill.CurrentSkill.DoSkill();
    //     //     }
    //     // }
    //     // #endregion
    // }

    
*/
