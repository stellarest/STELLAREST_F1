using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Monster : Creature
    {
        public Data.MonsterData MonsterData { get; private set; } = null;
        private MonsterBody _monsterBody = null;
        public MonsterBody MonsterBody
        {
            get => _monsterBody;
            private set
            {
                _monsterBody = value;
                if (CreatureBody == null)
                    CreatureBody = _monsterBody;
            }
        }
        public MonsterAnimation MonsterAnim { get; private set; } = null;
        public EMonsterType MonsterType { get; private set; } = EMonsterType.None;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Monster;
            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame();
                return false;
            }

            MonsterBody = new MonsterBody(this, dataID);
            MonsterAnim = CreatureAnim as MonsterAnimation;
            MonsterAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, target: this);

            MonsterData = Managers.Data.MonsterDataDict[dataID];
            MonsterType = Util.GetEnumFromString<EMonsterType>(MonsterData.Type);

            gameObject.name += $"_{MonsterData.DescriptionTextID.Replace(" ", "")}";
            Collider.radius = MonsterData.ColliderRadius;

            CreatureSkillComponent = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkillComponent.SetInfo(this, Managers.Data.MonsterDataDict[dataID].SkillIDs);

            EnterInGame();
            return true;
        }

        protected override void EnterInGame()
        {
            base.EnterInGame();
            _initPos = transform.position;
            LookAtDir = ELookAtDirection.Left;
        }

        private Vector3 _destPos = Vector3.zero;
        private Vector3 _initPos = Vector3.zero;

        #region AI
        protected override void UpdateIdle()
        {
            if (Target.IsValid())
                LookAtTarget(Target);

            if (CreatureSkillComponent.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
                return;

            {
                if (Target.IsValid() == false)
                {
                    int patrolPercent = 20;
                    if (UnityEngine.Random.Range(0, 100) <= patrolPercent)
                    {
                        _destPos = _initPos + new Vector3(Random.Range(-4, 4), Random.Range(-4, 4));
                        CreatureState = ECreatureState.Move;
                        return;
                    }
                    else
                    {
                        SetRigidBodyVelocity(Vector2.zero);
                        //StartWait(Random.Range(1f, 2f));
                    }
                }
            }

            {
                // Research Enemy
                Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_SearchDistance, Managers.Object.Heroes) as Creature;
                if (creature.IsValid())
                {
                    Target = creature;
                    CreatureState = ECreatureState.Move;
                    CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                    return;
                }
            }
        }

        protected override void UpdateMove()
        {
            if (Target.IsValid() == false)
            {
                Vector3 toDestDir = _destPos - transform.position;
                float endThreshold = 0.01f;
                if (toDestDir.sqrMagnitude < endThreshold)
                {
                    CreatureState = ECreatureState.Idle;
                    return;
                }

                // Research Enemy When Patroling
                SetRigidBodyVelocity(toDestDir.normalized * MovementSpeed);
                Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_SearchDistance, Managers.Object.Heroes, func: IsValid) as Creature;
                if (creature.IsValid())
                {
                    Target = creature;
                    CreatureState = ECreatureState.Move;
                    CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                    return;
                }
            }
            else
            {
                ChaseOrAttackTarget(ReadOnly.Numeric.Temp_SearchDistance, AttackDistance);
                if (Target.IsValid() == false)
                {
                    Target = null;
                    _destPos = _initPos;
                    return;
                }
            }
        }

        // Attack이나 Skill은 실행 이후 Creature Move로 돌아가면 된다.
        protected override void UpdateSkill()
        {
            SetRigidBodyVelocity(Vector2.zero);
            if (Target.IsValid())
                LookAtTarget(Target);
            else
            {
                Target = null;
                _destPos = _initPos;
                CreatureState = ECreatureState.Move;
            }
        }
        #endregion

        #region Battle
        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDamaged(attacker, skillFromAttacker);
            Debug.Log($"{gameObject.name} is damaged. ({Hp} / {MaxHp})");
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDead(attacker, skillFromAttacker);
        }
        protected override void OnDisable()
        {
            Debug.Log("Monster::OnDisable");
            base.OnDisable();
        }
        #endregion
    }
}
