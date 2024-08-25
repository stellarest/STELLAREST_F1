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
// --- Lancer Spawn, Test
// --- Wizard Spawn, Test
// --- Hero AI, Monster AI

>>> Archer Attack Anim 동작을 할 때(화살을 당기고 있는 동안)는 타겟 방향을 바라보도록. OnSkillClip 발동시에 해제하면 될듯
>>> EStartTargetRange, XShaped
>>> Wizard - Projectile, Straight
>>> Priest, Gather Ally Targets (Add Skill Heal)
>>> Melee Hero, Ranged Hero AI
>>> Organize Effect and Skills
>>> Add Monsters

// --- Note
> Projectile, Effect쪽 그럭저럭 해결되면 조금 더 수월해지긴할듯.
********************************************************************
*/