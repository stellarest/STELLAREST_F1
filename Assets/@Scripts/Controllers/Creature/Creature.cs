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
                return false;

            CreatureAnim = BaseAnim as CreatureAnimation;
            return true;
        }

        protected override void EnterInGame()
        {
            Hp = MaxHp;
            ShowBody(false);
            StartWait(waitCondition: () => BaseAnim.IsPlay() == false,
                      waitCompleted: () => {
                          ShowBody(true);
                          Target = null;
                          CancelWait();
                          CreatureState = ECreatureState.Idle;
                          CreatureMoveState = ECreatureMoveState.None;
                          StartCoroutine(CoUpdateAI());
                      });
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
                        UpdateSkillAttack();
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
        protected virtual void UpdateSkillAttack() { }
        protected virtual void UpdateCollectEnv() { }
        protected virtual void UpdateDead() => SetRigidBodyVelocity(Vector2.zero);

        protected virtual void ChangeColliderSize(EColliderSize colliderSize = EColliderSize.Default) { }
        protected virtual void TryResizeCollider() { }

        #region Wait
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

        protected void StartWait(System.Func<bool> waitCondition, System.Action waitCompleted = null)
        {
            CancelWait();
            _coWait = StartCoroutine(CoWait(waitCondition, waitCompleted));
        }
        private IEnumerator CoWait(System.Func<bool> waitCondition, System.Action waitCompleted = null)
        {
            yield return new WaitUntil(waitCondition);
            _coWait = null;
            waitCompleted?.Invoke();
        }

        // seconds, callback
        protected void StartWait(float seconds, System.Action callback = null)
        {
            //CancelWait();
            //_coWait = StartCoroutine(CoStartWait(seconds, callback));
            StartCoroutine(CoWait(seconds, callback));
        }
        private IEnumerator CoWait(float seconds, System.Action callback = null)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }

        protected void CancelWait()
        {
            if (_coWait != null)
                StopCoroutine(_coWait);
            _coWait = null;
        }

        #endregion

        /*
            Idle,
            Move,
            Skill_Attack,
            Skill_A,
            Skill_B,
            CollectEnv,
            OnDamaged,
            Dead,
        */

        #region Animation Events - Update
        protected void OnAnimationUpdate(ECreatureState updateState)
        {
            switch (updateState)
            {
                case ECreatureState.Idle:
                    OnIdleAnimationUpdate();
                    break;

                case ECreatureState.Move:
                    OnMoveAnimationUpdate();
                    break;

                case ECreatureState.Skill_Attack:
                    OnSkillAttackAnimationUpdate();
                    break;

                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    break;

                case ECreatureState.CollectEnv:
                    OnCollectEnvAnimationUpdate();
                    break;

                case ECreatureState.OnDamaged:
                    break;

                case ECreatureState.Dead:
                    OnDeadAnimationUpdate();
                    break;
            }
        }

        protected virtual void OnIdleAnimationUpdate() { }
        protected virtual void OnMoveAnimationUpdate() { }
        protected virtual void OnSkillAttackAnimationUpdate() { }
        protected virtual void OnCollectEnvAnimationUpdate() { }
        protected virtual void OnDeadAnimationUpdate() 
        {
            
        }
        #endregion

        /*
            Idle,
            Move,
            Skill_Attack,
            Skill_A,
            Skill_B,
            CollectEnv,
            OnDamaged,
            Dead,
        */

        #region Animation Events - Completed
        protected void OnAnimationCompleted(ECreatureState endState)
        {
            switch (endState)
            {
                case ECreatureState.Idle:
                    OnIdleAnimationCompleted();
                    break;

                case ECreatureState.Move:
                    OnMoveAnimationCompleted();
                    break;

                case ECreatureState.Skill_Attack:
                    OnSkillAttackAnimationCompleted();
                    break;

                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    break;

                case ECreatureState.CollectEnv:
                    OnCollectEnvAnimationCompleted();
                    break;

                case ECreatureState.OnDamaged:
                    break;

                case ECreatureState.Dead:
                    break;
            }
        }

        protected virtual void OnIdleAnimationCompleted() { }
        protected virtual void OnMoveAnimationCompleted() { }
        protected virtual void OnSkillAttackAnimationCompleted() { }
        protected virtual void OnCollectEnvAnimationCompleted() { }
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

        protected void ChaseOrAttackTarget(float attackRange, float chaseRange)
        {
            Vector3 toDir = Target.transform.position - transform.position;
            float distToTargetSQR = toDir.sqrMagnitude;

            float attackDistToTargetSQR = attackRange * attackRange;
            // ATTACK
            if (attackDistToTargetSQR >= distToTargetSQR)
            {
                if (CreatureMoveState == ECreatureMoveState.TargetToEnemy)
                {
                    CreatureState = ECreatureState.Skill_Attack;
                }
                else if (CreatureMoveState == ECreatureMoveState.CollectEnv)
                {
                    CollectEnv = true;
                    CreatureState = ECreatureState.CollectEnv;
                }
                return;
            }
            // CHASE
            else
            {
                SetRigidBodyVelocity(toDir.normalized * MovementSpeed);
                // 너무 멀어지면 포기
                if (distToTargetSQR > chaseRange * chaseRange)
                {
                    Target = null;
                    CreatureState = ECreatureState.Move;
                }
                return;
            }
        }

        protected void LookAtTarget()
        {
            Vector3 toTargetDir = Target.transform.position - transform.position;
            if (toTargetDir.x < 0)
                LookAtDir = ELookAtDirection.Left;
            else
                LookAtDir = ELookAtDirection.Right;
        }

        protected bool IsValid(BaseObject bo) => bo.IsValid();
        #endregion

        #region Battle
        public override void OnDamaged(BaseObject attacker)
        {
            Creature creature = attacker as Creature;
            if (creature.IsValid() == false)
                return;

            float finalDamage = creature.Atk;
            Hp = UnityEngine.Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);

            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker);
            }
        }

        public override void OnDead(BaseObject attacker)
        {
            //attacker.Target = null;
            //Managers.Object.Despawn(this); // TEMP
            CreatureState = ECreatureState.Dead;
            base.OnDead(attacker);
        }
        #endregion
    }
}
