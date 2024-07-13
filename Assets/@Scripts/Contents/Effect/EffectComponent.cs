using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class EffectComponent : InitBase
    {
        private Creature _owner = null;
        public List<EffectBase> ActiveEffects = new List<EffectBase>();

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public void SetInfo(Creature owner) => this._owner = owner;

        public List<EffectBase> GenerateEffect(IEnumerable<int> effectIDs, EEffectSpawnType spawnType, BaseObject source)
        {
            List<EffectBase> generatedEffects = new List<EffectBase>();

            foreach (var id in effectIDs)
            {
                EffectBase effect = Managers.Object.Spawn<EffectBase>(EObjectType.Effect, dataID: id);

                ActiveEffects.Add(effect);
                generatedEffects.Add(effect);

                effect.SetInfo(id, _owner, source);
                effect.ApplyEffect();
            }

            return generatedEffects;
        }

        public void RemoveEffect(EffectBase effect)
        {
            ActiveEffects.Remove(effect);
            Managers.Object.Despawn(effect, effect.DataTemplateID);
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
