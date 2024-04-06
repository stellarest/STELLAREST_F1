using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using Unity.Profiling;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BaseAnimation : InitBase
    {
        public BaseObject Owner { get; set; } = null;
        public T GetOwner<T>() where T : BaseObject => Owner as T;
        public Animator Animator { get; private set; } = null;
        public AnimationCallback AnimCallback { get; private set; } = null;

        protected readonly int Play_Idle = Animator.StringToHash(ReadOnly.String.AnimParam_Idle);
        protected readonly int Play_Move = Animator.StringToHash(ReadOnly.String.AnimParam_Move);
        protected readonly int Play_Attack = Animator.StringToHash(ReadOnly.String.AnimParam_Attack);
        protected readonly int Play_Skill_A = Animator.StringToHash(ReadOnly.String.AnimParam_Skill_A);
        protected readonly int Play_Skill_B = Animator.StringToHash(ReadOnly.String.AnimParam_Skill_B);
        protected readonly int Play_Dead = Animator.StringToHash(ReadOnly.String.AnimParam_Dead);

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

            Animator = GetComponent<Animator>();
            AnimCallback = GetComponent<AnimationCallback>();
            //_originScaleX = transform.localScale.x;

            return true;
        }

        public virtual void SetInfo(int dataID, BaseObject owner)
            => Owner = owner;

        public bool IsPlay()
        {
            AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime > 0f)
                return true;
            else
                return false;
        }

        public virtual void UpdateAnimation() {  }

        protected virtual void Idle()
            => Animator.Play(Play_Idle);

        protected virtual void Move()
            => Animator.Play(Play_Move);

        protected virtual void Attack()
            => Animator.Play(Play_Attack);

        protected virtual void SkillA()
            => Animator.Play(Play_Skill_A);

        protected virtual void SkillB()
            => Animator.Play(Play_Skill_B);

        protected virtual void Dead()
            => Animator.Play(Play_Dead);

        public virtual void Flip(ELookAtDirection lookAtDir) { }
    }
}
