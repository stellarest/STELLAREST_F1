using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MonsterAnimation : CreatureAnimation
    {
        private Monster _owner = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override void SetInfo(int dataID, BaseObject owner)
        {
            base.SetInfo(dataID, owner);
            string animatorTextID = Managers.Data.MonsterDataDict[dataID].AnimatorLabel;
            RuntimeAnimatorController animController = Managers.Resource.Load<RuntimeAnimatorController>(animatorTextID);
            if (string.IsNullOrEmpty(animatorTextID) == false && animController != null)
            {
                RuntimeAnimatorController cloned = UnityEngine.Object.Instantiate(animController);
                this.Animator.runtimeAnimatorController = cloned;
            }

            _owner = owner as Monster;
            // 몬스터 sprite는 플레이어와 반대 방향으로 구성되어 있음
            //_originScaleX = _owner.transform.localScale.x * -1;
        }

        public override void UpdateAnimation()
        {
            if (_owner == null)
                return;

            switch (_owner.CreatureState)
            {
                case ECreatureState.Idle:
                    {
                        _owner.MonsterBody.SetEmoji(EMonsterEmoji.Default);
                        Idle();
                    }
                    break;

                case ECreatureState.Move:
                    {
                        _owner.MonsterBody.SetEmoji(EMonsterEmoji.Default);
                        Move();
                    }
                    break;

                case ECreatureState.Attack:
                    {
                        _owner.MonsterBody.SetEmoji(EMonsterEmoji.Angry);
                        Attack();
                    }
                    break;

                case ECreatureState.Skill:
                    break;

                case ECreatureState.Dead:
                    {
                        _owner.MonsterBody.SetEmoji(EMonsterEmoji.Dead);
                        Dead();
                    }
                    break;
            }
        }

        public override void Flip(ELookAtDirection lookAtDir)
        {
            // Monster LookAtDir Default Sprite : Left
            Vector3 localScale = _owner.transform.localScale;
            int sign = (lookAtDir == ELookAtDirection.Left) ? (localScale.x > 0 ? 1 : -1) : -1;
            localScale.x = localScale.x * sign;
            _owner.transform.localScale = localScale;
        }
    }
}
