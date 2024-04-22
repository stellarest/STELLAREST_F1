using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;

/*
[ TODO LIST ]
- Wizard Attack Anim 교정
- Monster RigidBody Linear Drag (100 -> 300) : 나중에 종류별로 바꿔야함.
- 그리고 예를 들어 바위를 채집할 때, 바위 완전 밑에서 채집하면 이상하니까, 이런거 위치 조정해줘야 할 것 같은데.
- Melee Attack 구현, Init Stat 구현. Exp는 아직 X

Skill First
- Skill_Attack : Paladin MeleeAttack
- Skill_A : Double Attack
- Skill_B : Shield

Projectile 제작
EnterInGame 정리

두 번째 아쳐에서 오른손 무기 SPR이 꺼져있음.
애니메이션으로 켜주면 되긴함. 근데 일단 원인부터.
*/

public class T_E_S_T : MonoBehaviour
{
    private float _movementSpeed = 3.0f;
    private Vector2 _moveDir = Vector3.zero;
    private int TestProperty { get; set; }

    private IEnumerator Start()
    {
        Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
        Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;
        yield return null;
        // yield return new WaitForSeconds(3f);
        // Debug.Log("MOVE START !!");
        // while (true)
        // {
        //     // 거 = 속 * 시
        //     float distance = _movementSpeed * Time.deltaTime;
        //     while (true)
        //     {
        //         transform.position += Vector3.up * distance;
        //         //transform.position = transform.position + (Vector3.up * _movementSpeed * Time.deltaTime);
        //         yield return null;
        //     }
        // }
    }

    private void Update()
    {
        if (Managers.Game.JoystickState == EJoystickState.Drag)
        {
            float moveDistancePerFrame = _movementSpeed * Time.deltaTime;
            transform.Translate(_moveDir * moveDistancePerFrame);
        }
    }

    private void OnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
    }
}

/* 
[ MEMO ]
가칭 : 매우 간단한 RPG 게임. (Only One류, FQ4와 비슷한)
정처없이 돌아다니면서 사냥 + 육성 + 대략 100레벨 정도 만들고 클리어하면 끝.
게임 가격은 1100원 또는 2200원. 빠르게 개발하고 출시하는 것이 목적
-- 포폴 : 웹
-- 출시버전 : 싱글
--- 레스트 서버란 무엇인가?

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
1. Paladin
2. Archer
3. Wizard
4. Warrior
5. Assassin
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
