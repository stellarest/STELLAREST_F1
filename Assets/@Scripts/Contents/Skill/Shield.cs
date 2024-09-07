using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class Shield : SkillBase
    {
        #region Events
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            LockCoolTimeUntilDisable = true;
            return true;
        }

        public override bool OnSkillStateEnter()
        {
            if (base.OnSkillStateEnter() == false)
                return false;

            return true;
        }

        public override bool OnSkillCallback()
        {
            if (base.OnSkillCallback() == false)
                return false;

            return true;
        }

        public override void OnSkillStateExit()
            => base.OnSkillStateExit();
        #endregion
    }
}
