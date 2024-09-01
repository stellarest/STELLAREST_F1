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
// --- DID LIST
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
- Seperate DataFile, Organize, Check Data ID (O)
- Check Monster BodyAttack (O)
- Effect Swing Flip Test (O)

// --- TODO LIST
- Next Commit: Add Projectile Start Effects 
- Melee Swing Particle Fixed Enter Dir vs Projectile Melee Swing Skill
- Effect Swing Melee -> Projectile (need to add projectile effect)

// --- LATER LIST
- 몬스터가 Hero에게 이동할 때, 길이 막혀있는 경우 무한 와리가리하다가 자리를 찾아 오는 경우가 있음(심각한 버그는 아님)
- How to gather Allies Targets?

// --- NOTE
- 스킬 데이터 예시, 쉴드 궁극기 같은 경우는 같은편에게는 쉴드 효과를 걸어주고, 적에게는 스턴 효과를 줄려면
- TargetToEnemyEffectIDs, TargetToAllyEffectIDs. 이펙트로 효과 먹여도 되긴함.
- Paladin Double Attack -> Shield
- Spawn Turkey, Bunny, Pug

>>> Hero AI, Monster AI
>>> EStartTargetRange, XShaped
>>> Priest, Gather Ally Targets (Add Skill Heal, How to Gather Ally Targets)
>>> Melee Hero, Ranged Hero AI
>>> Organize Effect, Skills
>>> Next Skill ID in Hero Stat Data?
>>> Add Monsters

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