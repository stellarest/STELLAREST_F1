using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class BodyAttack : SkillBase
    {
        private CircleCollider2D _bodyCol = null;
        private float _bodyColRadius = 0f;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _bodyCol = GetComponent<CircleCollider2D>();

            return true;
        }

        public override bool SetInfo(BaseObject owner, int dataID)
        {
            if (base.SetInfo(owner, dataID) == false)
            {
                EnterInGame(owner, dataID);
                return false;
            }

            EnterInGame(owner, dataID);
            return true;
        }

        protected override void EnterInGame(BaseObject owner, int dataID)
        {
            base.EnterInGame(owner, dataID);
        }

        public override void OnSkillStateEnd()
        {
            if (Owner.Target.IsValid())
                Owner.LookAtValidTarget();
        }

        public override void OnSkillStateEnter()
        {
        }

        public override void OnSkillStateUpdate()
        {
        }
    }
}
