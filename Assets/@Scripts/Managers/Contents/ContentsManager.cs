using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class ContentsManager
    {
        public void ActivateEffects(EffectComponent effectComp, Vector3 enteredStartPos, Vector3 enteredDir, int enteredSignX)
        {
            if (effectComp.Owner.IsValid() == false)
                return;

            List<EffectBase> effects = effectComp.ActiveEffects;
            for (int i = 0; i < effects.Count; ++i)
            {
                EffectBase effect = effects[i];
                if (effect.gameObject.activeSelf == false)
                    effect.ActivateSelf(enteredStartPos, enteredDir, enteredSignX);
            }
        }
    }
}
