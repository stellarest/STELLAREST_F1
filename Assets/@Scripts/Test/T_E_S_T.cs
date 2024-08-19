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

- Archer(Chara + Anim) - Projectile, Parabola, Archer Attack Lower Anim
-- Skill Target Range 여러가지로 고쳐야할지. 예를 들어 좌우 동시, 상하 동시, 엑스자 등...
-- 북마크 참조(ESkillTargetRange, ETargetDirection), Horizontal, VerticalUp, ... , DiagonalDown은 Single에서 사용하는 것은 맞고
Single에서 그대로 사용하면 됨. 추가해야 할 것은...
-- Skill Target Range 예시
Single (그대로 사용)
BothHorizontal
BothVertical
BothDiagonal

- Wizard - Projectile, Straight
- Priest, Gather Ally Targets
- Failed Effect_OnDeadSkull (O, TEMP, Again)

> Projectile, Effect쪽 그럭저럭 해결되면 조금 더 수월해지긴할듯.
> 스테이지별로 클리어, 대충 20~30스테이지 정도까지. (Normal / Extreme으로 분리)
> 영웅을 스킬을 육성할 것인가? vs 영웅을 고용할 것인가? vs 영웅 스탯을 키울 것인가? (결국, 게임의 핵심은 가챠 + 선택하라)
********************************************************************
*/