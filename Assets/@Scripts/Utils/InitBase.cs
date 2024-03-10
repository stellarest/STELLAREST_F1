using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class InitBase : MonoBehaviour
    {
        protected bool _init = false;

        public virtual bool Init()
        {
            // Init 실패
            if (_init)
                return false;

            // Init 완료
            _init = true;
            return true;
        }

        private void Awake() 
            => Init();
    }
}
