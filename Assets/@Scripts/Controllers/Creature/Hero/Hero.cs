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
                        NeedArrange = true;
                        break;

                    case EHeroMoveState.TargetMonster:
                        NeedArrange = true;
                        break;

                    case EHeroMoveState.ForceMove:
                        NeedArrange = true;
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
            heroStateMachine.OnHeroAnimationComplatedHandler -= OnAnimationComplated;
            heroStateMachine.OnHeroAnimationComplatedHandler += OnAnimationComplated;
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
                return;

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
                    NeedArrange = false;
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

        // Helper
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

        private BaseObject FindClosestInRange(IEnumerable<BaseObject> objs)
        {
            BaseObject target = null;
            float bestDistanceSQR = float.MaxValue;

            foreach (BaseObject obj in objs)
            {
                Vector3 dir = obj.transform.position - transform.position;
                float distToTargetSqr = dir.sqrMagnitude;

                if (SearchDistanceSQR < distToTargetSqr)
                    continue;

                if (bestDistanceSQR < distToTargetSqr)
                    continue;

                bestDistanceSQR = distToTargetSqr;
                target = obj;
            }

            return target;
        }

        public float TestAttackCoolTime = 2f;
        #region Hero Animation Events
        protected override void OnAttackAnimationCompleted()
        {
            CreatureState = ECreatureState.Idle;
            StartWait(TestAttackCoolTime);
        }
        #endregion
    }
}
