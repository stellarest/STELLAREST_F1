using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BuffBase : EffectBase
    {
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
    }
}

