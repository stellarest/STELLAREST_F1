using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;
using System;

namespace STELLAREST_F1
{
    public abstract class SkillBase : InitBase
    {
        public Creature Owner { get; private set; } = null;
        protected bool IsValidOwner => Owner.IsValid();
        protected bool IsValidTarget => Owner.Target.IsValid();

        public SkillData SkillData { get; private set; } = null;
        public int InvokeRange { get; private set; } = -1;
        public ESkillTargetRange TargetRange { get; private set; } = ESkillTargetRange.None;
        public int TargetDistance { get; private set; } = -1;
        public int DataTemplateID { get; private set; } = -1;
        public ESkillType SkillType { get; private set; } = ESkillType.None;
        public EAttachmentPoint SkillFromPoint { get; private set; } = EAttachmentPoint.None;
        protected bool[] _lockTargetDirs = null;
        protected bool IsLockTargetDir(ETargetDirection targetDir) 
            => _lockTargetDirs[(int)targetDir];
        protected void LockTargetDir(ETargetDirection targetDir) 
            => _lockTargetDirs[(int)targetDir] = true;
        protected void ReleaseAllTargetDirs()
        {
            for (int i = 0; i < _lockTargetDirs.Length; ++i)
                _lockTargetDirs[i] = false;
        }

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
                // --- Generate Effect
                return null;
            }

            Projectile projectile = Managers.Object.SpawnBaseObject<Projectile>(EObjectType.Projectile, 
                Owner.CenterPosition, SkillData.ProjectileID);


            // Projectile projectile = Managers.Object.Spawn<Projectile>(EObjectType.Projectile, SkillData.ProjectileID);
            // projectile.transform.position = spawnPos;
            // return projectile;
            return null;
        }

        private IEnumerator CoActivateSkill()
        {
            RemainCoolTime = SkillData.CoolTime;
            yield return new WaitForSeconds(SkillData.CoolTime);
            RemainCoolTime = 0f;
            if (Owner.CreatureSkill != null)
                Owner.CreatureSkill.ActiveSkills.Add(this);
        }

        protected Vector3 GetSpawnPos() // --- Ref: Fire Socket
        {
            switch (Owner.ObjectType)
            {
                case EObjectType.Hero:
                    {
                        // HeroBody heroBody = (Owner as Hero).HeroBody;
                        // if (SkillFromPoint == EAttachmentPoint.WeaponL)
                        //     return heroBody.GetComponent<Transform>(EHeroWeapon.WeaponL).position;
                        // else if (SkillFromPoint == EAttachmentPoint.WeaponLSocket)
                        //     return heroBody.GetComponent<Transform>(EHeroWeapon.WeaponLSocket).position;
                    }
                    break;
            }

            return Owner.CenterPosition;
        }

        protected List<BaseObject> _skillTargets = new List<BaseObject>();
        #region Init Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _lockTargetDirs = new bool[(int)ETargetDirection.Max];
            for (int i = 0; i < _lockTargetDirs.Length; ++i)
                _lockTargetDirs[i] = false;

            return true;
        }

        public void InitialSetInfo(int dataID, BaseObject owner)
        {
            Owner = owner as Creature;
            DataTemplateID = dataID;
            SkillData = Managers.Data.SkillDataDict[dataID];
            InvokeRange = SkillData.InvokeRange;
            TargetRange = SkillData.TargetRange;
            TargetDistance = SkillData.TargetDistance;
            SkillType = SkillData.SkillType;
            SkillFromPoint = Util.GetEnumFromString<EAttachmentPoint>(SkillData.AttachmentPoint);
        }
        #endregion

        #region Events
        public abstract void OnSkillStateEnter();
        public abstract void OnSkillStateExit();
        public abstract void OnSkillCallback();
        #endregion Events

        private const float c_angleDiagonal = 45f;
        protected virtual void ReserveSelfTargets(BaseObject target) { }
        protected virtual void ReserveSingleTargets(ELookAtDirection enteredLookAtDir, BaseObject target)
        {
            Vector3 nLookAtDir = new Vector3((int)enteredLookAtDir, 0, 0);
            Vector3 nTargetDir = target.CellPos - Owner.CellPos;
            float dot = Vector3.Dot(nLookAtDir, nTargetDir.normalized);

            int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            if (dx <= TargetDistance && dy <= TargetDistance)
            {
                // --- Horizontal
                if (IsLockTargetDir(ETargetDirection.Horizontal) == false)
                {
                    if (dot == 1)
                    {
                        LockTargetDir(ETargetDirection.VerticalUp);
                        LockTargetDir(ETargetDirection.VerticalDown);
                        LockTargetDir(ETargetDirection.DiagonalUp);
                        LockTargetDir(ETargetDirection.DiagonalDown);
                        _skillTargets.Add(target);
                    }
                }

                // --- VerticalUp
                if (IsLockTargetDir(ETargetDirection.VerticalUp) == false)
                {
                    if (target.CellPos.y > Owner.CellPos.y)
                    {
                        if (dot == 0)
                        {
                            LockTargetDir(ETargetDirection.Horizontal);
                            LockTargetDir(ETargetDirection.VerticalDown);
                            LockTargetDir(ETargetDirection.DiagonalUp);
                            LockTargetDir(ETargetDirection.DiagonalDown);
                            _skillTargets.Add(target);
                        }
                    }
                }

                // --- VerticalDown
                if (IsLockTargetDir(ETargetDirection.VerticalDown) == false)
                {
                    if (target.CellPos.y < Owner.CellPos.y)
                    {
                        if (dot == 0)
                        {
                            LockTargetDir(ETargetDirection.Horizontal);
                            LockTargetDir(ETargetDirection.VerticalUp);
                            LockTargetDir(ETargetDirection.DiagonalUp);
                            LockTargetDir(ETargetDirection.DiagonalDown);
                            _skillTargets.Add(target);
                        }
                    }
                }

                // --- DiagonalUp
                if (IsLockTargetDir(ETargetDirection.DiagonalUp) == false)
                {
                    if (target.CellPos.y > Owner.CellPos.y)
                    {
                        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                        if (angle == c_angleDiagonal)
                        {
                            LockTargetDir(ETargetDirection.Horizontal);
                            LockTargetDir(ETargetDirection.VerticalUp);
                            LockTargetDir(ETargetDirection.VerticalDown);
                            LockTargetDir(ETargetDirection.DiagonalDown);
                            _skillTargets.Add(target);
                        }
                    }
                }

                // --- DiagonalDown
                if (IsLockTargetDir(ETargetDirection.DiagonalDown) == false)
                {
                    if (target.CellPos.y < Owner.CellPos.y)
                    {
                        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                        if (angle == c_angleDiagonal)
                        {
                            LockTargetDir(ETargetDirection.Horizontal);
                            LockTargetDir(ETargetDirection.VerticalUp);
                            LockTargetDir(ETargetDirection.VerticalDown);
                            LockTargetDir(ETargetDirection.DiagonalUp);
                            _skillTargets.Add(target);
                        }
                    }
                }
            }
        }

        protected virtual void ReserveHalfTargets(ELookAtDirection enteredLookAtDir, BaseObject target)
        {
            Vector3 nLookAtDir = new Vector3((int)enteredLookAtDir, 0, 0);
            //Vector3 nTargetDir = (target.transform.position - Owner.transform.position).normalized;
            Vector3 nTargetDir = target.CellPos - Owner.CellPos;
            float dot = Vector3.Dot(nLookAtDir, nTargetDir.normalized);
            //Debug.Log($"Dot: {dot} - {target.CellPos}");
            if (dot < 0) // --- 둔각일때는 종료
                return;

            int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            if (dx <= TargetDistance && dy <= TargetDistance)
            {
                if (nTargetDir.y >= 0 || nTargetDir.y <= 0)
                    _skillTargets.Add(target);
            }
        }

        protected virtual void ReserveAroundTargets(BaseObject target)
        {
            int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            if (dx <= TargetDistance && dy <= TargetDistance)
                _skillTargets.Add(target);
        }
    }
}
