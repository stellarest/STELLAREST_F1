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
- Projectile - Single(O), Half, Around
>>> Projectile Single
>> Straight (O)
>> Parabola (^), 개선 필요
Projectile LastCellPos를 정확하게 받아와서 Dot을 처리하면 훨씬 도움이 될 것 같은데
LastCellPos를 받아올 때 target의 맞은 위치와 프로젝타일의 방향을 기반으로, dot을 활용해서
LastCellPos를 추측할 수 있을 것 같음. 아니면 단순하게 아래쪽에서 맞았는가 그러면 바로 밑인거고 이런식으로. Staight는 가능.

>>> Archer Attack Anim 동작을 할 때(화살을 당기고 있는 동안)는 타겟 방향을 바라보도록. OnSkillClip 발동시에 해제하면 될듯
>>> EStartTargetRange, XShaped
>>> Wizard - Projectile, Straight
>>> Priest, Gather Ally Targets (Add Skill Heal)
>>> Melee Hero, Ranged Hero AI
>>> Organize Effect and Skills
>>> Add Monsters

> Projectile, Effect쪽 그럭저럭 해결되면 조금 더 수월해지긴할듯.
> 스테이지별로 클리어, 대충 20~30스테이지 정도까지. (Normal / Extreme으로 분리)
> 영웅을 스킬을 육성할 것인가? vs 영웅을 고용할 것인가? vs 영웅 스탯을 키울 것인가? (선택하라)
> 결론은 대부분의 유저가 나의 게임을 클리어할 수 있게 만들어야함. 극악의 난이도는 절대 안되고, 약간 쉬움~노말 중간 상태로
********************************************************************
*/