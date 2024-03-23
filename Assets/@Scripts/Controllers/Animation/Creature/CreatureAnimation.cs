using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CreatureAnimation : BaseAnimation
    {
        private readonly int Play_Idle = Animator.StringToHash(ReadOnly.String.AnimParam_Idle);
        private readonly int Play_Move = Animator.StringToHash(ReadOnly.String.AnimParam_Move);

        private Creature _creature = null;
        public override BaseObject Owner
        {
            get => _creature;
            protected set
            {
                if (_creature == null)
                    _creature = value as Creature;
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfoFromOwner(int dataID, BaseObject owner)
        {
            base.SetInfoFromOwner(dataID, owner);
            Owner = owner as Creature;
        }

        protected override void Idle()
        {
            Animator.Play(Play_Idle);
        }

        protected override void Move()
        {
            Animator.Play(Play_Move);
        }
    }
}
