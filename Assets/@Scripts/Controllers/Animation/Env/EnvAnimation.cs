using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class EnvAnimation : BaseAnimation
    {
        private Env _envOwner = null;
        public override BaseObject Owner => _envOwner;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void InitialSetInfo(int dataID, BaseObject owner)
        {
            base.InitialSetInfo(dataID, owner);
            string animatorTextID = Managers.Data.EnvDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
                Animator.runtimeAnimatorController = animController;

            _envOwner = owner as Env;
            Debug.Log("EnvAnim::InitialSetInfo");
        }
    }
}
