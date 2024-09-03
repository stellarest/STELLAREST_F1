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
        //private Creature _owner = null;
        public BaseObject Owner { get; private set; } = null;
        public List<EffectBase> ActiveEffects { get; } = new List<EffectBase>();
        // public override bool Init()
        // {
        //     if (base.Init() == false)
        //         return false;

        //     return true;
        // }

        public void InitialSetInfo(BaseObject owner) => Owner = owner;

        public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, Vector3 spawnPos, Action startCallback = null)
        {
            List<EffectBase> generatedEffects = new List<EffectBase>();
            foreach (var id in effectIDs)
            {
                EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                    objectType: EObjectType.Effect,
                    spawnPos: spawnPos,
                    dataID: id,
                    owner: Owner
                );

                generatedEffects.Add(effect);
                ActiveEffects.Add(effect);
            }

            startCallback?.Invoke();
            return generatedEffects;
        }

        public void RemoveEffect(EffectBase effect)
        {
            ActiveEffects.Remove(effect);
            Managers.Object.Despawn(effect, effect.DataTemplateID);
        }

        public void ClearDebuffsBySkill()
        {
            // .ToArray()로 해야할 이유 있음??
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
