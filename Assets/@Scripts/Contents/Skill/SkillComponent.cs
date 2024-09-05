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
        // public List<SkillBase> ActiveSkills { get; } = new List<SkillBase>();

        // --- DEV 
        public List<SkillBase> ActiveSkills = new List<SkillBase>();

        public SkillBase FindSkill(int dataID) => Skills.FirstOrDefault(n => n.DataTemplateID == dataID);
        public SkillBase[] SkillArray { get; private set; } = new SkillBase[(int)ESkillType.Max]; // --- Caching
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

        public SkillBase GetSkill
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

            if (SkillArray == null)
                SkillArray = new SkillBase[(int)ESkillType.Max];

            // --- Default Skill(A)
            SkillArray[(int)ESkillType.Skill_A] = InitialAddSkill(creatureData.Skill_A_ID);

            // --- Active Skill1(B)
            SkillArray[(int)ESkillType.Skill_B] = InitialAddSkill(creatureData.Skill_B_ID);
            if (SkillArray[(int)ESkillType.Skill_B] != null)
                ActiveSkills.Add(SkillArray[(int)ESkillType.Skill_B]);

            // --- Active Skill2(C)
            SkillArray[(int)ESkillType.Skill_C] = InitialAddSkill(creatureData.Skill_C_ID);
            if (SkillArray[(int)ESkillType.Skill_C] != null)
                ActiveSkills.Add(SkillArray[(int)ESkillType.Skill_C]);

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

        private SkillBase InitialAddSkill(int skillDataID)
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
                Debug.LogError($"{nameof(InitialAddSkill)}, You have a SkillDataID, but Add Failed.");
                Debug.Break();
                return null;
            }

            skill.InitialSetInfo(dataID: skillDataID, owner: _owner);
            Skills.Add(skill);
            return skill;
        }

        public void AddActiveSkill(SkillBase skill)
        {
            if (skill.SkillType == ESkillType.Skill_A)
                return;

            Debug.Log($"<color=cyan>Ready(Add): {skill.Dev_TextID}</color>");
            ActiveSkills.Add(skill);
        }

        public void RemoveActiveSkill(SkillBase skill)
        {
            if (skill.SkillType == ESkillType.Skill_A)
                return;

            Debug.Log($"<color=cyan>End(Remove): {skill.Dev_TextID}</color>");
            ActiveSkills.Remove(skill);
        }

        public void OnSkillStateEnter(ESkillType skillType)
                => SkillArray[(int)skillType]?.OnSkillStateEnter();
        public void OnSkillStateExit(ESkillType skillType)
                => SkillArray[(int)skillType]?.OnSkillStateExit();

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