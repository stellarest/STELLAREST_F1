using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;

namespace STELLAREST_F1
{
    // 이펙트는 기본적으로 스킬에 붙어있다. 어떠한 스킬이 묻었을 때 일어나는 효과.
    // Ex. TickTime: 1, TickCount: 5 -> 1초 마다 5번 실행하겠다. 근데 난 그냥 Duration, Period로
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

            return true;
        }

        public virtual void SetInfo(EffectData effectData, Creature owner, EEffectSpawnType effectSpawnType)
        {
            Owner = owner;
            EffectData = effectData;
            DataTemplateID = effectData.DataID;
            EffectSpawnType = effectSpawnType;
            IsLoop = true;

            // ... Effect Sorting 해주고 ... Add Events 등등.. //
            // EffectType = Util.GetEnumFromString<EEffectType>(effectData.Type);

            if (effectSpawnType == EEffectSpawnType.External)
                Remains = float.MaxValue;
            else
                Remains = effectData.Duration;
        }

        public virtual void ApplyEffect()
        {
            ShowEffect();
        }

        protected void ShowEffect()
        {
            // Effect 애니메이션을 보여주는 부분. VFX 매니저로해야할듯.
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
                        // Managers.Object.Despawn(this, -1);
                        Owner.CreatureEffect.RemoveEffect(this);
                        return true;
                    }

                case EEffectClearType.ClearSkill:
                {
                    // Aoe 범위 안에 있는 경우 해제x
                    if (EffectSpawnType != EEffectSpawnType.External)
                    {
                        Managers.Object.Despawn(this, -1);
                        return true;
                    }
                    break;
                }
            }

            return false;
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
