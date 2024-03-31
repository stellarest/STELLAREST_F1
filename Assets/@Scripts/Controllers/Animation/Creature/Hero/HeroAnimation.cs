using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroAnimation : CreatureAnimation
    {
        private Hero _owner = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _originScaleX = transform.localScale.x;
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

            _owner = owner as Hero;
        }

        public override void UpdateAnimation()
        {
            if (_owner == null)
                return;

            switch (_owner.CreatureState)
            {
                case ECreatureState.Idle:
                    {
                        //GetOwner<Hero>().HeroBody.SetEmoji(EHeroEmoji.Default);
                        _owner.HeroBody.SetEmoji(EHeroEmoji.Default);
                        Idle();
                    }
                    break;

                case ECreatureState.Move:
                    {
                        //GetOwner<Hero>().HeroBody.SetEmoji(EHeroEmoji.Default);
                        Move();
                    }
                    break;

                case ECreatureState.Attack:
                    {
                        //GetOwner<Hero>().HeroBody.SetEmoji(EHeroEmoji.Combat);
                        _owner.HeroBody.SetEmoji(EHeroEmoji.Combat);
                        Attack(); // ***** TEMP *****
                    }
                    break;

                case ECreatureState.Dead:
                    {
                        _owner.HeroBody.SetEmoji(EHeroEmoji.Dead);
                        Dead();
                    }
                    break;
            }
        }
    }
}

