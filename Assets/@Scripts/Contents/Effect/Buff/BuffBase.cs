using System;
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
        }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
            Owner.ApplyBuffStat();
        }

        public override void OnShowEffect()
            => base.OnShowEffect();

        public override void ExitEffect()
            => base.ExitEffect();

        protected override void OnRemoveSelfByCondition(Action endCallback = null)
        {
            endCallback?.Invoke();
        }
    }
}
