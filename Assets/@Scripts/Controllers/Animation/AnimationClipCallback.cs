using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class AnimationClipCallback : MonoBehaviour
    {
        public event System.Action OnDustVFXHandler = null;
        public void OnDustVFX() => OnDustVFXHandler?.Invoke();
    }
}
