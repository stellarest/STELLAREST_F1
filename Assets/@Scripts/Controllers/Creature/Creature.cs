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
        public ECreatureRarity CreatureRarity { get; protected set; } = ECreatureRarity.Common;
        [SerializeField] protected ECreatureState _creatureState = ECreatureState.None;
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
            CreatureBody.ShowBody(false);
            CreatureState = ECreatureState.Idle;
            StartWait(() => BaseAnim.IsPlay() == false,
                      () => CreatureBody.ShowBody(true));

            StartCoroutine(CoUpdateAI());
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

                    case ECreatureState.Attack:
                        UpdateAttack();
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
        protected virtual void UpdateAttack() { }
        protected virtual void UpdateDead() { }

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


        protected void StartWait(System.Func<bool> waitCondition, System.Action callback = null)
        {
            CancelWait();
            _coWait = StartCoroutine(CoWait(waitCondition, callback));
        }
        private IEnumerator CoWait(System.Func<bool> waitCondition, System.Action callback = null)
        {
            yield return new WaitUntil(waitCondition);
            _coWait = null;
            callback?.Invoke();
        }



        protected void CancelWait()
        {
            if (_coWait != null)
                StopCoroutine(_coWait);
            _coWait = null;
        }
        #endregion

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

                case ECreatureState.Attack:
                    OnAttackAnimationUpdate();
                    break;
            }
        }

        protected virtual void OnIdleAnimationUpdate() { }
        protected virtual void OnMoveAnimationUpdate() { }
        protected virtual void OnAttackAnimationUpdate() { }
        #endregion

        #region Animation Events - Completed
        protected void OnAnimationComplated(ECreatureState endState)
        {
            switch (endState)
            {
                case ECreatureState.Idle:
                    OnIdleAnimationCompleted();
                    break;

                case ECreatureState.Move:
                    OnMoveAnimationCompleted();
                    break;

                case ECreatureState.Attack:
                    OnAttackAnimationCompleted();
                    break;
            }
        }

        protected virtual void OnIdleAnimationCompleted() { }
        protected virtual void OnMoveAnimationCompleted() { }
        protected virtual void OnAttackAnimationCompleted() { }
        #endregion

        #region Helper
        protected BaseObject FindClosestInRange(IEnumerable<BaseObject> objs)
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
        #endregion

        #region Battle
        public override void OnDamaged(BaseObject attacker)
        {
            if (attacker.IsValid() == false)
                return;

            // 나중에 BaseObject(ex. Cannon)에서도 적용하려면 바꿔야함.
            // 일단 지금은 Creature끼리 데미지를 주고 받도록.
            Creature creature = attacker as Creature;
            if (creature == null)
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
            attacker.Target = null;
            Managers.Object.Despawn(this); // TEMP
            //CreatureState = ECreatureState.Dead;
        }
        #endregion
    }
}
