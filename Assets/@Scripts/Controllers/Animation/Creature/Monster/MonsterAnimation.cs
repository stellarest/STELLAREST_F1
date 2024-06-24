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
        // --- Enter
        public override void OnUpperIdleEnter()
        {
            if (Owner.Target.IsValid() == false)
                Owner.MonsterBody.MonsterEmoji = EMonsterEmoji.Default;
            else
                Owner.MonsterBody.MonsterEmoji = EMonsterEmoji.Angry;
            
            base.OnUpperIdleEnter();
        }
        public override void OnUpperIdleToSkillAEnter()
        {
            Owner.MonsterBody.MonsterEmoji = EMonsterEmoji.Angry;
            base.OnUpperIdleToSkillAEnter();
        }
        public override void OnUpperIdleToSkillBEnter()
        {
            base.OnUpperIdleToSkillBEnter();
        }
        public override void OnUpperIdleToSkillCEnter()
        {
            base.OnUpperIdleToSkillCEnter();
        }
        public override void OnUpperIdleToCollectEnvEnter()
        {
            base.OnUpperIdleToCollectEnvEnter();
        }

        public override void OnUpperMoveEnter()
        {
            Owner.MonsterBody.MonsterEmoji = EMonsterEmoji.Default;
            base.OnUpperMoveEnter();
            if (Owner.IsValid() == false)
                return;

            //Debug.Log($"<color=cyan>{nameof(OnUpperMoveEnter)}</color>");
            //Owner.MonsterBody.MonsterEmoji = EMonsterEmoji.Angry;
            Owner.CreatureAIState = ECreatureAIState.Move;
            Owner.StartCoLerpToCellPos();
        }
        public override void OnUpperMoveToSkillAEnter()
        {
            base.OnUpperMoveToSkillAEnter();
        }
        public override void OnUpperMoveToSkillBEnter()
        {
            base.OnUpperMoveToSkillBEnter();
        }
        public override void OnUpperMoveToSkillCEnter()
        {
            base.OnUpperMoveToSkillCEnter();
        }

        public override void OnUpperDeadEnter()
        {
            Owner.MonsterBody.MonsterEmoji = EMonsterEmoji.Dead;
            base.OnUpperDeadEnter();
        }

        // --- Update
        public override void OnUpperIdleUpdate()
        {
            base.OnUpperIdleUpdate();
        }
        public override void OnUpperIdleToSkillAUpdate()
        {
            base.OnUpperIdleToSkillAUpdate();
        }
        public override void OnUpperIdleToSkillBUpdate()
        {
            base.OnUpperIdleToSkillBUpdate();
        }
        public override void OnUpperIdleToSkillCUpdate()
        {
            base.OnUpperIdleToSkillCUpdate();
        }
        public override void OnUpperIdleToCollectEnvUpdate()
        {
            base.OnUpperIdleToCollectEnvUpdate();
        }

        public override void OnUpperMoveUpdate()
        {
            base.OnUpperMoveUpdate();
        }
        public override void OnUpperMoveToSkillAUpdate()
        {
            base.OnUpperMoveToSkillAUpdate();
        }
        public override void OnUpperMoveToSkillBUpdate()
        {
            base.OnUpperMoveToSkillBUpdate();
        }
        public override void OnUpperMoveToSkillCUpdate()
        {
            base.OnUpperMoveToSkillCUpdate();
        }

        public override void OnUpperDeadUpdate()
        {
            base.OnUpperDeadUpdate();
        }

        // --- Exit
        public override void OnUpperIdleExit()
        {
            base.OnUpperIdleExit();
        }
        public override void OnUpperIdleToSkillAExit()
        {
            base.OnUpperIdleToSkillAExit();
        }
        public override void OnUpperIdleToSkillBExit()
        {
            base.OnUpperIdleToSkillBExit();
        }
        public override void OnUpperIdleToSkillCExit()
        {
            base.OnUpperIdleToSkillCExit();
        }
        public override void OnUpperIdleToCollectEnvExit()
        {
            base.OnUpperIdleToCollectEnvExit();
        }

        public override void OnUpperMoveExit()
        {
            base.OnUpperMoveExit();
        }
        public override void OnUpperMoveToSkillAExit()
        {
            base.OnUpperMoveToSkillAExit();
        }
        public override void OnUpperMoveToSkillBExit()
        {
            base.OnUpperMoveToSkillBExit();
        }
        public override void OnUpperMoveToSkillCExit()
        {
            base.OnUpperMoveToSkillCExit();
        }

        public override void OnUpperDeadExit()
        {
            base.OnUpperDeadExit();
        }
        #endregion
    }
}
