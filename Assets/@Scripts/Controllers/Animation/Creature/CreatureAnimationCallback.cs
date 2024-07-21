using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAnimationCallback : MonoBehaviour
    {
        public Creature Owner { get; private set; } = null;
        public void InitialSetInfo(Creature owner) => this.Owner = owner;

        public event System.Action OnDustEffectHandler = null;
        public void OnDustEffect() => OnDustEffectHandler?.Invoke();
        

        public event System.Action OnSkillCallbackHandler = null;
        public void OnSkill() => OnSkillCallbackHandler?.Invoke();


        public event System.Action OnCollectEnvCallbackHandler = null;
        public void OnCollectEnv() => OnCollectEnvCallbackHandler?.Invoke();


        public event System.Action OnEnterLowerAnimIdleToSkillACallbackHandler = null;
        public void OnEnterLowerAnimIdleToSkillA() => OnEnterLowerAnimIdleToSkillACallbackHandler?.Invoke();


        public event System.Action OnExitLowerAnimIdleToSkillACallbackHandler = null;
        public void OnExitLowerAnimIdleToSkillA() => OnExitLowerAnimIdleToSkillACallbackHandler?.Invoke();
    }
}
