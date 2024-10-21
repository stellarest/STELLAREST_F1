using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class InitBase : MonoBehaviour
    {
#if UNITY_EDITOR
        public string Dev_NameTextID = null;
        public string Dev_DescriptionTextID = null;
#endif

        private void Awake() 
            => Init();

        protected bool _awake = false;
        public virtual bool Init()
        {
            if (_awake)
                return false;

            _awake = true;

#if UNITY_EDITOR
            Dev_NameTextID = string.Empty;
            Dev_DescriptionTextID = string.Empty;
#endif
            return true;
        }
    }
}
