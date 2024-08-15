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

        // protected virtual Projectile GenerateProjectile(Creature owner, Vector3 spawnPos)
        // {
        //     // --- 프로젝타일 객체가 없는 즉발성 원거리 스킬
        //     if (SkillData.ProjectileID == -1)
        //     {
        //         owner.Target.OnDamaged(owner, this);
        //         return null;
        //     }

        //     Projectile projectile = Managers.Object.Spawn<Projectile>(EObjectType.Projectile, SkillData.ProjectileID);
        //     projectile.transform.position = spawnPos;
        //     return projectile;
        // }

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
#if UNITY_EDITOR
                        if (!IsLockTargetDir(ETargetDirection.VerticalUp) || !IsLockTargetDir(ETargetDirection.VerticalDown) ||
                            !IsLockTargetDir(ETargetDirection.DiagonalUp) || !IsLockTargetDir(ETargetDirection.DiagonalDown))
                            Debug.Log("<color=white>Single - Horizontal</color>");
#endif
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
#if UNITY_EDITOR
                            if (!IsLockTargetDir(ETargetDirection.Horizontal) || !IsLockTargetDir(ETargetDirection.VerticalDown) || 
                                !IsLockTargetDir(ETargetDirection.DiagonalUp) || !IsLockTargetDir(ETargetDirection.DiagonalDown))
                                Debug.Log("<color=yellow>Single - VerticalUp</color>");
#endif
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
#if UNITY_EDITOR
                            if (!IsLockTargetDir(ETargetDirection.Horizontal) || !IsLockTargetDir(ETargetDirection.VerticalUp) || 
                                !IsLockTargetDir(ETargetDirection.DiagonalUp) || !IsLockTargetDir(ETargetDirection.DiagonalDown))
                                Debug.Log("<color=brown>Single - VerticalDown</color>");
#endif
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
#if UNITY_EDITOR
                            if (!IsLockTargetDir(ETargetDirection.Horizontal) || !IsLockTargetDir(ETargetDirection.VerticalUp) ||
                                !IsLockTargetDir(ETargetDirection.VerticalDown) || !IsLockTargetDir(ETargetDirection.DiagonalDown))
                                Debug.Log("<color=magenta>Single - DiagonalUp</color>");
#endif
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
#if UNITY_EDITOR
                            if (!IsLockTargetDir(ETargetDirection.Horizontal) || !IsLockTargetDir(ETargetDirection.VerticalUp) ||
                                !IsLockTargetDir(ETargetDirection.VerticalDown) || !IsLockTargetDir(ETargetDirection.DiagonalUp))
                                Debug.Log("<color=green>Single - DiagonalDown</color>");
#endif
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
            Vector3 nLookAtDir  = new Vector3((int)enteredLookAtDir, 0, 0);
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

        protected virtual void ReserveAroundTargets(ELookAtDirection enteredLookAtDir, BaseObject target)
        {
        }

        protected virtual void GatherAroundTargets(BaseObject target)
        {
            // --- REF
            // int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            // int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            // if (dx <= TargetDistance && dy <= TargetDistance)
            // {
            //     target.OnDamaged(attacker: Owner, skillByAttacker: this);
            //     if (SkillData.EffectIDs.Length != 0)
            //     {
            //         List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
            //             effectIDs: SkillData.EffectIDs,
            //                 spawnPos: Util.GetRandomQuadPosition(target.CenterPosition),
            //             //spawnPos: Util.GetCellRandomQuadPosition(Managers.Map.GetCenterWorld(target.CellPos)),
            //             startCallback: null
            //         );
            //     }
            // }
        }
    }
}

/*
// protected virtual void GatherSingleTargetRange(BaseObject target)
        // {
        //     if (target.IsValid() == false)
        //         return;

        //     Vector3 nLookDir = new Vector3((int)Owner.LookAtDir, 0, 0);
        //     Vector3 nTargetDir = (target.transform.position - Owner.transform.position).normalized;
        //     float dot = Vector3.Dot(nLookDir, nTargetDir);
        //     float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        //     int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
        //     int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
        //     if (dx <= TargetDistance && dy <= TargetDistance)
        //     {
        //         if (IsLockTargetDir(ETargetDirection.Horizontal) == false)
        //         {
        //             if (target.CellPos.y == Owner.CellPos.y)
        //             {
        //                 if (angle < angleTreshhold)
        //                 {
        //                     Debug.Log("<color=white>Horizontal</color>");
        //                     LockTargetDir(ETargetDirection.Vertical);
        //                     LockTargetDir(ETargetDirection.Diagonal);
        //                     target.OnDamaged(attacker: Owner, skillByAttacker: this);
        //                     if (SkillData.EffectIDs.Length != 0)
        //                     {
        //                         List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
        //                             effectIDs: SkillData.EffectIDs,
        //                             source: target,
        //                             startCallback: null
        //                         );
        //                     }
        //                 }
        //             }
        //         }

        //         if (IsLockTargetDir(ETargetDirection.Vertical) == false)
        //         {
        //             if (target.CellPos.x == Owner.CellPos.x)
        //             {
        //                 if (angle >= _angleVerticalMin && angle <= _angleVerticalMax)
        //                 {
        //                     Debug.Log("<color=white>Vertical</color>");
        //                     LockTargetDir(ETargetDirection.Horizontal);
        //                     LockTargetDir(ETargetDirection.Diagonal);
        //                     target.OnDamaged(attacker: Owner, skillByAttacker: this);
        //                     if (SkillData.EffectIDs.Length != 0)
        //                     {
        //                         List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
        //                             effectIDs: SkillData.EffectIDs,
        //                             source: target,
        //                             startCallback: null
        //                         );
        //                     }
        //                 }
        //             }
        //         }

        //         if (IsLockTargetDir(ETargetDirection.Diagonal) == false)
        //         {
        //             if (Mathf.Abs(angle - _angleDiagonal) < angleTreshhold)
        //             {
        //                 Debug.Log("<color=white>Diagonal</color>");
        //                 LockTargetDir(ETargetDirection.Horizontal);
        //                 LockTargetDir(ETargetDirection.Vertical);
        //                 target.OnDamaged(attacker: Owner, skillByAttacker: this);
        //                 if (SkillData.EffectIDs.Length != 0)
        //                 {
        //                     List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
        //                         effectIDs: SkillData.EffectIDs,
        //                         source: target,
        //                         startCallback: null
        //                     );
        //                 }
        //             }
        //         }
        //     }
        // }
*/