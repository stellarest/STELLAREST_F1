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
        private BaseObject _owner = null;
        public List<EffectBase> ActiveEffects = new List<EffectBase>();

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public void InitialSetInfo(BaseObject owner) => _owner = owner;

        public EffectBase GenerateEffect(int effectDataID, Vector3 spawnPos, Action startCallback = null)
        {
            EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                objectType: EObjectType.Effect,
                spawnPos: spawnPos,
                dataID: effectDataID,
                owner: _owner
            );

            startCallback?.Invoke();
            return effect;
        }

        public EffectBase GenerateEffect(int effectDataID, BaseObject source, Action startCallback = null)
        {
            EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                objectType: EObjectType.Effect,
                spawnPos: source.CenterPosition,
                dataID: effectDataID,
                owner: _owner
            );

            startCallback?.Invoke();
            return effect;
        }

        public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, Vector3 spawnPos, Action startCallback = null)
        {
            List<EffectBase> generatedEffects = new List<EffectBase>();
            foreach (var id in effectIDs)
            {
                EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                    objectType: EObjectType.Effect,
                    spawnPos: spawnPos,
                    dataID: id,
                    owner: _owner
                );

                startCallback?.Invoke();
                generatedEffects.Add(effect);
                ActiveEffects.Add(effect);
            }

            return generatedEffects;
        }

        public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, BaseObject source, Action startCallback = null)
        {
            List<EffectBase> generatedEffects = new List<EffectBase>();

            foreach (var id in effectIDs)
            {
                EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                    objectType: EObjectType.Effect,
                    spawnPos: source.CenterPosition,
                    dataID: id,
                    owner: _owner
                );

                startCallback?.Invoke();
                generatedEffects.Add(effect);
                ActiveEffects.Add(effect);

                // --- Prev
                // EffectBase effect = Managers.Object.Spawn<EffectBase>(EObjectType.Effect, dataID: id);
                // ActiveEffects.Add(effect);
                // generatedEffects.Add(effect);

                // effect.SetInfo(id, _owner, source);
                // effect.ApplyEffect();
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
