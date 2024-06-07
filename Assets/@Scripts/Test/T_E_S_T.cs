using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;
using DG.Tweening;

/*
[ TODO TODAY LIST ]
// 24. 06. 07
>>> 데미지 트윈 애니메이션 지속적인 수정 필요(ULTIMATE DMG ASSET 가져오기)
>>> 리더랑 히어로 멤버 같은 위치에 있을 때 아직 고장나있는거 확인 (AI 지속적으로 개선해야함)

// 24. 06. 05
>>> 진도 빼기
>>> Leader 행동 우선 
---> Leader가 Idle 상태에서는 알아서 해라
---> Leader가 공격 또는 채집 중이라면 리더 근처로 이동 시도 후 타겟 검색.

>>> Hero들 Idle 상태에서 멍때리는거 잡는중
---> 얼추 잡힌것같긴한데, 가끔 팔라딘 리더가 원거리 몬스터를 공격한다. 사거리도 안되는 주제에.
>>> 가끔 Search Target을 안하는 느낌
>>> 같은 위치에 리더랑 히어로 멤버가 있으면 멤버가 달리기를 시도함.

[ 찐 히어로 제작 순서 목록 ]
>>> 1. Paladin (Male)
>>> 2. Archer (Female)
>>> 3. Wizard (Female)
>>> 4. Lancer (Male)
>>> 5. Gunner (Male)
>>> 6. Assassin (Male)
>>> 7. Druid (Female)

[ 찐 몬스터 제작 순서 목록, 보스 포함 ]
>>> 1. Chicken
>>> 2. Turkey
>>> 3. Bunny
>>> 4. Pug


[ MEMO LIST ]
>> 목표, Collider, Rigidbody를 빼는 것. 아니면 Collider 하나만 쓰고, OnTrigger등 메서드 사용하지 않기.
>> 강화된 스킬은 내가 예상한대로 무조건 100% 새로 파서 해주는게 좋다고 함.
>> 예를 들어 NextSkillLevelID를 박아서 한다던지
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
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        Sequence seq = DOTween.Sequence();
        // seq.Append(transform.DOMove(new Vector3(3f, 3f, 0), 3f));

        // seq.Join(transform.DOScale(new Vector3(3f, 3f, 0), 3f));
        // seq.Join(GetComponent<SpriteRenderer>().material.DOColor(Color.red, 3f));

        // seq.Append(transform.DORotate(new Vector3(0, 0, -135f), 3f));
        seq.Append(transform.DOScale(endValue: 1.3f, duration: 0.3f)).SetEase(Ease.InOutBounce)
       .Join(transform.DOMove(endValue: transform.position + Vector3.up, duration: 0.3f)).SetEase(Ease.Linear)
       .Append(transform.DOScale(endValue: 1.0f, duration: 0.3f).SetEase(Ease.InOutBounce))
       .Join(transform.GetComponent<SpriteRenderer>().material.DOFade(endValue: 0f, duration: 0.3f)).SetEase(Ease.InQuint)
       .OnComplete(() => Debug.Log("COMPLETED !!"));

        seq.Play();
    }
}

/* 
[ MEMO ]
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