using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;

/*
// TODO NEXT
- Wizard Attack Anim 교정
- 전투 준비 (A* 없이)
- Monster RigidBody Linear Drag (100 -> 300) : 나중에 종류별로 바꿔야함.
- 멀어졌을 때 이동속도가 증가하는 로그함수 구현하기.

- 그리고 예를 들어 바위를 채집할 때, 바위 완전 밑에서 채집하면 이상하니까, 이런거 위치 조정해줘야 할 것 같은데.
- *** 시작하자마자 Camp Pivot으로 닥돌하는거 고쳐야함 ***
- *** Hero Dead Animation -> Fade ***
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
            float distancePerFrame = _movementSpeed * Time.deltaTime;
            transform.Translate(_moveDir * distancePerFrame);
        }
    }

    private void OnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
    }
}

/* Memo
Only One류 게임. (벤서류는 아니고)
정처없이 돌아다니면서 사냥 + 육성 + 대략 100레벨 정도 만들고 클리어하면 끝.
조금 더 쉽게 클리어 하고 싶다 -> 엘리트 패키지 구매(광고 제거 포함, 3,300 또는 5,500, 걍 돈 쓴거에 대한 경험만 시켜주면 됨)

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

*/
