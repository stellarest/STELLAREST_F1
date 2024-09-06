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
// --- TODO LIST
- InitBaseError 이 부분 고치기 
- SimpleVFX(EffectType)도 EffectComponent로 무조건 호출

// --- DID NOTE
- Collider, Rigid는 일단 모두 다 붙여줌(Effect도 언젠가 필요한 녀석이 생길지도)
- Stat은 BaseCellObject에서 관리
- HERO LEVEL UP TEST

// --- NOTE
- *** 지금 게임 실행 불가. Effect 대국적으로 수정해야함 ***
- Skill이 가지고 있는 EffectType부터 생각해봐야할듯.

        Instant,    // --- DotBase
        Buff,       // --- BuffBase
        Debuff,     // --- BuffBase
        Dot,        // --- DotBase
        Infinite,   // --- BuffBase
        Knockback,  // --- CCBase
        Airborne,   // --- CCBase
        Freeze,     // --- CCBase
        Stun,       // --- CCBase
        Pull,       // --- CCBase

Double Slash
- Instant
Shield
- Buff(Infinite)
와 진짜 개쩌네 ;; 이펙트

- 크게 수정하지 말고, 일단 GenerateEffect부터.
- 그 다음 GenerateProjectile.
- 그 다음 구조 생각해보기
- Effect 쪽 1차 수정, 이펙트 스탯 적용
- Effect를 애초에 Base Owner에 붙인다??

// --- LATER LIST
- Archer Muzzle Effect

// --- NOTE
- 결국 Effect 주도(중심)의 게임
- Heroes + Monsters + Skill + Effect만 무한정 찍어낼 수 있으면 싸구려 게임(최대 3300)으로써 중간 이상은 갈 수 있을지도.
- 사실 이정도면 잘 해도 3300 판매 가능.
- Skill, Effect부터 차례대로 적용하고 가장 나중에 Hero AI, Monster AI 적용
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
- Fix Skill Trggers (O)

// --- TODO LIST
- Skill_C 넣으면 동작 또 이상해짐...
- BaseStat 1차 정리
- Next Commit: [Feat]Paladin Skill_C(Shield)
- Add Skill Value
- ...

// --- LATER LIST
- 몬스터가 Hero에게 이동할 때, 길이 막혀있는 경우 무한 와리가리하다가 자리를 찾아 오는 경우가 있음(심각한 버그는 아님)
- How to gather Allies Targets?
- OnDustEffectCallback, Paladin만 나오는 중
- EEffectSourceType::Projectile은 어떻게 할 계획?
- Archer Skill_A Effect His Pos Random으로 할것임? Hit Effect Pos 옵션도 줘야하지 않을까
- 예를 들어 Hit Effect의 데이터를 추가한다던지(맞춘 위치에 스폰 시키는)
- Effect Swing도 처음에는 target Range1, 그 다음에는 Swing의 크기를 늘린 것으로 target range2 이런식으로 해야할까
- 투명 프로젝타일 못맞추면 엉뚱한 녀석이 맞는데 크기를 키워야되나. 아니면 그냥 Melee Swing으로 할까.
- MeleeSwing으로 고친다고해도 Projectile Effect 설정하는 부분 덕분에 많은 부분을 고칠 수 있게 되긴 함.
- ex. Paladin_Skill_B Effect에 AddPercent같은거 붙여서 추가 데미지 줘야할지 (SourceDmg + AddPercentEffect)
- Paladin Skill_C(Shield) 대략 완성하고 Effect Stat 대략적으로 잡고 캐릭터, 스킬 레벨업하는거 잡기(진화 스킬까지)
- 이런식으로 캐릭터 선형으로 추가해나가기(Druid까지, Barbarian까지 할 수 있으면 더 좋고)

// --- HERO NOTE
- Paladin (101000)
>>> Skills (히어로 레벨업시 스킬도 자동 레벨업)
> Skill_A (101100): Melee Enemy
> Skill_B (101200): Projectile Enemy
ex. lv.2: + Dmg
ex. lv.3: + InvokeRange
ex. lv.4: + Cooldown
ex. lv.5(Max): Triple Attack 이런식으로하면 가능하겠네

> Skill_C (101300): Self -> Self + Allies
ex. lv.2: + Increase Shield Value
ex. lv.3: + Increase Shield Value
ex. lv.4: + Cooldown
ex. lv.5(Max): Judgement of the Heaven 


// --- NOTE
1. Paladin
2. Archer
3. Lancer
4. Wizard
5. Assassin
6. Gunner
7. Tricster
8. Druid

- 스킬 데이터 예시, 쉴드 궁극기 같은 경우는 같은편에게는 쉴드 효과를 걸어주고, 적에게는 스턴 효과를 줄려면
- TargetToEnemyEffectIDs, TargetToAllyEffectIDs. 이펙트로 효과 먹여도 되긴함. (나중에)
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