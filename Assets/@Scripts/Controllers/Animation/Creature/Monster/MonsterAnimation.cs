using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class MonsterAnimation : CreatureAnimation
    {
        private Monster _monster = null;
        public override BaseObject Owner 
        { 
            get => _monster;
            protected set
            {
                if (_monster == null)
                    _monster = value as Monster;
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
            Owner = owner as Monster;
        }
    }
}
