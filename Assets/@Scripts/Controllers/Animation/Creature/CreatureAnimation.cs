using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // [System.Serializable]
    // public class CreatureAnimState
    // {
    //     [field: SerializeField] public bool CanEnter { get; set; } = false;
    // }

    public class CreatureAnimation : BaseAnimation
    {
        private Creature _creatureOwner = null;
        public new Creature Owner => _creatureOwner;
        private CreatureAnimationCallback _creatureAnimCallback = null;

        [SerializeField] private bool[] _canEnterAnimStates = null;
        protected void EnteredAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState] = false;
        protected void ReleaseAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState] = true;
        public bool CanEnterAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState];

        // --- Upper Layer
        public readonly int Upper_Idle = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle);
        public readonly int Upper_Idle_To_Skill_Attack = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle_To_Skill_Attack);
        public readonly int Upper_Idle_To_CollectEnv = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle_To_CollectEnv);
        public readonly int Upper_Idle_To_Dead = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Idle_To_Dead);

        public readonly int Upper_Move = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Move);
        public readonly int Upper_Move_To_Skill_Attack = Animator.StringToHash(ReadOnly.AnimationParams.Upper_Move_To_Skill_Attack);

        // --- Lower Layer
        public readonly int Lower_Idle = Animator.StringToHash(ReadOnly.AnimationParams.Lower_Idle);
        public readonly int Lower_Move = Animator.StringToHash(ReadOnly.AnimationParams.Lower_Move);

        // --- Parameters
        protected readonly int Moving = Animator.StringToHash(ReadOnly.AnimationParams.Moving);
        protected readonly int Skill_Attack = Animator.StringToHash(ReadOnly.AnimationParams.Skill_Attack);
        protected readonly int CollectEnv = Animator.StringToHash(ReadOnly.AnimationParams.CollectEnv);
        protected readonly int AttackRange = Animator.StringToHash(ReadOnly.AnimationParams.AttackRange);
        protected readonly int ReadySkillAttack = Animator.StringToHash(ReadOnly.AnimationParams.ReadySkillAttack);
        protected readonly int OnDead = Animator.StringToHash(ReadOnly.AnimationParams.OnDead);

        // public bool IsDead
        // {
        //     get => Animator.GetBool(Dead);
        //     set => Animator.SetBool(Dead, value);
        // }

        public void Dead() => Animator.SetTrigger(OnDead);

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

        public override void SetInfo(int dataID, BaseObject owner)
        {
            base.SetInfo(dataID, owner);
            _creatureOwner = owner as Creature;
        }

        public override void UpdateAnimation() 
            => base.UpdateAnimation();


        public bool IsMoving
        {
            get => Animator.GetBool(Moving);
            set => Animator.SetBool(Moving, value);
        }

        public bool IsInAttackRange
        {
            get => Animator.GetBool(AttackRange);
            set => Animator.SetBool(AttackRange, value);
        }

        public bool CanSkillAttack
        {
            get => Animator.GetBool(ReadySkillAttack);
            set => Animator.SetBool(ReadySkillAttack, value);
        }

        public void Skill(ESkillType skillType)
        {
            switch (skillType)
            {
                case ESkillType.Skill_Attack:
                    {
                        // EnteredAnimState(ECreatureAnimState.Upper_Idle_To_Skill_Attack);
                        // EnteredAnimState(ECreatureAnimState.Upper_Move_To_Skill_Attack);
                        Animator.SetTrigger(Skill_Attack);
                    }
                    break;
            }
        }
        
        public virtual void Collect()
        {
            EnteredAnimState(ECreatureAnimState.Upper_Idle_To_CollectEnv);
            Animator.SetTrigger(CollectEnv);
        }

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
        }

        public virtual void OnCollectEnvCallback() { }
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

                case ECreatureAnimState.Upper_Idle_To_Skill_Attack:
                    OnUpperIdleToSkillAttackEnter();
                    break;

                case ECreatureAnimState.Upper_Idle_To_CollectEnv:
                    OnUpperIdleToCollectEnvEnter();
                    break;

                case ECreatureAnimState.Upper_Move:
                    OnUpperMoveEnter();
                    break;

            }
        }
        public virtual void OnUpperIdleEnter() { }
        public virtual void OnUpperIdleToSkillAttackEnter() { }
        public virtual void OnUpperIdleToCollectEnvEnter() { }
        public virtual void OnUpperMoveEnter() { }

        // --- Update
        public void OnAnimStateUpdate(ECreatureAnimState animState) { }

        // --- Exit
        public void OnAnimStateExit(ECreatureAnimState animState)
        {
            switch (animState)
            {
                case ECreatureAnimState.Upper_Idle_To_Skill_Attack:
                    OnUpperIdleToSkillAttackExit();
                    break;

                case ECreatureAnimState.Upper_Idle_To_CollectEnv:
                    OnUpperIdleToCollectEnvExit();
                    break;

                case ECreatureAnimState.Upper_Move_To_Skill_Attack:
                    OnUpperMoveToSkillAttackExit();
                    break;
            }
        }
        public virtual void OnUpperIdleToSkillAttackExit() { }
        public virtual void OnUpperIdleToCollectEnvExit() { }
        public virtual void OnUpperMoveToSkillAttackExit() { }

        #endregion
    }
}
