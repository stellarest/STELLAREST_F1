using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

/*
********************************************************************
- Monster ForceWait When Spawned (O)
- ENV Test (O)
- Leader LookAtValidTarget Off when using Joystick (TODO: Out Leader CoPathFinding Immediately (O)
- Env And Chicken Test (First Target Chicken, Second Target Env) (O)
- Default Skill Test: Hero Half Target(O) -> Signle Target(O) -> Around Target(O)
- Failed Effect_OnDeadSkull (O, TEMP)
- Archer(Chara + Anim)(O) - Projectile(O), Straight(O), Parabola(O), Archer Attack Lower Anim(O)
- Projectile - Single(O), Half(O), Around(O)
- Lancer Spawn, Play Test(O)
- Wizard Spawn, Projectile Test(O)
- Hero Level Up Test (O)
- Seperate Hero Stat Data(O), Monster Stat Data(O), Env Stat Data(O)
- CollectEnv -> DoSkill Immediately (O)
- DataFile 분리ㅡ DataID 검토, 테스트(^) -> 완료 후 push(Feat, Seperate Skill Data)
- Monster Body Attack 약간 뜸들이는듯(Paladin으로 그리드 위치 바꿔가면서 Chicken 주변에 서성일 때 BodyAttack 제대로 동작하는지 테스트)
>> 맞음. Wizard로 테스트해본 결과, 위자드 그리드까지 오고 나서 멈추고, 그 이후에 쿨타임 체크하고 떄리는 것 같음. 고쳐야함.

>>> Hero AI, Monster AI
>>> EStartTargetRange, XShaped
>>> Priest, Gather Ally Targets (Add Skill Heal, How to Gather Ally Targets)
>>> Melee Hero, Ranged Hero AI
>>> Organize Effect, Skills
>>> Next Skill ID in Hero Stat Data?
>>> Add Monsters

// --- Note
> Projectile, Effect쪽 그럭저럭 해결되면 조금 더 수월해지긴할듯.

// --- 이 방식이 제일 직관적이고 깔끔한듯.
// --- 스킬까지 스킬 포인트로 따로 찍게 하는 것은 조금 오바스러운 것 같음
// --- 차라리 육성하고 싶은 육성 선택 후 레벨업을 하면 Skill_A, B, C는 모두 자동으로 레벨업
// --- 이후 다른 가챠 요소는 브로테이토처럼 Item, Passive User Skill(모든 히어로광역 적용)같은 것을 넣던지
- Paladin Lv.1 (LvStatID - 101000)
> Skill_A Lv.1 (101100)
> Skill_B Lv.1 (101200)
> Skill_C Lv.1 (101300)

- Paladin Lv.2 (LvStatID - 101001)
> Skill_A Lv.2 (101101)
> Skill_B Lv.2 (101201)
> Skill_C Lv.2 (101301)

- Paladin Lv.3 (LvStatID - 101002)
> Skill_A Lv.3 (101102)
> Skill_B Lv.3 (101202)
> Skill_C Lv.3 (101302)

- Paladin Lv.4 (LvStatID - 101003)
> Skill_A Lv.4 (101103)
> Skill_B Lv.4 (101203)
> Skill_C Lv.4 (101303)

- Paladin Lv.5 (LvStatID - 101004)
> Skill_A Lv.5 (101104)
> Skill_B Lv.5 (101204)
> Skill_C Lv.5 (101304)
********************************************************************
*/