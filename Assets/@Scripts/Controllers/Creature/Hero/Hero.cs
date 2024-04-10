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
                if (value == ECreatureState.Move)
                    RigidBody.mass = 3.0f;
                else
                    RigidBody.mass = 0.3f;
            }
        }

        public HeroAnimation HeroAnim { get; private set; } = null;

        [SerializeField] private EHeroMoveState _heroMoveState = EHeroMoveState.None;
        public EHeroMoveState HeroMoveState
        {
            get => _heroMoveState;
            private set
            {
                _heroMoveState = value;
                switch (value)
                {
                    case EHeroMoveState.CollectEnv:
                        NeedArrange = true; // -> ChangeColliderSize(EColliderSize.Large);
                        break;

                    case EHeroMoveState.TargetMonster:
                        NeedArrange = true; // -> ChangeColliderSize(EColliderSize.Large);
                        break;

                    case EHeroMoveState.ForceMove:
                        NeedArrange = true; // -> ChangeColliderSize(EColliderSize.Large);
                        break;
                }
            }
        }

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
            HeroStateMachine heroStateMachine = HeroAnim.Animator.GetBehaviour<HeroStateMachine>();
            heroStateMachine.OnHeroAnimUpdateHandler -= OnAnimationUpdate;
            heroStateMachine.OnHeroAnimUpdateHandler += OnAnimationUpdate;
            heroStateMachine.OnHeroAnimComplatedHandler -= OnAnimationComplated;
            heroStateMachine.OnHeroAnimComplatedHandler += OnAnimationComplated;
            Managers.Sprite.SetInfo(dataID, target: this);

            // SetStat
            HeroData = Managers.Data.HeroDataDict[dataID];
            _maxHp = new Stat(HeroData.MaxHp);
            _atk = new Stat(HeroData.Atk);
            _atkRange = new Stat(HeroData.AtkRange);
            _movementSpeed = new Stat(HeroData.MovementSpeed);

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

        #region Hero AI - Idle
        protected override void UpdateIdle()
        {
            if (HeroMoveState == EHeroMoveState.ForceMove)
            {
                CreatureState = ECreatureState.Move;
                return;
            }

            if (_coWait != null)
            {
                return;
            }

            Creature creature = FindClosestInRange(Managers.Object.Monsters) as Creature;
            if (creature != null)
            {
                Target = creature;
                CreatureState = ECreatureState.Move;
                HeroMoveState = EHeroMoveState.TargetMonster;
                return;
            }

            Env env = FindClosestInRange(Managers.Object.Envs) as Env;
            if (env != null)
            {
                Target = env;
                CreatureState = ECreatureState.Move;
                HeroMoveState = EHeroMoveState.CollectEnv;
                return;
            }

            if (NeedArrange)
            {
                CreatureState = ECreatureState.Move;
                HeroMoveState = EHeroMoveState.ReturnToCamp;
                return;
            }
        }
        #endregion

        #region Hero AI - Move
        protected override void UpdateMove()
        {
            if (HeroMoveState == EHeroMoveState.ForceMove)
            {
                Vector3 dir = CampDestination.transform.position - transform.position;
                SetRigidBodyVelocity(dir.normalized * MovementSpeed);
                return;
            }

            if (HeroMoveState == EHeroMoveState.TargetMonster)
            {
                if (Target.IsValid() == false)
                {
                    HeroMoveState = EHeroMoveState.None;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                ChaseOrAttackTarget();
                return;
            }

            if (HeroMoveState == EHeroMoveState.CollectEnv)
            {
                Creature creature = FindClosestInRange(Managers.Object.Monsters) as Creature;
                if (creature != null)
                {
                    Target = creature;
                    HeroMoveState = EHeroMoveState.TargetMonster;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                // 이미 채집했으면 포기
                if (Target.IsValid() == false)
                {
                    HeroMoveState = EHeroMoveState.None;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                ChaseOrAttackTarget();
                return;
            }

            if (HeroMoveState == EHeroMoveState.ReturnToCamp)
            {
                Vector3 toDir = CampDestination.position - transform.position;
                if (toDir.sqrMagnitude < StopDistanceSQR * StopDistanceSQR)
                {
                    HeroMoveState = EHeroMoveState.None;
                    CreatureState = ECreatureState.Idle;
                    NeedArrange = false; // -> TryResizeCollider()
                }
                else
                {
                    // 로그함수로 바꿔보기.
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
        protected override void UpdateAttack()
        {
            if (HeroMoveState == EHeroMoveState.ForceMove)
            {
                CreatureState = ECreatureState.Move;
                return;
            }

            if (Target.IsValid() == false)
            {
                CreatureState = ECreatureState.Move;
                return;
            }
            else
            {
                Vector3 toTargetDir = Target.transform.position - transform.position;
                if (toTargetDir.x < 0)
                    LookAtDir = ELookAtDirection.Left;
                else
                    LookAtDir = ELookAtDirection.Right;
            }

            ChangeColliderSize(EColliderSize.Default);
        }
        #endregion

        protected override void UpdateDead()
        {
        }

        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            switch (joystickState)
            {
                case EJoystickState.PointerUp:
                    HeroMoveState = EHeroMoveState.None;
                    break;

                case EJoystickState.Drag:
                    HeroMoveState = EHeroMoveState.ForceMove;
                    CancelWait();
                    break;

                default:
                    break;
            }
        }

        private void ChaseOrAttackTarget()
        {
            Vector3 toDir = Target.transform.position - transform.position;
            float distToTargetSQR = toDir.sqrMagnitude;
            float attackDistToTargetSQR = AttackDistance * AttackDistance;

            if (attackDistToTargetSQR >= distToTargetSQR)
            {
                CreatureState = ECreatureState.Attack;
                return;
            }
            else
            {
                SetRigidBodyVelocity(toDir.normalized * MovementSpeed);
                
                // 너무 멀어지면 포기
                if (distToTargetSQR > SearchDistanceSQR)
                {
                    Target = null;
                    HeroMoveState = EHeroMoveState.None;
                    CreatureState = ECreatureState.Move;
                }
                return;
            }
        }

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
                if (hero.HeroMoveState == EHeroMoveState.ReturnToCamp)
                    return;
            }

            foreach (Hero hero in Managers.Object.Heroes)
            {
                if (hero.CreatureState == ECreatureState.Idle)
                    hero.ChangeColliderSize(EColliderSize.Large);
            }
        }

        private Transform CampDestination
        {
            get
            {
                HeroCamp camp = Managers.Object.Camp;
                if (HeroMoveState == EHeroMoveState.ReturnToCamp)
                    return camp.Pivot;

                return camp.Destination;
            }
        }

        #region Hero Animation Events - Update
        protected override void OnAttackAnimationUpdate()
        {
            if (Target.IsValid() == false)
                return;

            Target.OnDamaged(this);
        }
        #endregion

        #region Hero Animation Events - Completed
        private float TestCoolTime = 1.25f;
        protected override void OnAttackAnimationCompleted()
        {
            if (Target.IsValid())
                StartWait(TestCoolTime);

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
    }
}
