using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAnimationCallback : MonoBehaviour
    {
        public Creature Owner { get; private set; } = null;
        public void SetInfo(Creature owner) => this.Owner = owner;


        public event System.Action OnSkillCallbackHandler = null;
        public void OnSkillClipCallback() => OnSkillCallbackHandler?.Invoke();


        public event System.Action OnCollectEnvCallbackHandler = null;
        public void OnCollectEnvClipCallback() => OnCollectEnvCallbackHandler?.Invoke();
    }
}
