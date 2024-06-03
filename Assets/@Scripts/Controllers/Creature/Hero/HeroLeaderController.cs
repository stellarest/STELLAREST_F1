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
    public class HeroLeaderController : InitBase
    {
        private Transform _pointerPivot = null;
        private Transform _pointer = null;
        private Transform _leaderMark = null;

        [SerializeField] private Vector3 _nMovementDir = Vector3.zero;
        [SerializeField] private EHeroLeaderChaseMode _heroLeaderChaseMode = EHeroLeaderChaseMode.JustFollowClosely;

        public float ForceStopWarpSeconds = 0f;

        public EHeroLeaderChaseMode HeroLeaderChaseMode
        {
            get => _heroLeaderChaseMode;
            set
            {
                if (_heroLeaderChaseMode != value)
                {
                    // 이전의 _heroLeaderChaseMode가 ForceStop이었다면...
                    if (_heroLeaderChaseMode == EHeroLeaderChaseMode.ForceStop)
                    {
                        List<Hero> heroes = Managers.Object.Heroes;
                        for (int i = 0; i < heroes.Count; ++i)
                        {
                            if (heroes[i].IsValid() == false)
                                continue;

                            if (heroes[i].IsLeader)
                                continue;

                            heroes[i].StartCoWaitForceStopWarp(ReadOnly.Numeric.WaitHeroesForceStopWarpSeconds);
                        }
                    }

                    if (value != EHeroLeaderChaseMode.RandomFormation)
                    {
                        StopCoRandomFormation();
                        // if (_coRandomFormation != null)
                        // {
                        //     StopCoroutine(_coRandomFormation);
                        //     _coRandomFormation = null;
                        // }
                    }
                    else
                    {
                        StartCoRandomFormation();
                        // if (_coRandomFormation == null)
                        //     _coRandomFormation = StartCoroutine(CoRandomFormation());
                    }

                    _heroLeaderChaseMode = value;
                    if (value != EHeroLeaderChaseMode.ForceStop)
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

        [SerializeField] private EHeroMemberBattleMode _heroMemberBattleMode = EHeroMemberBattleMode.EngageEnemy;
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
        public void SetRandomFormation()
            => HeroLeaderChaseMode = EHeroLeaderChaseMode.RandomFormation;

        public void SetForceStop()
        {
            List<Hero> heroes = Managers.Object.Heroes;
            for (int i = 0; i < heroes.Count; ++i)
            {
                if (heroes[i].IsValid() == false)
                    continue;

                if (heroes[i].IsLeader)
                    continue;

                heroes[i].PrevCellPosForForceStop = heroes[i].CellPos;
            }

            HeroLeaderChaseMode = EHeroLeaderChaseMode.ForceStop;
        }

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
            _heroMemberBattleMode = EHeroMemberBattleMode.EngageEnemy;
            EnablePointer(false);
            return true;
        }

        public Vector3 LeaderDestPos
             => _leader.Target.IsValid() ? _leader.Target.transform.position : _pointer.position;

        private Vector3 JoystickPos => _leader.transform.position + (_nMovementDir * _leader.MovementSpeed * Time.deltaTime);
        private void Update()
        {
            if (_leader == null)
                return;

            // --- ForceExitState() 때문에 CreatureMoveState가 None으로 변할것임. 
            // --- 근데 이미 Target을 서칭 중이라 다시 CreatureMoveState가 TargetToEnemy로 변함.
            if (_leader.CreatureMoveState != ECreatureMoveState.ForceMove)
            {
                if (_leader.CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
                    return;
            }

            if (_leader.Target.IsValid() == false && _lockFindPath == false && Managers.Map.CanMove(JoystickPos, ignoreObjects: true))
            {
                Move();
            }
            else if (_lockFindPath == false && _arriveToTarget == false)
            {
                _lockFindPath = true;
                Vector3Int currentCellPos = Managers.Map.WorldToCell(_leader.transform.position);
                Vector3Int destCellPos = Managers.Map.WorldToCell(LeaderDestPos);
                AddLeaderPath(currentCellPos, destCellPos);
                // // --- AddLeaderPath에서 _leaderPath.Dequeue()로 인해 시작점에 그대로 있을때는 현재 서있는 위치에서 그대로 스킬 실행.
                // ECreatureMoveState.MoveToTarget 상태에서만 실행되므로 문제 없음.
                if (_leaderPath.Count == 0 && _leader.CreatureMoveState == ECreatureMoveState.MoveToTarget)
                {
                    _lockFindPath = false; // 추가
                    return;
                }

                _coLeaderPathFinding = StartCoroutine(CoLeaderPathFinding());
            }
        }

        [SerializeField] private bool _lockFindPath = false; // Lock Find Path 용도 외에 건드리지 말것.
        [SerializeField] private bool _arriveToTarget = false;
        private void LateUpdate()
        {
            if (_leader == null)
            {
                Debug.LogError("Failed to Leader Update Cell Pos. Please check leader character.");
                return;
            }

            LateTickLeaderPos();
            if (_leader.CreatureMoveState == ECreatureMoveState.MoveToTarget) // 처음엔 이동이겠지만, 그 다음은 Idle이 된다.
            {
                if (_leader.Target.IsValid())
                {
                    EObjectType targetObjectType = _leader.Target.ObjectType;
                    if (_leader.CanAttackOrChase() && _arriveToTarget == false) // 2칸 이상일 때, 대각선, 상하좌우포함
                    {
                        // ***** 최초 공격 시도 *****
                        if (_leaderPath.Count >= 1) // 2 - 대각선, 1 - 상하좌우
                        {
                            Vector3 centeredPos = Managers.Map.CenteredCellToWorld(_leaderPath.Peek());
                            if ((centeredPos - _leader.transform.position).sqrMagnitude < 0.01f)
                            {
                                if (targetObjectType == EObjectType.Monster && IsSkillCoolTimeReady(ESkillType.Skill_Attack))
                                {
                                    _leader.CreatureSkill?.CurrentSkill.DoSkill(); // Skill -> Idle
                                }
                                else if (targetObjectType == EObjectType.Env)
                                {
                                    _leader.LookAtValidTarget();
                                    _leader.CreatureState = ECreatureState.CollectEnv;                                    
                                }

                                _arriveToTarget = true;
                                //Debug.Log("First attack.");
                            }
                        }
                        // ***** 다만 Pointer Up을 하게 되면 _arriveToTarget가 false가 되는데, 이때 Target의 근처에 있다면 이쪽으로 오게됨 *****
                        // ***** Pointer Up, 또는 타겟이 존재하지 않을 경우 _arriveToTarget가 false이고 path count가 0이면 일로옴 *****
                        else if (_leaderPath.Count == 0 && _arriveToTarget == false)
                        {
                            if (targetObjectType == EObjectType.Monster && IsSkillCoolTimeReady(ESkillType.Skill_Attack))
                            {
                                _leader.CreatureSkill?.CurrentSkill.DoSkill(); // Skill -> Idle
                            }
                            else if (targetObjectType == EObjectType.Env)
                            {
                                _leader.LookAtValidTarget();
                                _leader.CreatureState = ECreatureState.CollectEnv;
                            }

                            //Debug.Log("Is in close with target.");
                        }
                    }
                    // ***** 최초 이후의 공격은 이쪽으로 실행하게 됨, 여전히 범위내에 있다면 쿨타임 기다렸다가 공격 *****
                    else if (_arriveToTarget)
                    {
                        if (targetObjectType == EObjectType.Monster && IsSkillCoolTimeReady(ESkillType.Skill_Attack))
                        {
                            _leader.CreatureSkill?.CurrentSkill.DoSkill(); // Skill -> Idle
                        }
                        else if (targetObjectType == EObjectType.Env)
                        {
                            _leader.LookAtValidTarget();
                            _leader.CreatureState = ECreatureState.CollectEnv;
                        }

                        //Debug.Log("Just stop and attack.");
                    }
                    else
                    {
                        // ***** 여기는 필요 없음 !! *****
                        // 어떠한 사유로 타겟과 멀어졌다면 _arriveToTarget flag를 false로 돌릴 준비.
                        // --> PointerUp이 되면 무조건 false로 변경. 움직인 것이므로 의미상 _arriveToTarget이 아니게 됨.
                    }
                }
                else // Invalid Leader's Target
                {
                    _arriveToTarget = false;
                    if (_leader.CollectEnv)
                    {
                        _leader.CollectEnv = false;
                        _leader.CreatureMoveState = ECreatureMoveState.None;
                        _leader.CreatureState = ECreatureState.Idle;
                    }
                }
            }

        }

        private Queue<Vector3Int> _leaderPath = new Queue<Vector3Int>();
        private void AddLeaderPath(Vector3Int startCellPos, Vector3Int targetCellPos)
        {
            List<Vector3Int> path = Managers.Map.FindPath(startCellPos: startCellPos,
                                                        destCellPos: targetCellPos,
                                                        maxDepth: 2, // Max to 2 (이게 더 좋은듯)
                                                        ignoreObjectType: EObjectType.Hero);
            _leaderPath.Clear();
            for (int i = 0; i < path.Count; ++i)
                _leaderPath.Enqueue(path[i]);
            _leaderPath.Dequeue();
        }

        private void Move()
        {
            if (_leader.IsValid() == false)
            {
                _nMovementDir = Vector2.zero;
            }
            else
            {
                if (_nMovementDir == Vector3.zero)
                {
                    if (_leader.CreatureState == ECreatureState.Move) // --- DEFENSE
                        _leader.CreatureState = ECreatureState.Idle;
                    return;
                }

                if (_nMovementDir.x < 0f)
                    Leader.LookAtDir = ELookAtDirection.Left;
                else
                    Leader.LookAtDir = ELookAtDirection.Right;

                // 일반적인 이동은 무조건 부드럽게
                _leader.transform.position = JoystickPos;
            }
        }

        private void SetLeader(Hero newLeader)
        {
            if (_leader == newLeader)
            {
                Debug.Log($"Maybe Same Leader, Current: {_leader.name}, new: {newLeader.name}");
                return;
            }
            else if (_leader.IsValid() == false)
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

            newLeader.CreatureMoveState = ECreatureMoveState.None;
            newLeader.CreatureState = ECreatureState.Idle;
            Managers.Object.CameraController.Target = newLeader;
            EnableLeaderMark(true);
        }

        public void EnableLeaderMark(bool enable)
            => _leaderMark.gameObject.SetActive(enable);

        public void EnablePointer(bool enable)
            => _pointer.gameObject.SetActive(enable);

        private void LateTickLeaderPos()
        {
            _leader.UpdateCellPos();
            transform.position = _leader.CenterPosition;
        }

        #region Leader State
        private bool IsSkillCoolTimeReady(ESkillType skillType)
        {
            if (_leader.IsValid() == false) // --- DEFENSE
                return false;

            if (_leader.Target.IsValid() == false)
                return false;

            return _leader.CreatureSkill.IsRemainingCoolTime(skillType) == false;
        }
        #endregion

        #region Event
        private void OnMoveDirChanged(Vector2 nDir)
        {
            if (_leader.IsValid() == false) // --- DEFENSE
                return;

            if (_leader.CreatureState == ECreatureState.Dead)
            {
                _nMovementDir = Vector3.zero;
                return;
            }

            _nMovementDir = nDir;
            if (nDir != Vector2.zero)
            {
                float degree = Mathf.Atan2(-nDir.x, nDir.y) * (180f / Mathf.PI);
                _pointerPivot.rotation = Quaternion.Euler(0, 0, degree);
            }
        }

        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            if (_leader.IsValid() == false) // --- DEFENSE
                return;

            if (_leader.CreatureState == ECreatureState.Dead)
                return;

            switch (joystickState)
            {
                case EJoystickState.Drag:
                    {
                        // 이미 진행중인 PathFinding은 멈추지 않는다.
                        if (_coLeaderPathFinding != null && _lockFindPath == false)
                        {
                            StopCoroutine(_coLeaderPathFinding);
                            _coLeaderPathFinding = null;
                        }

                        // 무기 되돌려놓기
                        if (_leader.CollectEnv)
                            _leader.CollectEnv = false;

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
                        _arriveToTarget = false; // 이동했다가 마우스를 놓으면 어쨋든 false로 돌려 놓는다.
                        if (_coLeaderPathFinding != null && _lockFindPath == false) // 이미 진행중인 PathFinding은 멈추지 않는다.
                        {
                            StopCoroutine(_coLeaderPathFinding);
                            _coLeaderPathFinding = null;
                        }

                        if (_coLeaderPathFinding == null)
                            _leader.CreatureState = ECreatureState.Idle;

                        _leader.CreatureMoveState = ECreatureMoveState.None;
                    }
                    break;
            }
        }
        #endregion

        #region Coroutines
        private Coroutine _coLeaderPathFinding = null;
        private IEnumerator CoLeaderPathFinding()
        {
            _leader.CreatureState = ECreatureState.Move;
            while (_leaderPath.Count != 0)
            {
                Vector3Int nextPos = _leaderPath.Peek();
                Vector3 destPos = Managers.Map.CenteredCellToWorld(nextPos);
                Vector3 dir = destPos - _leader.transform.position;
                if (_leader.Target.IsValid() == false)
                {
                    if (dir.x < 0f)
                        _leader.LookAtDir = ELookAtDirection.Left;
                    else if (dir.x > 0f)
                        _leader.LookAtDir = ELookAtDirection.Right;
                }
                else
                    _leader.LookAtValidTarget();

                if (dir.sqrMagnitude < 0.001f)
                {
                    _leader.transform.position = destPos;
                    _leaderPath.Dequeue();
                    // Path Finding 도중에 중간에 움직일 수 있는 곳이 발견되었을 경우 중지
                    if (Managers.Map.CanMove(JoystickPos, ignoreObjects: true))
                    {
                        _lockFindPath = false;
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

            _lockFindPath = false;
            if (_leader.CreatureMoveState == ECreatureMoveState.None)
                _leader.CreatureState = ECreatureState.Idle;
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

        public Coroutine _coCanChangeLeader { get; private set; } = null;
        // 나중에 UI CoolTime을 표기하기 위한 Percentage
        public float CanChangeLeaderCurrentPercentage { get; private set; } = 0f;
        private IEnumerator CoCanChangeLeader()
        {
            float delta = 0f;
            while (true)
            {
                delta += Time.deltaTime;
                CanChangeLeaderCurrentPercentage = delta / ReadOnly.Numeric.DesiredCanChangeLeaderTime;
                if (CanChangeLeaderCurrentPercentage >= 1f)
                    break;

                yield return null;
            }

            StopCoCanChangeLeader();
        }

        public void StartCoCanChangeLeader()
        {
            if (_coCanChangeLeader == null)
                _coCanChangeLeader = StartCoroutine(CoCanChangeLeader());
        }

        public void StopCoCanChangeLeader()
        {
            if (_coCanChangeLeader != null)
            {
                StopCoroutine(_coCanChangeLeader);
                _coCanChangeLeader = null;
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
            while (newHeroLeader == currentLeader || newHeroLeader.CreatureState == ECreatureState.Dead)
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

        private Coroutine _coAllMembersStopped = null;
        private IEnumerator CoAllMembersStopped(System.Action endCallback = null)
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

            yield return null; // 한프레임 쉬고
            endCallback?.Invoke();
            StopCoAllMembersStopped();
        }

        public void StartCoAllMembersStopped(System.Action endCallback = null)
        {
            if (_coAllMembersStopped == null)
                _coAllMembersStopped = StartCoroutine(CoAllMembersStopped(endCallback));
        }

        public void StopCoAllMembersStopped()
        {
            if (_coAllMembersStopped != null)
            {
                StopCoroutine(_coAllMembersStopped);
                _coAllMembersStopped = null;
            }
        }
        #endregion

        #region Misc
        #endregion

        #region Debug
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_leader == null)
                return;

            if (_heroLeaderChaseMode == EHeroLeaderChaseMode.JustFollowClosely)
                return;

            if (_heroLeaderChaseMode == EHeroLeaderChaseMode.RandomFormation)
                return;

            if (_heroLeaderChaseMode == EHeroLeaderChaseMode.ForceStop)
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
#endif
        #endregion

        #region OBSOLETE TEMPORARY
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
        #endregion
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