using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAnimationCallback : MonoBehaviour
    {
        public Creature Owner { get; private set; } = null;
        public event System.Action OnCreatureStateCallbackHandler = null;

        public void SetInfo(Creature owner) => this.Owner = owner;
        public void OnCreatureStateCallback()
        {
            OnCreatureStateCallbackHandler?.Invoke();
        }
    }
}
