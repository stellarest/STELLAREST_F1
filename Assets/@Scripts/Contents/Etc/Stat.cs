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

    // --- Buff, Etc Stat
    public class SubStat : InitBase
    {
        [field: SerializeField] public float BonusHealth { get; set; } = 0.0f;
        [field: SerializeField] public float FixedBonusAttack { get; set; } = 0.0f;
        [field: SerializeField] public float DamageReductionRate { get; set; } = 0.0f;
        [field: SerializeField] public float AllDebuffResistance { get; set; } = 0.0f;
        [field: SerializeField] public int InvincibleCount { get; set; } = 0;

        public void RefreshSubStat()
        {
            // ... DO SOMETHING
        }

        public void ApplySubStat()
        {
            // ... DO SOMETHING
        }
    }

    // --- Main Stat
    public class BaseStat : InitBase
    {
        protected int _dataTemplateID = -1;
        protected BaseCellObject _owner = null;
        [SerializeField] private SubStat _subStat = null;

        public void InitialSetInfo(int dataID, BaseCellObject owner)
        {
            _dataTemplateID = dataID;
            _levelID = dataID;
            _owner = owner;
            _subStat = _owner.gameObject.GetOrAddComponent<SubStat>();
            if (_owner.ObjectType == EObjectType.Hero)
            {
                for (int i = dataID; i < dataID + ReadOnly.Util.HeroMaxLevel;)
                    _maxLevelID = i++;
            }
            else if (_owner.ObjectType == EObjectType.Monster)
            {
                MonsterData monsterData = Managers.Data.MonsterDataDict[dataID];
                for (int i = dataID; i < dataID + monsterData.MonsterMaxLevel;)
                    _maxLevelID = i++;
                // // 이런식으로 불러와야할듯
                // // int monsterMaxLv = Managers.Data.MonsterDataDict[dataID].MonsterMaxLevel;
                // _maxLevelID = dataID;
            }
            else if (_owner.ObjectType == EObjectType.Env)
                _maxLevelID = dataID;

            Dev_TextID = $"Lv: {((_levelID % _dataTemplateID) + 1).ToString()} / {MaxLevel.ToString()}";
        }

        public void RefreshStat()
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

            _subStat.RefreshSubStat();
        }

        // --- Main + Buff Stat
        public void ApplyStat()
        {
            _subStat.ApplySubStat();
        }

        // --- Base
        public float MaxHealthBase { get; private set; } = 0.0f;
        public float MinAttackBase { get; private set; } = 0.0f;
        public float MaxAttackBase { get; private set; } = 0.0f;
        public float CriticalRateBase { get; private set; } = 0.0f;
        public float DodgeRateBase { get; private set; } = 0.0f;
        public float MovementSpeedBase { get; private set; } = 0.0f;
        public float LuckBase { get; private set; } = 0.0f;

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
        [field: SerializeField] public float MinAttack { get; set; } = 0.0f;
        [field: SerializeField] public float MaxAttack { get; set; } = 0.0f;
        [field: SerializeField] public float CriticalRate { get; set; } = 0.0f;
        [field: SerializeField] public float DodgeRate { get; set; } = 0.0f;
        [field: SerializeField] public float MovementSpeed { get; set; } = 0.0f;
        [field: SerializeField] public float Luck { get; set; } = 0.0f;

        // --- Sub
        public float BonusHealth { get => _subStat.BonusHealth; set => _subStat.BonusHealth = value; }
        public float DamageReductinoRate { get => _subStat.DamageReductionRate; set => _subStat.DamageReductionRate = value; }
        public float AllDebuffResistance { get => _subStat.AllDebuffResistance; set => _subStat.AllDebuffResistance = value; }
        public int InvincibleCount { get => _subStat.InvincibleCount; set => _subStat.InvincibleCount = value; }

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

            RefreshStat();
            Debug.Log($"<color=white>Success to LvUp - Lv: {Level} / {MaxLevel}</color>");
            return true;
        }
    }
}
