using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class StatModifier : InitBase
    {
        private BaseStat _baseStat = null;
        private BaseCellObject _owner = null;

        #region Util: Main Stats
        public float Health { get => _baseStat.Health; set => _baseStat.Health = value; }
        public float MaxHealth { get => _baseStat.MaxHealth; set => _baseStat.MaxHealth = value; }
        public float MinDamage { get => _baseStat.MinDamage; set => _baseStat.MinDamage = value; }
        public float MaxDamage { get => _baseStat.MaxDamage; set => _baseStat.MaxDamage = value; }
        public float AttackRate { get => _baseStat.AttackRate; set => _baseStat.AttackRate = value; }
        public float MovementSpeed { get => _baseStat.MovementSpeed; set => _baseStat.MovementSpeed = value; } 
        #endregion

        #region Util: Sub Stats
        public float Shield { get => _baseStat.Shield; set => _baseStat.Shield = value; }
        public float BonusHealth { get => _baseStat.BonusHealth; set => _baseStat.BonusHealth = value; }
        public float Armor { get => _baseStat.Armor; set => _baseStat.Armor = value; }
        public float Critical { get => _baseStat.Critical; set => _baseStat.Critical = value; }
        public float Dodge { get => _baseStat.Dodge; set => _baseStat.Dodge = value; }
        public float Luck { get => _baseStat.Luck; set => _baseStat.Luck = value; }
        public int InvincibleBlockCountPerWave
        {
            get => _baseStat.InvincibleBlockCountPerWave;
            set => _baseStat.InvincibleBlockCountPerWave = value;
        }
        #endregion

        public void InitialSetInfo(BaseStat baseStat)
        {
            _baseStat = baseStat;
            _owner = _baseStat.Owner;
        }

        public void ApplyBuffStat(EEffectType effectBuffType)
        {
            switch (effectBuffType)
            {
                case EEffectType.MainStat_MaxHealth:
                    {
                        float baseValue = MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        MaxHealth = baseValue;
                    }
                    break;

                case EEffectType.MainStat_Damage:
                    {
                        float baseValue = _baseStat.MinDamage;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        MinDamage = baseValue;

                        baseValue = _baseStat.MaxDamage;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        MaxDamage = baseValue;
                    }
                    break;

                case EEffectType.MainStat_AttackRate:
                    {
                        float baseValue = _baseStat.AttackRate;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        //_baseStat.AttackRate = Mathf.Clamp(baseValue, 0.1f, ReadOnly.Util.MaxAttackRate);
                        AttackRate = baseValue;
                    }
                    break;

                case EEffectType.MainStat_MovementSpeed:
                    {
                        float baseValue = _baseStat.MovementSpeed;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        //_baseStat.MovementSpeed = Mathf.Clamp(baseValue, ReadOnly.Util.MinMovementSpeed, ReadOnly.Util.MaxMovementSpeed);
                        MovementSpeed = baseValue;
                    }
                    break;

                case EEffectType.SubStat_Shield:
                    {
                        float baseValue = _baseStat.MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        Shield = Mathf.Clamp(baseValue - MaxHealth, 0.0f, MaxHealth);
                    }
                    break;

                case EEffectType.SubStat_BonusHealth:
                    {
                        float baseValue = _baseStat.MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        BonusHealth = Mathf.Clamp(baseValue - MaxHealth, 0.0f, MaxHealth);
                    }
                    break;

                case EEffectType.SubStat_Armor:
                    {
                        float baseValue = Armor;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        //Armor = Mathf.Clamp(baseValue, 0.0f, ReadOnly.Util.MaxArmor);
                        Armor = Mathf.Clamp(baseValue, 0.0f, Util.MaxFloat(EConstFloat.Armor));
                    }
                    break;


                case EEffectType.SubStat_Critical:
                    {
                        float baseValue = Critical;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        Critical = Mathf.Clamp(baseValue, 0.0f, 1.0f);
                    }
                    break;

                case EEffectType.SubStat_Dodge:
                    {
                        float baseValue = Dodge;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        Dodge = Mathf.Clamp(baseValue, 0.0f, 1.0f);
                    }
                    break;

                case EEffectType.SubStat_Luck:
                    {
                        float baseValue = Luck;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        //Luck = Mathf.Clamp(baseValue, 0.0f, ReadOnly.Util.MaxLuck);
                        Luck = Mathf.Clamp(baseValue, 0.0f, Util.MaxFloat(EConstFloat.Luck));
                    }
                    break;

                case EEffectType.SubStat_InvincibleBlockCountPerWave:
                    {
                        int baseValue = InvincibleBlockCountPerWave;
                        baseValue += (int)_owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        InvincibleBlockCountPerWave = baseValue;
                    }
                    break;
            }
        }
    }
}
