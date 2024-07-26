using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MonsterAnimation : CreatureAnimation
    {
        private Monster _monsterOwner = null;
        public override BaseObject Owner => _monsterOwner;

        public override void InitialSetInfo(int dataID, BaseObject owner)
        {
            base.InitialSetInfo(dataID, owner);
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

        public override void Flip(ELookAtDirection lookAtDir)
        {
            // Monster LookAtDir Default Sprite : Left
            Vector3 localScale = _monsterOwner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ? (localScale.x > 0 ? 1 : -1) : -1;
            localScale.x = localScale.x * sign;
            _monsterOwner.transform.localScale = localScale;
        }

        // #region Anim State Events
        // // --- Enter
        // public override void OnUpperIdleEnter()
        // {
        //     if (_monsterOwner.Target.IsValid() == false)
        //         _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;
        //     else
        //         _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Angry;
            
        //     base.OnUpperIdleEnter();
        // }
        // public override void OnUpperIdleToSkillAEnter()
        // {
        //     _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Angry;
        //     base.OnUpperIdleToSkillAEnter();
        // }
        // public override void OnUpperIdleToSkillBEnter()
        // {
        //     base.OnUpperIdleToSkillBEnter();
        // }
        // public override void OnUpperIdleToSkillCEnter()
        // {
        //     base.OnUpperIdleToSkillCEnter();
        // }
        // public override void OnUpperIdleToCollectEnvEnter()
        // {
        //     base.OnUpperIdleToCollectEnvEnter();
        // }

        // public override void OnUpperMoveEnter()
        // {
        //     if (_monsterOwner.IsValid() == false)
        //         return;

        //     _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;
        //     _monsterOwner.CreatureAIState = ECreatureAIState.Move;
        //     _monsterOwner.StartCoLerpToCellPos();
        // }
        // public override void OnUpperMoveToSkillAEnter()
        // {
        //     base.OnUpperMoveToSkillAEnter();
        // }
        // public override void OnUpperMoveToSkillBEnter()
        // {
        //     base.OnUpperMoveToSkillBEnter();
        // }
        // public override void OnUpperMoveToSkillCEnter()
        // {
        //     base.OnUpperMoveToSkillCEnter();
        // }

        // public override void OnUpperDeadEnter()
        // {
        //     _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Dead;
        //     base.OnUpperDeadEnter();
        // }

        // // --- Update
        // public override void OnUpperIdleUpdate()
        // {
        //     base.OnUpperIdleUpdate();
        // }
        // public override void OnUpperIdleToSkillAUpdate()
        // {
        //     base.OnUpperIdleToSkillAUpdate();
        // }
        // public override void OnUpperIdleToSkillBUpdate()
        // {
        //     base.OnUpperIdleToSkillBUpdate();
        // }
        // public override void OnUpperIdleToSkillCUpdate()
        // {
        //     base.OnUpperIdleToSkillCUpdate();
        // }
        // public override void OnUpperIdleToCollectEnvUpdate()
        // {
        //     base.OnUpperIdleToCollectEnvUpdate();
        // }

        // public override void OnUpperMoveUpdate()
        // {
        //     base.OnUpperMoveUpdate();
        // }
        // public override void OnUpperMoveToSkillAUpdate()
        // {
        //     base.OnUpperMoveToSkillAUpdate();
        // }
        // public override void OnUpperMoveToSkillBUpdate()
        // {
        //     base.OnUpperMoveToSkillBUpdate();
        // }
        // public override void OnUpperMoveToSkillCUpdate()
        // {
        //     base.OnUpperMoveToSkillCUpdate();
        // }

        // public override void OnUpperDeadUpdate()
        // {
        //     base.OnUpperDeadUpdate();
        // }

        // // --- Exit
        // public override void OnUpperIdleExit()
        // {
        //     base.OnUpperIdleExit();
        // }
        // public override void OnUpperIdleToSkillAExit()
        // {
        //     base.OnUpperIdleToSkillAExit();
        // }
        // public override void OnUpperIdleToSkillBExit()
        // {
        //     base.OnUpperIdleToSkillBExit();
        // }
        // public override void OnUpperIdleToSkillCExit()
        // {
        //     base.OnUpperIdleToSkillCExit();
        // }
        // public override void OnUpperIdleToCollectEnvExit()
        // {
        //     base.OnUpperIdleToCollectEnvExit();
        // }

        // public override void OnUpperMoveExit()
        // {
        //     base.OnUpperMoveExit();
        // }
        // public override void OnUpperMoveToSkillAExit()
        // {
        //     base.OnUpperMoveToSkillAExit();
        // }
        // public override void OnUpperMoveToSkillBExit()
        // {
        //     base.OnUpperMoveToSkillBExit();
        // }
        // public override void OnUpperMoveToSkillCExit()
        // {
        //     base.OnUpperMoveToSkillCExit();
        // }

        // public override void OnUpperDeadExit()
        // {
        //     base.OnUpperDeadExit();
        // }
        // #endregion
    }
}
