using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class Shield : SkillBase
    {
        #region Events
        public override bool OnSkillStateEnter()
        {
            if (base.OnSkillStateEnter() == false)
                return false;

            Owner.Moving = false;
            return true;
        }

        public override bool OnSkillCallback()
        {
            return base.OnSkillCallback();
        }

        public override void OnSkillStateExit()
            => base.OnSkillStateExit();
        #endregion
    }
}
