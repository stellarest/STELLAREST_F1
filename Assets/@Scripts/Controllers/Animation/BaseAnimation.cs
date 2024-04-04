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
        public AnimationCallback AnimCallback { get; private set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Animator = GetComponent<Animator>();
            AnimCallback = GetComponent<AnimationCallback>();
            //_originScaleX = transform.localScale.x;

            return true;
        }

        public virtual void SetInfo(int dataID, BaseObject owner)
            => Owner = owner;

        public bool IsPlay()
        {
            AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime > 0f)
                return true;
            else
                return false;
        }

        public virtual void UpdateAnimation() { }
        protected virtual void Idle() { }
        protected virtual void Move() { }
        protected virtual void Attack() { }
        protected virtual void SkillA() { }
        protected virtual void SkillB() { }
        protected virtual void Dead() { }
        public virtual void Flip(ELookAtDirection lookAtDir)
        {
            // Vector3 localScale = transform.localScale;
            // localScale.x = _originScaleX * (int)lookAtDir;
            // transform.localScale = localScale;
        }
    }
}
