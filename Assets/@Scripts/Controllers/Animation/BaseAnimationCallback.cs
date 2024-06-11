using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class BaseAnimationCallback : MonoBehaviour
    {
        public event System.Action OnDustVFXHandler = null;
        public void OnDustVFX() => OnDustVFXHandler?.Invoke();
    }
}
