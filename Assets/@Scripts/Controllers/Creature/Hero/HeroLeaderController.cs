using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroLeaderController : InitBase
    {
        private Transform _pointerPivot = null;
        private Transform _pointer = null;
        public Vector3 PointerPos => _pointer.transform.position;

        private Transform _leaderMark = null;
        private Vector3 _nMovementDir = Vector3.zero; // normalized Vector

        private List<Vector3Int> _delta = new List<Vector3Int>()
        {
            // U기준, 시계 방향
            new Vector3Int(0, 1, 0), // U
            new Vector3Int(1, 1, 0), // UR
            new Vector3Int(1, 0, 0), // R
            new Vector3Int(1, -1, 0), // DR
            new Vector3Int(0, -1, 0), // D
            new Vector3Int(-1, -1, 0), // LD
            new Vector3Int(-1, 0, 0), // L
            new Vector3Int(-1, 1, 0) // LU
        };

        private Hero _leader = null;
        public Hero Leader
        {
            get => _leader;
            set => SetLeader(value);
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _pointerPivot = Util.FindChild<Transform>(gameObject, "PointerPivot");
            _pointer = Util.FindChild<Transform>(_pointerPivot.gameObject, "Pointer");
            _leaderMark = Util.FindChild<Transform>(gameObject, "LeaderMark");

            Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;
            return true;
        }

        // 천천히...

        Queue<Vector3Int> _leaderFindPath = new Queue<Vector3Int>();
        private Vector3Int _targetCellPos = Vector3Int.zero;
        private bool _findPath = true;
        private void Update()
        {
            if (_leader == null)
                return;

            
            Vector3 targetPos = _leader.transform.position + (_nMovementDir * _leader.MovementSpeed * Time.deltaTime);
            //Vector3Int targetCellPos = Managers.Map.WorldToCell(targetPos);
            _targetCellPos = Managers.Map.WorldToCell(targetPos);
            if (Managers.Map.CanMove(_targetCellPos))
                Move(targetPos);
            else
            {
                Vector3Int currentCellPos = Managers.Map.WorldToCell(_leader.transform.position);
                _targetCellPos = Managers.Map.WorldToCell(_pointer.position);
                if (_findPath)
                {
                    // 하나씩...
                    Leader.LerpToCellPosCompleted = false;
                    List<Vector3Int> path = Managers.Map.FindPath(startCellPos: currentCellPos, destCellPos: _targetCellPos,
                        maxDepth: ReadOnly.Numeric.HeroMaxMoveDepth);

                    for (int i = 0; i < path.Count; ++i)
                        _leaderFindPath.Enqueue(path[i]);
                    _leaderFindPath.Dequeue();
                    _findPath = false;
                    return;
                }
                else
                {
                    if (_leaderFindPath.Count > 0)
                    {
                        Vector3Int nextPos = _leaderFindPath.Peek();
                        Leader.LerpToCellPosCompleted = true;
                        if (Leader.MoveToCellPos(destCellPos: nextPos, maxDepth: 2))
                        {
                            _leaderFindPath.Dequeue();
                            return;
                        }
                    }
                    else
                    {
                        _findPath = true;
                        Leader.LerpToCellPosCompleted = false;
                    }

                }

                // 내일하자... Path하나씩 꺼내와야할듯...
                //_leader.FindPathAndMoveToCellPos(targetCellPos, ReadOnly.Numeric.HeroMaxMoveDepth);


                // Debug.Log("===== Check Path =====");
                // for (int i = 0; i < path.Count; ++i)
                // {
                //     Debug.Log($"[{i}]: {path[i]}");
                // }
            }

            // if (Managers.Map.CanMove(_predictedCellPos))
            //     Move(_nMovementDir * _leader.MovementSpeed * Time.deltaTime);
            // else
            //     Debug.Log("CAN'T MOVE !!");
        }

        private bool CanMove(Vector3 currentPos)
        {
            Vector3Int currentCellPos = Managers.Map.WorldToCell(currentPos);


            return false;
        }

        private void Move(Vector3 targetPos)
        {
            _leader.transform.position = targetPos;
            if (_nMovementDir == Vector3.zero)
                return;
            else if (_nMovementDir.x < 0f)
                Leader.LookAtDir = ELookAtDirection.Left;
            else
                Leader.LookAtDir = ELookAtDirection.Right;
        }

        // Slowly..
        private void SetLeader(Hero newLeader)
        {
            if (_leader == newLeader)
            {
                Debug.Log($"Maybe Same Leader, Current: {_leader.name}, new: {newLeader.name}");
                return;
            }
            else if (_leader == null)
            {
                _leader = newLeader;
                _leader.IsLeader = true;
                Managers.Map.RemoveObject(newLeader);
            }
            else if (_leader != newLeader)
            {
                _leader.IsLeader = false;
                Managers.Map.AddObject(obj: _leader, cellPos: Managers.Map.WorldToCell(_leader.transform.position));
                Managers.Map.RemoveObject(newLeader);
                _leader = newLeader;
                _leader.IsLeader = true;
            }

            Managers.Object.CameraController.Target = newLeader;
            transform.SetParent(newLeader.transform);
            transform.localPosition = newLeader.CenterLocalPosition;
            _pointer.localPosition = Vector3.up * 3f;
            _leaderMark.localPosition = Vector2.up * 2f;
            EnablePointer(false);
        }

        public void EnablePointer(bool enable)
            => StartCoroutine(CoEnablePointer(enable));

        private IEnumerator CoEnablePointer(bool enable)
        {
            yield return null;
            _pointer.gameObject.SetActive(enable);
        }


        #region Event
        private void OnMoveDirChanged(Vector2 nDir)
        {
            _nMovementDir = nDir;
            if (nDir != Vector2.zero)
            {
                float degree = Mathf.Atan2(-nDir.x, nDir.y) * (180f / Mathf.PI);
                _pointerPivot.rotation = Quaternion.Euler(0, 0, degree);
            }
        }

        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            switch (joystickState)
            {
                case EJoystickState.PointerDown:
                    EnablePointer(true);
                    break;

                case EJoystickState.PointerUp:
                    EnablePointer(false);
                    break;
            }
        }
        #endregion
    }
}
