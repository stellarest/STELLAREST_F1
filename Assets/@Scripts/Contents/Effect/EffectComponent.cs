using System;
using System.Collections;
using System.Collections.Generic;
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

        public List<EffectBase> GenerateEffect(IEnumerable<int> effectIDs, EEffectSpawnType spawnType)
        {
            List<EffectBase> generatedEffects = new List<EffectBase>();

            foreach (var id in effectIDs)
            {
                string className = Managers.Data.EffectDataDict[id].ClassName;
                System.Type effectType = System.Type.GetType(className);
                if (effectType == null)
                {
                    Debug.LogError($"{nameof(GenerateEffect)}");
                    return null;
                }

                GameObject go = Managers.Object.SpawnGameObject(_owner.CenterPosition, ReadOnly.Prefabs.PFName_EffectBase, id);
                go.name = Managers.Data.EffectDataDict[id].ClassName;
                
                EffectBase effect = go.AddComponent(effectType) as EffectBase;
                effect.transform.parent = _owner.CreatureEffect.transform; // --- 이거를 해야할까??
                effect.transform.localPosition = Vector3.zero;
                Managers.Object.Effects.Add(effect); // --- 임시

                ActiveEffects.Add(effect);
                generatedEffects.Add(effect);

                effect.SetInfo(id, _owner, spawnType);
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
