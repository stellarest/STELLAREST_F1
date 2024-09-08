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

        public EffectBase GenerateEffect(int effectID, SkillBase skill = null)
        {
            EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                    objectType: EObjectType.Effect,
                    spawnPos: _owner.CenterPosition, // --- Default Value
                    dataID: effectID,
                    owner: _owner
                    );

            ActiveEffects.Add(effect);
            if (skill != null)
                effect.SetSkill(skill);
            effect.ApplyEffect();

            return effect;
        }

        public EffectBase GenerateEffect(int effectID, Vector3 spawnPos, SkillBase skill = null)
        {
            EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                                objectType: EObjectType.Effect,
                                spawnPos: spawnPos,
                                dataID: effectID,
                                owner: _owner
                                );

            ActiveEffects.Add(effect);
            if (skill != null)
                effect.SetSkill(skill);
            effect.ApplyEffect();

            return effect;
        }

        public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, SkillBase skill = null)
        {
            List<EffectBase> generatedEffects = new List<EffectBase>();
            foreach (var id in effectIDs)
            {
                EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                    objectType: EObjectType.Effect,
                    spawnPos: _owner.CenterPosition, // --- Default Value
                    dataID: id,
                    owner: _owner
                );

                generatedEffects.Add(effect);
                ActiveEffects.Add(effect);
                if (skill != null)
                    effect.SetSkill(skill);
                effect.ApplyEffect();
            }

            return generatedEffects;
        }

        public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, Vector3 spawnPos, SkillBase skill = null)
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
                if (skill != null)
                    effect.SetSkill(skill);
                effect.ApplyEffect();
            }

            return generatedEffects;
        }

        public float ApplyStatModifier(EApplyStatType applyStatType, EStatModType statModType)
        {
            float value = 0.0f;
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectData.ApplyStatType != applyStatType)
                    continue;

                if (CanApplyStatEffectType(ActiveEffects[i].EffectType))
                {
                    value = statModType == EStatModType.AddAmount ? 
                            ActiveEffects[i].EffectData.AddAmount : ActiveEffects[i].EffectData.AddPercent;
                }
            }

            return value;
        }

        public void OnShowEffects(EEffectType effectType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectType != effectType)
                    continue;

                ActiveEffects[i].OnShowEffect();
            }
        }

        public void OnShowBuffEffects(EEffectBuffType effectBuffType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectType == EEffectType.Buff)
                {
                    BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
                    if (buff != null && buff.EffectBuffType == effectBuffType)
                        buff.OnShowEffect();
                }
            }
        }

        public void ExitShowEffects(EEffectType effectType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectType != effectType)
                    continue;

                ActiveEffects[i].ExitShowEffect();
            }
        }

        public void ExitShowBuffEffects(EEffectBuffType effectBuffType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectType == EEffectType.Buff)
                {
                    BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
                    if (buff != null && buff.EffectBuffType == effectBuffType)
                        buff.ExitShowEffect();
                }
            }
        }

        private bool CanApplyStatEffectType(EEffectType type)
            => type == EEffectType.Buff || type == EEffectType.DeBuff;

        public void RemoveEffect(EffectBase effect)
        {
            ActiveEffects.Remove(effect);
            Managers.Object.Despawn(effect, effect.DataTemplateID);
        }

        // public void ClearDebuffsBySkill()
        // {
        //     // .ToArray()로 해야할 이유 있음??
        //     foreach (var buff in ActiveEffects.ToArray())
        //     {
        //         if (buff.EffectType != EEffectType.Buff)
        //         {
        //             buff.ClearEffect(EEffectClearType.ClearSkill);
        //         }
        //     }
        // }
    }
}
