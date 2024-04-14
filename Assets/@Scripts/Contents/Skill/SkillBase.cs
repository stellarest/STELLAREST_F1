using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

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

            EnterInGame();
            return true;
        }

        protected virtual void EnterInGame()
        {
            AddAnimationEvents();
        }

        private void AddAnimationEvents()
        {
            if (Owner != null && Owner.CreatureAnim != null)
            {
                CreatureStateMachine[] creatureStateMachines = Owner.CreatureAnim.Animator.GetBehaviours<CreatureStateMachine>();
                for (int i = 0; i < creatureStateMachines.Length; ++i)
                {
                    creatureStateMachines[i].OnAnimUpdateHandler -= OnAnimationUpdate;
                    creatureStateMachines[i].OnAnimUpdateHandler += OnAnimationUpdate;

                    creatureStateMachines[i].OnAnimCompletedHandler -= OnAnimationCompleted;
                    creatureStateMachines[i].OnAnimCompletedHandler += OnAnimationCompleted;
                }
            }
            else
                Debug.LogError($"{nameof(SkillBase)}, {nameof(AddAnimationEvents)}");
        }

        protected void OnAnimationUpdate(ECreatureState creatureState)
        {
        }

        protected void OnAnimationCompleted(ECreatureState creatureState)
        {
        }
    }
}
