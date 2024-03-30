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
        private readonly int Play_Attack = Animator.StringToHash(ReadOnly.String.AnimParam_Attack);
        private readonly int Play_Skill_A = Animator.StringToHash(ReadOnly.String.AnimParam_Skill_A);
        private readonly int Play_Skill_B = Animator.StringToHash(ReadOnly.String.AnimParam_Skill_B);
        private readonly int Play_Dead = Animator.StringToHash(ReadOnly.String.AnimParam_Dead);

        public int GetHash(ECreatureState state)
        {
            switch (state)
            {
                case ECreatureState.Idle:
                    return Play_Idle;

                case ECreatureState.Move:
                    return Play_Move;

                case ECreatureState.Attack:
                    return Play_Attack; // TEMP

                default:
                    return -1;
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        protected override void Idle()
            => Animator.Play(Play_Idle);

        protected override void Move()
            => Animator.Play(Play_Move);

        protected override void Attack()
            => Animator.Play(Play_Attack);

        protected override void SkillA()
            => Animator.Play(Play_Skill_A);

        protected override void SkillB()
            => Animator.Play(Play_Skill_B);

        protected override void Dead()
            => Animator.Play(Play_Dead);
    }
}
