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

        protected bool _awake = false;
        public virtual bool Init()
        {
            if (_awake)
                return false;

            _awake = true;
            return true;
        }
    }
}
