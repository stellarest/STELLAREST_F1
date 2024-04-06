using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAnimation : BaseAnimation
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }
    }
}
