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
        [field: SerializeField] public float BonusHealth { get; set; } = 0.0f;
        public float BonusHealthBase { get; private set; } = 0.0f;

        [field: SerializeField] public float FixedBonusAttackAmount { get; set; } = 0.0f;
        public float FixedBonusAttackAmountBase {get; private set; } = 0.0f;

        [field: SerializeField] public float DamageReductionRate { get; set; } = 0.0f;
        public float DamageReductionRateBase { get; private set; } = 0.0f;

        [field: SerializeField] public float DebuffResistanceRate { get; set; } = 0.0f;
        public float DebuffResistanceRateBase { get; private set; } = 0.0f;

        [field: SerializeField] public int InvincibleCountPerWave { get; set; } = 0;
        public int InvincibleCountPerWaveBase { get; private set; } = 0;

        public void SetZero()
        {
            BonusHealth = BonusHealthBase = 0.0f;
            FixedBonusAttackAmount = FixedBonusAttackAmountBase = 0.0f;
            DamageReductionRate = DamageReductionRateBase = 0.0f;
            DebuffResistanceRate = DebuffResistanceRateBase = 0.0f;
            InvincibleCountPerWave = InvincibleCountPerWaveBase = 0;
        }
    }

    public class BaseStat : InitBase
    {
        private int _dataTemplateID = -1;
        private BaseCellObject _owner = null;
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

        [field: SerializeField] public float MinAttack { get; set; } = 0.0f;
        public float MinAttackBase { get; private set; } = 0.0f;

        [field: SerializeField] public float MaxAttack { get; set; } = 0.0f;
        public float MaxAttackBase { get; private set; } = 0.0f;

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
        public float BonusHealthBase => _subStat.BonusHealthBase;

        public float FixedBonusAttackAmount { get => _subStat.FixedBonusAttackAmount; set => _subStat.FixedBonusAttackAmount = value; }
        public float FixedBonusAttackAmoutBase => _subStat.FixedBonusAttackAmountBase;

        public float DamageReductinoRate { get => _subStat.DamageReductionRate; set => _subStat.DamageReductionRate = value; }
        public float DamageReductionRateBase => _subStat.DamageReductionRateBase;

        public float DebuffResistanceRate { get => _subStat.DebuffResistanceRate; set => _subStat.DebuffResistanceRate = value; }
        public float DebuffResistanceRateBase => _subStat.DebuffResistanceRateBase;

        public int InvincibleCountPerWave { get => _subStat.InvincibleCountPerWave; set => _subStat.InvincibleCountPerWave = value; }
        public int InvincibleCountPerWaveBase => _subStat.InvincibleCountPerWaveBase;

        // --- Level
        public int Level
        {
            get
            {
#if UNITY_EDITOR
                Dev_TextID = $"Lv: {((_levelID % _dataTemplateID) + 1).ToString()} / {MaxLevel.ToString()}";
#endif
                return (_levelID % _dataTemplateID) + 1; // --- 1부터 시작하므로
            }
        }

        public int MaxLevel => (_maxLevelID % _dataTemplateID) + 1; // --- 1부터 시작하므로

        [SerializeField] protected int _levelID = -1;
        public int LevelID => _levelID;

        [SerializeField] protected int _maxLevelID = -1;
        public bool IsMaxLevel => _levelID == _maxLevelID;

        public bool LevelUp()
        {
            if (_owner.IsValid() == false)
                return false;

            if (IsMaxLevel)
            {
                Debug.Log($"<color=magenta>{_owner.Dev_TextID} is already MaxLv !!</color>");
                return false;
            }

            EObjectType objType = _owner.ObjectType;
            if (objType == EObjectType.Env)
                return false;

            _levelID = Mathf.Clamp(_levelID + 1, _dataTemplateID, _maxLevelID);
            switch (objType)
            {
                case EObjectType.Hero:
                    {
                        if (Managers.Data.HeroStatDataDict.TryGetValue(key: _levelID, value: out HeroStatData heroStatData) == false)
                        {
                            Debug.LogError($"{nameof(BaseStat)}::{nameof(LevelUp)}, Failed to load HeroStatData.");
                            return false;
                        }
                    }
                    break;

                case EObjectType.Monster:
                    if (Managers.Data.MonsterStatDataDict.TryGetValue(key: _levelID, value: out MonsterStatData monsterStatData) == false)
                    {
                        Debug.LogError($"{nameof(BaseStat)}::{nameof(LevelUp)}, Failed to load HeroStatData.");
                        return false;
                    }
                    break;
            }

            SetBaseStat();
            Debug.Log($"<color=white>Success to LvUp - Lv: {Level} / {MaxLevel}</color>");
            return true;
        }

        public void InitialSetInfo(int dataID, BaseCellObject owner)
        {
            _dataTemplateID = dataID;
            _levelID = dataID;
            _owner = owner;
            _subStat = _owner.gameObject.GetOrAddComponent<SubStat>();
            _subStat.SetZero();
            if (_owner.ObjectType == EObjectType.Hero)
            {
                for (int i = dataID; i < dataID + ReadOnly.Util.HeroMaxLevel;)
                    _maxLevelID = i++;
            }
            else if (_owner.ObjectType == EObjectType.Monster)
            {
                // _maxLevelID = dataID; (이제 모든 몬스터의 레벨이 1이 아님)
                // *** MonsterMaxLevel이 몬스터 데이터에 추가되었으므로 추후에 MonsterStatData 작성할 때 주의해야함. 
                MonsterData monsterData = Managers.Data.MonsterDataDict[dataID];
                for (int i = dataID; i < dataID + monsterData.MonsterMaxLevel;)
                    _maxLevelID = i++;
            }
            else if (_owner.ObjectType == EObjectType.Env)
                _maxLevelID = dataID;

            Dev_TextID = $"Lv: {((_levelID % _dataTemplateID) + 1).ToString()} / {MaxLevel.ToString()}";
        }

        public void SetBaseStat()
        {
            StatData statData = null;
            switch (_owner.ObjectType)
            {
                case EObjectType.Hero:
                    statData = Managers.Data.HeroStatDataDict[_levelID];
                    break;

                case EObjectType.Monster:
                    statData = Managers.Data.MonsterStatDataDict[_levelID];
                    break;

                case EObjectType.Env:
                    {
                        EnvData envData = Managers.Data.EnvDataDict[_dataTemplateID];
                        Health = envData.MaxHealth;
                        MaxHealth = MaxHealthBase = envData.MaxHealth;
                        return;
                    }
            }

            Health = statData.MaxHealth;
            MaxHealth = MaxHealthBase = statData.MaxHealth;
            MinAttack = MinAttackBase = statData.MinAttack;
            MaxAttack = MaxAttackBase = statData.MaxAttack;
            CriticalRate = CriticalRateBase = statData.CriticalRate;
            DodgeRate = DodgeRateBase = statData.DodgeRate;
            MovementSpeed = MovementSpeedBase = statData.MovementSpeed;
            Luck = LuckBase = statData.Luck;
        }

        /*
        public enum EApplyStatType
        {
            None = -1,
            MaxHp,
            BonusHealth,
            HealthRegenerationRate,
            DamageReductionRate,
            AttackPowerUpRate,
            AttackSpeedUpRate,
            CriticalRate,
            DodgeRate,
            MovementSpeed,
            Max = MovementSpeed + 1
        }
        */

        // --- Main + Buff Stat
        public void ApplyStat()
        {
            if (_owner.IsValid() == false)
                return;

            float prevMaxHealth = MaxHealth;
            for (int i = 0; i < (int)EApplyStatType.Max; ++i)
                ApplyPerStat((EApplyStatType)i);                

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

        private void ApplyPerStat(EApplyStatType statType)
        {
            if (statType == EApplyStatType.BonusHealth)
            {
                //_subStat.BonusHealth = 
            }
        }
    }
}
