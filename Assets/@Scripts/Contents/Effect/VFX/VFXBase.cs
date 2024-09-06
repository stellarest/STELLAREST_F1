using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class VFXBase : EffectBase
    {
        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EffectType = EEffectType.SimpleVFX;
        }
    }
}

