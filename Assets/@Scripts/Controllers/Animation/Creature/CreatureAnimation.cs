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
        private Creature _creatureOwner = null;
        public override BaseObject Owner => _creatureOwner;

        private CreatureAnimationCallback _creatureAnimCallback = null;

        [SerializeField] private bool[] _canEnterAnimStates = null;
        public void EnteredAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState] = false;
        public bool CanEnterAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState];
        public void ReleaseAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState] = true;
        public void ReleaseAllAnimStates()
        {
            for (int i = 0; i < _canEnterAnimStates.Length; ++i)
                _canEnterAnimStates[i] = true;
        }

        // --- Upper Layer (New)
        public readonly int Upper_Idle = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle);
        public readonly int Upper_Move = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Move);
        public readonly int Upper_SkillA = Animator.StringToHash(ReadOnly.AnimationParams.Upper_SkillA);
        public readonly int Upper_CollectEnv = Animator.StringToHash(ReadOnly.AnimationParams.Upper_CollectEnv);
        public readonly int Upper_Dead = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Dead);

        // --- Lower Layer (New)
        public readonly int Lower_Idle = Animator.StringToHash(ReadOnly.AnimationParams.Lower_Idle);
        public readonly int Lower_Move = Animator.StringToHash(ReadOnly.AnimationParams.Lower_Move);

        // --- Parameters
        protected readonly int IsMoving = Animator.StringToHash(ReadOnly.AnimationParams.IsMoving);
        protected readonly int CanSkill = Animator.StringToHash(ReadOnly.AnimationParams.CanSkill);
        protected readonly int OnSkill_A = Animator.StringToHash(ReadOnly.AnimationParams.OnSkill_A);
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

        public void Skill(ESkillType skillType)
        {
            switch (skillType)
            {
                case ESkillType.Skill_A:
                    {
                        // ---> AnimState
                        // EnteredAnimState(ECreatureAnimState.Upper_SkillA);
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
            //EnteredAnimState(ECreatureAnimState.Upper_CollectEnv);
            Animator.SetTrigger(OnCollectEnv);
        }

        // --- BaseAnimation으로 옮김
        //public void Dead() => Animator.SetTrigger(OnDead);

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _creatureAnimCallback = GetComponent<CreatureAnimationCallback>();
            _canEnterAnimStates = new bool[(int)ECreatureAnimState.Max];
            ReleaseAllAnimStates();

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
            for (int i = 0; i < _creatureOwner.CreatureSkill.SkillArray.Length; ++i)
            {
                SkillBase skill = _creatureOwner.CreatureSkill.SkillArray[i];
                if (skill != null)
                {
                    _creatureAnimCallback.OnSkillCallbackHandler -= skill.OnSkillCallback;
                    _creatureAnimCallback.OnSkillCallbackHandler += skill.OnSkillCallback;
                }
            }

            _creatureAnimCallback.OnCollectEnvCallbackHandler -= OnCollectEnvCallback;
            _creatureAnimCallback.OnCollectEnvCallbackHandler += OnCollectEnvCallback;

            // _creatureAnimCallback.OnEnterLowerAnimIdleToSkillACallbackHandler -= OnEnterLowerAnimIdleToSkillACallback;
            // _creatureAnimCallback.OnEnterLowerAnimIdleToSkillACallbackHandler += OnEnterLowerAnimIdleToSkillACallback;
            
            // _creatureAnimCallback.OnExitLowerAnimIdleToSkillACallbackHandler -= OnExitLowerAnimIdleToSkillACallback;
            // _creatureAnimCallback.OnExitLowerAnimIdleToSkillACallbackHandler += OnExitLowerAnimIdleToSkillACallback;

            _creatureAnimCallback.OnDustEffectHandler -= OnDustEffectCallback;
            _creatureAnimCallback.OnDustEffectHandler += OnDustEffectCallback;
        }

        public virtual void OnCollectEnvCallback() { }
        // public void OnEnterLowerAnimIdleToSkillACallback() => EnterLowerAnimIdleToSkillA = true;
        // public void OnExitLowerAnimIdleToSkillACallback() => EnterLowerAnimIdleToSkillA = false;
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

                // --- 일단 생략
                // creatureStateMachines[i].OnAnimStateUpdateHandler -= OnAnimStateUpdate;
                // creatureStateMachines[i].OnAnimStateUpdateHandler += OnAnimStateUpdate;

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
                case ECreatureAnimState.Upper_Move:
                    OnUpperMoveEnter();
                    break;
                case ECreatureAnimState.Upper_SkillA:
                    OnUpperSkillAEnter();
                    break;
                case ECreatureAnimState.Upper_SkillB:
                    OnUpperSkillBEnter();
                    break;
                case ECreatureAnimState.Upper_SkillC:
                    OnUpperSkillCEnter();
                    break;
                case ECreatureAnimState.Upper_CollectEnv:
                    OnUpperCollectEnvEnter();
                    break;
                case ECreatureAnimState.Upper_Dead:
                    OnUpperDeadEnter();
                    break;
            }
        }
        protected virtual void OnUpperIdleEnter() 
        {
            EnteredAnimState(ECreatureAnimState.Upper_Idle);
        }
        protected virtual void OnUpperMoveEnter()
        {
            EnteredAnimState(ECreatureAnimState.Upper_Move);
        }
        protected virtual void OnUpperSkillAEnter()
        {
            EnteredAnimState(ECreatureAnimState.Upper_SkillA);
            _creatureOwner.LookAtValidTarget();
            _creatureOwner.CreatureSkill.OnSkillStateEnter(ESkillType.Skill_A);
        }
        protected virtual void OnUpperSkillBEnter() 
        {
            EnteredAnimState(ECreatureAnimState.Upper_SkillB);
        }
        protected virtual void OnUpperSkillCEnter() 
        { 
            EnteredAnimState(ECreatureAnimState.Upper_SkillC);
        }
        protected virtual void OnUpperCollectEnvEnter() 
        { 
            EnteredAnimState(ECreatureAnimState.Upper_CollectEnv);
        }
        protected virtual void OnUpperDeadEnter() 
        {
            ReleaseAllAnimStates();
            EnteredAnimState(ECreatureAnimState.Upper_Dead);
        }
        
        // --- Exit
        public void OnAnimStateExit(ECreatureAnimState animState)
        {
            switch (animState)
            {
                case ECreatureAnimState.Upper_Idle:
                    OnUpperIdleExit();
                    break;
                case ECreatureAnimState.Upper_Move:
                    OnUpperMoveExit();
                    break;
                case ECreatureAnimState.Upper_SkillA:
                    OnUpperSkillAExit();
                    break;
                case ECreatureAnimState.Upper_SkillB:
                    OnUpperSkillBExit();
                    break;
                case ECreatureAnimState.Upper_SkillC:
                    OnUpperSkillCExit();
                    break;
                case ECreatureAnimState.Upper_CollectEnv:
                    OnUpperCollectEnvExit();
                    break;
                case ECreatureAnimState.Upper_Dead:
                    OnUpperDeadExit();
                    break;
            }
        }
        protected virtual void OnUpperIdleExit() 
        { 
            ReleaseAnimState(ECreatureAnimState.Upper_Idle);
        }
        protected virtual void OnUpperMoveExit() 
        { 
            ReleaseAnimState(ECreatureAnimState.Upper_Move);
        }
        protected virtual void OnUpperSkillAExit() 
        {
            ReleaseAnimState(ECreatureAnimState.Upper_SkillA);
            _creatureOwner.CreatureSkill.OnSkillStateExit(ESkillType.Skill_A);
        }
        protected virtual void OnUpperSkillBExit() 
        {
            ReleaseAnimState(ECreatureAnimState.Upper_SkillB);
            _creatureOwner.CreatureSkill.OnSkillStateExit(ESkillType.Skill_B);
        }
        protected virtual void OnUpperSkillCExit() 
        {
            ReleaseAnimState(ECreatureAnimState.Upper_SkillC);
            _creatureOwner.CreatureSkill.OnSkillStateExit(ESkillType.Skill_C);
        }
        protected virtual void OnUpperCollectEnvExit() 
        { 
            ReleaseAnimState(ECreatureAnimState.Upper_CollectEnv);
        }
        protected virtual void OnUpperDeadExit() 
        { 
            ReleaseAnimState(ECreatureAnimState.Upper_Dead);
        }
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