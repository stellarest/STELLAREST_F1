using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class DebuffBase : EffectBase
    {
        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
        }

        public override void ApplyEffect()
        {
            if (Owner.IsValid() == false)
                return;

            if (Owner.Target.IsValid() == false)
                return;

            base.ApplyEffect();
            Owner.ApplyStatToTarget(effectID: DataTemplateID, effectType: EffectType, target: Owner.Target, addStat: false);
        }

        public override void OnShowEffect()
            => base.OnShowEffect();

        public override void ExitEffect()
        {
            if (KeepEffectOnExit == false)
                Owner.ApplyStatToTarget(effectID: DataTemplateID, effectType: EffectType, target: Owner.Target, addStat: true);

            base.ExitEffect();
        }
    }
}
