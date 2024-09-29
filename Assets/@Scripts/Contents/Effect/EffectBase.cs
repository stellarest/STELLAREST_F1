using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STELLAREST_F1.Data;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // 이펙트는 기본적으로 스킬에 붙어있다. 어떠한 스킬이 묻었을 때 일어나는 효과.
    // Ex. TickTime: 1, TickCount: 5 -> 1초 마다 5번 실행하겠다. 근데 난 그냥 Duration, Period로
    // Monster와 다르게 Effect 여러가지 상속 구조로 가기 위해 베이스 클래스가 이렇게 구성되어 있음. Effect의 핵심은 "상속"
    // --- 단순 VFX, Buff/DeBuff, Dot, CC
    public abstract class EffectBase : BaseObject
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
        protected SkillBase _skill = null;
        public void SetSkill(SkillBase skill) => _skill = skill;

        public EffectData EffectData { get; private set; } = null;
        public EEffectType EffectType { get; protected set; } = EEffectType.None;
        public EEffectClearType EffectClearType { get; protected set; } = EEffectClearType.OnDisable;
        
        // --- 지금 당장 우아한 방법은 아니긴 하지만, ByCondition에 의한 이펙트는 OnRemoveSelfByCondition에서 재정의만 하면 됨
        public Action<Action> OnRemoveSelfByConditionHandler = null;
        protected virtual void OnRemoveSelfByCondition(Action endCallback = null) { }

        public bool IsLoop { get; private set; } = false;
        public float Period { get; private set; } = 0.0f;
        public float Remains { get; private set; } = 0.0f;

        protected Vector3 _enteredDir = Vector3.zero;
        protected int _enteredSignX = 0;

        public bool RemoveImmediately { get; protected set; } = false;

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

            if (Owner.ObjectType == EObjectType.Hero)
                EffectData = Managers.Data.HeroEffectDataDict[dataID];
            else if (Owner.ObjectType == EObjectType.Monster)
                EffectData = Managers.Data.MonsterEffectDataDict[dataID];
            else if (Owner.ObjectType == EObjectType.Env)
                EffectData = Managers.Data.EnvEffectDataDict[dataID];

#if UNITY_EDITOR
            Dev_NameTextID = EffectData.Dev_NameTextID;
            gameObject.name += $"_{EffectData.Dev_NameTextID}";
#endif

            IsLoop = EffectData.IsLoop;
            Period = EffectData.Period;
            InitialSetSize(EffectData.EffectSize);
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            base.EnterInGame(spawnPos);
            Remains = (EffectData.Duration < 0.0f && EffectData.Period < 0.0f) ? 
                      float.MaxValue : EffectData.Duration * EffectData.Period;

            if (EffectData.Duration < 0.0f)
            {
                Remains = float.MaxValue;
                EffectClearType = EEffectClearType.ByCondition;

                OnRemoveSelfByConditionHandler -= OnRemoveSelfByCondition;
                OnRemoveSelfByConditionHandler += OnRemoveSelfByCondition;
            }
            else
            {
                Remains = EffectData.Duration * EffectData.Period;
                EffectClearType = EEffectClearType.TimeOut;
            }

            transform.position = EffectSpawnInfo(EffectData.EffectSpawnType);
        }

        private void InitialSetSize(EObjectSize objSize)
        {
            switch (objSize)
            {
                case EObjectSize.None:
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

        private Vector3 EffectSpawnInfo(EEffectSpawnType effectSpawnType)
        {
            if (effectSpawnType == EEffectSpawnType.None)
                return SpawnedPos;
            else if (effectSpawnType == EEffectSpawnType.SetParentOwner)
            {
                transform.SetParent(Owner.transform);
                return SpawnedPos;
            }

            SkillBase currentSkill = Owner.GetComponent<Creature>().CreatureSkill.CurrentSkill;
            _enteredDir = currentSkill.EnteredTargetDir;
            _enteredSignX = currentSkill.EnteredSignX;

            if (effectSpawnType == EEffectSpawnType.SkillFromOwner)
            {
                SpawnedPos = currentSkill.EnteredOwnerPos;
                return currentSkill.EnteredOwnerPos;
            }
            else if (effectSpawnType == EEffectSpawnType.SkillFromTarget)
            {
                SpawnedPos = currentSkill.EnteredTargetPos;
                return currentSkill.EnteredTargetPos;
            }

            return SpawnedPos;
        }
        #endregion

        public virtual void ApplyEffect()
        {
            if (EffectClearType == EEffectClearType.TimeOut)
                StartCoroutine(CoStartLifeTimer());

            EnterEffect();
        }
        public abstract void EnterEffect();
        public abstract void ExitEffect();

        public abstract void DoEffect();
        protected virtual void ProcessDot() { }

        protected IEnumerator CoStartLifeTimer()
        {
            if (EffectType == EEffectType.Airborne || EffectType == EEffectType.Knockback)
                yield break;

            ProcessDot();
            float tickTimer = 0f;
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

            Remains = 0f;
            //ClearEffect(EEffectClearType.TimeOut);
            Owner.BaseEffect.RemoveEffect(this);
        }

        public void ClearEffect(EEffectClearType clearType)
        {
             if (Owner.IsValid() == false)
                return;
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
        // public virtual bool ClearEffect(EEffectClearType clearType)
        // {
        //     if (Owner.IsValid() == false)
        //     {
        //         Owner.BaseEffect.RemoveEffect(this);
        //         // Managers.Object.Despawn(this, DataTemplateID);
        //         return false;
        //     }

        //     switch (clearType)
        //     {
        //         case EEffectClearType.Disable:
        //         case EEffectClearType.TimeOut:
        //             {
        //                 Owner.BaseEffect.RemoveEffect(this);
        //                 break;
        //             }

        //         case EEffectClearType.EndOfCC:
        //             {
        //                 Owner.BaseEffect.RemoveEffect(this);
        //                 //Owner.CreatureEffect.RemoveEffect(this);
        //                 break;
        //             }

        //         case EEffectClearType.ByCondition:
        //             {
        //                 break;
        //             }
        //     }

        //     return true;
        // }

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

        protected IEnumerator CoStartTimer()
        {
            if (EffectType == EEffectType.Airborne || EffectType == EEffectType.Knockback)
                yield break;

            float tickTimer = 0f;
            ProcessDot();
            if (EffectType == EEffectType.Instant) // -- ???
            {
                yield return new WaitForSeconds(1f);
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
*/