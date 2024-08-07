using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static STELLAREST_F1.Define;
using System.Linq;
using UnityEngine.Analytics;
using UnityEditor;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEditor.Experimental.GraphView;

namespace STELLAREST_F1
{
    public class HeroLeaderController : InitBase
    {
        private Transform _pointerPivot = null;
        private Transform _pointer = null;
        private Transform _leaderMark = null;
        // --- Member들이 전투중에 Leader가 ForceMove를 시도하면 Env처럼 무조건 MoveState로 전환
        [field: SerializeField] public bool ForceFollowToLeader { get; set; } = false;

        [SerializeField] private Vector3 _nMovementDir = Vector3.zero;
        public Vector3 nLastMovementDir { get; private set;} = Vector3.zero;
 
        [SerializeField] private EHeroMemberFormationMode _heroMemberFormationMode = EHeroMemberFormationMode.FollowLeaderClosely;
        public EHeroMemberFormationMode HeroMemberFormationMode
        {
            get => _heroMemberFormationMode;
            set
            {
                if (_heroMemberFormationMode != value)
                {
                    // --- 이전의 _heroLeaderChaseMode가 ForceStop이었다면...
                    if (_heroMemberFormationMode == EHeroMemberFormationMode.ForceStop)
                    {
                        List<Hero> heroes = Managers.Object.Heroes;
                        for (int i = 0; i < heroes.Count; ++i)
                        {
                            if (heroes[i].IsValid() == false)
                                continue;

                            if (heroes[i].IsLeader)
                                continue;

                            // --- ForceStop이후 Leader에게 Warp하지 않고, 달려가서 쫓아가도록한다.
                            heroes[i].HeroAI.StartCoWaitForceStopWarp(ReadOnly.Util.WaitHeroesForceStopWarpSeconds);
                            //heroes[i].CreatureAIState = ECreatureAIState.Idle; // --- DEFENSE
                        }
                    }

                    if (value != EHeroMemberFormationMode.RandomFormation)
                        StopCoRandomFormation();
                    else
                    {
                        StartCoRandomFormation();
                    }

                    _heroMemberFormationMode = value;
                    if (value != EHeroMemberFormationMode.ForceStop || value != EHeroMemberFormationMode.RandomFormation)
                        MoveStartMembersToTheirChasePos();
                }
            }
        }

        private List<Hero> GetHeroMembers()
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

            return heroMembers;
        }

        private void MoveStartMembersToTheirChasePos()
        {
            List<Hero> members = GetHeroMembers();
            for (int i = 0; i < members.Count; ++i)
            {
                if (members[i].IsValid() == false)
                    continue;

                if (members[i].Target.IsValid())
                    continue;

                members[i].CreatureAIState = ECreatureAIState.Move;
            }
        }

        private int _changeFormation_Dev = 0;
        public void ChangeFormation_Dev()
        {
            switch (_changeFormation_Dev)
            {
                case (int)EHeroMemberFormationMode.FollowLeaderClosely:
                    HeroMemberFormationMode = EHeroMemberFormationMode.FollowLeaderClosely;
                    _changeFormation_Dev = (int)EHeroMemberFormationMode.NarrowFormation;
                    break;

                case (int)EHeroMemberFormationMode.NarrowFormation:
                    HeroMemberFormationMode = EHeroMemberFormationMode.NarrowFormation;
                    _changeFormation_Dev = (int)EHeroMemberFormationMode.WideFormation;
                    break;

                case (int)EHeroMemberFormationMode.WideFormation:
                    HeroMemberFormationMode = EHeroMemberFormationMode.WideFormation;
                    _changeFormation_Dev = (int)EHeroMemberFormationMode.RandomFormation;
                    break;

                case (int)EHeroMemberFormationMode.RandomFormation:
                    HeroMemberFormationMode = EHeroMemberFormationMode.RandomFormation;
                    _changeFormation_Dev = (int)EHeroMemberFormationMode.ForceStop;
                    break;

                case (int)EHeroMemberFormationMode.ForceStop:
                    {
                        List<Hero> heroes = Managers.Object.Heroes;
                        for (int i = 0; i < heroes.Count; ++i)
                        {
                            if (heroes[i].IsValid() == false)
                                continue;

                            if (heroes[i].IsLeader)
                                continue;

                            heroes[i].HeroAI.PrevCellPosForForceStop = heroes[i].CellPos;
                        }

                        HeroMemberFormationMode = EHeroMemberFormationMode.ForceStop;
                        _changeFormation_Dev = (int)EHeroMemberFormationMode.FollowLeaderClosely;
                    }
                    break;
            }

            Debug.Log($"<color=white>{HeroMemberFormationMode}</color>");
        }

        public void ShuffleMembersPosition()
        {
            if (Managers.Object.Heroes.Count <= 2)
            {
                Debug.Log($"<color=white>Hero Count: {Managers.Object.Heroes.Count}</color>");
                return;
            }

            // --- RandomFormation에서 멤버를 섞기 싫다면 추가
            if (HeroMemberFormationMode == EHeroMemberFormationMode.FollowLeaderClosely ||
                HeroMemberFormationMode == EHeroMemberFormationMode.ForceStop)
            {
                Debug.LogWarning("You have to set \"Narrow\", or \"Wide\" formation before.");
                return;
            }

            List<Hero> shuffleHeroes = Managers.Object.Heroes.Skip(1).ToList();
            System.Random rnd = new System.Random();
            shuffleHeroes = shuffleHeroes.OrderBy(n => rnd.Next()).ToList();

            for (int i = 0; i < shuffleHeroes.Count; ++i)
            {
                if (shuffleHeroes[i].IsValid() == false)
                    continue;

                if (shuffleHeroes[i].Target.IsValid())
                    continue;

                Managers.Object.Heroes[i + 1] = shuffleHeroes[i];
                Managers.Object.Heroes[i + 1].CreatureAIState = ECreatureAIState.Move;
            }

            Debug.Log($"<color=white>{nameof(ShuffleMembersPosition)}</color>");
        }

        public Vector3Int RequestFormationCellPos(Hero heroMember)
        {
            if (heroMember.IsValid() == false) // --- DEFENSE
                return Managers.Map.WorldToCell(_leader.transform.position);

            float distance = (float)_heroMemberFormationMode;
            List<Hero> heroes = Managers.Object.Heroes;
            int index = heroes.IndexOf(heroMember);

            // --- Leader는 항상 0번 인덱스에 있으므로, 다른 Hero들의 index를 1부터 시작하도록 조정
            int count = heroes.Count - 1;
            float angle = 360f * (index - 1) / count; // / 1부터 시작하도록 -1

            if (index == 0) // --- DEFENSE
                return Managers.Map.WorldToCell(_leader.transform.position);

            angle = Mathf.Deg2Rad * angle;
            float x = _leader.transform.position.x + Mathf.Cos(angle) * distance;
            float y = _leader.transform.position.y + Mathf.Sin(angle) * distance;
            return Managers.Map.WorldToCell(new Vector3(x, y, 0));
        }

        public Vector3Int RequestFormationCellPos(Hero heroMember, float distance)
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

        public Vector3Int RequestRandomFormationCellPos(Hero heroMember)
        {
            if (_randomPingPongFlag == false)
                return RequestFormationCellPos(heroMember, (float)EHeroMemberFormationMode.NarrowFormation + _randomPingPongDistance);
            else
                return RequestFormationCellPos(heroMember, (float)EHeroMemberFormationMode.WideFormation + _randomPingPongDistance);
        }

        private Hero _leader = null;
        public Hero Leader
        {
            get => _leader;
            set => SetLeader(value);
        }

        private void OnMovementDirChanged(Vector2 nDir)
        {
            if (_leader.IsValid() == false)
                return;

            _nMovementDir = nDir;
            if (nDir != Vector2.zero)
            {
                float degree = Mathf.Atan2(-nDir.x, nDir.y) * (180f / Mathf.PI);
                _pointerPivot.rotation = Quaternion.Euler(0, 0, degree);
                nLastMovementDir = nDir;
            }
        }

        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            Debug.Log("<color=yellow>Hero::OnJoystickStateChanged</color>");
            if (_leader.IsValid() == false)
                return;

            switch (joystickState)
            {
                case EJoystickState.Drag:
                    {
                        _leader.ForceMove = true;
                        EnablePointer(true);
                    }
                    break;

                case EJoystickState.PointerUp:
                    {
                        _leader.ForceMove = false;
                        EnablePointer(false);
                    }
                    break;
            }
        }

        private bool SkillOrCollectEnv()
        {
            if (_leader.CanSkill)
            {
                _leader.CreatureSkill.CurrentSkill.DoSkill();
                return true;
            }
            else if (_leader.CanCollectEnv)
            {
                _leader.CollectEnv();
                return true;
            }

            return false;
        }

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            GetComponent<SortingGroup>().sortingOrder = ReadOnly.SortingLayers.SLOrder_UI;

            _pointerPivot = Util.FindChild<Transform>(gameObject, "PointerPivot");
            _pointer = Util.FindChild<Transform>(_pointerPivot.gameObject, "Pointer");
            _leaderMark = Util.FindChild<Transform>(gameObject, "LeaderMark");

            _pointer.localPosition = Vector3.up * 3f;
            _leaderMark.localPosition = Vector2.up * 2f;

            Managers.Game.OnMoveDirChangedHandler -= OnMovementDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMovementDirChanged;

            HeroMemberFormationMode = EHeroMemberFormationMode.FollowLeaderClosely;
            ForceFollowToLeader = false;
            EnablePointer(false);
            return true;
        }

        // private IEnumerator Start()
        // {
        //     while (true)
        //     {
        //         // if (_leader.CanSkill)
        //         //     _leader.CreatureSkill.CurrentSkill.DoSkill();
        //         // if (_leader.CanSkill == false && _leader.CanCollectEnv)
        //         //     _leader.CollectEnv();

        //         yield return null;
        //     }
        // }

        private void Update()
        {
            if (_leader.IsValid() == false)
                return;

            if (SkillOrCollectEnv())
                return;

            if (_leader.ForceMove)
            {
                // --- Joystick Only
                if (_lockFindPath == false && Managers.Map.CanMove(GoToJoystickPos, ignoreObjectType: EObjectType.Hero))
                    Move();
                // --- Joystick + Path Finding
                else if (_lockFindPath == false && Managers.Map.CanMove(GoToJoystickPos, ignoreObjectType: EObjectType.Hero) == false)
                    TryPathFinding(_pointer.position);
            }
            else if (_lockFindPath == false)
            {
                if (_leader.Target.IsValid())
                {
                    // --- Go to Target
                    if (_leader.IsInTheNearestTarget == false)
                        TryPathFinding(_leader.Target.transform.position);
                    // --- Check Move To Current Cell Center / Stop when moving is not
                    else if (_leader.Moving == false)
                    {
                        //MoveToCurrentCellCenter();
                        MoveToNearCellCenter();
                    }
                    // --- Stop Moving is in the Nearest Target 
                    else
                        _leader.Moving = false;
                }
                else if (_leader.Target.IsValid() == false)
                {
                    // MoveToCurrentCellCenter();
                    MoveToNearCellCenter();
                }
                // -- Stop Moving in default when is not ForceMove
                else
                    _leader.Moving = false;
            }

        }

        private void LateUpdate()
        {
            if (_leader.IsValid() == false)
                return;

            LateTickLeaderPos();
        }
        #endregion

        private void MoveToNearCellCenter()
        {
            //Vector3 center = Managers.Map.GetCenterWorld(_leader.CellPos);
            Vector3Int nearCell = Managers.Map.WorldToCell(_leader.transform.position);
            Vector3 center = Managers.Map.GetCenterWorld(nearCell);
            Vector3 dir = center - _leader.transform.position;

            float threshold = 0.1f;
            if (dir.sqrMagnitude < threshold * threshold)
            {
                _leader.transform.position = center;
                _leader.Moving = false;
            }
            else
            {
                if (dir.x < 0f)
                    _leader.LookAtDir = ELookAtDirection.Left;
                else if (dir.x > 0f)
                    _leader.LookAtDir = ELookAtDirection.Right;

                _leader.transform.position += dir.normalized * _leader.MovementSpeed * Time.deltaTime;
            }
        }

        private void MoveToCurrentCellCenter()
        {
            Vector3 center = Managers.Map.GetCenterWorld(_leader.CellPos);
            Vector3 dir = center - _leader.transform.position;

            float threshold = 0.1f;
            if (dir.sqrMagnitude < threshold * threshold)
            {
                _leader.transform.position = center;
                _leader.Moving = false;

                // _nLastMovementDir로 Joystick LookAtTarget 정하면 되니까 갖고만 있기
                // if (_nLastMovementDir.x < 0f)
                // {
                //     _leader.LookAtDir = ELookAtDirection.Left;
                //     Debug.Log("LEFT");
                // }
                // else if (_nLastMovementDir.x > 0f)
                // {
                //     _leader.LookAtDir = ELookAtDirection.Right;
                //     Debug.Log("RIGHT");
                // }
            }
            else
            {
                if (dir.x < 0f)
                    _leader.LookAtDir = ELookAtDirection.Left;
                else if (dir.x > 0f)
                    _leader.LookAtDir = ELookAtDirection.Right;

                _leader.transform.position += dir.normalized * _leader.MovementSpeed * Time.deltaTime;
            }
        }

        private void TryPathFinding(Vector3 destPos)
        {
            _lockFindPath = true;
            Vector3Int currentCellPos = Managers.Map.WorldToCell(_leader.transform.position);
            Vector3Int destCellPos = Managers.Map.WorldToCell(destPos);
            int pathCount = AddPath(currentCellPos, destCellPos);
            if (pathCount == 0)
            {
                // --- 타겟이 존재할 경우, 이동하면서 공격
                // if (_leader.ForceMove == false && _leader.Target.IsValid() == false)
                //     _leader.Moving = false;

                _leader.Moving = false;
                _lockFindPath = false;
                return;
            }
            _coPathFinding = StartCoroutine(CoPathFinding());
        }

        public Vector3 PathFindingPos => _leader.Target.IsValid() ? _leader.Target.transform.position : _pointer.position;
        private Vector3 GoToJoystickPos => _leader.transform.position + (_nMovementDir * _leader.MovementSpeed * Time.deltaTime);
        private Queue<Vector3Int> _leaderPath = new Queue<Vector3Int>();

        private int AddPath(Vector3Int startCellPos, Vector3Int targetCellPos)
        {
            int depth = 2;
            if (_leader.Target.IsValid())
                depth = ReadOnly.Util.HeroDefaultMoveDepth;

            List<Vector3Int> path = Managers.Map.FindPath(startCellPos: startCellPos,
                                                        destCellPos: targetCellPos,
                                                        maxDepth: depth,
                                                        ignoreObjectType: EObjectType.Hero);
            _leaderPath.Clear();
            for (int i = 0; i < path.Count; ++i)
                _leaderPath.Enqueue(path[i]);
            _leaderPath.Dequeue();

            return _leaderPath.Count;
        }

        private void Move()
        {
            if (_leader.IsValid() == false)
                return;
            else if (_coPathFinding == null)
            {
                // --- MOVEMENT
                if (_leader.CanSkill == false)
                {
                    if (_nMovementDir.x < 0f)
                        Leader.LookAtDir = ELookAtDirection.Left;
                    else
                        Leader.LookAtDir = ELookAtDirection.Right;
                }

                _leader.Moving = true;
                _leader.transform.position = GoToJoystickPos;
            }
        }

        [SerializeField] private bool _lockFindPath = false; // Lock Find Path 용도 외에 건드리지 말것.
        [SerializeField] private bool _arriveToTarget = false;
        private void SetLeader(Hero newLeader)
        {
            if (_leader == newLeader)
            {
                Debug.Log($"Maybe Same Leader, Current: {_leader.name}, new: {newLeader.name}");
                return;
            }
            else if (_leader.IsValid() == false) // --- First Leader
            {
                _leader = newLeader;
                _leader.IsLeader = true;
            }
            else if (_leader != newLeader)
            {
                // --- Release Prev Leader
                _leader.StartCoUpdateAI();
                //_leader.HeroAI.StartSearchTarget(allTargetsCondition: null);
                _leader.HeroAI.StartCoFindEnemies();

                _leader.IsLeader = false;
                // _leader.NextCenteredCellPos = Managers.Map.CenteredCellPos(_leader.CellPos);
                // --- Set New Leader
                _leader = newLeader;
                _leader.IsLeader = true;
            }
            newLeader.StopCoUpdateAI();
            newLeader.StopCoLerpToCellPos();
            newLeader.HeroAI.StartCoFindEnemies();

            // Leader Hero는 항상 0번 인덱스로 변경
            List<Hero> heroes = Managers.Object.Heroes;
            if (heroes.Contains(newLeader))
                heroes.Remove(newLeader);
            heroes.Insert(index: 0, item: newLeader);

            // Leader 제외 인덱스. 이미 따닥 따닥 붙어있을 때, 리더 변경시 멤버들을 Move상태로 만들지 않는다. 
            if (_heroMemberFormationMode != EHeroMemberFormationMode.FollowLeaderClosely)
            {
                // for (int i = 1; i < heroes.Count; ++i)
                //     heroes[i].CreatureAIState = ECreatureAIState.Move;
            }

            // newLeader.CreatureMoveState = ECreatureMoveState.None;
            // newLeader.CreatureAIState = ECreatureAIState.Idle;

            // 일단 원인은 이게 맞았음
            // newLeader.CreatureAIState = ECreatureAIState.None; // --- 리더는 항상 None, 아니면 Dead
            Managers.Object.CameraController.Target = newLeader;
            EnableLeaderMark(true);
        }

        public void EnableLeaderMark(bool enable)
            => _leaderMark.gameObject.SetActive(enable);

        public void EnablePointer(bool enable)
            => _pointer.gameObject.SetActive(enable);

        private void LateTickLeaderPos()
        {
            // --- Sorting heroes in same cellpos for leader
            {
                BaseObject obj = Managers.Map.GetObject(_leader.CellPos);
                if (obj.IsValid() && obj.ObjectType == EObjectType.Hero)
                {
                    if (obj != _leader)
                    {
                        float distSQR = (obj.transform.position - _leader.transform.position).sqrMagnitude;
                        if (distSQR <= 0.4f * 0.4f)
                            obj.transform.position += Vector3.up * 0.01f;
                    }
                }
            }

            transform.position = _leader.CenterPosition;
            _leader.UpdateCellPos();
        }

        private Coroutine _coPathFinding = null;
        private IEnumerator CoPathFinding()
        {
            _leader.Moving = true;
            while (_leaderPath.Count != 0)
            {
                Vector3Int nextPos = _leaderPath.Peek();
                Vector3 destPos = Managers.Map.GetCenterWorld(nextPos);
                Vector3 dir = destPos - _leader.transform.position;
                if (dir.x < 0f)
                    _leader.LookAtDir = ELookAtDirection.Left;
                else if (dir.x > 0f)
                    _leader.LookAtDir = ELookAtDirection.Right;

                if (dir.sqrMagnitude < 0.001f)
                {
                    _leader.transform.position = destPos;
                    _leaderPath.Dequeue();
                    if (Managers.Map.CanMove(GoToJoystickPos, ignoreObjectType: EObjectType.Hero))
                    {
                        _lockFindPath = false;
                        StopCoPathFinding();
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

            // --- 타겟이 존재할 경우, 이동하면서 공격
            if (_leader.ForceMove == false && _leader.Target.IsValid() == false)
                _leader.Moving = false;

            _lockFindPath = false;
            StopCoPathFinding();
        }

        private void StopCoPathFinding()
        {
            if (_coPathFinding != null)
            {
                StopCoroutine(_coPathFinding);
                _coPathFinding = null;
            }
        }

        private Coroutine _coRandomFormation = null;
        private bool _randomPingPongFlag = false;
        private float _randomPingPongDistance = 0f;
        private IEnumerator CoRandomFormation()
        {
            float min = ReadOnly.Util.MinSecPatrolPingPong;
            float max = ReadOnly.Util.MaxSecPatrolPingPong;
            List<Hero> members = GetHeroMembers();
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
                yield return new WaitUntil(() =>
                {
                    bool isAllStop = true;
                    for (int i = 0; i < members.Count; ++i)
                    {
                        if (members[i].CreatureAIState == ECreatureAIState.Move)
                            isAllStop = false;
                    }

                    if (isAllStop)
                    {
                        Debug.Log("<color=yellow>IS ALL STOPPED !!</color>");
                        return true;
                    }

                    return false;
                });
                yield return null;
            }
        }

        public void StartCoRandomFormation()
        {
            if (_coRandomFormation == null)
                _coRandomFormation = StartCoroutine(CoRandomFormation());
        }

        public void StopCoRandomFormation()
        {
            if (_coRandomFormation != null)
            {
                StopCoroutine(_coRandomFormation);
                _coRandomFormation = null;
            }
        }

        public Coroutine _coActivateChangeLeaderCoolTime { get; private set; } = null;
        // 나중에 UI CoolTime을 표기하기 위한 Percentage
        public float CanChangeLeaderCurrentPercentage { get; private set; } = 0f;
        private IEnumerator CoActivateChangeLeaderCoolTime()
        {
            float delta = 0f;
            while (true)
            {
                delta += Time.deltaTime;
                CanChangeLeaderCurrentPercentage = delta / ReadOnly.Util.DesiredCanChangeLeaderTime;
                if (CanChangeLeaderCurrentPercentage >= 1f)
                    break;

                yield return null;
            }

            StopCoActivateChangeLeaderCoolTime();
        }

        public void StartCoActivateChangeLeaderCoolTime()
        {
            if (_coActivateChangeLeaderCoolTime == null)
                _coActivateChangeLeaderCoolTime = StartCoroutine(CoActivateChangeLeaderCoolTime());
        }

        public void StopCoActivateChangeLeaderCoolTime()
        {
            if (_coActivateChangeLeaderCoolTime != null)
            {
                StopCoroutine(_coActivateChangeLeaderCoolTime);
                _coActivateChangeLeaderCoolTime = null;
                CanChangeLeaderCurrentPercentage = 0f;
            }
        }

        private Coroutine _coChangeRandomHeroLeader = null;
        private IEnumerator CoChangeRandomHeroLeader()
        {
            if (Managers.Object.Heroes.Count == 1)
                yield break;

            int randIdx = UnityEngine.Random.Range(0, Managers.Object.Heroes.Count);
            Hero newHeroLeader = Managers.Object.Heroes[randIdx];
            Hero currentLeader = Managers.Object.HeroLeaderController.Leader;
            while (newHeroLeader == currentLeader || newHeroLeader.CreatureAIState == ECreatureAIState.Dead)
            {
                if (Managers.Object.Heroes.Count == 0)
                    StopCoChangeRandomHeroLeader(); // yield break 안해도됨. 알아서 코루틴이 종료가 바로 되버리고 또한 오히려 이렇게 관리하는게 더 좋은게
                // _coChangeRandomHeroLeader의 null여부를 통해서 코루틴 중복 여부를 체크할 수 있음.

                randIdx = UnityEngine.Random.Range(0, Managers.Object.Heroes.Count);
                newHeroLeader = Managers.Object.Heroes[randIdx];
                yield return null; // 무한루프 방지
            }

            if (Managers.Object.Heroes.Count > 0)
                Managers.Object.HeroLeaderController.Leader = newHeroLeader;

            StopCoChangeRandomHeroLeader();
        }

        public void StartCoChangeRandomHeroLeader()
        {
            if (_coChangeRandomHeroLeader == null)
                _coChangeRandomHeroLeader = StartCoroutine(CoChangeRandomHeroLeader());
        }

        public void StopCoChangeRandomHeroLeader()
        {
            if (_coChangeRandomHeroLeader != null)
            {
                StopCoroutine(_coChangeRandomHeroLeader);
                _coChangeRandomHeroLeader = null;
            }
        }

        private Coroutine _coIsAllHeroMembersStopped = null;
        private IEnumerator CoIsAllHeroMembersStopped(System.Action endCallback = null)
        {
            List<Hero> heroes = Managers.Object.Heroes;
            while (true)
            {
                bool isAllStopped = true;
                for (int i = 0; i < heroes.Count; ++i)
                {
                    Hero hero = heroes[i];
                    if (hero.IsLeader)
                        continue;

                    if (hero.IsValid() == false) // --- DEFENSE
                        continue;

                    // if (hero.LerpToCellPosCompleted == false || hero.CreatureAIState == ECreatureAIState.Move)
                    // {
                    //     isAllStopped = false;
                    //     break;
                    // }
                }

                if (isAllStopped)
                    break;

                yield return null;
            }

            yield return null; // --- DEFENSE
            endCallback?.Invoke();
            StopCoIsAllHeroMembersStopped();
        }

        public void StartCoIsAllHeroMembersStopped(System.Action endCallback = null)
        {
            if (_coIsAllHeroMembersStopped == null)
                _coIsAllHeroMembersStopped = StartCoroutine(CoIsAllHeroMembersStopped(endCallback));
        }

        private void StopCoIsAllHeroMembersStopped()
        {
            if (_coIsAllHeroMembersStopped != null)
            {
                StopCoroutine(_coIsAllHeroMembersStopped);
                _coIsAllHeroMembersStopped = null;
            }
        }

#if UNITY_EDITOR
        public float ScanRange = 1f;
        private void OnDrawGizmos()
        {
            if (_leader == null)
                return;

            // --- Check Dist
            // -- center to center: 1.5
            // -- center to edge: 2
            // Gizmos.color = Color.red;

            // Vector3 leaderPos = _leader.transform.position;
            // Vector3 target = Managers.Map.CenteredCellToWorld(new Vector3Int(1, 0, 0));
            // //Vector3 target = leaderPos + new Vector3((int)_leader.LookAtDir, 0, 0);
            // target = new Vector3(target.x * ScanRange, target.y, 0);

            // Gizmos.DrawLine(leaderPos, target);
            // GUIStyle style = new GUIStyle();
            // style.normal.textColor = Color.white;
            // Handles.Label(leaderPos + (Vector3.up * 3f), $"mag: {(leaderPos - target).magnitude:F2}", style);
            // style.normal.textColor = Color.blue;
            // Handles.Label(leaderPos + (Vector3.up * 5f), $"sqrMag: {(leaderPos - target).sqrMagnitude:F2}", style);


            // --- Draw Formation Point
            // if (_leader == null)
            //     return;

            // if (_heroMemberFormationMode == EHeroMemberFormationMode.FollowLeaderClosely)
            //     return;

            // if (_heroMemberFormationMode == EHeroMemberFormationMode.RandomFormation)
            //     return;

            // if (_heroMemberFormationMode == EHeroMemberFormationMode.ForceStop)
            //     return;

            // float distance = (float)_heroMemberFormationMode;
            // int memberCount = Managers.Object.Heroes.Count - 1;
            // for (int i = 0; i < memberCount; ++i)
            // {
            //     float degree = 360f * i / memberCount;
            //     degree = Mathf.PI / 180f * degree;
            //     float x = Leader.transform.position.x + Mathf.Cos(degree) * distance;
            //     float y = Leader.transform.position.y + Mathf.Sin(degree) * distance;

            //     Vector3Int cellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
            //     Vector3 worldCenterPos = Managers.Map.CenteredCellToWorld(cellPos);

            //     Gizmos.color = Color.red;
            //     Gizmos.DrawSphere(worldCenterPos, radius: 0.5f);
            // }
        }
#endif
    }
}

/*
        [PREV]
        // --- PREV UPDATE
        // private void Update()
        // {
        //     if (_leader == null)
        //         return;

        //     // --- ForceExitState() 때문에 CreatureMoveState가 None으로 변할것임. 
        //     // --- 근데 이미 Target을 서칭 중이라 다시 CreatureMoveState가 TargetToEnemy로 변함.
        //     if (_leader.CreatureMoveState != ECreatureMoveState.ForceMove)
        //     {
        //         if (_leader.CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
        //             return;
        //     }

        //     if (_leader.Target.IsValid() == false && _lockFindPath == false && Managers.Map.CanMove(JoystickPos, ignoreObjects: true))
        //     {
        //         Move();
        //     }
        //     else if (_lockFindPath == false && _arriveToTarget == false)
        //     {
        //         _lockFindPath = true;
        //         Vector3Int currentCellPos = Managers.Map.WorldToCell(_leader.transform.position);
        //         Vector3Int destCellPos = Managers.Map.WorldToCell(LeaderDestPos);
        //         AddLeaderPath(currentCellPos, destCellPos);
        //         // --- AddLeaderPath에서 _leaderPath.Dequeue()로 인해 시작점에 그대로 있을때는 현재 서있는 위치에서 그대로 스킬 실행.
        //         // ECreatureMoveState.MoveToTarget 상태에서만 실행되므로 문제 없음.
        //         if (_leaderPath.Count == 0 && _leader.CreatureMoveState == ECreatureMoveState.MoveToTarget)
        //         {
        //             _lockFindPath = false; // 추가
        //             return;
        //         }

        //         _coLeaderPathFinding = StartCoroutine(CoLeaderPathFinding());
        //     }
        // }

        // --- PREV LATE UPDATE
        // private void LateUpdate()
        // {
        //     if (_leader == null)
        //     {
        //         Debug.LogError("Failed to Leader Update Cell Pos. Please check leader character.");
        //         return;
        //     }

        //     LateTickLeaderPos();
        //     if (_leader.CreatureMoveState == ECreatureMoveState.MoveToTarget || _leader.CreatureMoveState == ECreatureMoveState.ForceMove) // 처음엔 이동이겠지만, 그 다음은 Idle이 된다.
        //     {
        //         if (_leader.Target.IsValid())
        //         {
        //             EObjectType targetObjectType = _leader.Target.ObjectType;
        //             if (_leader.CanAttackOrChase(EObjectType.Monster) && _arriveToTarget == false) // 2칸 이상일 때, 대각선, 상하좌우포함
        //             {
        //                 Debug.Log("A");
        //                 // ***** 최초 공격 시도 *****
        //                 if (_leaderPath.Count >= 1) // 2 - 대각선, 1 - 상하좌우
        //                 {
        //                     Vector3 centeredPos = Managers.Map.CenteredCellToWorld(_leaderPath.Peek());
        //                     if ((centeredPos - _leader.transform.position).sqrMagnitude < 0.01f)
        //                     {
        //                         if (targetObjectType == EObjectType.Monster && IsSkillCoolTimeReady(ESkillType.Skill_Attack))
        //                         {
        //                             _leader.CreatureSkill?.CurrentSkill.DoSkill();
        //                         }
        //                         else if (targetObjectType == EObjectType.Env)
        //                         {
        //                             _leader.LookAtValidTarget();
        //                             _leader.CreatureState = ECreatureAIState.CollectEnv;                                    
        //                         }

        //                         _arriveToTarget = true;
        //                         //Debug.Log("First attack.");
        //                     }
        //                 }
        //                 // ***** 다만 Pointer Up을 하게 되면 _arriveToTarget가 false가 되는데, 이때 Target의 근처에 있다면 이쪽으로 오게됨 *****
        //                 // ***** Pointer Up, 또는 타겟이 존재하지 않을 경우 _arriveToTarget가 false이고 path count가 0이면 일로옴 *****
        //                 else if (_leaderPath.Count == 0 && _arriveToTarget == false)
        //                 {
        //                     Debug.Log("B");
        //                     if (targetObjectType == EObjectType.Monster && IsSkillCoolTimeReady(ESkillType.Skill_Attack))
        //                     {
        //                         _leader.CreatureSkill?.CurrentSkill.DoSkill(); // Skill -> Idle
        //                     }
        //                     else if (targetObjectType == EObjectType.Env)
        //                     {
        //                         _leader.LookAtValidTarget();
        //                         _leader.CreatureState = ECreatureAIState.CollectEnv;
        //                     }

        //                     //Debug.Log("Is in close with target.");
        //                 }
        //             }
        //             // ***** 최초 이후의 공격은 이쪽으로 실행하게 됨, 여전히 범위내에 있다면 쿨타임 기다렸다가 공격 *****
        //             else if (_arriveToTarget)
        //             {
        //                 Debug.Log("C");
        //                 if (targetObjectType == EObjectType.Monster && IsSkillCoolTimeReady(ESkillType.Skill_Attack))
        //                 {
        //                     _leader.CreatureSkill?.CurrentSkill.DoSkill(); // Skill -> Idle
        //                 }
        //                 else if (targetObjectType == EObjectType.Env)
        //                 {
        //                     _leader.LookAtValidTarget();
        //                     _leader.CreatureState = ECreatureAIState.CollectEnv;
        //                 }

        //                 //Debug.Log("Just stop and attack.");
        //             }
        //             else
        //             {
        //                 // ***** 여기는 필요 없음 !! *****
        //                 // 어떠한 사유로 타겟과 멀어졌다면 _arriveToTarget flag를 false로 돌릴 준비.
        //                 // --> PointerUp이 되면 무조건 false로 변경. 움직인 것이므로 의미상 _arriveToTarget이 아니게 됨.
        //             }
        //         }
        //         else
        //         {

        //             if (_leader.CanAttackOrChase(EObjectType.Monster) && _arriveToTarget == false)
        //             {
        //                 if (IsSkillCoolTimeReady(ESkillType.Skill_Attack))
        //                      _leader.CreatureSkill?.CurrentSkill.DoSkill();
        //                 else if (_leader.CreatureMoveState == ECreatureMoveState.ForceMove)
        //                     _leader.CreatureUpperState = ECreatureUpperAnimState.UA_Move;
        //                 else
        //                     _leader.CreatureUpperState = ECreatureUpperAnimState.UA_Idle;
        //             }

        //             // _arriveToTarget = false;
        //             // if (_leader.CollectEnv)
        //             // {
        //             //     _leader.CollectEnv = false;
        //             //     _leader.CreatureMoveState = ECreatureMoveState.None;
        //             //     _leader.CreatureState = ECreatureAIState.Idle;
        //             // }
        //         }
        //     }
        // }

        // PREV - OnJoystickChanged
        // private void OnJoystickStateChanged(EJoystickState joystickState)
        // {
        //     if (_leader.IsValid() == false) // --- DEFENSE
        //         return;

        //     if (_leader.CreatureState == ECreatureAIState.Dead)
        //         return;

        //     switch (joystickState)
        //     {
        //         case EJoystickState.Drag:
        //             {
        //                 // 이미 진행중인 PathFinding은 멈추지 않는다.
        //                 if (_coLeaderPathFinding != null && _lockFindPath == false)
        //                 {
        //                     StopCoroutine(_coLeaderPathFinding);
        //                     _coLeaderPathFinding = null;
        //                 }

        //                 // 무기 되돌려놓기
        //                 if (_leader.CollectEnv)
        //                     _leader.CollectEnv = false;

        //                 _leader.CreatureMoveState = ECreatureMoveState.ForceMove;
        //                 _leader.CreatureState = ECreatureAIState.Move;

        //                 if (_leader.CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack) == false)
        //                     _leader.CreatureUpperAnimState = ECreatureUpperAnimState.UA_Move;

        //                 _leader.CreatureLowerAnimState = ECreatureLowerAnimState.LA_Move;
        //                 _leader.Target = null;
        //                 EnablePointer(true);
        //             }
        //             break;

        //         case EJoystickState.PointerUp:
        //             {
        //                 _nMovementDir = Vector3.zero;
        //                 EnablePointer(false);
        //                 _arriveToTarget = false; // 이동했다가 마우스를 놓으면 어쨋든 false로 돌려 놓는다.
        //                 if (_coLeaderPathFinding != null && _lockFindPath == false) // 이미 진행중인 PathFinding은 멈추지 않는다.
        //                 {
        //                     StopCoroutine(_coLeaderPathFinding);
        //                     _coLeaderPathFinding = null;
        //                 }

        //                 if (_coLeaderPathFinding == null)
        //                 {
        //                     _leader.CreatureState = ECreatureAIState.Idle;
        //                     _leader.CreatureUpperAnimState = ECreatureUpperAnimState.UA_Idle;
        //                     _leader.CreatureLowerAnimState = ECreatureLowerAnimState.LA_Idle;
        //                 }

        //                 _leader.CreatureMoveState = ECreatureMoveState.None;
        //             }
        //             break;
        //     }
        // }

        // --- MOVE PREV
        // private void Move()
        // {
        //     if (_leader.IsValid() == false)
        //     {
        //         _nMovementDir = Vector2.zero;
        //     }
        //     else
        //     {
        //         if (_nMovementDir == Vector3.zero)
        //         {
        //             if (_leader.CreatureState == ECreatureAIState.Move) // --- DEFENSE
        //                 _leader.CreatureState = ECreatureAIState.Idle;
        //             return;
        //         }

        //         if (_nMovementDir.x < 0f)
        //             Leader.LookAtDir = ELookAtDirection.Left;
        //         else
        //             Leader.LookAtDir = ELookAtDirection.Right;

        //         _leader.transform.position = JoystickPos;
        //     }
        // }

        // public Vector3 PathFindingPos
        //      => _leader.Target.IsValid() ? _leader.Target.transform.position : _pointer.position;

        // private bool CanSkillAttack()
        // {
        //     if (_leader.Target.IsValid() && _leader.IsInAttackRange_Temp())
        //     {
        //         if (_leader.Target.ObjectType == EObjectType.Monster)
        //         {
        //             if (_leader.CreatureSkill.IsRemainingCoolTime(ESkillType.Skill_Attack) == false)
        //                 return true;
        //         }
        //         else if (_leader.Target.ObjectType == EObjectType.Env)
        //             return true;
        //     }

        //     return false;
        // }
*/