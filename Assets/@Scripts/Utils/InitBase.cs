using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class InitBase : MonoBehaviour
    {
        private void Awake() 
            => Init();

        protected bool _init = false;
        public virtual bool Init()
        {
            if (_init)
                return false;

            _init = true;
            return true;
        }
    }
}
