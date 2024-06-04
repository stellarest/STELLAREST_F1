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
        private Hero _heroOwner = null;

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

            _heroOwner = owner as Hero;
        }

        public override void UpdateAnimation()
        {
            if (_heroOwner == null)
                return;

            switch (_heroOwner.CreatureState)
            {
                case ECreatureState.Idle:
                    _heroOwner.HeroBody.SetEmoji(EHeroEmoji.Idle);
                    break;

                case ECreatureState.Move:
                    _heroOwner.HeroBody.SetEmoji(EHeroEmoji.Move);
                    break;

                case ECreatureState.Skill_Attack:
                    _heroOwner.HeroBody.SetEmoji(EHeroEmoji.Skill_Attack);
                    break;

                case ECreatureState.Skill_A:
                    _heroOwner.HeroBody.SetEmoji(EHeroEmoji.Skill_Attack);
                    break;

                case ECreatureState.Skill_B:
                    _heroOwner.HeroBody.SetEmoji(EHeroEmoji.Skill_Attack);
                    break;

                case ECreatureState.CollectEnv:
                    if (_heroOwner.DataTemplateID == ReadOnly.Numeric.DataID_Hero_Wizard)
                        _heroOwner.HeroBody.SetEmoji(EHeroEmoji.Move);
                    else
                        _heroOwner.HeroBody.SetEmoji(EHeroEmoji.Idle);
                    break;

                case ECreatureState.Dead:
                    _heroOwner.HeroBody.SetEmoji(EHeroEmoji.Dead);
                    break;
            }

            PlayCreatureAnimation(_heroOwner.CreatureState);
        }

        public override void Flip(ELookAtDirection lookAtDir)
        {
            // Hero LookAtDir Default Sprite : Right
            Vector3 localScale = _heroOwner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ?
                                             -1 : (localScale.x < 0) ? -1 : 1;
            localScale.x = localScale.x * sign;
            _heroOwner.transform.localScale = localScale;
        }
    }
}

/*
    [PREV CODE]
// public override void UpdateAnimation(ECreatureState creatureState)
//         {
//             if (_owner == null)
//                 return;

//             switch (_owner.CreatureState)
//             {
//                 case ECreatureState.Idle:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Idle);
//                         Idle();
//                     }
//                     break;

//                 case ECreatureState.Move:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Move);
//                         Move();
//                     }
//                     break;

//                 case ECreatureState.Skill_Attack:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Skill_Attack);
//                         Skill_Attack();
//                     }
//                     break;

//                 case ECreatureState.Skill_A:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Skill_Attack);
//                         Skill_A();
//                     }
//                     break;

//                 case ECreatureState.Skill_B:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Skill_Attack);
//                         Skill_B();
//                     }
//                     break;

//                 case ECreatureState.CollectEnv:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Idle);
//                         CollectEnv();
//                     }
//                     break;

//                 case ECreatureState.Dead:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Dead);
//                         Dead();
//                     }
//                     break;
//             }
//         }
*/