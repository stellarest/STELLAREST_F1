using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroAnimation : CreatureAnimation
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfo(int dataID, BaseObject owner)
        {
            base.SetInfo(dataID, owner);
            string animatorTextID = Managers.Data.HeroDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
            {
                RuntimeAnimatorController cloned = UnityEngine.Object.Instantiate(animController);
                this.Animator.runtimeAnimatorController = cloned;
            }
        }

        public override void UpdateAnimation()
        {
            if (GetOwner<Hero>() == null)
                return;

            switch (GetOwner<Hero>().CreatureState)
            {
                case ECreatureState.Idle:
                    {
                        GetOwner<Hero>().HeroBody.SetEmoji(EHeroEmoji.Default);
                        Idle();
                    }
                    break;

                case ECreatureState.Move:
                    {
                        GetOwner<Hero>().HeroBody.SetEmoji(EHeroEmoji.Default);
                        Move();
                    }
                    break;

                case ECreatureState.Dead:
                    {
                        GetOwner<Hero>().HeroBody.SetEmoji(EHeroEmoji.Dead);
                        Dead();
                    }
                    break;
            }
        }
    }
}

