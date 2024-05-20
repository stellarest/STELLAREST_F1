using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // Hero AI랑 별도로 동작, slowly...
    public class HeroLeaderController : InitBase
    {
        private Transform _pointerPivot = null;
        private Transform _pointer = null;
        private Transform _leaderMark = null;
        private Vector3 _nMovementDir = Vector3.zero; // normalized Vector

        private Hero _leader = null;
        public Hero Leader
        {
            get => _leader;
            set => SetLeader(value);
        }
        public Vector3 LeaderPos => _leader.transform.position;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _pointerPivot = Util.FindChild<Transform>(gameObject, "PointerPivot");
            _pointer = Util.FindChild<Transform>(_pointerPivot.gameObject, "Pointer");
            _leaderMark = Util.FindChild<Transform>(gameObject, "LeaderMark");

            // 한 번만 설정해놓으면 됨
            // 히어로 자식으로 두면, Sorting Group 때문에 벽에 가려짐.
            _pointer.localPosition = Vector3.up * 3f;
            _leaderMark.localPosition = Vector2.up * 2f;

            Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;
            return true;
        }

        private Vector3 TargetPos => _leader.transform.position + (_nMovementDir * _leader.MovementSpeed * Time.deltaTime);
        private Queue<Vector3Int> _leaderPath = new Queue<Vector3Int>();
        [SerializeField] public bool _leaderFindPathFlag = true;
        private Coroutine _coLeaderFindPath = null;

        private void Update() 
        {
            if (_leader == null)
                return;

            transform.position = _leader.CenterPosition;
            if (Managers.Map.CanMove(TargetPos) && _leaderFindPathFlag)
            {
                // if (_coLeaderFindPath != null)
                // {
                //     StopCoroutine(_coLeaderFindPath);
                //     _coLeaderFindPath = null;
                //     _leaderFindPathFlag = true;
                // }

                //Debug.Log("Normal Move");
                Move(TargetPos);
            }
            else if (_leaderFindPathFlag) // A* 강제
            {
                //Debug.Log("Find Path");
                _leaderFindPathFlag = false;
                Vector3Int currentCellPos = Managers.Map.WorldToCell(_leader.transform.position);
                Vector3Int targetPointerCellPos = Managers.Map.WorldToCell(_pointer.position);

                List<Vector3Int> path = Managers.Map.FindPath(startCellPos: currentCellPos, destCellPos: targetPointerCellPos,
                        maxDepth: ReadOnly.Numeric.HeroMaxMoveDepth);

                if (path != null && path.Count > 0)
                {
                    _leaderPath.Clear();
                    foreach (var cell in path)
                        _leaderPath.Enqueue(cell);
                    _leaderPath.Dequeue();

                    _coLeaderFindPath = StartCoroutine(CoLeaderFindPath());
                }
            }
        }

        private void RetracePath()
        {
            Vector3Int currentCellPos = Managers.Map.WorldToCell(_leader.transform.position);
            Vector3Int targetPointerCellPos = Managers.Map.WorldToCell(_pointer.position);
            List<Vector3Int> path = Managers.Map.FindPath(startCellPos: currentCellPos, destCellPos: targetPointerCellPos,
        maxDepth: ReadOnly.Numeric.HeroMaxMoveDepth);

            if (path != null && path.Count > 0)
            {
                _leaderPath.Clear();
                foreach (var cell in path)
                    _leaderPath.Enqueue(cell);
                _leaderPath.Dequeue();
            }
        }

        private IEnumerator CoLeaderFindPath()
        {
            _leader.CreatureState = ECreatureState.Move;
            while (_leaderPath.Count != 0)
            {
                // RetracePath(); // RetracePath를 조금 건드리면 될것같긴한데
                // if (_leaderPath.Count == 0)
                // {
                //     yield break;
                // }

                Vector3Int nextPos = _leaderPath.Peek();
                Vector3 destPos = Managers.Map.CenteredCellToWorld(nextPos);
                Vector3 dir = destPos - _leader.transform.position;

                if (dir.x < 0f)
                    _leader.LookAtDir = ELookAtDirection.Left;
                else if (dir.x > 0f)
                    _leader.LookAtDir = ELookAtDirection.Right;

                if (dir.sqrMagnitude < 0.001f)
                {
                    _leader.transform.position = destPos;
                    _leaderPath.Dequeue();
                    if (Managers.Map.CanMoveDeltaPos(_leader) == false)
                    {
                        Debug.Log("### OUT COROUTINE ###");
                        break;
                    }
                }
                else
                {
                    float moveDist = Mathf.Min(dir.magnitude, _leader.MovementSpeed * Time.deltaTime);
                    _leader.transform.position += dir.normalized * moveDist; // 한 번 이동은 완료한다.
                }

                yield return null;
            }

            _leaderFindPathFlag = true;
            if (_nMovementDir.x == 0f)
                _leader.CreatureState = ECreatureState.Idle;

            Debug.Log("End Coroutine.");
        }

        private void Move(Vector3 targetPos)
        {
            if (_nMovementDir == Vector3.zero)
                return;

            if (_nMovementDir.x < 0f)
                Leader.LookAtDir = ELookAtDirection.Left;
            else
                Leader.LookAtDir = ELookAtDirection.Right;

            _leader.transform.position = targetPos;
        }

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
                // Release Prev Leader
                _leader.IsLeader = false;
                Managers.Map.AddObject(obj: _leader, cellPos: Managers.Map.WorldToCell(_leader.transform.position));

                // Set Leader
                Managers.Map.RemoveObject(newLeader);
                _leader = newLeader;
                _leader.IsLeader = true;
            }

            Managers.Object.CameraController.Target = newLeader;
            EnablePointer(false);
        }

        public void EnablePointer(bool enable)
            => _pointer.gameObject.SetActive(enable);

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

                case EJoystickState.Drag:
                    {
                        // Drag 중일때는 무조건 Move
                        if (_leader.CreatureState != ECreatureState.Move)
                            _leader.CreatureState = ECreatureState.Move;
                    }
                    break;

                case EJoystickState.PointerUp:
                    {
                        EnablePointer(false);
                        if (_coLeaderFindPath != null)
                        {
                            StopCoroutine(_coLeaderFindPath);
                            _coLeaderFindPath = null;
                            _leaderFindPathFlag = true;
                        }

                        _leader.CreatureState = ECreatureState.Idle;
                    }
                    break;
            }
        }
        #endregion
    }
}