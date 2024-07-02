using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class EffectComponent : InitBase
    {
        public Creature Owner { get; private set; } = null;
        public List<EffectBase> ActiveEffects = new List<EffectBase>();

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public void SetOwner(Creature owner) => Owner = owner;

        public List<EffectBase> GenerateEffect(IEnumerable<int> effectIDs, EEffectSpawnType spawnType)
        {
            return null;
        }

        public void RemoveEffect(EffectBase effect)
        {
            ActiveEffects.Remove(effect);
            // Despawn Effect
            // effect.enabled = false; // Pulling으로 안했나보네
        }

        public void ClearDebuffsBySkill()
        {
            foreach (var buff in ActiveEffects.ToArray())
            {
                if (buff.EffectType != EEffectType.Buff)
                {
                    buff.ClearEffect(EEffectClearType.ClearSkill);
                }
            }
        }


    }
}
