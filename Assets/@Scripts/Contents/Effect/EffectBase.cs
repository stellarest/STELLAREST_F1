using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;

namespace STELLAREST_F1
{
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

            // ... Effect Sorting 해주고 ... //

            EffectType = Util.GetEnumFromString<EEffectType>(effectData.Type);

            if (effectSpawnType == EEffectSpawnType.External)
                Remains = float.MaxValue;
            else
                Remains = effectData.Duration * effectData.Period; // 이해 안됨...

            // Duration = effectData.TickTime * effectData.TickCount;
            // Period = effectData.TickTime;
        }

        public virtual void ApplyEffect()
        {
            ShowEffect();
        }

        protected void ShowEffect()
        {
            
        }

        public virtual bool ClearEffect(EEffectClearType clearType)
        {
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
    }
}
