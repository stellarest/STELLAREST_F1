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
                    _movementSpeed = Mathf.Clamp(value, ReadOnly.Util.MinMovementSpeed, ReadOnly.Util.MaxMovementSpeed);
                    (Owner as Creature).CreatureAnim.SetMovementSpeed(_movementSpeed);
                }
            }
        }
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

#if UNITY_EDITOR
            Dev_NameTextID = $"Lv: {((_levelID % _dataTemplateID) + 1).ToString()} / {MaxLevel.ToString()}";
#endif
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

            RefreshAllStats(currentHealthToMax: true);
            Debug.Log($"<color=white>Success to LvUp - Lv: {Level} / {MaxLevel}</color>");
            return true;
        }

        public void RefreshAllStats(bool currentHealthToMax = false)
        {
            _buffStat.SetZeroBuffStats();
            float prevMaxHealth = MaxHealth;
            for (int i = 0; i < (int)EEffectType.Max; ++i)
            {
                EEffectType effectType = (EEffectType)i;
                if (Util.IsEffectBuffType(effectType))
                    _buffStat.ApplyBuffStat(effectType);
            }

            if (prevMaxHealth != MaxHealth)
            {
                // 현재의 체력을 증가된 최대 체력 만큼의 비율로 조정한다.
                // (Health / prevMaxHealth); 이전 Ratio
                Health = MaxHealth * (Health / prevMaxHealth);
                Health = Mathf.Clamp(Health, 0.0f, MaxHealth);
            }

            if (currentHealthToMax)
                Health = MaxHealth;
        }

        public void ApplyBuffStat(EEffectType effectBuffType)
        {
            if (Util.IsEffectBuffType(effectBuffType) == false)
                return;

            float prevMaxHealth = MaxHealth;
            _buffStat.ApplyBuffStat(effectBuffType);
            if (prevMaxHealth != MaxHealth)
            {
                float prevRatio = Health / prevMaxHealth;
                Health = Mathf.Clamp(MaxHealth * prevRatio, 0.0f, MaxHealth);
            }
        }

        // PREV
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

/*
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
*/