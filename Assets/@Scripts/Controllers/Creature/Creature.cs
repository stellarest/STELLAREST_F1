using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Creature : BaseObject
    {
        public float Speed { get; protected set; } = 1.0f; // TEMP
        public CreatureBody CreatureBody { get; protected set; } = null;
        public CreatureAnimation CreatureAnim { get; private set; } = null;
        public ECreatureRarity CreatureRarity { get; protected set; } = ECreatureRarity.Common;
        [SerializeField]
        protected ECreatureState _creatureState = ECreatureState.None;
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

        public override void SetInfo(int dataID)
        {
            base.SetInfo(dataID);
            if (CreatureAnim == null)
                CreatureAnim = BaseAnim as CreatureAnimation;
        }

        protected virtual void SetCreatureFromData(int dataID) {  }

        protected virtual void RefreshCreature()
        {
            CreatureBody.ShowBody(false);
            // *** 모든 크리쳐는 Idle State에서 시작 (고정) ***
            CreatureState = ECreatureState.Idle;
            StartWait(() => BaseAnim.IsPlay() == false, 
                      () => CreatureBody.ShowBody(true));

            // 일단은 몬스터에만..
            //StartCoroutine(CoUpdateAI());
        }

        // AI
        public float UpdateAITick { get; protected set; } = 0f;
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

                if (UpdateAITick > 0f)
                    yield return new WaitForSeconds(UpdateAITick);
                else
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
            // _coWait가 null인지 아닌지만 판단할 수 있게 되었다.
            _coWait = StartCoroutine(CoWait(seconds));
        }

        private IEnumerator CoWait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _coWait = null;
        }

        protected void StartWait(System.Func<bool> func, System.Action callback = null)
        {
            CancelWait();
            _coWait = StartCoroutine(CoWait(func, callback));
        }

        private IEnumerator CoWait(System.Func<bool> func, System.Action callback = null)
        {
            yield return new WaitUntil(func);
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
    }
}
