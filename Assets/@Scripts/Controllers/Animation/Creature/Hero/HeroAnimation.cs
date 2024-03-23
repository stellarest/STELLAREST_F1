using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // Creature Animation 공용으로 사용 가능할수도..
    // 일단 이 부분은 몬스터, 일반 오브젝트쪽 보고 결정해야할듯.
    public class HeroAnimation : CreatureAnimation
    {
        private Hero _hero = null;
        public override BaseObject Owner
        {
            get => _hero;
            protected set
            {
                if (_hero == null)
                    _hero = value as Hero;
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfoFromOwner(int dataID, BaseObject owner)
        {
            base.SetInfoFromOwner(dataID, owner);
            Owner = owner as Hero;
            string animatorTextID = Managers.Data.HeroAnimationDataDict[dataID].AnimatorTextID;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (animController != null)
            {
                RuntimeAnimatorController cloned = UnityEngine.Object.Instantiate(animController);
                this.Animator.runtimeAnimatorController = cloned;
            }
        }

        public override void UpdateAnimation()
        {
            if (_hero == null)
                return;

            switch (_hero.CreatureState)
            {
                case ECreatureState.Idle:
                    Idle();
                    break;

                case ECreatureState.Move:
                    Move();
                    break;
            }
        }
    }
}

