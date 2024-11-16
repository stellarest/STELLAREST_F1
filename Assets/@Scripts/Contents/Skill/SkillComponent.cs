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

        // --- SpecialSkills: Skill_B, Skill_C
        [field: SerializeField] public List<SkillBase> SpecialSkills { get; private set; } = new List<SkillBase>();
        [field: SerializeField] public SkillBase[] SkillArray { get; private set; } = new SkillBase[(int)ESkillType.Max];

#if UNITY_EDITOR
        public string Skill_A = ""; // --- Basic Skill (* All creatures must have one *)
        public string Skill_B = ""; // --- Special Skill 1
        public string Skill_C = ""; // --- Special Skill 2
#endif
        [field: SerializeField] public ESkillType CurrentSkillType { get; set; } = ESkillType.None;
        public SkillBase CurrentSkill
            => CurrentSkillType == ESkillType.None ? null : SkillArray[(int)CurrentSkillType];

        public SkillBase ReadyToActivate
        {
            get
            {
                if (SpecialSkills.Count == 0)
                    return SkillArray[(int)ESkillType.Skill_A];

                int rand = UnityEngine.Random.Range(0, SpecialSkills.Count);
                SkillBase getSkill = SpecialSkills[rand];
                if (getSkill.RemainCoolTime > 0f)
                {
                    getSkill = SpecialSkills[rand == 0 ? ++rand : --rand];
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
                Util.LogError(obj: nameof(SkillComponent), method: nameof(InitialSetInfo), log: "You must have a skill.");
                return;
            }

#if UNITY_EDITOR
            Dev_NameTextID = $"{_owner.Dev_NameTextID} Skills";
            Skill_A = $"{skillA.SkillData.Dev_NameTextID}";
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
                Util.LogError(obj: nameof(SkillComponent), method: nameof(TryUnlockSkill));
                return null;
            }

            newSkill.InitialSetInfo(dataID: skillID, owner: _owner);
            SkillArray[(int)newSkill.SkillType] = newSkill;
            if (IsSpecialSkill(newSkill))
                AddSpecialSkill(newSkill);
            
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
                Util.LogError(obj: nameof(SkillComponent), method: nameof(TryLevelUpSkill));
                return null;
            }

            // --- 기존 스킬, 기존 스킬 이펙트 제거
            SkillArray[(int)currentSkill.SkillType] = null;
            DestroySkill(currentSkill);

            // --- 새로운 스킬로 교체
            lvUpSkill.InitialSetInfo(dataID: skillID, owner: _owner);
            SkillArray[(int)lvUpSkill.SkillType] = lvUpSkill;
            AddSpecialSkill(lvUpSkill);

            return lvUpSkill;
        }

        private void DestroySkill(SkillBase skill)
        {
            skill.StopAllCoroutines();
            _owner.CreatureAnim.RemoveOnSkillEventHandler(skill);
            RemoveActiveSkill(skill);
            SkillData skillData = skill.SkillData;
            _owner.RemoveEffect(skillData.OnCreate_GenEffectIDs);
            _owner.RemoveEffect(skillData.OnSkillEnter_GenEffectIDs);
            _owner.RemoveEffect(skillData.OnSkillCallback_GenEffectIDs);
            _owner.RemoveEffect(skillData.OnSkillExit_GenEffectIDs);
            UnityEngine.Object.Destroy(skill, Time.deltaTime);
        }

        public void AddSpecialSkill(SkillBase skill)
        {
            if (IsSpecialSkill(skill) == false)
                return;

            Util.Log($"Add Skill: {skill.Dev_NameTextID}");
#if UNITY_EDITOR
            if (skill.SkillType == ESkillType.Skill_B)
                Skill_B = skill.Dev_NameTextID;
            else if (skill.SkillType == ESkillType.Skill_C)
                Skill_C = skill.Dev_NameTextID;
#endif
            SpecialSkills.Add(skill);
        }

        public void RemoveActiveSkill(SkillBase skill)
        {
            if (IsSpecialSkill(skill) == false)
                return;

            Util.Log($"Remove Skill: {skill.Dev_NameTextID}", highlight: true);
            SpecialSkills.Remove(skill);
        }

        public void OnSkillEnter(ESkillType skillType)
                => SkillArray[(int)skillType]?.OnSkillEnter();
        public void OnSkillExit(ESkillType skillType)
                => SkillArray[(int)skillType]?.OnSkillExit();

        private bool IsSpecialSkill(SkillBase skill)
            => skill != null ? skill.SkillType != ESkillType.Skill_A : false; 

        private void OnDisable() { }

        #region Util
        private bool CanUnlockSkill(SkillData skillData)
        {
            for (int i = 0; i < SkillArray.Length; ++i)
            {
                SkillBase skill = SkillArray[i];
                if (skill != null && skill.SkillType == skillData.SkillType)
                {
                    Util.LogError(obj: nameof(SkillComponent), method: nameof(CanUnlockSkill), log: $"{skill.Dev_NameTextID} already exists.");
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
