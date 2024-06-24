using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Monster : Creature
    {
        #region Background
        public Data.MonsterData MonsterData { get; private set; } = null;
        [SerializeField] private MonsterBody _monsterBody = null;
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

        public override ECreatureAIState CreatureAIState
        {
            get => base.CreatureAIState;
            set
            {
                base.CreatureAIState = value;
                switch (value)
                {

                    case ECreatureAIState.Idle:
                        {
                            Moving = false;
                            return;
                        }

                    case ECreatureAIState.Move:
                        {
                            Moving = true;
                            return;
                        }

                    case ECreatureAIState.Dead:
                        {
                            Dead();
                            return;
                        }
                }
            }
        }
        public MonsterAI MonsterAI { get; private set; } = null;
        #endregion

        #region Core
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
            Type aiClassType = Util.GetTypeFromName(MonsterData.AIClassName);
            CreatureAI = gameObject.AddComponent(aiClassType) as CreatureAI;
            CreatureAI.SetInfo(this);
            MonsterAI = CreatureAI as MonsterAI;

            CreatureRarity = Util.GetEnumFromString<ECreatureRarity>(MonsterData.CreatureRarity);
            MonsterType = Util.GetEnumFromString<EMonsterType>(MonsterData.Type);

            gameObject.name += $"_{MonsterData.DescriptionTextID.Replace(" ", "")}";
            Collider.radius = MonsterData.ColliderRadius;

            CreatureSkill = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkill.SetInfo(owner: this, MonsterData);
        }

        protected override void EnterInGame()
        {
            // --- Monsters Default Dir
            LookAtDir = ELookAtDirection.Left;
            base.EnterInGame();
            CreatureAIState = ECreatureAIState.Idle;
        }

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDamaged(attacker, skillFromAttacker);
            // Debug.Log($"<color=white>{gameObject.name}: ({Hp}/{MaxHp})</color>");
            
            // 어그로 시스템 일단 중지
            // if (Target.IsValid() == false && attacker.IsValid())
            // {
            //     float minSec = ReadOnly.Numeric.MinSecWaitSearchTargetForSettingAggroFromRange;
            //     float maxSec = ReadOnly.Numeric.MaxSecWaitSearchTargetForSettingAggroFromRange;
            //     StartCoWaitSearchTarget(UnityEngine.Random.Range(minSec, maxSec));
            //     Target = attacker;
            // }
            // else
            //     StopCoWaitSearchTarget();
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDead(attacker, skillFromAttacker);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        #endregion
    }
}

/*
    [Prev Ref]
  // protected override void UpdateIdle()
        // {
        //     LookAtValidTarget();

        //     if (CanSkill(ESkillType.Skill_Attack) == false)
        //         return;

        //     if (Target.IsValid())
        //     {
        //         if (IsInAttackRange() && CanSkill(ESkillType.Skill_Attack))
        //         {
        //             Debug.Log("<color=yellow>Do BodyAttack</color>");
        //             StopCoLerpToCellPos();
        //             CreatureSkill.CurrentSkill.DoSkill();
        //             return;
        //         }

        //         StopCoPingPongPatrol(); // ---> 패트롤은 아이들에서만 실행하기 때문에 여기서만 체크해주면 됨.
        //         CreatureAIState = ECreatureAIState.Move;
        //         return;
        //     }
        //     else if (_coPingPongPatrol == null)
        //     {
        //         _waitPingPongPatrolDelta += Time.deltaTime;
        //         if (_waitPingPongPatrolDelta >= _desiredNextPingPongPatrolDelta)
        //         {
        //             _waitPingPongPatrolDelta = 0f;
        //             _desiredNextPingPongPatrolDelta = SetDesiredNextPingPongPatrolDelta(2f, 4f);
        //             StartCoPingPongPatrol(-5f, 5f);
        //         }
        //     }
        // }

        // // ##### TargetToEnemy였는데 Idle로 되어 있어서 가만히 있었던 버그 있었던 것 같기도... 확인 필요 #####
        // protected override void UpdateMove()
        // {
        //     LookAtValidTarget();
        //     EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos, maxDepth: ReadOnly.Numeric.MonsterDefaultMoveDepth);
        //     Vector3 centeredPos = Managers.Map.CenteredCellToWorld(ChaseCellPos);

        //     if (Target.IsValid() == false)
        //     {
        //         if ((transform.position - centeredPos).sqrMagnitude < 0.01f || result == EFindPathResult.Fail_NoPath)
        //         {
        //             CreatureAIState = ECreatureAIState.Idle;
        //             return;
        //         }
        //     }
        //     // 치킨 모습 전에 업데이트가 되고나서 스킬을 사용해서 이꼴나는 것임.
        //     if (Target.IsValid() && IsInAttackRange())
        //     {
        //         CreatureAIState = ECreatureAIState.Idle;
        //         return;

        //         // if (CanSkill(ESkillType.Skill_Attack))
        //         // {
        //         //     CreatureAIState = ECreatureAIState.Skill;
        //         //     CreatureSkill.CurrentSkill.DoSkill();
        //         //     return;
        //         // }

        //         // if (IsInAttackRange() && CanSkill(ESkillType.Skill_Attack))
        //         // {
        //         //      CreatureAIState = ECreatureAIState.Idle;
        //         //      CreatureSkill?.CurrentSkill.DoSkill();
        //         // }
        //     }

        //     // result == EFindPathResult.Fail_NoPath 근처로 갔으면 멈추거라
        //     // if ((transform.position - centeredPos).sqrMagnitude < 0.01f || result == EFindPathResult.Fail_NoPath)
        //     // {
        //     //     if (Target.IsValid() == false)
        //     //     {
        //     //         CreatureAIState = ECreatureAIState.Idle;
        //     //         return;
        //     //     }
        //     //     // else if (CanSkill(ESkillType.Skill_Attack))
        //     //     // {
        //     //     //     CreatureAIState = ECreatureAIState.Idle;
        //     //     //     CreatureSkill?.CurrentSkill.DoSkill();
        //     //     //     return;
        //     //     // }

        //     //     // 일단 되긴 함... 그리고 Chicken StateMachine 붙어있는지도 확인하고 ForceExit를 통해서 호출되는지도 확인하고
        //     //     // else if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack) == false)
        //     //     // {
        //     //     //     CreatureSkill?.CurrentSkill.DoSkill();
        //     //     //     return;
        //     //     // }
        //     // }
        // }
*/