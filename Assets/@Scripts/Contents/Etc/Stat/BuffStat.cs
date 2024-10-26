using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            _owner = _baseStat.Owner;
            SetZeroBuffStats();
        }

        public void SetZeroBuffStats()
        {
            Shield = 0.0f;
            BonusHealth = 0.0f;
            Armor = 0.0f;
            Critical = 0.0f;
            Dodge = 0.0f;
            Luck = 0.0f;
            InvincibleBlockCountPerWave = 0;
        }

        public void ApplyBuffStat(EEffectType effectBuffType)
        {
            switch (effectBuffType)
            {
                case EEffectType.BuffStat_MaxHealth:
                    {
                        float baseValue = _baseStat.MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        _baseStat.MaxHealth = baseValue;
                    }
                    break;

                case EEffectType.BuffStat_Damage:
                    {
                        float baseValue = _baseStat.MinDamage;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        _baseStat.MinDamage = baseValue;

                        baseValue = _baseStat.MaxDamage;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        _baseStat.MaxDamage = baseValue;
                    }
                    break;

                case EEffectType.BuffStat_AttackRate:
                    {
                        float baseValue = _baseStat.AttackRate;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        _baseStat.AttackRate = Mathf.Clamp(baseValue, 0.1f, ReadOnly.Util.MaxAttackRate);
                    }
                    break;

                case EEffectType.BuffStat_MovementSpeed:
                    {
                        float baseValue = _baseStat.MovementSpeed;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        _baseStat.MovementSpeed = Mathf.Clamp(baseValue, ReadOnly.Util.MinMovementSpeed, ReadOnly.Util.MaxMovementSpeed);
                    }
                    break;

                case EEffectType.BuffStat_Shield:
                    {
                        float baseValue = _baseStat.MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        Shield = Mathf.Clamp(baseValue - _baseStat.MaxHealth, 0.0f, _baseStat.MaxHealth);
                    }
                    break;

                case EEffectType.BuffStat_BonusHealth:
                    {
                        float baseValue = _baseStat.MaxHealth;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        BonusHealth = Mathf.Clamp(baseValue - _baseStat.MaxHealth, 0.0f, _baseStat.MaxHealth);
                    }
                    break;

                case EEffectType.BuffStat_Armor:
                    {
                        float baseValue = Armor;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        Armor = Mathf.Clamp(baseValue, 0.0f, ReadOnly.Util.MaxArmor);
                    }
                    break;


                case EEffectType.BuffStat_Critical:
                    {
                        float baseValue = Critical;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        Critical = Mathf.Clamp(baseValue, 0.0f, 1.0f);
                    }
                    break;

                case EEffectType.BuffStat_Dodge:
                    {
                        float baseValue = Dodge;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        Dodge = Mathf.Clamp(baseValue, 0.0f, 1.0f);
                    }
                    break;

                case EEffectType.BuffStat_Luck:
                    {
                        float baseValue = Luck;
                        baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
                        baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
                        Luck = Mathf.Clamp(baseValue, 0.0f, ReadOnly.Util.MaxLuck);
                    }
                    break;

                case EEffectType.BuffStat_InvincibleBlockCountPerWave:
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
