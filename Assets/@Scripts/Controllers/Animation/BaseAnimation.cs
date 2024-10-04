using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using Unity.Profiling;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BaseAnimation : InitBase
    {
        protected BaseCellObject _owner = null;
        // protected bool IsValidOwner => _baseOwner.IsValid();
        // protected bool IsValidTarget => _baseOwner.Target.IsValid();
        
        public Animator Animator { get; private set; } = null;
   
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Animator = GetComponent<Animator>();
            return true;
        }

        public virtual void InitialSetInfo(int dataID, BaseCellObject owner)
            => _owner = owner;

        public virtual void EnterInGame() { }

        public bool IsPlay()
        {
            AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime > 0f)
                return true;
            else
                return false;
        }

        // --- Parameters
        protected readonly int OnDead = Animator.StringToHash(ReadOnly.AnimationParams.OnDead);
        
        public void Dead() => Animator.SetTrigger(OnDead);

        // --- A*랑 같이 Creature로 옮겨야함.
        public virtual void Flip(ELookAtDirection lookAtDir) { }
    }
}
