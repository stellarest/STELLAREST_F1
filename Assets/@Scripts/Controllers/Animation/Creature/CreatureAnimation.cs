using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAnimation : BaseAnimation
    {
        private Creature _creatureOwner = null;
        private CreatureAnimationCallback _creatureAnimCallback = null;

        [SerializeField] private bool[] _canEnterAnimStates = null;
        public bool[] CanEnterAnimStates => _canEnterAnimStates;
        public void EnteredAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState] = false;
        public bool CanEnterAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState];
        public bool IsEnteredAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState] == false;
        public void ReleaseAnimState(ECreatureAnimState animState)
            => _canEnterAnimStates[(int)animState] = true;
        private void ReleaseAllAnimStates()
        {
            for (int i = 0; i < _canEnterAnimStates.Length; ++i)
                _canEnterAnimStates[i] = true;
        }

        private void ReleaseAllSkillsAnimStates()
        {
            _canEnterAnimStates[(int)ECreatureAnimState.Upper_SkillA] = true;
            _canEnterAnimStates[(int)ECreatureAnimState.Upper_SkillB] = true;
            _canEnterAnimStates[(int)ECreatureAnimState.Upper_SkillC] = true;
        }

        public void ResetAllAnimations()
        {
            Debug.Log($"<color=red>Cancel Anim</color>");
            ReleaseAllAnimStates();
            ResetAllTriggers();
            Animator.Play(Upper_Idle);
        }

        // --- Anim States
        public readonly int Upper_Idle = Animator.StringToHash(CString.AnimState(EString.AnimState_Upper_Idle));
        public readonly int Upper_Move = Animator.StringToHash(CString.AnimState(EString.AnimState_Upper_Move));
        public readonly int Upper_SkillA = Animator.StringToHash(CString.AnimState(EString.AnimState_Upper_SkillA));

        public readonly int Upper_SkillB = Animator.StringToHash(CString.AnimState(EString.AnimState_Upper_SkillB));
        public readonly int Upper_SkillB_Elite = Animator.StringToHash(CString.AnimState(EString.AnimState_Upper_SkillB_Elite));
        
        public readonly int Upper_SkillC = Animator.StringToHash(CString.AnimState(EString.AnimState_Upper_SkillC));
        public readonly int Upper_CollectEnv = Animator.StringToHash(CString.AnimState(EString.AnimState_Upper_CollectEnv));
        public readonly int Upper_Dead = Animator.StringToHash(CString.AnimState(EString.AnimState_Upper_Dead));

        // --- Anim Params
        protected readonly int IsMoving = Animator.StringToHash(CString.AnimParam(EString.AnimParam_IsMoving));
        protected readonly int IsEliteMax = Animator.StringToHash(CString.AnimParam(EString.AnimParam_IsEliteMax));
        protected readonly int CanSkill = Animator.StringToHash(CString.AnimParam(EString.AnimParam_CanSkill));
        protected readonly int OnSkillA = Animator.StringToHash(CString.AnimParam(EString.AnimParam_OnSkillA));
        protected readonly int OnSkillB = Animator.StringToHash(CString.AnimParam(EString.AnimParam_OnSkillB));
        protected readonly int OnSkillC = Animator.StringToHash(CString.AnimParam(EString.AnimParam_OnSkillC));
        protected readonly int OnCollectEnv = Animator.StringToHash(CString.AnimParam(EString.AnimParam_OnCollectEnv));
        protected readonly int AttackRate = Animator.StringToHash(CString.AnimParam(EString.AnimParam_AttackRate));
        protected readonly int CollectRate = Animator.StringToHash(CString.AnimParam(EString.AnimParam_CollectRate));
        protected readonly int MovementSpeed = Animator.StringToHash(CString.AnimParam(EString.AnimParam_MovementSpeed));

        public bool Moving
        {
            get => Animator.GetBool(IsMoving);
            set => Animator.SetBool(IsMoving, value);
        }

        public bool ReadySkill
        {
            get => Animator.GetBool(CanSkill);
            set 
            {
                Animator.SetBool(CanSkill, value);
            }
        }

        public bool CanSkillTrigger { get; private set; } = false;
        
        public void Skill(ESkillType skillType)
        {
            // --- 임시 제거
            //ResetAllTriggers();
            CanSkillTrigger = false;
            switch (skillType)
            {
                case ESkillType.Skill_A:
                    {
                        Animator.SetTrigger(OnSkillA);
                        // Animator.ResetTrigger(OnSkillB);
                        // Animator.ResetTrigger(OnSkillC);
                        // Animator.ResetTrigger(OnCollectEnv);
                    }
                    break;

                case ESkillType.Skill_B:
                    {
                        Animator.SetTrigger(OnSkillB);
                        // Animator.ResetTrigger(OnSkillA);
                        // Animator.ResetTrigger(OnSkillC);
                        // Animator.ResetTrigger(OnCollectEnv);
                    }
                    break;

                case ESkillType.Skill_C:
                    {
                        Animator.SetTrigger(OnSkillC);
                        // Animator.ResetTrigger(OnSkillA);
                        // Animator.ResetTrigger(OnSkillB);
                        // Animator.ResetTrigger(OnCollectEnv);
                    }
                    break;
            }
        }

        public void CollectEnv()
        {
            //EnteredAnimState(ECreatureAnimState.Upper_CollectEnv);
            Animator.SetTrigger(OnCollectEnv);
            Animator.ResetTrigger(OnSkillA);
            Animator.ResetTrigger(OnSkillB);
            Animator.ResetTrigger(OnSkillC);
        }

        private void ResetAllTriggers()
        {
            Animator.ResetTrigger(OnSkillA);
            Animator.ResetTrigger(OnSkillB);
            Animator.ResetTrigger(OnSkillC);
            Animator.ResetTrigger(OnCollectEnv);
            CanSkillTrigger = true;
        }

        public void SetAttackRate(float attackRate)
        {
            float minAttackRate = CFloat.Min(EFloat.Range_AttackRate);
            float maxAttackRate = CFloat.Max(EFloat.Range_AttackRate);

            float minAttackAnimRate = CFloat.Min(EFloat.Range_AttackAnimRate);
            float maxAttackAnimRate = CFloat.Max(EFloat.Range_AttackAnimRate);

            float normalize = Mathf.InverseLerp(minAttackRate, maxAttackRate, attackRate);
            float result = Mathf.Lerp(minAttackAnimRate, maxAttackAnimRate, normalize);
            Animator.SetFloat(AttackRate, result);
        }

        public void SetCollectRate(float collectRate)
        {
            float minCollectRate = CFloat.Min(EFloat.Range_CollectRate);
            float maxCollectRate = CFloat.Max(EFloat.Range_CollectRate);

            float minCollectAnimRate = CFloat.Min(EFloat.Range_CollectAnimRate);
            float maxCollectAnimRate = CFloat.Max(EFloat.Range_CollectAnimRate);

            float normalize = Mathf.InverseLerp(minCollectRate, maxCollectRate, collectRate);
            float result = Mathf.Lerp(minCollectAnimRate, maxCollectAnimRate, normalize);
            Animator.SetFloat(CollectRate, result);
        }

        public void SetMovementSpeed(float movementSpeed)
        {
            float minMovementSpeed = CFloat.Min(EFloat.Range_MovementSpeed);
            float maxMovementSpeed = CFloat.Max(EFloat.Range_MovementSpeed);

            float minMovementAnimSpeed = CFloat.Min(EFloat.Range_MovementAnimSpeed);
            float maxMovementAnimSpeed = CFloat.Max(EFloat.Range_MovementAnimSpeed);

            float normalize = Mathf.InverseLerp(minMovementSpeed, maxMovementSpeed, movementSpeed);
            float result = Mathf.Lerp(minMovementAnimSpeed, maxMovementAnimSpeed, normalize);
            Animator.SetFloat(MovementSpeed, result);
        }

        public void SetEliteMax()
        {
            Animator.SetBool(IsEliteMax, true);
        }

        #region Init Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _creatureAnimCallback = GetComponent<CreatureAnimationCallback>();
            _canEnterAnimStates = new bool[(int)ECreatureAnimState.Max];
            ReleaseAllAnimStates();
            return true;
        }

        public override void InitialSetInfo(int dataID, BaseCellObject owner)
        {
            base.InitialSetInfo(dataID, owner);
            _creatureOwner = owner as Creature;
            _creatureAnimCallback.InitialSetInfo(owner: _creatureOwner);
            CanSkillTrigger = true;
        }

        public override void EnterInGame()
        {
            //AddAnimClipEvents();
            RefreshAnimEventHandlers();
            AddAnimStateEvents();
        }
        #endregion

        #region Anim Clip Callbacks
        private void AddAnimClipEvents()
        {
            for (int i = 0; i < _creatureOwner.CreatureSkill.SkillArray.Length; ++i)
            {
                SkillBase skill = _creatureOwner.CreatureSkill.SkillArray[i];
                if (skill != null)
                {
                    _creatureAnimCallback.OnSkillHandler -= skill.OnSkillCallback;
                    _creatureAnimCallback.OnSkillHandler += skill.OnSkillCallback;
                }
            }

            _creatureAnimCallback.OnCollectEnvHandler -= OnCollectEnvCallback;
            _creatureAnimCallback.OnCollectEnvHandler += OnCollectEnvCallback;

            _creatureAnimCallback.OnDustEffectHandler -= OnDustEffectCallback;
            _creatureAnimCallback.OnDustEffectHandler += OnDustEffectCallback;
        }

        public void RemoveOnSkillEventHandler(SkillBase skill)
            => _creatureAnimCallback.OnSkillHandler -= skill.OnSkillCallback;

        public void RefreshAnimEventHandlers()
        {
            if (_creatureOwner == null || _creatureAnimCallback == null)
                return;

            SkillBase[] skills = _creatureOwner.CreatureSkill.SkillArray;
            if (skills == null)
            {
                Debug.LogError("Creatute must have Skill_A at least.");
                return;
            }

            // --- Remove
            for (int i = 0; i < skills.Length; ++i)
            {
                if (skills[i] != null)
                    _creatureAnimCallback.OnSkillHandler -= skills[i].OnSkillCallback;
            }

            _creatureAnimCallback.OnCollectEnvHandler -= OnCollectEnvCallback;
            _creatureAnimCallback.OnDustEffectHandler -= OnDustEffectCallback;

            // --- Add Again
            for (int i = 0; i < skills.Length; ++i)
            {
                if (skills[i] != null)
                    _creatureAnimCallback.OnSkillHandler += skills[i].OnSkillCallback;
            }

            _creatureAnimCallback.OnCollectEnvHandler += OnCollectEnvCallback;
            _creatureAnimCallback.OnDustEffectHandler += OnDustEffectCallback;
        }

        public virtual void OnCollectEnvCallback() { }
        public virtual void OnDustEffectCallback() { }
        #endregion

        #region Anim State Events
        private void AddAnimStateEvents()
        {
            CreatureStateMachine[] creatureStateMachines = Animator.GetBehaviours<CreatureStateMachine>();
             for (int i = 0; i < creatureStateMachines.Length; ++i)
             {
                creatureStateMachines[i].OnAnimStateEnterHandler -= OnAnimStateEnter;
                creatureStateMachines[i].OnAnimStateEnterHandler += OnAnimStateEnter;

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

            // --- *** 모든 스킬은 반드시 종료 이후, 또는 중간에 끊겼을 경우 반드시 UpperStateEnter로 경유해야함. ***
            // --- *** (나중에 Dead State도 반드시 Idle -> Dead로 갈 수 있도록 할 것) ***
            ReleaseAllSkillsAnimStates();
            ResetAllTriggers();
            OnSkillExit(_creatureOwner.CreatureSkill.CurrentSkillType);
        }
            
        protected virtual void OnUpperMoveEnter()
        {
            EnteredAnimState(ECreatureAnimState.Upper_Move);
        }
        protected virtual void OnUpperSkillAEnter()
        {
            EnteredAnimState(ECreatureAnimState.Upper_SkillA);
            _creatureOwner.CreatureSkill.OnSkillEnter(ESkillType.Skill_A);
        }
        protected virtual void OnUpperSkillBEnter()
        {
            EnteredAnimState(ECreatureAnimState.Upper_SkillB);
            _creatureOwner.CreatureSkill.OnSkillEnter(ESkillType.Skill_B);
        }

        protected virtual void OnUpperSkillCEnter()
        {
            EnteredAnimState(ECreatureAnimState.Upper_SkillC);
            _creatureOwner.CreatureSkill.OnSkillEnter(ESkillType.Skill_C);
        }

        protected virtual void OnUpperCollectEnvEnter()
            => EnteredAnimState(ECreatureAnimState.Upper_CollectEnv);
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

        // ---> OnUpperIdleEnter
        protected virtual void OnUpperSkillAExit() 
        {
            // ReleaseAnimState(ECreatureAnimState.Upper_SkillA);
            // _creatureOwner.CreatureSkill.OnSkillStateExit(ESkillType.Skill_A);
        }
        protected virtual void OnUpperSkillBExit() 
        {
            // ReleaseAnimState(ECreatureAnimState.Upper_SkillB);
            // _creatureOwner.CreatureSkill.OnSkillStateExit(ESkillType.Skill_B);
        }
        protected virtual void OnUpperSkillCExit() 
        {
            // ReleaseAnimState(ECreatureAnimState.Upper_SkillC);
            // _creatureOwner.CreatureSkill.OnSkillStateExit(ESkillType.Skill_C);
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

        private void OnSkillExit(ESkillType skillType)
        {
            if (skillType == ESkillType.None)
                return;

            _creatureOwner.CreatureSkill.OnSkillExit(skillType);
        }
    }
}
