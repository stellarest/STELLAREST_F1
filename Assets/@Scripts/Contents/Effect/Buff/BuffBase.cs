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
            base.ApplyEffect();
            Owner.ApplyStat();
        }

        public override void EnterShowEffect() { }
        public override void OnShowEffect() { }
        public override void ExitShowEffect() { }
    }
}

