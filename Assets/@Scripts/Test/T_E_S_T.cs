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

- Archer(Chara + Anim)(O) - Projectile(^), Straight(O), Parabola(O), Archer Attack Lower Anim(O)
- Projectile - Single(O), Half(O), Around(O)
- Lancer Spawn, Play Test(O)
- Wizard Spawn, Projectile Test(O)
- Seperate Hero Stat Data, Monster Stat Data, Env Stat Data(^)
- DataID 검토, 스킬 데이터 분리 시도
- 위자드로 채집하고 있다가 몬스터가 등장하면, 쿨타임 훨씬 지났을 텐데 늦게 공격함. (원인은 RemainCoolTime)
// --- DataSet FileName 변경
> ex) TreeSpriteData -> EnvTreeSpriteData
> ex) BirdSpriteData -> MonsterBirdSprtieData
// --- SkillData도 분리 시도(어차피 스킬은 크리쳐로부터 작동되므로)

>>> Hero AI, Monster AI
>>> EStartTargetRange, XShaped
>>> Priest, Gather Ally Targets (Add Skill Heal, How to Gather Ally Targets)
>>> Melee Hero, Ranged Hero AI
>>> Organize Effect and Skills
>>> Add Monsters

// --- Note
> Projectile, Effect쪽 그럭저럭 해결되면 조금 더 수월해지긴할듯.

// --- 이 방식이 제일 직관적이고 깔끔한듯.
- Paladin Lv.1
> Double Attack Lv.1 (Unlock)
> Shield Lv.1 (Unlock)

- Paladin Lv.2
> Double Attack Lv.2 (Unlock)
> Shield Lv.2 (Unlock)

- Paladin Lv.3
> Double Attack Lv.3 (Unlock)
> Shield Lv.3 (Unlock)

- Paladin Lv.4
> Double Attack Lv.4 (Unlock)
> Shield Lv.4 (Unlock)

- Paladin Lv.5(Max) // Evolved (Evolv 조건, Skill_B, Skill_C 둘 다 lv.4)
> Triple Attack Lv.5 (Unlock) - 진화시 자동 장착
> Heaven's Shield Lv.5 (Unlock) - 진화시 자동 장착

********************************************************************
*/