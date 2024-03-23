using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Monster : Creature
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            
            CreatureType = ECreatureType.Monster;
            Speed = 3.0f;

            return true;
        }
    }
}
