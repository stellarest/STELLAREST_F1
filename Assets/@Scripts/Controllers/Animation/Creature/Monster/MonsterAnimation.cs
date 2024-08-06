using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MonsterAnimation : CreatureAnimation
    {
        private Monster _monsterOwner = null;
        public override void InitialSetInfo(int dataID, BaseObject owner)
        {
            base.InitialSetInfo(dataID, owner);
            string animatorTextID = Managers.Data.MonsterDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
                Animator.runtimeAnimatorController = animController;

            _monsterOwner = owner as Monster;
        }

        public override void Flip(ELookAtDirection lookAtDir)
        {
            // --- Monster Default Sprite Dir: Left
            Vector3 localScale = _monsterOwner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ? (localScale.x > 0 ? 1 : -1) : -1;
            localScale.x = localScale.x * sign;
            _monsterOwner.transform.localScale = localScale;
        }

        // --- Enter
        protected override void OnUpperIdleEnter()
        {
            if (IsValidOwner == false)
                return;

            if (_monsterOwner.Target.IsValid() == false)
                _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;
            else
                _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Angry;
            
            base.OnUpperIdleEnter();
        }

        protected override void OnUpperMoveEnter()
        {
            if (IsValidOwner == false)
                return;

            _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;
            _monsterOwner.CreatureAIState = ECreatureAIState.Move; // --- ???
            _monsterOwner.StartCoLerpToCellPos(); // --- ???
            base.OnUpperMoveEnter();
        }

        protected override void OnUpperSkillAEnter()
        {
            if (IsValidOwner == false)
                return;

            _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Angry;
            base.OnUpperSkillAEnter();
        }

        protected override void OnUpperSkillBEnter() => base.OnUpperSkillBEnter();
        protected override void OnUpperSkillCEnter() => base.OnUpperSkillCEnter();
        protected override void OnUpperDeadEnter()
        {
            _monsterOwner.MonsterBody.MonsterEmoji = EMonsterEmoji.Dead;
            base.OnUpperDeadEnter();
        }

        // --- Exit
        protected override void OnUpperIdleExit()
        {
            if (IsValidOwner == false)
                return;

            base.OnUpperIdleExit();
        }
        protected override void OnUpperMoveExit()
        {
            if (IsValidOwner == false)
                return;

            base.OnUpperMoveExit();
        }
        protected override void OnUpperSkillAExit()
        {
            if (IsValidOwner == false)
                return;

            base.OnUpperSkillAExit();
        }
        protected override void OnUpperSkillBExit()
        {
            if (IsValidOwner == false)
                return;

            base.OnUpperSkillBExit();
        }
        protected override void OnUpperSkillCExit()
        {
            if (IsValidOwner == false)
                return;

            base.OnUpperSkillCExit();
        }
        
        protected override void OnUpperDeadExit()
            => base.OnUpperDeadExit();
    }
}

/*
        // Cloned
        // RuntimeAnimatorController cloned = UnityEngine.Object.Instantiate(animController);
        // Animator.runtimeAnimatorController = cloned;
*/