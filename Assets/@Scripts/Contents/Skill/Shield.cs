using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class Shield : SkillBase
    {
        public override void OnSkillStateEnter() { }
        public override void OnSkillStateExit() { }
        public override void OnSkillCallback() { }
        protected override void DoSelfTarget(BaseObject target) { }
        protected override void DoSingleTarget(BaseObject target) { }
        protected override void DoHalfTarget(BaseObject target) { }
        protected override void DoAroundTarget(BaseObject target) { }
    }
}
