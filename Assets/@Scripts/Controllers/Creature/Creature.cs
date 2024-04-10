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

        [SerializeField] private bool[] _animState = null;
        public bool this[ECreatureState state]
        {
            get => _animState[(int)state];
            set => _animState[(int)state] = value;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                CreatureState = ECreatureState.Attack;

            if (Input.GetKeyDown(KeyCode.Q))
                CreatureState = ECreatureState.Idle;
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _animState = new bool[(int)ECreatureState.Max];

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

        #region Animation Events
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
    }
}
