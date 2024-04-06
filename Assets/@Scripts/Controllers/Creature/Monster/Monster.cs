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
        public override ECreatureState CreatureState
        {
            get => base.CreatureState;
            set
            {
                base.CreatureState = value;
                switch (CreatureState)
                {
                    case ECreatureState.Idle:
                        UpdateAITick = 0.5f;
                        break;

                    case ECreatureState.Move:
                        UpdateAITick = 0f;
                        break;

                    case ECreatureState.Attack:
                        UpdateAITick = 0f;
                        break;

                    case ECreatureState.Dead:
                        UpdateAITick = 1f;
                        break;
                }
            }
        }

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
                Refresh();
                return false;
            }

            MonsterBody = new MonsterBody(this, dataID);
            MonsterAnim = CreatureAnim as MonsterAnimation;
            MonsterAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, target: this);

            MonsterData = Managers.Data.MonsterDataDict[dataID];
            gameObject.name += $"_{MonsterData.DescriptionTextID}";

            Collider.radius = MonsterData.ColliderRadius;
            Speed = MonsterData.MovementSpeed;
            Refresh();
            
            return true;
        }

        protected override void Refresh()
        {
            base.Refresh();
            StartCoroutine(CoUpdateAI());
            _initPos = transform.position;
            Speed = MonsterData.MovementSpeed;
            LookAtDir = ELookAtDirection.Left;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                CreatureState = ECreatureState.Idle;

            if (Input.GetKeyDown(KeyCode.W))
                CreatureState = ECreatureState.Dead;
        }

        // TEMP
        public float SearchDistance { get; private set; } = 8.0f;
        public float AttackDistance { get; private set; } = 2.0f;
        private Creature _target = null;
        private Vector3 _destPos = Vector3.zero;
        private Vector3 _initPos = Vector3.zero;
        protected override void UpdateIdle()
        {
            if (_coWait != null)
                return;

            Debug.Log("Update Idle");
            // Patrol(0.5초마다 Idle 실행중인데, 10% 확률로 패트롤 이동을 한다고 하면)
            {
                int patrolPercent = 20;
                if (UnityEngine.Random.Range(0, 100) <= patrolPercent)
                {
                    _destPos = _initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                    CreatureState = ECreatureState.Move;
                    return;
                }
            }

            // Search Player
            {
                Creature target = null;
                float bestDistSqr = float.MaxValue;
                float searchDistSqr = SearchDistance * SearchDistance;

                foreach (Hero hero in Managers.Object.Heroes)
                {
                    Vector3 toTargetDir = hero.transform.position - transform.position;
                    float toTargetDistSqr = toTargetDir.sqrMagnitude;

                    if (toTargetDistSqr > searchDistSqr)
                        continue;

                    if (toTargetDistSqr > bestDistSqr)
                        continue;

                    target = hero;
                    bestDistSqr = toTargetDistSqr;
                }

                _target = target;
                if (_target != null)
                    CreatureState = ECreatureState.Move;
            }
        }

        protected override void UpdateMove()
        {
            Debug.Log("Update Move");
            if (_target == null)
            {
                // TODO : Patrol or Return
                Vector3 toDestDir = _destPos - transform.position;
                float moveDistPerFrame = Time.deltaTime * Speed;
                float moveDist = UnityEngine.Mathf.Min(toDestDir.magnitude, moveDistPerFrame);
                transform.TranslateEx(toDestDir.normalized * moveDist);

                float endThreshold = 0.01f;
                if (toDestDir.sqrMagnitude < endThreshold)
                    CreatureState = ECreatureState.Idle;
            }
            else
            {
                // Chase
                Vector3 toTargetDir = (_target.transform.position - transform.position);
                float toTargetDistSqr = toTargetDir.sqrMagnitude;
                float attackDistSqr = AttackDistance * AttackDistance;
                if (attackDistSqr > toTargetDistSqr)
                {
                    CreatureState = ECreatureState.Attack;
                    StartWait(2f);
                }
                else
                {
                    float moveDistPerFrame = Time.deltaTime * Speed;
                    // movementPerFrame 그냥 얘로 하면 안되나
                    // toTargetDir.magnitude : 만약 실제 거리가 10인데,
                    // movementPerFrame : 내가 이번 틱에서 스피드가 높아서 20이 나올수도 있으니 이러면 초과되는거니까 이렇게 하는 거라고 함.
                    float moveDist = Mathf.Min(toTargetDir.magnitude, moveDistPerFrame);
                    transform.TranslateEx(toTargetDir.normalized * moveDist);

                    // 너무 멀어지면 포기
                    float searchDistSqr = SearchDistance * SearchDistance;
                    if (toTargetDistSqr > searchDistSqr)
                    {
                        _destPos = _initPos;
                        _target = null;
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
            Debug.Log("Update Attack");
            if (_coWait != null)
                return;

            CreatureState = ECreatureState.Move;
        }

        protected override void UpdateDead()
        {
            Debug.Log("Update Dead");
        }
    }
}
