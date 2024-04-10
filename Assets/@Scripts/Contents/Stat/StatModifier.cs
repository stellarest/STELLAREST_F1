using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class StatModifier
    {
        public readonly float Value = 0f;
        public readonly EStatModType Type = EStatModType.None;
        public readonly int Order = -1;
        public readonly object Source = null;

        public StatModifier(float value, EStatModType type, int order, object source)
        {
            this.Value = value;
            this.Type = type;
            this.Type = type;
            this.Source = source;
        }

        public StatModifier(float value, EStatModType type)
            : this(value, type, (int)type, null)
        {
        }

        public StatModifier(float value, EStatModType type, int order)
            : this(value, type, order, null)
        {
        }

        public StatModifier(float value, EStatModType type, object source)
            : this(value, type, (int)type, source)
        {
        }
    }
}