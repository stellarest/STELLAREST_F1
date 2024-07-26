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
        private BaseObject _baseOwner = null;
        public virtual BaseObject Owner => _baseOwner;
        
        public Animator Animator { get; private set; } = null;
   
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Animator = GetComponent<Animator>();
            return true;
        }

        public virtual void InitialSetInfo(int dataID, BaseObject owner)
            => _baseOwner = owner;

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

        public virtual void Flip(ELookAtDirection lookAtDir) { }
    }
}
