using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEditor;
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
                
                float mag = dir.magnitude;
                float sqrMag = dir.sqrMagnitude;
                if (scanRangeSQR < sqrMag)
                    continue;

                if (bestDistSQR < sqrMag)
                    continue;

                if (func?.Invoke(obj) == false)
                    continue;

                bestDistSQR = sqrMag;
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
                            if (_currentPingPongCantMoveCount >= ReadOnly.Util.MaxCanPingPongConditionCount && IsForceMovingPingPongObject == false)
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
        // protected Vector3Int GetWorldToCellDest(Vector3 dir, float dist)
        // {
        //     Vector3 dest = dir * dist;
        //     float minRot = -30f;
        //     float maxRot = 30f;
        //     Quaternion randRot = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(minRot, maxRot));
        //     Vector3Int destCellPos = Managers.Map.WorldToCell(randRot * dest);
        //     int attemptCount = 0;
        //     while (Managers.Map.CanMove(destCellPos) == false)
        //     {
        //         if (attemptCount++ > 100)
        //         {
        //             destCellPos = Owner.CellPos;
        //             break;
        //         }

        //         dest = dir * UnityEngine.Random.Range(dist, dist++);
        //         randRot = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(minRot--, maxRot++));
        //         destCellPos = Managers.Map.WorldToCell(randRot * dest);
        //     }

        //     return destCellPos;
        // }

        #endregion

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public virtual void SetInfo(Creature owner) => Owner = owner;
        public virtual void EnterInGame() { }
        public virtual void UpdateIdle() { }
        public virtual void UpdateMove() { }
        public virtual void OnDead()
        {
            StopCoFindEnemies();
        }
        #endregion

        #region Coroutines
        public bool PauseFindEnemies { get; protected set; } = false;
        private Coroutine _coFindEnemies = null;
        private IEnumerator CoFindEnemies()
        {
            int scanRange = ReadOnly.Util.ObjectScanRange; // --- 6칸
            float scanTick = ReadOnly.Util.ObjectScanTick;
            while (true)
            {
                Owner.Targets.Clear();
                if (Owner.IsValid() == false)
                    yield break;

                if (PauseFindEnemies)
                {
                    Owner.Targets.Clear();
                    yield return null;
                    continue;
                }

                EObjectType ownerType = Owner.ObjectType;
                EObjectType targetType = Util.GetTargetType(ownerType, isAlly: false);
                if (targetType == EObjectType.Monster)
                {
                    List<Monster> monsters = Managers.Map.GatherObjects<Monster>(Owner.transform.position, scanRange, scanRange);
                    for (int i = 0; i < monsters.Count; ++i)
                    {
                        if (monsters[i].IsValid() == false)
                            continue;

                        Owner.Targets.Add(monsters[i]);
                    }

                    List<Env> Envs = Managers.Map.GatherObjects<Env>(Owner.transform.position, scanRange, scanRange);
                    for (int i = 0; i < Envs.Count; ++i)
                    {
                        if (Envs[i].IsValid() == false)
                            continue;

                        Owner.Targets.Add(Envs[i]);
                    }

                    (Owner as Hero).SortTargets();
                }
                else if (targetType == EObjectType.Hero)
                {
                    List<Hero> heroes = Managers.Map.GatherObjects<Hero>(Owner.transform.position, scanRange, scanRange);
                    for (int i = 0; i < heroes.Count; ++i)
                    {
                        if (heroes[i].IsValid() == false)
                            continue;
                        
                        Owner.Targets.Add(heroes[i]);
                    }
                }

                yield return new WaitForSeconds(scanTick);
            }
        }

        public void StartCoFindEnemies()
        {
            StopCoFindEnemies();
            _coFindEnemies = StartCoroutine(CoFindEnemies());
        }

        private void StopCoFindEnemies()
        {
            if (_coFindEnemies != null)
                StopCoroutine(_coFindEnemies);
            
            Owner.Targets.Clear();
            _coFindEnemies = null;
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
