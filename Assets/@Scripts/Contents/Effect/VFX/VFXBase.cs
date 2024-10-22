using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class VFXBase : EffectBase
    {
        protected override void InitialSetInfo(int dataID)
            => base.InitialSetInfo(dataID);

        protected override void EnterInGame(Vector3 spawnPos)
            => base.EnterInGame(spawnPos);

        public override void ApplyEffect()
            => base.ApplyEffect();

        public override void OnShowEffect()
            => base.OnShowEffect();

        public override void ExitEffect()
            => base.ExitEffect();
    }
}

