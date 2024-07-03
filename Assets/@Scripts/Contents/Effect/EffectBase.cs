using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;

namespace STELLAREST_F1
{
    // 이펙트는 기본적으로 스킬에 붙어있다. 어떠한 스킬이 묻었을 때 일어나는 효과.
    // Ex. TickTime: 1, TickCount: 5 -> 1초 마다 5번 실행하겠다. 근데 난 그냥 Duration, Period로
    // Monster와 다르게 Effect 여러가지 상속 구조로 가기 위해 베이스 클래스가 이렇게 구성되어 있음. Effect의 핵심은 "상속"
    public class EffectBase : BaseObject
    {
        public Creature Owner { get; protected set; } = null;
        public EffectData EffectData { get; private set; } = null;
        public EEffectType EffectType { get; private set; } = EEffectType.None;

        [field: SerializeField] public float Remains { get; protected set; } = 0f; // 지속시간이 얼마나 남았는지
        public EEffectSpawnType EffectSpawnType { get; protected set; } = EEffectSpawnType.None;
        public bool IsLoop { get; protected set; } = false;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Effect;
            SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_Effect;
            return true;
        }

        public virtual void SetInfo(int effectDataID, Creature owner, EEffectSpawnType effectSpawnType)
        {
            Owner = owner;
            EffectData = Managers.Data.EffectDataDict[effectDataID];
            DataTemplateID = effectDataID;

            EffectType = EffectData.EffectType;
            EffectSpawnType = effectSpawnType;
            IsLoop = true;

            // ... DO SOMETHING ... Add Events 등 //

            if (effectSpawnType == EEffectSpawnType.External)
                Remains = float.MaxValue;
            else
                Remains = EffectData.Duration;
        }

        public virtual void SetInfo(EffectData effectData, Creature owner, EEffectSpawnType effectSpawnType)
        {
            Owner = owner;
            EffectData = effectData;
            DataTemplateID = effectData.DataID;
            EffectSpawnType = effectSpawnType;
            IsLoop = true;

            // ... DO SOMETHING ... //

            if (effectSpawnType == EEffectSpawnType.External)
                Remains = float.MaxValue;
            else
                Remains = effectData.Duration;
        }

        public virtual void ApplyEffect()
        {
            ShowEffect();
            StartCoroutine(CoStartTimer());
            // 여기에다가 도트 뎀, 도트 힐, 패시브 영구적, 힘 버프, 체력 버프, 민첩 버프 등등을 적용시킨다.
            // 힘 버프(Buff, TypeID:1), 체력 버프(Buff, TypeID:2) 이런식..
        }

        protected void ShowEffect()
        {
            // ... SHOW VFX ... // 
            // (ex) PlaySeletonAnimation(EffectState.Idle, ..., Loop)
        }

        protected virtual void ProcessDot() { }

        private IEnumerator CoStartTimer()
        {
            if (EffectType == EEffectType.Airborne || EffectType == EEffectType.Knockback)
                yield break;

            float tickTimer = 0f;
            ProcessDot();
            if (EffectType == EEffectType.Instant)
                yield return new WaitForSeconds(1f);
            else
            {
                while (Remains > 0f)
                {
                    Remains -= Time.deltaTime;
                    tickTimer += Time.deltaTime;

                    if (tickTimer >= EffectData.Period)
                    {
                        ProcessDot();
                        tickTimer -= EffectData.Period;
                    }

                    yield return null;
                }
            }
            Remains = 0f;
            ClearEffect(EEffectClearType.TimeOut);
        }

        // StatModifier 부분은 생략
        public virtual bool ClearEffect(EEffectClearType clearType)
        {
            if (Owner.IsValid() == false)
                return false;

            switch (clearType)
            {
                case EEffectClearType.TimeOut:
                case EEffectClearType.TriggerOutAoE:
                case EEffectClearType.EndOfCC:
                    {
                        Owner.CreatureEffect.RemoveEffect(this);
                        return true;
                    }

                case EEffectClearType.ClearSkill:
                {
                    // Aoe 범위 안에 있는 경우 해제x
                    if (EffectSpawnType != EEffectSpawnType.External)
                    {
                        Managers.Object.Despawn(this, DataTemplateID);
                        return true;
                    }
                    break;
                }
            }

            return false;
        }

        public bool IsCroudControl()
        {
            switch (EffectType)
            {
                case EEffectType.Airborne:
                case EEffectType.Knockback:
                case EEffectType.Freeze:
                case EEffectType.Stun:
                    return true;
            }

            return false;
        }
    }
}
