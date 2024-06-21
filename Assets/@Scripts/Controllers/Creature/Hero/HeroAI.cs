using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroAI : CreatureAI
    {
        #region Background
        public new Hero Owner { get; private set; } = null;
        protected override T SearchClosestInRange<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, Func<T, bool> func = null, Func<bool> allTargetsCondition = null)
        {
            T firstTarget = null;
            T secondTarget = null;
            float bestDistSQR = float.MaxValue;
            float scanRangeSQR = scanRange * scanRange;

            if (Owner.IsValid() && Owner.IsLeader)
                scanRangeSQR *= 0.8f;

            foreach (T obj in firstTargets)
            {
                Vector3Int dir = obj.CellPos - Owner.CellPos;
                float distToTargetSQR = dir.sqrMagnitude;
                if (scanRangeSQR < distToTargetSQR)
                    continue;

                if (bestDistSQR < distToTargetSQR)
                    continue;

                if (func?.Invoke(obj) == false)
                    continue;

                bestDistSQR = distToTargetSQR;
                firstTarget = obj;
            }

            // --- 일반적인 Searching 또는 AutoTarget이 켜져있을 때
            if (allTargetsCondition == null || allTargetsCondition.Invoke() == false)
            {
                if (firstTarget != null)
                    return firstTarget;
                else if (firstTarget == null && secondTargets != null)
                {
                    foreach (T obj in secondTargets)
                    {
                        Vector3Int dir = obj.CellPos - Owner.CellPos;
                        float distToTargetSQR = dir.sqrMagnitude;
                        if (scanRangeSQR < distToTargetSQR)
                            continue;

                        if (bestDistSQR < distToTargetSQR)
                            continue;

                        if (func?.Invoke(obj) == false)
                            continue;

                        bestDistSQR = distToTargetSQR;
                        secondTarget = obj;
                    }
                }

                return secondTarget;
            }
            // --- AutoTarget과 상관 없이 리더이고 ForceMove가 True일 때.
            else if (allTargetsCondition != null && allTargetsCondition.Invoke())
            {
                foreach (T obj in secondTargets)
                {
                    Vector3Int dir = obj.CellPos - Owner.CellPos;
                    float distToTargetSQR = dir.sqrMagnitude;
                    if (scanRangeSQR < distToTargetSQR)
                        continue;

                    if (bestDistSQR < distToTargetSQR)
                        continue;

                    if (func?.Invoke(obj) == false)
                        continue;

                    bestDistSQR = distToTargetSQR;
                    secondTarget = obj;
                }

                if (func?.Invoke(firstTarget) == false)
                    return secondTarget;
                else if (func?.Invoke(secondTarget) == false)
                    return firstTarget;
                else
                {
                    float fDistSQR = (firstTarget.CellPos - Owner.CellPos).sqrMagnitude;
                    float sDistSQR = (secondTarget.CellPos - Owner.CellPos).sqrMagnitude;
                    if (fDistSQR < sDistSQR)
                        return firstTarget;
                    else
                        return secondTarget;
                }
            }

            return null;
        }
        public override Vector3Int ChaseCellPos
        {
            get
            {
                if (Owner.ForceMove == false && Owner.Target.IsValid())
                    return Owner.Target.CellPos;
                
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
        public Vector3Int PrevCellPosForForceStop { get; set; } = Vector3Int.zero;
        private bool IsForceStopMode()
        {
            HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
            if (leaderController == null)
                return false;

            return leaderController.HeroMemberFormationMode == EHeroMemberFormationMode.ForceStop;
        }
        #endregion

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfo(Creature owner)
        {
            base.SetInfo(owner);
            Owner = owner as Hero;
        }
        public override void EnterInGame()
        {
            base.EnterInGame();

            // --- First Targets: Monsters, Second Targets: Envs
            StartCoSearchTarget<BaseObject>(scanRange: ReadOnly.Numeric.HeroDefaultScanRange,
                            firstTargets: Managers.Object.Monsters,
                            secondTargets: Managers.Object.Envs,
                            func: Owner.IsValid);
        }

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
                    Owner.CreatureAIState = ECreatureAIState.Idle;
                }

                return;
            }
            

            {
                List<Vector3Int> path = Managers.Map.FindPath(Owner.CellPos, ChaseCellPos, 2);
                if (path.Count > 0)
                {
                    Vector3 centeredLastPathPos = Managers.Map.CenteredCellToWorld(path[path.Count - 1]);
                    if (Owner.Target.IsValid() && (Owner.transform.position - centeredLastPathPos).sqrMagnitude < 0.01f)
                    {
                        if (IsPingPongAndCantMoveToDest(Owner.CellPos))
                        {
                            if (_currentPingPongCantMoveCount >= ReadOnly.Numeric.MaxCanPingPongConditionCount && IsForceMovingPingPongObject == false)
                            {
                                Debug.Log($"<color=magenta>[!]{Owner.gameObject.name}, Start force moving for PingPong Object.</color>");
                                Owner.StopCoLerpToCellPos();
                                CoStartForceMovePingPongObject(Owner.CellPos, ChaseCellPos, endCallback: delegate ()
                                {
                                    Debug.Log($"<color=cyan>[!]{Owner.gameObject.name}, End ForcePingPong..</color>");
                                    _currentPingPongCantMoveCount = 0;
                                    CoStopForceMovePingPongObject();
                                    Owner.StartCoLerpToCellPos();
                                    Owner.CreatureAIState = ECreatureAIState.Idle;
                                });
                            }
                            else if (IsForceMovingPingPongObject == false)
                                ++_currentPingPongCantMoveCount;
                        }
                    }
                }
            }
        }

        public override void OnDead()
        {
            StopCoIsFarFromLeaderTick();
            base.OnDead();
        }
        #endregion

        #region Coroutines
        public void StartSearchTarget(System.Func<bool> allTargetsCondition = null)
        {
            StartCoSearchTarget<BaseObject>(scanRange: ReadOnly.Numeric.HeroDefaultScanRange,
                            firstTargets: Managers.Object.Monsters,
                            secondTargets: Managers.Object.Envs,
                            func: Owner.IsValid);

            StartCoIsFarFromLeaderTick();
        }

        [SerializeField] private bool _isFarFromLeader = false;
        private Coroutine _coIsFarFromLeaderTick = null;
        private IEnumerator CoIsFarFromLeaderTick()
        {
            // Scan Range 보다 50%이상 멀어졌을 때
            float farFromLeaderDistSQR = ReadOnly.Numeric.HeroDefaultScanRange * ReadOnly.Numeric.HeroDefaultScanRange + (ReadOnly.Numeric.HeroDefaultScanRange * ReadOnly.Numeric.HeroDefaultScanRange * 0.5f);
            float canWarpDistSQR = ReadOnly.Numeric.CheckFarFromHeroesLeaderDistanceForWarp * ReadOnly.Numeric.CheckFarFromHeroesLeaderDistanceForWarp;
            while (true)
            {
                Hero leader = Managers.Object.HeroLeaderController.Leader;
                if (leader.IsValid() == false) // --- DEFENSE
                {
                    yield return null;
                    continue;
                }

                if ((leader.CellPos - Owner.CellPos).sqrMagnitude > farFromLeaderDistSQR)
                {
                    _isFarFromLeader = true;
                    PauseSearchTarget = true;
                }
                else
                {
                    _isFarFromLeader = false;
                    PauseSearchTarget = false;
                }

                // 15칸(225) 이상일 때, 어차피 로그 함수 이동속도로 금방 따라오긴하지만 알수 없는 이유로 히어로가 막혀있을 때
                if ((leader.CellPos - Owner.CellPos).sqrMagnitude > canWarpDistSQR && _coWaitForceStopWarp == null && IsForceStopMode() == false)
                {
                    Vector3 leaderWorldPos = Managers.Object.HeroLeaderController.Leader.transform.position;
                    Managers.Map.WarpTo(Owner, Managers.Map.WorldToCell(leaderWorldPos), warpEndCallback: null);
                }

                yield return new WaitForSeconds(ReadOnly.Numeric.CheckFarFromHeroesLeaderTick);
            }
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
