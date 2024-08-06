using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAnimationCallback : MonoBehaviour
    {
        public Creature Owner { get; private set; } = null;
        public void InitialSetInfo(Creature owner) => Owner = owner;


        public event System.Action OnDustEffectHandler = null;
        public void OnDustEffect() => OnDustEffectHandler?.Invoke();
        

        public event System.Action OnSkillHandler = null;
        public void OnSkill() => OnSkillHandler?.Invoke();


        public event System.Action OnCollectEnvHandler = null;
        public void OnCollectEnv() => OnCollectEnvHandler?.Invoke();


        // public event System.Action OnEnterLowerAnimIdleToSkillACallbackHandler = null;
        // public void OnEnterLowerAnimIdleToSkillA() => OnEnterLowerAnimIdleToSkillACallbackHandler?.Invoke();


        // public event System.Action OnExitLowerAnimIdleToSkillACallbackHandler = null;
        // public void OnExitLowerAnimIdleToSkillA() => OnExitLowerAnimIdleToSkillACallbackHandler?.Invoke();
    }
}
