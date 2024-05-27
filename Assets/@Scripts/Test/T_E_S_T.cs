using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;

/*
[ TODO TODAY LIST ]
// 24. 5. 27
>>> 히어로 리더가 움직이기 시작할 떄, 멤버가 이동할 수 있는 칸이 있을때만 Move 애니메이션? - 딱히 중요하진 않지만
>>> Heroes Attack -> Monster (+ Monster Dead)
>>> Monster Default Body Attack

>>> Chicken vs Paladin, Archer 전투 테스트
>>> Env 테스트
>>> Env까지 테스트 완료 후 RigidBody 제거, Camp 제거
>>> 리더가 죽었을 때, 죽는 모션이 나오고 다른 히어로 멤버로 자동으로 설정되도록
>>> CanMove Ignore Object Type 개선사항
>>> 리더 -> 히어로는 Ignore 가능.
>>> 히어로 -> 리더는 Ignore가 안됨. (이거부터 잡아볼까.)

[ MEMO LIST ]
>> --- 출시 계획
>> -- IOS, IPAD, MAC OS
>> 부대1(7명), 부대2(7명), 부대3(7명): 최대 히어로 개수 21개
>> --- 히어로 등급
>> -- Normal -> Ultimate: 진화형 히어로
>> -- Epic: 진화 불가 단일 개체, Normal보다 강하고 Ultimate보다 약함
>> -- Special: 진화 불가 단일 개체, Epic보다 강하고 Ultimate와 비동, 또는 다소 약함
>> 히어로 최소 이동속도는 6부터
>> 캐쥬얼하게 만들기.
>> Parabola 모션 개선 필요 (타겟 거리에 따라 HeightArc 조정)
>> Wizard Attack Anim 완성.
>> 채집물 채집할 떄 위치 파악. 어색하지 않게 채집하는지.
>> 스탯은 나중에
>> 스킬 예시 Character Skills Ex
- Skill_Attack : Paladin MeleeAttack - 일반 공격
- Skill_A : Double Attack - 액티브 스킬1
- Skill_B : Shield - 액티브 스킬2
>> 위아래 꼬불꼬불 위자드 특수스킬 프로젝타일
>>>>> 맵 대충 배치해놓고, 채집물 배치해보고, 몬스터 배치해보고, 기본 전투
>>>>> 히어로 애니메이션 바디에 있는 sorting group 제거함
>>>>> TileMapRenderer Mode를 Individual로 바꾸니까 해결된 느낌
>>>>> 나무쪽 타일맵 랜더러 Sorting Group 추가하지 말고 그냥 Sorting Layer만 BaseObject로
>>>>> Mode를 Individual로 바꾸고, 나무1 스프라이트의 피벗을 0.5, 0.1로 바꾸면 자동정렬 완성
>>>>> AnimationCurveManager -> 제거 예정
*/

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
    private float _movementSpeed = 3.0f;
    private Vector2 _moveDir = Vector3.zero;
    private int TestProperty { get; set; }

    private IEnumerator CoStart()
    {
        yield return null;
    }

    private Coroutine _co = null;

    private IEnumerator Start()
    {
        _co = StartCoroutine(CoStart());

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
곡선은 삼각함수, 베지에 곡선으로도 가능.
GameObject.CreatePrimitive(PrimitiveType.Sphere)
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