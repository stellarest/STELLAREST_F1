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
// --- Projectile::DamageToTarget
// --- 이미 이전에 데미지를 받온 오브젝트를 Penetration으로 인해 데미지를 다시 주기 싫으면
// --- 여기서 처리하면 됨. 그러나 Around Targets가 조금 어색해보여서 다시 변경함.
// --- Around Targets는 target 기준 뒤에 있는 오브젝트에게 데미지를 전달해야 Around의 의미가 있지만
// --- 겹쳐있는 오브젝트에 대해서 Half처럼 보이기 때문에 여기서 따로 처리 안함. (Straight, Parabola Test 완료)
// --- _includedTarget 플래그 옵션 줘야할지. 옵션에 따라서 includedTargets 무시하고 데미지를 적용할지.
>> Archer 바라보는 방향 수정

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