using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class SkillComponent : InitBase
    {
        private Creature _owner = null;

        public List<SkillBase> Skills { get; } = new List<SkillBase>();
        public List<SkillBase> ActiveSkills = new List<SkillBase>();

        private const int c_Skill_A_INTERVAL_NUMBER = 100;
        private const int c_Skill_B_INTERVAL_NUMBER = 200;
        private const int c_Skill_C_INTERVAL_NUMBER = 300;

#if UNITY_EDITOR
        public string ActiveSkillB = "";
        public string ActiveSkillC = "";
#endif

        public SkillBase FindSkill(int skillDataID) 
            => Skills.FirstOrDefault(s => s.DataTemplateID == skillDataID);
        public SkillBase FindSkill(ESkillType skillType) 
            => Skills.FirstOrDefault(s => s.SkillType == skillType);


        // public SkillBase[] SkillArray { get; private set; } = new SkillBase[(int)ESkillType.Max]; // --- Caching
        public SkillBase[] SkillArray { get; private set; } = null; // --- Caching

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

        public bool IsRemainingCoolTime(ESkillType skillType) => SkillArray[(int)skillType]?.RemainCoolTime > 0.0f;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public void InitialSetInfo(Creature owner, Data.CreatureData creatureData)
        {
            _owner = owner;

            SkillArray = SkillArray == null ? new SkillBase[(int)ESkillType.Max] : SkillArray;

            // --- Default Skill(A)
            SkillArray[(int)ESkillType.Skill_A] = AddSkill(creatureData.Skill_A_ID);
            Skills.Add(SkillArray[(int)ESkillType.Skill_A]);

            // --- Active Skill(B)
            SkillArray[(int)ESkillType.Skill_B] = AddSkill(creatureData.Skill_B_ID);
            if (SkillArray[(int)ESkillType.Skill_B] != null)
            {
                // ActiveSkills.Add(SkillArray[(int)ESkillType.Skill_B]);
                AddActiveSkill(SkillArray[(int)ESkillType.Skill_B]);
                Skills.Add(SkillArray[(int)ESkillType.Skill_B]);
            }
            
            // --- Active Skill(C)
            SkillArray[(int)ESkillType.Skill_C] = AddSkill(creatureData.Skill_C_ID);
            if (SkillArray[(int)ESkillType.Skill_C] != null)
            {
                // ActiveSkills.Add(SkillArray[(int)ESkillType.Skill_C]);
                AddActiveSkill(SkillArray[(int)ESkillType.Skill_C]);
                Skills.Add(SkillArray[(int)ESkillType.Skill_C]);
            }

            // --- Check Validation (All Creatures must have one skill at least.)
            {
                int skillCount = 0;
                for (int i = 0; i < SkillArray.Length; ++i)
                {
                    SkillBase skill = SkillArray[i];
                    if (skill != null)
                        ++skillCount;
                }

                if (skillCount == 0)
                {
                    Debug.LogError($"{nameof(SkillComponent)}");
                    Debug.Break();
                }
            }
        }

        private SkillBase AddSkill(int skillDataID)
        {
            if (skillDataID == -1)
                return null;

            Data.SkillData skillData = null;
            if (_owner.ObjectType == EObjectType.Hero)
                skillData = Managers.Data.HeroSkillDataDict[skillDataID];
            else if (_owner.ObjectType == EObjectType.Monster)
                skillData = Managers.Data.MonsterSkillDataDict[skillDataID];

            Type skillClassType = Util.GetTypeFromName(skillData.ClassName);
            SkillBase skill = gameObject.AddComponent(skillClassType) as SkillBase;
            if (skill == null)
            {
                Debug.LogError($"{nameof(AddSkill)}, You have a SkillDataID, but Add Failed.");
                Debug.Break();
                return null;
            }

            skill.InitialSetInfo(dataID: skillDataID, owner: _owner);
            return skill;
        }

        public void AddActiveSkill(SkillBase skill)
        {
            if (skill.SkillType == ESkillType.Skill_A)
                return;

            // if (skill.SkillType == ESkillType.Skill_C)
            //     Debug.Log("ADDED SKILL C !!");

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

        /*
            // LevelUpSkill Ref

            [Paladin]: 101000 ~ 101004
            - Skill_A: 101100 ~ 101104
            - Skill_B: 101200 ~ 101204
            - Skill_C: 101300 ~ 101304

            1. 일반 (Common)
            2. 고급 (Uncommon)
            3. 레어 (Rare)
            4. 에픽 (Epic)
            5. 전설 (Legendary)
        */
        public void LevelUpSkill(int ownerLevelID)
        {
            // --- Creature는 Skill_A를 무조건 가지고 있어야함.
            LevelUpSkill(ownerLevelID + c_Skill_A_INTERVAL_NUMBER, ESkillType.Skill_A);

            // --- 애초에 처음부터 존재하지 않는 스킬은 스킬 레벨업 불가능
            if (SkillArray[(int)ESkillType.Skill_B] != null)
                LevelUpSkill(ownerLevelID + c_Skill_B_INTERVAL_NUMBER, ESkillType.Skill_B);
            else
                Debug.LogWarning($"Faield to {nameof(LevelUpSkill)}: Skill_B");

            if (SkillArray[(int)ESkillType.Skill_C] != null)
                LevelUpSkill(ownerLevelID + c_Skill_C_INTERVAL_NUMBER, ESkillType.Skill_C);
            else
                Debug.LogWarning($"Faield to {nameof(LevelUpSkill)}: Skill_C");
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

                Skills.Remove(prevSkill);

                Debug.Log($"--- Success to remove prev Skill: {prevSkill.Dev_NameTextID}");
                UnityEngine.Object.Destroy(prevSkill);
                Debug.Log($"<color=white>Success {nameof(LevelUpSkill)}: {skillType}, {nextSkill.Dev_NameTextID}</color>");
            }
            else
            {
                Debug.LogError($"Failed to find next Skill data: {skillDataID}, {skillType}");
            }
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