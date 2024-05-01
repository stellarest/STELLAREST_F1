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

        // SKILL_ATTACK을 무조건 AI COOLTIME으로 고정할지는 고민
        public bool IsRemainingCoolTime(ESkillType skillType)
            => SkillArray[(int)skillType]?.RemainCoolTime > 0.0f;

        private Creature _owner = null;
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override bool SetInfo(BaseObject owner, List<int> skillDataIDs) // TEMP
        {
            if (skillDataIDs.Count == 0)
            {
                // SkillComponent를 들고 있다는 것은 크리쳐가 스킬을 최소 1개라도 들고 있다는 의미이므로 SkillCount가 0이면 에러 처리(크리쳐의 경우)
                //Debug.LogError($"{nameof(SkillComponent)}, {nameof(SetInfo)}, Input : \"{skillDataIDs.Count}, Skills zero count\"");
                Util.LogError($"{nameof(SkillComponent)}, {nameof(SetInfo)}, Input : \"{skillDataIDs.Count}, Skills zero count\"");
                return false;
            }

            _owner = owner as Creature;
            foreach (int skillDataID in skillDataIDs)
                AddSkill(skillDataID);

            if (SkillArray == null)
                SkillArray = new SkillBase[(int)ESkillType.Max];

            return true;
        }

        private void AddSkill(int skillDataID)
        {
            if (skillDataID == -1)
                return;

            if (Managers.Data.SkillDataDict.TryGetValue(skillDataID, out Data.SkillData skillData) == false)
            {
                //Debug.LogError($"{nameof(SkillComponent)}, {nameof(AddSkill)}, Input : \"{skillDataID}\"");
                Util.LogError($"{nameof(SkillComponent)}, {nameof(AddSkill)}, Input : \"{skillDataID}\"");
                return;
            }

            Type skillClassType = Util.GetTypeFromClassName(skillData.ClassName);
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

        public float GetInvokeRatio(ECreatureState skillState)
        {
            float skillInvokeRatio = SkillArray[(int)skillState - ReadOnly.Numeric.MaxActiveSkillsCount].InvokeRatioOnUpdate;
            return UnityEngine.Mathf.Clamp(skillInvokeRatio, 0.01f, 0.99f);
        }

        public void PassOnSkillStateEnter(ECreatureState onEnterState)
                => SkillArray[(int)onEnterState - ReadOnly.Numeric.MaxActiveSkillsCount]?.OnSkillStateEnter();
        public void PassOnSkillStateUpdate(ECreatureState onUpdateState)
                => SkillArray[(int)onUpdateState - ReadOnly.Numeric.MaxActiveSkillsCount]?.OnSkillStateUpdate();
        public void PassOnSkillStateEnd(ECreatureState onEndState)
                => SkillArray[(int)onEndState - ReadOnly.Numeric.MaxActiveSkillsCount]?.OnSkillStateEnd();
    }
}
