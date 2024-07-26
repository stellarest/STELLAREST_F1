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
        public override BaseObject Owner => _heroOwner;

        public override void InitialSetInfo(int dataID, BaseObject owner)
        {
            // Debug.Log("1");
            base.InitialSetInfo(dataID, owner);
            string animatorTextID = Managers.Data.HeroDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
                this.Animator.runtimeAnimatorController = animController;

            _heroOwner = owner as Hero;
            Debug.Log("HeroAnim::InitialSetInfo");
        }

        public override void Flip(ELookAtDirection lookAtDir)
        {
            // --- Hero LookAtDir Default Sprite : Right
            Vector3 localScale = _heroOwner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ?
                                             -1 : (localScale.x < 0) ? -1 : 1;
            localScale.x = localScale.x * sign;
            _heroOwner.transform.localScale = localScale;
        }

        #region Anim Clip Callbacks
        public override void OnCollectEnvCallback()
        {
            if (_heroOwner.IsValid() == false)
                return;

            if (_heroOwner.Target.IsValid() == false)
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
            Vector3 spawnPos = _heroOwner.HeroBody.GetContainer(EHeroBody_Lower.LegR).TR.position;
            EffectBase dustEffect = Managers.Object.SpawnBaseObject<EffectBase>(
                objectType: EObjectType.Effect,
                spawnPos: spawnPos,
                dataID: ReadOnly.DataAndPoolingID.DNPID_Effect_Dust,
                owner: _heroOwner
            );
            dustEffect.SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_BaseObject;
        }
        #endregion

        #region Anim State Events (New)
        // --- Enter
        protected override void OnUpperIdleEnter()
        {
            if (_heroOwner.IsValid() == false)
                return;

            _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Idle;
            if (CanEnterAnimState(ECreatureAnimState.Upper_SkillA) == false)
                ReleaseAnimState(ECreatureAnimState.Upper_SkillA);

            base.OnUpperIdleEnter();
        }

        protected override void OnUpperMoveEnter()
        {
            if (_heroOwner.IsValid() == false)
                return;

            _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Move;
            base.OnUpperMoveEnter();
        }

        protected override void OnUpperSkillAEnter()
        {
            if (_heroOwner.IsValid() == false)
                return;

            _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Skill_A;
            base.OnUpperSkillAEnter();
        }

        protected override void OnUpperSkillBEnter()
        {
            if (_heroOwner.IsValid() == false)
                return;

            base.OnUpperSkillBEnter();
        }

        protected override void OnUpperSkillCEnter()
        {
            if (_heroOwner.IsValid() == false)
                return;

            base.OnUpperSkillCEnter();
        }

        protected override void OnUpperCollectEnvEnter()
        {
            if (_heroOwner.IsValid() == false)
                return;

            Debug.Log($"<color=white>{nameof(OnUpperCollectEnvEnter)}</color>");
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
            if (_heroOwner.IsValid() == false)
                return;

            base.OnUpperIdleExit();
        }

        protected override void OnUpperMoveExit()
        {
            if (_heroOwner.IsValid() == false)
                return;

            base.OnUpperMoveExit();
        }

        protected override void OnUpperSkillAExit()
        {
            if (_heroOwner.IsValid() == false)
                return;

            base.OnUpperSkillAExit();
        }

        protected override void OnUpperSkillBExit()
        {
            if (_heroOwner.IsValid() == false)
                return;

            base.OnUpperSkillBExit();
        }

        protected override void OnUpperSkillCExit()
        {
            if (_heroOwner.IsValid() == false)
                return;

            base.OnUpperSkillCExit();
        }

        protected override void OnUpperCollectEnvExit()
        {
            if (_heroOwner.IsValid() == false)
                return;

            Debug.Log($"<color=white>{nameof(OnUpperCollectEnvExit)}</color>");
            if (_heroOwner.HeroWeaponType != EHeroWeaponType.Default)
                _heroOwner.HeroWeaponType = EHeroWeaponType.Default;

            base.OnUpperCollectEnvExit();
        }

        protected override void OnUpperDeadExit()
        {
            base.OnUpperDeadExit();
        }
        #endregion
    }
}

/*
    [PREV CODE]
// public override void UpdateAnimation(ECreatureState creatureState)
//         {
//             if (_owner == null)
//                 return;

//             switch (_owner.CreatureState)
//             {
//                 case ECreatureState.Idle:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Idle);
//                         Idle();
//                     }
//                     break;

//                 case ECreatureState.Move:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Move);
//                         Move();
//                     }
//                     break;

//                 case ECreatureState.Skill_Attack:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Skill_Attack);
//                         Skill_Attack();
//                     }
//                     break;

//                 case ECreatureState.Skill_A:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Skill_Attack);
//                         Skill_A();
//                     }
//                     break;

//                 case ECreatureState.Skill_B:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Skill_Attack);
//                         Skill_B();
//                     }
//                     break;

//                 case ECreatureState.CollectEnv:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Idle);
//                         CollectEnv();
//                     }
//                     break;

//                 case ECreatureState.Dead:
//                     {
//                         _owner.HeroBody.SetEmoji(EHeroEmoji.Dead);
//                         Dead();
//                     }
//                     break;
//             }
//         }

// #region Anim State Events (Prev)
        // // --- Enter
        // public override void OnUpperIdleToCollectEnvEnter()
        // {
        //     _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.CollectEnv;
        //     base.OnUpperIdleToCollectEnvEnter();
        //     // Debug.Log($"<color=white>{nameof(OnUpperIdleToCollectEnvEnter)}</color>");
        //     if (_heroOwner.Target.IsValid() && _heroOwner.Target.ObjectType == EObjectType.Env)
        //     {
        //         if (_heroOwner.HeroWeaponType == EHeroWeaponType.Default)
        //         {
        //             Env envTarget = _heroOwner.Target as Env;
        //             if (envTarget.EnvType == EEnvType.Tree)
        //                 _heroOwner.HeroWeaponType = EHeroWeaponType.CollectTree;
        //             else if (envTarget.EnvType == EEnvType.Rock)
        //                 _heroOwner.HeroWeaponType = EHeroWeaponType.CollectRock;
        //         }

        //         _heroOwner.LookAtValidTarget();
        //     }
        // }

        // public override void OnUpperMoveEnter()
        // {
        //     _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Move;
        //     base.OnUpperMoveEnter();
        // }
        // public override void OnUpperMoveToSkillAEnter()
        // {
        //     _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Skill_A;
        //     base.OnUpperIdleToSkillAEnter();
        // }
        // public override void OnUpperMoveToSkillBEnter()
        // {
        //     base.OnUpperIdleToSkillBEnter();
        // }
        // public override void OnUpperMoveToSkillCEnter()
        // {
        //     base.OnUpperIdleToSkillCEnter();
        // }

        // public override void OnUpperDeadEnter()
        // {
        //     _heroOwner.HeroBody.HeroEmoji = EHeroEmoji.Dead;
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
        //     base.OnUpperMoveToSkillAUpdate();
        // }
        // public override void OnUpperMoveToSkillCUpdate()
        // {
        //     base.OnUpperMoveToSkillAUpdate();
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
        //     // --- DEFENSE
        //     if (CanEnterAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A) == false ||
        //         CanEnterAnimState(ECreatureAnimState.Upper_Move_To_Skill_A) == false)
        //     {
        //         ReleaseAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A);
        //         ReleaseAnimState(ECreatureAnimState.Upper_Move_To_Skill_A);
        //     }

        //     ReleaseAnimState(ECreatureAnimState.Upper_Idle_To_CollectEnv);
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
*/