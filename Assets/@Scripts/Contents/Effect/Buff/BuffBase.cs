using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BuffBase : EffectBase
    {
        public EEffectBuffType EffectBuffType { get; protected set; } = EEffectBuffType.None;

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EffectType = EEffectType.Buff;
        }

        public override void ApplyEffect()
        {
            Owner.ApplyStat();
            base.ApplyEffect();
        }

        public override void EnterEffect()
            => Owner.BaseEffect.SetIsOnEffectBuff(EffectBuffType, true);
        public override void ExitEffect()
            => Owner.BaseEffect.SetIsOnEffectBuff(EffectBuffType, false);

        public override void DoEffect() { }
    }
}
