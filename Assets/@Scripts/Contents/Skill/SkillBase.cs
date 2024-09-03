using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;
using System;

namespace STELLAREST_F1
{
    public class SkillBase : InitBase
    {
        public Vector3 EnteredOwnerPos { get; protected set; } = Vector3.zero;
        public Vector3 EnteredTargetPos { get; protected set; } = Vector3.zero;
        public Vector3 EnteredTargetDir { get; protected set; } = Vector3.zero;
        public int EnteredSignX { get; protected set; } = 0;

        #if UNITY_EDITOR
        public string Dev_TextID = null;
        #endif

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
            private set
            {
                _remainCoolTime = value;
                if (value == 0f)
                    CheckReleaseSkillAnimState(SkillType);
            }
        }

        // --- 보통은 애니메이션보다, 스킬 쿨타임이 길어서 가능할 것 같긴 함.
        private void CheckReleaseSkillAnimState(ESkillType skillType)
        {
            if (Owner.IsValid() == false)
                return;

            CreatureAnimation creatureAnim = Owner.CreatureAnim;
            switch (skillType)
            {
                case ESkillType.Skill_A:
                    {
                        if (creatureAnim.IsEnteredAnimState(ECreatureAnimState.Upper_SkillA))
                            creatureAnim.ReleaseAnimState(ECreatureAnimState.Upper_SkillA);
                    }
                    break;
                case ESkillType.Skill_B:
                    {
                        if (creatureAnim.IsEnteredAnimState(ECreatureAnimState.Upper_SkillB))
                            creatureAnim.ReleaseAnimState(ECreatureAnimState.Upper_SkillB);
                    }
                    break;
                case ESkillType.Skill_C:
                    {
                        if (creatureAnim.IsEnteredAnimState(ECreatureAnimState.Upper_SkillC))
                            creatureAnim.ReleaseAnimState(ECreatureAnimState.Upper_SkillC);
                    }
                    break;
            }
        }

        public virtual void DoSkill()
        {
            // --- OK (Heroes)
            if (Owner.CreatureAnim.IsEnteredAnimState(ECreatureAnimState.Upper_CollectEnv))
                return;

            if (Owner.CreatureAnim.CanSkillTrigger == false)
            {
                // Debug.Log($"<color=white>OOPS !!: {Dev_TextID}</color>");
                return;
            }

            // --- REAL ACTIVATE SKILL,,
            Owner.CreatureSkill.RemoveActiveSkill(this);
            // Owner.CreatureSkill.CurrentSkillType = SkillType;

            // Owner.CreatureAnim.Skill(SkillType);
            Owner.CreatureAnim.Skill(Owner.CreatureSkill.CurrentSkillType);
            StartCoroutine(CoActivateSkill());
        }

        private IEnumerator CoActivateSkill()
        {
            RemainCoolTime = SkillData.CoolTime;

            yield return new WaitForSeconds(SkillData.CoolTime);
            RemainCoolTime = 0f;
            Owner.CreatureSkill.AddActiveSkill(this);
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
        private Vector3 _firstTargetPos = Vector3.zero;
        public void SetFirstTargetPos(Vector3 pos) => _firstTargetPos = pos;
        public Vector3 FirstTargetPos => _firstTargetPos;

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

            if (owner.ObjectType == EObjectType.Hero)
                SkillData = Managers.Data.HeroSkillDataDict[dataID];
            else if (owner.ObjectType == EObjectType.Monster)
                SkillData = Managers.Data.MonsterSkillDataDict[dataID];

            #if UNITY_EDITOR
            Dev_TextID = SkillData.DevTextID;
            #endif

            InvokeRange = SkillData.InvokeRange;
            TargetRange = SkillData.TargetRange;
            TargetDistance = SkillData.TargetDistance;
            SkillType = SkillData.SkillType;
            // --- AttachmentPoint: 필요할까??
            SkillFromPoint = Util.GetEnumFromString<EAttachmentPoint>(SkillData.AttachmentPoint);
        }
        #endregion

        #region Events
        protected ESkillType _currentSkillType= ESkillType.None;
        public virtual bool OnSkillStateEnter()
        {
            if (Owner.IsValid() == false || Owner.Target.IsValid() == false)
            {
                Owner.CreatureAnim.ResetAnimation();
                return false;
            }

            EnteredOwnerPos = Owner.CenterPosition;
            EnteredTargetPos = Owner.Target.CenterPosition;
            EnteredTargetDir = Owner.Target.CellPos - Owner.CellPos;
            EnteredSignX = (Owner.LookAtDir == ELookAtDirection.Left) ? 1 : 0;
            Owner.Moving = false;
            return true;
        }

        private bool IsCorrectSkillType
            => Owner.CreatureSkill.CurrentSkillType == SkillType;
        
        public virtual bool OnSkillCallback() 
            => IsCorrectSkillType;
        
        public virtual void OnSkillStateExit()
        {
            // Debug.Log(Owner.CreatureSkill.CurrentSkillType + " OOPS");
            _skillTargets.Clear();
        }
        #endregion Events

        protected void GatherMeleeTargets()
        {
            _skillTargets.Clear();
            ELookAtDirection enteredLookAtDir = Owner.LookAtDir;
            for (int i = 0; i < Owner.Targets.Count; ++i)
            {
                if (Owner.Targets[i].IsValid() == false)
                    continue;

                BaseObject target = Owner.Targets[i];
                if (target.ObjectType == EObjectType.Env)
                    continue;

                switch (TargetRange)
                {
                    case ESkillTargetRange.Single:
                        ReserveSingleTargets(enteredLookAtDir, target);
                        break;

                    case ESkillTargetRange.Half:
                        ReserveHalfTargets(enteredLookAtDir, target);
                        break;

                    case ESkillTargetRange.Around:
                        ReserveAroundTargets(target);
                        break;
                }
            }

            ReleaseAllTargetDirs();
        }

        protected BaseObject GetFirstTarget()
        {
            if (Owner.IsValid() == false)
                return null;

            return Owner.Target;
        }

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