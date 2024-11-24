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

        public Creature Owner { get; private set; } = null;
        protected bool IsValidOwner => Owner.IsValid();
        protected bool IsValidTarget => Owner.Target.IsValid();

        public SkillData SkillData { get; private set; } = null;
        public int InvokeRange { get; private set; } = -1;
        public ESkillTargetRange TargetRange { get; private set; } = ESkillTargetRange.None;
        public int TargetDistance { get; private set; } = -1;
        public int DataTemplateID { get; private set; } = -1;
        public ESkillType SkillType { get; private set; } = ESkillType.None;
        public ESkillElementType SkillElementType { get; private set; } = ESkillElementType.None;

        // --- 이게 필요 없을 것 같긴 한데
        // public EAttachmentPoint SkillFromPoint { get; private set; } = EAttachmentPoint.None;
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

        private bool _manualCoolTime = false;
        public void StartManualCoolTime()
        {
            if (_manualCoolTime == false)
            {
                Dev.LogError(obj: nameof(SkillBase), method: nameof(StartManualCoolTime));
                return;
            }

            if (Owner.IsValid() == false)
            {
                RemainCoolTime = 0.0f;
                return;
            }

            StartCoroutine(CoManualCoolTime());
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

            Owner.CreatureSkill.RemoveActiveSkill(this);
            Owner.CreatureAnim.Skill(Owner.CreatureSkill.CurrentSkillType);
            StartCoroutine(CoSkillCoolTime());
        }

        private IEnumerator CoSkillCoolTime()
        {
            RemainCoolTime = SkillData.CoolTime;
            if (_manualCoolTime)
            {
                Dev.Log($"Is Manual CoolTime Skill: {SkillData.Dev_NameTextID}");
                yield break;
            }

            yield return new WaitForSeconds(SkillData.CoolTime);
            RemainCoolTime = 0f;
            Owner.CreatureSkill.AddSpecialSkill(this);
        }

        private IEnumerator CoManualCoolTime()
        {
            Dev.Log($"{this.SkillType}: Activates Manual CoolTime");
            yield return new WaitForSeconds(RemainCoolTime);
            //_manualCoolTime = true;
            RemainCoolTime = 0f;
            Owner.CreatureSkill.AddSpecialSkill(this);
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

        protected List<BaseCellObject> _skillTargets = new List<BaseCellObject>();
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

        public virtual void InitialSetInfo(int dataID, BaseCellObject owner)
        {
            Owner = owner as Creature;
            DataTemplateID = dataID;
            SkillData = Util.GetSkillData(dataID, Owner);
#if UNITY_EDITOR
            Dev_NameTextID = SkillData.Dev_NameTextID;
            Dev_DescriptionTextID = SkillData.Dev_DescriptionTextID;
#endif
            InvokeRange = SkillData.InvokeRange;
            TargetRange = SkillData.TargetRange;
            TargetDistance = SkillData.TargetDistance;
            SkillType = SkillData.SkillType;
            SkillElementType = SkillData.SkillElementType;
            _manualCoolTime = SkillData.ManualCoolTime;

            if (SkillData.OnCreate_GenEffectIDs.Length > 0)
                GenerateSkillEffects(SkillData.OnCreate_GenEffectIDs);
        }
        #endregion

        #region Events
        protected ESkillType _currentSkillType= ESkillType.None;
        public virtual bool OnSkillEnter()
        {
            if (Owner.IsValid() == false || Owner.Target.IsValid() == false)
            {
                Owner.CreatureAnim.ResetAllAnimations();
                return false;
            }

            if (Owner.IsMaxLevel && this.SkillType == ESkillType.Skill_B)
            {
                Owner.CreatureBody.EnableSTrail(eTarget: EBaseBodyParts.Weapon, 
                    startCallback: () => Dev.Log("EnableSTrail_Weapon, in SkillBase.", highlight: true));
            }

            EnteredOwnerPos = Owner.CenterPosition;
            EnteredTargetPos = Owner.Target.CenterPosition; // FOR PROJECTILE,,,
            EnteredTargetDir = Owner.Target.CellPos - Owner.CellPos;
            EnteredSignX = (Owner.LookAtDir == ELookAtDirection.Left) ? 1 : 0;
            Owner.Moving = false; // --- Blending Anim(Move to Idle)

            if (SkillData.OnSkillEnter_GenEffectIDs.Length != 0)
                GenerateSkillEffects(SkillData.OnSkillEnter_GenEffectIDs);

            return true;
        }

        private bool IsCorrectSkillType
            => Owner.CreatureSkill.CurrentSkillType == SkillType;

        private int _currentEffectIdx = 0;
        public virtual bool OnSkillCallback()
        {
            if (IsCorrectSkillType)
            {
                // if (SkillData.OnSkillCallback_GenEffectIDs.Length != 0)
                // {
                //     GenerateSkillEffects(SkillData.OnSkillCallback_GenEffectIDs);
                // }
                
                int length = SkillData.OnSkillCallback_GenEffectIDs.Length;
                if (length > 0)
                {
                    if (_currentEffectIdx < length)
                        GenerateSkillEffect(SkillData.OnSkillCallback_GenEffectIDs[_currentEffectIdx++]);
                    
                    if (_currentEffectIdx == length)
                    {
                        _currentEffectIdx = 0;
                    }
                }

                return true;

                // if (_currentEffectIdx++ == length)
                // {
                //     _currentEffectIdx = 0;
                //     return true;
                // }
                // else
                // {
                //     GenerateSkillEffect(SkillData.OnSkillCallback_GenEffectIDs[_currentEffectIdx]);
                // }

                // if (length > 0 && _currentEffectIdx < length)
                // {
                //     GenerateSkillEffect(SkillData.OnSkillCallback_GenEffectIDs[_currentEffectIdx]);
                //     // if (_currentEffectIdx == length)
                //     //     _currentEffectIdx = 0;
                // }

                // return true;
            }

            return false;
        }
        
        public virtual void OnSkillExit()
        {
            _skillTargets.Clear();
            if (Owner.IsMaxLevel && this.SkillType == ESkillType.Skill_B)
            {
                Owner.CreatureBody.DisableSTrail(eTarget: EBaseBodyParts.Weapon,
                    endCallback: () => Dev.Log("Disable_Weapon, in SkillBase."));
            }

            if (SkillData.OnSkillExit_GenEffectIDs.Length > 0)
                GenerateSkillEffects(SkillData.OnSkillExit_GenEffectIDs);

            if (SkillData.OnSkillExit_RemoveEffectIDs.Length > 0)
                RemoveSkillEffects(SkillData.OnSkillExit_RemoveEffectIDs);
        }
        #endregion Events

        private void GenerateSkillEffects(IEnumerable<int> effectIDs)
        {
            if (Owner == null)
                return;

            foreach (var effectID in effectIDs)
                Owner.GenerateSkillEffect(effectID, this);
        }

        private void GenerateSkillEffect(int effectID)
        {
            if (Owner == null)
                return;

            Owner.GenerateSkillEffect(effectID, this);
        }

        private void RemoveSkillEffects(IEnumerable<int> effectIDs)
        {
            if (Owner.IsValid() == false)
                return;

            Owner.BaseEffect.RemoveEffect(effectIDs);
        }

        protected void GatherMeleeTargets()
        {
            _skillTargets.Clear();
            ELookAtDirection enteredLookAtDir = Owner.LookAtDir;
            for (int i = 0; i < Owner.Targets.Count; ++i)
            {
                if (Owner.Targets[i].IsValid() == false)
                    continue;

                BaseCellObject target = Owner.Targets[i];
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
        protected virtual void ReserveSingleTargets(ELookAtDirection enteredLookAtDir, BaseCellObject target)
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

        protected virtual void ReserveVerticalBothTargets(ELookAtDirection enteredLookAtDir, BaseCellObject target)
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

        protected virtual void ReserveHalfTargets(ELookAtDirection enteredLookAtDir, BaseCellObject target)
        {
            Vector3 nLookAtDir = new Vector3((int)enteredLookAtDir, 0, 0);
            //Vector3 nTargetDir = (target.transform.position - Owner.transform.position).normalized;
            Vector3 nTargetDir = target.CellPos - Owner.CellPos;
            float dot = Vector3.Dot(nLookAtDir, nTargetDir.normalized);
            // Util.Log($"Dot: {dot} - {target.CellPos}");
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

        protected virtual void ReserveAroundTargets(BaseCellObject target)
        {
            int dx = Mathf.Abs(target.CellPos.x - Owner.CellPos.x);
            int dy = Mathf.Abs(target.CellPos.y - Owner.CellPos.y);
            if (dx <= TargetDistance && dy <= TargetDistance)
                _skillTargets.Add(target);
        }
    }
}