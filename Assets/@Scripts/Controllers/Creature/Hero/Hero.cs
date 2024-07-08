using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.UIElements;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Hero : Creature
    {
        #region Background
        public HeroData HeroData { get; private set; } = null;
        public HeroAnimation HeroAnim { get; private set; } = null;
        public HeroAI HeroAI { get; private set; } = null;
        [SerializeField] private HeroBody _heroBody = null;
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
        public Transform WeaponLFireSocket => HeroBody.GetContainer(EHeroBody_Weapon.WeaponL_FireSocket).TR;
        public Transform WeaponRFireSocket => HeroBody.GetContainer(EHeroBody_Weapon.WeaponR_FireSocket).TR;

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

        [SerializeField] private EHeroStateWeaponType _heroStateWeaponType = EHeroStateWeaponType.Default;
        public EHeroStateWeaponType HeroStateWeaponType
        {
            get => _heroStateWeaponType;
            set
            {
                if (_heroStateWeaponType != value)
                {
                    _heroStateWeaponType = value;
                    // --- 다시 잡아야함
                    // switch (value)
                    // {
                    //     case EHeroStateWeaponType.Default:
                    //         HeroBody.HeroStateWeaponType(EHeroStateWeaponType.Default);
                    //         break;

                    //     case EHeroStateWeaponType.EnvTree:
                    //         HeroBody.HeroStateWeaponType(EHeroStateWeaponType.EnvTree);
                    //         break;

                    //     case EHeroStateWeaponType.EnvRock:
                    //         HeroBody.HeroStateWeaponType(EHeroStateWeaponType.EnvRock);
                    //         break;
                    // }
                }
            }
        }

        public override void LerpToCellPos(float movementSpeed) // Coroutine every tick
        {
            if (IsLeader)
                return;

            Hero leader = Managers.Object.HeroLeaderController.Leader;
            if (LerpToCellPosCompleted && leader.Moving == false)
            {
                Moving = false;
                return;
            }

            Vector3 destPos = Managers.Map.CenteredCellToWorld(CellPos); // 이동은 가운데로.
            Vector3 dir = destPos - transform.position;

            if (dir.x < 0f)
                LookAtDir = ELookAtDirection.Left;
            else if (dir.x > 0f)
                LookAtDir = ELookAtDirection.Right;

            if (dir.sqrMagnitude < 0.001f)
            {
                transform.position = destPos;
                LerpToCellPosCompleted = true;
                return;
            }

            Moving = true;
            float moveDist = Mathf.Min(dir.magnitude, movementSpeed * Time.deltaTime);
            transform.position += dir.normalized * moveDist;
        }

        public override bool Moving 
        { 
            get => base.Moving; 
            set
            {
                base.Moving = value;
                if (value && HeroStateWeaponType != EHeroStateWeaponType.Default)
                    HeroStateWeaponType = EHeroStateWeaponType.Default;
            }
        }

        protected override void OnDeadFadeOutEnded()
        {
            if (IsLeader)
                Managers.Game.ChangeHeroLeader(autoChangeFromDead: true);

            Debug.Log("Hero::OnDeadFadeOutEnded");
            base.OnDeadFadeOutEnded();
        }

        public override bool CanCollectEnv
        {
            get
            {
                if (CreatureAnim.CanEnterAnimState(ECreatureAnimState.Upper_Idle_To_CollectEnv) == false)
                    return false;

                if (Moving)
                {
                    if (HeroStateWeaponType != EHeroStateWeaponType.Default)
                        HeroStateWeaponType = EHeroStateWeaponType.Default;
                    return false;
                }

                if (this.IsValid() == false)
                {
                    if (HeroStateWeaponType != EHeroStateWeaponType.Default)
                        HeroStateWeaponType = EHeroStateWeaponType.Default;

                    return false;
                }

                if (Target.IsValid() == false)
                {
                    if (HeroStateWeaponType != EHeroStateWeaponType.Default)
                        HeroStateWeaponType = EHeroStateWeaponType.Default;

                    return false;
                }

                if (Target.IsValid() && Target.ObjectType != EObjectType.Env)
                {
                    if (HeroStateWeaponType != EHeroStateWeaponType.Default)
                        HeroStateWeaponType = EHeroStateWeaponType.Default;

                    return false;
                }

                if (ForceMove)
                {
                    if (HeroStateWeaponType != EHeroStateWeaponType.Default)
                        HeroStateWeaponType = EHeroStateWeaponType.Default;

                    return false;
                }

                int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
                int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);
                if (dx <= 1 && dy <= 1)
                    return true;

                return false;
            }
        }
        #endregion

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Hero;
            HeroBody = GetComponent<HeroBody>();

            Collider.isTrigger = true;
            RigidBody.simulated = false;
            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame();
                return false;
            }
            
            InitialSetInfo(dataID);
            EnterInGame();
            StartCoroutine(CoInitialReleaseLeaderAI());
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            HeroData = Managers.Data.HeroDataDict[dataID];
            HeroBody.SetInfo(this, dataID);

            HeroAnim = CreatureAnim as HeroAnimation;
            HeroAnim.SetInfo(dataID, this);

            Type aiClassType = Util.GetTypeFromName(HeroData.AIClassName);
            CreatureAI = gameObject.AddComponent(aiClassType) as CreatureAI;          
            CreatureAI.SetInfo(this);  
            HeroAI = CreatureAI as HeroAI;

            CreatureRarity = HeroData.CreatureRarity;
            gameObject.name += $"_{HeroData.NameTextID.Replace(" ", "")}";
            Collider.radius = HeroData.ColliderRadius;

            CreatureSkill = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkill.SetInfo(owner: this, HeroData);
        }

        protected override void InitStat(int dataID)
        {
            base.InitStat(dataID);
            for (int i = DataTemplateID; i < DataTemplateID + ReadOnly.Util.HeroMaxLevel;)
            {
                if (Managers.Data.StatDataDict.ContainsKey(i) == false)
                    break;

                _maxLevel = i++;
            }
        }

        protected override void EnterInGame()
        {
            // --- Default Heroes Dir: Right
            LookAtDir = ELookAtDirection.Right;
            base.EnterInGame();
            CreatureAIState = ECreatureAIState.Move;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;
        }

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDamaged(attacker, skillFromAttacker);
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            if (IsLeader)
            {
                Managers.Object.HeroLeaderController.EnableLeaderMark(false);
                Managers.Object.HeroLeaderController.EnablePointer(false);
            }

            HeroBody.ResetHeroMaterialsAndColors();
            base.OnDead(attacker, skillFromAttacker);
        }

        protected override void OnDisable() // --- TEMP
        {
            base.OnDisable();

            if (Managers.Game == null)
                return;
        }
        #endregion

        #region Event
        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            if (this.IsValid() == false)
                return;

            switch (joystickState)
            {
                case EJoystickState.Drag:
                    ForceMove = true;
                    break;

                case EJoystickState.PointerUp:
                    ForceMove = false;
                    break;
            }
        }
        #endregion

        #region Coroutines
        protected override IEnumerator CoDeadFadeOut(System.Action callback = null)
        {
            if (this.isActiveAndEnabled == false)
                yield break;

            yield return new WaitForSeconds(ReadOnly.Util.StartDeadFadeOutTime);

            float delta = 0f;
            float percent = 1f;
            AnimationCurve curve = Managers.Contents.Curve(EAnimationCurveType.Ease_In);
            // --- 1. Fade Out - Skin
            while (percent > 0f)
            {
                delta += Time.deltaTime;
                percent = 1f - (delta / ReadOnly.Util.DesiredDeadFadeOutEndTime);
                for (int i = 0; i < HeroBody.Skin.Count; ++i)
                {
                    float current = Mathf.Lerp(0f, 1f, curve.Evaluate(percent));
                    HeroBody.Skin[i].color = new Color(HeroBody.Skin[i].color.r,
                                                       HeroBody.Skin[i].color.g,
                                                       HeroBody.Skin[i].color.b, current);
                }

                yield return null;
            }

            // --- 2. Fade Out - Appearance
            delta = 0f;
            percent = 1f;
            while (percent > 0f)
            {
                delta += Time.deltaTime;
                percent = 1f - (delta / ReadOnly.Util.DesiredDeadFadeOutEndTime);
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

        private IEnumerator CoInitialReleaseLeaderAI()
        {
            // 여기서 하면 안됨... Leader랑 관련 있는듯.
            // Debug.Log($"1 - Inactive: {HeroBody.GetContainer(EHeroWeapon.WeaponL_Armor).TR.gameObject.name}");
            // HeroBody.GetContainer(EHeroWeapon.WeaponL_Armor).TR.gameObject.SetActive(false);

            yield return new WaitUntil(() => 
            {
                bool allStartedHeroAI = true;
                for (int i = 0; i < Managers.Object.Heroes.Count; ++i)
                {
                    Hero hero = Managers.Object.Heroes[i];
                    if (hero._coUpdateAI == null)
                        allStartedHeroAI = false;
                }

                if (allStartedHeroAI)
                    return true;

                return false;
            });

            if (IsLeader)
            {
                StopCoUpdateAI();
                Debug.Log("<color=white>Initial Release Leader Hero's AI</color>");

                // 여기서 하니까 됨.. 이거 때문인가??
                // Debug.Log($"1 - Inactive: {HeroBody.GetContainer(EHeroWeapon.WeaponL_Armor).TR.gameObject.name}");
                // HeroBody.GetContainer(EHeroWeapon.WeaponL_Armor).TR.gameObject.SetActive(false);
            }
        }
        #endregion
    }
}

/*
    [PREV REF]
    public override Vector3Int ChaseCellPos
        {
            get
            {
                if (ForceMove == false && Target.IsValid())
                    return Target.CellPos;
                
                HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
                switch (leaderController.HeroMemberFormationMode)
                {
                    case EHeroMemberFormationMode.FollowLeaderClosely:
                        return Managers.Map.WorldToCell(leaderController.Leader.transform.position); ;

                    case EHeroMemberFormationMode.NarrowFormation:
                    case EHeroMemberFormationMode.WideFormation:
                        return leaderController.RequestFormationCellPos(this); ;

                    case EHeroMemberFormationMode.RandomFormation:
                        return leaderController.RequestRandomFormationCellPos(this);

                    case EHeroMemberFormationMode.ForceStop:
                        return HeroAI.PrevCellPosForForceStop;
                }

                return Vector3Int.zero;
            }
        }
     // public override Vector3Int ChaseCellPos
        // {
        //     get
        //     {
        //         if (CreatureMoveState != ECreatureMoveState.ForceMove && Target.IsValid())
        //         {
        //             if (CanAttackOrChase())
        //             {
        //                 if (_canHandleSkill)
        //                 {
        //                     if (Target.ObjectType == EObjectType.Env)
        //                         CreatureAIState = ECreatureAIState.CollectEnv;
        //                     else if (Target.ObjectType == EObjectType.Monster)
        //                     {
        //                         if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack) == false)
        //                             CreatureSkill?.CurrentSkill.DoSkill();
        //                         else
        //                             CreatureAIState = ECreatureAIState.Idle;
        //                     }
        //                 }
        //             }

        //             return Target.CellPos;
        //         }

        //         HeroLeaderController heroLeaderController = Managers.Object.HeroLeaderController;
        //         switch (heroLeaderController.HeroMemberFormationMode)
        //         {
        //             case EHeroMemberFormationMode.FollowLeaderClosely:
        //             {
        //                 return Managers.Map.WorldToCell(heroLeaderController.Leader.transform.position);
        //             }

        //             case EHeroMemberFormationMode.NarrowFormation:
        //             case EHeroMemberFormationMode.WideFormation:
        //             {
        //                 return Managers.Object.HeroLeaderController.RequestFormationCellPos(this);
        //             }

        //             case EHeroMemberFormationMode.RandomFormation:
        //             {
        //                 return Managers.Object.HeroLeaderController.RequestRandomFormationCellPos(this);
        //             }

        //             case EHeroMemberFormationMode.ForceStop:
        //             {
        //                 return PrevCellPosForForceStop;
        //             }

        //             default:
        //                 return Vector3Int.zero;
        //         }
        //     }
        // }

        // private void Update()
        // {
        //     if (IsLeader)
        //         return;

        //     if (MoveAfterLeader())
        //     {
        //         EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos,
        //             maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

        //         if (result == EFindPathResult.Fail_ForceMovePingPongObject) // 확인됨
        //             return;

        //         if (result == EFindPathResult.Fail_NoPath)
        //             IsMoving = false;
        //         else
        //             IsMoving = true;
        //     }
        // }

        // private void LateUpdate()
        // {
        //     if (IsLeader)
        //         return;

        //     // 와리가리 핑퐁 체크
        // }

        protected override void UpdateMove()
        {
            if (IsLeader)
                return;

            if (ForceMove == false && IsInTargetAttackRange())
            {
                CreatureAIState = ECreatureAIState.Idle;
                return;
            }

            EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos,
                maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

            if (result == EFindPathResult.Fail_NoPath)
            {
                Hero leader = Managers.Object.HeroLeaderController.Leader;
                if (leader.IsMoving == false)
                {
                    //Debug.Log("111-111");
                    CreatureAIState = ECreatureAIState.Idle;
                }

                return;
            }
        }
    private bool MoveAfterLeader()
        {
            if (_isFarFromLeader)
            {
                PauseSearchTarget = true;
                return true;
            }

            if (ForceMove)
            {
                // --- 리더가 이동한다고해서 바로 움직이지 않는다.
                Hero leader = Managers.Object.HeroLeaderController.Leader;
                if ((transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
                    return true;

                List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
                if (idlePathFind.Count > 1)
                {
                    if (Managers.Map.CanMove(idlePathFind[1]))
                        return true;
                }
            }

            return false;
        }
     // Prev
        // private void OnJoystickStateChanged(EJoystickState joystickState)
        // {
        //     // 일부러 리더랑 분리해서 보는게 편해서 이렇게 했음
        //     if (IsLeader)
        //         return;

        //     if (CreatureAIState == ECreatureAIState.Dead)
        //         return;

        //     // switch (joystickState)
        //     // {
        //     //     case EJoystickState.PointerUp:
        //     //         {
        //     //             if (CreatureMoveState != ECreatureMoveState.MoveToTarget)
        //     //                 CreatureMoveState = ECreatureMoveState.None;
        //     //         }
        //     //         break;

        //     //     case EJoystickState.Drag:
        //     //         if (Managers.Object.HeroLeaderController.HeroMemberChaseMode == EHeroMemberChaseMode.EngageTarget)
        //     //         {
        //     //             if (Target.IsValid() && _isFarFromLeader == false)
        //     //             {
        //     //                 CreatureMoveState = ECreatureMoveState.MoveToTarget;
        //     //                 return;
        //     //             }
        //     //         }

        //     //         if (IsForceStopMode() == false)
        //     //         {
        //     //             CreatureMoveState = ECreatureMoveState.ForceMove;
        //     //             Target = null;
        //     //         }
        //     //         break;

        //     //     default:
        //     //         break;
        //     // }
        // }

            public void StartSearchTarget(System.Func<bool> allTargetsCondition = null)
        {
            StartCoSearchTarget<BaseObject>(scanRange: ReadOnly.Numeric.HeroDefaultScanRange,
                                        firstTargets: Managers.Object.Monsters,
                                        secondTargets: Managers.Object.Envs,
                                        func: IsValid,
                                        allTargetsCondition: allTargetsCondition);
        }

        public void StopSearchTarget()
            => StopCoSearchTarget();
    #region AI - Prev
        // protected override void UpdateIdle()
        // {
        //     if (IsLeader)
        //         return;

        //     if (MoveAfterLeader2())
        //     {
        //         CreatureAIState = ECreatureAIState.Move;
        //         return;
        //     }

        //     // Prev Code
        //     // --- Follow Leader
        //     // if (CanForceMoveInFollowLeaderMode() && IsForceStopMode() == false)
        //     // {
        //     //     MoveFromLeader();
        //     //     return;
        //     // }

        //     // LookAtValidTarget();
        //     // if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
        //     //     return;

        //     // --- 타겟이 존재하거나, 타겟이 죽었을 경우, Move 상태로 전환
        //     // if (Target.IsValid() || CreatureMoveState == ECreatureMoveState.MoveToTarget && IsForceStopMode() == false)
        //     // {
        //     //     CreatureAIState = ECreatureAIState.Move;
        //     //     CreatureUpperAnimState = ECreatureUpperAnimState.UA_Move;
        //     //     CreatureLowerAnimState = ECreatureLowerAnimState.LA_Move;
        //     //     return;
        //     // }

        //     // if (IsForceStopMode() == false)
        //     // {
        //     //     MoveFromLeader();
        //     //     return;
        //     // }
        // }

        private bool _canHandleSkill = false;

        // --- UpdateMove
        // >> Upper - Move, Skill_Attack, Skill_A, Skill_B
        // >> Lower - Move
        // protected override void UpdateMove()
        // {
        //     if (IsLeader)
        //         return;

        //     EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos,
        //         maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

        //     if (result == EFindPathResult.Fail_ForceMovePingPongObject) // 확인됨
        //         return;

        //     if (CreatureMoveState == ECreatureMoveState.None)
        //     {
        //         if (result == EFindPathResult.Fail_NoPath)
        //         {
        //             PauseSearchTarget = false;
        //             CreatureAIState = ECreatureAIState.Idle;
        //             return;
        //         }
        //     }
        //     // else if (CreatureMoveState == ECreatureMoveState.MoveToTarget)
        //     // {
        //     //     _canHandleSkill = false;
        //     //     List<Vector3Int> path = Managers.Map.FindPath(CellPos, ChaseCellPos, 2);
        //     //     if (path.Count > 0)
        //     //     {
        //     //         for (int i = 0; i < path.Count; ++i)
        //     //         {
        //     //             // 일단 무조건 Cell의 중앙에 와야함. (스키타는 움직임 방지)
        //     //             Vector3 centeredPathPos = Managers.Map.CenteredCellToWorld(path[i]);
        //     //             if ((transform.position - centeredPathPos).sqrMagnitude > 0.01f)
        //     //                 continue;

        //     //             // Cell 중앙에 위치 후, 공격 범위에 들어오면 공격 시작.
        //     //             if (IsInAttackRange())
        //     //             {
        //     //                 _canHandleSkill = true;
        //     //                 return;
        //     //             }
        //     //         }

        //     //         // 겹쳐서 계속 MoveState로 허공질하고 있을 때
        //     //         // --- 여기 고장남
        //     //         // if (CellPos == Managers.Object.HeroLeaderController.Leader.CellPos)
        //     //         // {
        //     //         //     Debug.Log($"<color=magenta>!!! NID FIX !!!{gameObject.name}</color>");
        //     //         //     CreatureMoveState = ECreatureMoveState.None;
        //     //         //     CreatureState = ECreatureState.Idle;
        //     //         //     return;
        //     //         // }

        //     //         // 와리가리 핑퐁 오브젝트 체크
        //     //         Vector3 centeredLastPathPos = Managers.Map.CenteredCellToWorld(path[path.Count - 1]);
        //     //         if (Target.IsValid() && (transform.position - centeredLastPathPos).sqrMagnitude < 0.01f)
        //     //         {
        //     //             if (IsPingPongAndCantMoveToDest(CellPos))
        //     //             {
        //     //                 if (_currentPingPongCantMoveCount >= ReadOnly.Numeric.MaxCanPingPongConditionCount && IsForceMovingPingPongObject == false)
        //     //                 {
        //     //                     Debug.Log($"<color=cyan>[!]{gameObject.name}, Start force moving for PingPong Object.</color>");
        //     //                     CoStartForceMovePingPongObject(CellPos, ChaseCellPos, endCallback: delegate ()
        //     //                     {
        //     //                         _currentPingPongCantMoveCount = 0;
        //     //                         CoStopForceMovePingPongObject();
        //     //                         CreatureMoveState = ECreatureMoveState.None;
        //     //                         CreatureAIState = ECreatureAIState.Idle;
        //     //                         CreatureUpperAnimState = ECreatureUpperAnimState.UA_Idle;
        //     //                         CreatureLowerAnimState = ECreatureLowerAnimState.LA_Idle;
        //     //                     });
        //     //                 }
        //     //                  else if (IsForceMovingPingPongObject == false)
        //     //                     ++_currentPingPongCantMoveCount;

        //     //                 return;
        //     //             }
        //     //         }

        //     //         // 타겟이 죽었다면.. return to leader
        //     //         if (Target.IsValid() == false && CreatureMoveState == ECreatureMoveState.MoveToTarget)
        //     //         {
        //     //             if ((transform.position - centeredLastPathPos).sqrMagnitude < 0.01f)
        //     //             {
        //     //                 CreatureMoveState = ECreatureMoveState.None;
        //     //                 CreatureAIState = ECreatureAIState.Idle;
        //     //                 CreatureUpperAnimState = ECreatureUpperAnimState.UA_Idle;
        //     //                 CreatureLowerAnimState = ECreatureLowerAnimState.LA_Idle;
        //     //                 Debug.Log($"<color=cyan>{gameObject.name}, return to leader</color>");
        //     //                 return;
        //     //             }
        //     //         }
        //     //     }
        //     // }
        // }

        // protected override void UpdateSkill()
        // {
        //     if (IsLeader)
        //         return;

        //     if (CanForceMoveInFollowLeaderMode())
        //         MoveFromLeader();
        // }

        // protected override void UpdateCollectEnv()
        // {
        //     if (IsLeader)
        //         return;

        //     LookAtValidTarget();
        //     // Retarget Monster or Env is dead
        //     if ((Target.IsValid() && Target.ObjectType == EObjectType.Monster) || Target.IsValid() == false)
        //     {
        //         // CollectEnv = false;
        //         CreatureAIState = ECreatureAIState.Move;
        //     }

        //     if (CanForceMoveInFollowLeaderMode())
        //         MoveFromLeader();
        // }

        protected override void UpdateDead()
        {
            base.UpdateDead();
        }
        #endregion
      // private void MoveAfterLeader()
        // {
        //     if (_isFarFromLeader)
        //     {
        //         PauseSearchTarget = true;
        //         return;
        //     }

        //     if (ForceMove)
        //     {
        //         Hero leader = Managers.Object.HeroLeaderController.Leader;
        //         if ((transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
        //             return;

        //         List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
        //         if (idlePathFind.Count > 1)
        //         {
        //             if (Managers.Map.CanMove(idlePathFind[1]))
        //             {
        //                 CreatureAIState = ECreatureAIState.Move;
        //                 return;
        //             }
        //         }
        //         return;
        //     }
        // }

        private void MoveFromLeader()
        {
            if (_isFarFromLeader)
            {
                PauseSearchTarget = true;
                //CreatureAIState = ECreatureAIState.Move;
                return;
            }

            // if (CreatureMoveState == ECreatureMoveState.ForceMove || CreatureMoveState == ECreatureMoveState.None)
            // {
            //     Hero leader = Managers.Object.HeroLeaderController.Leader;
            //     // --- 리더가 이동한다고해서 바로 움직이지 않는다.
            //     if ((transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
            //         return;

            //     List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
            //     if (idlePathFind.Count > 1)
            //     {
            //         if (Managers.Map.CanMove(idlePathFind[1]))
            //         {
            //             //CreatureAIState = ECreatureAIState.Move;
            //             return;
            //         }
            //     }
            //     return;
            // }
        }

    // -- 가끔 간헐적으로 멍때림을 방지하는 코루틴(임시)
        private Coroutine _coCheckSpaceOut = null;
        private IEnumerator CoCheckSpaceOut()
        {
            bool spaceOut = false;
            while (true)
            {
                if (IsLeader)
                {
                    yield return null;
                    continue;
                }

                // 아무것도 안하고 있을 때.. 타겟이 있는데 가만히 멍대리고 있는다..
                // 아까 Idle 상태였는데 
                // if (CreatureAIState == ECreatureAIState.Idle && Target.IsValid())
                //     spaceOut = true;

                // --- 검토 필요. 전체적인 AI 검토 필요 ---
                if (spaceOut)
                {
                    Debug.Log($"<color=yellow>ZoneOut: {gameObject.name}</color>");
                    spaceOut = false;
                    Target = null;
                    CreatureAIState = ECreatureAIState.Idle;
                }

                yield return new WaitForSeconds(3f);
            }
        }

        private void StartCoCheckSpaceOut()
        {
            if (_coCheckSpaceOut != null)
                return;

            _coCheckSpaceOut = StartCoroutine(CoCheckSpaceOut());
        }

        private void StopCoCheckSpaceOut()
        {
            if (_coCheckSpaceOut != null)
            {
                StopCoroutine(_coCheckSpaceOut);
                _coCheckSpaceOut = null;
            }
        }

    // if (_isFarFromLeader)
                // {
                //     _pauseSearchTarget = true;
                //     CreatureState = ECreatureState.Move;
                //     return;
                // }

                // if (CreatureMoveState == ECreatureMoveState.ForceMove)
                // {
                //     Hero leader = Managers.Object.HeroLeaderController.Leader;
                //     // --- 리더가 이동한다고해서 바로 움직이지 않는다.
                //     if ((transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
                //         return;

                //     List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
                //     if (idlePathFind.Count > 1)
                //     {
                //         if (Managers.Map.CanMove(idlePathFind[1]))
                //         {
                //             CreatureState = ECreatureState.Move;
                //             return;
                //         }
                //     }
                //     return;
                // }

                // if (IsCorrectHeroMemberChaseMode(EHeroMemberChaseMode.FollowLeader) && CreatureMoveState == ECreatureMoveState.ForceMove)
            // {
            //     CreatureState = ECreatureState.Move;
            //     return;
            // }

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

        // #################### PREV ####################
        // --- DO NOT SKI MOVEMENT
        // Vector3 centeredPos = Managers.Map.CenteredCellToWorld(path[path.Count - 1]);
        // if ((transform.position - centeredPos).sqrMagnitude < 0.01f)
        // {
        //     // 타겟을 잡고(타겟이 히어로에게 죽고) 돌아오는 길이었다면..
        //     if (Target.IsValid() == false && CreatureMoveState == ECreatureMoveState.TargetToEnemy)
        //     {
        //         CreatureMoveState = ECreatureMoveState.None;
        //         CreatureState = ECreatureState.Idle;
        //         return;
        //     }

        //     if (IsPingPongAndCantMoveToDest(CellPos))
        //     {
        //         if (_currentPingPongCantMoveCount >= ReadOnly.Numeric.MaxCanPingPongConditionCount && IsForceMovingPingPongObject == false)
        //         {
        //             CoStartForceMovePingPongObject(CellPos, ChaseCellPos, delegate ()
        //             {
        //                 _currentPingPongCantMoveCount = 0;
        //                 CoStopForceMovePingPongObject();
        //                 CreatureMoveState = ECreatureMoveState.None;
        //                 CreatureState = ECreatureState.Idle;
        //             });
        //         }
        //         else if (IsForceMovingPingPongObject == false)
        //         {
        //             Debug.Log($"<color=white>{gameObject.name}, PingPong Count: {++_currentPingPongCantMoveCount}</color>");
        //         }
        //     }

        //     _canHandleSkill = true;
        // }
*/