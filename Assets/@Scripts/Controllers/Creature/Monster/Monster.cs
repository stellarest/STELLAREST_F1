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

        public override Vector3Int ChaseCellPos
        {
            get
            {
                if (Target.IsValid() == false)
                {
                    return _patrolCellPos;
                }
                else
                {
                    return Target.CellPos;
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
                EnterInGame();
                return false;
            }

            InitialSetInfo(dataID);
            EnterInGame();
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            MonsterBody = new MonsterBody(this, dataID);
            MonsterAnim = CreatureAnim as MonsterAnimation;
            MonsterAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, target: this);

            MonsterData = Managers.Data.MonsterDataDict[dataID];
            CreatureRarity = Util.GetEnumFromString<ECreatureRarity>(MonsterData.CreatureRarity);
            MonsterType = Util.GetEnumFromString<EMonsterType>(MonsterData.Type);

            gameObject.name += $"_{MonsterData.DescriptionTextID.Replace(" ", "")}";
            Collider.radius = MonsterData.ColliderRadius;

            CreatureSkill = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkill.SetInfo(owner: this, MonsterData);
        }

        protected override void EnterInGame()
        {
            _initPos = transform.position;
            LookAtDir = ELookAtDirection.Left;
            CreatureState = ECreatureState.Idle;
            _desiredNextPingPongPatrolDelta = Random.Range(10f, 12f);

            StartCoSearchTarget<Creature>(scanRange: ReadOnly.Numeric.MonsterDefaultScanRange,
                                         firstTargets: Managers.Object.Heroes,
                                         secondTargets: null,
                                         func: IsValid);

            base.EnterInGame();
        }

        private Vector3 _destPos = Vector3.zero;
        private Vector3 _initPos = Vector3.zero;

        #region AI
        private float _waitPingPongPatrolDelta = 0f;
        private float _desiredNextPingPongPatrolDelta = 0f;
        private float SetDesiredNextPingPongPatrolDelta(float minSec, float maxSec)
            => Random.Range(minSec, maxSec);

        protected override void UpdateIdle()
        {
            LookAtValidTarget();

            // ...Check CoolTime... //
            if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
                return;

            if (Target.IsValid())
            {
                StopCoPingPongPatrol(); // ---> 패트롤은 아이들에서만 실행하기 때문에 여기서만 체크해주면 됨.
                CreatureMoveState = ECreatureMoveState.MoveToTarget;
                CreatureState = ECreatureState.Move;
                return;
            }
            else if (_coPingPongPatrol == null)
            {
                _waitPingPongPatrolDelta += Time.deltaTime;
                if (_waitPingPongPatrolDelta >= _desiredNextPingPongPatrolDelta)
                {
                    _waitPingPongPatrolDelta = 0f;
                    _desiredNextPingPongPatrolDelta = SetDesiredNextPingPongPatrolDelta(2f, 4f);
                    StartCoPingPongPatrol(-5f, 5f);
                }
            }
        }

        // ##### TargetToEnemy였는데 Idle로 되어 있어서 가만히 있었던 버그 있었던 것 같기도... 확인 필요 #####
        protected override void UpdateMove()
        {
            LookAtValidTarget();
            EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos, maxDepth: ReadOnly.Numeric.MonsterDefaultMoveDepth);
            Vector3 centeredPos = Managers.Map.CenteredCellToWorld(ChaseCellPos);

            // result == EFindPathResult.Fail_NoPath 근처로 갔으면 멈추거라
            if ((transform.position - centeredPos).sqrMagnitude < 0.01f || result == EFindPathResult.Fail_NoPath)
            {
                if (Target.IsValid() == false)
                {
                    CreatureState = ECreatureState.Idle;
                    return;
                }
                // 일단 되긴 함... 그리고 Chicken StateMachine 붙어있는지도 확인하고 ForceExit를 통해서 호출되는지도 확인하고
                else if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack) == false)
                {
                    CreatureSkill?.CurrentSkill.DoSkill();
                    return;
                }
            }
        }

        protected override void UpdateSkill()
        {

        }
        #endregion

        #region Battle
        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDamaged(attacker, skillFromAttacker);

            if (Target.IsValid() == false && attacker.IsValid())
            {
                float minSec = ReadOnly.Numeric.MinSecWaitSearchTargetForSettingAggroFromRange;
                float maxSec = ReadOnly.Numeric.MaxSecWaitSearchTargetForSettingAggroFromRange;
                StartCoWaitSearchTarget(Random.Range(minSec, maxSec));
                Target = attacker;
            }
            else
                StopCoWaitSearchTarget();
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            StopCoPingPongPatrol();
            base.OnDead(attacker, skillFromAttacker);
        }
        protected override void OnDisable()
        {
            //Debug.Log("Monster::OnDisable");
            base.OnDisable();
        }
        #endregion

        #region Coroutines for monsters
        private Coroutine _coPingPongPatrol = null;
        private Vector3Int _patrolCellPos = Vector3Int.zero;
        private bool _patrolPingPongFlag = false;
        private IEnumerator CoPingPongPatrol(float minDistance, float maxDistance)
        {
            int attemptCount = 0;
            int maxAttemptCount = 100;
            Vector3 _initialSpawnPos = Managers.Map.CenteredCellToWorld(InitialSpawnedCellPos.Value);
            if (_patrolPingPongFlag == false)
            {
                float x = _initialSpawnPos.x + Random.Range(minDistance, maxDistance);
                float y = _initialSpawnPos.y + Random.Range(minDistance, maxDistance);
                _patrolCellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                bool isShortDist = (InitialSpawnedCellPos.Value - _patrolCellPos).sqrMagnitude < 9f; // 최소 3칸이상으로 움직여라
                while (Managers.Map.CanMove(_patrolCellPos) == false || isShortDist)
                {
                    if (attemptCount++ >= maxAttemptCount)
                    {
                        _patrolCellPos = InitialSpawnedCellPos.Value;
                        break;
                    }

                    x = _initialSpawnPos.x + Random.Range(minDistance, maxDistance);
                    y = _initialSpawnPos.y + Random.Range(minDistance, maxDistance);
                    _patrolCellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                    if ((InitialSpawnedCellPos.Value - _patrolCellPos).sqrMagnitude < 9f)
                        isShortDist = true;
                    else
                        isShortDist = false;

                    yield return null;
                }

                if (_patrolCellPos == InitialSpawnedCellPos.Value) // --- DEFENSE 
                {
                    // 어차피 Idle에서 패트롤 시도를 하기 때문에 이거 필요 없음.
                    // CreatureMoveState = ECreatureMoveState.None;
                    // CreatureState = ECreatureState.Idle;
                    Debug.LogWarning("FAILED TO SET PATROL POS. RETRY AGAIN FROM IDLE STATE.");
                    StopCoPingPongPatrol(); // ---> 이것만 해주면 됨.
                    yield break;
                }
            }
            else
            {
                // --- InitialSpawnedCellPos.Value 이미 Spawn을 통해 Non-blocking 되어있는 위치
                _patrolCellPos = InitialSpawnedCellPos.Value;
            }

            CreatureState = ECreatureState.Move;
            _patrolPingPongFlag = !_patrolPingPongFlag;
            StopCoPingPongPatrol();
        }

        private void StartCoPingPongPatrol(float minDistance, float maxDistance)
        {
            if (CreatureState != ECreatureState.Idle)
            {
                Debug.LogWarning("Patrol can only run in idle state.");
                return;
            }

            if (_coPingPongPatrol == null)
                _coPingPongPatrol = StartCoroutine(CoPingPongPatrol(minDistance, maxDistance));
        }

        private void StopCoPingPongPatrol()
        {
            if (_coPingPongPatrol != null)
            {
                StopCoroutine(_coPingPongPatrol);
                _coPingPongPatrol = null;
            }
        }
        #endregion 
    }
}
