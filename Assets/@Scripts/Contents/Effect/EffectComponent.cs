using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    /// <summary>
    /// All of BaseCellObject have this component.
    /// </summary> <summary>
    public class EffectComponent : InitBase
    {
        private const string EffectPoolingRootName = "";
        private BaseCellObject _owner = null;
        public List<EffectBase> ActiveEffects { get; } = new List<EffectBase>();
        private Transform EffectPoolingRoot
        {
            get => null;
        }

        public void InitialSetInfo(BaseObject owner)
        {
            _owner = owner.GetComponent<BaseCellObject>();
        }

        public EffectBase GenerateEffect(int effectID)
        {
            EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                    objectType: EObjectType.Effect,
                    spawnPos: _owner.CenterPosition,
                    dataID: effectID,
                    owner: _owner
                    );

            ActiveEffects.Add(effect);
            return effect;
        }

        public EffectBase GenerateEffect(int effectID, Vector3 spawnPos)
        {
            EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                                objectType: EObjectType.Effect,
                                spawnPos: spawnPos,
                                dataID: effectID,
                                owner: _owner
                                );

            ActiveEffects.Add(effect);
            return effect;
        }

        public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs)
        {
            List<EffectBase> generatedEffects = new List<EffectBase>();
            foreach (var id in effectIDs)
            {
                EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                    objectType: EObjectType.Effect,
                    spawnPos: _owner.CenterPosition,
                    dataID: id,
                    owner: _owner
                );

                generatedEffects.Add(effect);
                ActiveEffects.Add(effect);
            }

            return generatedEffects;
        }

        public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, Vector3 spawnPos)
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

                generatedEffects.Add(effect);
                ActiveEffects.Add(effect);
            }

            return generatedEffects;
        }

        // public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, Vector3 spawnPos, Action startCallback = null)
        // {
        //     List<EffectBase> generatedEffects = new List<EffectBase>();
        //     foreach (var id in effectIDs)
        //     {
        //         EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
        //             objectType: EObjectType.Effect,
        //             spawnPos: spawnPos,
        //             dataID: id,
        //             owner: _owner
        //         );

        //         generatedEffects.Add(effect);
        //         ActiveEffects.Add(effect);
        //     }

        //     startCallback?.Invoke();
        //     return generatedEffects;
        // }

        // public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, BaseObject owner, EEffectSpawnType effectSpawnType)
        // {
        //     if (owner.IsValid() == false)
        //         return null;

        //     List<EffectBase> generatedEffects = new List<EffectBase>();
        //     foreach (var id in effectIDs)
        //     {
        //         EffectData data = Managers.Data.EffectDataDict[id];
        //         // EEffectType type = data.EffectType;

        //         EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
        //             objectType: EObjectType.Effect,
        //             spawnPos: owner.CenterPosition,
        //             dataID: id
        //         );

        //         generatedEffects.Add(effect);
        //         ActiveEffects.Add(effect);
        //     }

        //     return null;
        // }

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
