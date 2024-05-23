using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using STELLAREST_F1.Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Lumin;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Hero : Creature
    {
        public Data.HeroData HeroData { get; private set; } = null;

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
        public override ECreatureMoveState CreatureMoveState
        {
            get => base.CreatureMoveState;
            protected set
            {
                base.CreatureMoveState = value;
                switch (value)
                {
                    case ECreatureMoveState.TargetToEnemy:
                    case ECreatureMoveState.CollectEnv:
                        NeedArrange = true;
                        break;

                    case ECreatureMoveState.ForceMove:
                        NeedArrange = true;
                        CancelWait();
                        Target = null;
                        break;
                }
            }
        }

        // private Transform CampDestination
        // {
        //     get
        //     {
        //         HeroCamp camp = Managers.Object.Camp;
        //         // ***** Leader Test *****
        //         // if (CreatureMoveState == ECreatureMoveState.ReturnToBase)
        //         //     return camp.Pivot;

        //         return camp.Pointer;
        //     }
        // }

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

        public HeroAnimation HeroAnim { get; private set; } = null;
        [field: SerializeField] public bool NeedArrange { get; set; } = false;

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
                    StopLerpToCellPos();
                else
                    TickLerpToCellPos();
            }
        }

        #region ##### TEST AREA #####   
        // ########################################
        private void Update()
        {
            // Managers.Map.CanMove(transform.position, ignoreObjects: false, ignoreSemiWall: true);
            // Managers.Map.CheckOnTile(transform.position);
            // Managers.Map.CheckOnTile(this);
            // if (Input.GetKeyDown(KeyCode.T))
            // {
            // }

            if (Input.GetKeyDown(KeyCode.S))
            {
                //Managers.Game.ReplaceHeroes();
            }

        }
        // ########################################
        #endregion

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Hero;
            Collider.isTrigger = true;
            RigidBody.simulated = false;
            return true;
        }

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

            EnterInGame(dataID);
            return true;
        }

        protected override void EnterInGame(int dataID)
        {
            LookAtDir = ELookAtDirection.Right;
            base.EnterInGame(dataID);
            
            // 나오고 나서 조이스틱 등록
            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;

            // _onLerpToCellPosEndHandler -= OnLerpToCellPosEnd;
            // _onLerpToCellPosEndHandler += OnLerpToCellPosEnd;

            // TEST
            // _onFindPathEndHandler += (delegate ()
            // {
            //     // 기본적으로는 무지성으로 쫓아오고
            //     // 리더가 멈추면 Repoistion하게 해도 되고. (쫙 펼쳐지는 느낌으로)
            //     if (CreatureMoveState != ECreatureMoveState.ForceMove)
            //     {
            //         Debug.Log("INVOKE.");
            //         CreatureMoveState = ECreatureMoveState.None;
            //         CreatureState = ECreatureState.Idle;
            //     }
            // });

            // A* Test
            // NeedArrange = true; // TEMP
        }

        // 요행은 없다.
        public Vector3Int ChaseCellPos
        {
            get
            {
                HeroLeaderController heroLeaderController = Managers.Object.HeroLeaderController;
                switch (heroLeaderController.HeroLeaderChaseMode)
                {
                    case EHeroLeaderChaseMode.JustFollowClosely:
                        return Managers.Map.WorldToCell(heroLeaderController.Leader.transform.position);;

                    case EHeroLeaderChaseMode.NarrowFormation:
                    case EHeroLeaderChaseMode.WideFormation:
                        return Managers.Object.HeroLeaderController.RequestChaseCellPos(this); ;

                    case EHeroLeaderChaseMode.Freedom:
                    default:
                        return Vector3Int.zero;
                }
            }
        }

        #region ##### AI #####
        protected override void UpdateIdle()
        {
            if (IsLeader)
                return;

            // Idle to Move
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                CreatureState = ECreatureState.Move;
                return;
            }

            // if (CreatureMoveState == ECreatureMoveState.ForceMove || CreatureMoveState == ECreatureMoveState.Replace)
            // {
            //     CreatureState = ECreatureState.Move;
            //     return;
            // }

            // if (CreatureMoveState == ECreatureMoveState.ForceMove || CreatureMoveState == ECreatureMoveState.Replace)
            // {
            //     CreatureState = ECreatureState.Move;
            //     return;
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
        }

        protected override void UpdateMove()
        {
            if (IsLeader)
                return;

            EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos,
                maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

            // ForceMove 상태일때는 계속 움직임
            if (CreatureMoveState == ECreatureMoveState.None)
            {
                // 여기 때문에 실시간으로 안따라가는 거긴한데...
                if (result == EFindPathResult.Fail_NoPath)
                {
                    CreatureMoveState = ECreatureMoveState.None;
                    CreatureState = ECreatureState.Idle;
                    return;
                }
            }

            // if (CreatureState == ECreatureState.Move)
            // {
            //     if (CreatureMoveState == ECreatureMoveState.Replace)
            //     {
            //         EFindPathResult result = FindPathAndMoveToCellPos(destPos: _replaceDestPos, 
            //             maxDepth: ReadOnly.Numeric.HeroMaxMoveDepth);
            //         if (result == EFindPathResult.Fail_NoPath)
            //         {
            //             Debug.Log($"Replace NoPath: {gameObject.name}");
            //             CreatureMoveState = ECreatureMoveState.None;
            //             CreatureState = ECreatureState.Idle;
            //         }
            //     }
            //     else if (CreatureMoveState == ECreatureMoveState.ForceMove)
            //     {
            //         EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos, 
            //             maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);
            //         if (result == EFindPathResult.Fail_NoPath)
            //         {
            //             CreatureMoveState = ECreatureMoveState.None;
            //             CreatureState = ECreatureState.Idle;
            //         }
            //     }
            //     return;
            // }
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

        protected override void UpdateSkill()
        {
            base.UpdateSkill();
            // SetRigidBodyVelocity(Vector2.zero); - DELETED
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                CreatureState = ECreatureState.Move;
                return;
            }
            else if (Target.IsValid())
                LookAtTarget(Target);
            else if (Target.IsValid() == false)
            {
                CreatureState = ECreatureState.Move;
                return;
            }
        }

        protected override void UpdateCollectEnv()
        {
            // SetRigidBodyVelocity(Vector2.zero); - DELETED
            if (CreatureMoveState == ECreatureMoveState.ForceMove || Target.IsValid() == false)
            {
                CollectEnv = false;
                CreatureState = ECreatureState.Move;
            }
            else if (Target.IsValid())
            {
                // Research Enemies
                Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Monsters, func: IsValid) as Creature;
                if (creature != null)
                {
                    CollectEnv = false;
                    Target = creature;
                    CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                LookAtTarget(Target);
            }
        }

        protected override void UpdateDead()
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

        protected override IEnumerator CoDeadFadeOut(Action callback = null)
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

        #region Event
        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            switch (joystickState)
            {
                case EJoystickState.PointerUp:
                    CreatureMoveState = ECreatureMoveState.None;
                    break;

                case EJoystickState.Drag:
                    CreatureMoveState = ECreatureMoveState.ForceMove;
                    break;

                default:
                    break;
            }
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

        // TEST
        // private void OnLerpToCellPosEnd()
        // {
        //     if (CreatureState == ECreatureState.Move)
        //     {
        //         if (CreatureMoveState == ECreatureMoveState.Replace)
        //         {
        //             // 이걸로 체크해서 막힌 곳에 갔을 때 Idle 상태로 전환못함.
        //             // CellPos == _replaceDestPos -> (CellPos - _replaceDestPos).magnitude <= 1f : 1칸 이하일 때
        //             if (CellPos == _replaceDestPos)
        //             {
        //                 CreatureMoveState = ECreatureMoveState.None;
        //                 CreatureState = ECreatureState.Idle;
        //                 return;
        //             }
        //         }
        //         // 계속 ForceMove State일 때는... 중간에 Idle로 만들어주면 안된다!!
        //         else if (CreatureMoveState != ECreatureMoveState.ForceMove)
        //         {
        //             CreatureMoveState = ECreatureMoveState.None;
        //             CreatureState = ECreatureState.Idle;
        //             return;
        //         }
        //     }
        // }
    }
}

/*
                // Research Enemies
                // Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Monsters, func: IsValid) as Creature;
                // if (creature != null)
                // {
                //     CollectEnv = false;
                //     Target = creature;
                //     CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                //     CreatureState = ECreatureState.Move;
                //     return;
                // }

                // Vector3 toDir = CampDestination.position - transform.position;
                // float stopDistSQR = ReadOnly.Numeric.Temp_StopDistance * ReadOnly.Numeric.Temp_StopDistance;
                // if (toDir.sqrMagnitude < stopDistSQR)
                // {
                //     CreatureMoveState = ECreatureMoveState.None;
                //     CreatureState = ECreatureState.Idle;
                //     NeedArrange = false; // -> TryResizeCollider()
                // }
                // else
                // {
                //     // *** 로그함수로 바꿔보기 ***
                //     // float ratio = Mathf.Min(1, toDir.magnitude);
                //     // float moveSpeed = MovementSpeed * (float)Math.Pow(ratio, 3);
                //     // SetRigidBodyVelocity(toDir.normalized * moveSpeed);

                //     // 캠프로 돌아올 때만 거리에 따라 스피드 조정
                //     // float movementSpeed = CalculateMovementSpeed(toDir.sqrMagnitude);
                    
                //     float movementSpeed = Util.CalculateValueFromDistance(
                //                                 value: MovementSpeed, 
                //                                 maxValue: MovementSpeed * 2f,
                //                                 distanceToTargetSQR: toDir.sqrMagnitude,
                //                                 maxDistanceSQR: ReadOnly.Numeric.Temp_ScanRange);
                //     //Debug.Log($"MovementSpeed: {movementSpeed}");
                //     // SetRigidBodyVelocity(toDir.normalized * movementSpeed); - DELETED
                // }
*/