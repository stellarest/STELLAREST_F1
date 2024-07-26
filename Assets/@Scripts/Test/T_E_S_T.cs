using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

/*
// --- Animation 간소화해서 다시 잡는중
// --- CreatureData 통합해볼것
// --- using STELLAREST_F1.Data; 이거 using 쓰는 부분 지우기. Data는 명시해서 쓰자.

// --- Paladin 무빙샷 잡아보기
// --- Archer Projectile Test

// --- Move Attack 거리도 잡아야할 것 같고.
// --- Projectile Test 필요

// [ TODO ]
// ---> Projectile 부분 얼추 고침
// ---> PalabolaMotion 부분에서 TargetDir이 있어야하는데 이거 없다면 Owner의 LookAtDir 부분으로 대체
// ---> EffectComponent::GenerateEffect Spawn할 떄 고민해볼것
// ---> Effect::SetInfo 부분 고쳐야함. 일단 BaseObject이므로 이 부분은 여기에 맞게 따라야할듯
// (특히 ObjectManager의 SpawnBaseObject를 따라야 할 것 같음)

// ---> SpawnPos 부분 다시 보기.
// ---> Moving Shot 제거 예정
// ---> 리더는 AutoTarget옵션만 있으면 될 것 같고, 무조건 100% Follow Leader
// ---> 클라우드 지원 확정(구글 플레이, 앱스토어 API 기반, 아무래도 조금이라도 육상하는 맛은 있어야함)

// ---> SkillBody, ProjectileBody ??
// ---> AI 정리. BaseObject부터 불필요한 부분 정리 / 제거.
// ---> 정리 완료후 스킬 이펙트 작업.
// ---> 크리처가 Obstacle 뒤에 가려졌을 때, Outline 효과 처리. (나중에)

// --- ((Env Tree, Rock)), 내일 --> Change Sprite Set, Fade Out
// --- + ProjectileBodyBase ?? 추가 ?? SkillBodyBase ??
// --- HeroStateWeaponType, EnvWeapon 다시 잡아야함.
// --- EnvTree Fruits Pos, Rot, Scale 생성 될때마다 Min, Max사이로 배치되도록 수정해야할듯.
// --- this.DefaultMat = defaultMat; // ---> Name 변경 필요: DefaultSPRMat
// --- OreLight SPR Alpha가 1로 되어있음... (고침, BaseObject ShowBody 부분 고쳐야함)
// --- 골드 OreLight가 피격되면 이상하게 어두워짐.
// --- BodyContainer 생성자에서 new MatPropertyBlock 받도록하게 변경해야할듯(안쓰는 것은 굳이 만들 필요 없을것같아서)
// --- 이거 때문에 Gold Glow가 이상하게 적용 되는 것이었음.
// --- Arhcer CollectEnv 마치고 무기랑 애니메이션 전환 어색한거 진짜 진짜 엄청 나중에 잡아볼것. 안잡아도 되긴함.
// --- HeroBody.ChangeSpriteSet 완성했지만 히어로 다 불러와서 제대로 On, Off 되어있는지 확인해볼것
// --- 제거 얘정

// --- SpriteManager에서 EnvBody SPR.color 따로 셋팅해야될지 고민중. 안해도 될 것 같긴 한데.
// --- 동일한 이유로, 몬스터도 SkinColor를 제외하고는 SPR.color를 안줘도 될 것 같음. 어차피 하얀색이 디폴트라.
// --- 어쨋든 Env부분 Color 세팅하는거 안해도 될 것 같고, 몬스터도 SkinColor 안해도 될듯.
// --- MonsterBody도 foreach 루프로 돌려서 하면 될 것 같고. 이후 EnvRockBody 제작할것.
// --- MonsterBody부터 테스트. 아니면 몬스터 SkinColor 없애도 상관없을 것 같긴 함. 디폴트는 모두 하얀색으로.
// --- BodyContainer의 DefaultColor는 모두 있어야함. 단, 몬스터, Env의 SPR default color는 white로.

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