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

        [SerializeField] private float _remainCoolTime = 0f;
        public virtual float RemainCoolTime
        {
            get => _remainCoolTime;
            set => _remainCoolTime = value;
        }
        public virtual void DoSkill()
        {
            // --- OK
            if (Owner.CreatureAnim.IsEnteredAnimState(ECreatureAnimState.Upper_CollectEnv))
            {
                Debug.Log("<color=cyan>WAIT</color>");
                return;
            }

            if (Owner.CreatureSkill != null)
                Owner.CreatureSkill.ActiveSkills.Remove(this);

            Debug.Log($"<color=white>{nameof(DoSkill)}</color>");
            Owner.CreatureAnim.Skill(this.SkillType);
            // --- 이게 먼저 실행이 되고 바로, CanSkill을 가서 그런것임 (*** OnTrigger문제 ***, OnTrigger, 애니메이션 문제임)
            // --- CollectEnv하다가 갑자기 팍 넘어가면 미사일 쏴야하는데, 애니메이션에서 이걸 인식 못하는 것임
            // --- OnCollectEnv와 UpperSkillA가 연결이 안되어있어서 그런듯. 그니까 코드 문제는 아님. 애니메이션 문제.
            StartCoroutine(CoActivateSkill());
        }

        protected virtual Projectile GenerateProjectile(Creature owner, Vector3 spawnPos)
        {
            Projectile projectile = Managers.Object.SpawnBaseObject<Projectile>(EObjectType.Projectile, 
                Owner.CenterPosition, SkillData.ProjectileID, owner: Owner);
            
            // Projectile projectile = Managers.Object.Spawn<Projectile>(EObjectType.Projectile, SkillData.ProjectileID);
            // projectile.transform.position = spawnPos;
            // return projectile;
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
        public BaseObject FirstTarget
        {
            get
            {
                if (_skillTargets.Count > 0 && _skillTargets[0] != null && _skillTargets[0].IsValid())
                    return _skillTargets[0];

                return null;
            }
        }

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

        public virtual void InitialSetInfo(int dataID, BaseObject owner)
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

        protected virtual void ReserveVerticalBothTargets(ELookAtDirection enteredLookAtDir, BaseObject target)
        {
            Vector3 nLookAtDir = new Vector3((int)enteredLookAtDir, 0, 0);
            Vector3 nTargetDir = target.CellPos - Owner.CellPos;
            float dot = Vector3.Dot(nLookAtDir, nTargetDir.normalized);

            int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            if (dx <= TargetDistance && dy <= TargetDistance)
            {
                if (dot == 0 || dot == 1)
                {
                    _skillTargets.Add(target);
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
