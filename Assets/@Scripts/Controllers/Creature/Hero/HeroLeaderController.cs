using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroLeaderController : InitBase
    {
        private Transform _pointerPivot = null;
        private Transform _pointer = null;
        private Transform _leaderMark = null;
        [SerializeField] private Vector3 _nMovementDir = Vector3.zero;
        //[SerializeField] private EReplaceMode _replaceMode = EReplaceMode.FollowLeader;

        [SerializeField] private EHeroLeaderChaseMode _heroLeaderChaseMode = EHeroLeaderChaseMode.JustFollowClosely;
        public EHeroLeaderChaseMode HeroLeaderChaseMode
        {
            get => _heroLeaderChaseMode;
            set
            {
                if (_heroLeaderChaseMode != value)
                {
                    _heroLeaderChaseMode = value;
                    MoveStartMembersToTheirChasePos();
                }
            }
        }

        private void MoveStartMembersToTheirChasePos()
        {
            List<Hero> heroes = new List<Hero>();
            for (int i = 0; i < Managers.Object.Heroes.Count; ++i)
            {
                Hero hero = Managers.Object.Heroes[i];
                if (hero.IsValid() == false)
                    continue;

                heroes.Add(hero);
            }

            for (int i = 0; i < heroes.Count; ++i)
                heroes[i].CreatureState = ECreatureState.Move;
        }

        public void SetJustFollowClosely()
        {
            HeroLeaderChaseMode = EHeroLeaderChaseMode.JustFollowClosely;
        }

        public void SetNarrowFormation()
        {
            HeroLeaderChaseMode = EHeroLeaderChaseMode.NarrowFormation;
        }

        public Vector3Int RequestChaseCellPos(Hero heroMember)
        {
            if (heroMember.IsValid() == false)
                return Managers.Map.WorldToCell(_leader.transform.position);

            float distance = DevManager.Instance.TestReplaceDistance;
            int index = Managers.Object.Heroes.IndexOf(heroMember);
            int count = Managers.Object.Heroes.Count - 1;

            float angle = 360f * index / count;
            angle = Mathf.Deg2Rad * angle;

            float x = Leader.transform.position.x + Mathf.Cos(angle) * distance;
            float y = Leader.transform.position.y + Mathf.Sin(angle) * distance;
            return Managers.Map.WorldToCell(new Vector3(x, y, 0));
        }

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

            // 한 번만 설정해놓으면 됨, 히어로 자식으로 두면, Sorting Group 때문에 벽에 가려짐.
            _pointer.localPosition = Vector3.up * 3f;
            _leaderMark.localPosition = Vector2.up * 2f;

            Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;

            //_replaceMode = EReplaceMode.FocusingLeader;
            HeroLeaderChaseMode = EHeroLeaderChaseMode.JustFollowClosely;
            EnablePointer(false);
            return true;
        }

        // Leader가 걸을 때, 동료들의 위치를 업데이트해주면 될듯...??? 실시간이 아니라..

        public bool _waitFindPath = true;
        private Vector3 TargetPos => _leader.transform.position + (_nMovementDir * _leader.MovementSpeed * Time.deltaTime);
        private void Update() 
        {
            if (_leader == null)
                return;

            transform.position = _leader.CenterPosition;
            if (_waitFindPath && Managers.Map.CanMove(TargetPos, ignoreObjects: true))
            {
                Move();
            }
            else if (_waitFindPath)
            {
                _waitFindPath = false;
                Vector3Int currentCellPos = Managers.Map.WorldToCell(_leader.transform.position);
                Vector3Int pointerCellPos = Managers.Map.WorldToCell(_pointer.position);
                AddLeaderPath(currentCellPos, pointerCellPos);
                _coLeaderFindPath = StartCoroutine(CoLeaderFindPath());
            }
        }

        private Queue<Vector3Int> _leaderPath = new Queue<Vector3Int>();
        private void AddLeaderPath(Vector3Int startCellPos, Vector3Int targetCellPos)
        {
            List<Vector3Int> path = Managers.Map.FindPath(startCellPos, targetCellPos, maxDepth: ReadOnly.Numeric.HeroMaxMoveDepth);
            _leaderPath.Clear();
            for (int i = 0; i < path.Count; ++i)
                _leaderPath.Enqueue(path[i]);
            _leaderPath.Dequeue();
        }

        private Coroutine _coLeaderFindPath = null;
        private IEnumerator CoLeaderFindPath()
        {
            _leader.CreatureState = ECreatureState.Move;
            while (_leaderPath.Count != 0)
            {
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

                    // Path Finding 도중에 중간에 움직일 수 있는 곳이 발견되었을 경우 중지
                    if (Managers.Map.CanMove(TargetPos, ignoreObjects: true))
                        break;
                }
                else
                {
                    float moveDist = Mathf.Min(dir.magnitude, _leader.MovementSpeed * Time.deltaTime);
                    _leader.transform.position += dir.normalized * moveDist; // 한 번 이동은 완료한다.
                }

                yield return null;
            }

            _waitFindPath = true;
        }

        private void Move()
        {
            if (_nMovementDir == Vector3.zero)
            {
                if (_leader.CreatureState == ECreatureState.Move)
                    _leader.CreatureState = ECreatureState.Idle;
                return;
            }

            if (_nMovementDir.x < 0f)
                Leader.LookAtDir = ELookAtDirection.Left;
            else
                Leader.LookAtDir = ELookAtDirection.Right;

            Vector3Int targetCellPos = Managers.Map.WorldToCell(TargetPos);
            _leader.SetCellPos(cellPos: targetCellPos, stopLerpToCell: true, forceMove: false);
            _leader.transform.position = TargetPos;
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
                Managers.Map.RemoveObject(newLeader); // Set New Leader, Cell 위치는 무조건 동료들 것
            }
            else if (_leader != newLeader)
            {
                // Release Prev Leader
                _leader.IsLeader = false;
                // --> 아마 MoveTo로 바꿔야할수도 있음.
                Managers.Map.AddObject(obj: _leader, cellPos: Managers.Map.WorldToCell(_leader.transform.position));

                // Set New Leader, Cell 위치는 무조건 동료들 것
                Managers.Map.RemoveObject(newLeader);
                _leader = newLeader;
                _leader.IsLeader = true;
            }

            Managers.Object.CameraController.Target = newLeader;
        }

        private IEnumerator CoReadyToReplaceMembers()
        {
            while (true)
            {
                bool isAllStopped = true;
                for (int i = 0; i < Managers.Object.Heroes.Count; ++i)
                {
                    Hero hero = Managers.Object.Heroes[i];
                    if (hero == _leader)
                        continue;

                    if (hero.LerpToCellPosCompleted == false || hero.CreatureState == ECreatureState.Move)
                    {
                        isAllStopped = false;
                        break;
                    }
                }

                if (isAllStopped)
                    break;

                yield return null;
            }

            yield return null;
            // Managers.Game.ReplaceHeroes();
            // StartCoroutine(DevManager.Instance.CoUpdateReplacePosition()); --> 고민, 만들지 말지.
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
                case EJoystickState.Drag:
                    {
                        if (_leader.CreatureState != ECreatureState.Move)
                        {
                            EnablePointer(true);
                            _leader.CreatureState = ECreatureState.Move;
                        }
                    }
                    break;

                case EJoystickState.PointerUp:
                    {
                        _nMovementDir = Vector3.zero;
                        EnablePointer(false);
                        if (_coLeaderFindPath != null)
                        {
                            StopCoroutine(_coLeaderFindPath);
                            _coLeaderFindPath = null;
                            _waitFindPath = true;
                        }

                        _leader.CreatureState = ECreatureState.Idle;
                        // if (_replaceMode != EReplaceMode.FollowLeader)
                        //     StartCoroutine(CoReadyToReplaceMembers());
                    }
                    break;
            }
        }
        #endregion
    }
}