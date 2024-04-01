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

        // Hero LookAtDir Default Sprite : Right
        public override void Flip(ELookAtDirection lookAtDir)
        {
            Vector3 localScale = _owner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ?
                                             -1 : (localScale.x < 0) ? -1 : 1;
            localScale.x = localScale.x * sign;
            _owner.transform.localScale = localScale;
        }
    }
}

