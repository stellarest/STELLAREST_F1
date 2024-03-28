using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MonsterAnimation : CreatureAnimation
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public void SetInfo(int dataID, Monster owner)
        {
            base.SetInfo(dataID, owner);
            string animatorTextID = Managers.Data.MonsterDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
            {
                RuntimeAnimatorController cloned = UnityEngine.Object.Instantiate(animController);
                this.Animator.runtimeAnimatorController = cloned;
            }
        }

        public override void UpdateAnimation()
        {
            if (GetOwner<Monster>() == null)
                return;

            switch (GetOwner<Monster>().CreatureState)
            {
                case ECreatureState.Idle:
                    {
                        GetOwner<Monster>().MonsterBody.SetEmoji(EMonsterEmoji.Default);
                        Idle();
                    }
                    break;

                case ECreatureState.Move:
                    {
                        GetOwner<Monster>().MonsterBody.SetEmoji(EMonsterEmoji.Default);
                        Move();
                    }
                    break;

                case ECreatureState.Skill:
                    {
                        GetOwner<Monster>().MonsterBody.SetEmoji(EMonsterEmoji.Angry);
                        Move();
                    }
                    break;

                case ECreatureState.Dead:
                    {
                        GetOwner<Monster>().MonsterBody.SetEmoji(EMonsterEmoji.Dead);
                        Dead();
                    }
                    break;
            }
        }
    }
}
