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

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Monster;
            return true;
        }

        #region Monster SetInfo
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
            MonsterStateMachine monsterStateMachine = MonsterAnim.Animator.GetBehaviour<MonsterStateMachine>();
            monsterStateMachine.OnMonsterAnimUpdateHandler -= OnAnimationUpdate;
            monsterStateMachine.OnMonsterAnimUpdateHandler += OnAnimationUpdate;
            monsterStateMachine.OnMonsterAnimComplatedHandler -= OnAnimationCompleted;
            monsterStateMachine.OnMonsterAnimComplatedHandler += OnAnimationCompleted;
            Managers.Sprite.SetInfo(dataID, target: this);

            // SetStat
            MonsterData = Managers.Data.MonsterDataDict[dataID];
            _maxHp = new Stat(MonsterData.MaxHp);
            _atk = new Stat(MonsterData.Atk);
            _atkRange = new Stat(MonsterData.AtkRange);
            _movementSpeed = new Stat(MonsterData.MovementSpeed);

            gameObject.name += $"_{MonsterData.DescriptionTextID.Replace(" ", "")}";
            Collider.radius = MonsterData.ColliderRadius;
            EnterInGame();
            
            return true;
        }
        #endregion

        protected override void EnterInGame()
        {
            base.EnterInGame();
            _initPos = transform.position;
            LookAtDir = ELookAtDirection.Left;
        }

        private Vector3 _destPos = Vector3.zero;
        private Vector3 _initPos = Vector3.zero;

        #region Monster - AI
        protected override void UpdateIdle()
        {
            if (Target.IsValid())
                LookAtTarget();

            if (_coWait != null)
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
                }
            }

            {
                Creature creature = FindClosestInRange(ReadOnly.Numeric.DefaultSearchRange, Managers.Object.Heroes) as Creature;
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
                    CreatureMoveState = ECreatureMoveState.None;
                    return;
                }

                SetRigidBodyVelocity(toDestDir.normalized * MovementSpeed);
                Creature creature = FindClosestInRange(ReadOnly.Numeric.DefaultSearchRange, Managers.Object.Heroes, func: IsValid) as Creature;
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
                ChaseOrAttackTarget(AttackDistance, ReadOnly.Numeric.DefaultSearchRange);
                if (Target.IsValid() == false)
                {
                    Target = null;
                    _destPos = _initPos;
                    return;
                }
            }
        }

        // Attack이나 Skill은 실행 이후 Creature Move로 돌아가면 된다.
        protected override void UpdateSkillAttack()
        {
            if (Target.IsValid())
                LookAtTarget();
        }

        protected override void UpdateDead()
        {
        }
        #endregion

        #region Monster Animation Events - Update
        protected override void OnSkillAttackAnimationUpdate()
        {
            if (Target.IsValid() == false)
                return;

            Target.OnDamaged(this);
        }
        #endregion

        #region Monster Animation Events - Completed
        private float TestCoolTime = 2.25f;
        protected override void OnSkillAttackAnimationCompleted()
        {
            if (Target.IsValid())
                StartWait(TestCoolTime);

            CreatureState = ECreatureState.Idle;
        }
        #endregion

        #region Monster - Battle
        public override void OnDamaged(BaseObject attacker)
        {
            base.OnDamaged(attacker);
            Debug.Log($"{gameObject.name} Hp : {Hp} / {MaxHp}");
        }

        public override void OnDead(BaseObject attacker)
        {
            base.OnDead(attacker);
            // TODO : DROP ITEM
            // Managers.Object.Despawn(this);
        }
        #endregion
    }
}
