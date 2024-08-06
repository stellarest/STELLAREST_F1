using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

/*
// --- Chicken AI 고쳐기 (바디어택)
// --- 히어로 리더 둘러쌓여있을 때 조이스틱 방향 참고
// --- 조이스틱을 돌리고 공격할 때, Target을 바라보지 않는거(아니면 이미 공격 동작 나오면 데미지 들어가게, 근데 이건 어쌔신때문에 안될듯)
// --- 나머지 애니메이션 간소화
// --- 프로젝타일 테스트

public class Test1 : IEnumerator
{
    public object Current => throw new System.NotImplementedException();

    public bool MoveNext()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }
}

public class Test2 : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        throw new System.NotImplementedException();
    }
}

public class T_E_S_T : MonoBehaviour
{
    public GameObject Player;
    public GameObject Monster;

    private void OnDrawGizmos()
    {
        Vector3 toMonsterDir = (Monster.transform.position - Player.transform.position).normalized;
        float dot = Vector2.Dot(Player.transform.right, toMonsterDir);

        // --- Draw line: Player -> Monster
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(from: Player.transform.position, to: Monster.transform.position);

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        Handles.Label((Player.transform.position + Monster.transform.position) / 2, $"Dot: {dot:F2}", style);
    }

    // private IEnumerator Start()
    // {
    //     yield return new WaitForSeconds(1f);
    //     Sequence seq = DOTween.Sequence();
    //     // seq.Append(transform.DOMove(new Vector3(3f, 3f, 0), 3f));

    //     // seq.Join(transform.DOScale(new Vector3(3f, 3f, 0), 3f));
    //     // seq.Join(GetComponent<SpriteRenderer>().material.DOColor(Color.red, 3f));

    //     // seq.Append(transform.DORotate(new Vector3(0, 0, -135f), 3f));
    //     seq.Append(transform.DOScale(endValue: 1.3f, duration: 0.3f)).SetEase(Ease.InOutBounce)
    //    .Join(transform.DOMove(endValue: transform.position + Vector3.up, duration: 0.3f)).SetEase(Ease.Linear)
    //    .Append(transform.DOScale(endValue: 1.0f, duration: 0.3f).SetEase(Ease.InOutBounce))
    //    .Join(transform.GetComponent<SpriteRenderer>().material.DOFade(endValue: 0f, duration: 0.3f)).SetEase(Ease.InQuint)
    //    .OnComplete(() => Debug.Log("COMPLETED !!"));

    //     seq.Play();
    // }
}

/* 
[ PREV NOTE ]
// --- Skills
// -- Paladin (Common)
// - Skill_A : Double Attack
// - Skill_B : Shield

// -- Paladin (Elite)
// - Skill_A : Triple Attack
// - Skill_B : Defense of heavens

// -- Archer (Common)
// - Skill_A : Multi Shot (Three Arrows)
// - Skill_B : Elemental Bomb Shot

// -- Archer (Elite)
// - Skill_A : Triple Multi + Continuous + Penetrated Shot
// - Skill_B : Five Elemental Arrows (Guided Arrows)


곡선은 삼각함수, 베지에 곡선으로도 가능.
GameObject.CreatePrimitive(PrimitiveType.Sphere)

Cloud Services - Save data in the cloud (iCloud & Saved Games)
이거 가능한가본데?(Cross-Platform Native Plugins : Essential Kit (Mobile - iOS & Android 에셋 참고)

Data ID Rule Temp 
1xxxxx : Creature
- 101xxx : Hero
- 102xxx : Monster
- 103xxx : Creature Etc

2xxxxx : Skills
- 201xxx : Base Skills
- 202xxx : AoE
- 203xxx : Effect
- 204xxx : Projectile

3xxxxx : Items
- 301xxx : Base Items

9xxxxx : Etc
- 901xxx : Env
- 902xxx : NPC

- 레디스는 디비 캐싱용도
- 요일던전같은 스케쥴관련된 컨텐츠? 세이브파일에 시간 연산을 넣어서. 해킹은 어쩔 수 없이 가능것임.

StateBehaviourMachine - 하나의 스크립트 = 하나의 스테이트로 해야함..ㅅㅂ;;

데미지 처리는 항상 피해를 받는 입장(Creature.OnDamaged)에서 처리하는 것이 정석.

Rigidbody Velocity는 방향 * 크기만 넣어주면 된다 (시간은 넣는것이 아니다)

// public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
// {
//     float endThreshold = 0.9f;
//     if (stateInfo.shortNameHash == _heroAnim?.GetHash(ECreatureState.Attack))
//     {
//         if (stateInfo.normalizedTime >= endThreshold)
//             _owner.CreatureState = ECreatureState.Idle;
//     }
// }

private void Update()
{
    float moveDistPerFrame = Speed * Time.deltaTime;
    transform.TranslateEx(MoveDir * moveDistPerFrame);
}

캐릭터 해금의 재미를 줘야할듯
1. Paladin - Common > Ultimate
2. Archer - Common > Ultimate
3. Wizard - Common > Ultimate
4. Warrior - Common > Ultimate
5. Assassin - Elite
6. Druid (Forest Guardian)
7. Mutant
8. Skeleton King
9. Phantom Knight
10. Babarian
11. Gunner
12. Sniper
13. Hunter
14. Trickster
15. Ninja
16. Thief
17. Pirate
18. Queen
19. King
20. ...
21. ...
30. Baseball Player
---------------
7. Lancer

// 아이들
// 이동
// 공격
// 죽음

Paladin
- Skill_A : 강타 -> 더블 어택(Unique) -> 트리플 어택(Elite)
- Skill_B : 인내 -> 쉴드(Unique) -> 축복의 심판(Elite)

Archer
- Skill_A : 더블샷 -> 멀티샷(Unique) -> 연발 멀티샷(Elite)
- Skill_B : 윈드 애로우(슬로우) -> 넉백 애로우(넉백 + 스턴)


        // protected void ChaseOrAttackTarget(float attackRange, float chaseRange)
        // {
        //     Vector3 toDir = Target.transform.position - transform.position;
        //     float distToTargetSQR = toDir.sqrMagnitude;

        //     float attackDistToTargetSQR = attackRange * attackRange;
        //     // ATTACK
        //     if (attackDistToTargetSQR >= distToTargetSQR)
        //     {
        //         if (CreatureMoveState == ECreatureMoveState.TargetToEnemy)
        //         {
        //             CreatureState = ECreatureState.Skill_Attack;
        //         }
        //         else if (CreatureMoveState == ECreatureMoveState.CollectEnv)
        //         {
        //             CollectEnv = true;
        //             CreatureState = ECreatureState.CollectEnv;
        //         }
        //         return;
        //     }
        //     // CHASE
        //     else
        //     {
        //         SetRigidBodyVelocity(toDir.normalized * MovementSpeed);
        //         // 너무 멀어지면 포기
        //         if (distToTargetSQR > chaseRange * chaseRange)
        //         {
        //             Target = null;
        //             CreatureState = ECreatureState.Move;
        //         }
        //         return;
        //     }
        // }

*/

// [Obsolete]
// public void SetRigidBodyVelocity(Vector2 velocity) - DELETED
// {
//     if (RigidBody == null)
//         return;

//     RigidBody.velocity = velocity;
//     if (velocity == Vector2.zero) // DO NOT FLIP.
//         return;

//     if (velocity.x < 0)
//         LookAtDir = ELookAtDirection.Left;
//     else
//         LookAtDir = ELookAtDirection.Right;
// }

/*
    - 상하좌우 - 
    1칸 : 1
    2칸 : 4
    3칸 : 9
    4칸 : 16

    대각선 1칸 : 2
    대각선 2칸: 8
    대각선 3칸: 13
*/

/*
        Collider.includeLayers = 1 << (int)ELayer.Obstacle;
        Collider.excludeLayers = 1 << (int)ELayer.Monster | (1 << (int)ELayer.Hero);

        // protected float DistanceToTargetSQR
        // {
        //     get
        //     {
        //         if (Target.IsValid() == false)
        //             return 0.0f;

        //         Vector3 toTargetDir = Target.transform.position - transform.position;
        //         return UnityEngine.Mathf.Max(0.0f, toTargetDir.sqrMagnitude); // ??? 의미 없는데 어차피 무조건 양수 나오는데
        //     }
        // }

        // protected float AttackDistance // TEMP
        // {
        //     get
        //     {
        //         float threshold = 2.2f;
        //         if (Target.IsValid() && Target.ObjectType == EObjectType.Env)
        //             return UnityEngine.Mathf.Max(threshold, Collider.radius + Target.Collider.radius);

        //         return AtkRange + Collider.radius + Target.ColliderRadius;
        //     }
        // }

        // public void ShowBody(bool show)
        // {
        //     if (show == false)
        //         Collider.enabled = false;

        //     // includeInactive: true (임시, 나중에 개선 필요)
        //     foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>(includeInactive: true))
        //     {
        //         spr.enabled = show;
        //         if (show && spr.sprite != null)
        //         {
        //             //spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
        //             spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, spr.color.a);
        //             spr.gameObject.SetActive(true);
        //         }
        //     }

        //     if (show)
        //         Collider.enabled = true;
        // }

*/

/*
    [PREV REF: HERO]
    public override Vector3Int ChaseCellPos
        {
            get
            {
                if (ForceMove == false && Target.IsValid())
                    return Target.CellPos;
                
                HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
                switch (leaderController.HeroMemberFormationMode)
                {
                    case EHeroMemberFormationMode.FollowLeaderClosely:
                        return Managers.Map.WorldToCell(leaderController.Leader.transform.position); ;

                    case EHeroMemberFormationMode.NarrowFormation:
                    case EHeroMemberFormationMode.WideFormation:
                        return leaderController.RequestFormationCellPos(this); ;

                    case EHeroMemberFormationMode.RandomFormation:
                        return leaderController.RequestRandomFormationCellPos(this);

                    case EHeroMemberFormationMode.ForceStop:
                        return HeroAI.PrevCellPosForForceStop;
                }

                return Vector3Int.zero;
            }
        }
     // public override Vector3Int ChaseCellPos
        // {
        //     get
        //     {
        //         if (CreatureMoveState != ECreatureMoveState.ForceMove && Target.IsValid())
        //         {
        //             if (CanAttackOrChase())
        //             {
        //                 if (_canHandleSkill)
        //                 {
        //                     if (Target.ObjectType == EObjectType.Env)
        //                         CreatureAIState = ECreatureAIState.CollectEnv;
        //                     else if (Target.ObjectType == EObjectType.Monster)
        //                     {
        //                         if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack) == false)
        //                             CreatureSkill?.CurrentSkill.DoSkill();
        //                         else
        //                             CreatureAIState = ECreatureAIState.Idle;
        //                     }
        //                 }
        //             }

        //             return Target.CellPos;
        //         }

        //         HeroLeaderController heroLeaderController = Managers.Object.HeroLeaderController;
        //         switch (heroLeaderController.HeroMemberFormationMode)
        //         {
        //             case EHeroMemberFormationMode.FollowLeaderClosely:
        //             {
        //                 return Managers.Map.WorldToCell(heroLeaderController.Leader.transform.position);
        //             }

        //             case EHeroMemberFormationMode.NarrowFormation:
        //             case EHeroMemberFormationMode.WideFormation:
        //             {
        //                 return Managers.Object.HeroLeaderController.RequestFormationCellPos(this);
        //             }

        //             case EHeroMemberFormationMode.RandomFormation:
        //             {
        //                 return Managers.Object.HeroLeaderController.RequestRandomFormationCellPos(this);
        //             }

        //             case EHeroMemberFormationMode.ForceStop:
        //             {
        //                 return PrevCellPosForForceStop;
        //             }

        //             default:
        //                 return Vector3Int.zero;
        //         }
        //     }
        // }

        // private void Update()
        // {
        //     if (IsLeader)
        //         return;

        //     if (MoveAfterLeader())
        //     {
        //         EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos,
        //             maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

        //         if (result == EFindPathResult.Fail_ForceMovePingPongObject) // 확인됨
        //             return;

        //         if (result == EFindPathResult.Fail_NoPath)
        //             IsMoving = false;
        //         else
        //             IsMoving = true;
        //     }
        // }

        // private void LateUpdate()
        // {
        //     if (IsLeader)
        //         return;

        //     // 와리가리 핑퐁 체크
        // }

        protected override void UpdateMove()
        {
            if (IsLeader)
                return;

            if (ForceMove == false && IsInTargetAttackRange())
            {
                CreatureAIState = ECreatureAIState.Idle;
                return;
            }

            EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos,
                maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

            if (result == EFindPathResult.Fail_NoPath)
            {
                Hero leader = Managers.Object.HeroLeaderController.Leader;
                if (leader.IsMoving == false)
                {
                    //Debug.Log("111-111");
                    CreatureAIState = ECreatureAIState.Idle;
                }

                return;
            }
        }
    private bool MoveAfterLeader()
        {
            if (_isFarFromLeader)
            {
                PauseSearchTarget = true;
                return true;
            }

            if (ForceMove)
            {
                // --- 리더가 이동한다고해서 바로 움직이지 않는다.
                Hero leader = Managers.Object.HeroLeaderController.Leader;
                if ((transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
                    return true;

                List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
                if (idlePathFind.Count > 1)
                {
                    if (Managers.Map.CanMove(idlePathFind[1]))
                        return true;
                }
            }

            return false;
        }
     // Prev
        // private void OnJoystickStateChanged(EJoystickState joystickState)
        // {
        //     // 일부러 리더랑 분리해서 보는게 편해서 이렇게 했음
        //     if (IsLeader)
        //         return;

        //     if (CreatureAIState == ECreatureAIState.Dead)
        //         return;

        //     // switch (joystickState)
        //     // {
        //     //     case EJoystickState.PointerUp:
        //     //         {
        //     //             if (CreatureMoveState != ECreatureMoveState.MoveToTarget)
        //     //                 CreatureMoveState = ECreatureMoveState.None;
        //     //         }
        //     //         break;

        //     //     case EJoystickState.Drag:
        //     //         if (Managers.Object.HeroLeaderController.HeroMemberChaseMode == EHeroMemberChaseMode.EngageTarget)
        //     //         {
        //     //             if (Target.IsValid() && _isFarFromLeader == false)
        //     //             {
        //     //                 CreatureMoveState = ECreatureMoveState.MoveToTarget;
        //     //                 return;
        //     //             }
        //     //         }

        //     //         if (IsForceStopMode() == false)
        //     //         {
        //     //             CreatureMoveState = ECreatureMoveState.ForceMove;
        //     //             Target = null;
        //     //         }
        //     //         break;

        //     //     default:
        //     //         break;
        //     // }
        // }

            public void StartSearchTarget(System.Func<bool> allTargetsCondition = null)
        {
            StartCoSearchTarget<BaseObject>(scanRange: ReadOnly.Numeric.HeroDefaultScanRange,
                                        firstTargets: Managers.Object.Monsters,
                                        secondTargets: Managers.Object.Envs,
                                        func: IsValid,
                                        allTargetsCondition: allTargetsCondition);
        }

        public void StopSearchTarget()
            => StopCoSearchTarget();
    #region AI - Prev
        // protected override void UpdateIdle()
        // {
        //     if (IsLeader)
        //         return;

        //     if (MoveAfterLeader2())
        //     {
        //         CreatureAIState = ECreatureAIState.Move;
        //         return;
        //     }

        //     // Prev Code
        //     // --- Follow Leader
        //     // if (CanForceMoveInFollowLeaderMode() && IsForceStopMode() == false)
        //     // {
        //     //     MoveFromLeader();
        //     //     return;
        //     // }

        //     // LookAtValidTarget();
        //     // if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
        //     //     return;

        //     // --- 타겟이 존재하거나, 타겟이 죽었을 경우, Move 상태로 전환
        //     // if (Target.IsValid() || CreatureMoveState == ECreatureMoveState.MoveToTarget && IsForceStopMode() == false)
        //     // {
        //     //     CreatureAIState = ECreatureAIState.Move;
        //     //     CreatureUpperAnimState = ECreatureUpperAnimState.UA_Move;
        //     //     CreatureLowerAnimState = ECreatureLowerAnimState.LA_Move;
        //     //     return;
        //     // }

        //     // if (IsForceStopMode() == false)
        //     // {
        //     //     MoveFromLeader();
        //     //     return;
        //     // }
        // }

        private bool _canHandleSkill = false;

        // --- UpdateMove
        // >> Upper - Move, Skill_Attack, Skill_A, Skill_B
        // >> Lower - Move
        // protected override void UpdateMove()
        // {
        //     if (IsLeader)
        //         return;

        //     EFindPathResult result = FindPathAndMoveToCellPos(destPos: ChaseCellPos,
        //         maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);

        //     if (result == EFindPathResult.Fail_ForceMovePingPongObject) // 확인됨
        //         return;

        //     if (CreatureMoveState == ECreatureMoveState.None)
        //     {
        //         if (result == EFindPathResult.Fail_NoPath)
        //         {
        //             PauseSearchTarget = false;
        //             CreatureAIState = ECreatureAIState.Idle;
        //             return;
        //         }
        //     }
        //     // else if (CreatureMoveState == ECreatureMoveState.MoveToTarget)
        //     // {
        //     //     _canHandleSkill = false;
        //     //     List<Vector3Int> path = Managers.Map.FindPath(CellPos, ChaseCellPos, 2);
        //     //     if (path.Count > 0)
        //     //     {
        //     //         for (int i = 0; i < path.Count; ++i)
        //     //         {
        //     //             // 일단 무조건 Cell의 중앙에 와야함. (스키타는 움직임 방지)
        //     //             Vector3 centeredPathPos = Managers.Map.CenteredCellToWorld(path[i]);
        //     //             if ((transform.position - centeredPathPos).sqrMagnitude > 0.01f)
        //     //                 continue;

        //     //             // Cell 중앙에 위치 후, 공격 범위에 들어오면 공격 시작.
        //     //             if (IsInAttackRange())
        //     //             {
        //     //                 _canHandleSkill = true;
        //     //                 return;
        //     //             }
        //     //         }

        //     //         // 겹쳐서 계속 MoveState로 허공질하고 있을 때
        //     //         // --- 여기 고장남
        //     //         // if (CellPos == Managers.Object.HeroLeaderController.Leader.CellPos)
        //     //         // {
        //     //         //     Debug.Log($"<color=magenta>!!! NID FIX !!!{gameObject.name}</color>");
        //     //         //     CreatureMoveState = ECreatureMoveState.None;
        //     //         //     CreatureState = ECreatureState.Idle;
        //     //         //     return;
        //     //         // }

        //     //         // 와리가리 핑퐁 오브젝트 체크
        //     //         Vector3 centeredLastPathPos = Managers.Map.CenteredCellToWorld(path[path.Count - 1]);
        //     //         if (Target.IsValid() && (transform.position - centeredLastPathPos).sqrMagnitude < 0.01f)
        //     //         {
        //     //             if (IsPingPongAndCantMoveToDest(CellPos))
        //     //             {
        //     //                 if (_currentPingPongCantMoveCount >= ReadOnly.Numeric.MaxCanPingPongConditionCount && IsForceMovingPingPongObject == false)
        //     //                 {
        //     //                     Debug.Log($"<color=cyan>[!]{gameObject.name}, Start force moving for PingPong Object.</color>");
        //     //                     CoStartForceMovePingPongObject(CellPos, ChaseCellPos, endCallback: delegate ()
        //     //                     {
        //     //                         _currentPingPongCantMoveCount = 0;
        //     //                         CoStopForceMovePingPongObject();
        //     //                         CreatureMoveState = ECreatureMoveState.None;
        //     //                         CreatureAIState = ECreatureAIState.Idle;
        //     //                         CreatureUpperAnimState = ECreatureUpperAnimState.UA_Idle;
        //     //                         CreatureLowerAnimState = ECreatureLowerAnimState.LA_Idle;
        //     //                     });
        //     //                 }
        //     //                  else if (IsForceMovingPingPongObject == false)
        //     //                     ++_currentPingPongCantMoveCount;

        //     //                 return;
        //     //             }
        //     //         }

        //     //         // 타겟이 죽었다면.. return to leader
        //     //         if (Target.IsValid() == false && CreatureMoveState == ECreatureMoveState.MoveToTarget)
        //     //         {
        //     //             if ((transform.position - centeredLastPathPos).sqrMagnitude < 0.01f)
        //     //             {
        //     //                 CreatureMoveState = ECreatureMoveState.None;
        //     //                 CreatureAIState = ECreatureAIState.Idle;
        //     //                 CreatureUpperAnimState = ECreatureUpperAnimState.UA_Idle;
        //     //                 CreatureLowerAnimState = ECreatureLowerAnimState.LA_Idle;
        //     //                 Debug.Log($"<color=cyan>{gameObject.name}, return to leader</color>");
        //     //                 return;
        //     //             }
        //     //         }
        //     //     }
        //     // }
        // }

        // protected override void UpdateSkill()
        // {
        //     if (IsLeader)
        //         return;

        //     if (CanForceMoveInFollowLeaderMode())
        //         MoveFromLeader();
        // }

        // protected override void UpdateCollectEnv()
        // {
        //     if (IsLeader)
        //         return;

        //     LookAtValidTarget();
        //     // Retarget Monster or Env is dead
        //     if ((Target.IsValid() && Target.ObjectType == EObjectType.Monster) || Target.IsValid() == false)
        //     {
        //         // CollectEnv = false;
        //         CreatureAIState = ECreatureAIState.Move;
        //     }

        //     if (CanForceMoveInFollowLeaderMode())
        //         MoveFromLeader();
        // }

        protected override void UpdateDead()
        {
            base.UpdateDead();
        }
        #endregion
      // private void MoveAfterLeader()
        // {
        //     if (_isFarFromLeader)
        //     {
        //         PauseSearchTarget = true;
        //         return;
        //     }

        //     if (ForceMove)
        //     {
        //         Hero leader = Managers.Object.HeroLeaderController.Leader;
        //         if ((transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
        //             return;

        //         List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
        //         if (idlePathFind.Count > 1)
        //         {
        //             if (Managers.Map.CanMove(idlePathFind[1]))
        //             {
        //                 CreatureAIState = ECreatureAIState.Move;
        //                 return;
        //             }
        //         }
        //         return;
        //     }
        // }

        private void MoveFromLeader()
        {
            if (_isFarFromLeader)
            {
                PauseSearchTarget = true;
                //CreatureAIState = ECreatureAIState.Move;
                return;
            }

            // if (CreatureMoveState == ECreatureMoveState.ForceMove || CreatureMoveState == ECreatureMoveState.None)
            // {
            //     Hero leader = Managers.Object.HeroLeaderController.Leader;
            //     // --- 리더가 이동한다고해서 바로 움직이지 않는다.
            //     if ((transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
            //         return;

            //     List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
            //     if (idlePathFind.Count > 1)
            //     {
            //         if (Managers.Map.CanMove(idlePathFind[1]))
            //         {
            //             //CreatureAIState = ECreatureAIState.Move;
            //             return;
            //         }
            //     }
            //     return;
            // }
        }

    // -- 가끔 간헐적으로 멍때림을 방지하는 코루틴(임시)
        private Coroutine _coCheckSpaceOut = null;
        private IEnumerator CoCheckSpaceOut()
        {
            bool spaceOut = false;
            while (true)
            {
                if (IsLeader)
                {
                    yield return null;
                    continue;
                }

                // 아무것도 안하고 있을 때.. 타겟이 있는데 가만히 멍대리고 있는다..
                // 아까 Idle 상태였는데 
                // if (CreatureAIState == ECreatureAIState.Idle && Target.IsValid())
                //     spaceOut = true;

                // --- 검토 필요. 전체적인 AI 검토 필요 ---
                if (spaceOut)
                {
                    Debug.Log($"<color=yellow>ZoneOut: {gameObject.name}</color>");
                    spaceOut = false;
                    Target = null;
                    CreatureAIState = ECreatureAIState.Idle;
                }

                yield return new WaitForSeconds(3f);
            }
        }

        private void StartCoCheckSpaceOut()
        {
            if (_coCheckSpaceOut != null)
                return;

            _coCheckSpaceOut = StartCoroutine(CoCheckSpaceOut());
        }

        private void StopCoCheckSpaceOut()
        {
            if (_coCheckSpaceOut != null)
            {
                StopCoroutine(_coCheckSpaceOut);
                _coCheckSpaceOut = null;
            }
        }

    // if (_isFarFromLeader)
                // {
                //     _pauseSearchTarget = true;
                //     CreatureState = ECreatureState.Move;
                //     return;
                // }

                // if (CreatureMoveState == ECreatureMoveState.ForceMove)
                // {
                //     Hero leader = Managers.Object.HeroLeaderController.Leader;
                //     // --- 리더가 이동한다고해서 바로 움직이지 않는다.
                //     if ((transform.position - leader.transform.position).sqrMagnitude < _waitMovementDistanceSQRFromLeader)
                //         return;

                //     List<Vector3Int> idlePathFind = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: ChaseCellPos, maxDepth: 2);
                //     if (idlePathFind.Count > 1)
                //     {
                //         if (Managers.Map.CanMove(idlePathFind[1]))
                //         {
                //             CreatureState = ECreatureState.Move;
                //             return;
                //         }
                //     }
                //     return;
                // }

                // if (IsCorrectHeroMemberChaseMode(EHeroMemberChaseMode.FollowLeader) && CreatureMoveState == ECreatureMoveState.ForceMove)
            // {
            //     CreatureState = ECreatureState.Move;
            //     return;
            // }

    // if (Target.IsValid())
            // {

            //     // if (Target.ObjectType == EObjectType.Monster)
            //     // {
            //     //     CreatureMoveState = ECreatureMoveState.TargetToEnemy;
            //     //     return;
            //     // }

            //     // if (Target.ObjectType == EObjectType.Env)
            //     // {
            //     //     CreatureMoveState = ECreatureMoveState.CollectEnv; // 필요없을지도?
            //     //     CreatureState = ECreatureState.Move;
            //     //     return;
            //     // }
            // }      

            // if (Target.IsValid())
            // {
            //     LookAtTarget();

            //     // ... CHECK SKILL COOL TIME ...
            //     if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
            //         return;

            //     // ... 무조건 몬스터부터 ...
            //     if (Target.ObjectType == EObjectType.Monster)
            //     {
            //         CreatureMoveState = ECreatureMoveState.TargetToEnemy;
            //         CreatureState = ECreatureState.Move;
            //         return;
            //     }

            //     // ... ENV TARGET ...
            //     if (Target.ObjectType == EObjectType.Env)
            //     {
            //         CreatureMoveState = ECreatureMoveState.CollectEnv;
            //         CreatureState = ECreatureState.Move;
            //         return;
            //     }
            // }

            // if (Target.IsValid())
            //     LookAtTarget(Target);

            // // 조금 극단적인 방법. 쳐다보면서 가만히 짱박혀있어라.
            // if (CreatureSkill.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
            //     return;

            // Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Monsters, func: IsValid) as Creature;
            // if (creature.IsValid())
            // {
            //     Target = creature;
            //     CreatureState = ECreatureState.Move;
            //     CreatureMoveState = ECreatureMoveState.TargetToEnemy;
            //     return;
            // }

            // Env env = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Envs, func: IsValid) as Env;
            // if (env.IsValid())
            // {
            //     Target = env;
            //     CreatureState = ECreatureState.Move;
            //     CreatureMoveState = ECreatureMoveState.CollectEnv;
            //     return;
            // }

            // if (NeedArrange)
            // {
            //     CreatureState = ECreatureState.Move;
            //     CreatureMoveState = ECreatureMoveState.ReturnToBase;
            //     return;
            // }

        private void MoveByForcePath()
        {
            // 이미 다 소모해서 이동이 끝났으면 종료
            if (_forcePath.Count == 0)
            {
                CreatureMoveState = ECreatureMoveState.None;
                return;
            }

            // 그게 아니면 _forcePath를 하나씩 찾음
            // 2로 넣은 이유, 나, 다음길, 이렇게 2개
            Vector3Int cellPos = _forcePath.Peek();
            if (MoveToCellPos(destCellPos: cellPos, maxDepth: 2))
            {
                _forcePath.Dequeue();
                return;
            }

            Hero hero = Managers.Map.GetObject(cellPos) as Hero;
            if (hero != null && hero.CreatureState == ECreatureState.Idle)
            {
                CreatureMoveState = ECreatureMoveState.None;
                return;
            }
        }

        private Queue<Vector3Int> _forcePath = new Queue<Vector3Int>();
        // ####################### TEMP #######################
        private List<Vector3Int> _pathList = new List<Vector3Int>();
        //  ###################################################
        private bool CheckHeroCampDistanceAndForcePath()
        {
            // Vector3 destPos = CampDestination.position;
            // Vector3Int destCellPos = Managers.Map.WorldToCell(destPos);
            // if ((CellPos - destCellPos).magnitude <= 10f) // 10칸 이상으로 너무 멀어졌을 경우,, (ㄹㅇ 거리로 판정)
            //     return false;

            // if (Managers.Map.CanMove(destCellPos, ignoreObjects: true) == false)
            //     return false;

            // List<Vector3Int> path = Managers.Map.FindPath(startCellPos: CellPos, destCellPos: destCellPos, maxDepth: ReadOnly.Numeric.HeroMaxMoveDepth);
            // if (path.Count < 2)
            //     return false;

            // CreatureMoveState = ECreatureMoveState.ForcePath;

            // _forcePath.Clear();
            // foreach (var p in path)
            // {
            //     _forcePath.Enqueue(p);
            // }
            // _forcePath.Dequeue(); // 시작 위치는 제거한듯?

            return true;
        }

        // Prev Origin
        // protected override void UpdateMove()
        // {
        //     // A* Test
        //     if (CreatureMoveState == ECreatureMoveState.Replace)
        //     {
        //         // 되긴 하는데 장애물 근처에 있을 때 Fail_NoPath 떠가지고 제자리 걸음 하는 녀석도 있긴함.
        //         // 이거 고치고 코드 우아하게 AI 수정. Idle 너무 강제임. 고쳐야함.
        //         // 지금도 대강 되긴하는데 이거 고치고, 체인지 포지션?
        //         // 그리고 HeroLeader가 Camp의 Dest가 되어야함. --> 이것부터 할까?
        //         FindPathAndMoveToCellPos(destPos: _replaceDestPos, ReadOnly.Numeric.HeroMaxMoveDepth);
        //         if (LerpToCellPosCompleted) // A* Test
        //         {
        //             // A* Test
        //             CreatureMoveState = ECreatureMoveState.None; // 이렇게만 처리하고 싶은데 확실하게 Idle로 안가는 녀석도 있음.
        //             CreatureState = ECreatureState.Idle; // 그래서 이것도 추가 (임시)
        //             NeedArrange = false;
        //             return;
        //         }

        //         return;
        //     }

        //     // ***** 일단 생략, 헷갈림 *****
        //     // ForceMove 보다 더 우선순위
        //     if (CreatureMoveState == ECreatureMoveState.ForcePath)
        //     {
        //         // 너무 멀리있으면 무조건 강제이동
        //         MoveByForcePath();
        //         return;
        //     }

        //     if (CheckHeroCampDistanceAndForcePath())
        //         return;

        //     if (CreatureMoveState == ECreatureMoveState.ForceMove)
        //     {
        //         // ### SET DIR ###
        //         _findPathResult = FindPathAndMoveToCellPos(CampDestination.position, ReadOnly.Numeric.HeroDefaultMoveDepth);
        //         return;
        //     }

        //     if (CreatureMoveState == ECreatureMoveState.TargetToEnemy)
        //     {
        //         if (Target.IsValid() == false)
        //         {
        //             // 여기서 None
        //             CreatureMoveState = ECreatureMoveState.None;
        //             CreatureState = ECreatureState.Move;
        //             return;
        //         }

        //         ChaseOrAttackTarget(ReadOnly.Numeric.Temp_ScanRange, AttackDistance);
        //         return;
        //     }

        //     if (CreatureMoveState == ECreatureMoveState.CollectEnv)
        //     {
        //         // Research Enemies
        //         Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_ScanRange, Managers.Object.Monsters, func: IsValid) as Creature;
        //         if (creature != null)
        //         {
        //             Target = creature;
        //             CreatureMoveState = ECreatureMoveState.TargetToEnemy;
        //             CreatureState = ECreatureState.Move;
        //             return;
        //         }

        //         // 이미 채집했으면 포기
        //         if (Target.IsValid() == false)
        //         {
        //             CreatureMoveState = ECreatureMoveState.None;
        //             CreatureState = ECreatureState.Move;
        //             CollectEnv = false;
        //             return;
        //         }

        //         ChaseOrAttackTarget(ReadOnly.Numeric.Temp_ScanRange, AttackDistance);
        //         return;
        //     }

        //     if (CreatureMoveState == ECreatureMoveState.ReturnToBase && IsLeader == false) // ***** TEST *****
        //     {
        //         _findPathResult = FindPathAndMoveToCellPos(CampDestination.position, ReadOnly.Numeric.HeroDefaultMoveDepth);

        //         // 실패사유 검사
        //         // 여기서 뭉치는 알고리즘이 진행됨
        //         BaseObject obj = Managers.Map.GetObject(CampDestination.position);
        //         if (obj.IsValid())
        //         {
        //             // 내가 그 자리를 차지하고 있음.
        //             if (obj == this)
        //             {
        //                 Debug.Log($"<color=magenta>findPathResult: {_findPathResult}</color>");
        //                 CreatureMoveState = ECreatureMoveState.None;
        //                 NeedArrange = false;
        //                 return;
        //             }

        //             // 다른 영웅이 그 자리를 차지하고 있음.
        //             Hero hero = obj as Hero;
        //             if (hero != null && hero.CreatureState == ECreatureState.Idle)
        //             {
        //                 Debug.Log($"<color=orange>findPathResult: {_findPathResult}</color>");
        //                 CreatureMoveState = ECreatureMoveState.None;
        //                 NeedArrange = false;
        //                 return;
        //             }
        //         }
        //     }

        //     if (LerpToCellPosCompleted)
        //     {
        //         CreatureState = ECreatureState.Idle;
        //         // 여기서는 누르지 않은 상태이니까 None
        //         if (CreatureMoveState == ECreatureMoveState.ReturnToBase && _findPathResult != EFindPathResult.Success)
        //         {
        //             // 이거 조건걸어서 단체로 움직일때 누구는 캠프 근처로가고 누구는 안가고 이러는건가.
        //             NeedArrange = false; // 이것만 해도 임시적으로 되긴함
        //             CreatureMoveState = ECreatureMoveState.None;
        //             // Lerp하다가 중간에 누가 차지하면 계속 걸어가려고하는데 이거 막아야할듯
        //         }
        //         // NeedArrange = false; // --> 이거 주면 해결 되긴하는데, 캠프까지 쫓아가질 않음
        //     }
        // }

        // #################### PREV ####################
        // --- DO NOT SKI MOVEMENT
        // Vector3 centeredPos = Managers.Map.CenteredCellToWorld(path[path.Count - 1]);
        // if ((transform.position - centeredPos).sqrMagnitude < 0.01f)
        // {
        //     // 타겟을 잡고(타겟이 히어로에게 죽고) 돌아오는 길이었다면..
        //     if (Target.IsValid() == false && CreatureMoveState == ECreatureMoveState.TargetToEnemy)
        //     {
        //         CreatureMoveState = ECreatureMoveState.None;
        //         CreatureState = ECreatureState.Idle;
        //         return;
        //     }

        //     if (IsPingPongAndCantMoveToDest(CellPos))
        //     {
        //         if (_currentPingPongCantMoveCount >= ReadOnly.Numeric.MaxCanPingPongConditionCount && IsForceMovingPingPongObject == false)
        //         {
        //             CoStartForceMovePingPongObject(CellPos, ChaseCellPos, delegate ()
        //             {
        //                 _currentPingPongCantMoveCount = 0;
        //                 CoStopForceMovePingPongObject();
        //                 CreatureMoveState = ECreatureMoveState.None;
        //                 CreatureState = ECreatureState.Idle;
        //             });
        //         }
        //         else if (IsForceMovingPingPongObject == false)
        //         {
        //             Debug.Log($"<color=white>{gameObject.name}, PingPong Count: {++_currentPingPongCantMoveCount}</color>");
        //         }
        //     }

        //     _canHandleSkill = true;
        // }

         // protected override IEnumerator CoDeadFadeOut(System.Action callback = null)
        // {
        //     if (this.isActiveAndEnabled == false)
        //         yield break;

        //     yield return new WaitForSeconds(ReadOnly.Util.StartDeadFadeOutTime);

        //     float delta = 0f;
        //     float percent = 1f;
        //     AnimationCurve curve = Managers.Contents.Curve(EAnimationCurveType.Ease_In);
        //     // --- 1. Fade Out - Skin
        //     while (percent > 0f)
        //     {
        //         delta += Time.deltaTime;
        //         percent = 1f - (delta / ReadOnly.Util.DesiredDeadFadeOutEndTime);
        //         for (int i = 0; i < HeroBody.Skin.Count; ++i)
        //         {
        //             float current = Mathf.Lerp(0f, 1f, curve.Evaluate(percent));
        //             HeroBody.Skin[i].color = new Color(HeroBody.Skin[i].color.r,
        //                                                HeroBody.Skin[i].color.g,
        //                                                HeroBody.Skin[i].color.b, current);
        //         }

        //         yield return null;
        //     }

        //     // --- 2. Fade Out - Appearance
        //     delta = 0f;
        //     percent = 1f;
        //     while (percent > 0f)
        //     {
        //         delta += Time.deltaTime;
        //         percent = 1f - (delta / ReadOnly.Util.DesiredDeadFadeOutEndTime);
        //         for (int i = 0; i < HeroBody.Appearance.Count; ++i)
        //         {
        //             float current = Mathf.Lerp(0f, 1f, curve.Evaluate(percent));
        //             HeroBody.Appearance[i].color = new Color(HeroBody.Appearance[i].color.r,
        //                                                      HeroBody.Appearance[i].color.g,
        //                                                      HeroBody.Appearance[i].color.b, current);
        //         }

        //         yield return null;
        //     }

        //     callback?.Invoke();
        // }
*/

/*
        [ PREV REF: CREATURE ]
        public virtual bool IsMoving
        {
            get => CreatureAnim.IsMoving;
            set => CreatureAnim.IsMoving = value;
        }

        public bool IsValidTargetInAttackRange()
        {
            if (Target.IsValid() == false)
            {
                CreatureAnim.IsInAttackRange = false;
                return false;
            }

            int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
            int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);

            if (Target.ObjectType == EObjectType.Monster || Target.ObjectType == EObjectType.Hero)
            {
                if (dx <= AtkRange && dy <= AtkRange)
                {
                    CreatureAnim.IsInAttackRange = true;
                    return true;
                }
            }
            else if (Target.ObjectType == EObjectType.Env)
            {
                if (dx <= 1 && dy <= 1)
                    return true;
            }

            CreatureAnim.IsInAttackRange = false;
            return false;
        }

        private T SearchClosestInRange<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null,
                                        System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            T firstTarget = null;
            T secondTarget = null;
            float bestDistSQR = float.MaxValue;
            float scanRangeSQR = scanRange * scanRange;

            // --- Set Hero Leader Scan Range (half)
            // --- 없어도 될 것 같은데
            Hero hero = this as Hero;
            if (hero.IsValid() && hero.IsLeader)
                scanRangeSQR *= 0.8f;

            foreach (T obj in firstTargets)
            {
                Vector3Int dir = obj.CellPos - CellPos;
                float distToTargetSQR = dir.sqrMagnitude;
                if (scanRangeSQR < distToTargetSQR)
                    continue;

                if (bestDistSQR < distToTargetSQR)
                    continue;

                if (func?.Invoke(obj) == false)
                    continue;

                bestDistSQR = distToTargetSQR;
                firstTarget = obj;
            }

            // --- 일반적인 Searching 또는 AutoTarget이 켜져있을 때
            if (allTargetsCondition == null || allTargetsCondition.Invoke() == false)
            {
                if (firstTarget != null)
                    return firstTarget;
                else if (firstTarget == null && secondTargets != null)
                {
                    foreach (T obj in secondTargets)
                    {
                        Vector3Int dir = obj.CellPos - CellPos;
                        float distToTargetSQR = dir.sqrMagnitude;
                        if (scanRangeSQR < distToTargetSQR)
                            continue;

                        if (bestDistSQR < distToTargetSQR)
                            continue;

                        if (func?.Invoke(obj) == false)
                            continue;

                        bestDistSQR = distToTargetSQR;
                        secondTarget = obj;
                    }
                }

                return secondTarget;
            }
            // --- AutoTarget과 상관 없이 리더이고 ForceMove가 True일 때.
            else if (allTargetsCondition != null && allTargetsCondition.Invoke())
            {
                foreach (T obj in secondTargets)
                {
                    Vector3Int dir = obj.CellPos - CellPos;
                    float distToTargetSQR = dir.sqrMagnitude;
                    if (scanRangeSQR < distToTargetSQR)
                        continue;

                    if (bestDistSQR < distToTargetSQR)
                        continue;

                    if (func?.Invoke(obj) == false)
                        continue;

                    bestDistSQR = distToTargetSQR;
                    secondTarget = obj;
                }

                if (func?.Invoke(firstTarget) == false)
                    return secondTarget;
                else if (func?.Invoke(secondTarget) == false)
                    return firstTarget;
                else
                {
                    float fDistSQR = (firstTarget.CellPos - CellPos).sqrMagnitude;
                    float sDistSQR = (secondTarget.CellPos - CellPos).sqrMagnitude;
                    if (fDistSQR < sDistSQR)
                        return firstTarget;
                    else
                        return secondTarget;
                }

                // // 두 개 모두 있는지부터 가정
                // if (firstTarget != null && secondTarget != null)
                // {
                //     float fDistSQR = (firstTarget.CellPos - CellPos).sqrMagnitude;
                //     float sDistSQR = (secondTarget.CellPos - CellPos).sqrMagnitude;
                //     if (fDistSQR < sDistSQR)
                //         return firstTarget;

                //     return secondTarget;
                // }
                // else if (firstTarget != null)
                //     return firstTarget;
                // else
                //     return secondTarget; // null이 나올수도 있음
            }

            return null;
        }

        // --- Creature -> BaseObject
        //public bool PauseSearchTarget { get; protected set; } = false;

        private Coroutine _coSearchTarget = null;
        private IEnumerator CoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            float tick = ReadOnly.Numeric.SearchFindTargetTick;
            while (true)
            {
                if (this.IsValid() == false)
                    yield break;

                yield return new WaitForSeconds(tick);
                if (PauseSearchTarget)
                {
                    Target = null; // --- DEFENSE
                    yield return null;
                    continue;
                }

                // if (_coWaitSearchTarget != null) // 이것도 필요할지...
                // {
                //     yield return null;
                //     continue;
                // }
                Target = SearchClosestInRange(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition);

                // if (Target.IsValid() == false)
                // {
                //     Debug.Log("<color=red>!@#!@#!@#!@#!@#!@#!@#</color>");
                // }

                // CreatureAnim.IsTargetValid = Target != null ? true : false;
                // if (Target.IsValid())
                //     CreatureMoveState = ECreatureMoveState.MoveToTarget;
                // --- Target이 존재하지 않을 때, MoveToTarget 해제는 Creature AI에서 해결    
            }
        }

        protected void StartCoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            // if (_coSearchTarget != null)
            //     return;
            StopCoSearchTarget();
            _coSearchTarget = StartCoroutine(CoSearchTarget<T>(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition));
        }

        protected void StopCoSearchTarget()
        {
            Target = null;
            if (_coSearchTarget != null)
            {
                StopCoroutine(_coSearchTarget);
                _coSearchTarget = null;
            }
        }private T SearchClosestInRange<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null,
                                        System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            T firstTarget = null;
            T secondTarget = null;
            float bestDistSQR = float.MaxValue;
            float scanRangeSQR = scanRange * scanRange;

            // --- Set Hero Leader Scan Range (half)
            // --- 없어도 될 것 같은데
            Hero hero = this as Hero;
            if (hero.IsValid() && hero.IsLeader)
                scanRangeSQR *= 0.8f;

            foreach (T obj in firstTargets)
            {
                Vector3Int dir = obj.CellPos - CellPos;
                float distToTargetSQR = dir.sqrMagnitude;
                if (scanRangeSQR < distToTargetSQR)
                    continue;

                if (bestDistSQR < distToTargetSQR)
                    continue;

                if (func?.Invoke(obj) == false)
                    continue;

                bestDistSQR = distToTargetSQR;
                firstTarget = obj;
            }

            // --- 일반적인 Searching 또는 AutoTarget이 켜져있을 때
            if (allTargetsCondition == null || allTargetsCondition.Invoke() == false)
            {
                if (firstTarget != null)
                    return firstTarget;
                else if (firstTarget == null && secondTargets != null)
                {
                    foreach (T obj in secondTargets)
                    {
                        Vector3Int dir = obj.CellPos - CellPos;
                        float distToTargetSQR = dir.sqrMagnitude;
                        if (scanRangeSQR < distToTargetSQR)
                            continue;

                        if (bestDistSQR < distToTargetSQR)
                            continue;

                        if (func?.Invoke(obj) == false)
                            continue;

                        bestDistSQR = distToTargetSQR;
                        secondTarget = obj;
                    }
                }

                return secondTarget;
            }
            // --- AutoTarget과 상관 없이 리더이고 ForceMove가 True일 때.
            else if (allTargetsCondition != null && allTargetsCondition.Invoke())
            {
                foreach (T obj in secondTargets)
                {
                    Vector3Int dir = obj.CellPos - CellPos;
                    float distToTargetSQR = dir.sqrMagnitude;
                    if (scanRangeSQR < distToTargetSQR)
                        continue;

                    if (bestDistSQR < distToTargetSQR)
                        continue;

                    if (func?.Invoke(obj) == false)
                        continue;

                    bestDistSQR = distToTargetSQR;
                    secondTarget = obj;
                }

                if (func?.Invoke(firstTarget) == false)
                    return secondTarget;
                else if (func?.Invoke(secondTarget) == false)
                    return firstTarget;
                else
                {
                    float fDistSQR = (firstTarget.CellPos - CellPos).sqrMagnitude;
                    float sDistSQR = (secondTarget.CellPos - CellPos).sqrMagnitude;
                    if (fDistSQR < sDistSQR)
                        return firstTarget;
                    else
                        return secondTarget;
                }

                // // 두 개 모두 있는지부터 가정
                // if (firstTarget != null && secondTarget != null)
                // {
                //     float fDistSQR = (firstTarget.CellPos - CellPos).sqrMagnitude;
                //     float sDistSQR = (secondTarget.CellPos - CellPos).sqrMagnitude;
                //     if (fDistSQR < sDistSQR)
                //         return firstTarget;

                //     return secondTarget;
                // }
                // else if (firstTarget != null)
                //     return firstTarget;
                // else
                //     return secondTarget; // null이 나올수도 있음
            }

            return null;
        }

        // --- Creature -> BaseObject
        //public bool PauseSearchTarget { get; protected set; } = false;

        private Coroutine _coSearchTarget = null;
        private IEnumerator CoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            float tick = ReadOnly.Numeric.SearchFindTargetTick;
            while (true)
            {
                if (this.IsValid() == false)
                    yield break;

                yield return new WaitForSeconds(tick);
                if (PauseSearchTarget)
                {
                    Target = null; // --- DEFENSE
                    yield return null;
                    continue;
                }

                // if (_coWaitSearchTarget != null) // 이것도 필요할지...
                // {
                //     yield return null;
                //     continue;
                // }
                Target = SearchClosestInRange(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition);

                // if (Target.IsValid() == false)
                // {
                //     Debug.Log("<color=red>!@#!@#!@#!@#!@#!@#!@#</color>");
                // }

                // CreatureAnim.IsTargetValid = Target != null ? true : false;
                // if (Target.IsValid())
                //     CreatureMoveState = ECreatureMoveState.MoveToTarget;
                // --- Target이 존재하지 않을 때, MoveToTarget 해제는 Creature AI에서 해결    
            }
        }

        protected void StartCoSearchTarget<T>(float scanRange, IEnumerable<T> firstTargets, IEnumerable<T> secondTargets = null, System.Func<T, bool> func = null, System.Func<bool> allTargetsCondition = null) where T : BaseObject
        {
            // if (_coSearchTarget != null)
            //     return;
            StopCoSearchTarget();
            _coSearchTarget = StartCoroutine(CoSearchTarget<T>(scanRange, firstTargets: firstTargets, secondTargets: secondTargets, func: func, allTargetsCondition: allTargetsCondition));
        }

        protected void StopCoSearchTarget()
        {
            Target = null;
            if (_coSearchTarget != null)
            {
                StopCoroutine(_coSearchTarget);
                _coSearchTarget = null;
            }
        }

          // --- WILL DEPRECIATE
        protected Coroutine _coWaitSearchTarget = null;
        private IEnumerator CoWaitSearchTarget(float waitSeconds)
        {
            // --- 확인됨
            //Debug.Log($"<color=white>{nameof(CoWaitSearchTarget)}</color>");
            yield return new WaitForSeconds(waitSeconds);
            StopCoWaitSearchTarget();
        }

        protected void StartCoWaitSearchTarget(float waitSeconds)
        {
            if (_coWaitSearchTarget != null)
                return;

            _coWaitSearchTarget = StartCoroutine(CoWaitSearchTarget(waitSeconds));
        }

        protected void StopCoWaitSearchTarget() // PRIVATE
        {
            if (_coWaitSearchTarget != null)
            {
                StopCoroutine(_coWaitSearchTarget);
                _coWaitSearchTarget = null;
            }

            // --- 확인됨
            // Debug.Log($"<color=white>{nameof(StopCoWaitSearchTarget)}</color>");
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

        // protected void ChaseOrAttackTarget_Prev_Temp(float chaseRange, float atkRange)
        // {
        //     //Vector3 toTargetDir = Target.transform.position - transform.position;
        //     if (DistanceToTargetSQR <= atkRange * atkRange)
        //     {
        //         if (Target.IsValid() && Target.ObjectType == EObjectType.Env)
        //             CreatureState = ECreatureState.CollectEnv;
        //         else
        //             CreatureSkill?.CurrentSkill.DoSkill();
        //     }
        //     else
        //     {
        //         _findPathResult = FindPathAndMoveToCellPos(Target.transform.position, ReadOnly.Numeric.HeroMaxMoveDepth);
        //         float searchDistSQR = chaseRange * chaseRange;
        //         if (DistanceToTargetSQR > searchDistSQR)
        //         {
        //             Target = null;
        //             CreatureState = ECreatureState.Move;
        //         }
        //     }
        // }

                // 제거예정
        public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
        {
            if (LerpToCellPosCompleted == false)
                return false;

            return Managers.Map.MoveTo(this, cellPos: destCellPos);
        }

        protected BaseObject FindClosestInRange(float scaneRange, IEnumerable<BaseObject> objs, System.Func<BaseObject, bool> func = null)
        {
            BaseObject target = null;
            float bestDistSQR = float.MaxValue;
            float scanRangeSQR = scaneRange * scaneRange;

            foreach (BaseObject obj in objs)
            {
                //Vector3 dir = obj.transform.position - transform.position;
                // 5칸인지 테스트
                Vector3Int dir = obj.CellPos - CellPos;
                float distToTargetSQR = dir.sqrMagnitude;

                if (scanRangeSQR < distToTargetSQR)
                    continue;

                if (bestDistSQR < distToTargetSQR)
                    continue;

                // 추가 조건 (ex)객체가 살아있는가? 람다 기반으로 넘겨줄수도 있으니까 뭐, 편함 이런게 있으면.
                // 살아있는 객체만 찾아서, 살아있는 객체에게만 Target을 잡을 수 있도록. 이런식으로 가능.
                if (func?.Invoke(obj) == false)
                    continue;

                bestDistSQR = distToTargetSQR;
                target = obj;
            }

            return target;
        }

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
*/

/*
    [Prev Ref: Monster]
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