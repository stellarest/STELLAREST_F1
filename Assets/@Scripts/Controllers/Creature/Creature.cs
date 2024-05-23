using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Creature : BaseObject
    {
        public SkillComponent CreatureSkill { get; protected set; } = null;
        public CreatureBody CreatureBody { get; protected set; } = null;
        public CreatureAnimation CreatureAnim { get; private set; } = null;

        [SerializeField] private ECreatureState _creatureState = ECreatureState.None;
        public virtual ECreatureState CreatureState
        {
            get => _creatureState;
            set
            {
                if (_creatureState != value)
                {
                    _creatureState = value;
                    UpdateAnimation();
                }
            }
        }

        [SerializeField] private ECreatureMoveState _creatureMoveState = ECreatureMoveState.None;
        public virtual ECreatureMoveState CreatureMoveState
        {
            get => _creatureMoveState;
            protected set => _creatureMoveState = value;
        }

        private bool _collectEnv = false;
        public virtual bool CollectEnv
        {
            get => _collectEnv;
            protected set => _collectEnv = value;
        }

        [SerializeField] protected EFindPathResult _findPathResult = EFindPathResult.None;
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame(dataID);
                return false;
            }

            CreatureAnim = BaseAnim as CreatureAnimation;
            //EnterInGame();
            return true;
        }

        protected override void EnterInGame(int dataID)
        {
            base.EnterInGame(dataID);
            RigidBody.simulated = false;
            ShowBody(false);
            StartWait(waitCondition: () => BaseAnim.IsPlay() == false,
                      callbackWaitCompleted: () => {
                          ShowBody(true);
                          RigidBody.simulated = true;
                          Target = null;
                          CancelWait();
                          AddAnimationEvents();
                          CreatureState = ECreatureState.Idle;
                          CreatureMoveState = ECreatureMoveState.None;
                          StartCoroutine(CoUpdateAI()); // --> Leader도 AI 돌고 있어야함.
                          TickLerpToCellPos();
                          // StartCoroutine(CoLerpToCellPos()); // Map
                      });

            // StartWait(waitCondition: () => BaseAnim.IsPlay() == false,
            //         delegate()
            //         {
            //         });
        }

        private void AddAnimationEvents()
        {
            CreatureStateMachine[] creatureStateMachines = CreatureAnim.Animator.GetBehaviours<CreatureStateMachine>();
            for (int i = 0; i < creatureStateMachines.Length; ++i)
            {
                creatureStateMachines[i].OnStateEnterHandler -= OnStateEnter;
                creatureStateMachines[i].OnStateEnterHandler += OnStateEnter;

                creatureStateMachines[i].OnStateUpdateHandler -= OnStateUpdate;
                creatureStateMachines[i].OnStateUpdateHandler += OnStateUpdate;

                creatureStateMachines[i].OnStateEndHandler -= OnStateEnd;
                creatureStateMachines[i].OnStateEndHandler += OnStateEnd;
            }
        }

        private void ReleaseAnimationEvents()
        {
            if (CreatureAnim == null)
                return;

            CreatureStateMachine[] creatureStateMachines = CreatureAnim.Animator.GetBehaviours<CreatureStateMachine>();
            for (int i = 0; i < creatureStateMachines.Length; ++i)
            {
                creatureStateMachines[i].OnStateEnterHandler -= OnStateEnter;
                creatureStateMachines[i].OnStateUpdateHandler -= OnStateUpdate;
                creatureStateMachines[i].OnStateEndHandler -= OnStateEnd;
            }
        }

        protected IEnumerator CoUpdateAI()
        {
            while (true)
            {
                switch (CreatureState)
                {
                    case ECreatureState.Idle:
                        UpdateIdle();
                        break;

                    case ECreatureState.Move:
                        UpdateMove();
                        break;

                    case ECreatureState.Skill_Attack:
                    case ECreatureState.Skill_A:
                    case ECreatureState.Skill_B:
                        UpdateSkill();
                        break;

                    case ECreatureState.CollectEnv:
                        UpdateCollectEnv();
                        break;

                    case ECreatureState.Dead:
                        UpdateDead();
                        break;
                }
                
                yield return null;
            }
        }

        protected virtual void UpdateIdle() { }
        protected virtual void UpdateMove() { }
        protected virtual void UpdateSkill() { }
        protected virtual void UpdateCollectEnv() { }
        protected virtual void UpdateDead()
        {
            // SetRigidBodyVelocity(Vector2.zero); - DELETED
            CancelWait();
        }

        #region Coroutines
        // Co Wait 없앨준비하라네
        protected Coroutine _coWait = null;
        protected void StartWait(float seconds)
        {
            CancelWait();
            _coWait = StartCoroutine(CoWait(seconds));
        }
        private IEnumerator CoWait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _coWait = null;
        }

        protected void StartWait(Func<bool> waitCondition, Action callbackWaitCompleted = null)
        {
            CancelWait();
            _coWait = StartCoroutine(CoWait(waitCondition, callbackWaitCompleted));
        }
        private IEnumerator CoWait(Func<bool> waitCondition, Action waitCompleted = null)
        {
            yield return new WaitUntil(waitCondition);
            _coWait = null;
            waitCompleted?.Invoke();
        }

        protected void CancelWait()
        {
            if (_coWait != null)
                StopCoroutine(_coWait);
            _coWait = null;
        }
        #endregion Coroutines

        #region State Machine Events 
        protected void OnStateEnter(ECreatureState enterState)
        {
            switch (enterState)
            {
                case ECreatureState.Skill_Attack:
                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    CreatureSkill?.PassOnSkillStateEnter(enterState);
                    break;

                case ECreatureState.CollectEnv:
                    OnCollectEnvStateEnter();
                    break;
            }
        }
        protected virtual void OnCollectEnvStateEnter()
        {
             CollectEnv = true;
        }

        protected void OnStateUpdate(ECreatureState updateState)
        {
            switch (updateState)
            {
                case ECreatureState.Skill_Attack:
                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    // 스킬을 사용하는 주체가 크리처이기 때문에 여기서 이벤트 등록, 삭제하고 호출
                    CreatureSkill.PassOnSkillStateUpdate(updateState);
                    break;

                case ECreatureState.CollectEnv:
                    OnCollectEnvStateUpdate();
                    break;
            }
        }

        protected virtual void OnCollectEnvStateUpdate() { }

        protected void OnStateEnd(ECreatureState endState)
        {
            switch (endState)
            {
                case ECreatureState.Skill_Attack:
                case ECreatureState.Skill_A:
                case ECreatureState.Skill_B:
                    CreatureSkill?.PassOnSkillStateEnd(endState);
                    break;
            }
        }
        #endregion State Machine Events 

        #region Helper
        protected BaseObject FindClosestInRange(float range, IEnumerable<BaseObject> objs, Func<BaseObject, bool> func = null)
        {
            BaseObject target = null;
            float bestDistanceSQR = float.MaxValue;
            float searchDistanceSQR = range * range;

            foreach (BaseObject obj in objs)
            {
                Vector3 dir = obj.transform.position - transform.position;
                float distToTargetSqr = dir.sqrMagnitude;

                if (searchDistanceSQR < distToTargetSqr)
                    continue;

                if (bestDistanceSQR < distToTargetSqr)
                    continue;

                // 추가 조건 (ex)객체가 살아있는가? 람다 기반으로 넘겨줄수도 있으니까 뭐, 편함 이런게 있으면.
                // 살아있는 객체만 찾아서, 살아있는 객체에게만 Target을 잡을 수 있도록. 이런식으로 가능.
                if (func?.Invoke(obj) == false)
                    continue;

                bestDistanceSQR = distToTargetSqr;
                target = obj;
            }

            return target;
        }

        #region Battle
        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            Creature creature = attacker as Creature;
            if (creature.IsValid() == false)
                return;

            float finalDamage = creature.Atk;
            Hp = UnityEngine.Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);

            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillFromAttacker);
            }
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            CreatureState = ECreatureState.Dead;
            base.OnDead(attacker, skillFromAttacker);
        }
        #endregion Battle

        protected void ChaseOrAttackTarget(float chaseRange, float atkRange)
        {
            //Vector3 toTargetDir = Target.transform.position - transform.position;
            if (DistanceToTargetSQR <= atkRange * atkRange)
            {
                if (Target.IsValid() && Target.ObjectType == EObjectType.Env)
                    CreatureState = ECreatureState.CollectEnv;
                else
                    CreatureSkill?.CurrentSkill.DoSkill();
            }
            else
            {
                _findPathResult = FindPathAndMoveToCellPos(Target.transform.position, ReadOnly.Numeric.HeroMaxMoveDepth);
                float searchDistSQR = chaseRange * chaseRange;
                if (DistanceToTargetSQR > searchDistSQR)
                {
                    Target = null;
                    CreatureState = ECreatureState.Move;
                }
            }
        }

        // Util로 빼야할듯
        protected float CalculateMovementSpeed(float distanceToTargetSQR)
        {
            float maxDistance = ReadOnly.Numeric.MaxDistanceForMovementSpeed;
            float maxMovementSpeed = MovementSpeed * ReadOnly.Numeric.MaxMovementSpeedMultiplier;
            float movementSpeed = Mathf.Lerp(MovementSpeed, 
                                            maxMovementSpeed, 
                                            Mathf.Log(distanceToTargetSQR + 1.0f) / Mathf.Log(maxDistance * maxDistance + 1.0f));

            return movementSpeed;
        }

        protected virtual void OnDisable()
        {
            Debug.Log("Creature::OnDisable");
            ReleaseAnimationEvents();
        }

        #region Misc
        protected bool IsValid(BaseObject bo) => bo.IsValid();
        #endregion Misc

        #region Map

        // maxDepth: Mobile용 성능 조절 Offset 깊이 값
        public EFindPathResult FindPathAndMoveToCellPos(Vector3 destPos, int maxDepth, bool forceMoveCloser = false)
        {
            Vector3Int destCellPos = Managers.Map.WorldToCell(destPos);
            return FindPathAndMoveToCellPos(destCellPos, maxDepth, forceMoveCloser);
        }

        public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destPos, int maxDepth, bool forceMoveCloser = false)
        {
            // 움직임 진행중
            if (LerpToCellPosCompleted == false)
            {
                return EFindPathResult.Fail_LerpCell;
            }

            // A*
            List<Vector3Int> path = Managers.Map.FindPath(startCellPos: CellPos, destPos, maxDepth);
            if (path.Count < 2)
                return EFindPathResult.Fail_NoPath;

            #region TEST
            // 다른 오브젝트가 길막해서 와리가리할수있다는데, 그럴때 diff1, diff2의 점수 계산을 해서 안가게끔 막는 것이라고 함.
            // 근데 아주 예외적인 케이스라고함.
            // if (forceMoveCloser)
            // {
            //     Vector3Int diff1 = CellPos - destPos;
            //     Vector3Int diff2 = path[1] - destPos;
            //     if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
            //         return EFindPathResult.Fail_NoPath;
            // }
            #endregion

            Vector3Int dirCellPos = path[1] - CellPos;
            Vector3Int nextPos = CellPos + dirCellPos;

            if (Managers.Map.MoveTo(creature: this, nextPos) == false)
                return EFindPathResult.Fail_MoveTo;

            return EFindPathResult.Success;
        }

        public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
        {
            if (LerpToCellPosCompleted == false)
                return false;

            return Managers.Map.MoveTo(this, cellPos: destCellPos);
        }


        protected void TickLerpToCellPos()
        {
            StopLerpToCellPos();
            _coLerpToCellPos = StartCoroutine(CoLerpToCellPos());
        }

        protected void StopLerpToCellPos()
        {
            if (_coLerpToCellPos != null)
            {
                StopCoroutine(_coLerpToCellPos);
                _coLerpToCellPos = null;
            }
        }

        // ### Creature: O, Projectile: X
        protected Coroutine _coLerpToCellPos = null;
        protected IEnumerator CoLerpToCellPos()
        {
            while (true)
            {
                Hero hero = this as Hero;
                // if (hero.IsLeader)
                // {
                //     Debug.Log("BREAK LEADER AI.");
                //     yield break;
                // }

                // 여기도 고쳐야할듯
                if (hero != null)
                {
                    //float divOffsetSQR = 5f * 5f;
                    // pointerCellPos : 중요하진않음. 그냥 이속조절을 위한 용도 뿐
                    // ***** Managers.Object.Camp.Pointer.position ---> Leader로 변경 예정 *****
                    //Vector3Int pointerCellPos = Managers.Map.WorldToCell(Managers.Object.Camp.Pointer.position);
                    // Vector3Int pointerCellPos = Managers.Map.WorldToCell(Managers.Object.LeaderController.Leader.transform.position);
                    //float ratio = Mathf.Max(1, (CellPos - pointerCellPos).sqrMagnitude / divOffsetSQR); // --> 로그로 변경 필요
                    //LerpToCellPos(MovementSpeed * ratio);
                    LerpToCellPos(MovementSpeed);
                }
                else
                    LerpToCellPos(MovementSpeed);

                yield return null;
            }
        }
        #endregion Map
        #endregion Helper


    }
}
