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
                    _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Default);
                    break;

                case ECreatureState.Move:
                    _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Default);
                    break;

                case ECreatureState.Skill_Attack:
                    {
                        // --- 이미 쿨타임은 돌아가고 있는 상태. 타겟은 없더라도 스킬은 실행되었기 때문에 쿨타임만 돌림.
                        if (_monsterOwner.Target.IsValid() == false)
                            return;

                        _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Angry);
                    }
                    break;

                case ECreatureState.Dead:
                    _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Dead);
                    break;
            }

            PlayCreatureAnimation(_monsterOwner.CreatureState);
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
