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
        public Monster _monsterOwner { get; private set; } = null;
        private float _patrolDelta = 0f;
        private float _desiredStartPatrolTime = 0f;
        public override Vector3Int CellChasePos
        {
            get
            {
                if (IsValidTarget)
                    return _monsterOwner.Target.CellPos;

                return _cellPatrolPos;
            }
        }

        protected override IEnumerator CoFindTargets()
        {
            int scanRange = ReadOnly.Util.ObjectScanRange; // --- 6칸
            float scanTick = ReadOnly.Util.ObjectScanTick;
            while (true)
            {
                Owner.Targets.Clear();
                if (Owner.IsValid() == false)
                {
                    StopCoFindTargets();
                    yield break;
                }

                if (PauseFindTargets)
                {
                    yield return null;
                    continue;
                }
                
                // Need to add ally option later
                List<Hero> heroes = Managers.Map.GatherObjects<Hero>(Owner.transform.position, scanRange, scanRange);
                for (int i = 0; i < heroes.Count; ++i)
                {
                    if (heroes[i].IsValid() == false)
                        continue;

                    Owner.Targets.Add(heroes[i]);
                }

                yield return new WaitForSeconds(scanTick);
            }
        }

        #region Init Core
        public override void InitialSetInfo(Creature owner)
        {
            base.InitialSetInfo(owner);
            _monsterOwner = owner as Monster;
        }
        
        public override void EnterInGame()
        {
            _desiredStartPatrolTime = Random.Range(2f, 4f);
            //base.EnterInGame();
            StartCoForceWait();
        }
        #endregion Init Core

        public override void UpdateIdle()
        {
            if (IsValidOwner == false)
                return;

            _monsterOwner.LookAtValidTarget();
            if (IsEndMoveAnim && _monsterOwner.CanSkill)
            {
                _monsterOwner.CreatureSkill.CurrentSkill.DoSkill();
                return;
            }
            else if (_coPatrol == null)
            {
                _patrolDelta += Time.deltaTime;
                if (_patrolDelta >= _desiredStartPatrolTime)
                {
                    _patrolDelta = 0f;
                    _desiredStartPatrolTime = Random.Range(2f, 4f);
                    StartCoPatrol(-5f, 5f);
                }
            }
        }

        public override void UpdateMove()
        {
            if (_monsterOwner.IsValid() == false)
                return;

            EFindPathResult result = _monsterOwner.FindPathAndMoveToCellPos(destPos: CellChasePos, maxDepth: ReadOnly.Util.MonsterDefaultMoveDepth);
            if (result == EFindPathResult.Fail_LerpCell)
            {
                _monsterOwner.CreatureAIState = ECreatureAIState.Idle;
                return;
            }
        }

        public override void OnDead()
        {
            base.OnDead();
            StopCoPatrol();
        }

        protected Coroutine _coPatrol = null;
        protected Vector3Int _cellPatrolPos = Vector3Int.zero;
        protected bool _patrolPingPongFlag = false;
        protected IEnumerator CoPatrol(float minDistance, float maxDistance)
        {
            int attemptCount = 0;
            int maxAttemptCount = 999;
            Vector3 _initialSpawnPos = _monsterOwner.SpawnedPos;
            if (_patrolPingPongFlag == false)
            {
                float x = _initialSpawnPos.x + Random.Range(minDistance, maxDistance);
                float y = _initialSpawnPos.y + Random.Range(minDistance, maxDistance);
                _cellPatrolPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                // --- 최소 3칸 이상일 때 패트롤 시작
                bool isShortDist = (_monsterOwner.CellPos - _cellPatrolPos).sqrMagnitude < 9f; 
                while (Managers.Map.CanMove(_cellPatrolPos) == false || isShortDist)
                {
                    if (attemptCount++ >= maxAttemptCount)
                    {
                        //_cellPatrolPos = _monsterOwner.CellPos;
                        _cellPatrolPos = Managers.Map.WorldToCell(_monsterOwner.transform.position); // --- TEMP
                        break;
                    }

                    x = _initialSpawnPos.x + Random.Range(minDistance, maxDistance);
                    y = _initialSpawnPos.y + Random.Range(minDistance, maxDistance);
                    _cellPatrolPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                    if ((_monsterOwner.CellPos - _cellPatrolPos).sqrMagnitude < 9f)
                        isShortDist = true;
                    else
                        isShortDist = false;

                    yield return null;
                }
            }
            else
                _cellPatrolPos = Managers.Map.WorldToCell(_initialSpawnPos);

            _monsterOwner.CreatureAIState = ECreatureAIState.Move;
            _patrolPingPongFlag = !_patrolPingPongFlag;
            StopCoPatrol();
        }

        protected void StartCoPatrol(float minDistance, float maxDistance)
        {
            if (_monsterOwner.CreatureAIState != ECreatureAIState.Idle)
            {
                Debug.LogWarning("Patrol can only run in idle state.");
                return;
            }

            StopCoPatrol();
            _coPatrol = StartCoroutine(CoPatrol(minDistance, maxDistance));
        }

        protected void StopCoPatrol()
        {
            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }
        }
    }
}
