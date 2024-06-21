using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class EnvAnimation : BaseAnimation
    {
        private Env _owner = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfo(int dataID, BaseObject owner)
        {
            base.SetInfo(dataID, owner);
            string animatorTextID = Managers.Data.EnvDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
                Animator.runtimeAnimatorController = animController;

            _owner = owner as Env;
        }

        public override void UpdateAnimation()
        {
            if (_owner == null)
                return;

            switch (_owner.EnvState)
            {
                case EEnvState.Idle:
                    Idle();
                    break;

                case EEnvState.OnDamaged:
                    break;

                case EEnvState.Dead:
                    // Dead();
                    break;
            }
        }
    }
}
