using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Hero : Creature
    {
        public HeroData HeroData { get; private set; } = null;
        public HeroAnimation HeroAnim { get; private set; } = null;
        private HeroBody _heroBody = null;
        public HeroBody HeroBody
        {
            get => _heroBody;
            private set
            {
                _heroBody = value;
                if (CreatureBody == null)
                    CreatureBody = value;
            }
        }
        public Transform WeaponLSocket { get; private set; } = null;
        public Transform WeaponRFireSocket { get; private set; } = null;
        public override bool CollectEnv
        {
            get => base.CollectEnv;
            protected set
            {
                base.CollectEnv = value;
                if (value && Target.IsValid())
                {
                    Env envTarget = Target as Env;
                    HeroBody.EnvWeapon(envTarget.EnvType);
                }
                else
                    HeroBody.DefaultWeapon();
            }
        }
        [SerializeField] private bool _isLeader = false;
        public bool IsLeader
        {
            get => _isLeader;
            set
            {
                if (_isLeader == value)
                    return;

                _isLeader = value;
                if (value)
                    StopCoLerpToCellPos();
                else
                    StartCoLerpToCellPos();
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Hero;
            Collider.isTrigger = true;
            RigidBody.simulated = false;
            return true;
        }

        private float _waitMovementDistanceSQRFromLeader = 0f;
        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame(dataID);
                return false;
            }

            // SortingGroup.sortingOrder = 20; // TEST - 일단 Wall이랑 똑같이.
            SortingGroup.sortingOrder = 100; // TEST - 일단 Wall이랑 똑같이.

            HeroBody = new HeroBody(this, dataID);
            HeroAnim = CreatureAnim as HeroAnimation;
            HeroAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, target: this);

            // SET WEAPONS SOCKERT INFO
            WeaponLSocket = HeroBody.GetComponent<Transform>(EHeroWeapon.WeaponLSocket);

            HeroData = Managers.Data.HeroDataDict[dataID];
            gameObject.name += $"_{HeroData.DescriptionTextID.Replace(" ", "")}";
            Collider.radius = HeroData.ColliderRadius;

            CreatureSkill = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkill.SetInfo(owner: this, skillDataIDs: Managers.Data.HeroDataDict[dataID].SkillIDs);
            _waitMovementDistanceSQRFromLeader = ReadOnly.Numeric.WaitMovementDistanceSQRFromLeader;

            EnterInGame(dataID);
            return true;
        }

        protected override void EnterInGame(int dataID)
        {
            LookAtDir = ELookAtDirection.Right;
            CreatureState = ECreatureState.Move;
            base.EnterInGame(dataID);
            StartCoIsFarFromLeaderTick();

            /*
                --- NOTE
                - First Targets: Monsters, Second Targets: Envs
            */
            StartCoSearchTarget<BaseObject>(scanRange: ReadOnly.Numeric.HeroDefaultScanRange,
                            firstTargets: Managers.Object.Monsters,
                            secondTargets: Managers.Object.Envs,
                            func: IsValid);

            // *** Events ***
            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;
        }

        public override Vector3Int ChaseCellPos
        {
            get
            {
                if (CreatureMoveState != ECreatureMoveState.ForceMove && Target.IsValid())
                {
                    if (CanAttackOrChase())
                    {
                        if (Target.ObjectType == EObjectType.Env)
                            CreatureState = ECreatureState.CollectEnv;
                        else
                        {
                            if (_canHandleSkill)
                                CreatureSkill?.CurrentSkill.DoSkill();
                        }
                    }

                    return Target.CellPos;
                }

                HeroLeaderController heroLeaderController = Managers.Object.HeroLeaderController;
                switch (heroLeaderController.HeroLeaderChaseMode)
                {
                    case EHeroLeaderChaseMode.JustFollowClosely:
                    {
                        return Managers.Map.WorldToCell(heroLeaderController.Leader.transform.position);
                    }

                    case EHeroLeaderChaseMode.NarrowFormation:
                    case EHeroLeaderChaseMode.WideFormation:
                    {
                        return Managers.Object.HeroLeaderController.RequestChaseCellPos(this);
                    }

                    case EHeroLeaderChaseMode.RandomFormation:
                    {
                        return Managers.Object.HeroLeaderController.RequestRandomChaseCellPos(this);
                    }

                    case EHeroLeaderChaseMode.ForceStop:
                    {
                        return PrevCellPosForForceStop;
                    }

                    default:
                        return Vector3Int.zero;
                }
            }
        }

        #region AI
        protected override void UpdateIdle()
        {
            if (IsLeader)
                return;

            LookAtValidTarget();
            if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
                return;

            if (IsForceStopMode() == false)
            {
                if (_isFarFromLeader)
                {
                    _pauseSearchTarget = true;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                if (CreatureMoveState == ECreatureMoveState.ForceMove)
                {
                    Hero leader = Managers.Object.HeroLeaderController.Leader;
                    // --- 리더가 이동한다고해서 바로 움직이지 않는다.
                    if ((transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
                        return;

                    List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
                    if (idlePathFind.Count > 1)
                    {
                        if (Managers.Map.CanMove(idlePathFind[1]))
                        {
                            CreatureState = ECreatureState.Move;
                            return;
                        }
                    }
                    return;
                }
            }

            // --- 타겟이 존재하거나, 타겟이 죽었을 경우, Move 상태로 전환
            if (CreatureMoveState == ECreatureMoveState.TargetToEnemy)
                CreatureState = ECreatureState.Move;
        }

        private bool _canHandleSkill = false;
        protected override void UpdateMove()
        {
            if (IsLeader)
                return;

            EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos,
                maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

            if (result == EFindPathResult.Fail_ForceMovePingPongObject) // 확인됨
                return;

            if (CreatureMoveState == ECreatureMoveState.None)
            {
                if (result == EFindPathResult.Fail_NoPath)
                {
                    _pauseSearchTarget = false;
                    CreatureState = ECreatureState.Idle;
                    return;
                }
            }
            else if (CreatureMoveState == ECreatureMoveState.TargetToEnemy)
            {
                _canHandleSkill = false;
                List<Vector3Int> path = Managers.Map.FindPath(CellPos, ChaseCellPos, 2);
                if (path.Count > 0)
                {
                    /*
                        --- NOTE
                        - DO NOT SKI MOVEMENT
                    */
                    Vector3 centeredPos = Managers.Map.CenteredCellToWorld(path[path.Count - 1]);
                    if ((transform.position - centeredPos).sqrMagnitude < 0.01f)
                    {
                        // 타겟을 잡고(타겟이 히어로에게 죽고) 돌아오는 길이었다면..
                        if (Target.IsValid() == false && CreatureMoveState == ECreatureMoveState.TargetToEnemy)
                        {
                            CreatureMoveState = ECreatureMoveState.None;
                            CreatureState = ECreatureState.Idle;
                            return;
                        }

                        if (IsPingPongAndCantMoveToDest(CellPos))
                        {
                            if (_currentPingPongCantMoveCount >= ReadOnly.Numeric.MaxCanPingPongConditionCount && IsForceMovingPingPongObject == false)
                            {
                                CoStartForceMovePingPongObject(CellPos, ChaseCellPos, delegate ()
                                {
                                    _currentPingPongCantMoveCount = 0;
                                    CoStopForceMovePingPongObject();
                                    CreatureMoveState = ECreatureMoveState.None;
                                    CreatureState = ECreatureState.Idle;
                                });
                            }
                            else if (IsForceMovingPingPongObject == false)
                            {
                                Debug.Log($"<color=white>{gameObject.name}, PingPong Count: {++_currentPingPongCantMoveCount}</color>");
                            }
                        }

                        _canHandleSkill = true;
                    }
                }
            }
        }

        protected override void UpdateSkill()
        {
            if (IsLeader)
                return;

            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                // --- DEFENSE
                CreatureMoveState = ECreatureMoveState.None;
            }
        }

        protected override void UpdateCollectEnv()
        {
            if (CreatureMoveState == ECreatureMoveState.ForceMove || Target.IsValid() == false)
            {
                CollectEnv = false;
                CreatureState = ECreatureState.Move;
            }
            else if (Target.IsValid())
            {
                // Research Enemies
                Creature creature = FindClosestInRange(ReadOnly.Numeric.HeroDefaultScanRange, Managers.Object.Monsters, func: IsValid) as Creature;
                if (creature != null)
                {
                    CollectEnv = false;
                    Target = creature;
                    CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                //LookAtTarget(Target);
            }
        }

        protected override void UpdateDead() // --> 없어도 될듯? 보스몹같은거 아니면. ㄴㄴ 보스몹에서도 없어도 될듯
        {
            base.UpdateDead();
        }
        #endregion

        protected override void OnCollectEnvStateUpdate()
        {
            if (Target.IsValid() == false)
                return;

            Target.OnDamaged(this, null);
        }

        #region MISC
        public Vector3Int PrevCellPosForForceStop { get; set; } = Vector3Int.zero;
        private bool IsForceStopMode()
        {
            HeroLeaderController leaderController = Managers.Object.HeroLeaderController;

            if (leaderController == null)
                return false;

            return leaderController.HeroLeaderChaseMode == EHeroLeaderChaseMode.ForceStop;
        }
        #endregion

        #region Event
        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            if (IsLeader)
                return;

            if (CreatureState == ECreatureState.Dead)
                return;

            switch (joystickState)
            {
                case EJoystickState.PointerUp:
                    {
                        if (CreatureMoveState != ECreatureMoveState.TargetToEnemy)
                            CreatureMoveState = ECreatureMoveState.None;
                    }
                    break;

                case EJoystickState.Drag:
                    if (Managers.Object.HeroLeaderController.HeroMemberBattleMode == EHeroMemberBattleMode.EngageEnemy)
                    {
                        if (Target.IsValid() && _isFarFromLeader == false)
                        {
                            CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                            return;
                        }
                    }

                    if (IsForceStopMode() == false)
                    {
                        CreatureMoveState = ECreatureMoveState.ForceMove;
                        Target = null;
                    }
                    break;

                default:
                    break;
            }
        }

        protected override void OnDeadFadeOutEnded()
        {
            Debug.Log("<color=magenta>Hero Func !!!!!</color>");
            if (IsLeader)
                Managers.Game.ChangeHeroLeader(isFromDead: true);

            Debug.Log("Hero::OnDeadFadeOutEnded");
            base.OnDeadFadeOutEnded();
        }
        #endregion

        #region Battle
        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDamaged(attacker, skillFromAttacker);
            Debug.Log($"{gameObject.name} is damaged. ({Hp} / {MaxHp})");
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            if (IsLeader)
            {
                Managers.Object.HeroLeaderController.EnableLeaderMark(false);
                Managers.Object.HeroLeaderController.EnablePointer(false);
            }

            StopCoisFarFromLeaderTick();
            base.OnDead(attacker, skillFromAttacker);
        }
        protected override void OnDisable()
        {
            Debug.Log("Hero::OnDisable");
            base.OnDisable();

            if (Managers.Game == null)
                return;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
        }
        #endregion

        #region Coroutines
        protected override IEnumerator CoDeadFadeOut(System.Action callback = null)
        {
            if (this.isActiveAndEnabled == false)
                yield break;

            yield return new WaitForSeconds(ReadOnly.Numeric.StartDeadFadeOutTime);

            float delta = 0f;
            float percent = 1f;
            AnimationCurve curve = Managers.Animation.Curve(EAnimationCurveType.Ease_In);

            // 1. Fade Out - Skin
            while (percent > 0f)
            {
                delta += Time.deltaTime;
                percent = 1f - (delta / ReadOnly.Numeric.DesiredDeadFadeOutEndTime);
                for (int i = 0; i < HeroBody.Skin.Count; ++i)
                {
                    float current = Mathf.Lerp(0f, 1f, curve.Evaluate(percent));
                    HeroBody.Skin[i].color = new Color(HeroBody.Skin[i].color.r,
                                                       HeroBody.Skin[i].color.g,
                                                       HeroBody.Skin[i].color.b, current);
                }

                yield return null;
            }

            // 2. Fade Out - Appearance
            delta = 0f;
            percent = 1f;
            while (percent > 0f)
            {
                delta += Time.deltaTime;
                percent = 1f - (delta / ReadOnly.Numeric.DesiredDeadFadeOutEndTime);
                for (int i = 0; i < HeroBody.Appearance.Count; ++i)
                {
                    float current = Mathf.Lerp(0f, 1f, curve.Evaluate(percent));
                    HeroBody.Appearance[i].color = new Color(HeroBody.Appearance[i].color.r,
                                                             HeroBody.Appearance[i].color.g,
                                                             HeroBody.Appearance[i].color.b, current);
                }

                yield return null;
            }

            callback?.Invoke();
        }

        [SerializeField] private bool _isFarFromLeader = false;
        private Coroutine _coIsFarFromLeaderTick = null;
        private IEnumerator CoIsFarFromLeaderTick()
        {
            // Scan Range 보다 50%이상 멀어졌을 때
            float farFromLeaderDistSQR = ReadOnly.Numeric.HeroDefaultScanRange * ReadOnly.Numeric.HeroDefaultScanRange + (ReadOnly.Numeric.HeroDefaultScanRange * ReadOnly.Numeric.HeroDefaultScanRange * 0.5f);
            float canWarpDistSQR = ReadOnly.Numeric.CheckFarFromHeroesLeaderDistanceForWarp * ReadOnly.Numeric.CheckFarFromHeroesLeaderDistanceForWarp;
            while (true)
            {
                Hero leader = Managers.Object.HeroLeaderController.Leader;
                if (leader.IsValid() == false) // --- DEFENSE
                {
                    yield return null;
                    continue;
                }

                if ((leader.CellPos - CellPos).sqrMagnitude > farFromLeaderDistSQR)
                    _isFarFromLeader = true;
                else
                    _isFarFromLeader = false;

                // FORCE WARP
                // 워프를 없앨까. 이쁘지가 않은데. 아니면 코루틴을 따로 둬야할지.
                // 15칸(225) 이상일 때, 어차피 로그 함수 이동속도로 금방 따라오긴하지만 알수 없는 이유로 히어로가 막혀있을 때
                if ((leader.CellPos - CellPos).sqrMagnitude > canWarpDistSQR && _coWaitForceStopWarp == null && IsForceStopMode() == false)
                {
                    Vector3 leaderWorldPos = Managers.Object.HeroLeaderController.Leader.transform.position;
                    Managers.Map.WarpTo(this, Managers.Map.WorldToCell(leaderWorldPos), warpEndCallback: null);
                }

                yield return new WaitForSeconds(ReadOnly.Numeric.CheckFarFromHeroesLeaderTick);
            }
        }

        private void StartCoIsFarFromLeaderTick()
        {
            if (_coIsFarFromLeaderTick == null)
                _coIsFarFromLeaderTick = StartCoroutine(CoIsFarFromLeaderTick());
        }

        private void StopCoisFarFromLeaderTick()
        {
            if (_coIsFarFromLeaderTick != null)
            {
                StopCoroutine(_coIsFarFromLeaderTick);
                _coIsFarFromLeaderTick = null;
            }
        }

        private Coroutine _coWaitForceStopWarp = null;
        private IEnumerator CoWaitForceStopWarp(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            StopCoWaitForceStopWarp();
        }

        public void StartCoWaitForceStopWarp(float seconds)
        {
            StopCoWaitForceStopWarp(); // --- 즉시 곧바로 다시 Force Stop 모드로 돌아올 수도 있으므로
            _coWaitForceStopWarp = StartCoroutine(CoWaitForceStopWarp(seconds));
        }

        public void StopCoWaitForceStopWarp()
        {
            if (_coWaitForceStopWarp != null)
            {
                StopCoroutine(_coWaitForceStopWarp);
                _coWaitForceStopWarp = null;
            }
        }
        #endregion
    }
}

/*
    [PREV CODE]
    // if (Target.IsValid())
            // {

            //     // if (Target.ObjectType == EObjectType.Monster)
            //     // {
            //     //     CreatureMoveState = ECreatureMoveState.TargetToEnemy;
            //     //     return;
            //     // }

            //     // if (Target.ObjectType == EObjectType.Env)
            //     // {
            //     //     CreatureMoveState = ECreatureMoveState.CollectEnv; // 필요없을지도?
            //     //     CreatureState = ECreatureState.Move;
            //     //     return;
            //     // }
            // }      

            // if (Target.IsValid())
            // {
            //     LookAtTarget();

            //     // ... CHECK SKILL COOL TIME ...
            //     if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
            //         return;

            //     // ... 무조건 몬스터부터 ...
            //     if (Target.ObjectType == EObjectType.Monster)
            //     {
            //         CreatureMoveState = ECreatureMoveState.TargetToEnemy;
            //         CreatureState = ECreatureState.Move;
            //         return;
            //     }

            //     // ... ENV TARGET ...
            //     if (Target.ObjectType == EObjectType.Env)
            //     {
            //         CreatureMoveState = ECreatureMoveState.CollectEnv;
            //         CreatureState = ECreatureState.Move;
            //         return;
            //     }
            // }

            // if (Target.IsValid())
            //     LookAtTarget(Target);

            // // 조금 극단적인 방법. 쳐다보면서 가만히 짱박혀있어라.
            // if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
            //     return;

            // Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Monsters, func: IsValid) as Creature;
            // if (creature.IsValid())
            // {
            //     Target = creature;
            //     CreatureState = ECreatureState.Move;
            //     CreatureMoveState = ECreatureMoveState.TargetToEnemy;
            //     return;
            // }

            // Env env = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Envs, func: IsValid) as Env;
            // if (env.IsValid())
            // {
            //     Target = env;
            //     CreatureState = ECreatureState.Move;
            //     CreatureMoveState = ECreatureMoveState.CollectEnv;
            //     return;
            // }

            // if (NeedArrange)
            // {
            //     CreatureState = ECreatureState.Move;
            //     CreatureMoveState = ECreatureMoveState.ReturnToBase;
            //     return;
            // }

        private void MoveByForcePath()
        {
            // 이미 다 소모해서 이동이 끝났으면 종료
            if (_forcePath.Count == 0)
            {
                CreatureMoveState = ECreatureMoveState.None;
                return;
            }

            // 그게 아니면 _forcePath를 하나씩 찾음
            // 2로 넣은 이유, 나, 다음길, 이렇게 2개
            Vector3Int cellPos = _forcePath.Peek();
            if (MoveToCellPos(destCellPos: cellPos, maxDepth: 2))
            {
                _forcePath.Dequeue();
                return;
            }

            Hero hero = Managers.Map.GetObject(cellPos) as Hero;
            if (hero != null && hero.CreatureState == ECreatureState.Idle)
            {
                CreatureMoveState = ECreatureMoveState.None;
                return;
            }
        }

        private Queue<Vector3Int> _forcePath = new Queue<Vector3Int>();
        // ####################### TEMP #######################
        private List<Vector3Int> _pathList = new List<Vector3Int>();
        //  ###################################################
        private bool CheckHeroCampDistanceAndForcePath()
        {
            // Vector3 destPos = CampDestination.position;
            // Vector3Int destCellPos = Managers.Map.WorldToCell(destPos);
            // if ((CellPos - destCellPos).magnitude <= 10f) // 10칸 이상으로 너무 멀어졌을 경우,, (ㄹㅇ 거리로 판정)
            //     return false;

            // if (Managers.Map.CanMove(destCellPos, ignoreObjects: true) == false)
            //     return false;

            // List<Vector3Int> path = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: destCellPos, maxDepth: ReadOnly.Numeric.HeroMaxMoveDepth);
            // if (path.Count < 2)
            //     return false;

            // CreatureMoveState = ECreatureMoveState.ForcePath;

            // _forcePath.Clear();
            // foreach (var p in path)
            // {
            //     _forcePath.Enqueue(p);
            // }
            // _forcePath.Dequeue(); // 시작 위치는 제거한듯?

            return true;
        }

        // Prev Origin
        // protected override void UpdateMove()
        // {
        //     // A* Test
        //     if (CreatureMoveState == ECreatureMoveState.Replace)
        //     {
        //         // 되긴 하는데 장애물 근처에 있을 때 Fail_NoPath 떠가지고 제자리 걸음 하는 녀석도 있긴함.
        //         // 이거 고치고 코드 우아하게 AI 수정. Idle 너무 강제임. 고쳐야함.
        //         // 지금도 대강 되긴하는데 이거 고치고, 체인지 포지션?
        //         // 그리고 HeroLeader가 Camp의 Dest가 되어야함. --> 이것부터 할까?
        //         FindPathAndMoveToCellPos(destPos: _replaceDestPos, ReadOnly.Numeric.HeroMaxMoveDepth);
        //         if (LerpToCellPosCompleted) // A* Test
        //         {
        //             // A* Test
        //             CreatureMoveState = ECreatureMoveState.None; // 이렇게만 처리하고 싶은데 확실하게 Idle로 안가는 녀석도 있음.
        //             CreatureState = ECreatureState.Idle; // 그래서 이것도 추가 (임시)
        //             NeedArrange = false;
        //             return;
        //         }

        //         return;
        //     }

        //     // ***** 일단 생략, 헷갈림 *****
        //     // ForceMove 보다 더 우선순위
        //     if (CreatureMoveState == ECreatureMoveState.ForcePath)
        //     {
        //         // 너무 멀리있으면 무조건 강제이동
        //         MoveByForcePath();
        //         return;
        //     }

        //     if (CheckHeroCampDistanceAndForcePath())
        //         return;

        //     if (CreatureMoveState == ECreatureMoveState.ForceMove)
        //     {
        //         // ### SET DIR ###
        //         _findPathResult = FindPathAndMoveToCellPos(CampDestination.position, ReadOnly.Numeric.HeroDefaultMoveDepth);
        //         return;
        //     }

        //     if (CreatureMoveState == ECreatureMoveState.TargetToEnemy)
        //     {
        //         if (Target.IsValid() == false)
        //         {
        //             // 여기서 None
        //             CreatureMoveState = ECreatureMoveState.None;
        //             CreatureState = ECreatureState.Move;
        //             return;
        //         }

        //         ChaseOrAttackTarget(ReadOnly.Numeric.Temp_ScanRange, AttackDistance);
        //         return;
        //     }

        //     if (CreatureMoveState == ECreatureMoveState.CollectEnv)
        //     {
        //         // Research Enemies
        //         Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Monsters, func: IsValid) as Creature;
        //         if (creature != null)
        //         {
        //             Target = creature;
        //             CreatureMoveState = ECreatureMoveState.TargetToEnemy;
        //             CreatureState = ECreatureState.Move;
        //             return;
        //         }

        //         // 이미 채집했으면 포기
        //         if (Target.IsValid() == false)
        //         {
        //             CreatureMoveState = ECreatureMoveState.None;
        //             CreatureState = ECreatureState.Move;
        //             CollectEnv = false;
        //             return;
        //         }

        //         ChaseOrAttackTarget(ReadOnly.Numeric.Temp_ScanRange, AttackDistance);
        //         return;
        //     }

        //     if (CreatureMoveState == ECreatureMoveState.ReturnToBase && IsLeader == false) // ***** TEST *****
        //     {
        //         _findPathResult = FindPathAndMoveToCellPos(CampDestination.position, ReadOnly.Numeric.HeroDefaultMoveDepth);

        //         // 실패사유 검사
        //         // 여기서 뭉치는 알고리즘이 진행됨
        //         BaseObject obj = Managers.Map.GetObject(CampDestination.position);
        //         if (obj.IsValid())
        //         {
        //             // 내가 그 자리를 차지하고 있음.
        //             if (obj == this)
        //             {
        //                 Debug.Log($"<color=magenta>findPathResult: {_findPathResult}</color>");
        //                 CreatureMoveState = ECreatureMoveState.None;
        //                 NeedArrange = false;
        //                 return;
        //             }

        //             // 다른 영웅이 그 자리를 차지하고 있음.
        //             Hero hero = obj as Hero;
        //             if (hero != null && hero.CreatureState == ECreatureState.Idle)
        //             {
        //                 Debug.Log($"<color=orange>findPathResult: {_findPathResult}</color>");
        //                 CreatureMoveState = ECreatureMoveState.None;
        //                 NeedArrange = false;
        //                 return;
        //             }
        //         }
        //     }

        //     if (LerpToCellPosCompleted)
        //     {
        //         CreatureState = ECreatureState.Idle;
        //         // 여기서는 누르지 않은 상태이니까 None
        //         if (CreatureMoveState == ECreatureMoveState.ReturnToBase && _findPathResult != EFindPathResult.Success)
        //         {
        //             // 이거 조건걸어서 단체로 움직일때 누구는 캠프 근처로가고 누구는 안가고 이러는건가.
        //             NeedArrange = false; // 이것만 해도 임시적으로 되긴함
        //             CreatureMoveState = ECreatureMoveState.None;
        //             // Lerp하다가 중간에 누가 차지하면 계속 걸어가려고하는데 이거 막아야할듯
        //         }
        //         // NeedArrange = false; // --> 이거 주면 해결 되긴하는데, 캠프까지 쫓아가질 않음
        //     }
        // }
*/