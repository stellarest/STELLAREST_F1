using System.Collections;
using System.Collections.Generic;
using System.Security;
using STELLAREST_F1.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    /// <summary>
    /// Main: MaxHealth, Min/Max Damage, AttackRate, MovementSpeed
    /// Sub: Shield, BonusHealth, Armor, Critical, Dodge, Luck, InvincibleBlockCount
    /// </summary>
    public class BaseStat : InitBase
    {
        private int _dataTemplateID = -1;
        public BaseCellObject Owner { get; private set; } = null;
        [SerializeField] private StatModifier _modifier = null;

        #region Main Stats
        [SerializeField] private float _health = 0.0f;
        public float Health
        {
            get => _health;
            set
            {
                _health = value;
                if (_health < 0.0f)
                    _health = 0.0f;
            }
        }

        [field: SerializeField] public float MaxHealth { get; set; } = 0.0f;
        public float MaxHealthBase { get; private set; } = 0.0f;

        [field: SerializeField] public float MinDamage { get; set; } = 0.0f;
        public float MinDamageBase { get; private set; } = 0.0f;

        [field: SerializeField] public float MaxDamage { get; set; } = 0.0f;
        public float MaxDamageBase { get; private set; } = 0.0f;
        public float Damage => Mathf.Max(UnityEngine.Random.Range(MinDamage, MaxDamage), 1.0f);

        [SerializeField] private float _attackRate = 0.0f;
        public float AttackRate
        {
            get => _attackRate;
            set
            {
                if (Owner == null)
                    return;

                if (_attackRate != value)
                {
                    _attackRate = Mathf.Clamp(value,
                            min: CFloat.Min(EFloat.Range_AttackRate),
                            max: CFloat.Max(EFloat.Range_AttackRate));

                    (Owner as Creature).CreatureAnim.SetAttackRate(_attackRate);
                }
            }
        }
        public float AttackRateBase { get; private set; } = 0.0f;

        [SerializeField] private float _collectRate = 0.0f;
        public float CollectRate
        {
            get => _collectRate;
            set
            {
                if (Owner == null)
                    return;

                _collectRate = Mathf.Clamp(value,
                        min: CFloat.Min(EFloat.Range_CollectRate),
                        max: CFloat.Max(EFloat.Range_CollectRate));

                (Owner as Creature).CreatureAnim.SetCollectRate(_collectRate);
            }
        }
        public float CollectRateBase { get; private set; } = 0.0f;


        [SerializeField] private float _movementSpeed = 0.0f;
        public float MovementSpeed
        {
            get => _movementSpeed;
            set
            {
                if (Owner == null)
                    return;

                if (_movementSpeed != value)
                {
                    _movementSpeed = Mathf.Clamp(value,
                            min: CFloat.Min(EFloat.Range_MovementSpeed),
                            max: CFloat.Max(EFloat.Range_MovementSpeed));

                    (Owner as Creature).CreatureAnim.SetMovementSpeed(_movementSpeed);
                }
            }
        }
        public float MovementSpeedBase { get; private set; } = 0.0f;
        #endregion

        #region Level
        public int Level
        {
            get
            {
                //int level = (_levelID % _dataTemplateID) + 1;
                int level = Mathf.Min((_levelID % _dataTemplateID) + 1, CInt.CValue(EInt.CValue_HeroMaxLevel));
#if UNITY_EDITOR
                Dev_NameTextID = $"Lv: {level.ToString()} / {MaxLevel.ToString()}";
#endif
                return level;
            }
        }

        public int MaxLevel => (_maxLevelID % _dataTemplateID) + 1;

        [SerializeField] protected int _levelID = -1;
        public int LevelID => _levelID;

        [SerializeField] protected int _maxLevelID = -1;
        public int MaxLevelID => _maxLevelID;
        public bool IsMaxLevel => _levelID == _maxLevelID;
        #endregion

        #region Sub Stats
        [field: SerializeField] public float Shield { get; set; } = 0.0f;
        [field: SerializeField] public float BonusHealth { get; set; } = 0.0f;
        [field: SerializeField] public float Armor { get; set; } = 0.0f;
        [field: SerializeField] public float Critical { get; set; } = 0.0f;
        [field: SerializeField] public float Dodge { get; set; } = 0.0f;
        [field: SerializeField] public float Luck { get; set; } = 0.0f;
        [field: SerializeField] public int InvincibleBlockCountPerWave { get; set; } = 0;
        #endregion

        #region Public Stat
        // 
        // Luck
        // ExpUp
        // 
        #endregion

        public void InitialSetInfo(int dataID, BaseCellObject owner)
        {
            _dataTemplateID = dataID;
            _levelID = dataID;
            Owner = owner;
            InitMainStats(dataID, owner);
            SubStatsToZero();

            _modifier = Owner.gameObject.GetOrAddComponent<StatModifier>();
            _modifier.InitialSetInfo(baseStat: this);
            if (Owner.ObjectType == EObjectType.Hero)
            {
                for (int i = dataID; i < dataID + CInt.CValue(EInt.CValue_HeroMaxLevel);)
                    _maxLevelID = i++;
            }
            else
                _maxLevelID = dataID;

#if UNITY_EDITOR
            Dev_NameTextID = $"Lv: {((_levelID % _dataTemplateID) + 1).ToString()} / {MaxLevel.ToString()}";
#endif
        }

        private void InitMainStats(int dataID, BaseCellObject owner)
        {
            if (Util.IsCreatureType(owner))
            {
                CreatureData creatureData = Util.GetCreatureData(dataID, owner as Creature);
                if (creatureData == null)
                    return;

                MaxHealth = MaxHealthBase = creatureData.MaxHealth;
                Health = MaxHealth;

                MinDamage = MinDamageBase = creatureData.MinDamage;
                MaxDamage = MaxDamageBase = creatureData.MaxDamage;
                AttackRate = AttackRateBase = creatureData.AttackRate;
                CollectRate = CollectRateBase = creatureData.CollectRate;
                MovementSpeed = MovementSpeedBase = creatureData.MovementSpeed;
            }
            else
            {
                if (Managers.Data.EnvDataDict.TryGetValue(dataID, out EnvData envData) == false)
                    return;

                MaxHealth = MaxHealthBase = envData.MaxHealth;
                Health = MaxHealth;
            }
        }

        //public void TEMP_MAX_HEALTH() => Health = MaxHealth;
        public void FullHealth()
            => Health = MaxHealth;

        public void SetBaseStats(bool currentHealthToMax = false)
        {
            if (Owner.ObjectType == EObjectType.Env)
            {
                Health = MaxHealth;
                return;
            }

            MaxHealth = MaxHealthBase;
            if (currentHealthToMax)
                Health = MaxHealth;

            MinDamage = MinDamageBase;
            MaxDamage = MaxDamageBase;
            AttackRate = AttackRateBase;
            MovementSpeed = MovementSpeedBase;
        }

        private void SubStatsToZero()
        {
            Shield = 0.0f;
            BonusHealth = 0.0f;
            Armor = 0.0f;
            Critical = 0.0f;
            Dodge = 0.0f;
            Luck = 0.0f;
            InvincibleBlockCountPerWave = 0;
        }

        public bool LevelUp()
        {
            if (Owner.IsValid() == false)
            {
                Dev.LogWarning(obj: nameof(BaseStat), method: nameof(LevelUp), log: "Owner is not valid.");
                return false;
            }

            EObjectType objType = Owner.ObjectType;
            if (objType == EObjectType.Env)
                return false;

            if (IsMaxLevel)
            {
                Dev.Log($"{Owner.Dev_NameTextID} is already MaxLv.", highlight: true);
                return false;
            }

            _levelID = Mathf.Clamp(_levelID + 1, _dataTemplateID, _maxLevelID);
            Dev.Log($"Lv: {Level}/{MaxLevel}");
            // RefreshAllStats(currentHealthToMax: true);
            return true;
        }

        public void ApplyStat(int effectID, EEffectType effectType, bool addStat = true)
        {
            if (Util.IsEffectStatType(effectType) == false)
                return;

            float prevMaxHealth = MaxHealth;
            _modifier.ApplyStat(effectID, effectType, addStat);
            if (prevMaxHealth != MaxHealth)
            {
                float currentRatio = Health / prevMaxHealth;
                Health = Mathf.Clamp(MaxHealth * currentRatio, 0.0f, MaxHealth);
            }

            // --- + TODO: Refresh HP Bar UI
        }

        public void ApplyStatToTarget(int effectID, EEffectType effectType, BaseCellObject target, bool addStat = true)
        {
            if (target.IsValid() == false)
            {
                // TODO: 이미 타겟이 죽어있다면 ActiveEffects에서 가지고 있는 것을 제거(이펙트 제거)
                return;
            }

            if (Util.IsEffectStatType(effectType) == false)
                return;

            float prevTargetMaxHealth = target.MaxHealth;

            // --- 내가 가지고 있는 이펙트로 타겟에게 적용
            this._modifier.ApplyStatToTarget(effectID, effectType, target, addStat);
            
            if (prevTargetMaxHealth != target.MaxHealth)
            {
                float currentRatio = target.Health / prevTargetMaxHealth;
                target.Health = Mathf.Clamp(target.MaxHealth * currentRatio, 0.0f, target.MaxHealth);
            }
        }
    }
}
/*
        // PREV
        // public void ApplyBuffStat(EEffectType effectBuffType)
        // {
        //     if (Util.IsEffectStatType(effectBuffType) == false)
        //         return;

        //     // --- InGameMode(Not UIMode) 도중 버프등을 받았을 때, 비율대로 적용해야함.
        //     float prevMaxHealth = MaxHealth;
        //     _modifier.ApplyBuffStat(effectBuffType);
        //     if (prevMaxHealth != MaxHealth)
        //     {
        //         float prevRatio = Health / prevMaxHealth;
        //         Health = Mathf.Clamp(MaxHealth * prevRatio, 0.0f, MaxHealth);
        //     }

        //     // --- +++ Refresh HP Bar
        // }

        // public void RemoveBuffStat(EEffectType effectBuffType)
        // {
        //     if (Util.IsEffectStatType(effectBuffType) == false)
        //         return;

        //     float prevMaxHealth = MaxHealth;
        //     _modifier.RemoveBuffStat(effectBuffType);
        //     if (prevMaxHealth != MaxHealth)
        //     {
        //         float prevRatio = Health / prevMaxHealth;
        //         Health = Mathf.Clamp(MaxHealth * prevRatio, 0.0f, MaxHealth);
        //     }

        //     // --- +++ Refresh HP Bar
        // }

        // public void ApplyBuffStat()
        // {
        //     float prevMaxHealth = MaxHealth;
        //     for (int i = 0; i < (int)EEffectType.Max; ++i)
        //     {
        //         EEffectType effectType = (EEffectType)i;
        //         if (Util.IsEffectBuffType(effectType))
        //             _buffStat.ApplyBuffStat(effectType);
        //     }

        //     if (prevMaxHealth != MaxHealth)
        //     {
        //         // 현재의 체력을 증가된 최대 체력 만큼의 비율로 조정한다.
        //         // (Health / prevMaxHealth); 이전 Ratio
        //         Health = MaxHealth * (Health / prevMaxHealth);
        //         Health = Mathf.Clamp(Health, 0.0f, MaxHealth);
        //     }

        //     // _healthBar.Refresh(Health / MaxHealth)
        // }

        // public void SetBaseStat() // ***EnterInGame ***
        // {
        //     if (Util.IsCreatureType(Owner))
        //     {
        //         MaxHealth = MaxHealthBase;
        //         MinDamage = MinDamageBase;
        //         MaxDamage = MaxDamageBase;
        //         AttackRate = AttackRateBase;
        //         MovementSpeed = MovementSpeedBase;
        //     }
        //     else
        //         MaxHealth = MaxHealthBase;
        // }

         // public void RefreshAllStats(bool currentHealthToMax = false)
        // {
        //     if (Owner.ObjectType == EObjectType.Env)
        //     {
        //         Health = MaxHealth;
        //         return;
        //     }

        //     SubStatsToZero();
        //     MaxHealth = MaxHealthBase;
        //     MinDamage = MinDamageBase;
        //     MaxDamage = MaxDamageBase;
        //     AttackRate = AttackRateBase;
        //     MovementSpeed = MovementSpeedBase;
        //     for (int i = 0; i < (int)EEffectType.Max; ++i)
        //     {
        //         EEffectType effectType = (EEffectType)i;
        //         if (Util.IsEffectStatType(effectType))
        //             ApplyStat(effectType);
        //     }

        //     if (currentHealthToMax)
        //         Health = MaxHealth;
        // }

        // public void RefreshAllStats()
        // {
        //     SetBaseStats();
        //     // SubStatsToZero(); // --- 근데 이렇게하면 지워지는데...
        //     // --- + TODO: ClearAllDebuffs
        //     for (int i = 0; i < (int)EEffectType.Max; ++i)
        //     {
        //         EEffectType effectType = (EEffectType)i;
        //         if (Util.IsEffectStatType(effectType))
        //             _modifier.ApplyBuffStat(effectType);
        //     }

        //     Health = MaxHealth;
        // }
*/