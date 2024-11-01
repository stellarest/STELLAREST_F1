using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class DeBuffBase : EffectBase // --- TEMP
    {
        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
        }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
            Owner.ApplyStat(effectID: DataTemplateID, effectStatType: EffectType, addStat: false);
        }

        public override void OnShowEffect()
            => base.OnShowEffect();

        public override void ExitEffect()
            => base.ExitEffect();
    }
}
