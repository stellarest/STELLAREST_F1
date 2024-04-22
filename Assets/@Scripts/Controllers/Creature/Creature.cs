using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Creature : BaseObject
    {
        public SkillComponent CreatureSkillComponent { get; protected set; } = null;
        public CreatureBody CreatureBody { get; protected set; } = null;
        public CreatureAnimation CreatureAnim { get; private set; } = null;

        [SerializeField] private ECreatureState _creatureState = ECreatureState.None;
        public virtual ECreatureState CreatureState
        {
            get => _creatureState;
            set
            {
                if (_creatureState != value)
                {
                    _creatureState = value;
                    UpdateAnimation();
                }
            }
        }

        [SerializeField] private ECreatureMoveState _creatureMoveState = ECreatureMoveState.None;
        public virtual ECreatureMoveState CreatureMoveState
        {
            get => _creatureMoveState;
            protected set => _creatureMoveState = value;
        }

        private bool _collectEnv = false;
        public virtual bool CollectEnv
        {
            get => _collectEnv;
            protected set => _collectEnv = value;
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame();
                return false;
            }

            CreatureAnim = BaseAnim as CreatureAnimation;
            //EnterInGame();
            return true;
        }

        public HeroBody HeroBody;

        protected override void EnterInGame()
        {
            base.EnterInGame();
            RigidBody.simulated = false;
            ShowBody(false);
            StartWait(waitCondition: () => BaseAnim.IsPlay() == false,
                      waitCompleted: () => {
                          ShowBody(true);
                          RigidBody.simulated = true;
                          Target = null;
                          CancelWait();
                          AddAnimationEvents();
                          CreatureState = ECreatureState.Idle;
                          CreatureMoveState = ECreatureMoveState.None;
                          StartCoroutine(CoUpdateAI());
                      });
        }

        private void AddAnimationEvents()
        {
            CreatureStateMachine[] creatureStateMachines = CreatureAnim.Animator.GetBehaviours<CreatureStateMachine>();
            for (int i = 0; i < creatureStateMachines.Length; ++i)
            {
                creatureStateMachines[i].OnStateEnterHandler -= OnStateEnter;
                creatureStateMachines[i].OnStateEnterHandler += OnStateEnter;

                creatureStateMachines[i].OnStateUpdateHandler -= OnStateUpdate;
                creatureStateMachines[i].OnStateUpdateHandler += OnStateUpdate;

                creatureStateMachines[i].OnStateEndHandler -= OnStateEnd;
                creatureStateMachines[i].OnStateEndHandler += OnStateEnd;
            }
        }

        private void ReleaseAnimationEvents()
        {
            if (CreatureAnim == null)
                return;

            CreatureStateMachine[] creatureStateMachines = CreatureAnim.Animator.GetBehaviours<CreatureStateMachine>();
            for (int i = 0; i < creatureStateMachines.Length; ++i)
            {
                creatureStateMachines[i].OnStateEnterHandler -= OnStateEnter;
                creatureStateMachines[i].OnStateUpdateHandler -= OnStateUpdate;
                creatureStateMachines[i].OnStateEndHandler -= OnStateEnd;
            }
        }

        protected IEnumerator CoUpdateAI()
        {
            while (true)
            {
                switch (CreatureState)
                {
                    case ECreatureState.Idle:
                        UpdateIdle();
                        break;

                    case ECreatureState.Move:
                        UpdateMove();
                        break;

                    case ECreatureState.Skill_Attack:
                    case ECreatureState.Skill_A:
                    case ECreatureState.Skill_B:
                        UpdateSkill();
                        break;

                    case ECreatureState.CollectEnv:
                        UpdateCollectEnv();
                        break;

                    case ECreatureState.Dead:
                        UpdateDead();
                        break;
                }
                
                yield return null;
            }
        }

        protected virtual void UpdateIdle() { }
        protected virtual void UpdateMove() { }
        protected virtual void UpdateSkill() { }
        protected virtual void UpdateCollectEnv() { }
        protected virtual void UpdateDead()
        {
            SetRigidBodyVelocity(Vector2.zero);
            CancelWait();
        }
        protected virtual void ChangeColliderSize(EColliderSize colliderSize = EColliderSize.Default) { }
        protected virtual void TryResizeCollider() { }

        #region Coroutines
        // Co Wait 없앨준비하라네
        protected Coroutine _coWait = null;
        protected void StartWait(float seconds)
        {
            CancelWait();
            _coWait = StartCoroutine(CoWait(seconds));
        }
        private IEnumerator CoWait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _coWait = null;
        }

        protected void StartWait(Func<bool> waitCondition, Action waitCompleted = null)
        {
            CancelWait();
            _coWait = StartCoroutine(CoWait(waitCondition, waitCompleted));
        }
        private IEnumerator CoWait(Func<bool> waitCondition, Action waitCompleted = null)
        {
            yield return new WaitUntil(waitCondition);
            _coWait = null;
            waitCompleted?.Invoke();
        }

        protected void CancelWait()
        {
            if (_coWait != null)
                StopCoroutine(_coWait);
            _coWait = null;
        }
        #endregion

        #region State Machine Events 
        protected void OnStateEnter(ECreatureState enterState)
        {
            switch (enterState)
            {
                case ECreatureState.Skill_Attack:
                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    CreatureSkillComponent?.PassOnSkillStateEnter(enterState);
                    break;

                case ECreatureState.CollectEnv:
                    OnCollectEnvStateEnter();
                    break;
            }
        }
        protected virtual void OnCollectEnvStateEnter()
        {
             CollectEnv = true;
        }

        protected void OnStateUpdate(ECreatureState updateState)
        {
            switch (updateState)
            {
                case ECreatureState.Skill_Attack:
                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    // 스킬을 사용하는 주체가 크리처이기 때문에 여기서 이벤트 등록, 삭제하고 호출
                    CreatureSkillComponent.PassOnSkillStateUpdate(updateState);
                    break;

                case ECreatureState.CollectEnv:
                    OnCollectEnvStateUpdate();
                    break;
            }
        }

        protected virtual void OnCollectEnvStateUpdate() { }

        protected void OnStateEnd(ECreatureState endState)
        {
            switch (endState)
            {
                case ECreatureState.Skill_Attack:
                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    CreatureSkillComponent?.PassOnSkillStateEnd(endState);
                    break;
            }
        }
        #endregion

        #region Helper
        protected BaseObject FindClosestInRange(float range, IEnumerable<BaseObject> objs, Func<BaseObject, bool> func = null)
        {
            BaseObject target = null;
            float bestDistanceSQR = float.MaxValue;
            float searchDistanceSQR = range * range;

            foreach (BaseObject obj in objs)
            {
                Vector3 dir = obj.transform.position - transform.position;
                float distToTargetSqr = dir.sqrMagnitude;

                if (searchDistanceSQR < distToTargetSqr)
                    continue;

                if (bestDistanceSQR < distToTargetSqr)
                    continue;

                // 추가 조건 (ex)객체가 살아있는가? 람다 기반으로 넘겨줄수도 있으니까 뭐, 편함 이런게 있으면.
                // 살아있는 객체만 찾아서, 살아있는 객체에게만 Target을 잡을 수 있도록. 이런식으로 가능.
                if (func?.Invoke(obj) == false)
                    continue;

                bestDistanceSQR = distToTargetSqr;
                target = obj;
            }

            return target;
        }

        #region Battle
        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            Creature creature = attacker as Creature;
            if (creature.IsValid() == false)
                return;

            float finalDamage = creature.Atk;
            Hp = UnityEngine.Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);

            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillFromAttacker);
            }
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            CreatureState = ECreatureState.Dead;
            base.OnDead(attacker, skillFromAttacker);
        }
        #endregion

        protected void ChaseOrAttackTarget(float chaseRange, float atkRange)
        {
            Vector3 toTargetDir = Target.transform.position - transform.position;
            if (DistanceToTargetSQR <= atkRange * atkRange)
            {
                if (Target.IsValid() && Target.ObjectType == EObjectType.Env)
                    CreatureState = ECreatureState.CollectEnv;
                else
                    CreatureSkillComponent?.CurrentSkill.DoSkill();
            }
            else
            {
                SetRigidBodyVelocity(toTargetDir.normalized * MovementSpeed);
                float searchDistSQR = chaseRange * chaseRange;
                if (DistanceToTargetSQR > searchDistSQR)
                {
                    Target = null;
                    CreatureState = ECreatureState.Move;
                }
            }
        }

        protected float CalculateMovementSpeed(float distanceToTargetSQR)
        {
            float maxDistance = ReadOnly.Numeric.MaxDistanceForMovementSpeed;
            float maxMovementSpeed = MovementSpeed * ReadOnly.Numeric.MaxMovementSpeedMultiplier;
            float movementSpeed = Mathf.Lerp(MovementSpeed, 
                                            maxMovementSpeed, 
                                            Mathf.Log(distanceToTargetSQR + 1.0f) / Mathf.Log(maxDistance * maxDistance + 1.0f));

            return movementSpeed;
        }

        protected virtual void OnDisable()
        {
            Debug.Log("Creature::OnDisable");
            ReleaseAnimationEvents();
        }

        protected bool IsValid(BaseObject bo) => bo.IsValid();
        #endregion
    }
}
