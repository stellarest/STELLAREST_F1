using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class MonsterAnimation : CreatureAnimation
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public void SetInfo(int dataID, Monster owner)
        {
            base.SetInfo(dataID, owner);
        }
    }
}
