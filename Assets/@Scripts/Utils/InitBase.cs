using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class InitBase : MonoBehaviour
    {
#if UNITY_EDITOR
        public string Dev_TextID = null;
#endif

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
