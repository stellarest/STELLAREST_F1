using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace STELLAREST_F1
{
    public class ActiveSkillBase : SkillBase
    {
        public override void InitialSetInfo(int dataID, BaseCellObject owner)
            => base.InitialSetInfo(dataID, owner);

        public override bool OnSkillEnter()
            => base.OnSkillEnter();

        public override bool OnSkillCallback()
            => base.OnSkillCallback();

        public override void OnSkillExit()
            => base.OnSkillExit();
    }
}
