using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MonsterAnimation : CreatureAnimation
    {
        private Monster _monsterOwner = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfo(int dataID, BaseObject owner)
        {
            base.SetInfo(dataID, owner);
            string animatorTextID = Managers.Data.MonsterDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
            {
                // Ref Origin
                Animator.runtimeAnimatorController = animController;

                // Cloned
                // RuntimeAnimatorController cloned = UnityEngine.Object.Instantiate(animController);
                // Animator.runtimeAnimatorController = cloned;
            }

            _monsterOwner = owner as Monster;
        }

        public override void UpdateAnimation()
        {
            if (_monsterOwner == null)
                return;

            switch (_monsterOwner.CreatureState)
            {
                case ECreatureState.Idle:
                    {
                        _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Default);
                        Idle();
                    }
                    break;

                case ECreatureState.Move:
                    {
                        _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Default);
                        Move();
                    }
                    break;

                case ECreatureState.Skill_Attack:
                    {
                        _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Angry);
                        Skill_Attack();
                    }
                    break;

                case ECreatureState.Dead:
                    {
                        _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Dead);
                        Dead();
                    }
                    break;
            }
        }

        public override void Flip(ELookAtDirection lookAtDir)
        {
            // Monster LookAtDir Default Sprite : Left
            Vector3 localScale = _monsterOwner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ? (localScale.x > 0 ? 1 : -1) : -1;
            localScale.x = localScale.x * sign;
            _monsterOwner.transform.localScale = localScale;
        }
    }
}
