using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;

/*
[ TODO TODAY LIST ]
// 24. 06. 04
>>> 이후에 위자드 프로젝타일 테스트 만들고
>>> 치킨 바디어택 만들고 진도 넘어가기
>>> Follow Leader Mode도 좀 이상한게.
아쳐가 공격하고 있으면 계속 공격해야되는데 리더가 움직이면 같이 움직이니까...
근데 이게 맞기도 한데. 아니 근데 공격하고 있으면 공격해야하는게 맞지 않나.
리더가 채집을 하면 똑같이 채집.

// 24. 06. 03
>>> 아쳐가 사냥중일 때 몬스터 앞으로 이동하려고하는거 (Engage, Follow 모드에서) 확인 필요하고
>>> 아쳐 무기 없어짐. 개판임 그냥. (CollectEnv, DoSkill과 엇갈려서 그런 것 같음, ---> 이것부터 시급. Collect Env가 체크되어 있었다)
>>> 팔라딘 리더 가끔씩 사정거리 밖인데, 몬스터를 공격할 수 있는 것 같다.
>>> HeroMemberBattleMode에서 리더의 행동만 따라하는 (수집, 공격, 아이들) 모드만 필요할 것 같다.
---> 아니면 이것보다, Follow Leader모드 일 때, 리더의 행동만 따라하는 것으로 하면 될 것 같다.
>>> 코루틴 WaitForSec이후, 호출 될 수도 있다는 점 디버깅하는거 주의. 이것때문에 시간 많이 잡아먹음.

// 24. 06. 02
-------------------------------------------------------
COLLECT ENV PING PONG
위자드 테스트 프로젝타일 (파이어볼)
치킨 바디 어택
-------------------------------------------------------
>>> 치킨 바디 어택
>>> Normal Mode, Engage Enemy, Follow Leader

// 24. 06. 01
>>> 리더 죽었을 때, 리더 체인지 막아야함.
>>> FORCE SOTP MODE (완료)
>>> 아처 기본 공격(전투), 위자드 기본 공격(전투)부터 해결.
>>> 리더 변경 쿨타임 추가 완료

>>> 다음 챕터로 넘어가기 전에...
>>>>> 닭 바디 어택 확인 (기본으로 바꿀지 고민)
>>>>> 아처 프로젝타일 확인. 아처 전투하나??
>>>>> 위자드 프로젝타일 확인
>>>>> 체력 없을 때 리더 주변으로 도망쳐서 체력회복하는거 만들어야되는데 이건 애니메이션이 있어야할 것 같음 --> 애니메이션, 이펙트 만들어야되서 일단 메모만
>>>>> 통상 모드(기존의 Engage Enemy -> Normal Mode)
--- 이동을 하다가 마우스에서 떼었을 떼, 적을 발견하면 적을 공격한다.
--- 리더와 거리가 멀어지면, 리더를 향해 쫓아간다.

>>>>> Engage Enemy
--- 마우스를 떼지도 않았는데 적을 발견하면 적에게 개돌한다.
--- 적과 죽을때까지 싸운다.

>>>>> Engage Enemy (Force Move 상태에서 적만 발견하면 그냥 개돌)
>>>>> 체력이 없을때 리더 근처로 도망치는 Recovory Mode, Creature Recovory State 필요


// 24. 05. 30
>>> 히어로 멤버 체력 없을 때 Run하는거 추가해야함. 표정 스테이트도 넣어야하나. 고민.
>>> --- 퍼스트퀸4처럼 리더 근처에서 체력회복 애니메이션 넣는게 좋을 것 같음. CreatureState = ECreatureState.Recovery
>>> ForceMove 상태에서도 Engage Enemy 모드라면 Target을 찾게 되었을 때, Force Move 무시
>>> 리더 죽었을 때 Off Pointer, Leader Mark, Change Leader -- 이거부터 하는게 나을듯 (완료)
>>> 닭 잡기 (완료)
>>> 닭 바디 어택
>>> 플레이어 데드 (완료)
>>> 리더 데드 (완료)
>>> 전투 테스트 완료

// 24. 05. 29
>>> 와리가리거리는거 함 잡아보기 (완료)

// 24. 05. 28
>>> Leader A* PathFind Passing Blocked Cell 수정(완료)
>>> Leader Skill Attack 수정 완료
>>> 히어로 멤버 배틀 모드 Engage Enemy 확인해 볼것. FarFromLeader 체크 범위가 너무 가까워서 그런지 바로 붙는것처럼 보임.

// 24. 05. 27
>>> 히어로 리더가 움직이기 시작할 떄, 멤버가 이동할 수 있는 칸이 있을때만 Move 애니메이션? - 딱히 중요하진 않지만 (완료)
>>> Leader Monster Attack
>>> Heroes Attack -> Monster (+ Monster Dead)
>>> Monster Default Body Attack

>>> Leader 기본 공격
>>> Chicken Dead
>>> Chicken Body Attack

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