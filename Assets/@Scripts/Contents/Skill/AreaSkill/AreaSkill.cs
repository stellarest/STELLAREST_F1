using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class AreaSkill : SkillBase
    {
        public SpellIndicator Indicator { get; protected set; } = null;

        #region Events
        public override bool OnSkillStateEnter()
            => base.OnSkillStateEnter();
        // public override void OnSkillCallback() { }
        public override void OnSkillStateExit() { }
        #endregion
    }
}
