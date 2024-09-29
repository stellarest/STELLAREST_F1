using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CCBase : EffectBase
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
            // Set CreatureAIStatr, Emoji, and etc...
            switch (EffectType)
            {
                case EEffectType.None:
                    break;
            }
        }

        public override void EnterEffect() { }
        public override void ExitEffect() { }

        public override void DoEffect() { }
    }
}

