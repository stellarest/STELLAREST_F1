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
        // public List<SkillBase> Skills { get; } = new List<SkillBase>();

        public List<SkillBase> ActiveSkills = new List<SkillBase>();

        private const int c_Skill_A_INTERVAL_NUMBER = 100; // --- 나중에 건드릴수도 있으므로
        private const int c_Skill_B_INTERVAL_NUMBER = 200; // 필요 없어질 듯?
        private const int c_Skill_C_INTERVAL_NUMBER = 300; // 필요 없어질 듯?
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
        [field: SerializeField] public SkillBase[] SkillArray { get; private set; } = null; // --- Caching

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

        public void InitialSetInfo(Creature owner, CreatureData creatureData)
        {
            _owner = owner;

            SkillArray = SkillArray == null ? new SkillBase[(int)ESkillType.Max] : SkillArray;

            // --- Add: Skill_A(Default)
            //SkillArray[(int)ESkillType.Skill_A] = AddSkill(dataID: creatureData.Skill_A_ID);
            SkillArray[(int)ESkillType.Skill_A] = UnlockSkill(dataID: creatureData.Skill_A_ID);
#if UNITY_EDITOR
            DefaultSkillA = SkillArray[(int)ESkillType.Skill_A].SkillData.Dev_NameTextID;
            Dev_NameTextID = $"{_owner.Dev_NameTextID}_Skills";
#endif
            // Skills.Add(SkillArray[(int)ESkillType.Skill_A]);
            // --- 지금 이 부분을 추가하면 안되고, 히어로 레벨이 Lv.3, Lv.5가 되었을 때로 변경해야함
            // --- Active Skill(B)
            // SkillArray[(int)ESkillType.Skill_B] = AddSkill(creatureData.Skill_B_ID);
            // if (SkillArray[(int)ESkillType.Skill_B] != null)
            // {
            //     // ActiveSkills.Add(SkillArray[(int)ESkillType.Skill_B]);
            //     AddActiveSkill(SkillArray[(int)ESkillType.Skill_B]);
            //     // Skills.Add(SkillArray[(int)ESkillType.Skill_B]);
            // }

            // // --- Active Skill(C)
            // SkillArray[(int)ESkillType.Skill_C] = AddSkill(creatureData.Skill_C_ID);
            // if (SkillArray[(int)ESkillType.Skill_C] != null)
            // {
            //     // ActiveSkills.Add(SkillArray[(int)ESkillType.Skill_C]);
            //     AddActiveSkill(SkillArray[(int)ESkillType.Skill_C]);
            //     // Skills.Add(SkillArray[(int)ESkillType.Skill_C]);
            // }

            // --- Check Validation (All Creatures must have one skill at least.)
            // {
            //     int skillCount = 0;
            //     for (int i = 0; i < SkillArray.Length; ++i)
            //     {
            //         SkillBase skill = SkillArray[i];
            //         if (skill != null)
            //             ++skillCount;
            //     }

            //     if (skillCount == 0)
            //     {
            //         Debug.LogError($"{nameof(SkillComponent)}");
            //         Debug.Break();
            //     }
            // }
        }

        public SkillBase UnlockSkill(int dataID)
        {
            SkillData skillData = Util.GetSkillData(dataID, owner: _owner);
            if (skillData == null)
                return null;

            for (int i = 0; i < SkillArray.Length; ++i)
            {
                SkillBase skill = SkillArray[i];
                if (skill != null && skill.SkillType == skillData.SkillType)
                {
                    Debug.LogError($"Failed: {nameof(UnlockSkill)}, {skill.Dev_NameTextID} already exists.");
                    return null;
                }
            }

            Type skillClassType = Util.GetTypeFromClassName(skillData.ClassName);
            SkillBase newSkill = gameObject.AddComponent(skillClassType) as SkillBase;
            if (newSkill == null)
            {
                Debug.LogError($"Failed: {nameof(UnlockSkill)}");
                return null;
            }

            newSkill.InitialSetInfo(dataID: dataID, owner: _owner);
            if (IsActiveSkill(newSkill.SkillType))
            {
                SkillArray[(int)newSkill.SkillType] = newSkill;
                AddActiveSkill(newSkill);
            }
            
            return newSkill;
        }

        public SkillBase LevelUpMySkill(SkillBase currentSkill, int dataID)
        {
            SkillData skillData = Util.GetSkillData(dataID, owner: _owner);
            if (skillData == null)
                return null;

            if (SkillArray[(int)currentSkill.SkillType] == null)
            {
                Debug.LogError($"{nameof(LevelUpMySkill)}: You need to try {nameof(UnlockSkill)}.");
                Debug.Break();
                return null;
            }

            Type skillClassType = Util.GetTypeFromClassName(skillData.ClassName);
            SkillBase lvUpSkill = gameObject.AddComponent(skillClassType) as SkillBase;
            if (lvUpSkill == null)
            {
                Debug.LogError($"{nameof(LevelUpMySkill)}");
                return null;
            }
            
            lvUpSkill.InitialSetInfo(dataID: dataID, owner: _owner);
            if (lvUpSkill.SkillType != currentSkill.SkillType)
            {
                Debug.LogError($"{nameof(LevelUpMySkill)}, Difference of between skill type.");
                UnityEngine.Object.Destroy(lvUpSkill);
                return null;
            }

            RemoveActiveSkill(currentSkill);
            UnityEngine.Object.Destroy(currentSkill);

            SkillArray[(int)lvUpSkill.SkillType] = lvUpSkill;
            AddActiveSkill(lvUpSkill);
            return lvUpSkill;
        }

        public void LevelUpSkill(int ownerLevelID)
        {
            if (SkillArray[(int)ESkillType.Skill_B] != null)
                LevelUpSkill(ownerLevelID + c_Skill_B_INTERVAL_NUMBER, ESkillType.Skill_B);
            else
                Debug.LogWarning($"Failed: {nameof(LevelUpSkill)}: Skill_B");

            if (SkillArray[(int)ESkillType.Skill_C] != null)
                LevelUpSkill(ownerLevelID + c_Skill_C_INTERVAL_NUMBER, ESkillType.Skill_C);
            else
                Debug.LogWarning($"Failed: {nameof(LevelUpSkill)}: Skill_C");
        }

        private void LevelUpSkill(int skillDataID, ESkillType skillType)
        {
            int nextSkillID = skillDataID;
            SkillBase prevSkill = SkillArray[(int)skillType];
            SkillBase nextSkill = AddSkill(nextSkillID);
            if (nextSkill != null)
            {
                SkillArray[(int)skillType] = nextSkill;
                if (IsActiveSkill(skillType))
                {
                    // ActiveSkills.Remove(prevSkill);
                    RemoveActiveSkill(prevSkill);
                    AddActiveSkill(nextSkill);
                    // ActiveSkills.Add(nextSkill);
                }

                // Skills.Remove(prevSkill);

                Debug.Log($"--- Success to remove prev Skill: {prevSkill.Dev_NameTextID}");
                UnityEngine.Object.Destroy(prevSkill);
                Debug.Log($"<color=white>Success {nameof(LevelUpSkill)}: {skillType}, {nextSkill.Dev_NameTextID}</color>");
            }
            else
            {
                Debug.LogError($"Failed to find next Skill data: {skillDataID}, {skillType}");
            }
        }

        private SkillBase AddSkill(int dataID)
        {
            if (dataID == -1)
                return null;

            Data.SkillData skillData = null;
            if (_owner.ObjectType == EObjectType.Hero)
                skillData = Managers.Data.HeroSkillDataDict[dataID];
            else if (_owner.ObjectType == EObjectType.Monster)
                skillData = Managers.Data.MonsterSkillDataDict[dataID];

            Type skillClassType = Util.GetTypeFromClassName(skillData.ClassName);
            SkillBase skill = gameObject.AddComponent(skillClassType) as SkillBase;
            if (skill == null)
            {
                Debug.LogError($"{nameof(AddSkill)}, You have a SkillDataID, but Add Failed.");
                Debug.Break();
                return null;
            }

            skill.InitialSetInfo(dataID: dataID, owner: _owner);
            return skill;
        }

        public void AddActiveSkill(SkillBase skill)
        {
            if (skill.SkillType == ESkillType.Skill_A)
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
            if (skill.SkillType == ESkillType.Skill_A)
                return;

            Debug.Log($"<color=cyan>End(Remove): {skill.Dev_NameTextID}</color>");
            ActiveSkills.Remove(skill);
        }

        public void OnSkillStateEnter(ESkillType skillType)
                => SkillArray[(int)skillType]?.OnSkillStateEnter();
        public void OnSkillStateExit(ESkillType skillType)
                => SkillArray[(int)skillType]?.OnSkillStateExit();

        public SkillBase UnlockOrLevelUpSkill(int levelID)
        {
            SkillBase skillB = SkillArray[(int)ESkillType.Skill_B];
            // --- Unlock Skill_B
            if (skillB == null)
            {
                int skill_B_ID = levelID + c_Skill_B_INTERVAL_NUMBER;
                SkillBase newSkill_B = UnlockSkill(dataID: skill_B_ID);
                if (newSkill_B != null)
                {
                    SkillArray[(int)ESkillType.Skill_B] = newSkill_B;
#if UNITY_EDITOR
                    ActiveSkillB = newSkill_B.SkillData.Dev_NameTextID;
#endif
                }
            }
            else // --- LevelUp Skill_B
            {
                RemoveActiveSkill(SkillArray[(int)ESkillType.Skill_B]);
            }

            return null;
        }

        private bool IsActiveSkill(ESkillType skillType)
            => skillType == ESkillType.Skill_B || skillType == ESkillType.Skill_C;

        private void OnDisable()
        {
        }
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