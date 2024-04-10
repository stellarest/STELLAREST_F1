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
            monsterStateMachine.OnMonsterAnimationComplatedHandler -= OnAnimationComplated;
            monsterStateMachine.OnMonsterAnimationComplatedHandler += OnAnimationComplated;
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
            if (_coWait != null)
                return;

            // Patrol
            {
                if (Target == null)
                {
                    int patrolPercent = 20;
                    if (UnityEngine.Random.Range(0, 100) <= patrolPercent)
                    {
                        _destPos = _initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                        CreatureState = ECreatureState.Move;
                        return;
                    }
                }
            }

            // Search Player
            {
                Creature target = null;
                float bestDistSqr = float.MaxValue;

                foreach (Hero hero in Managers.Object.Heroes)
                {
                    Vector3 toTargetDir = hero.transform.position - transform.position;
                    float toTargetDistSQR = toTargetDir.sqrMagnitude;

                    if (toTargetDistSQR > SearchDistanceSQR)
                        continue;

                    if (toTargetDistSQR > SearchDistanceSQR)
                        continue;

                    target = hero;
                    bestDistSqr = toTargetDistSQR;
                }

                Target = target;
                Debug.Log(Target != null);
                if (Target != null)
                    CreatureState = ECreatureState.Move;
            }
        }

        protected override void UpdateMove()
        {
            if (Target == null)
            {
                Vector3 toDestDir = _destPos - transform.position;

                float endThreshold = 0.01f;
                if (toDestDir.sqrMagnitude < endThreshold)
                {
                    CreatureState = ECreatureState.Idle;
                    return;
                }

                SetRigidBodyVelocity(toDestDir.normalized * MovementSpeed);
            }
            else
            {
                // Chase
                Vector3 toTargetDir = Target.transform.position - transform.position;
                float toTargetDistSQR = toTargetDir.sqrMagnitude;
                float attackDistSQR = AttackDistance * AttackDistance;
                if (attackDistSQR > toTargetDistSQR)
                {
                    CreatureState = ECreatureState.Attack;
                    // [ TODO ]
                    // ***** Animation이 끝났을 때, Idle로 돌아가고, 스킬의 쿨타임 만큼 Wait로 바꿔야함. *****
                    // 영웅전설5 같은 느낌으로
                    // Monster의 Rigidbody.Mass도 본스터별로 설정을 해야할듯. 플레이어한테 밀림.
                    // StartWait(2f);
                }
                else
                {
                    SetRigidBodyVelocity(toTargetDir.normalized * MovementSpeed);
                    // 너무 멀어지면 포기
                    if (toTargetDistSQR > SearchDistanceSQR)
                    {
                        _destPos = _initPos;
                        Target = null;
                        CreatureState = ECreatureState.Move; // Idle로 가면 안되나. 아 그냥 바로 획돌아가는 것이구만
                        // 사실 이미 무드상태라 넣어도 그만 안넣어도 그만. 알아서 하셈.
                        // 그냥 그대로 이동한다는 것을 명시하는 것임.
                    }
                }
            }
        }

        // Attack이나 Skill은 실행 이후 Creature Move로 돌아가면 된다.
        protected override void UpdateAttack()
        {
            if (Target != null)
            {
                Vector3 toTargetDir = Target.transform.position - transform.position;
                if (toTargetDir.x < 0)
                    LookAtDir = ELookAtDirection.Left;
                else
                    LookAtDir = ELookAtDirection.Right;
            }
        }

        protected override void UpdateDead()
        {
        }
        #endregion


        public float TestAttackCoolTime = 1.25f;
        #region Monster Animation Events
        protected override void OnAttackAnimationCompleted()
        {
            CreatureState = ECreatureState.Idle;
            StartWait(TestAttackCoolTime);
        }
        #endregion
    }
}
