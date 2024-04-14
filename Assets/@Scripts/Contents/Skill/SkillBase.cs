using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    // TEMP
    public class SkillBase : InitBase
    {
        public Creature Owner { get; protected set; } = null;
        public Data.SkillData SkillData { get; private set; } = null;
        public int DataTemplateID { get; private set; } = -1;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        private bool _initialSet = false;
        
        public virtual bool SetInfo(Creature owner, int dataID)
        {
            if (_initialSet)
            {
                EnterInGame();
                return false;
            }

            Owner = owner;
            SkillData = Managers.Data.SkillDataDict[dataID];
            DataTemplateID = dataID;

            // Rigister Animation Event
            {

            }

            EnterInGame();
            return true;
        }

        protected virtual void EnterInGame()
        {

        }
    }
}
