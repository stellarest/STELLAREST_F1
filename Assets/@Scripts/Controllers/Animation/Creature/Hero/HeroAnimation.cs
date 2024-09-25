using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    /*
        Hero Idle : 60 frame,  hero Move : 25 frame        
        Paladin Attack : 30 frame // 공속 나중에 따로 조정
        Archer Attack : 50 frame
        Wizard Attack : 60 frame
    */
    public class HeroAnimation : CreatureAnimation
    {
        private Hero _heroOwner = null;
        public override void InitialSetInfo(int dataID, BaseCellObject owner)
        {
            base.InitialSetInfo(dataID, owner);
            string animatorTextID = Managers.Data.HeroDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
                Animator.runtimeAnimatorController = animController;

            _heroOwner = owner as Hero;
        }

        public override void Flip(ELookAtDirection lookAtDir)
        {
            // --- Hero Default Sprite Dir: Left
            Vector3 localScale = _heroOwner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ?
                                             -1 : (localScale.x < 0) ? -1 : 1;
            localScale.x = localScale.x * sign;
            _heroOwner.transform.localScale = localScale;
        }

        #region Anim Clip Callbacks
        public override void OnCollectEnvCallback()
        {
            if (_owner.IsValid() == false)
                return;

            if (_owner.Target.IsValid() == false)
                return;

            if (_heroOwner.Target.ObjectType != EObjectType.Env)
                return;

            _heroOwner.Target.OnDamaged(attacker: _heroOwner, skillByAttacker: null);
            if (_heroOwner.Target.IsValid() == false && _heroOwner.HeroWeaponType != EHeroWeaponType.Default)
            {
                _heroOwner.HeroWeaponType = EHeroWeaponType.Default;
                _heroOwner.CreatureAIState = ECreatureAIState.Move;
            }
        }

        public override void OnDustEffectCallback()
        {
            if (_owner.IsValid() == false)
                return;

            Vector3 spawnPos = _heroOwner.HeroBody.GetContainer(EHeroBody_Lower.LegR).TR.position;
            EffectBase dustEffect = _heroOwner.BaseEffect.GenerateEffect(
                                        effectID: ReadOnly.DataAndPoolingID.DNPID_Effect_Global_Dust,
                                        spawnPos: spawnPos
                                        );
            
            dustEffect.SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_BaseObject;
        }
        #endregion

        #region Anim State Events (New)
        // --- Enter
        protected override void OnUpperIdleEnter()
        {
            if (_owner.IsValid() == false)
                return;

            _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Idle;
            if (CanEnterAnimState(ECreatureAnimState.Upper_SkillA) == false)
                ReleaseAnimState(ECreatureAnimState.Upper_SkillA);
            if (CanEnterAnimState(ECreatureAnimState.Upper_SkillB) == false)
                ReleaseAnimState(ECreatureAnimState.Upper_SkillB);

            base.OnUpperIdleEnter();
        }

        protected override void OnUpperMoveEnter()
        {
            if (_owner.IsValid() == false)
                return;

            _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Move;
            base.OnUpperMoveEnter();
        }

        protected override void OnUpperSkillAEnter()
        {
            if (_owner.IsValid() == false)
                return;

            _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Skill_A;
            base.OnUpperSkillAEnter();
        }

        protected override void OnUpperSkillBEnter()
        {
            if (_owner.IsValid() == false)
                return;

            _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Skill_B;
            base.OnUpperSkillBEnter();
        }

        protected override void OnUpperSkillCEnter()
        {
            if (_owner.IsValid() == false)
                return;

            _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Skill_C;
            base.OnUpperSkillCEnter();
        }

        protected override void OnUpperCollectEnvEnter()
        {
            if (_owner.IsValid() == false)
                return;

            // Debug.Log($"<color=white>{nameof(OnUpperCollectEnvEnter)}</color>");
            if (_heroOwner.Target.IsValid() && _heroOwner.Target.ObjectType == EObjectType.Env)
            {
                if (_heroOwner.HeroWeaponType == EHeroWeaponType.Default)
                {
                    Env envTarget = _heroOwner.Target as Env;
                    if (envTarget.EnvType == EEnvType.Tree)
                        _heroOwner.HeroWeaponType = EHeroWeaponType.CollectTree;
                    else if (envTarget.EnvType == EEnvType.Rock)
                        _heroOwner.HeroWeaponType = EHeroWeaponType.CollectRock;

                    _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.CollectEnv;
                    _heroOwner.LookAtValidTarget();
                    base.OnUpperCollectEnvEnter();
                }
            }
        }

        protected override void OnUpperDeadEnter()
        {
            _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Dead;
            base.OnUpperDeadEnter();
        }

        // --- Update (필요시 정의)

        // --- Exit
        protected override void OnUpperIdleExit()
        {
            if (_owner.IsValid() == false)
                return;

            base.OnUpperIdleExit();
        }

        protected override void OnUpperMoveExit()
        {
            if (_owner.IsValid() == false)
                return;

            base.OnUpperMoveExit();
        }

        protected override void OnUpperSkillAExit()
        {
            if (_owner.IsValid() == false)
                return;

            base.OnUpperSkillAExit();
        }

        protected override void OnUpperSkillBExit()
        {
            if (_owner.IsValid() == false)
                return;

            base.OnUpperSkillBExit();
        }

        protected override void OnUpperSkillCExit()
        {
            if (_owner.IsValid() == false)
                return;

            base.OnUpperSkillCExit();
        }

        protected override void OnUpperCollectEnvExit()
        {
            if (_owner.IsValid() == false)
                return;

            // Debug.Log($"<color=white>{nameof(OnUpperCollectEnvExit)}</color>");
            if (_heroOwner.HeroWeaponType != EHeroWeaponType.Default)
                _heroOwner.HeroWeaponType = EHeroWeaponType.Default;

            base.OnUpperCollectEnvExit();
        }

        protected override void OnUpperDeadExit()
            => base.OnUpperDeadExit();
        #endregion
    }
}
