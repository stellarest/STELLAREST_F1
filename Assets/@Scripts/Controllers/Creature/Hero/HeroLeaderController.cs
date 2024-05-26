using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static STELLAREST_F1.Define;
using System.Linq;

namespace STELLAREST_F1
{
    // --- [MEMO]
    // -- 리더는 공격 도중(쿨타임시) 조작 불가능.
    // -- 옵션 플래그 하나 줘서, Scan Range에 들어오면 자동으로 추적하느냐, 안하느냐 넣을것.
    // - 리더 - 추적 or No. 멤버는 무조건 추적해야지.

    public class HeroLeaderController : InitBase
    {
        private Transform _pointerPivot = null;
        private Transform _pointer = null;
        private Transform _leaderMark = null;

        [SerializeField] private Vector3 _nMovementDir = Vector3.zero;
        [SerializeField] private EHeroLeaderChaseMode _heroLeaderChaseMode = EHeroLeaderChaseMode.JustFollowClosely;
        public EHeroLeaderChaseMode HeroLeaderChaseMode
        {
            get => _heroLeaderChaseMode;
            set
            {
                if (_heroLeaderChaseMode != value)
                {
                    if (value != EHeroLeaderChaseMode.RandomFormation)
                    {
                        if (_coRandomFormation != null)
                        {
                            StopCoroutine(_coRandomFormation);
                            _coRandomFormation = null;
                        }
                    }
                    else
                    {
                        if (_coRandomFormation == null)
                            _coRandomFormation = StartCoroutine(CoRandomFormation());
                    }

                    _heroLeaderChaseMode = value;
                    MoveStartMembersToTheirChasePos();
                }
            }
        }

        private void MoveStartMembersToTheirChasePos()
        {
            List<Hero> heroMembers = new List<Hero>();
            for (int i = 0; i < Managers.Object.Heroes.Count; ++i)
            {
                Hero hero = Managers.Object.Heroes[i];
                if (hero.IsValid() == false)
                    continue;

                if (hero.IsLeader)
                    continue;

                heroMembers.Add(hero);
            }

            for (int i = 0; i < heroMembers.Count; ++i)
                heroMembers[i].CreatureState = ECreatureState.Move;
        }

        [SerializeField] private EHeroMemberBattleMode _heroMemberBattleMode = EHeroMemberBattleMode.FollowLeader;
        public EHeroMemberBattleMode HeroMemberBattleMode => _heroMemberBattleMode;

        // TEMP
        public void SetJustFollowClosely()
            => HeroLeaderChaseMode = EHeroLeaderChaseMode.JustFollowClosely;
        
        // TEMP
        public void SetNarrowFormation()
            => HeroLeaderChaseMode = EHeroLeaderChaseMode.NarrowFormation;

        // TEMP
        public void SetWideFormation() 
            => HeroLeaderChaseMode = EHeroLeaderChaseMode.WideFormation;

        // TEMP
        public void SetPatrolFree()
            => HeroLeaderChaseMode = EHeroLeaderChaseMode.RandomFormation;

        public void ShuffleMembersPosition()
        {
            if (Managers.Object.Heroes.Count < 2)
                return;

            List<Hero> shuffleHeroes = Managers.Object.Heroes.Skip(1).ToList();
            System.Random rnd = new System.Random();
            shuffleHeroes = shuffleHeroes.OrderBy(n => rnd.Next()).ToList();

            for (int i = 0; i < shuffleHeroes.Count; ++i)
            {
                Managers.Object.Heroes[i + 1] = shuffleHeroes[i];
                Managers.Object.Heroes[i + 1].CreatureState = ECreatureState.Move;
            }
        }

        public Vector3Int RequestChaseCellPos(Hero heroMember)
        {
            if (heroMember.IsValid() == false) // 방어
                return Managers.Map.WorldToCell(_leader.transform.position);

            float distance = (float)_heroLeaderChaseMode;
            List<Hero> heroes = Managers.Object.Heroes;
            int index = heroes.IndexOf(heroMember);

            // Leader는 0번 인덱스에 있으므로, 다른 Hero들의 index를 1부터 시작하도록 조정
            int count = heroes.Count - 1;
            float angle = 360f * (index - 1) / count; // / 1부터 시작하도록 -1

            if (index == 0) // 방어
                return Managers.Map.WorldToCell(_leader.transform.position);

            angle = Mathf.Deg2Rad * angle;
            float x = _leader.transform.position.x + Mathf.Cos(angle) * distance;
            float y = _leader.transform.position.y + Mathf.Sin(angle) * distance;
            return Managers.Map.WorldToCell(new Vector3(x, y, 0));
        }

        public Vector3Int RequestChaseCellPos(Hero heroMember, float distance)
        {
            if (heroMember.IsValid() == false) // 방어
                return Managers.Map.WorldToCell(_leader.transform.position);

            List<Hero> heroes = Managers.Object.Heroes;
            int index = heroes.IndexOf(heroMember);

            // Leader는 0번 인덱스에 있으므로, 다른 Hero들의 index를 1부터 시작하도록 조정
            int count = heroes.Count - 1;
            float angle = 360f * (index - 1) / count; // / 1부터 시작하도록 -1

            if (index == 0) // 방어
                return Managers.Map.WorldToCell(_leader.transform.position);

            angle = Mathf.Deg2Rad * angle;
            float x = _leader.transform.position.x + Mathf.Cos(angle) * distance;
            float y = _leader.transform.position.y + Mathf.Sin(angle) * distance;
            return Managers.Map.WorldToCell(new Vector3(x, y, 0));
        }

        private Coroutine _coRandomFormation = null;
        private bool _randomPingPongFlag = false;
        private float _randomPingPongDistance = 0f;
        private IEnumerator CoRandomFormation()
        {
            float min = ReadOnly.Numeric.MinSecPatrolPingPong;
            float max = ReadOnly.Numeric.MaxSecPatrolPingPong;
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(min, max));
                _randomPingPongFlag = !_randomPingPongFlag;
                if (_randomPingPongFlag) // Wide Formation
                    _randomPingPongDistance = Random.Range(min, max);
                else // Narrow Formation
                    _randomPingPongDistance = Random.Range(0, min);

                ShuffleMembersPosition();
                MoveStartMembersToTheirChasePos();
            }
        }

        public Vector3Int RequestRandomChaseCellPos(Hero heroMember)
        {
            // 겹치는거 막기, FollowClosely에서도 겹치는거 막기. 단, Leader 위치 Add는 하지 않기.
            // 어쩌다가 겹치는것은 괜찮음. 그러나 자주 겹치면 안됨. 테스트 필요.
            if (_randomPingPongFlag == false)
                return RequestChaseCellPos(heroMember, (float)EHeroLeaderChaseMode.NarrowFormation + _randomPingPongDistance);
            else
                return RequestChaseCellPos(heroMember, (float)EHeroLeaderChaseMode.WideFormation + _randomPingPongDistance);
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

        public Vector3 LeaderDestPos
             => _leader.Target.IsValid() ? _leader.Target.transform.position : _pointer.position;

        public bool _waitFindPath = true;
        private Vector3 TargetPos => _leader.transform.position + (_nMovementDir * _leader.MovementSpeed * Time.deltaTime);
        private void Update() 
        {
            if (_leader == null)
                return;

            transform.position = _leader.CenterPosition;
            if (_leader.Target.IsValid() == false && _waitFindPath && Managers.Map.CanMove(TargetPos, ignoreObjects: true))
            {
                Move();
            }
            else if (_waitFindPath)
            {
                _waitFindPath = false;
                Vector3Int currentCellPos = Managers.Map.WorldToCell(_leader.transform.position);
                Vector3Int DestCellPos = Managers.Map.WorldToCell(LeaderDestPos);
                AddLeaderPath(currentCellPos, DestCellPos);
                _coLeaderFindPath = StartCoroutine(CoLeaderFindPath());
            }
        }

        private void LateUpdate()
        {
            if (_leader == null)
            {
                Debug.LogError("Failed to Leader Update Cell Pos. Please check leader character.");
                return;
            }

            _leader.UpdateCellPos();
        }
        
        private Queue<Vector3Int> _leaderPath = new Queue<Vector3Int>();
        private void AddLeaderPath(Vector3Int startCellPos, Vector3Int targetCellPos)
        {
            // Leader는 다른 Hero들 패스 가능!
            List<Vector3Int> path = Managers.Map.FindPath(startCellPos: startCellPos, 
                                                        destCellPos: targetCellPos, 
                                                        maxDepth: ReadOnly.Numeric.HeroMaxMoveDepth,
                                                        ignoreObjectType: EObjectType.Hero);
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
                // 임시 주석. 안해도 될 것 같아서.
                // if (_leader.CreatureState == ECreatureState.Move)
                //     _leader.CreatureState = ECreatureState.Idle;
                return;
            }

            if (_nMovementDir.x < 0f)
                Leader.LookAtDir = ELookAtDirection.Left;
            else
                Leader.LookAtDir = ELookAtDirection.Right;

            // 일반적인 이동은 무조건 부드럽게
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
            }
            else if (_leader != newLeader)
            {                
                _leader.IsLeader = false; // Release Prev Leader
                _leader = newLeader;
                _leader.IsLeader = true;
            }

            // Leader Hero는 항상 0번 인덱스로 변경
            List<Hero> heroes = Managers.Object.Heroes;
            if (heroes.Contains(newLeader))
                heroes.Remove(newLeader);
            heroes.Insert(0, newLeader);

            // Leader 제외 인덱스. 이미 따닥 따닥 붙어있을 때, 리더 변경시 멤버들을 Move상태로 만들지 않는다. 
            if (_heroLeaderChaseMode != EHeroLeaderChaseMode.JustFollowClosely)
            {
                for (int i = 1; i < heroes.Count; ++i)
                    heroes[i].CreatureState = ECreatureState.Move;
            }

            // 개발 중독...허허..
            newLeader.CreatureMoveState = ECreatureMoveState.None;
            newLeader.CreatureState = ECreatureState.Idle;
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
                        if (_coLeaderFindPath != null)
                        {
                            StopCoroutine(_coLeaderFindPath);
                            _coLeaderFindPath = null;
                            _waitFindPath = true;
                        }

                        _leader.CreatureMoveState = ECreatureMoveState.ForceMove;
                        _leader.CreatureState = ECreatureState.Move;
                        _leader.Target = null;
                        EnablePointer(true);
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

                        _leader.CreatureMoveState = ECreatureMoveState.None;
                        _leader.CreatureState = ECreatureState.Idle;
                        // if (_replaceMode != EReplaceMode.FollowLeader)
                        //     StartCoroutine(CoReadyToReplaceMembers());
                    }
                    break;
            }
        }
        #endregion

        private void OnDrawGizmos()
        {
            if (_leader == null)
                return;

            if (_heroLeaderChaseMode == EHeroLeaderChaseMode.JustFollowClosely)
                return;

            if (_heroLeaderChaseMode == EHeroLeaderChaseMode.RandomFormation)
                return;

            float distance = (float)_heroLeaderChaseMode;
            int memberCount = Managers.Object.Heroes.Count - 1;
            for (int i = 0; i < memberCount; ++i)
            {
                float degree = 360f * i / memberCount;
                degree = Mathf.PI / 180f * degree;
                float x = Leader.transform.position.x + Mathf.Cos(degree) * distance;
                float y = Leader.transform.position.y + Mathf.Sin(degree) * distance;

                Vector3Int cellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                Vector3 worldCenterPos = Managers.Map.CenteredCellToWorld(cellPos);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(worldCenterPos, radius: 0.5f);
            }
        }
    }
}

/*
    [Prev Note]
    // public Vector3Int RequestChaseCellPos(Hero heroMember) // Prev Ver
    // {
    //     if (heroMember.IsValid() == false)
    //         return Managers.Map.WorldToCell(_leader.transform.position);

    //     float distance = DevManager.Instance.TestLeaderChaseDistance;
    //     int index = Managers.Object.Heroes.IndexOf(heroMember);
    //     int count = Managers.Object.Heroes.Count - 1;

    //     float angle = 360f * index / count;
    //     angle = Mathf.Deg2Rad * angle;

    //     float x = Leader.transform.position.x + Mathf.Cos(angle) * distance;
    //     float y = Leader.transform.position.y + Mathf.Sin(angle) * distance;
    //     return Managers.Map.WorldToCell(new Vector3(x, y, 0));
    // }
*/