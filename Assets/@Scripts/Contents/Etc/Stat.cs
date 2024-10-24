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
    public class BuffStat : InitBase
    {
        private BaseStat _baseStat = null;
        private BaseCellObject _owner = null;

        [field: SerializeField] public float Shield { get; set; } = 0.0f;
        [field: SerializeField] public float BonusHealth { get; set; } = 0.0f;
        [field: SerializeField] public float Armor { get; set; } = 0.0f;
        [field: SerializeField] public float Critical { get; set; } = 0.0f;
        [field: SerializeField] public float Dodge { get; set; } = 0.0f;
        [field: SerializeField] public float Luck { get; set; } = 0.0f;
        [field: SerializeField] public int InvincibleBlockCountPerWave { get; set; } = 0;

        public void InitialSetInfo(BaseStat baseStat)
        {
            _baseStat = baseStat;
            _owner = baseStat.Owner;
        }

        public void ApplyBuffStat(EEffectType effectType)
        {
            switch (effectType)
            {
                case EEffectType.BuffStat_MaxHealth:
                    {
                        float baseValue = _baseStat.MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        _baseStat.MaxHealth = baseValue;
                    }
                    break;

                case EEffectType.BuffStat_Damage:
                    {
                        float baseValue = _baseStat.MinDamage;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        _baseStat.MinDamage = baseValue;

                        baseValue = _baseStat.MaxDamage;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        _baseStat.MaxDamage = baseValue;
                    }
                    break;

                case EEffectType.BuffStat_AttackRate:
                    {
                        float baseValue = _baseStat.AttackRate;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        _baseStat.AttackRate = Mathf.Clamp(baseValue, 0.1f, ReadOnly.Util.MaxAttackRate);
                    }
                    break;

                case EEffectType.BuffStat_MovementSpeed:
                    {
                        float baseValue = _baseStat.MovementSpeed;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        _baseStat.MovementSpeed = _baseStat.MovementSpeed;
                    }
                    break;

                case EEffectType.BuffStat_Shield:
                    {
                        float baseValue = _baseStat.MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        BonusHealth = Mathf.Clamp(baseValue - _baseStat.MaxHealth, 0.0f, _baseStat.MaxHealth);
                    }
                    break;

                case EEffectType.BuffStat_BonusHealth:
                    {
                        float baseValue = _baseStat.MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        Shield = Mathf.Clamp(baseValue - _baseStat.MaxHealth, 0.0f, _baseStat.MaxHealth);
                    }
                    break;

                case EEffectType.BuffStat_Armor:
                    {
                        Debug.Log(_baseStat.MaxHealth);
                        float baseValue = Armor;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        Armor = Mathf.Clamp(baseValue, 0.0f, ReadOnly.Util.MaxArmor);
                    }
                    break;


                case EEffectType.BuffStat_Critical:
                    {
                        float baseValue = Critical;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        Critical = Mathf.Clamp(baseValue, 0.0f, 1.0f);
                    }
                    break;

                case EEffectType.BuffStat_Dodge:
                    {
                        float baseValue = Dodge;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        Critical = Mathf.Clamp(Dodge, 0.0f, 1.0f);
                    }
                    break;

                case EEffectType.BuffStat_Luck:
                    {
                        float baseValue = Luck;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        Critical = Mathf.Clamp(Dodge, 0.0f, ReadOnly.Util.MaxLuck);
                    }
                    break;

                case EEffectType.BuffStat_InvincibleBlockCountPerWave:
                    {
                        int baseValue = InvincibleBlockCountPerWave;
                        baseValue += (int)_owner.BaseEffect.GetStatModifier(effectType, EStatModType.Amount);
                        InvincibleBlockCountPerWave = baseValue;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// MaxHealth, Damage, AttackRate, MovementSpeed
    /// </summary>
    public class BaseStat : InitBase
    {
        private int _dataTemplateID = -1;
        public BaseCellObject Owner { get; private set; } = null;
        [SerializeField] private BuffStat _buffStat = null;

        // --- Main Stat
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
                    _attackRate = value;
                    (Owner as Creature).CreatureAnim.SetAttackRate(value);
                }
            }
        }
        public float AttackRateBase { get; private set; } = 0.0f;

        [field: SerializeField] public float MovementSpeed { get; set; } = 0.0f;
        public float MovementSpeedBase { get; private set; } = 0.0f;

        // --- Level
        public int Level
        {
            get
            {
                int level = (_levelID % _dataTemplateID) + 1;
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
        public bool IsMaxLevel => _levelID == _maxLevelID;

        public bool LevelUp()
        {
            // Effect_BuffDamageReduction: 바꿔야되는데. 데이터에서 받게.
            if (Owner.IsValid() == false)
                return false;

            if (IsMaxLevel)
            {
                Debug.Log($"<color=magenta>{Owner.Dev_NameTextID} is already MaxLv !!</color>");
                return false;
            }

            EObjectType objType = Owner.ObjectType;
            if (objType == EObjectType.Env)
                return false;

            _levelID = Mathf.Clamp(_levelID + 1, _dataTemplateID, _maxLevelID);
            SetBaseStat();

            // + Apply Effect Stat
            Debug.Log($"<color=white>Success to LvUp - Lv: {Level} / {MaxLevel}</color>");
            return true;
        }

        public void InitialSetInfo(int dataID, BaseCellObject owner)
        {
            _dataTemplateID = dataID;
            _levelID = dataID;
            Owner = owner;
            InitBaseStat(dataID, owner);

            _buffStat = Owner.gameObject.GetOrAddComponent<BuffStat>();
            _buffStat.InitialSetInfo(baseStat: this);
            if (Owner.ObjectType == EObjectType.Hero)
            {
                for (int i = dataID; i < dataID + ReadOnly.Util.HeroMaxLevel;)
                    _maxLevelID = i++;
            }
            else
                _maxLevelID = dataID;

            Dev_NameTextID = $"Lv: {((_levelID % _dataTemplateID) + 1).ToString()} / {MaxLevel.ToString()}";
        }

        private void InitBaseStat(int dataID, BaseCellObject owner)
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

        public void SetBaseStat() // ***EnterInGame ***
        {
            if (Util.IsCreatureType(Owner))
            {
                MaxHealth = MaxHealthBase;
                Health = MaxHealthBase;
                MinDamage = MinDamageBase;
                MaxDamage = MaxDamageBase;
                AttackRate = AttackRateBase;
                MovementSpeed = MovementSpeedBase;
            }
            else // EnvType
                MaxHealth = MaxHealthBase;

            // CreatureData creatureData = null;
            // switch (Owner.ObjectType)
            // {
            //     case EObjectType.Hero:
            //         creatureData = (Owner as Hero).HeroData;
            //         break;

            //     case EObjectType.Monster:
            //         creatureData = (Owner as Monster).MonsterData;
            //         break;

            //     case EObjectType.Env:
            //         {
            //             EnvData envData = (Owner as Env).EnvData;
            //             Health = envData.MaxHealth;
            //             MaxHealth = MaxHealthBase = envData.MaxHealth;
            //             return;
            //         }
            // }

            // // MaxHealth = MaxHealthBase = creatureData.MaxHealth;
            // // Health = creatureData.MaxHealth;
            // // MinDamage = MinDamageBase = creatureData.MinDamage;
            // // MaxDamage = MaxDamageBase = creatureData.MaxDamage;
            // // AttackRate = AttackRateBase = creatureData.AttackRate;
            // // MovementSpeed = MovementSpeedBase = creatureData.MovementSpeed;
            // MaxHealth = MaxHealthBase;
            // Health = MaxHealthBase;
            // MinDamage = MinDamageBase;
            // MaxDamage = MaxDamageBase;
            // AttackRate = AttackRateBase;
            // MovementSpeed = MovementSpeedBase;
        }

        public void ApplyBuffStat()
        {
            float prevMaxHealth = MaxHealth;
            for (int i = 0; i < (int)EEffectType.Max; ++i)
            {
                EEffectType effectType = (EEffectType)i;
                if (Util.IsEffectBuffStat(effectType))
                    _buffStat.ApplyBuffStat(effectType);
            }

            if (prevMaxHealth != MaxHealth)
            {
                // 현재의 체력을 증가된 최대 체력 만큼의 비율로 조정한다.
                // (Health / prevMaxHealth); 이전 Ratio
                Health = MaxHealth * (Health / prevMaxHealth);
                Health = Mathf.Clamp(Health, 0.0f, MaxHealth);
            }

            // _healthBar.Refresh(Health / MaxHealth)
        }

        #region Util: Buff Stat
        public float BonusHealth { get => _buffStat.BonusHealth; set => _buffStat.BonusHealth = value; }
        public float Shield { get => _buffStat.Shield; set => _buffStat.Shield = value; }
        public float Armor { get => _buffStat.Armor; set => _buffStat.Armor = value; }
        public float Critical { get => _buffStat.Critical; set => _buffStat.Critical = value; }
        public float Dodge { get => _buffStat.Dodge; set => _buffStat.Dodge = value; }
        public float Luck { get => _buffStat.Luck; set => _buffStat.Luck = value; }
        public int InvincibleBlockCountPerWave { get => _buffStat.InvincibleBlockCountPerWave; set => _buffStat.InvincibleBlockCountPerWave = value; }
        #endregion
    }
}