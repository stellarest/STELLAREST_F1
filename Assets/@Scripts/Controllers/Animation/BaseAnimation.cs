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
        private float _originScaleX = 0f;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Animator = GetComponent<Animator>();
            AnimCallback = GetComponent<AnimationCallback>();
            _originScaleX = transform.localScale.x;

            return true;
        }

        public virtual void SetInfo(int dataID, BaseObject owner)
            => Owner = owner;

        public virtual void UpdateAnimation() { }
        protected virtual void Idle() { }
        protected virtual void Move() { }
        protected virtual void Dead() { }
        public void Flip(LookAtDirection lookAtDir)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = _originScaleX * (int)lookAtDir;
            transform.localScale = localScale;
        }
    }
}
