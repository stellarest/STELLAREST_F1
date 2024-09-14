using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    // --- TEST COMP
    public class NewSkillComp : SkillBase
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Debug.Log($"{nameof(NewSkillComp)}, {nameof(Init)}");

            return true;
        }
    }
}

