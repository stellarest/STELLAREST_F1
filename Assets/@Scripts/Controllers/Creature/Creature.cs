using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Creature : BaseObject
    {
        public float Speed { get; protected set; } = 1.0f; // TEMP

        public CreatureAnimation CreatureAnim { get; private set; } = null;
        public ECreatureRarity CreatureRarity { get; protected set; } = ECreatureRarity.Common;
        [SerializeField]
        protected ECreatureState _creatureState = ECreatureState.None;
        public ECreatureState CreatureState
        {
            get => _creatureState;
            set
            {
                if (_creatureState != value)
                {
                    _creatureState = value;
                    UpdateAnimation();
                }
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfo(int dataID)
        {
            base.SetInfo(dataID);
            if (CreatureAnim == null)
                CreatureAnim = BaseAnim as CreatureAnimation;
        }

        protected virtual void SetCreatureFromData(int dataID) {  }

        protected virtual void RefreshCreature()
            => CreatureState = ECreatureState.Idle;
    }
}
