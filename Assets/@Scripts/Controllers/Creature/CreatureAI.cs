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
        public Creature Owner { get; private set; } = null;
        protected bool IsValidOwner => Owner.IsValid();
        protected bool IsValidTarget => Owner.Target.IsValid();

        public virtual Vector3Int CellChasePos { get; } = Vector3Int.zero;
        private Queue<Vector3Int> _cantMoveCheckQueue = new Queue<Vector3Int>();
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
            // if (Owner.ForceMove)
            //     return;
            {
                List<Vector3Int> path = Managers.Map.FindPath(Owner.CellPos, CellChasePos, 2);
                if (path.Count > 0)
                {
                    Vector3 centeredLastPathPos = Managers.Map.CellToCenteredWorld(path[path.Count - 1]);
                    if (Owner.Target.IsValid() && (Owner.transform.position - centeredLastPathPos).sqrMagnitude < 0.01f)
                    {
                        if (IsPingPongAndCantMoveToDest(Owner.CellPos))
                        {
                            if (_currentPingPongCantMoveCount >= ReadOnly.Util.MaxCanPingPongConditionCount && IsForceMovingPingPongObject == false)
                            {
                                Debug.Log($"<color=magenta>[!]{Owner.gameObject.name}, Start force moving for PingPong Object.</color>");
                                Owner.StopCoLerpToCellPos();
                                CoStartForceMovePingPongObject(Owner.CellPos, CellChasePos, endCallback: delegate ()
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

        #region Init Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public virtual void InitialSetInfo(Creature owner) 
            => Owner = owner;
            
        public virtual void EnterInGame()
            => StartCoFindTargets();
        #endregion Init Core
        public virtual void UpdateIdle() { }
        public virtual void UpdateMove() { }
        public virtual void OnDead()
            => StopCoFindTargets();

        protected bool IsEndMoveAnim
        {
            get
            {
                if (Owner.IsValid() == false)
                    return false;

                return Owner.CreatureAnim.IsEnteredAnimState(ECreatureAnimState.Upper_Move) == false;
            }
        }


        public bool PauseFindTargets { get; protected set; } = false;
        private Coroutine _coFindTargets = null;
        protected virtual IEnumerator CoFindTargets() // --- Virtual로 바꿔도 될 것 같은데...
        {
            int scanRange = ReadOnly.Util.ObjectScanRange; // --- 6칸
            float scanTick = ReadOnly.Util.ObjectScanTick;
            while (true)
            {
                Owner.Targets.Clear();
                if (Owner.IsValid() == false)
                    yield break;

                if (PauseFindTargets)
                {
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

                    Owner.Targets = Owner.Targets
                                    .Where(n => n.IsValid())
                                    .OrderBy(n =>
                                    {
                                         return n.ObjectType == EObjectType.Monster ? 0 :
                                                n.ObjectType == EObjectType.Env ? 1 : 2;
                                    })
                                    .ThenBy(n => (transform.position - n.transform.position).sqrMagnitude)
                                    .ToList();
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

        // Enemies
        // Allies
        public void StartCoFindTargets()
        {
            StopCoFindTargets();
            _coFindTargets = StartCoroutine(CoFindTargets());
        }

        protected void StopCoFindTargets()
        {
            if (_coFindTargets != null)
                StopCoroutine(_coFindTargets);
            
            Owner.Targets.Clear();
            _coFindTargets = null;
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
                // Owner.UpdateCellPos();
                yield break;
            }

            Vector3Int nextPos = pathQueue.Dequeue();
            Vector3 currentWorldPos = Managers.Map.CellToWorld(currentCellPos);
            while (pathQueue.Count != 0)
            {
                Vector3 destPos = Managers.Map.CellToCenteredWorld(nextPos);
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
            // Owner.UpdateCellPos();
        }

        protected void CoStartForceMovePingPongObject(Vector3Int currentCellPos, Vector3Int destCellPos, Action endCallback = null)
        {
            if (_coForceMovePingPongObject != null)
                return;

            //CoStopForceMovePingPongObject();
            _coForceMovePingPongObject = StartCoroutine(CoForceMovePingPongObject(currentCellPos, destCellPos, endCallback));
        }

        protected void CoStopForceMovePingPongObject()
        {
            if (_coForceMovePingPongObject != null)
                StopCoroutine(_coForceMovePingPongObject);

            _coForceMovePingPongObject = null;
        }

        [field: SerializeField] public bool ForceWaitCompleted { get; private set; } = false;
        private Coroutine _coForceWait = null;
        private IEnumerator CoForceWait()
        {
            ForceWaitCompleted = false;
            if (IsValidOwner == false)
                yield break;

            StopCoFindTargets();
            Owner.CreatureAIState = ECreatureAIState.Idle;
            yield return new WaitUntil(() => IsValidOwner && Owner.IsRunningAITick);
            yield return new WaitForSeconds(ReadOnly.Util.CoForceWaitTime);
            if (IsValidOwner == false)
                yield break;

            ForceWaitCompleted = true;
            StartCoFindTargets();
        }
        protected void StartCoForceWait()
        {
            if (_coForceWait != null)
                StopCoroutine(_coForceWait);
            
            _coForceWait = StartCoroutine(CoForceWait());
        }
    }
}
