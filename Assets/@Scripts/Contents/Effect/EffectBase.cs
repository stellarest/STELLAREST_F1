using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;
using UnityEditor;
using System.Data;

namespace STELLAREST_F1
{
    // 이펙트는 기본적으로 스킬에 붙어있다. 어떠한 스킬이 묻었을 때 일어나는 효과.
    // Ex. TickTime: 1, TickCount: 5 -> 1초 마다 5번 실행하겠다. 근데 난 그냥 Duration, Period로
    // Monster와 다르게 Effect 여러가지 상속 구조로 가기 위해 베이스 클래스가 이렇게 구성되어 있음. Effect의 핵심은 "상속"
    // --- 단순 VFX, Buff/DeBuff, Dot, CC
    public class EffectBase : BaseObject
    {
        private BaseCellObject _owner = null;
        public BaseCellObject Owner 
        {
            get => _owner;
            set
            {
                if (_owner == null || _owner != value)
                    _owner = value;
            }
        }

        private SkillBase _skill = null;
        public SkillBase Skill
        {
            get => _skill;
            private set
            {
                if (_skill == null || _skill != value)
                    _skill = value;
            }
        }

        public EffectData EffectData { get; private set; } = null;
        public bool IsLoop { get; private set; } = false;
        public float Amount { get; private set; } = 0.0f;
        public float Percent { get; private set; } = 0.0f;
        public float Period { get; private set; } = 0.0f;
        public float Remains { get; private set; } = 0.0f;
        public EEffectType EffectType { get; protected set; } = EEffectType.None;
        public EEffectSpawnType EffectSpawnType { get; private set; } = EEffectSpawnType.None;
        public EApplyStatType ApplyStateType { get; private set; } = EApplyStatType.None;
        protected Vector3 _enteredDir = Vector3.zero;
        protected int _enteredSignX = 0;

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Effect;
            SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_Effect;
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EffectType = EEffectType.None;
            EffectData = Managers.Data.EffectDataDict[dataID];
            IsLoop = EffectData.IsLoop;
            Amount = EffectData.AddAmount;
            Percent = EffectData.AddPercent;
            Period = EffectData.Period;
            InitialSetSize(EffectData.EffectSize);
        }

        private void InitialSetSize(EObjectSize objSize)
        {
            switch (objSize)
            {
                case EObjectSize.VerySmall:
                    break;

                case EObjectSize.Small:
                    transform.localScale = new Vector3(0.4F, 0.4F, 1F);
                    break;

                case EObjectSize.Medium:
                    break;

                case EObjectSize.Large:
                    break;

                case EObjectSize.VeryLarge:
                    break;

                case EObjectSize.UsePreset:
                    break;
            }
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            base.EnterInGame(spawnPos);
            Remains = EffectData.Duration * EffectData.Period;
            transform.position = EffectSpawnInfo(EffectData.EffectSpawnType);
            ApplyEffect();
        }

        private Vector3 EffectSpawnInfo(EEffectSpawnType effectSpawnType)
        {
            if (effectSpawnType == EEffectSpawnType.None)
                return SpawnedPos;

            Skill = Owner.GetComponent<Creature>().CreatureSkill.CurrentSkill;
            _enteredDir = Skill.EnteredTargetDir;
            _enteredSignX = Skill.EnteredSignX;

            if (effectSpawnType == EEffectSpawnType.SkillFromOwner)
            {
                SpawnedPos = Skill.EnteredOwnerPos;
                return Skill.EnteredOwnerPos;
            }
            else if (effectSpawnType == EEffectSpawnType.SkillFromTarget)
            {
                SpawnedPos = Skill.EnteredTargetPos;
                return  Skill.EnteredTargetPos;
            }

            return SpawnedPos;
        }
        #endregion

        // --- 왜 protected로 안하고 public으로 했지???
        // --- 아마도 나중에 EffectComp(ActiveEffects)로 따로 처리할듯.
        public virtual void ApplyEffect()
        {
            StartCoroutine(CoStartTimer());
            // 여기에다가 도트 뎀, 도트 힐, 패시브 영구적, 힘 버프, 체력 버프, 민첩 버프 등등을 적용시킨다.
            // 힘 버프(Buff, TypeID:1), 체력 버프(Buff, TypeID:2) 이런식..
        }

        protected virtual void ProcessDot() { }

        protected IEnumerator CoStartTimer()
        {
            if (EffectType == EEffectType.Airborne || EffectType == EEffectType.Knockback)
                yield break;

            float tickTimer = 0f;
            ProcessDot();
            if (EffectType == EEffectType.Instant)
            {
                // yield return new WaitForSeconds(1f);
                yield return new WaitForSeconds(Remains);
            }
            else
            {
                while (Remains > 0f)
                {
                    Remains -= Time.deltaTime;
                    tickTimer += Time.deltaTime;

                    if (tickTimer >= EffectData.Period)
                    {
                        ProcessDot();
                        tickTimer -= EffectData.Period;
                    }

                    yield return null;
                }
            }

            Remains = 0f;
            ClearEffect(EEffectClearType.TimeOut);
        }

        public virtual bool ClearEffect(EEffectClearType clearType)
        {
            // --- TEMP
            if (Owner.IsValid() == false)
            {
                Managers.Object.Despawn(this, DataTemplateID);
                return false;
            }

            switch (clearType)
            {
                case EEffectClearType.TimeOut:
                case EEffectClearType.TriggerOutAoE:
                case EEffectClearType.EndOfCC:
                    {
                        Owner.BaseEffect.RemoveEffect(this);
                        //Owner.CreatureEffect.RemoveEffect(this);
                        return true;
                    }

                case EEffectClearType.ClearSkill:
                {
                    // Aoe 범위 안에 있는 경우 해제x
                    // if (EffectSpawnType != EEffectSpawnType.External)
                    // {
                    //     Managers.Object.Despawn(this, DataTemplateID);
                    //     return true;
                    // }
                    break;
                }
            }

            return false;
        }

        public bool IsCroudControl()
        {
            switch (EffectType)
            {
                case EEffectType.Airborne:
                case EEffectType.Knockback:
                case EEffectType.Freeze:
                case EEffectType.Stun:
                    return true;
            }

            return false;
        }
    }
}

/*
// protected void ApplyParticleInfo()
        // {
        //     if (EffectData.DataID != 1008)
        //         return;
        //     // if (_particles == null && _particleRenderers == null)
        //     //     return;
        //     // if (Owner.IsValid() == false || Owner.Target.IsValid() == false)
        //     //     return;

        //     for (int i = 0; i < _particles.Length; ++i)
        //     {
        //         var main = _particles[i].main;
        //         // Ref Joystick Angle
        //         Vector3 nDir = Owner.Target.CellPos - Owner.CellPos;
        //         float angle = Mathf.Atan2(-nDir.normalized.x, nDir.normalized.y) * Mathf.Rad2Deg;
        //         if (angle < 0)
        //         {
        //              angle += 360f;
        //         }
        //         Debug.Log($"Angle: {angle}");
        //         main.startRotation = (angle + (Owner as Hero).TestOffset) * Mathf.Deg2Rad * -1;
        //         //main.flipRotation = (int)Owner.LookAtDir * -1;
        //     }
        // }

        // public override bool SetInfo(int dataID, BaseObject owner, BaseObject source)
        // {
        //     if (base.SetInfo(dataID, owner, source))
        //     {
        //         gameObject.name = $"@{gameObject.name}";
        //         Owner = owner as Creature;
        //         Source = source;
        //         EffectData = Managers.Data.EffectDataDict[dataID];
        //         DataTemplateID = dataID;

        //         IsLoop = EffectData.IsLoop;
        //         EffectType = EffectData.EffectType;
        //         switch (EffectData.EffectSize)
        //         {
        //             case EObjectSize.None:
        //                 break;
        //             case EObjectSize.VerySmall:
        //                 break;
        //             case EObjectSize.Small:
        //                 transform.localScale = new Vector3(0.4F, 0.4F, 1F);
        //                 break;
        //             case EObjectSize.Medium:
        //                 break;
        //             case EObjectSize.Large:
        //                 break;
        //             case EObjectSize.VeryLarge:
        //                 break;
        //         }

        //         EffectSpawnType = EffectData.EffectSpawnType;
        //         if (EffectSpawnType == EEffectSpawnType.External)
        //             Remains = float.MaxValue;
        //         else
        //             Remains = EffectData.Duration;
        //         return false;
        //     }

        //     Source = source;
        //     return true;
        // }

        // protected void ShowEffect()
        // {
        //     if (Source.IsValid() == false)
        //         return;

        //     if (Source.ObjectType == EObjectType.Hero || Source.ObjectType == EObjectType.Monster)
        //     {
        //         transform.position = GetRandomSpawnPosition(Managers.Map.GetCenterWorld(Source.CellPos), cellSize: 1);
        //     }
        // }

        // protected Vector3 GetRandomSpawnPosition(Vector3 cellCenter, float cellSize)
        // {
        //     float offset = cellSize / 4.0f;
        //     Vector3[] quadCenters = new Vector3[4];
        //     quadCenters[0] = cellCenter + new Vector3(offset, offset, 0);  // --- 1사분면 중앙
        //     quadCenters[1] = cellCenter + new Vector3(-offset, offset, 0);   // --- 2사분면 중앙
        //     quadCenters[2] = cellCenter + new Vector3(-offset, -offset, 0); // --- 3사분면 중앙
        //     quadCenters[3] = cellCenter + new Vector3(offset, -offset, 0);  // --- 4사분면 중앙
        //     int randIdx = Random.Range(0, quadCenters.Length + 1);
        //     if (randIdx == quadCenters.Length)
        //         return cellCenter;

        //     return quadCenters[randIdx];
        // }
*/