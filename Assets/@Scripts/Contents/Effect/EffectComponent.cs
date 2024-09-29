using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.UI;
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

        // --- 원래 ReadOnly Property였음. 일단 개발용으로 Property로 바꿈.
        [field: SerializeField] public List<EffectBase> ActiveEffects { get; private set; } = new List<EffectBase>();

        // --- 이게 필요할까
        public Dictionary<EEffectBuffType, bool> IsOnEffectBuffDict { get; private set; } = null;

        public void InitialSetInfo(BaseObject owner)
        {
            _owner = owner.GetComponent<BaseCellObject>();
            IsOnEffectBuffDict = new Dictionary<EEffectBuffType, bool>();
        }

        public void EnterInGame()
        {
            RemoveAllEffects();
            // + Apply Base Effect
        }

        public void SetIsOnEffectBuff(EEffectBuffType buffType, bool isOn)
            => IsOnEffectBuffDict[buffType] = isOn;

        public bool IsOnEffectBuff(EEffectBuffType buffType)
        {
            if (IsOnEffectBuffDict.TryGetValue(key: buffType, out bool isOn) == false)
                return false;

            return isOn;
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

        public float GetStatModifier(EApplyStatType applyStatType, EStatModType statModType)
        {
            float value = 0.0f;
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectData.ApplyStatType != applyStatType)
                    continue;

                if (CanApplyStatEffectType(ActiveEffects[i].EffectType))
                {
                    switch (statModType)
                    {
                        case EStatModType.AddAmount:
                            value += ActiveEffects[i].EffectData.AddAmount;
                            break;

                        case EStatModType.AddPercent:
                            value += ActiveEffects[i].EffectData.AddPercent;
                            break;

                        case EStatModType.AddPercentMulti:
                            value += ActiveEffects[i].EffectData.AddPercentMulti;
                            break;
                    }

                    // value = statModType == EStatModType.AddAmount ? 
                    //         ActiveEffects[i].EffectData.AddAmount : ActiveEffects[i].EffectData.AddPercent;
                }
            }

            return value;
        }

        public void DoBuffEffects(EEffectBuffType effectBuffType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectType == EEffectType.Buff)
                {
                    BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
                    if (buff != null && buff.EffectBuffType == effectBuffType)
                        buff.DoEffect();
                }
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
                        buff.ExitEffect();
                }
            }
        }

        private bool CanApplyStatEffectType(EEffectType type)
            => type == EEffectType.Buff || type == EEffectType.DeBuff;

        public void RemoveEffect(EffectBase effect)
        {
            if (effect.IsValid() == false)
                return;

            effect.ExitEffect();
            if (effect.EffectClearType != EEffectClearType.ByCondition)
            {
                ActiveEffects.Remove(effect);
                effect.transform.SetParent(Managers.Object.EffectRoot); //--- FORCE
                Managers.Object.Despawn(effect, effect.DataTemplateID);
            }
            else
                effect.OnRemoveSelfByConditionHandler?.Invoke(() =>
                {
                    ActiveEffects.Remove(effect);
                    effect.transform.SetParent(Managers.Object.EffectRoot); //--- FORCE
                    Managers.Object.Despawn(effect, effect.DataTemplateID);
                    Debug.Log($"<color=yellow>REMOVE: {effect.Dev_NameTextID}</color>");
                });
        }

        public void RemoveBuffEffects(EEffectBuffType buffType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectType == EEffectType.Buff)
                {
                    BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
                    if (buff != null && buff.EffectBuffType == buffType)
                        this.RemoveEffect(buff);
                }
            }
        }

        public void RemoveAllEffects()
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                ActiveEffects.Remove(ActiveEffects[i]);
                Managers.Object.Despawn(ActiveEffects[i], ActiveEffects[i].DataTemplateID);
            }
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
