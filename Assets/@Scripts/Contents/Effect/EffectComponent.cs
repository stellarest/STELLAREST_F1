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
        // [field: SerializeField] public List<EffectBase> ActiveEffects { get; private set; } = new List<EffectBase>();
        // --- 다시 ReadOnly Property로
        public List<EffectBase> ActiveEffects { get; } = new List<EffectBase>();

#if UNITY_EDITOR
        [Header("Effects")] public List<string> Dev_ActiveEffects = null;
#endif
        public void InitialSetInfo(BaseObject owner)
        {
            _owner = owner.GetComponent<BaseCellObject>();
            Dev_ActiveEffects = new List<string>();
        }

        public void EnterInGame() { } // RemoveAllActiveEffects(); + Apply Base Effect

        public EffectBase GenerateGlobalEffect(EInt eInt, Vector3 spawnPos)
        {
            EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                        objectType: EObjectType.Effect,
                        spawnPos: spawnPos,
                        dataID: CInt.ID(eInt),
                        owner: _owner
                    );

            // TEMP: 일단 EEffectClearType.TimeOut 일때만
            if (effect != null && effect.EffectClearType == EEffectClearType.TimeOut)
                effect.ApplyEffect();

            return effect;
        }

        public EffectBase GenerateSkillEffect(int effectID, SkillBase skill)
        {
            if (skill == null)
                return null;

            EffectData data = Util.GetEffectData(effectID, _owner);
            if (data == null)
                return null;

            Vector3 spawnPos = GetSkillEffectSpawnPos(skill, data.EffectSpawnType);
            EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
                        objectType: EObjectType.Effect,
                        spawnPos: spawnPos,
                        dataID: effectID,
                        owner: _owner
                    );

            ActiveEffects.Add(effect);
#if UNITY_EDITOR
            Dev_ActiveEffects.Add(effect.Dev_NameTextID);
#endif

            effect.SetSkill(skill);
            effect.ApplyEffect();
            return effect;
        }

        private Vector3 GetSkillEffectSpawnPos(SkillBase skill, EEffectSpawnType effectSpawnType)
        {
            return effectSpawnType switch
            {
                //EEffectSpawnType.SetParentOwner => _owner.CenterLocalPosition,  // ---> TEMP
                EEffectSpawnType.None or EEffectSpawnType.SetParentOwner => Vector3.zero,
                EEffectSpawnType.SkillFromOwner => skill.EnteredOwnerPos,
                EEffectSpawnType.SkillFromTarget => skill.EnteredTargetPos,
                _ => throw new ArgumentOutOfRangeException(nameof(GetSkillEffectSpawnPos), $"Invalid value type: {effectSpawnType}")
            };
        }

        public float GetEffectStatModifier(int effectID, EStatModType statModType)
        {
            float value = 0.0f;
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                EffectBase effect = ActiveEffects[i];
                if (effect.DataTemplateID != effectID)
                    continue;

                if (Util.IsEffectStatType(effect.EffectType) == false)
                    continue;

                switch (statModType)
                {
                    case EStatModType.AddAmount:
                        return value += effect.EffectData.AddAmount;

                    case EStatModType.AddPercent:
                        return value += effect.EffectData.AddPercent;

                    case EStatModType.AddPercentMulti:
                        return value += effect.EffectData.AddPercentMulti;
                }
            }

            return value;
        }

        // 애초에 이거 자체가 잘못된 것 같은데
        // public float GetStatModifier(EEffectType effectType, EStatModType statModType)
        // {
        //     float value = 0.0f;
        //     for (int i = 0; i < ActiveEffects.Count; ++i)
        //     {
        //         if (ActiveEffects[i].EffectType != effectType)
        //             continue;

        //         if (Util.IsEffectStatType(effectType) == false)
        //             continue;

        //         switch (statModType)
        //         {
        //             case EStatModType.AddAmount:
        //                 return value += ActiveEffects[i].EffectData.AddAmount;

        //             case EStatModType.AddPercent:
        //                 return value += ActiveEffects[i].EffectData.AddPercent;

        //             case EStatModType.AddPercentMulti:
        //                 return value += ActiveEffects[i].EffectData.AddPercentMulti;
        //         }
        //     }

        //     return value;
        // }

        public EffectBase FindEffect(EEffectType effectType, int dataID)
            => ActiveEffects.Find(e => e.EffectType == effectType && e.DataTemplateID == dataID);

        public bool IsAppliedEffect(EEffectType effectType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                if (ActiveEffects[i].EffectType == effectType)
                    return true;
            }

            return false;
        }

        public void OnShowEffect(EEffectType effectType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                EffectBase effect = ActiveEffects[i];
                if (effect.EffectType == effectType)
                    effect.OnShowEffect();
            }
        }

        public void RemoveEffect(IEnumerable<int> effectIDs)
        {
            foreach (var effectID in effectIDs)
            {
                EffectBase effect = ActiveEffects.Find(e => e.DataTemplateID == effectID);
                if (effect != null)
                    RemoveEffect(effect);
            }
        }

        public void RemoveEffect(EEffectType effectType)
        {
            for (int i = 0; i < ActiveEffects.Count; ++i)
            {
                EffectBase effect = ActiveEffects[i];
                if (effect.EffectType == effectType)
                {
                    RemoveEffect(effect);
                    return;
                }
            }
        }

        public void RemoveEffect(EffectBase effect)
        {
            if (effect.IsValid() == false)
                return;

            effect.ExitEffect();
        }
    }
}

/*      TT
        // ---------------------------------------------
        // -------------------- CUT --------------------
        // --------------------------------------------- 
        public void RemoveEffect(EffectBase effect)
        {
            if (effect.IsValid() == false)
                return;

            //ActiveEffects.Remove(effect);
            effect.ExitEffect();

// #if UNITY_EDITOR
//             Dev_ActiveEffects.Remove(effect.Dev_NameTextID);
// #endif
            // if (Util.IsEffectStatType(effect.EffectType))
            //     _owner.RefreshAllStats();

            // -------------------- CUT --------------------
            // if (effect.EffectClearType == EEffectClearType.ByCondition)
            // {
            //     effect.OnRemoveSelfByConditionHandler?.Invoke(() =>
            //     {
            //         if (effect.EffectData.PrefabLabel != null)
            //         {
            //             effect.transform.SetParent(Managers.Object.EffectRoot); //--- FORCE
            //             Managers.Object.Despawn(effect, effect.DataTemplateID);
            //         }
            //         else 
            //         {
            //             // --- Component만 제거 (DEPRECIATE)
            //             //UnityEngine.Object.Destroy(effect, Time.deltaTime);
            //         }
            //     });
            // }
            // else if (effect.EffectData.PrefabLabel != null)
            // {
            //     effect.transform.SetParent(Managers.Object.EffectRoot); //--- FORCE
            //     Managers.Object.Despawn(effect, effect.DataTemplateID);
            // }
            // else 
            // {
            //     // --- Component만 제거
            //     //UnityEngine.Object.Destroy(effect, Time.deltaTime);
            // }

            // -------------------- CUT --------------------
            // PREV
            //             if (effect.EffectClearType != EEffectClearType.ByCondition)
            //             {
            //                 ActiveEffects.Remove(effect);
            // #if UNITY_EDITOR
            //                 Dev_ActiveEffects.Remove(effect.Dev_NameTextID);
            // #endif

            //                 if (effect.EffectData.PrefabLabel != null)
            //                 {
            //                     effect.transform.SetParent(Managers.Object.EffectRoot); //--- FORCE
            //                     Managers.Object.Despawn(effect, effect.DataTemplateID);
            //                 }
            //                 else if (effect.Owner != null)
            //                 {
            //                     Debug.Log($"<color=red>REMOVE: {effect.Dev_NameTextID}</color>");
            //                     // --- Component만 제거
            //                     UnityEngine.Object.Destroy(effect, Time.deltaTime);
            //                 }
            //             }
            //             else
            //                 effect.OnRemoveSelfByConditionHandler?.Invoke(() =>
            //                 {
            //                     ActiveEffects.Remove(effect);
            // #if UNITY_EDITOR
            //                     Dev_ActiveEffects.Remove(effect.Dev_NameTextID);
            // #endif

            //                     if (effect.EffectData.PrefabLabel != null)
            //                     {
            //                         effect.transform.SetParent(Managers.Object.EffectRoot); //--- FORCE
            //                         Managers.Object.Despawn(effect, effect.DataTemplateID);
            //                     }
            //                     else if (effect.Owner != null)
            //                     {
            //                         Debug.Log($"<color=red>REMOVE: {effect.Dev_NameTextID}</color>");
            //                         // --- Component만 제거(아래 처럼 제거 or 단순 데이터 교체 방식으로)
            //                         UnityEngine.Object.Destroy(effect, Time.deltaTime);
            //                     }
            //                 });
        }

        // public EffectBase GenerateEffect(int effectID, SkillBase skill = null)
        // {
        //     EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
        //             objectType: EObjectType.Effect,
        //             spawnPos: _owner.CenterPosition, // --- Default Value
        //             dataID: effectID,
        //             owner: _owner
        //         );

        //     ActiveEffects.Add(effect);
        //     if (skill != null)
        //         effect.SetSkill(skill);
        //     effect.ApplyEffect();

        //     return effect;
        // }

        // public EffectBase GenerateEffect(int effectID, Vector3 spawnPos, SkillBase skill = null)
        // {
        //     EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
        //                         objectType: EObjectType.Effect,
        //                         spawnPos: spawnPos,
        //                         dataID: effectID,
        //                         owner: _owner
        //                         );

        //     ActiveEffects.Add(effect);
        //     if (skill != null)
        //         effect.SetSkill(skill);
        //     effect.ApplyEffect();
        //     return effect;
        // }

        // public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, SkillBase skill = null)
        // {
        //     List<EffectBase> generatedEffects = new List<EffectBase>();
        //     foreach (var id in effectIDs)
        //     {
        //         EffectBase effect = Managers.Object.SpawnBaseObject<EffectBase>(
        //             objectType: EObjectType.Effect,
        //             spawnPos: _owner.CenterPosition, // --- Default Value
        //             dataID: id,
        //             owner: _owner
        //         );

        //         generatedEffects.Add(effect);
        //         ActiveEffects.Add(effect);
        //         if (skill != null)
        //             effect.SetSkill(skill);
        //         effect.ApplyEffect();
        //     }

        //     return generatedEffects;
        // }

        // public List<EffectBase> GenerateEffects(IEnumerable<int> effectIDs, Vector3 spawnPos, SkillBase skill = null)
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
        //         if (skill != null)
        //             effect.SetSkill(skill);
        //         effect.ApplyEffect();
        //     }

        //     return generatedEffects;
        // }

        // public void RemoveBuffEffect(EEffectType effectType)
        // {
        //     for (int i = 0; i < ActiveEffects.Count; ++i)
        //     {
        //         if (Util.IsEffectStatType(effectType) == false)
        //             continue;

        //         if (ActiveEffects[i].EffectType == effectType)
        //         {
        //             BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
        //             if (buff != null)
        //                 RemoveEffect(buff);
        //         }
        //     }
        // }

        // public void SetIsOnEffectBuff(EEffectType effectBuffType, bool isOn)
        // {
        //     // if (Util.IsEffectBuffBastStat(effectBuffType) == false || Util.IsEffectBuffSubStat(effectBuffType) == false)
        //     //     return;
        //     if (Util.IsEffectStatType(effectBuffType) == false)
        //         return;

        //     // IsOnEffectBuffDict[effectBuffType] = isOn;
        // }

        // public bool IsOnEffectBuff(EEffectType buffType)
        //     => IsOnEffectBuffDict.TryGetValue(key: buffType, out bool isOnBuff) == false ? false : isOnBuff;

        // public void DoBuffEffect(EEffectType effectType) // NEW
        // {
        //     for (int i = 0; i < ActiveEffects.Count; ++i)
        //     {
        //         if (Util.IsEffectStatType(ActiveEffects[i].EffectType) == false)
        //             continue;

        //         if (ActiveEffects[i].EffectType == effectType)
        //         {
        //             BuffBase buff = ActiveEffects[i].GetComponent<BuffBase>();
        //             if (buff != null)
        //                 buff.DoEffect();
        //         }
        //     }
        // }

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
*/