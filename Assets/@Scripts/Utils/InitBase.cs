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
            // Init 실패
            if (_init)
                return false;

            // Init 완료
            _init = true;
            return true;
        }

        private bool _initialSet = false;
        public virtual bool SetInfo(int dataID)
        {
            if (_initialSet)
                return false;

            _initialSet = true;
            return true;
        }

        public virtual bool SetInfo(BaseObject owner, List<int> dataIDs)
        {
            if (_initialSet)
                return false;

            _initialSet = true;
            return true;
        }

        protected virtual void EnterInGame() { }
    }
}
