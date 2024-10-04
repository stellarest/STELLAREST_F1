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
    // Hero LevelUp Info
    // --- 히어로별 1개의 Passive Buff 고정
    public class SubStat : InitBase
    {
        public const float c_ZeroBase = 0.0f;
        private BaseStat _baseStat = null;
        [field: SerializeField] public float BonusHealth { get; set; } = 0.0f;
        [field: SerializeField] public float FixedBonusAttackAmount { get; set; } = 0.0f;
        public float FixedBonusAttackAmountBase { get; private set; } = 0.0f;
        [field: SerializeField] public float ArmorRate { get; set; } = 0.0f;
        public float DamageReductionRateBase { get; private set; } = 0.0f;

        [field: SerializeField] public float DebuffResistanceRate { get; set; } = 0.0f;
        public float DebuffResistanceRateBase { get; private set; } = 0.0f;

        [field: SerializeField] public int InvincibleCountPerWave { get; set; } = 0;
        public int InvincibleCountPerWaveBase { get; private set; } = 0;

        public void InitialSetInfo(BaseStat baseStat)
        {
            _baseStat = baseStat;
            BonusHealth = c_ZeroBase;
            FixedBonusAttackAmount = c_ZeroBase;
            ArmorRate = c_ZeroBase;
            DebuffResistanceRate = c_ZeroBase;
            InvincibleCountPerWave = (int)c_ZeroBase;
        }

        public void ApplySubStat(EApplyStatType statType)
        {
            float baseValue = c_ZeroBase;
            if (statType == EApplyStatType.BonusHealth)
            {
                // BonusHealth의 Base는 MaxHealth
                baseValue = _baseStat.MaxHealth;

                // 1. FIXED AMOUNT
                baseValue += _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.BonusHealth,
                                                statModType: EStatModType.AddAmount); // + ITEM + STAT + ETC...

                // 2. ADD PERCENT
                baseValue *= 1 + _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.BonusHealth,
                                                statModType: EStatModType.AddPercent);

                // 3. ADD PERCENT MULTI
                baseValue *= 1 + _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.BonusHealth,
                                                statModType: EStatModType.AddPercentMulti);

                BonusHealth = Mathf.Clamp(baseValue - _baseStat.MaxHealth, 0.0f, _baseStat.MaxHealth);
            }
            else if (statType == EApplyStatType.Armor)
            {
                baseValue += _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.Armor,
                                                statModType: EStatModType.AddAmount);

                baseValue *= 1 + _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.Armor,
                                                statModType: EStatModType.AddPercent);

                baseValue *= 1 + _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.Armor,
                                                statModType: EStatModType.AddPercentMulti);

                ArmorRate = Mathf.Clamp(baseValue, 0.0f, ReadOnly.Util.MaxArmorRate);

                // --- PREV: 0.5이상부터 곱연산으로 했던 부분
                // --- 데미지 감소율은 Fixed AddAmount로 최대 50%까지 가능.
                // baseValue = Mathf.Clamp(baseValue, 0.0f, 0.5f);
                // if (baseValue >= 0.5f)
                // {
                //     // --- 전부 곱연산으로 적용 (로직 재확인 필요)
                //     baseValue *= 1 - (1 - baseValue) * (1 - _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.Armor,
                //                 statModType: EStatModType.AddAmount));

                //     baseValue *= 1 - (1 - baseValue) * (1 - _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.Armor,
                //                 statModType: EStatModType.AddPercent));

                //     baseValue *= 1 - (1 - baseValue) * (1 - _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.Armor,
                //                 statModType: EStatModType.AddPercentMulti));
                // }

                // --- 그 이후로는 곱연산을 적용하여 100%의 피해 감소율을 막는다(데미지 감소율로 인한 100% 무적상태 방지)
                // --- 삭제 금지. 이게 맞음.
                // baseValue *= 1 - (1 - baseValue) * ( 1 - _baseStat.Owner.BaseEffect.GetStatModifier(applyStatType: EApplyStatType.DamageReductionRate,
                //                                 statModType: EStatModType.AddPercentMulti))
                //                                 * (1 - Inventory.Armor)
                //                                 * (1 - TrainingStat.Endurance);

                // ArmorRate = baseValue;
            }
        }
    }

    public class BaseStat : InitBase
    {
        private int _dataTemplateID = -1;
        public BaseCellObject Owner { get; private set; } = null;
        [SerializeField] private SubStat _subStat = null;

        // --- Main
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

        [field: SerializeField] public float AttackRate { get; set; } = 0.0f;
        public float AttackRateBase { get; private set; } = 0.0f;

        [field: SerializeField] public float CriticalRate { get; set; } = 0.0f;
        public float CriticalRateBase { get; private set; } = 0.0f;

        [field: SerializeField] public float DodgeRate { get; set; } = 0.0f;
        public float DodgeRateBase { get; private set; } = 0.0f;

        [field: SerializeField] public float MovementSpeed { get; set; } = 0.0f;
        public float MovementSpeedBase { get; private set; } = 0.0f;

        [field: SerializeField] public float Luck { get; set; } = 0.0f;
        public float LuckBase { get; private set; } = 0.0f;

        // --- Util SubStat
        public float BonusHealth { get => _subStat.BonusHealth; set => _subStat.BonusHealth = value; }
        public float FixedBonusAttackAmount { get => _subStat.FixedBonusAttackAmount; set => _subStat.FixedBonusAttackAmount = value; }
        public float ArmorRate { get => _subStat.ArmorRate; set => _subStat.ArmorRate = value; }
        public float DebuffResistanceRate { get => _subStat.DebuffResistanceRate; set => _subStat.DebuffResistanceRate = value; }
        public int InvincibleCountPerWave { get => _subStat.InvincibleCountPerWave; set => _subStat.InvincibleCountPerWave = value; }

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

        public int MaxLevel => (_maxLevelID % _dataTemplateID) + 1; // --- 1부터 시작하므로
        [SerializeField] protected int _levelID = -1;
        public int LevelID => _levelID;

        [SerializeField] protected int _maxLevelID = -1;
        public bool IsMaxLevel => _levelID == _maxLevelID;

        public bool LevelUp()
        {
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
                for (int i = dataID; i < dataID + ReadOnly.Util.HeroMaxLevel;)
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
            (Owner as Creature).CreatureAnim.SetAttackRate(creatureData.AttackRate);

            CriticalRate = CriticalRateBase = creatureData.CriticalRate;
            DodgeRate = DodgeRateBase = creatureData.DodgeRate;
            MovementSpeed = MovementSpeedBase = creatureData.MovementSpeed;
            Luck = LuckBase = creatureData.Luck;
            // + ApplyEffectStat (_levelID) -- 여기다가 하면 안됨...
        }

        public void ApplyStat()
        {
            if (Owner.IsValid() == false)
                return;

            // ************ TEST ************
            // if (Owner.ObjectType == EObjectType.Monster)
            //     return;
            // ******************************

            float prevMaxHealth = MaxHealth;
            for (int i = 0; i < (int)EApplyStatType.Max; ++i)
            {
                EApplyStatType statType = (EApplyStatType)i;
                if (Util.IsBaseStat(statType))
                    ApplyBaseStat(statType);
                else
                    _subStat.ApplySubStat(statType);
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
        private void ApplyBaseStat(EApplyStatType statType)
        {
            if (statType == EApplyStatType.MaxHealth)
            {
            }
            else if (statType == EApplyStatType.AttackPower)
            {
            }
        }
    }
}
