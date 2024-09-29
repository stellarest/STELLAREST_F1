using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BuffDamageReduction : BuffBase
    {
        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EffectBuffType = EEffectBuffType.DamageReduction;
        }

        protected override void OnRemoveSelfByCondition(Action endCallback = null)
        {
            endCallback?.Invoke();
        }

        public override void EnterEffect()
        {
            base.EnterEffect();
        }
        public override void ExitEffect()
        {
            base.ExitEffect();
        }
    }
}
