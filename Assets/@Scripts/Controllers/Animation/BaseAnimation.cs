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
        public BaseObject Owner { get; set; } = null;
        public T GetOwner<T>() where T : BaseObject => Owner as T;
        public Animator Animator { get; private set; } = null;
        //public AnimationClipCallback AnimClipCallback { get; private set; } = null;
   
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Animator = GetComponent<Animator>();
            //AnimClipCallback = GetComponent<AnimationClipCallback>();
            return true;
        }

        public virtual void InitialSetInfo(int dataID, BaseObject owner)
        {
            Owner = owner;
        }

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
