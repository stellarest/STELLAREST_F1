using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STELLAREST_F1.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class SkillComponent : InitBase
    {
        public List<SkillBase> Skills { get; } = new List<SkillBase>();
        public List<SkillBase> ActiveSkills { get; } = new List<SkillBase>();
        public SkillBase FindSkill(int dataID) => Skills.FirstOrDefault(n => n.DataTemplateID == dataID);
        public SkillBase[] SkillArray { get; private set; } = new SkillBase[(int)ESkillType.Max]; // Caching
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
                    return SkillArray[(int)ESkillType.Skill_Attack];

                // SkillArray로 떔빵 할 수 있을 것 같긴 한데
                return ActiveSkills[UnityEngine.Random.Range(0, ActiveSkills.Count)];
            }
        }

        public bool IsRemainingCoolTime(ESkillType skillType)
            => SkillArray[(int)skillType]?.RemainCoolTime > 0.0f;

        private Creature _owner = null;
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        // --- 어차피 Creature를 통해서 들어오기 때문에 한 번만 초기화 됨.
        public void SetInfo(Creature owner, CreatureData creatureData)
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

            if (Managers.Data.SkillDataDict.TryGetValue(skillDataID, out SkillData skillData) == false)
            {
                Debug.LogError($"{nameof(SkillComponent)}, {nameof(AddSkill)}, Input : \"{skillDataID}\"");
                Debug.Break();
                return;
            }

            System.Type skillClassType = Util.GetTypeFromName(skillData.ClassName);
            SkillBase skill = gameObject.AddComponent(skillClassType) as SkillBase;
            if (skill == null)
                return;

            skill.SetInfo(_owner, skillDataID);
            Skills.Add(skill);

            ESkillType skillType = Util.GetEnumFromString<ESkillType>(skillData.Type);
            switch (skillType)
            {
                case ESkillType.Skill_Attack:
                    SkillArray[(int)ESkillType.Skill_Attack] = skill;
                    break;

                case ESkillType.Skill_A:
                    SkillArray[(int)ESkillType.Skill_A] = skill;
                    ActiveSkills.Add(skill);
                    break;

                case ESkillType.Skill_B:
                    SkillArray[(int)ESkillType.Skill_B] = skill;
                    ActiveSkills.Add(skill);
                    break;
            }
        }

        // public float GetInvokeRatio(ECreatureState skillState)
        // {
        //     float skillInvokeRatio = SkillArray[(int)skillState - ReadOnly.Numeric.MaxActiveSkillsCount].InvokeRatioOnUpdate;
        //     return UnityEngine.Mathf.Clamp(skillInvokeRatio, 0.01f, 0.99f);
        // }

        public void PassOnSkillStateEnter(ECreatureAIState onEnterState)
                => SkillArray[(int)onEnterState - ReadOnly.Numeric.MaxActiveSkillsCount]?.OnSkillStateEnter();
        public void PassOnSkillStateUpdate(ECreatureAIState onUpdateState)
                => SkillArray[(int)onUpdateState - ReadOnly.Numeric.MaxActiveSkillsCount]?.OnSkillStateUpdate();
        public void PassOnSkillStateEnd(ECreatureAIState onEndState)
                => SkillArray[(int)onEndState - ReadOnly.Numeric.MaxActiveSkillsCount]?.OnSkillStateEnd();

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