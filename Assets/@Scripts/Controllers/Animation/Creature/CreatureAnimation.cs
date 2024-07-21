using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAnimation : BaseAnimation
    {
        #region  Background
        private Creature _creatureOwner = null;
        public new Creature Owner => _creatureOwner;
        private CreatureAnimationCallback _creatureAnimCallback = null;

        [SerializeField] private bool[] _canEnterAnimStates = null;
        public void EnteredAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState] = false;
        public bool CanEnterAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState];
        public void ReleaseAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState] = true;
        public void ReleaseAllAnimState()
        {
            for (int i = 0; i < _canEnterAnimStates.Length; ++i)
                _canEnterAnimStates[i] = true;
        }

        // --- Upper Layer
        public readonly int Upper_Idle = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle);
        public readonly int Upper_Idle_To_Skill_A = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle_To_Skill_A);
        public readonly int Upper_Idle_To_Skill_B = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle_To_Skill_B);
        public readonly int Upper_Idle_To_Skill_C = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle_To_Skill_C);
        public readonly int Upper_Idle_To_CollectEnv = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle_To_CollectEnv);

        public readonly int Upper_Move = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Move);
        public readonly int Upper_Move_To_Skill_A = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Move_To_Skill_A);
        public readonly int Upper_Move_To_Skill_B = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Move_To_Skill_B);
        public readonly int Upper_Move_To_Skill_C = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Move_To_Skill_C);

        public readonly int Upper_Dead = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Dead);

        // --- Lower Layer
        public readonly int Lower_Idle = Animator.StringToHash(ReadOnly.AnimationParams.Lower_Idle);
        public readonly int Lower_Idle_To_Skill_A = Animator.StringToHash(ReadOnly.AnimationParams.Lower_Idle_To_Skill_A);
        public readonly int Lower_Move = Animator.StringToHash(ReadOnly.AnimationParams.Lower_Move);

        // --- Parameters
        protected readonly int IsMoving = Animator.StringToHash(ReadOnly.AnimationParams.IsMoving);
        protected readonly int CanSkill = Animator.StringToHash(ReadOnly.AnimationParams.CanSkill);
        protected readonly int OnSkill_A = Animator.StringToHash(ReadOnly.AnimationParams.OnSkill_A);
        protected readonly int LowerIdleToSkillA = Animator.StringToHash(ReadOnly.AnimationParams.LowerIdleToSkillA);
        
        protected readonly int OnSkill_B = Animator.StringToHash(ReadOnly.AnimationParams.OnSkill_B);
        protected readonly int OnSkill_C = Animator.StringToHash(ReadOnly.AnimationParams.OnSkill_C);
        protected readonly int OnCollectEnv = Animator.StringToHash(ReadOnly.AnimationParams.OnCollectEnv);

        public bool Moving
        {
            get => Animator.GetBool(IsMoving);
            set => Animator.SetBool(IsMoving, value);
        }

        public bool ReadySkill
        {
            get => Animator.GetBool(CanSkill);
            set => Animator.SetBool(CanSkill, value);
        }

        public bool EnterLowerAnimIdleToSkillA
        {
            get => Animator.GetBool(LowerIdleToSkillA);
            private set => Animator.SetBool(LowerIdleToSkillA, value);
        }

        public void Skill(ESkillType skillType)
        {
            switch (skillType)
            {
                case ESkillType.Skill_A:
                    {
                        EnteredAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A);
                        EnteredAnimState(ECreatureAnimState.Upper_Move_To_Skill_A);
                        Animator.SetTrigger(OnSkill_A);
                        Animator.ResetTrigger(OnSkill_B);
                        Animator.ResetTrigger(OnSkill_C);
                    }
                    break;

                case ESkillType.Skill_B:
                    {
                        Animator.SetTrigger(OnSkill_B);
                        Animator.ResetTrigger(OnSkill_A);
                        Animator.ResetTrigger(OnSkill_C);
                    }
                    break;

                case ESkillType.Skill_C:
                    {
                        Animator.SetTrigger(OnSkill_C);
                        Animator.ResetTrigger(OnSkill_A);
                        Animator.ResetTrigger(OnSkill_B);
                    }
                    break;
            }
        }

        public void CollectEnv()
        {
            EnteredAnimState(ECreatureAnimState.Upper_Idle_To_CollectEnv);
            Animator.SetTrigger(OnCollectEnv);
        }

        // --- BaseAnimation으로 옮김
        //public void Dead() => Animator.SetTrigger(OnDead);
       
        #endregion

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _creatureAnimCallback = GetComponent<CreatureAnimationCallback>();
            _canEnterAnimStates = new bool[(int)ECreatureAnimState.Max];
            for (int i = 0; i < _canEnterAnimStates.Length; ++i)
                _canEnterAnimStates[i] = true;

            return true;
        }

        public override void InitialSetInfo(int dataID, BaseObject owner)
        {
            base.InitialSetInfo(dataID, owner);
            _creatureOwner = owner as Creature;
        }
        #endregion

        #region Anim Clip Callbacks
        public void AddAnimClipEvents()
        {
            for (int i = 0; i < Owner.CreatureSkill.SkillArray.Length; ++i)
            {
                SkillBase skill = Owner.CreatureSkill.SkillArray[i];
                if (skill != null)
                {
                    _creatureAnimCallback.OnSkillCallbackHandler -= skill.OnSkillCallback;
                    _creatureAnimCallback.OnSkillCallbackHandler += skill.OnSkillCallback;
                }
            }

            _creatureAnimCallback.OnCollectEnvCallbackHandler -= OnCollectEnvCallback;
            _creatureAnimCallback.OnCollectEnvCallbackHandler += OnCollectEnvCallback;

            _creatureAnimCallback.OnEnterLowerAnimIdleToSkillACallbackHandler -= OnEnterLowerAnimIdleToSkillACallback;
            _creatureAnimCallback.OnEnterLowerAnimIdleToSkillACallbackHandler += OnEnterLowerAnimIdleToSkillACallback;
            
            _creatureAnimCallback.OnExitLowerAnimIdleToSkillACallbackHandler -= OnExitLowerAnimIdleToSkillACallback;
            _creatureAnimCallback.OnExitLowerAnimIdleToSkillACallbackHandler += OnExitLowerAnimIdleToSkillACallback;

            _creatureAnimCallback.OnDustEffectHandler -= OnDustEffectCallback;
            _creatureAnimCallback.OnDustEffectHandler += OnDustEffectCallback;
        }

        public virtual void OnCollectEnvCallback() { }
        public void OnEnterLowerAnimIdleToSkillACallback() => EnterLowerAnimIdleToSkillA = true;
        public void OnExitLowerAnimIdleToSkillACallback() => EnterLowerAnimIdleToSkillA = false;
        public virtual void OnDustEffectCallback() { }
        #endregion

        #region Anim State Events
        public void AddAnimStateEvents()
        {
            CreatureStateMachine[] creatureStateMachines = Animator.GetBehaviours<CreatureStateMachine>();
             for (int i = 0; i < creatureStateMachines.Length; ++i)
             {
                creatureStateMachines[i].OnAnimStateEnterHandler -= OnAnimStateEnter;
                creatureStateMachines[i].OnAnimStateEnterHandler += OnAnimStateEnter;

                creatureStateMachines[i].OnAnimStateUpdateHandler -= OnAnimStateUpdate;
                creatureStateMachines[i].OnAnimStateUpdateHandler += OnAnimStateUpdate;

                creatureStateMachines[i].OnAnimStateExitHandler -= OnAnimStateExit;
                creatureStateMachines[i].OnAnimStateExitHandler += OnAnimStateExit;
            }
        }

        // --- Enter
        public void OnAnimStateEnter(ECreatureAnimState animState)
        {
            switch (animState)
            {
                case ECreatureAnimState.Upper_Idle:
                    OnUpperIdleEnter();
                    break;
                case ECreatureAnimState.Upper_Idle_To_Skill_A:
                    OnUpperIdleToSkillAEnter();
                    break;
                case ECreatureAnimState.Upper_Idle_To_Skill_B:
                    OnUpperIdleToSkillBEnter();
                    break;
                case ECreatureAnimState.Upper_Idle_To_Skill_C:
                    OnUpperIdleToSkillCEnter();
                    break;
                case ECreatureAnimState.Upper_Idle_To_CollectEnv:
                    OnUpperIdleToCollectEnvEnter();
                    break;

                case ECreatureAnimState.Upper_Move:
                    OnUpperMoveEnter();
                    break;
                case ECreatureAnimState.Upper_Move_To_Skill_A:
                    OnUpperMoveToSkillAEnter();
                    break;
                case ECreatureAnimState.Upper_Move_To_Skill_B:
                    OnUpperMoveToSkillBEnter();
                    break;
                case ECreatureAnimState.Upper_Move_To_Skill_C:
                    OnUpperMoveToSkillCEnter();
                    break;

                case ECreatureAnimState.Upper_Dead:
                    OnUpperDeadEnter();
                    break;
            }
        }
        public virtual void OnUpperIdleEnter() { }
        public virtual void OnUpperIdleToSkillAEnter() 
        { 
            if (Owner.IsValid() == false)
                return;

            Owner.LookAtValidTarget();
            Owner.CreatureSkill.OnSkillStateEnter(ESkillType.Skill_A);
        }
        public virtual void OnUpperIdleToSkillBEnter() { }
        public virtual void OnUpperIdleToSkillCEnter() { }
        public virtual void OnUpperIdleToCollectEnvEnter() { }

        public virtual void OnUpperMoveEnter() { }
        public virtual void OnUpperMoveToSkillAEnter()
        {
            if (Owner.IsValid() == false)
                return;

            Owner.LookAtValidTarget();
            Owner.CreatureSkill.OnSkillStateEnter(ESkillType.Skill_A);
        }
        public virtual void OnUpperMoveToSkillBEnter() { }
        public virtual void OnUpperMoveToSkillCEnter() { }
        
        public virtual void OnUpperDeadEnter() {}


        // --- Update
        public void OnAnimStateUpdate(ECreatureAnimState animState)
        {
            switch (animState)
            {
                case ECreatureAnimState.Upper_Idle:
                    OnUpperIdleUpdate();
                    break;
                case ECreatureAnimState.Upper_Idle_To_Skill_A:
                    OnUpperIdleToSkillAUpdate();
                    break;
                case ECreatureAnimState.Upper_Idle_To_Skill_B:
                    OnUpperIdleToSkillBUpdate();
                    break;
                case ECreatureAnimState.Upper_Idle_To_Skill_C:
                    OnUpperIdleToSkillCUpdate();
                    break;
                case ECreatureAnimState.Upper_Idle_To_CollectEnv:
                    OnUpperIdleToCollectEnvUpdate();
                    break;

                case ECreatureAnimState.Upper_Move:
                    OnUpperMoveUpdate();
                    break;
                case ECreatureAnimState.Upper_Move_To_Skill_A:
                    OnUpperMoveToSkillAUpdate();
                    break;
                case ECreatureAnimState.Upper_Move_To_Skill_B:
                    OnUpperMoveToSkillBUpdate();
                    break;
                case ECreatureAnimState.Upper_Move_To_Skill_C:
                    OnUpperMoveToSkillCUpdate();
                    break;

                case ECreatureAnimState.Upper_Dead:
                    OnUpperDeadUpdate();
                    break;
            }
        }
        public virtual void OnUpperIdleUpdate() { }
        public virtual void OnUpperIdleToSkillAUpdate() 
        { 
            if (Owner.IsValid() == false)
                return;

            Owner.LookAtValidTarget();
            Owner.CreatureSkill.OnSkillStateUpdate(ESkillType.Skill_A);
        }
        public virtual void OnUpperIdleToSkillBUpdate() { }
        public virtual void OnUpperIdleToSkillCUpdate() { }
        public virtual void OnUpperIdleToCollectEnvUpdate() { }

        public virtual void OnUpperMoveUpdate() { }
        public virtual void OnUpperMoveToSkillAUpdate() 
        {
            if (Owner.IsValid() == false)
                return;

            Owner.LookAtValidTarget();
            Owner.CreatureSkill.OnSkillStateUpdate(ESkillType.Skill_A);
        }
        public virtual void OnUpperMoveToSkillBUpdate() { }
        public virtual void OnUpperMoveToSkillCUpdate() { }

        public virtual void OnUpperDeadUpdate() {}


        // --- Exit
        public void OnAnimStateExit(ECreatureAnimState animState)
        {
            switch (animState)
            {
                case ECreatureAnimState.Upper_Idle:
                    OnUpperIdleExit();
                    break;
                case ECreatureAnimState.Upper_Idle_To_Skill_A:
                    OnUpperIdleToSkillAExit();
                    break;
                case ECreatureAnimState.Upper_Idle_To_Skill_B:
                    OnUpperIdleToSkillAExit();
                    break;
                case ECreatureAnimState.Upper_Idle_To_Skill_C:
                    OnUpperIdleToSkillAExit();
                    break;
                case ECreatureAnimState.Upper_Idle_To_CollectEnv:
                    OnUpperIdleToCollectEnvExit();
                    break;

                case ECreatureAnimState.Upper_Move:
                    OnUpperMoveExit();
                    break;
                case ECreatureAnimState.Upper_Move_To_Skill_A:
                    OnUpperMoveToSkillAExit();
                    break;
                case ECreatureAnimState.Upper_Move_To_Skill_B:
                    OnUpperMoveToSkillBExit();
                    break;
                case ECreatureAnimState.Upper_Move_To_Skill_C:
                    OnUpperMoveToSkillCExit();
                    break;

                case ECreatureAnimState.Upper_Dead:
                    OnUpperDeadExit();
                    break;
            }
        }
        public virtual void OnUpperIdleExit() { }
        public virtual void OnUpperIdleToSkillAExit() 
        {
            if (Owner.IsValid() == false)
                return;

            ReleaseAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A);
            ReleaseAnimState(ECreatureAnimState.Upper_Move_To_Skill_A);
            Owner.CreatureSkill.OnSkillStateExit(ESkillType.Skill_A);
        }
        public virtual void OnUpperIdleToSkillBExit() { }
        public virtual void OnUpperIdleToSkillCExit() { }
        public virtual void OnUpperIdleToCollectEnvExit() { }

        public virtual void OnUpperMoveExit() { }
        public virtual void OnUpperMoveToSkillAExit() 
        { 
            if (Owner.IsValid() == false)
                return;

            ReleaseAnimState(ECreatureAnimState.Upper_Idle_To_Skill_A);
            ReleaseAnimState(ECreatureAnimState.Upper_Move_To_Skill_A);
            Owner.CreatureSkill.OnSkillStateExit(ESkillType.Skill_A);
        }
        public virtual void OnUpperMoveToSkillBExit() { }
        public virtual void OnUpperMoveToSkillCExit() { }

        public virtual void OnUpperDeadExit() { }
        #endregion
    }
}

/*
    [PREV REF]
       // public override void UpdateAnimation() 
        //     => base.UpdateAnimation();

        // public bool IsMoving
        // {
        //     get => Animator.GetBool(Moving);
        //     set => Animator.SetBool(Moving, value);
        // }

        // public bool IsInAttackRange
        // {
        //     get => Animator.GetBool(AttackRange);
        //     set => Animator.SetBool(AttackRange, value);
        // }

        // public bool CanSkillAttack
        // {
        //     get => Animator.GetBool(ReadySkillAttack);
        //     set => Animator.SetBool(ReadySkillAttack, value);
        // }



        // public virtual void PlayCreatureAnimation(ECreatureAIState creatureState)
        // {
        //     switch (creatureState)
        //     {
        //         case ECreatureAIState.Idle:
        //             Animator.Play(Upper_Idle);
        //             Animator.Play(Lower_Idle);
        //             break;

        //         case ECreatureAIState.Move:
        //             Animator.Play(Upper_Move);
        //             Animator.Play(Lower_Move);
        //             break;

        //         case ECreatureAIState.Skill_Attack:
        //             //Animator.Play(Play_Skill_Attack);
        //             //Animator.Play(Upper_Skill_Attack);
        //             break;

        //         case ECreatureAIState.CollectEnv:
        //             Animator.Play(Play_CollectEnv);
        //             break;

        //         case ECreatureAIState.Dead:
        //             Animator.Play(Play_Dead);
        //             break;
        //     }
        // }

        // public bool IsCurrentAnimationState(ECreatureAIState creatureState)
        // {
        //     AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(layerIndex: 0);
        //     return stateInfo.shortNameHash == GetHash(creatureState);
        // }
*/