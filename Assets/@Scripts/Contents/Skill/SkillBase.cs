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

        public override bool SetInfo(BaseObject owner, int dataID)
        {
            if (base.SetInfo(owner, dataID) == false)
            {
                EnterInGame();
                return false;
            }

            Owner = owner as Creature;
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
            Owner.LookAtTarget(Owner.Target);

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

        protected virtual void GenerateProjectile(Creature owner, Vector3 spawnPos)
        {
            // (ex) 프로젝타일 객체가 없는 즉발성 원거리 공격 스킬
            if (SkillData.ProjectileID == -1)
            {
                owner.Target.OnDamaged(owner, this);
                return;
            }

            Projectile projectile = Managers.Object.Spawn<Projectile>(spawnPos, EObjectType.Projectile, SkillData.ProjectileID);

            LayerMask excludeLayerMask = 0;
            excludeLayerMask.AddLayer(ELayer.Default);
            excludeLayerMask.AddLayer(ELayer.Projectile);
            excludeLayerMask.AddLayer(ELayer.Env);
            excludeLayerMask.AddLayer(ELayer.Obstacle);
            
            projectile.SetSpawnInfo(owner, this, excludeLayerMask);
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

        // #########################################
        // ######## IMPLEMENT SKILL METHODS ########
        // #########################################
        public abstract void OnSkillStateEnter();
        public abstract void OnSkillStateUpdate();
        public abstract void OnSkillStateEnd();
        // #########################################
    }
}
