using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // 굳이 InitBase를 붙여야할까?
    public class BaseAnimation : InitBase
    {
        private BaseObject _owner = null;
        public BaseObject Owner
        {
            get => _owner;
            set
            {
                if (_owner == null)
                    _owner = value;
            }
        }

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

        public virtual void UpdateAnimation()
        {
        }

        public void PlayAnimation(string animName) // temp string
        {
            Debug.Log($"Play Animation : {animName}");
        }

        public void Flip(LookAtDirection lookAtDir)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = _originScaleX * (int)lookAtDir;
            transform.localScale = localScale;
        }
    }
}
