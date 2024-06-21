using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;

namespace STELLAREST_F1
{
    // --- 스킬은 Creature부터 사용 가능 (BaseObject는 불가능)
    public abstract class SkillBase : InitBase
    {
        public Creature Owner { get; private set; } = null;
        public SkillData SkillData { get; private set; } = null;
        public int DataTemplateID { get; private set; } = -1;
        public ESkillType SkillType { get; private set; } = ESkillType.None;
        public EAttachmentPoint SkillFromPoint { get; private set; } = EAttachmentPoint.None;

        private float _remainCoolTime = 0f;
        public virtual float RemainCoolTime
        {
            get => _remainCoolTime;
            set => _remainCoolTime = value;
        }


        //public float InvokeRatioOnUpdate { get; private set; } = 0.0f;

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

            // --- Add Skill된 객체의 메서드만 OnSkillAnimationCallback 이벤트를 호출하게 되므로 상관없음.
            // Owner.CreatureAnimCallback.OnSkillCallbackHandler -= OnSkillCallback;
            // Owner.CreatureAnimCallback.OnSkillCallbackHandler += OnSkillCallback;

            SkillData = Managers.Data.SkillDataDict[dataID];
            DataTemplateID = dataID;
            SkillType = Util.GetEnumFromString<ESkillType>(SkillData.Type);
            SkillFromPoint = Util.GetEnumFromString<EAttachmentPoint>(SkillData.AttachmentPoint);
            //InvokeRatioOnUpdate = SkillData.InvokeRatioOnUpdate;

            EnterInGame(owner, dataID);
            return true;
        }

        protected virtual void EnterInGame(BaseObject owner, int dataID)
        {
            RemainCoolTime = 0.0f;
            Owner.CreatureAnim.CanSkillAttack = true;
        }

        // Creature -> CreatureAnim -> SkillComponent -> SkillBase
        public virtual void DoSkill()
        {
            Debug.Log($"<color=white>{Owner.gameObject.name} - Do Skill</color>");
            Owner.LookAtValidTarget();

            // 다소 무식한 방법이긴하지만 매우 직관적인 방식임.
            if (Owner.CreatureSkill != null)
                Owner.CreatureSkill.ActiveSkills.Remove(this);

            // StartCoroutine(CoActivateSkill());
            // Owner.CreatureAIState = ECreatureAIState.Skill;
            Owner.CreatureAnim.Skill(this.SkillType);
            
            // switch (SkillType)
            // {
            //     case ESkillType.Skill_Attack:
            //         {
            //             //Owner.CreatureUpperAnimState = ECreatureUpperAnimState.UA_Skill_Attack;
            //             // Owner.CreatureAnim.IsSkill = (int)ESkillType.Skill_Attack;
            //         }
            //         //Owner.CreatureState = ECreatureState.Skill_Attack;
            //         break;

            //     case ESkillType.Skill_A:
            //         Owner.CreatureAIState = ECreatureAIState.Skill_A;
            //         break;

            //     case ESkillType.Skill_B:
            //         Owner.CreatureAIState = ECreatureAIState.Skill_B;
            //         break;
            // }
            
            StartCoroutine(CoActivateSkill());
        }

        protected virtual Projectile GenerateProjectile(Creature owner, Vector3 spawnPos)
        {
            // (ex) 프로젝타일 객체가 없는 즉발성 원거리 공격 스킬. 프로젝타일 오브젝트를 생성할 필요가 없음.
            if (SkillData.ProjectileID == -1)
            {
                owner.Target.OnDamaged(owner, this);
                return null;
            }   

            return Managers.Object.Spawn<Projectile>(spawnPos, EObjectType.Projectile, SkillData.ProjectileID, owner);
            
            // Projectile projectile = Managers.Object.Spawn<Projectile>(spawnPos, EObjectType.Projectile, SkillData.ProjectileID);
            // LayerMask excludeLayerMask = 0;
            // excludeLayerMask.AddLayer(ELayer.Default);
            // excludeLayerMask.AddLayer(ELayer.Projectile);
            // excludeLayerMask.AddLayer(ELayer.Env);
            // excludeLayerMask.AddLayer(ELayer.Obstacle);
            // projectile.SetSpawnInfo(owner, spawnPos, this, excludeLayerMask);
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

        // >> From StateMachine, AnimCallback
        #region Anim State Events
        public abstract void OnSkillStateEnter();
        public abstract void OnSkillStateUpdate();
        public abstract void OnSkillStateEnd();
        #endregion

        #region Anim Clip Callback
        public abstract void OnSkillCallback();
        #endregion
    }
}
