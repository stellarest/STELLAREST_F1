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
        public new Hero Owner => _heroOwner;

        public override void InitialSetInfo(int dataID, BaseObject owner)
        {
            base.InitialSetInfo(dataID, owner);
            string animatorTextID = Managers.Data.HeroDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
                this.Animator.runtimeAnimatorController = animController;

            _heroOwner = owner as Hero;
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
            if (Owner.IsValid() == false)
                return;

            if (Owner.Target.IsValid() == false)
                return;

            if (Owner.Target.ObjectType != EObjectType.Env)
                return;

            Owner.Target.OnDamaged(attacker: Owner, skillByAttacker: null);
            if (Owner.Target.IsValid() == false && Owner.HeroWeaponType != EHeroWeaponType.Default)
            {
                Owner.HeroWeaponType = EHeroWeaponType.Default;
                Owner.CreatureAIState = ECreatureAIState.Move;
            }

            // --- Prev
            // if (Owner.IsValid() == false)
            //     return;

            // if (Owner.Target.IsValid() == false)
            //     return;

            // if (Owner.Target.ObjectType != EObjectType.Env)
            //     return;

            // Owner.Target.OnDamaged(attacker: Owner, skillFromAttacker: null);
            // if (Owner.Target.Hp <= 0f && Owner.HeroWeaponType != EHeroWeaponType.Default)
            // {
            //     Owner.HeroWeaponType = EHeroWeaponType.Default;
            //     Owner.CreatureAIState = ECreatureAIState.Move;
            // }
        }

        public override void OnDustEffectCallback()
        {
            Vector3 spawnPos = Owner.HeroBody.GetContainer(EHeroBody_Lower.LegR).TR.position;
            EffectBase dustEffect = Managers.Object.SpawnBaseObject<EffectBase>(
                objectType: EObjectType.Effect,
                spawnPos: spawnPos,
                dataID: ReadOnly.DataAndPoolingID.DNPID_Effect_Dust,
                owner: Owner
            );
            dustEffect.SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_BaseObject;
        }
        #endregion

        #region Anim State Events
        // --- Enter
        public override void OnUpperIdleEnter()
        {
            Owner.HeroBody.HeroEmoji = EHeroEmoji.Idle;

            base.OnUpperIdleEnter();
            if (CanEnterAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A) == false ||
               CanEnterAnimState(ECreatureAnimState.Upper_Move_To_Skill_A) == false)
            {
                ReleaseAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A);
                ReleaseAnimState(ECreatureAnimState.Upper_Move_To_Skill_A);
            }
        }
        public override void OnUpperIdleToSkillAEnter()
        {
            Owner.HeroBody.HeroEmoji = EHeroEmoji.Skill_A;
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
            Owner.HeroBody.HeroEmoji = EHeroEmoji.CollectEnv;
            base.OnUpperIdleToCollectEnvEnter();
            // Debug.Log($"<color=white>{nameof(OnUpperIdleToCollectEnvEnter)}</color>");
            if (Owner.Target.IsValid() && Owner.Target.ObjectType == EObjectType.Env)
            {
                if (Owner.HeroWeaponType == EHeroWeaponType.Default)
                {
                    Env envTarget = Owner.Target as Env;
                    if (envTarget.EnvType == EEnvType.Tree)
                        Owner.HeroWeaponType = EHeroWeaponType.CollectTree;
                    else if (envTarget.EnvType == EEnvType.Rock)
                        Owner.HeroWeaponType = EHeroWeaponType.CollectRock;
                }

                Owner.LookAtValidTarget();
            }
        }

        public override void OnUpperMoveEnter()
        {
            Owner.HeroBody.HeroEmoji = EHeroEmoji.Move;
            base.OnUpperMoveEnter();
        }
        public override void OnUpperMoveToSkillAEnter()
        {
            Owner.HeroBody.HeroEmoji = EHeroEmoji.Skill_A;
            base.OnUpperIdleToSkillAEnter();
        }
        public override void OnUpperMoveToSkillBEnter()
        {
            base.OnUpperIdleToSkillBEnter();
        }
        public override void OnUpperMoveToSkillCEnter()
        {
            base.OnUpperIdleToSkillCEnter();
        }

        public override void OnUpperDeadEnter()
        {
            Owner.HeroBody.HeroEmoji = EHeroEmoji.Dead;
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
            base.OnUpperMoveToSkillAUpdate();
        }
        public override void OnUpperMoveToSkillCUpdate()
        {
            base.OnUpperMoveToSkillAUpdate();
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
            // --- DEFENSE
            if (CanEnterAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A) == false ||
                CanEnterAnimState(ECreatureAnimState.Upper_Move_To_Skill_A) == false)
            {
                ReleaseAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A);
                ReleaseAnimState(ECreatureAnimState.Upper_Move_To_Skill_A);
            }

            ReleaseAnimState(ECreatureAnimState.Upper_Idle_To_CollectEnv);
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
*/