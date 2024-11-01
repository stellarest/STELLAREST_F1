using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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
            //Owner.ApplyBuffStat(EffectType);
            Owner.ApplyStat(effectID: DataTemplateID, effectStatType: EffectType, addStat: true);
        }

        public override void OnShowEffect()
            => base.OnShowEffect();

        public override void ExitEffect()
        {
            if (KeepEffectOnExit == false)
                Owner.ApplyStat(effectID: DataTemplateID, effectStatType: EffectType, addStat: false);

            base.ExitEffect();
        }
    }
}
