using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Stat
    {
        public Stat()
        {
        }

        public Stat(float baseValue)
            : this()
        {
            this.BaseValue = baseValue;
        }

        public float BaseValue { get; private set; } = 0f;
        private bool _isDirty = false;

        [SerializeField] private float _value = 0f;
        public virtual float Value
        {
            get
            {
                if (_isDirty)
                {
                    _value = CalculateFinalValue();
                    _isDirty = false;
                }

                return _value;
            }

            private set => _value = value;
        }

        public List<StatModifier> StatModifiers { get; private set; } = new List<StatModifier>();

        public virtual void AddModifier(StatModifier modifier)
        {
            _isDirty = true;
            StatModifiers.Add(modifier);
        }

        public virtual bool RemoveModifier(StatModifier modifier)
        {
            if (StatModifiers.Remove(modifier))
            {
                _isDirty = true;
                return true;
            }

            return false;
        }

        public virtual bool ClearModifiersFromSource(object source)
        {
            int numOfRemovals = StatModifiers.RemoveAll(s => s.Source == source);
            if (numOfRemovals > 0)
            {
                _isDirty = true;
                return true;
            }

            return false;
        }

        private float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0f;

            StatModifiers.Sort(CompareOrder);

            for (int i = 0; i < StatModifiers.Count; ++i)
            {
                StatModifier modifier = StatModifiers[i];
                switch (modifier.Type)
                {
                    case EStatModType.Add:
                        finalValue += modifier.Value;
                        break;

                    case EStatModType.PercentAdd:
                        sumPercentAdd += modifier.Value;
                        if (i == StatModifiers.Count - 1 || StatModifiers[i + 1].Type != EStatModType.PercentAdd)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0f;
                        }
                        break;

                    case EStatModType.PercentMulti:
                        finalValue *= 1 + modifier.Value;
                        break;
                }
            }

            return (float)System.Math.Round(finalValue, 4);
        }

        private int CompareOrder(StatModifier a, StatModifier b)
        {
            if (a.Order == b.Order)
                return 0;

            return (a.Order > b.Order) ? 1 : -1;
        }
    }
}
