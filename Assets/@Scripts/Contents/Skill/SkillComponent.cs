using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;

namespace STELLAREST_F1
{
    public class SkillComponent : InitBase
    {
        private Creature _owner = null;

        [field: SerializeField] public List<SkillBase> ActiveSkills { get; private set; } = new List<SkillBase>();
        [field: SerializeField] public SkillBase[] SkillArray { get; private set; } = new SkillBase[(int)ESkillType.Max];

#if UNITY_EDITOR
        public string DefaultSkillA = "";
        public string ActiveSkillB = "";
        public string ActiveSkillC = "";
#endif
        // --- *** Projectile: FindSkill에서 참고하고 있는거 고작 하나 때문에 이 컨테이너는 필요 없을지도
        // public SkillBase FindSkill(int skillDataID) 
        //     => Skills.FirstOrDefault(s => s.DataTemplateID == skillDataID);
        // public SkillBase FindSkill(ESkillType skillType)
        //     => Skills.FirstOrDefault(s => s.SkillType == skillType);

        // public SkillBase[] SkillArray { get; private set; } = new SkillBase[(int)ESkillType.Max]; // --- Caching

        [field: SerializeField] public ESkillType CurrentSkillType { get; set; } = ESkillType.None;
        public SkillBase CurrentSkill
        {
            get
            {
                if (CurrentSkillType == ESkillType.None)
                    return null;

                return SkillArray[(int)CurrentSkillType];
            }
        }

        public SkillBase ReadyToActivate
        {
            get
            {
                if (ActiveSkills.Count == 0)
                    return SkillArray[(int)ESkillType.Skill_A];

                int rand = UnityEngine.Random.Range(0, ActiveSkills.Count);
                SkillBase getSkill = ActiveSkills[rand];
                if (getSkill.RemainCoolTime > 0f)
                {
                    getSkill = ActiveSkills[rand == 0 ? ++rand : --rand];
                    if (getSkill.RemainCoolTime > 0f)
                        return SkillArray[(int)ESkillType.Skill_A];
                }

                return getSkill;
            }
        }

        public bool IsRemainingCoolTime(ESkillType skillType) 
            => SkillArray[(int)skillType]?.RemainCoolTime > 0.0f;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public void InitialSetInfo(Creature owner)
        {
            _owner = owner;
            SkillBase skillA = TryUnlockSkill(ESkillType.Skill_A);
            if (skillA == null)
            {
                Debug.LogError($"{nameof(SkillComponent)}::{nameof(InitialSetInfo)}, You must have one skill at least.");
                Debug.Break();
                return;
            }

#if UNITY_EDITOR
            DefaultSkillA = SkillArray[(int)ESkillType.Skill_A].SkillData.Dev_NameTextID;
            Dev_NameTextID = $"{_owner.Dev_NameTextID}_Skills";
#endif
        }

        private int GetSkillID(ESkillType skillType, Creature owner)
        {
            return skillType switch
            {
                ESkillType.Skill_A => owner.CreatureData.Skill_A_TemplateID + (owner.Level - 1),
                ESkillType.Skill_B => owner.CreatureData.Skill_B_TemplateID + (owner.Level - 1),
                ESkillType.Skill_C => owner.CreatureData.Skill_C_TemplateID + (owner.Level - 1),
                _ => throw new ArgumentOutOfRangeException(nameof(GetSkillID), $"Invalid value type: {skillType}")
            };
        }

        public SkillBase TryUnlockSkill(ESkillType skillType)
        {
            // 101000: 101100, 101200, 101300
            /*
                Passive: 101000(Lv.01), 101002(Lv.03), 101004(Lv.05), 101007(Lv.08)
                Skill_A: 101100(Lv.01), 101002(Lv.03), 101004(Lv.05), 101007(Lv.08)
                Skill_B: 101201(Lv.02), 101203(Lv.04), 101205(Lv.06), 102007(Lv.08)
                Skill_C: 101302(Lv.03), 101304(Lv.05), 101306(Lv.07), 103007(Lv.08)
            */
            int skillID = GetSkillID(skillType, _owner);
            SkillData skillData = Util.GetSkillData(skillID, owner: _owner);
            if (skillData == null)
                return null;

            // --- Simple Validation Check
            if (CanUnlockSkill(skillData) == false)
                return null;

            Type skillClassType = Util.GetTypeFromClassName(skillData.ClassName);
            SkillBase newSkill = gameObject.AddComponent(skillClassType) as SkillBase;
            if (newSkill == null)
            {
                Debug.LogError($"Failed: {nameof(TryUnlockSkill)}");
                return null;
            }

            newSkill.InitialSetInfo(dataID: skillID, owner: _owner);
            SkillArray[(int)newSkill.SkillType] = newSkill;
            if (IsActiveSkill(newSkill.SkillType))
                AddActiveSkill(newSkill);
            
            return newSkill;
        }
        
        public SkillBase TryLevelUpSkill(SkillBase currentSkill)
        {
            int skillID = GetSkillID(currentSkill.SkillType, _owner);
            SkillData skillData = Util.GetSkillData(skillID, owner: _owner);
            if (skillData == null)
                return null;

            Type skillClassType = Util.GetTypeFromClassName(skillData.ClassName);
            SkillBase lvUpSkill = gameObject.AddComponent(skillClassType) as SkillBase;
            if (lvUpSkill == null)
            {
                Debug.LogError($"{nameof(TryLevelUpSkill)}");
                return null;
            }

            // --- 기존 스킬, 기존 스킬 이펙트 제거
            SkillArray[(int)currentSkill.SkillType] = null;
            DestroySkill(currentSkill);

            // --- 새로운 스킬로 교체
            lvUpSkill.InitialSetInfo(dataID: skillID, owner: _owner);
            SkillArray[(int)lvUpSkill.SkillType] = lvUpSkill;
            AddActiveSkill(lvUpSkill);

            return lvUpSkill;
        }

        private void DestroySkill(SkillBase skill)
        {
            RemoveActiveSkill(skill);
            SkillData skillData = skill.SkillData;
            _owner.RemoveEffect(skillData.OnCreateEffectIDs);
            _owner.RemoveEffect(skillData.OnSkillEnterEffectIDs);
            _owner.RemoveEffect(skillData.OnSkillCallbackEffectIDs);
            _owner.RemoveEffect(skillData.OnSkillExitEffectIDs);
            UnityEngine.Object.Destroy(skill, Time.deltaTime);
        }

        public void AddActiveSkill(SkillBase skill)
        {
            if (IsActiveSkill(skill.SkillType) == false)
                return;

            Debug.Log($"<color=cyan>Ready(Add): {skill.Dev_NameTextID}</color>");
#if UNITY_EDITOR
            if (skill.SkillType == ESkillType.Skill_B)
                ActiveSkillB = skill.Dev_NameTextID;
            else if (skill.SkillType == ESkillType.Skill_C)
                ActiveSkillC = skill.Dev_NameTextID;
#endif
            ActiveSkills.Add(skill);
        }

        public void RemoveActiveSkill(SkillBase skill)
        {
            if (IsActiveSkill(skill.SkillType) == false)
                return;

            Debug.Log($"<color=cyan>End(Remove): {skill.Dev_NameTextID}</color>");
            ActiveSkills.Remove(skill);
        }

        public void OnSkillEnter(ESkillType skillType)
                => SkillArray[(int)skillType]?.OnSkillEnter();
        public void OnSkillExit(ESkillType skillType)
                => SkillArray[(int)skillType]?.OnSkillExit();

        private bool IsActiveSkill(ESkillType skillType)
            => skillType == ESkillType.Skill_B || skillType == ESkillType.Skill_C;

        private void OnDisable() { }

        #region Util
        private bool CanUnlockSkill(SkillData skillData)
        {
            for (int i = 0; i < SkillArray.Length; ++i)
            {
                SkillBase skill = SkillArray[i];
                if (skill != null && skill.SkillType == skillData.SkillType)
                {
                    Debug.LogError($"Failed: {nameof(CanUnlockSkill)}, {skill.Dev_NameTextID} already exists.");
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}

/*
    [Prev]
      // --- PREV
        // public override bool SetInfo(BaseObject owner, List<int> skillDataIDs) // CreatureData로 받아볼까.
        // {
        //     // --- Creature는 최소한 스킬 1개를 무조건 가지고 있어야한다.
        //     if (skillDataIDs.Count == 0)
        //     {
        //         Debug.LogError($"{nameof(SkillComponent)}, {nameof(SetInfo)}, Input : \"{skillDataIDs.Count}, Skills zero count\"");
        //         Debug.Break();
        //         return false;
        //     }

        //     _owner = owner as Creature;
        //     foreach (int skillDataID in skillDataIDs)
        //         AddSkill(skillDataID);

        //     if (SkillArray == null)
        //         SkillArray = new SkillBase[(int)ESkillType.Max];

        //     return true;
        // }

        private void AddSkill(int skillDataID)
        {
            if (skillDataID == -1)
                return;

            Data.SkillData skillData = null;
            if (_owner.ObjectType == EObjectType.Hero)
                skillData = Managers.Data.HeroSkillDataDict[skillDataID];
            else if (_owner.ObjectType == EObjectType.Monster)
                skillData = Managers.Data.MonsterSkillDataDict[skillDataID];
            // --- SkillData.. and Stat Data... How to organize?
            // if (Managers.Data.SkillDataDict.TryGetValue(skillDataID, out Data.SkillData skillData) == false)
            // {
            //     Debug.LogError($"{nameof(SkillComponent)}, {nameof(AddSkill)}, Input : \"{skillDataID}\"");
            //     Debug.Break();
            //     return;
            // }

            Type skillClassType = Util.GetTypeFromName(skillData.ClassName);
            SkillBase skill = gameObject.AddComponent(skillClassType) as SkillBase;
            if (skill == null)
            {
                Debug.LogError($"{nameof(AddSkill)}");
                Debug.Break();
                return;
            }

            skill.InitialSetInfo(dataID: skillDataID, owner: _owner);
            Skills.Add(skill);

            //ESkillType skillType = Util.GetEnumFromString<ESkillType>(skillData.Type);
            switch (skillData.SkillType)
            {
                case ESkillType.Skill_A:
                    SkillArray[(int)ESkillType.Skill_A] = skill;
                    break;

                case ESkillType.Skill_B:
                    SkillArray[(int)ESkillType.Skill_B] = skill;
                    ActiveSkills.Add(skill);
                    break;

                case ESkillType.Skill_C:
                    SkillArray[(int)ESkillType.Skill_C] = skill;
                    ActiveSkills.Add(skill);
                    break;
            }
        }

        // public float GetInvokeRatio(ECreatureState skillState)
        // {
        //     float skillInvokeRatio = SkillArray[(int)skillState - ReadOnly.Numeric.MaxActiveSkillsCount].InvokeRatioOnUpdate;
        //     return UnityEngine.Mathf.Clamp(skillInvokeRatio, 0.01f, 0.99f);
        // }
*/