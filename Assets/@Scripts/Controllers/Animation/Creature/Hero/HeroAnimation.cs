using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    /*
        Hero Idle : 60 frame
        hero Move : 25 frame
        // 공속 나중에 따로 조정
        Paladin Attack : 30 frame
        Archer Attack : 50 frame
        Wizard Attack : 60 frame
    */
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
                this.Animator.runtimeAnimatorController = animController;

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
                        _owner.HeroBody.SetEmoji(EHeroEmoji.Idle);
                        Idle();
                    }
                    break;

                case ECreatureState.Move:
                    {
                        _owner.HeroBody.SetEmoji(EHeroEmoji.Move);
                        Move();
                    }
                    break;

                case ECreatureState.Skill_Attack:
                    {
                        _owner.HeroBody.SetEmoji(EHeroEmoji.Combat);
                        Skill_Attack();
                    }
                    break;

                case ECreatureState.CollectEnv:
                    {
                        _owner.HeroBody.SetEmoji(EHeroEmoji.Idle);
                        CollectEnv();
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

        public override void Flip(ELookAtDirection lookAtDir)
        {
            // Hero LookAtDir Default Sprite : Right
            Vector3 localScale = _owner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ?
                                             -1 : (localScale.x < 0) ? -1 : 1;
            localScale.x = localScale.x * sign;
            _owner.transform.localScale = localScale;
        }
    }
}

