using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STELLAREST_F1.Data;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    /// <summary>
    /// All of BaseCellObject must have this component.
    /// </summary> <summary>
    public class EffectComponent : InitBase
    {
        private const string EffectPoolingRootName = "";
        private BaseCellObject _owner = null;

        // --- 원래 ReadOnly Property였음. 일단 개발용으로 Property로 바꿈.
        [field: SerializeField] public List<EffectBase> ActiveEffects { get; private set; } = new List<EffectBase>();

        // EEffectType: Buff, Debuff, CC, Dot..
        public Dictionary<EEffectType, bool> IsOnEffectBuffDict { get; private set; } = null;

        public void InitialSetInfo(BaseObject owner)
        {
            ActiveEffects = ActiveEffects == null ? new List<EffectBase>() : ActiveEffects;
            _owner = owner.GetComponent<BaseCellObject>();
            IsOnEffectBuffDict = new Dictionary<EEffectType, bool>();
        }

        public void EnterInGame()
        {
            // RemoveAllActiveEffects();
            // + Apply Base Effect
        }

        public EffectBase FindPrevEffect(int dataID)
        {
            if (ActiveEffects.Count == 0)
                return null;

            // dataID - 10까지 강제로 찾음
            for (int i = dataID; i > dataID - 10; --i)
            {
                EffectBase prevEffect = ActiveEffects.Find(e => e.EffectData.DataID == i);
                if (prevEffect != null)
                    return prevEffect;
            }

            return null;
        }

        public void SetIsOnEffectBuff(EEffectType effectBuffType, bool isOn)
        {
            if (Util.IsEffectBuffBastStat(effectBuffType) == false || Util.IsEffectBuffSubStat(effectBuffType) == false)
                return;

            IsOnEffectBuffDict[effectBuffType] = isOn;
        }

        public bool IsOnEffectBuff(EEffectType buffType)
            => IsOnEffectBuffDict.TryGetValue(key: buffType, out bool isOnBuff) == false ? false : isOnBuff;

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

        // public float GetStatModifier(EApplyStatType applyStatType, EStatModType statModType) // PREV
        // {
        //     float value = 0.0f;
        //     for (int i = 0; i < ActiveEffects.Count; ++i)
        //     {
        //         if (ActiveEffects[i].EffectData.ApplyStatType != applyStatType)
        //             continue;

        //         if (CanApplyStatEffectType(ActiveEffects[i].EffectType))
        //         {
        //             switch (statModType)
        //             {
        //                 case EStatModType.AddAmount:
        //                     value += ActiveEffects[i].EffectData.AddAmount;
        //                     break;

        //                 case EStatModType.AddPercent:
        //                     value += ActiveEffects[i].EffectData.AddPercent;
        //                     break;

        //                 case EStatModType.AddPercentMulti:
        //                     value += ActiveEffects[i].EffectData.AddPercentMulti;
        //                     break;
        //             }

        //             // value = statModType == EStatModType.AddAmount ? 
        //             //         ActiveEffects[i].EffectData.AddAmount : ActiveEffects[i].EffectData.AddPercent;
        //         }
        //     }

        //     return value;
        // }

        public float GetStatModifier(EEffectType effectType, EStatModType statModType) // NEW
        {
            float value = 0.0f;
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectType != effectType)
                    continue;

                if (Util.IsEffectStatType(effectType) == false)
                    continue;

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
            }

            return value;
        }

        // public void DoBuffEffect(EEffectType effectBuffType) // PREV
        // {
        //     for (int i = 0; i < ActiveEffects.Count; ++i)
        //     {
        //         if (Util.IsEffectBuffType(ActiveEffects[i].EffectType) == false)
        //             continue;

        //         if (ActiveEffects[i].EffectType == effectBuffType)
        //         {
        //             BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
        //             if (buff != null)
        //                 buff.DoEffect();
        //         }
        //     }
        // }

        public void DoBuffEffect(EEffectType effectType) // NEW
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (Util.IsEffectStatType(ActiveEffects[i].EffectType) == false)
                    continue;

                if (ActiveEffects[i].EffectType == effectType)
                {
                    BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
                    if (buff != null)
                        buff.DoEffect();
                }
            }
        }

        // public void RemoveBuffEffect(EEffectType effectBuffType, bool destroyOrigin = false) // PREV
        // {
        //     for (int i = 0; i < ActiveEffects.Count; ++i)
        //     {
        //         if (Util.IsEffectBuffType(ActiveEffects[i].EffectType) == false)
        //             continue;

        //         if (ActiveEffects[i].EffectType == effectBuffType)
        //         {
        //             BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
        //             if (buff != null)
        //                 RemoveEffect(effect: buff, destroyOrigin: destroyOrigin);
        //         }
        //     }   
        // }

        public void RemoveBuffEffect(EEffectType effectType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (Util.IsEffectStatType(effectType) == false)
                    continue;

                if (ActiveEffects[i].EffectType == effectType)
                {
                    BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
                    if (buff != null)
                        RemoveEffect(buff);
                }
            }
        }

        // public void RemoveBuffEffects(EEffectBuffType buffType, bool destroyPooling = false)
        // {
        //     for (int i = 0; i < ActiveEffects.Count; ++i)
        //     {
        //         if (ActiveEffects[i].EffectType == EEffectType.Buff)
        //         {
        //             BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
        //             if (buff != null && buff.EffectBuffType == buffType)
        //                 this.RemoveEffect(effect: buff, destroyPooling: destroyPooling);
        //         }
        //     }
        // }

        // private bool CanApplyStatEffectType(EEffectType effectType)
        //     => Util.IsEffectBuffType(effectType); // || Util.IsEffectDebuffType(effectType)

        public void RemoveEffect(EffectBase effect)
        {
            if (effect.IsValid() == false)
                return;

            effect.ExitEffect();
            if (effect.EffectClearType != EEffectClearType.ByCondition)
            {
                ActiveEffects.Remove(effect);

                if (effect.EffectData.PrefabLabel != null)
                {
                    effect.transform.SetParent(Managers.Object.EffectRoot); //--- FORCE
                    Managers.Object.Despawn(effect, effect.DataTemplateID);
                }
                else if (effect.Owner != null)
                {
                    // --- Component만 제거
                    UnityEngine.Object.Destroy(effect, Time.deltaTime);
                }
            }
            else
                effect.OnRemoveSelfByConditionHandler?.Invoke(() =>
                {
                    ActiveEffects.Remove(effect);
                    if (effect.EffectData.PrefabLabel != null)
                    {
                        effect.transform.SetParent(Managers.Object.EffectRoot); //--- FORCE
                        Managers.Object.Despawn(effect, effect.DataTemplateID);
                    }
                    else if (effect.Owner != null)
                    {
                        // --- Component만 제거
                        // 잘 되긴 하는데 제거가 아닌 단순 데이터 교체방식으로 해야할까? 이대로 해도 되긴 함
                        UnityEngine.Object.Destroy(effect, Time.deltaTime);
                    }
                });
        }

        public void RemoveAllActiveEffects()
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
