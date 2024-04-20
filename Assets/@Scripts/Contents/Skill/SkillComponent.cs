using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class SkillComponent : InitBase
    {
        public List<SkillBase> Skills { get; } = new List<SkillBase>();
        public List<SkillBase> ActiveSkills { get; } = new List<SkillBase>();
        public SkillBase[] SkillArray { get; private set; } = new SkillBase[(int)ESkillType.Max]; // Caching
        public SkillBase CurrentSkill
        {
            get
            {
                /*
                    ##### ActiveSkills가 없으면 무조건 자동으로 Default Attack. #####
                */
                if (ActiveSkills.Count == 0)
                    return SkillArray[(int)ESkillType.Skill_Attack];
                
                return ActiveSkills[UnityEngine.Random.Range(0, ActiveSkills.Count)];
            }
        }
        
        public bool IsRemainingCoolTime(ESkillType skillType)
            => SkillArray[(int)skillType].RemainCoolTime > 0.0f;

        private Creature _owner = null;
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override bool SetInfo(BaseObject owner, List<int> skillDataIDs) // TEMP
        {
            _owner = owner as Creature;
            foreach (int skillDataID in skillDataIDs)
                AddSkill(skillDataID);

            SkillArray = SkillArray == null ? new SkillBase[(int)ESkillType.Max] : SkillArray;
            return true;
        }

        private void AddSkill(int skillDataID)
        {
            if (skillDataID == -1)
                return;

            if (Managers.Data.SkillDataDict.TryGetValue(skillDataID, out Data.SkillData skillData) == false)
            {
                Debug.LogError($"{nameof(SkillComponent)}, {nameof(AddSkill)}, Input : \"{skillDataID}\"");
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
            float skillInvokeRatio = 0.0f;
            switch (skillState)
            {
                case ECreatureState.Skill_Attack:
                    skillInvokeRatio = SkillArray[(int)ESkillType.Skill_Attack].InvokeRatio;
                    break;

                case ECreatureState.Skill_A:
                    skillInvokeRatio = SkillArray[(int)ESkillType.Skill_Attack].InvokeRatio;
                    break;

                case ECreatureState.Skill_B:
                    skillInvokeRatio = SkillArray[(int)ESkillType.Skill_Attack].InvokeRatio;
                    break;
            }

            return UnityEngine.Mathf.Clamp(skillInvokeRatio, 0.0f, 1.0f);
        }

        public void PassOnSkillEnter(ECreatureState onEnterState)
        {
            switch (onEnterState)
            {
                case ECreatureState.Skill_Attack:
                    SkillArray[(int)ESkillType.Skill_Attack]?.OnSkillAnimationEnter();
                    break;

                case ECreatureState.Skill_A:
                    SkillArray[(int)ESkillType.Skill_A]?.OnSkillAnimationEnter();
                    break;

                case ECreatureState.Skill_B:
                    SkillArray[(int)ESkillType.Skill_B]?.OnSkillAnimationEnter();
                    break;
            }
        }

        public void PassOnSkillUpdate(ECreatureState onUpdateState)
        {
            switch (onUpdateState)
            {
                case ECreatureState.Skill_Attack:
                    SkillArray[(int)ESkillType.Skill_Attack]?.OnSkillAnimationUpdate();
                    break;

                case ECreatureState.Skill_A:
                    SkillArray[(int)ESkillType.Skill_A]?.OnSkillAnimationUpdate();
                    break;

                case ECreatureState.Skill_B:
                    SkillArray[(int)ESkillType.Skill_B]?.OnSkillAnimationUpdate();
                    break;
            }
        }

        public void PassOnSkillCompleted(ECreatureState onEndState)
        {
            switch (onEndState)
            {
                case ECreatureState.Skill_Attack:
                    SkillArray[(int)ESkillType.Skill_Attack]?.OnSkillAnimationCompleted();
                    break;

                case ECreatureState.Skill_A:
                    SkillArray[(int)ESkillType.Skill_A]?.OnSkillAnimationCompleted();
                    break;

                case ECreatureState.Skill_B:
                    SkillArray[(int)ESkillType.Skill_B]?.OnSkillAnimationCompleted();
                    break;
            }
        }
    }
}
