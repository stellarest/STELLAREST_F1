using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MonsterAnimation : CreatureAnimation
    {
        private Monster _monsterOwner = null;
        public new Monster Owner => _monsterOwner;

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

        // public override void UpdateAnimation()
        // {
        //     if (_monsterOwner == null)
        //         return;

        //     switch (_monsterOwner.CreatureAIState)
        //     {
        //         case ECreatureAIState.Idle:
        //             _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Default);
        //             break;

        //         case ECreatureAIState.Move:
        //             _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Default);
        //             break;

        //         case ECreatureAIState.Skill_Attack:
        //             {
        //                 // --- 이미 쿨타임은 돌아가고 있는 상태. 타겟은 없더라도 스킬은 실행되었기 때문에 쿨타임만 돌림.
        //                 if (_monsterOwner.Target.IsValid() == false)
        //                     return;

        //                 _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Angry);
        //             }
        //             break;

        //         case ECreatureAIState.Dead:
        //             _monsterOwner.MonsterBody.SetEmoji(EMonsterEmoji.Dead);
        //             break;
        //     }

        //     PlayCreatureAnimation(_monsterOwner.CreatureAIState);
        // }

        public override void Flip(ELookAtDirection lookAtDir)
        {
            // Monster LookAtDir Default Sprite : Left
            Vector3 localScale = _monsterOwner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ? (localScale.x > 0 ? 1 : -1) : -1;
            localScale.x = localScale.x * sign;
            _monsterOwner.transform.localScale = localScale;
        }

        #region Anim State Events
        public override void OnUpperIdleToSkillAttackEnter()
        {
            // Debug.Log($"<color=yellow>{nameof(OnUpperIdleToSkillAttackEnter)}</color>");
            // Owner.UpdateCellPos();
            Debug.Log("<color=white>Pass to Skill Method..</color>");
            Owner.CreatureSkill.SkillArray[(int)ESkillType.Skill_Attack].OnSkillStateEnter();
        }

        public override void OnUpperMoveEnter()
        {
            if (Owner.IsValid() == false)
                return;

            Debug.Log($"<color=cyan>{nameof(OnUpperMoveEnter)}</color>");
            Owner.StartCoLerpToCellPos();
        }
        #endregion
    }
}
