using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using UnityEngine;

namespace STELLAREST_F1
{
    public class BaseAnimation : InitBase
    {
        public Animator Animator { get; private set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Animator = GetComponent<Animator>();
            return true;
        }

        protected virtual void UpdateAnimation()
        {
        }

        public void PlayAnimation()
        {
        }

        public void Flip()
        {
        }
    }
}
