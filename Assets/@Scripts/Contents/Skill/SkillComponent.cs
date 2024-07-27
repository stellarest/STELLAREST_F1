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
        public List<SkillBase> ActiveSkills { get; } = new List<SkillBase>();
        public SkillBase FindSkill(int dataID) => Skills.FirstOrDefault(n => n.DataTemplateID == dataID);
        public SkillBase[] SkillArray { get; private set; } = new SkillBase[(int)ESkillType.Max]; // --- Caching
        public SkillBase CurrentSkill
        {
            get
            {
                /*
                    ###########################################################
                    ##### ActiveSkills가 없으면 무조건 자동으로 Default Attack. #####
                    ###########################################################
                */
                if (ActiveSkills.Count == 0)
                    return SkillArray[(int)ESkillType.Skill_A];

                // SkillArray로 떔빵 할 수 있을 것 같긴 한데
                return ActiveSkills[UnityEngine.Random.Range(0, ActiveSkills.Count)];
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

            AddSkill(creatureData.Skill_Attack_ID);
            AddSkill(creatureData.Skill_A_ID);
            AddSkill(creatureData.Skill_B_ID);
            // --- Check Validation
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
                    Debug.LogError($"{nameof(SkillComponent)}, {nameof(SetInfo)}");
                    Debug.Break();
                }
            }
        }

        public void SetInfo(Creature owner, Data.CreatureData creatureData)
        {
            _owner = owner;

            if (SkillArray == null)
                SkillArray = new SkillBase[(int)ESkillType.Max];

            AddSkill(creatureData.Skill_Attack_ID);
            AddSkill(creatureData.Skill_A_ID);
            AddSkill(creatureData.Skill_B_ID);
            // --- Check Validation
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
                    Debug.LogError($"{nameof(SkillComponent)}, {nameof(SetInfo)}");
                    Debug.Break();
                }
            }
        }

        private void AddSkill(int skillDataID)
        {
            if (skillDataID == -1)
                return;

            if (Managers.Data.SkillDataDict.TryGetValue(skillDataID, out Data.SkillData skillData) == false)
            {
                Debug.LogError($"{nameof(SkillComponent)}, {nameof(AddSkill)}, Input : \"{skillDataID}\"");
                Debug.Break();
                return;
            }

            System.Type skillClassType = Util.GetTypeFromName(skillData.ClassName);
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

        public void OnSkillStateEnter(ESkillType skillType)
                => SkillArray[(int)skillType]?.OnSkillStateEnter();
        // public void OnSkillStateUpdate(ESkillType skillType)
        //         => SkillArray[(int)skillType]?.OnSkillStateUpdate();
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
*/