using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MonsterAI : CreatureAI
    {
        #region Background
        public new Monster Owner { get; private set; } = null;
        private float _desiredNextPingPongPatrolDelta = 0f;

        private float SetDesiredNextPingPongPatrolDelta(float minSec, float maxSec)
            => UnityEngine.Random.Range(minSec, maxSec);

        private float _waitPingPongPatrolDelta = 0f;

        public override Vector3Int ChaseCellPos 
        { 
            get
            {
                if (Owner.IsValid() == false)
                    return base.ChaseCellPos;

                if (Owner.Target.IsValid())
                    return Owner.Target.CellPos;
                else
                    return _patrolCellPos;
            }
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
            Owner = owner as Monster;
        }
        public override void EnterInGame()
        {
            base.EnterInGame();
            _desiredNextPingPongPatrolDelta = UnityEngine.Random.Range(2f, 4f);
            StartCoSearchTarget<Creature>(scanRange: ReadOnly.Numeric.MonsterDefaultScanRange,
                             firstTargets: Managers.Object.Heroes,
                             secondTargets: null,
                             func: Owner.IsValid,
                             allTargetsCondition: null);
        }

        public override void UpdateIdle()
        {
            if (Owner.IsValid() == false)
                return;

            Owner.LookAtValidTarget();
            if (Owner.CanSkill && Owner.LerpToCellPosCompleted)
            {
                //Owner.StopCoLerpToCellPos();
                Owner.CreatureSkill.CurrentSkill.DoSkill();
                return;
            }
            else if (_coPingPongPatrol == null)
            {
                _waitPingPongPatrolDelta += Time.deltaTime;
                if (_waitPingPongPatrolDelta >= _desiredNextPingPongPatrolDelta)
                {
                    _waitPingPongPatrolDelta = 0f;
                    _desiredNextPingPongPatrolDelta = SetDesiredNextPingPongPatrolDelta(2f, 4f);
                    StartCoPingPongPatrol(-5f, 5f);
                }
            }
        }

        public override void UpdateMove()
        {
            if (Owner.IsValid() == false)
                return;

            Owner.LookAtValidTarget();
            EFindPathResult result = Owner.FindPathAndMoveToCellPos(destPos: ChaseCellPos, maxDepth: ReadOnly.Numeric.MonsterDefaultMoveDepth);
            //Vector3 centeredPos = Managers.Map.CenteredCellToWorld(ChaseCellPos);
            if (Owner.CanSkill || result == EFindPathResult.Fail_NoPath)
            {
                Owner.CreatureAIState = ECreatureAIState.Idle;
                return;
            }

            // if ((transform.position - centeredPos).sqrMagnitude < 0.01f && Owner.CanSkill)
            // {
            //     if ((transform.position - centeredPos).sqrMagnitude < 0.01f)
            //     {
            //         Debug.Log("<color=magenta>### 111 ###</color>");
            //         Debug.Break();
            //     }

            //     //Owner.StopCoLerpToCellPos();
            //     Owner.CreatureAIState = ECreatureAIState.Idle;
            //     return;
            // }
            // else if ((transform.position - centeredPos).sqrMagnitude < 0.01f || result == EFindPathResult.Fail_NoPath)
            // {
            //     Debug.Log("<color=magenta>### 222 ###</color>");
            //     Owner.CreatureAIState = ECreatureAIState.Idle;
            //     return;
            // }
        }

        public override void OnDead()
        {
            base.OnDead();
            StopCoPingPongPatrol();
        }
        #endregion

        #region Coroutines
        protected Coroutine _coPingPongPatrol = null;
        protected Vector3Int _patrolCellPos = Vector3Int.zero;
        protected bool _patrolPingPongFlag = false;
        protected IEnumerator CoPingPongPatrol(float minDistance, float maxDistance)
        {
            int attemptCount = 0;
            int maxAttemptCount = 100;
            Vector3 _initialSpawnPos = Managers.Map.CenteredCellToWorld(Owner.InitialSpawnedCellPos.Value);
            if (_patrolPingPongFlag == false)
            {
                float x = _initialSpawnPos.x + UnityEngine.Random.Range(minDistance, maxDistance);
                float y = _initialSpawnPos.y + UnityEngine.Random.Range(minDistance, maxDistance);
                _patrolCellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                bool isShortDist = (Owner.InitialSpawnedCellPos.Value - _patrolCellPos).sqrMagnitude < 9f; // 최소 3칸이상으로 움직여라
                while (Managers.Map.CanMove(_patrolCellPos) == false || isShortDist)
                {
                    if (attemptCount++ >= maxAttemptCount) // --- DEFENSE
                    {
                        _patrolCellPos = Owner.InitialSpawnedCellPos.Value;
                        break;
                    }

                    x = _initialSpawnPos.x + UnityEngine.Random.Range(minDistance, maxDistance);
                    y = _initialSpawnPos.y + UnityEngine.Random.Range(minDistance, maxDistance);
                    _patrolCellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                    if ((Owner.InitialSpawnedCellPos.Value - _patrolCellPos).sqrMagnitude < 9f)
                        isShortDist = true;
                    else
                        isShortDist = false;

                    yield return null;
                }
            }
            else
                _patrolCellPos = Owner.InitialSpawnedCellPos.Value;

            Owner.CreatureAIState = ECreatureAIState.Move;
            _patrolPingPongFlag = !_patrolPingPongFlag;
            StopCoPingPongPatrol();
        }

        protected void StartCoPingPongPatrol(float minDistance, float maxDistance)
        {
            if (Owner.CreatureAIState != ECreatureAIState.Idle) // --- DEFENSE
            {
                Debug.LogWarning("Patrol can only run in idle state.");
                return;
            }

            StopCoPingPongPatrol();
            _coPingPongPatrol = StartCoroutine(CoPingPongPatrol(minDistance, maxDistance));
        }

        protected void StopCoPingPongPatrol()
        {
            if (_coPingPongPatrol != null)
            {
                StopCoroutine(_coPingPongPatrol);
                _coPingPongPatrol = null;
            }
        }
        #endregion
    }
}
