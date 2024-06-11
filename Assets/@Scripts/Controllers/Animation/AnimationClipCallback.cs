using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class AnimationClipCallback : MonoBehaviour
    {
        public event System.Action OnDustVFXHandler = null;
        public void OnDustVFX() => OnDustVFXHandler?.Invoke();
    }
}
