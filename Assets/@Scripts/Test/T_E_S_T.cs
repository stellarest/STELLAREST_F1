using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;
using DG.Tweening;

/*
// --- 제거 예정
// public List<SpriteRenderer> Skin { get; } = new List<SpriteRenderer>();
// public List<SpriteRenderer> Appearance { get; } = new List<SpriteRenderer>();
// --- UI를 정복했을 때 자신감이 엄청나게 상승한다고 함. 그냥 참고.
// --- ((Env Tree, Rock)), 내일 --> Change Sprite Set, Fade Out
// --- + ProjectileBodyBase ?? 추가 ?? SkillBodyBase ??
// --- HeroStateWeaponType, EnvWeapon 다시 잡아야함.
// --- EnvTree Fruits Pos, Rot, Scale 생성 될때마다 Min, Max사이로 배치되도록 수정해야할듯.
// --- this.DefaultMat = defaultMat; // ---> Name 변경 필요: DefaultSPRMat
// --- OreLight SPR Alpha가 1로 되어있음... (고침, BaseObject ShowBody 부분 고쳐야함)
// --- 골드 OreLight가 피격되면 이상하게 어두워짐.
// --- BodyContainer 생성자에서 new MatPropertyBlock 받도록하게 변경해야할듯(안쓰는 것은 굳이 만들 필요 없을것같아서)
// --- 이거 때문에 Gold Glow가 이상하게 적용 되는 것이었음.
// --- 제거 얘정

// --- SpriteManager에서 EnvBody SPR.color 따로 셋팅해야될지 고민중. 안해도 될 것 같긴 한데.
// --- 동일한 이유로, 몬스터도 SkinColor를 제외하고는 SPR.color를 안줘도 될 것 같음. 어차피 하얀색이 디폴트라.
// --- 어쨋든 Env부분 Color 세팅하는거 안해도 될 것 같고, 몬스터도 SkinColor 안해도 될듯.
// --- MonsterBody도 foreach 루프로 돌려서 하면 될 것 같고. 이후 EnvRockBody 제작할것.
// --- MonsterBody부터 테스트. 아니면 몬스터 SkinColor 없애도 상관없을 것 같긴 함. 디폴트는 모두 하얀색으로.
// --- BodyContainer의 DefaultColor는 모두 있어야함. 단, 몬스터, Env의 SPR default color는 white로.

// --- Arhcer Hero Dead에서 Fade할 때, 다리부터 없어짐. 고쳐야함. 스킨부터 없애서 그런듯.
// --- 일반 EnvBody부터.

// --- Hero 죽을 때 전부 Reset해버려서 EyesColor까지 기본색깔로 바뀌어버림. 이거 파란색 고정해야함.
// --- ResetToDead 메서드를 만들던지, Eyes, Mouth, Eyebrows Container를 추가하던지. ---> OnDead FadeOut도 Body로 처리하면 될듯.
// --- Body와 관련도니 모든 이펙트는 Body에서 처리.
// --- Env Body 만들어야함.

// --- 내일 BodyContainer에서 Default Color Set하기
// --- Env BaseBody 만들기 + 캐싱, Monster Body Container도 미리 캐싱
// --- 미리 캐싱한걸로 Material Set

// --- Skill 관련하여..
// --- 히어로 레벨1. Skill_A, Skill_B
// --- 히어로 레벨3. Skill_A, Skill_B, Skill_C(New)
// --- 히어로 레벌5. Skill_A, Skill_B(강화된 새로운 스킬), Skill_C(강화된 새로운 스킬)
// --- Hero Skill ID 독립적으로 준거 고쳐야함. 재활용하는 방식으로 해야함.
// --- 그리고, 어차피 애니메이션 기반으로 작동하기 때문에 각 크리쳐마다 공격속도 적용하면 됨. 이게 훨씬 타당함.
// --- 그리고, 리더의 경우 데바데 발전기체크처럼 랜덤으로 강화공격 뜨게해서 하는것도 일단 메모. (리더 스킬)
// --- 이렇게하면 좋은점이 스킬 아이콘 같은거 배치할 때 전혀 부담이 없음. 각각 독립적이라. 일단은 그대로 진행. Effect 때문에.
// --- 포터블 모니터 가지고 댕기기

// --- 일단 EnvAnimation 제외(BaseAnimation쪽 손봐야함)

// --- 히어로 20개까지 고정시켜놓고 포폴까지 완성시켜놓은다음
// --- 나중에 출시할 때, 히어로 10개만 더 만들고 출시할 수 있는 방향으로 만들어놓기.

// --- 애니메이션 디테일한것들은 나중에 고쳐야함(ex. 트릭스터 공격시 왼팔 sprite sorting order)
// --- 어쌔신 공격 애니메이션 고치는건 조금 급하긴 함
// --- 리더 히어로 이동시 동료 히어로 무시하는거 변경해보기
// --- Druid 표정 정해야함
// --- Trickster Elite 방패 생겨서 애니메이션 고쳐야함. 아니면 나중에 Elite Hero DataSet 만들때 애니메이션까지 Elite 전용으로 새로 파줄수있긴한데
// --- 노가다가 굉장히 심해질듯. 그래서 안할 것 같지만 만약 기회가 된다면 시도해볼수도 있을듯.

// --- Assassin Upper_Skill_A 수정 필요
// --- 이제 Tricster, Druid 만들고 Assassin Skill_A 수정하면 됨. 그러면 딱 7명.
// --- 이후, 몬스터 애니메이션 체크하고 히어로 AI 다듬고 스킬 제대로 시작.

// --- Lower_Idle_To_Skill_A: 아쳐도 서서 공격할때가 있음
// --- Wizard의 경우 약간 자연스러워지긴했는데, 정말로 Lower_Idle_To_Skill_A 이게 필요할지.

// Assassin Skill_A 빠른 두방, 또는 세방으로 수정
// --- Lv.10 -> Lv.5로 다운 예정

// --- Ranged Hero AI 최초에 가까이 가는 듯한 느낌. 아니면 최초에는 리더를 쫓아가서 그런듯.
// --- FoeceMove랑 관계없이 타겟에게 가까이가는거 확인됨
// --- Ranged Hero AI TryBackStep 켜진상태에서 와리가리하면서 TryBackStep가 false가 안되는거 확인됨
// --- Zone Out, Idle 상태에서 아무것도안하고 가만히 있을 때 예외 처리 줘야할듯

// --- LeaderController에 BattleMode추가해서 true로 되어 있다면 Env와 마찬가지로 움직이면 바로 움직이도록

// --- MeleeHeroAI 대략적으로 수정 된듯. 근데 뭔가 똥을 덜 닦은 느낌.
// --- Formation 모드는 리더랑 멀어질 수 밖에 없긴함. 이거 떄문인가.
// --- 아 완성인듯. Formation 모드와 상관없이 모든 오너는, 오너가 ForceMove일 때, InInTheNearestTarget을 체크하고
// --- 이 때, 거리가 먼 상태라면 강제로 ForceMove 상태로 들어감.
// --- 그러니까 Force가 아닌 상태에서 가만히 있으면 Target에게 Skill 또는 Env를 시전하는 것이 맞는 것 같음.
// --- OnSkillCallback MeleeAttack 부분 체크해볼것. 여기 말고 다른 곳을 수정해야할 것임. 죽었는데 한번 더 때림. (간헐적으로 발생함)
// --- 이 부분 또 발생하면 수정해볼것.

// --- 공격 범위 대략 고침
// --- 리더가 처음으로 바뀌면 잘 되는데 다시 그 리더로 설정하게 되면 AI 먹통됨.

// --- Chicken 완료했으므로 나머지 몬스터들 완료해놓기
// --- Archer, Wizard, Gunner(RangedHeroAI) 만들어 놓고, 기존 RangedHeroAI 스크립트(Static, Dynamic AI) 제거
// --- 코드 정리랑 애니메이션을 다시 만들어서 하니까 애니메이션 파라미터를 제대로 인식하는듯. 이전에 어딘가 잘못되었었다는 뜻.
// --- Env 애니메이션도 복원시켜놔야함. (BaseAnim 신경쓰지말고 그냥 EnvAnim으로 퉁치기. 어차피 Env들 끼리는 똑같아서)
// --- 몬스터가 BodyAttack으로 히어로에게 들이댈 때, 히어로 Melee 어택이 안나가게 해야할까? 이건 선택이긴한데.

// --- Upper_Idle_To_Skill_A, Move_Skill_A 전부 CreatureAnimState Enter, Exit 넣어야함.
// --- 쿨탐 매우 낮아지면 빨라지면 답 없음.

// --- MeleeHeroAI, RangedHeroAI 완성 이후, Dynamic, Static Hero AI 전부 제거
// --- 모든 캐릭터는 이동하면서 Skill_A 사용 가능, 이후 Skill_B, C는 고민좀 해 볼것. 스킬 데이터에 static 옵션을 넣는다던지
// --- 원거리 히어로 AI도 마찬가지로 공격 State Update 중일때만 LookAtValidTarget으로 수정해야함

// --- Archar AI 구현 준비
// 일단 뒤로물러간다음에 공격하는 것으로, 무빙샷 말고. 그 다음에 무빙샷 구현.
// --- 다른 히어로들 애니메이션, AI 준비

// 물체 뒤에 가려진 보이지 않는 오브젝트 아웃라인 처리
// 몬스터 BodyAttack은 몬스터 본체가 아닌, AnimBody같은것으로 인식해서 히어로 앞에 가까이 와도 히어로의 AttackRange 1에 인식이 안되게끔 수정하는것도 괜찮을듯
// (아니면 이것보다 더 좋은것은 IsAttackInRange 자체를 수정하는 것)

// 전반적으로 문제가 많음
// --- 공격속도가 느리다면 상관 없지만, 엄청 빠른 녀석일 때
// --- Upper Move Attack -> Idle시, 트랜지션이 1.2이기 때문에 이 부분에서 어색한거 나중에 고쳐봐야함
// --- (그렇다고 해서 트랜지션을 0으로 하면 AutoTarget, 또는 히어로 동료가 멈추고 허공질하고 난 이후에 제대로 공격을 실행함)
// --- 아무튼 기회가 되면 이 부분은 전반적으로 고칠 필요가 있음. (부드럽지 않은 경험을 제공하기 때문)

// 게임 요소
-- Collect는 어쩔 수 없이 마우스에서 뗏을때 하도록 해야할듯. (완료)
-- CollectEnv 대각선까지 1로 체크해서 사거리가 너무 길어보인다.
-- LeaderController::OnJoystickStateChanged: 채집하다가, 채집물쪽으로 움직이면 기본무기 들고 채집할수도 있음. 이거 고쳐야할듯.

// STAGE 100
-- Animation Moving Shot으로 고려해봐야할지도
-- Force Stop 상태에서는 공격 범위에 들어왔을 때만 공격만 하도록, 채집도 불가능.
-- 아처 공격 애니메이션 수정 필요(트랜지션을 모두 0.99, 0.01로 바꿈. 애니메이션 자체로 바꾸는게 더 자연스러워서)
-- Gunner Skill Attack 두 번 공격 해야함
-- 이펙트 데이터 만들기
-- 히어로 제작
-- 몬스터 크기 조절, 히어로 무기 크기같은거 조절(ex)거너
-- Gunner Shell Base Local Position 데이터 추가 필요

// --- Gunner
// --- Assassin
// --- Tricster
// --- Druid
// --- Phantom Knight
// --- Frost Weaver
// --- ...

// --- Skeleton King (SPECIAL)
// --- Mutant (SPECIAL)
// --- Queen (SPECIAL)

[ TODO LIST ]
// 24. 06. 07
>>> 데미지 트윈 애니메이션 지속적인 수정 필요(ULTIMATE DMG ASSET 가져오기)
>>> 리더랑 히어로 멤버 같은 위치에 있을 때 아직 고장나있는거 확인 (AI 지속적으로 개선해야함)
>>> 프로젝타일부터 고칠까, 근데 이펙트가 있으면 좋긴해.

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

[ MEMO ]
--- Data Sheet ID
>> Heroes (101000 ~ 101999)
>> Monsters (102000 ~ 102999)
>> Envs (103000 ~ 103999)

>> 위아래 꼬불꼬불 위자드 특수스킬 프로젝타일
>>>>> TileMapRenderer Mode를 Individual로 바꾸니까 해결된 느낌
>>>>> 나무쪽 타일맵 랜더러 Sorting Group 추가하지 말고 그냥 Sorting Layer만 BaseObject로
>>>>> Mode를 Individual로 바꾸고, 나무1 스프라이트의 피벗을 0.5, 0.1로 바꾸면 자동정렬 완성
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