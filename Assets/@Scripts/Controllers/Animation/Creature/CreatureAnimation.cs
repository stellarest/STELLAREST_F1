using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAnimation : BaseAnimation
    {
        private Creature _creatureOwner = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfo(int dataID, BaseObject owner)
        {
            base.SetInfo(dataID, owner);
            _creatureOwner = owner as Creature;
        }

        public override void UpdateAnimation() 
            => base.UpdateAnimation();

        public virtual void PlayCreatureAnimation(ECreatureState creatureState)
        {
            switch (creatureState)
            {
                case ECreatureState.Idle:
                    Animator.Play(Play_Idle);
                    break;

                case ECreatureState.Move:
                    Animator.Play(Play_Move);
                    break;

                case ECreatureState.Skill_Attack:
                    Animator.Play(Play_Skill_Attack);
                    break;

                case ECreatureState.CollectEnv:
                    Animator.Play(Play_CollectEnv);
                    break;

                case ECreatureState.Dead:
                    Animator.Play(Play_Dead);
                    break;
            }
        }

        public void AnimationEnd(ECreatureState endAnimationState)
        {
            switch (endAnimationState)
            {
                case ECreatureState.Skill_Attack:
                    {
                        _creatureOwner.CreatureMoveState = ECreatureMoveState.MoveToTarget;
                        _creatureOwner.CreatureState = ECreatureState.Idle;
                    }
                    break;
            }
        }
    }
}
