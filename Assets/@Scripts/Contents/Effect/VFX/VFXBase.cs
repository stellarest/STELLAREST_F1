using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // --- Simple VFX
    public class VFXBase : EffectBase
    {
        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EffectType = EEffectType.SimpleVFX;
        }

        public override void ApplyEffect()
        {
            EnterShowEffect();
            StartCoroutine(CoStartTimer());
        }

        public override void EnterShowEffect() { }
        public override void OnShowEffect() { }
        public override void ExitShowEffect() { }
    }
}

