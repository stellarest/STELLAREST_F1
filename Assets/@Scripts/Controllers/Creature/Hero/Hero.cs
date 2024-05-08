using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

        private Transform CampDestination
        {
            get
            {
                HeroCamp camp = Managers.Object.Camp;
                if (CreatureMoveState == ECreatureMoveState.ReturnToBase)
                    return camp.Pivot;

                return camp.Destination;
            }
        }

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

        // NeedArrange -> MoveStartFlag: 이름 바꿔도 될듯
        [field: SerializeField] public bool NeedArrange { get; set; } = false;
        #region ##### TEST AREA #####   
        // ########################################
        private void Update()
        {
            // Managers.Map.CanMove(transform.position, ignoreObjects: false, ignoreSemiWall: true);
            // Managers.Map.CheckOnTile(transform.position);
            // Managers.Map.CheckOnTile(this);
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
        }

        #region AI
        protected override void UpdateIdle()
        {
            // SetRigidBodyVelocity(Vector2.zero); - DELETED
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                CreatureState = ECreatureState.Move;
                return;
            }

            if (Target.IsValid())
                LookAtTarget(Target);

            // 조금 극단적인 방법. 쳐다보면서 가만히 짱박혀있어라.
            if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
                return;

            Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Monsters, func: IsValid) as Creature;
            if (creature.IsValid())
            {
                Target = creature;
                CreatureState = ECreatureState.Move;
                CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                return;
            }

            Env env = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Envs, func: IsValid) as Env;
            if (env.IsValid())
            {
                Target = env;
                CreatureState = ECreatureState.Move;
                CreatureMoveState = ECreatureMoveState.CollectEnv;
                return;
            }

            if (NeedArrange)
            {
                CreatureState = ECreatureState.Move;
                CreatureMoveState = ECreatureMoveState.ReturnToBase;
                return;
            }
        }

        protected override void UpdateMove()
        {
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                //EFindPathResult result = FindPathAndMoveToCellPos(CampDestination.position, ReadOnly.Numeric.HeroDefaultMoveDepth);
                return;
            }

            if (CreatureMoveState == ECreatureMoveState.TargetToEnemy)
            {
                if (Target.IsValid() == false)
                {
                    CreatureMoveState = ECreatureMoveState.None;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                ChaseOrAttackTarget(ReadOnly.Numeric.Temp_ScanRange, AttackDistance);
                return;
            }

            if (CreatureMoveState == ECreatureMoveState.CollectEnv)
            {
                // Research Enemies
                Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Monsters, func: IsValid) as Creature;
                if (creature != null)
                {
                    Target = creature;
                    CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                // 이미 채집했으면 포기
                if (Target.IsValid() == false)
                {
                    CreatureMoveState = ECreatureMoveState.None;
                    CreatureState = ECreatureState.Move;
                    CollectEnv = false;
                    return;
                }

                ChaseOrAttackTarget(ReadOnly.Numeric.Temp_ScanRange, AttackDistance);
                return;
            }

            if (CreatureMoveState == ECreatureMoveState.ReturnToBase)
            {
                Vector3 destPos = CampDestination.position;
                if (FindPathAndMoveToCellPos(destPos, ReadOnly.Numeric.HeroDefaultMoveDepth) == EFindPathResult.Success)
                    return;

                // 실패사유 검사
                BaseObject obj = Managers.Map.GetObject(destPos);
                if (obj.IsValid())
                {
                    // 그 자리가 이미 나였다면
                    if (obj == this)
                    {
                        CreatureMoveState = ECreatureMoveState.None;
                        NeedArrange = false;
                        return;
                    }

                    // 다른 히어로가 그 자리에 서있는 것임
                    Hero hero = obj as Hero;
                    if (hero != null && hero.CreatureState == ECreatureState.Idle)
                    {
                        CreatureMoveState = ECreatureMoveState.None;
                        NeedArrange = false;
                        return;
                    }
                }

                return;
            }

            if (LerpToCellPosCompleted)
                CreatureState = ECreatureState.Idle;
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

        #region ##### Event: Hero Move State #####
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