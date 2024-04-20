using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public abstract class SkillBase : InitBase
    {
        public Creature Owner { get; protected set; } = null;
        public Data.SkillData SkillData { get; private set; } = null;
        public int DataTemplateID { get; private set; } = -1;
        public ESkillType SkillType { get; private set; } = ESkillType.None;
        public float RemainCoolTime { get; protected set; } = 0.0f;
        public float InvokeRatioOnUpdate { get; private set; } = 0.0f;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        private bool _initialSet = false;
        public virtual bool SetInfo(Creature owner, int dataID)
        {
            if (_initialSet)
            {
                EnterInGame();
                return false;
            }

            Owner = owner;
            SkillData = Managers.Data.SkillDataDict[dataID];
            DataTemplateID = dataID;
            SkillType = Util.GetEnumFromString<ESkillType>(SkillData.Type);
            InvokeRatioOnUpdate = SkillData.InvokeRatioOnUpdate;

            EnterInGame();
            return true;
        }

        protected override void EnterInGame()
        {
            RemainCoolTime = 0.0f;
        }

        private void OnDisable()
        {
            if (Managers.Game == null)
                return;
            if (Owner.IsValid() == false)
                return;
            if (Owner.CreatureAnim == null)
                return;
        }

        public virtual void DoSkill()
        {
            if (Owner.CreatureSkillComponent != null)
                Owner.CreatureSkillComponent.ActiveSkills.Remove(this);

            StartCoroutine(CoActivateSkill());
            switch (SkillType)
            {
                case ESkillType.Skill_Attack:
                    Owner.CreatureState = ECreatureState.Skill_Attack;
                    break;

                case ESkillType.Skill_A:
                    Owner.CreatureState = ECreatureState.Skill_A;
                    break;

                case ESkillType.Skill_B:
                    Owner.CreatureState = ECreatureState.Skill_B;
                    break;
            }
        }

        private IEnumerator CoActivateSkill()
        {
            RemainCoolTime = SkillData.CoolTime;
            yield return new WaitForSeconds(SkillData.CoolTime);            
            RemainCoolTime = 0f;
            Debug.Log("END COOLTIME.");

            if (Owner.CreatureSkillComponent != null)
                Owner.CreatureSkillComponent.ActiveSkills.Add(this);
        }

        // TODO : Generate Projectile
        public abstract void OnSkillStateEnter();
        public abstract void OnSkillStateUpdate();
        public abstract void OnSkillStateEnd();
    }
}
