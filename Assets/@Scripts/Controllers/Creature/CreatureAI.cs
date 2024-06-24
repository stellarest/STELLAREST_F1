using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAI : InitBase
    {
        #region Background
        public Creature Owner { get; private set; } = null;
        public virtual Vector3Int ChaseCellPos { get; } = Vector3Int.zero;
        private Queue<Vector3Int> _cantMoveCheckQueue = new Queue<Vector3Int>();

        protected virtual T SearchClosestInRange<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null,
                                      System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            T firstTarget = null;
            T secondTarget = null;
            float bestDistSQR = float.MaxValue;
            float scanRangeSQR = scanRange * scanRange;

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

        /*
            1 - 큐에 A 추가: [A]
            2 - 큐에 B 추가: [A, B]
            3 - 큐에 A 추가: [A, B, A]
            4 - 큐에 B 추가: [A, B, A, B]
            5 - 1 (검사)
            5 - 2 Dequeue: [B, A, B]
            5 - 3 큐에 A 추가: [B, A, B, A]
            6 - 1 (검사)
            6 - 2 Dequeue: [A, B, A]
            6 - 3 큐에 B 추가: [A, B, A, B]
            7 - 1 (검사)
            ...
        */
        [SerializeField] protected int _currentPingPongCantMoveCount = 0;
        private int maxCantMoveCheckCount = 4; //--- 2칸에 대해 왔다 갔다만 조사하는 것이라 4로 설정
        protected bool IsPingPongAndCantMoveToDest(Vector3Int cellPos)
        {
            if (_cantMoveCheckQueue.Count >= maxCantMoveCheckCount)
                _cantMoveCheckQueue.Dequeue();

            _cantMoveCheckQueue.Enqueue(cellPos);
            if (_cantMoveCheckQueue.Count == maxCantMoveCheckCount)
            {
                Vector3Int[] cellArr = _cantMoveCheckQueue.ToArray();
                HashSet<Vector3Int> uniqueCellPos = new HashSet<Vector3Int>(cellArr);
                if (uniqueCellPos.Count == 2)
                {
                    Dictionary<Vector3Int, int> checkCellPosCountDict = new Dictionary<Vector3Int, int>();
                    foreach (var pos in _cantMoveCheckQueue)
                    {
                        if (checkCellPosCountDict.ContainsKey(pos))
                            checkCellPosCountDict[pos]++;
                        else
                            checkCellPosCountDict[pos] = 1;
                    }

                    foreach (var count in checkCellPosCountDict.Values)
                    {
                        if (count != 2)
                            return false;
                    }

                    return true;
                }
            }

            return false;
        }

        protected bool IsForceMovingPingPongObject => _coForceMovePingPongObject != null;

        protected void EvadePingPongMovement()
        {
            if (Owner.ForceMove)
                return;

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
        protected Vector3Int GetWorldToCellDest(Vector3 dir, float dist)
        {
            Vector3 dest = dir * dist;
            float minRot = -30f;
            float maxRot = 30f;
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

            return destCellPos;
        }

        #endregion

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public virtual void SetInfo(Creature owner) => Owner = owner;
        public virtual void EnterInGame()
        {
        }

        public virtual void UpdateIdle()
        {
        }

        public virtual void UpdateMove()
        {
        }

        public virtual void OnDead()
        {
            StopCoSearchTarget();
        }
        #endregion

        #region Coroutines
        public bool PauseSearchTarget { get; protected set; } = false;
        protected Coroutine _coSearchTarget = null;
        private IEnumerator CoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null,
                                            System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            float tick = ReadOnly.Numeric.SearchFindTargetTick;
            while (true)
            {
                if (Owner.IsValid() == false)
                    yield break;

                yield return new WaitForSeconds(tick);
                if (PauseSearchTarget)
                {
                    Owner.Target = null;
                    yield return null;
                    continue;
                }

                Owner.Target = SearchClosestInRange(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition);
            }
        }

        protected void StartCoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            StopCoSearchTarget();
            _coSearchTarget = StartCoroutine(CoSearchTarget<T>(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition));
        }

        protected void StopCoSearchTarget()
        {
            if (_coSearchTarget != null)
                StopCoroutine(_coSearchTarget);

            Owner.Target = null;
            _coSearchTarget = null;
        }

        private Coroutine _coForceMovePingPongObject = null;
        private IEnumerator CoForceMovePingPongObject(Vector3Int currentCellPos, Vector3Int destCellPos, System.Action endCallback = null)
        {
            List<Vector3Int> path = Managers.Map.FindPath(currentCellPos, destCellPos);

            Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
            for (int i = 0; i < path.Count; ++i)
                pathQueue.Enqueue(path[i]);
            pathQueue.Dequeue();

            // --- DEFENSE
            if (pathQueue.Count == 0)
            {
                endCallback?.Invoke();
                Owner.UpdateCellPos();
                yield break;
            }

            Vector3Int nextPos = pathQueue.Dequeue();
            Vector3 currentWorldPos = Managers.Map.CellToWorld(currentCellPos);
            while (pathQueue.Count != 0)
            {
                Vector3 destPos = Managers.Map.CenteredCellToWorld(nextPos);
                Vector3 dir = destPos - Owner.transform.position;
                if (dir.x < 0f)
                    Owner.LookAtDir = ELookAtDirection.Left;
                else if (dir.x > 0f)
                    Owner.LookAtDir = ELookAtDirection.Right;

                if (dir.sqrMagnitude < 0.01f)
                {
                    Owner.transform.position = destPos;
                    currentWorldPos = Owner.transform.position;
                    nextPos = pathQueue.Dequeue();
                }
                else
                {
                    float moveDist = Mathf.Min(dir.magnitude, Owner.MovementSpeed * Time.deltaTime);
                    Owner.transform.position += dir.normalized * moveDist; // Movement per frame.
                }

                yield return null;
            }

            endCallback?.Invoke();
            Owner.UpdateCellPos();
        }

        protected void CoStartForceMovePingPongObject(Vector3Int currentCellPos, Vector3Int destCellPos, System.Action endCallback = null)
        {
            if (_coForceMovePingPongObject != null)
                return;

            //CoStopForceMovePingPongObject();
            _coForceMovePingPongObject = StartCoroutine(CoForceMovePingPongObject(currentCellPos, destCellPos, endCallback));
        }

        protected void CoStopForceMovePingPongObject()
        {
            Debug.Log("STOP PING PONG!!!");
            if (_coForceMovePingPongObject != null)
                StopCoroutine(_coForceMovePingPongObject);

            _coForceMovePingPongObject = null;
        }
        #endregion
    }
}
