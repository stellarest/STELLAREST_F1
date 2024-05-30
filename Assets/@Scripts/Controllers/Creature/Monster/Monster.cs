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
                EnterInGame(dataID);
                return false;
            }
            SortingGroup.sortingOrder = 100; // TEST

            MonsterBody = new MonsterBody(this, dataID);
            MonsterAnim = CreatureAnim as MonsterAnimation;
            MonsterAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, target: this);

            MonsterData = Managers.Data.MonsterDataDict[dataID];
            MonsterType = Util.GetEnumFromString<EMonsterType>(MonsterData.Type);

            gameObject.name += $"_{MonsterData.DescriptionTextID.Replace(" ", "")}";
            Collider.radius = MonsterData.ColliderRadius;

            CreatureSkill = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkill.SetInfo(this, Managers.Data.MonsterDataDict[dataID].SkillIDs);

            EnterInGame(dataID);
            return true;
        }

        protected override void EnterInGame(int dataID)
        {
            _initPos = transform.position;
            LookAtDir = ELookAtDirection.Left;
            CreatureState = ECreatureState.Idle;

            StartCoSearchTarget<Creature>(scanRange: ReadOnly.Numeric.MonsterDefaultScanRange,
                                         firstTargets: Managers.Object.Heroes,
                                         secondTargets: null,
                                         func: IsValid);

            base.EnterInGame(dataID);
        }

        private Vector3 _destPos = Vector3.zero;
        private Vector3 _initPos = Vector3.zero;

        #region AI
        protected override void UpdateIdle()
        {
            LookAtValidTarget();

            // ...Check CoolTime... //
            if (Target.IsValid())
            {
                CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                CreatureState = ECreatureState.Move;
            }
            else
            {
                  // ... Random Patrol ...
            }


            // 나중에 Hero 뿐만이 아니라, Pet등이 될 수도 있으므로 Creature로 받기. 뒤에 인자는 일단 임의로  Managers.Object.Heroes.
            // Creature creature = FindClosestInRange(ReadOnly.Numeric.CreatureDefaultScanRange, Managers.Object.Heroes, func: IsValid) as Creature;
            // if (creature.IsValid())
            // {
            //     Target = creature;
            //     CreatureMoveState = ECreatureMoveState.TargetToEnemy;
            //     CreatureState = ECreatureState.Move;
            // }
            // else
            // {
            //     // ... Random Patrol ...
            // }

            // if (Target.IsValid())
            //     LookAtTarget(Target);

            // if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
            //     return;

            // {
            //     if (Target.IsValid() == false)
            //     {
            //         int patrolPercent = 20;
            //         if (UnityEngine.Random.Range(0, 100) <= patrolPercent)
            //         {
            //             _destPos = _initPos + new Vector3(Random.Range(-4, 4), Random.Range(-4, 4));
            //             CreatureState = ECreatureState.Move;
            //             return;
            //         }
            //     }
            // }

            // {
            //     // Research Enemy
            //     Creature creature = FindClosestInRange(ReadOnly.Numeric.CreatureDefaultScanRange, Managers.Object.Heroes) as Creature;
            //     if (creature.IsValid())
            //     {
            //         Target = creature;
            //         CreatureState = ECreatureState.Move;
            //         CreatureMoveState = ECreatureMoveState.TargetToEnemy;
            //         return;
            //     }
            // }
        }

        // ##### TargetToEnemy였는데 Idle로 되어 있어서 가만히 있었던 버그 있었던 것 같기도... 확인 필요 #####
        protected override void UpdateMove()
        {
            // Research Target, 혹시 모르니까 더 가까운 녀석이 있을 경우 그녀석으로 타겟 변경. 한번 더 탐색. 근데 이거 코루틴으로 해도 될듯? 무거울 것 같아서.
            // if (Target.IsValid())
            // {
            //     int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
            //     int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);

            //     if (dx <= AtkRange && dy <= AtkRange)
            //     {
            //         // Attack Skill
            //     }
            //     else
            //     {
            //         // Move to Target
            //         EFindPathResult result = FindPathAndMoveToCellPos(destPos: Target.CellPos,
            //             maxDepth: ReadOnly.Numeric.MonsterDefaultMoveDepth);
            //     }
            // }

            // =========================================================================================================
            // if (Target.IsValid() == false)
            // {
            //     Creature creature = FindClosestInRange(ReadOnly.Numeric.CreatureDefaultScanRange, Managers.Object.Heroes, func: IsValid) as Creature;
            //     if (creature != null)
            //     {
            //         Target = creature;
            //         CreatureState = ECreatureState.Move;
            //         return;
            //     }

            //     _findPathResult = FindPathAndMoveToCellPos(destPos: _destPos, ReadOnly.Numeric.MonsterDefaultMoveDepth);
            //     if (LerpToCellPosCompleted)
            //     {
            //         CreatureState = ECreatureState.Idle;
            //         return;
            //     }
            // }
            // else
            // {
            //     //SkillBase skill = Skills.GetReadySkill();
            //     ChaseOrAttackTarget(ReadOnly.Numeric.CreatureDefaultScanRange, AttackDistance);
                
            //     // 너무 멀어지면 포기
            //     if (Target.IsValid() == false)
            //     {
            //         Target = null;
            //         _destPos = _initPos;
            //         return;
            //     }
            // }
        }

        // Attack이나 Skill은 실행 이후 Creature Move로 돌아가면 된다.
        protected override void UpdateSkill()
        {
            // SetRigidBodyVelocity(Vector2.zero); - DELETED
            // if (Target.IsValid())
            //     LookAtTarget(Target);
            // else
            // {
            //     Target = null;
            //     _destPos = _initPos;
            //     CreatureState = ECreatureState.Move;
            // }
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
