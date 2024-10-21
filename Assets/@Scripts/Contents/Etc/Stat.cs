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
    public class SubStat : InitBase
    {
        public const float c_ZeroBase = 0.0f;
        private BaseStat _baseStat = null;
        private BaseCellObject _owner = null;

        [field: SerializeField] public float BonusHealth { get; set; } = 0.0f;
        [field: SerializeField] public float BonusHealthShield { get; set; } = 0.0f;
        [field: SerializeField] public float Armor { get; set; } = 0.0f;
        [field: SerializeField] public float CriticalRate { get; set; } = 0.0f;
        [field: SerializeField] public float DodgeRate { get; set; } = 0.0f;
        [field: SerializeField] public float Luck { get; set; } = 0.0f;
        [field: SerializeField] public int InvincibleBlockCountPerWave { get; set; } = 0;

        public void InitialSetInfo(BaseStat baseStat)
        {
            _baseStat = baseStat;
            _owner = baseStat.Owner;
            BonusHealth = BonusHealthShield = Armor = CriticalRate = DodgeRate = Luck = c_ZeroBase;
            InvincibleBlockCountPerWave = (int)c_ZeroBase;
        }

        public void ApplySubStat(EEffectType effectType)
        {
            float baseValue = c_ZeroBase;
            switch (effectType)
            {
                case EEffectType.Buff_SubStat_BonusHealth:
                case EEffectType.Buff_SubStat_BonusHealthShield:
                    {
                        baseValue = _baseStat.MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        if (effectType == EEffectType.Buff_SubStat_BonusHealth)
                            BonusHealth = Mathf.Clamp(baseValue - _baseStat.MaxHealth, 0.0f, _baseStat.MaxHealth);
                        else
                        {
                            BonusHealthShield = Mathf.Clamp(baseValue - _baseStat.MaxHealth, 0.0f, _baseStat.MaxHealth);
                            Debug.Log("Yaho Yaho Yaho");
                        }
                    }
                    break;

                case EEffectType.Buff_SubStat_Armor:
                    {
                        baseValue += _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                        Armor = Mathf.Clamp(baseValue, 0.0f, ReadOnly.Util.MaxArmor);
                    }
                    break;

                // case EEffectType.Buff_SubStat_DamageRate:
                //     {
                //         baseValue = _baseStat.Damage;
                //         baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercent);
                //         baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectType, EStatModType.AddPercentMulti);
                //         Sub_DamageRate = baseValue;
                //     }
                //     break;
            }
        }
    }

    public class BaseStat : InitBase
    {
        private int _dataTemplateID = -1;
        public BaseCellObject Owner { get; private set; } = null;
        [SerializeField] private SubStat _subStat = null;

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

        // --- Util: SubStat
        /*
            Buff_SubStat_BonusHealth,
            Buff_SubStat_BonusHealthShield,
            Buff_SubStat_Armor,
            Buff_SubStat_CriticalRate,
            Buff_SubStat_DodgeRate,
            Buff_SubStat_Luck,
        */
        public float BonusHealth { get => _subStat.BonusHealth; set => _subStat.BonusHealth = value; }
        public float BonusHealthShield { get => _subStat.BonusHealthShield; set => _subStat.BonusHealthShield = value; }
        public float Armor { get => _subStat.Armor; set => _subStat.Armor = value; }
        public float CriticalRate { get => _subStat.CriticalRate; set => _subStat.CriticalRate = value; }
        public float DodgeRate { get => _subStat.DodgeRate; set => _subStat.DodgeRate = value; }
        public float Luck { get => _subStat.Luck; set => _subStat.Luck = value; }
        public int InvincibleBlockCountPerWave { get => _subStat.InvincibleBlockCountPerWave; set => _subStat.InvincibleBlockCountPerWave = value; }

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

            _subStat = Owner.gameObject.GetOrAddComponent<SubStat>();
            _subStat.InitialSetInfo(baseStat: this);
            if (Owner.ObjectType == EObjectType.Hero)
            {
                int heroMaxLevel = Managers.Game.HasElitePackage ? 
                                   ReadOnly.Util.EliteHeroMaxLevel :
                                   ReadOnly.Util.HeroMaxLevel;

                for (int i = dataID; i < dataID + heroMaxLevel;)
                    _maxLevelID = i++;
            }
            else
                _maxLevelID = dataID;

            Dev_NameTextID = $"Lv: {((_levelID % _dataTemplateID) + 1).ToString()} / {MaxLevel.ToString()}";
        }

        public void SetBaseStat()
        {
            CreatureData creatureData = null;
            switch (Owner.ObjectType)
            {
                case EObjectType.Hero:
                    creatureData = (Owner as Hero).HeroData;
                    break;

                case EObjectType.Monster:
                    creatureData = (Owner as Monster).MonsterData;
                    break;

                case EObjectType.Env:
                    {
                        EnvData envData = (Owner as Env).EnvData;
                        Health = envData.MaxHealth;
                        MaxHealth = MaxHealthBase = envData.MaxHealth;
                        return;
                    }
            }

            MaxHealth = MaxHealthBase = creatureData.MaxHealth;
            Health = creatureData.MaxHealth;
            MinDamage = MinDamageBase = creatureData.MinDamage;
            MaxDamage = MaxDamageBase = creatureData.MaxDamage;
            AttackRate = AttackRateBase = creatureData.AttackRate;
            MovementSpeed = MovementSpeedBase = creatureData.MovementSpeed;
        }

        public void ApplyStat()
        {
            Debug.Log(Owner.Dev_NameTextID);
            float prevMaxHealth = MaxHealth;
            for (int i = 0; i < (int)EEffectType.Max; ++i)
            {
                EEffectType effectType = (EEffectType)i;
                if (Util.IsEffectBuffBastStat(effectType))
                    ApplyBaseStat(effectType);
                else if (Util.IsEffectBuffSubStat(effectType))
                    _subStat.ApplySubStat(effectType);
            }

            // MaxHp = MaxHpBase, MaxHpBase가 증가했을 경우
            if (prevMaxHealth != MaxHealth)
            {
                // 현재의 체력을 증가된 최대 체력 만큼의 비율로 조정한다.
                // (Health / prevMaxHealth); 이전 Ratio
                Health = MaxHealth * (Health / prevMaxHealth);
                Health = Mathf.Clamp(Health, 0.0f, MaxHealth);
            }

            // _healthBar.Refresh(Health / MaxHealth)
        }

        // 각각의 스탯을 여기서 별도로 처리한다.
        private void ApplyBaseStat(EEffectType statType)
        {
            if (statType == EEffectType.Buff_BaseStat_MaxHealth)
            {
            }
            else if (statType == EEffectType.Buff_BaseStat_Damage)
            {
            }
        }
    }
}