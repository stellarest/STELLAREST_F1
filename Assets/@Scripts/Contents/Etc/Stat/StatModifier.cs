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

        // --- 버프, 디버프 모두 가능
        public void ApplyStat(int effectID, EEffectType effectStatType, bool addStat = true)
        {
            float addAmount = _owner.BaseEffect.GetEffectStatModifier(effectID, EStatModType.AddAmount);
            float addPercent = _owner.BaseEffect.GetEffectStatModifier(effectID, EStatModType.AddPercent);
            float addPercentMulti = _owner.BaseEffect.GetEffectStatModifier(effectID, EStatModType.AddPercentMulti);

            switch (effectStatType)
            {
                // --- Main Stats
                case EEffectType.MainStat_MaxHealth:
                    {
                        if (addStat)
                            MaxHealth = (MaxHealth + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
                        else
                            MaxHealth = (MaxHealth - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
                    }
                    break;
                case EEffectType.MainStat_Damage:
                    {
                        if (addStat)
                        {
                            MinDamage = (MinDamage + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
                            MaxDamage = (MaxDamage + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
                        }
                        else
                        {
                            MinDamage = (MinDamage - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
                            MaxDamage = (MaxDamage - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
                        }
                    }
                    break;
                case EEffectType.MainStat_AttackRate:
                    {
                        if (addStat)
                            AttackRate = (AttackRate + addAmount) * (1.0f + addAmount) * (1.0f + addPercentMulti);
                        else
                            AttackRate = (AttackRate - addAmount) / (1.0f + addAmount) / (1.0f + addPercentMulti);
                    }
                    break;
                case EEffectType.MainStat_MovementSpeed:
                    {
                        if (addStat)
                            MovementSpeed = (MovementSpeed + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
                        else
                            MovementSpeed = (MovementSpeed - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
                    }
                    break;

                // --- Sub Stats
                case EEffectType.SubStat_Shield:
                    {
                        float shieldBase = MaxHealth;
                        if (addStat)
                        {
                            shieldBase += addAmount;
                            shieldBase *= (1.0f + addPercent) * (1.0f + addPercentMulti);
                            Shield = Mathf.Clamp(shieldBase - MaxHealth, 0.0f, MaxHealth);
                        }
                        else
                        {
                            shieldBase -= addAmount;
                            shieldBase /= 1.0f + addPercent;
                            shieldBase /= 1.0f + addPercentMulti;
                            Shield = Mathf.Clamp(shieldBase - MaxHealth, 0.0f, MaxHealth);
                        }
                    }
                    break;
                case EEffectType.SubStat_BonusHealth:
                    {
                        float bonusHealthBase = MaxHealth;
                        if (addStat)
                        {
                            bonusHealthBase += addAmount;
                            bonusHealthBase *= (1.0f + addPercent) * (1.0f + addPercentMulti);
                            BonusHealth = Mathf.Clamp(bonusHealthBase - MaxHealth, 0.0f, MaxHealth);
                        }
                        else
                        {
                            bonusHealthBase -= addAmount;
                            bonusHealthBase /= 1.0f + addPercent;
                            bonusHealthBase /= 1.0f + addPercentMulti;
                            BonusHealth = Mathf.Clamp(bonusHealthBase - MaxHealth, 0.0f, MaxHealth);
                        }
                    }
                    break;
                case EEffectType.SubStat_Armor:
                    {
                        if (addStat)
                        {
                            float armorBase = (Armor + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
                            Armor = Mathf.Clamp(armorBase, CFloat.Min(EFloat.Range_Armor), CFloat.Max(EFloat.Range_Armor));
                            Debug.Log($"<color=yellow>### ARMOR: {Armor} ##</color>");
                        }
                        else
                        {
                            float armorBase = (Armor - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
                            Armor = Mathf.Clamp(armorBase, CFloat.Min(EFloat.Range_Armor), CFloat.Max(EFloat.Range_Armor));
                        }
                    }
                    break;
                case EEffectType.SubStat_Critical:
                    {
                        if (addStat)
                        {
                            float criticalBase = (Critical + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
                            Critical = Mathf.Clamp(criticalBase, CFloat.Min(EFloat.Range_Critical), CFloat.Max(EFloat.Range_Critical));
                        }
                        else
                        {
                            float criticalBase = (Critical - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
                            Critical = Mathf.Clamp(criticalBase, CFloat.Min(EFloat.Range_Critical), CFloat.Max(EFloat.Range_Critical));
                        }
                    }
                    break;
                case EEffectType.SubStat_Dodge:
                    {
                        if (addStat)
                        {
                            float dodgeBase = (Dodge + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
                            Dodge = Mathf.Clamp(dodgeBase, CFloat.Min(EFloat.Range_Dodge), CFloat.Max(EFloat.Range_Dodge));
                        }
                        else
                        {
                            float dodgeBase = (Dodge - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
                            Dodge = Mathf.Clamp(dodgeBase, CFloat.Min(EFloat.Range_Dodge), CFloat.Max(EFloat.Range_Dodge));
                        }
                    }
                    break;
                case EEffectType.SubStat_Luck:
                    {
                        if (addStat)
                        {
                            float luckBase = (Luck + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
                            Luck = Mathf.Clamp(luckBase, CFloat.Min(EFloat.Range_Luck), CFloat.Max(EFloat.Range_Luck));
                        }
                        else
                        {
                            float luckBase = (Luck - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
                            Luck = Mathf.Clamp(luckBase, CFloat.Min(EFloat.Range_Luck), CFloat.Max(EFloat.Range_Luck));
                        }
                    }
                    break;
                case EEffectType.SubStat_InvincibleBlockCountPerWave:
                    {
                        if (addStat)
                            InvincibleBlockCountPerWave += Mathf.RoundToInt(addAmount);
                        else
                            InvincibleBlockCountPerWave -= Mathf.RoundToInt(addAmount);
                    }
                    break;
            }
        }
    }
}

/*
        // --- DEPRECIATED
        // public void ApplyBuffStat(EEffectType effectBuffType)
        // {
        //     float addAmount = _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //     float addPercent = _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //     float addPercentMulti = _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);

        //     switch (effectBuffType)
        //     {
        //         // --- Main Stats
        //         case EEffectType.MainStat_MaxHealth:
        //             MaxHealth = (MaxHealth + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             break;
        //         case EEffectType.MainStat_Damage:
        //             MinDamage = (MinDamage + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             MaxDamage = (MaxDamage + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             break;
        //         case EEffectType.MainStat_AttackRate:
        //             AttackRate = (AttackRate + addAmount) * (1.0f + addAmount) * (1.0f + addPercentMulti);
        //             break;
        //         case EEffectType.MainStat_MovementSpeed:
        //             MovementSpeed = (MovementSpeed + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             break;

        //         // --- Sub Stats
        //         case EEffectType.SubStat_Shield:
        //             float shieldBase = MaxHealth;
        //             shieldBase += addAmount;
        //             shieldBase *= (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             Shield = Mathf.Clamp(shieldBase - MaxHealth, 0.0f, MaxHealth);
        //             break;
        //         case EEffectType.SubStat_BonusHealth:
        //             float bonusHealthBase = MaxHealth;
        //             bonusHealthBase += addAmount;
        //             bonusHealthBase *= (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             BonusHealth = Mathf.Clamp(bonusHealthBase - MaxHealth, 0.0f, MaxHealth);
        //             break;
        //         case EEffectType.SubStat_Armor:
        //             float armorBase = (Armor + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             Armor = Mathf.Clamp(armorBase, CFloat.Min(EFloat.Range_Armor), CFloat.Max(EFloat.Range_Armor));
        //             Debug.Log($"<color=yellow>ARMOR: {Armor}</color>");
        //             break;
        //         case EEffectType.SubStat_Critical:
        //             float criticalBase = (Critical + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             Critical = Mathf.Clamp(criticalBase, CFloat.Min(EFloat.Range_Critical), CFloat.Max(EFloat.Range_Critical));
        //             break;
        //         case EEffectType.SubStat_Dodge:
        //             float dodgeBase = (Dodge + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             Dodge = Mathf.Clamp(dodgeBase, CFloat.Min(EFloat.Range_Dodge), CFloat.Max(EFloat.Range_Dodge));
        //             break;
        //         case EEffectType.SubStat_Luck:
        //             float luckBase = (Luck + addAmount) * (1.0f + addPercent) * (1.0f + addPercentMulti);
        //             Luck = Mathf.Clamp(luckBase, CFloat.Min(EFloat.Range_Luck), CFloat.Max(EFloat.Range_Luck));
        //             break;
        //         case EEffectType.SubStat_InvincibleBlockCountPerWave:
        //             InvincibleBlockCountPerWave += Mathf.RoundToInt(addAmount); 
        //             break;
        //     }
        // }

        // public void RemoveBuffStat(EEffectType effectBuffType)
        // {
        //     float addAmount = _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //     float addPercent = _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //     float addPercentMulti = _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);

        //     switch (effectBuffType)
        //     {
        //         // --- Main Stats
        //         case EEffectType.MainStat_MaxHealth:
        //             MaxHealth = (MaxHealth - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
        //             break;
        //         case EEffectType.MainStat_Damage:
        //             MinDamage = (MinDamage - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
        //             MaxDamage = (MaxDamage - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
        //             break;
        //         case EEffectType.MainStat_AttackRate:
        //             AttackRate = (AttackRate - addAmount) / (1.0f + addAmount) / (1.0f + addPercentMulti);
        //             break;
        //         case EEffectType.MainStat_MovementSpeed:
        //             MovementSpeed = (MovementSpeed - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
        //             break;

        //         // --- Sub Stats
        //         case EEffectType.SubStat_Shield:
        //             float shieldBase = MaxHealth;
        //             shieldBase -= addAmount;
        //             shieldBase /= 1.0f + addPercent;
        //             shieldBase /= 1.0f + addPercentMulti;
        //             Shield = Mathf.Clamp(shieldBase - MaxHealth, 0.0f, MaxHealth);
        //             break;
        //         case EEffectType.SubStat_BonusHealth:
        //             float bonusHealthBase = MaxHealth;
        //             bonusHealthBase -= addAmount;
        //             bonusHealthBase /= 1.0f + addPercent;
        //             bonusHealthBase /= 1.0f + addPercentMulti;
        //             BonusHealth = Mathf.Clamp(bonusHealthBase - MaxHealth, 0.0f, MaxHealth);
        //             break;
        //         case EEffectType.SubStat_Armor:
        //             float armorBase = (Armor - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
        //             Armor = Mathf.Clamp(armorBase, CFloat.Min(EFloat.Range_Armor), CFloat.Max(EFloat.Range_Armor));
        //             break;
        //         case EEffectType.SubStat_Critical:
        //             float criticalBase = (Critical - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
        //             Critical = Mathf.Clamp(criticalBase, CFloat.Min(EFloat.Range_Critical), CFloat.Max(EFloat.Range_Critical));
        //             break;
        //         case EEffectType.SubStat_Dodge:
        //             float dodgeBase = (Dodge - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
        //             Dodge = Mathf.Clamp(dodgeBase, CFloat.Min(EFloat.Range_Dodge), CFloat.Max(EFloat.Range_Dodge));
        //             break;
        //         case EEffectType.SubStat_Luck:
        //             float luckBase = (Luck - addAmount) / (1.0f + addPercent) / (1.0f + addPercentMulti);
        //             Luck = Mathf.Clamp(luckBase, CFloat.Min(EFloat.Range_Luck), CFloat.Max(EFloat.Range_Luck));
        //             break;
        //         case EEffectType.SubStat_InvincibleBlockCountPerWave:
        //             InvincibleBlockCountPerWave -= Mathf.RoundToInt(addAmount);
        //             break;
        //     }
        // }

        // public void ApplyBuffStat(EEffectType effectBuffType, int prev = -1)
        // {
        //     switch (effectBuffType)
        //     {
        //         case EEffectType.MainStat_MaxHealth:
        //             {
        //                 float baseValue = MaxHealth;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 MaxHealth = baseValue;
        //             }
        //             break;

        //         case EEffectType.MainStat_Damage:
        //             {
        //                 float baseValue = _baseStat.MinDamage;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 MinDamage = baseValue;

        //                 baseValue = _baseStat.MaxDamage;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 MaxDamage = baseValue;
        //             }
        //             break;

        //         case EEffectType.MainStat_AttackRate:
        //             {
        //                 float baseValue = _baseStat.AttackRate;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 AttackRate = baseValue;
        //             }
        //             break;

        //         case EEffectType.MainStat_MovementSpeed:
        //             {
        //                 float baseValue = _baseStat.MovementSpeed;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 MovementSpeed = baseValue;
        //             }
        //             break;

        //         case EEffectType.SubStat_Shield:
        //             {
        //                 float baseValue = _baseStat.MaxHealth;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 Shield = Mathf.Clamp(baseValue - MaxHealth, 0.0f, MaxHealth);
        //             }
        //             break;

        //         case EEffectType.SubStat_BonusHealth:
        //             {
        //                 float baseValue = _baseStat.MaxHealth;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 BonusHealth = Mathf.Clamp(baseValue - MaxHealth, 0.0f, MaxHealth);
        //             }
        //             break;

        //         case EEffectType.SubStat_Armor:
        //             {
        //                 float baseValue = Armor;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 Armor = Mathf.Clamp(baseValue, CFloat.Min(EFloat.Range_Armor), CFloat.Max(EFloat.Range_Armor));
        //             }
        //             break;


        //         case EEffectType.SubStat_Critical:
        //             {
        //                 float baseValue = Critical;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 Critical = Mathf.Clamp(baseValue, CFloat.Min(EFloat.Range_Critical), CFloat.Max(EFloat.Range_Critical));
        //             }
        //             break;

        //         case EEffectType.SubStat_Dodge:
        //             {
        //                 float baseValue = Dodge;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 Dodge = Mathf.Clamp(baseValue, CFloat.Min(EFloat.Range_Dodge), CFloat.Max(EFloat.Range_Dodge));
        //             }
        //             break;

        //         case EEffectType.SubStat_Luck:
        //             {
        //                 float baseValue = Luck;
        //                 baseValue += _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercent);
        //                 baseValue *= 1 + _owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddPercentMulti);
        //                 Luck = Mathf.Clamp(baseValue, CFloat.Min(EFloat.Range_Luck), CFloat.Max(EFloat.Range_Luck));

        //             }
        //             break;

        //         case EEffectType.SubStat_InvincibleBlockCountPerWave:
        //             {
        //                 int baseValue = InvincibleBlockCountPerWave;
        //                 baseValue += (int)_owner.BaseEffect.GetStatModifier(effectBuffType, EStatModType.AddAmount);
        //                 InvincibleBlockCountPerWave = baseValue;
        //             }
        //             break;
        //     }
        // }
*/