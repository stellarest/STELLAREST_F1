using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    // 애초에 스킬 클래스도...
    // - SkillBase
    // - DefaultSkill, ActiveSkill, 
    public class Shield : ActiveSkill
    {
        #region Events
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            LockCoolTimeSkill = true;
            return true;
        }

        public override bool OnSkillEnter()
        {
            if (base.OnSkillEnter() == false)
                return false;

            return true;
        }

        public override bool OnSkillCallback()
        {
            if (base.OnSkillCallback() == false)
                return false;

            return true;
        }

        public override void OnSkillExit()
            => base.OnSkillExit();
        #endregion
    }
}
