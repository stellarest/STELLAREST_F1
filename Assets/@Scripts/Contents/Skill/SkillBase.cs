using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;

namespace STELLAREST_F1
{
    public abstract class SkillBase : InitBase
    {
        #region Background
        public Creature Owner { get; private set; } = null;
        public SkillData SkillData { get; private set; } = null;
        public int InvokeRange { get; private set; } = -1;
        public int TargetRange { get; private set; } = -1;
        public int DataTemplateID { get; private set; } = -1;
        public ESkillType SkillType { get; private set; } = ESkillType.None;
        public EAttachmentPoint SkillFromPoint { get; private set; } = EAttachmentPoint.None;

        private float _remainCoolTime = 0f;
        public virtual float RemainCoolTime
        {
            get => _remainCoolTime;
            set => _remainCoolTime = value;
        }
        public virtual void DoSkill()
        {
            if (Owner.CreatureSkill != null)
                Owner.CreatureSkill.ActiveSkills.Remove(this);

            Owner.CreatureAnim.Skill(this.SkillType);
            StartCoroutine(CoActivateSkill());
        }

        protected virtual Projectile GenerateProjectile(Creature owner, Vector3 spawnPos)
        {
            // --- 프로젝타일 객체가 없는 즉발성 원거리 스킬
            if (SkillData.ProjectileID == -1)
            {
                owner.Target.OnDamaged(owner, this);
                return null;
            }

            Projectile projectile = Managers.Object.Spawn<Projectile>(EObjectType.Projectile, SkillData.ProjectileID, owner);
            projectile.transform.position = spawnPos;
            return projectile;
        }

        private IEnumerator CoActivateSkill()
        {
            RemainCoolTime = SkillData.CoolTime;
            yield return new WaitForSeconds(SkillData.CoolTime);
            RemainCoolTime = 0f;
            if (Owner.CreatureSkill != null)
                Owner.CreatureSkill.ActiveSkills.Add(this);
        }

        protected Vector3 GetSpawnPos()
        {
            switch (Owner.ObjectType)
            {
                case EObjectType.Hero:
                    {
                        HeroBody heroBody = (Owner as Hero).HeroBody;
                        if (SkillFromPoint == EAttachmentPoint.WeaponL)
                            return heroBody.GetComponent<Transform>(EHeroWeapon.WeaponL).position;
                        else if (SkillFromPoint == EAttachmentPoint.WeaponLSocket)
                            return heroBody.GetComponent<Transform>(EHeroWeapon.WeaponLSocket).position;
                    }
                    break;
            }

            return Owner.CenterPosition;
        }

        public bool CanSkill()
        {
            // if (RemainCoolTime > 0f)
            //     return false;

            // if (Owner.Target.IsValid() && )

            return false;
        }
        #endregion

        #region Core
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
                EnterInGame(owner, dataID);
                return false;
            }

            Owner = owner as Creature;
            DataTemplateID = dataID;
            SkillData = Managers.Data.SkillDataDict[dataID];
            InvokeRange = SkillData.InvokeRange;
            TargetRange = SkillData.TargetRange;
            SkillType = SkillData.SkillType;
            SkillFromPoint = Util.GetEnumFromString<EAttachmentPoint>(SkillData.AttachmentPoint);

            EnterInGame(owner, dataID);
            return true;
        }

        protected virtual void EnterInGame(BaseObject owner, int dataID)
        {
            RemainCoolTime = 0.0f;
            Owner.CreatureAnim.ReadySkill = true;
        }
        #endregion

        #region Anim Clip Callback
        public abstract void OnSkillCallback();
        #endregion

        #region Anim State Events
        public abstract void OnSkillStateEnter();
        public abstract void OnSkillStateUpdate();
        public abstract void OnSkillStateExit();
        #endregion
    }
}
