using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Diagnostics;
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

        public override ECreatureState CreatureState
        {
            get => base.CreatureState;
            set
            {
                base.CreatureState = value;
                switch (value)
                {
                    case ECreatureState.Move:
                        RigidBody.mass = 3.0f * 5.0f;
                        break;

                    case ECreatureState.Skill_Attack:
                        RigidBody.mass = 3.0f * 500.0f;
                        break;

                    default:
                        RigidBody.mass = 3.0f;
                        break;
                }
            }
        }

        public override ECreatureMoveState CreatureMoveState
        {
            get => base.CreatureMoveState;
            protected set
            {
                base.CreatureMoveState = value;
                switch (value)
                {
                    case ECreatureMoveState.TargetToEnemy:
                        NeedArrange = true; // -> ChangeColliderSize(EColliderSize.Large);
                        break;

                    case ECreatureMoveState.CollectEnv:
                        NeedArrange = true; // -> ChangeColliderSize(EColliderSize.Large);
                        break;

                    case ECreatureMoveState.ForceMove:
                        NeedArrange = true; // -> ChangeColliderSize(EColliderSize.Large);
                        CancelWait();
                        Target = null;
                        break;
                }
            }
        }

        private Transform Destination 
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
                    HeroBody.ChangeEnvWeapon(envTarget.EnvType);
                }
                else
                    HeroBody.ChangeDefaultWeapon();
            } 
        }

        public HeroAnimation HeroAnim { get; private set; } = null;
        
        private bool _needArrange = true;
        public bool NeedArrange
        {
            get => _needArrange;
            private set
            {
                _needArrange = value;
                if (value)
                    ChangeColliderSize(EColliderSize.Large);
                else
                    TryResizeCollider();
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Hero;

            return true;
        }

        #region Hero SetInfo
        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame();
                return false;
            }

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;

            HeroBody = new HeroBody(this, dataID);
            HeroAnim = CreatureAnim as HeroAnimation;
            HeroAnim.SetInfo(dataID, this);
            HeroStateMachine[] heroStateMachines = HeroAnim.Animator.GetBehaviours<HeroStateMachine>();
            for (int i = 0; i < heroStateMachines.Length; ++i)
            {
                heroStateMachines[i].OnHeroAnimUpdateHandler -= OnAnimationUpdate;
                heroStateMachines[i].OnHeroAnimUpdateHandler += OnAnimationUpdate;

                heroStateMachines[i].OnHeroAnimCompletedHandler -= OnAnimationCompleted;
                heroStateMachines[i].OnHeroAnimCompletedHandler += OnAnimationCompleted;
            }
            Managers.Sprite.SetInfo(dataID, target: this);

            // SetStat
            HeroData = Managers.Data.HeroDataDict[dataID];
            _maxHp = new Stat(HeroData.MaxHp);
            _atk = new Stat(HeroData.Atk);
            _atkRange = new Stat(HeroData.AtkRange);
            _movementSpeed = new Stat(HeroData.MovementSpeed);

            // TEMP
            ObjectRarity = EObjectRarity.Elite;

            gameObject.name += $"_{HeroData.DescriptionTextID.Replace(" ", "")}";
            Collider.radius = HeroData.ColliderRadius;
            EnterInGame();
            
            return true;
        }
        #endregion

        protected override void EnterInGame()
        {
            base.EnterInGame();
            
            if (UnityEngine.Random.Range(0, 100) > 50f)
                LookAtDir = ELookAtDirection.Right;
            else
                LookAtDir = ELookAtDirection.Left;
        }

        private void Update()
        {
            Debug.Log(_isCooltime);;
        }

        #region Hero AI - Idle
        protected override void UpdateIdle()
        {
            SetRigidBodyVelocity(Vector2.zero);
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                CreatureState = ECreatureState.Move;
                return;
            }

            if (Target.IsValid())
                LookAtTarget();

            if (_isCooltime)
                return;

            Creature creature = FindClosestInRange(ReadOnly.Numeric.DefaultSearchRange, Managers.Object.Monsters, func: IsValid) as Creature;
            if (creature.IsValid())
            {
                Target = creature;
                CreatureState = ECreatureState.Move;
                CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                return;
            }

            Env env = FindClosestInRange(ReadOnly.Numeric.DefaultSearchRange, Managers.Object.Envs, func: IsValid) as Env;
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
        #endregion

        #region Hero AI - Move
        protected override void UpdateMove()
        {
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                Vector3 dir = Destination.transform.position - transform.position;
                SetRigidBodyVelocity(dir.normalized * MovementSpeed);
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

                ChaseOrAttackTarget(AttackDistance, ReadOnly.Numeric.DefaultSearchRange);
                return;
            }

            if (CreatureMoveState == ECreatureMoveState.CollectEnv)
            {
                Creature creature = FindClosestInRange(ReadOnly.Numeric.DefaultSearchRange, Managers.Object.Monsters, func: IsValid) as Creature;
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

                ChaseOrAttackTarget(AttackDistance, ReadOnly.Numeric.DefaultSearchRange);
                return;
            }

            if (CreatureMoveState == ECreatureMoveState.ReturnToBase)
            {
                Vector3 toDir = Destination.position - transform.position;
                if (toDir.sqrMagnitude < ReadOnly.Numeric.DefaultStopRange * ReadOnly.Numeric.DefaultStopRange)
                {
                    CreatureMoveState = ECreatureMoveState.None;
                    CreatureState = ECreatureState.Idle;
                    NeedArrange = false; // -> TryResizeCollider()
                }
                else
                {
                    // *** 로그함수로 바꿔보기 ***
                    float ratio = Mathf.Min(1, toDir.magnitude);
                    float moveSpeed = MovementSpeed * (float)Math.Pow(ratio, 3);
                    SetRigidBodyVelocity(toDir.normalized * moveSpeed);
                }
                return;
            }

            // 눌렀다가 땟을때
            CreatureState = ECreatureState.Idle;
        }
        #endregion

        #region Hero AI - Attack
        protected override void UpdateSkillAttack()
        {
            SetRigidBodyVelocity(Vector2.zero);
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                CreatureState = ECreatureState.Move;
                return;
            }
            else if (Target.IsValid())
                LookAtTarget();

            ChangeColliderSize(EColliderSize.Default);
        }
        #endregion

        protected override void UpdateCollectEnv()
        {
            SetRigidBodyVelocity(Vector2.zero);
            if (CreatureMoveState == ECreatureMoveState.ForceMove || Target.IsValid() == false)
            {
                CollectEnv = false;
                CreatureState = ECreatureState.Move;
            }
            else if (Target.IsValid())
                LookAtTarget();
        }

        protected override void UpdateDead()
        {
            SetRigidBodyVelocity(Vector2.zero);
        }

        #region Hero Animation Events - Update
        private float Test_SkilA_Cooltime = 1.0f;
        private bool _isCooltime = false;
        protected override void OnSkillAttackAnimationUpdate()
        {
            if (Target.IsValid() == false)
                return;

            Debug.Log("INVOKED..");
            // 데미지를 주기 직전 쿨타임 적용. (TEMP, 이후 추적하지않고 Idle로 강제되는 단점이 있음)
            _isCooltime = true;
            StartWaitCooltime(Test_SkilA_Cooltime, () => _isCooltime = false);
            Target.OnDamaged(this);
        }

        protected override void OnCollectEnvAnimationUpdate()
        {
            if (Target.IsValid() == false)
                return;

            Target.OnDamaged(this);
            // if (Target.IsValid() == false)
            // {
            //     CreatureState = ECreatureState.Idle;
            //     CollectEnv = false;
            //     Debug.Log("END COLLECT ENV !!");
            // }
        }
        #endregion

        #region Hero Animation Events - Completed
        protected override void OnSkillAttackAnimationCompleted()
        {
            CreatureState = ECreatureState.Idle;
        }
        #endregion

        #region Hero - Battle
        public override void OnDamaged(BaseObject attacker)
        {
            base.OnDamaged(attacker);
        }

        public override void OnDead(BaseObject attacker)
        {
            base.OnDead(attacker);
        }
        #endregion

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

        // ColliderRadius 이런건 CreatureData로 빼서 상위로 올려도 될듯
        // 근데 어차피 길찾기 적용시킬것이라..
        protected override void ChangeColliderSize(EColliderSize colliderSize = EColliderSize.Default)
        {
            switch (colliderSize)
            {
                case EColliderSize.Small:
                    Collider.radius = HeroData.ColliderRadius * 0.8f;
                    break;

                case EColliderSize.Default:
                    Collider.radius = HeroData.ColliderRadius;
                    break;

                case EColliderSize.Large:
                    Collider.radius = HeroData.ColliderRadius * 1.2f;
                    break;
            }
        }

        protected override void TryResizeCollider()
        {
            ChangeColliderSize(EColliderSize.Small);
            
            foreach (Hero hero in Managers.Object.Heroes)
            {
                if (hero.CreatureMoveState == ECreatureMoveState.ReturnToBase)
                    return;
            }

            foreach (Hero hero in Managers.Object.Heroes)
            {
                if (hero.CreatureState == ECreatureState.Idle)
                    hero.ChangeColliderSize(EColliderSize.Large);
            }
        }
    }
}
