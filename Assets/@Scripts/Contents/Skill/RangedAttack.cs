using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class RangedAttack : SkillBase
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void OnSkillStateEnd()
        {
        }

        public override void OnSkillStateEnter()
        {
        }

        public override void OnSkillStateUpdate()
        {
        }
    }
}

