using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroAI : CreatureAI
    {
        #region Background
        public Hero HeroOwner { get; private set; } = null;
        public override Vector3Int CellChasePos
        {
            get
            {
                // --- Owner.ForceMove == false는 계속 false로 잠그고 있어야함.
                // --- 기본적으로 Leader의 Position을 쫓는다.
                if (Owner.ForceMove == false && Owner.Target.IsValid())
                    return Owner.Target.CellPos;

                HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
                switch (leaderController.HeroMemberFormationMode)
                {
                    case EHeroMemberFormationMode.FollowLeaderClosely:
                        return Managers.Map.WorldToCell(leaderController.Leader.transform.position); ;

                    case EHeroMemberFormationMode.NarrowFormation:
                    case EHeroMemberFormationMode.WideFormation:
                        return leaderController.RequestFormationCellPos(HeroOwner); ;

                    case EHeroMemberFormationMode.RandomFormation:
                        return leaderController.RequestRandomFormationCellPos(HeroOwner);

                    case EHeroMemberFormationMode.ForceStop:
                        return PrevCellPosForForceStop;
                }

                return Vector3Int.zero;
            }
        }
        public Vector3Int PrevCellPosForForceStop { get; set; } = Vector3Int.zero;
        private bool IsForceStopMode()
        {
            HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
            if (leaderController == null)
                return false;

            return leaderController.HeroMemberFormationMode == EHeroMemberFormationMode.ForceStop;
        }

        protected bool TryForceMove()
        {
            // --- ForceMove가 아닌 상태에서 싸우고 있을 때
            if (Owner.ForceMove == false)
                return false;

            _currentPingPongCantMoveCount = 0;
            HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
            Hero leader = leaderController.Leader;
            if (leader.IsValid() == false)
                return false;

            // --- Idle에서 ForceMove 상태가 되었을 때
            if (Owner.Target.IsValid() == false)
            {
                // --- 타겟이 없으면 바로 움직인다.
                List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: Owner.CellPos, destCellPos: leader.CellPos, maxDepth: 2);
                if (idlePathFind.Count > 1)
                {
                    if (Managers.Map.CanMove(idlePathFind[1]))
                        return true;
                }
            }
            else
            {
                // --- 기존에 타겟이 있었을 경우(Skill 또는 Env를 하고 있었던 경우)
                if (Owner.IsInTheNearestTarget)
                {
                    int dx = Mathf.Abs(leader.CellPos.x - Owner.CellPos.x);
                    int dy = Mathf.Abs(leader.CellPos.y - Owner.CellPos.y);

                    // --- 리더가 움직였을 때, 리더와 일정한 칸 이상 떨어진 상태가 되면 강제로 움직인다.
                    // --- Force를 해야 Move 상태로 돌아갈 수 있다.
                    if (Owner.Target.ObjectType == EObjectType.Monster)
                    {
                        if (leaderController.ForceFollowToLeader == false)
                        {
                            // 몇 칸 떨어졌는지 약간 늦게 체크되는 것 같으면 이것만 LateUpdate 등으로 빼도 될 것 같긴함.
                            if (dx >= 5 && dy >= 5)
                                return true;
                        }
                        // --- ForceFollowToLeader가 true라면 ForceMove를 시도했을 때, Env처럼 강제로 바로 움직인다.
                        else
                            return true;
                    }
                    else // --- Env가 Target이었다면 움직였을 때 무조건 Move로 전환한다.
                        return true;
                }
            }
            
            return false;
        }
        #endregion

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void InitialSetInfo(Creature owner)
        {
            base.InitialSetInfo(owner);
            HeroOwner = owner as Hero;
        }
        public override void EnterInGame()
        {
            base.EnterInGame();
        }

        public override void UpdateIdle() { }
        public override void UpdateMove() { }
        public override void OnDead()
        {
            //StopCoIsFarFromLeaderTick(); // --- 일단 생략
            base.OnDead();

        }
        #endregion

        #region Coroutines
        [SerializeField] private bool _isFarFromLeader = false;
        private Coroutine _coIsFarFromLeaderTick = null;
        private IEnumerator CoIsFarFromLeaderTick()
        {
            // Scan Range 보다 50%이상 멀어졌을 때
            // float farFromLeaderDistSQR = ReadOnly.Util.HeroDefaultScanRange * ReadOnly.Util.HeroDefaultScanRange + (ReadOnly.Util.HeroDefaultScanRange * ReadOnly.Util.HeroDefaultScanRange * 0.5f);
            // float canWarpDistSQR = ReadOnly.Util.CheckFarFromHeroesLeaderDistanceForWarp * ReadOnly.Util.CheckFarFromHeroesLeaderDistanceForWarp;
            // while (true)
            // {
            //     Hero leader = Managers.Object.HeroLeaderController.Leader;
            //     if (leader.IsValid() == false) // --- DEFENSE
            //     {
            //         yield return null;
            //         continue;
            //     }

            //     if ((leader.CellPos - Owner.CellPos).sqrMagnitude > farFromLeaderDistSQR)
            //     {
            //         _isFarFromLeader = true;
            //         PauseSearchTarget = true;
            //     }
            //     else
            //     {
            //         _isFarFromLeader = false;
            //         PauseSearchTarget = false;
            //     }

            //     // 15칸(225) 이상일 때, 어차피 로그 함수 이동속도로 금방 따라오긴하지만 알수 없는 이유로 히어로가 막혀있을 때
            //     if ((leader.CellPos - Owner.CellPos).sqrMagnitude > canWarpDistSQR && _coWaitForceStopWarp == null && IsForceStopMode() == false)
            //     {
            //         Vector3 leaderWorldPos = Managers.Object.HeroLeaderController.Leader.transform.position;
            //         Managers.Map.WarpTo(Owner, Managers.Map.WorldToCell(leaderWorldPos), warpEndCallback: null);
            //     }

            //     yield return new WaitForSeconds(ReadOnly.Util.CheckFarFromHeroesLeaderTick);
            // }

            yield return new WaitForSeconds(ReadOnly.Util.CheckFarFromHeroesLeaderTick);

        }

        private void StartCoIsFarFromLeaderTick()
        {
            StopCoIsFarFromLeaderTick();
            _coIsFarFromLeaderTick = StartCoroutine(CoIsFarFromLeaderTick());
        }

        private void StopCoIsFarFromLeaderTick()
        {
            if (_coIsFarFromLeaderTick != null)
                StopCoroutine(_coIsFarFromLeaderTick);

            _coIsFarFromLeaderTick = null;
        }

         private Coroutine _coWaitForceStopWarp = null;
        private IEnumerator CoWaitForceStopWarp(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            StopCoWaitForceStopWarp();
        }

        public void StartCoWaitForceStopWarp(float seconds)
        {
            StopCoWaitForceStopWarp(); // --- 즉시 곧바로 다시 Force Stop 모드로 돌아올 수도 있으므로
            _coWaitForceStopWarp = StartCoroutine(CoWaitForceStopWarp(seconds));
        }

        public void StopCoWaitForceStopWarp()
        {
            if (_coWaitForceStopWarp != null)
                StopCoroutine(_coWaitForceStopWarp);

            _coWaitForceStopWarp = null;
        }
        #endregion
    }
}

/*
    [PREV REF]
    // --- 상태이상 등에 의해 못갈수도 있으므로 Try를 붙임.
    protected void TryForceMove_Prev()
    {
        List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: Owner.CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
        if (idlePathFind.Count > 1)
        {
            if (Managers.Map.CanMove(idlePathFind[1]))
            {
                Owner.CreatureAIState = ECreatureAIState.Move;
                return;
            }
        }
    }
*/