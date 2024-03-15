using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Creature : BaseObject
    {
        public CreatureAnimation CreatureAnim { get; private set; } = null;

        public float Speed { get; protected set; } = 1.0f;
        public ECreatureType CreatureType { get; protected set; } = ECreatureType.None;
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
            
            ObjectType = EObjectType.Creature;
            CreatureState = ECreatureState.Idle;
            
            if (BaseAnim != null)
                CreatureAnim = BaseAnim as CreatureAnimation;
            else
                Debug.LogWarning($"{nameof(Creature)}, {nameof(Init)}, Input : \"{CreatureAnim}\"");

            return true;
        }

        protected override void UpdateAnimation()
        {
            switch (CreatureState)
            {
                case ECreatureState.Idle:
                    PlayAnimation(ReadOnly.String.Anim_Idle);
                    break;

                case ECreatureState.Move:
                    PlayAnimation(ReadOnly.String.Anim_Move);
                    break;

                case ECreatureState.Attack:
                    PlayAnimation(ReadOnly.String.Anim_Attack);
                    break;

                case ECreatureState.Skill:
                    PlayAnimation(ReadOnly.String.Anim_Skill_A);
                    break;

                case ECreatureState.Dead:
                    PlayAnimation(ReadOnly.String.Anim_Dead);
                    break;

                default:
                    break;
            }
        }
    }
}
