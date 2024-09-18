using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

/*
// --- My Fantasy Heroes
// - 광고 있는 버전, 제한된 버전
// - 광고 없는 버전 (3,300)
// - Stat 클래스로 교체해서 수정. 버프류 같은것은 스탯별로 SubStat으로 처리. 합연산 곱연산등.
// --- BaseStat 주석 참고
// --- Hero LevelUp쪽을 건드려야.
// - 레벨과 스킬은 같이 오름. (개발의 편의성)
// - UnCommon(특성 강화), Rare(1차 변화), Epic(매우 좋은 특성 강화), Legendary (매우 좋은 2차 변화 )밸런스로 떼우기.

// --- STEP BY STEP
Common, Uncommon, Rare, Epic, Legendary

- 지금 레벨업하면, 히어로 스탯과 스킬 레벨이 같이 증가하는 것으로 되어 있음.
---> 이대로 가되, 스킬 효과만 Rare에서 조금 특별한 효과 주고, Legendary는 완전히 다르게 업그레이드 시켜주고.
---> SKILL_C는 조금 특별한 스킬이라 쿨타임 길게.
---> 히어로 레벨업 스탯 따로 + 레벨업하면 어차피 SKILL_A 레벨 오르니까 여기에 버프 효과 줘서 버프 효과 따로.

- 레벨업을 한다면 전체 스탯이 증가한다.
- 히어로는 원하는대로 따로 레벨업은 확정(Max lv.5)
> 근데 스킬은 어떻게할것인가? 히어로가 레벨업을 한다면 스킬은 자동으로 Unlock 되는가?
> 아니면 히어로 레벨(조건)에 따른 스킬 언락 또는 레벨업을 따로 둬야 할 것인가? Unlock은 되고 강화만? 이것도 괜찮은 것 같은데

- 히어로 데이터 시트, 몬스터 데이터 시트에 스킬ID는 전부 적어놓고 시작한다.
- 최초 레벨업은 Unlock이 되고, 그 이후에는 Max Level까지 레벨업이 되어야한다.
/************************************************************************************
// LevelUp Stat 증가부터,, 스킬 레벨업이랑 독립적으로 적용시켜야할듯.
// Organize related to find path methods

// - 실제 스탯 능력치의 증가는 곱연산으로. 
// - 아이템 조합(5% + 5% = 12% 카드 생성)은 합연산으로. (유저에게 직관적)
// - 능력치 부여는 곱연산으로.

// - * 히어로 레벨이 증가하면, 히로의 기본 스탯이 증가한다.
// - * 히어로 레벨은 어떻게 증가시킬 것인가?
// - * InGameGrade? 5단계로?
1. 일반 (Common)
2. 고급 (Uncommon)
3. 레어 (Rare)
4. 에픽 (Epic)
5. 전설 (Legendary)
아... 모든 것이 랜덤. 나쁘지 않을 것 같은데, 웨이브 종료할 때 마다.
일반 + 일반은 레어로 만들고. 레어+레어는 에픽으로.

최대 체력 증가 일반 등급 버프 카드가 나올수도.
리롤하거나 다음 웨이브 종료후, 레어 등급의 최대 체력 버프가 나올수도.

각 버프 카드는 중첩이 안된다.
예를 들어, 일반 등급의 체력 버프를 먹고. 그 다음 웨이브에서 레어 등급의 체력 버프가 나왔다면,
아 중첩이 되야되겠군.

// - * 레벨업
: 각 히어로마다 정해진 능력치 대폭 증가 (Max level.5)

// - * 고정 스탯 증가
: 각 히어로의 고정 스탯을 증가할 수 있다. (Infinite Buff)

// - 재화는(Wood, Stone, Apple, Ore... 등등 각종 다양한것들이 있는데, 웨이브가 종료되면 전부 Soul로 전환됨)
// - Soul은 히어로 스킬, 유저 아이템에 활용 된다.
// - 모든 웨이브가 종료(스테이지 클리어)되면 골드를 얻게 된다. 골드는 히어로 연구소 스탯, 인게임 아이템 구매(킹덤러쉬 참고)에 활용할 수 있다. 
// - 몬스터를 사냥해서 잡으면 웨이브 유저 경험치가 증가하고 웨이브 유저 레벨업을 하게 된다. (Max. 5)
// - 연구소 스탯은 예를 들어... 기절 확률리 적용되어 있는 모든 스킬에 기절 확률을 5% 증가한다. 이런거?
// - 모든 히어로의 기본 스킬(기본 공격등)의 쿨타임이 10% 감소한다. 이런거. 아니면 이것을 공격속도 증가로 해도 될 듯?
// - 무조건, 유저는 쉽게 받아들일 수 있도록. 유저의 입장에서 수학적인게 아닌, 공격 속도가 증가한다. 이런식으로.
// - 무조건 Soul로 All-in-One, Soul로 히어로의 레벨, 히어로의 스킬, 인게임 웨이브 아이템 구매 등.

// --- SKILL BLANCE NOTE
[Abstract]
- 나무, 광석, 초록 사과, 빨간 사과, 은색 광석, 금색 광석 등은 웨이브 종료시 모두 Soul로 전환
- 인게임 재화는 무조건 Soul로 All-in-One, Soul로 히어로의 레벨, 히어로의 스킬, 인게임 웨이브 아이템 구매 등.
- 히어로의 레벨을 올리면, 히어로의 기본 스킬(Skill_A)의 레벨은 자동으로 올라간다. (Hero Max Level.5)

- LevelUpInfo 구조체를 가지고 있어야 할까. 아니면 히어로의 기본 스탯만 정해져있는 상태에서 스탯을 찍어서 올려주는 형태로?
- 내가 착각하고 있었다. 기본 스탯은 변함이 없는 거고, 각종 버프만 기본 스킬이 가지고 있게끔.
- 레벨업을 한번 클릭하며 현재 스텟 기준으로 증가 될 스탯을 수치와 초록색%로 보여주고 한번 더 누르면 레벨업을 하게끔.
- 캐릭터마다 증가되는 스탯양이 당연히 다르다. 예를 들어 팔라딘은 레벨업하면 체력이 높은 수치로 증가함.

"DataID": 101000,
            "DevTextID": "Lv.01 - Paladin",

            "MaxHp": 550.0,
            "MinAtk": 12.0,
            "MaxAtk": 15.0,
            "CriticalRate": 0.0,
            "DodgeRate": 0.0,
            "MovementSpeed": 5.0

[Paladin]
S_A (Stat Template)     (101100)
: 기본 근접 공격
(캐릭터 스탯과 관련된 모든 버프를 이녀석이 갖고 있다)

S_B
- Slash I               (101200)
: 사정 거리 2까지 도달하는 검기를 날려 2회 공격한다. 

- Slash II              (101201)
: 검기의 사정 거리가 2 -> 3으로 증가한다.

- Slash III             (101202)
: 검기의 공격력이 10% 증가한다. 

- Slash IV              (101203)
: 검기의 공격력이 10% -> 20% 증가한다.

-  Slash V              (101204)
: 거대한 검기를 3회 날려 공격한다.

S_C
- Slash I               (101200)
- Slash II              (101201)
- Slash III             (101202) 
- Slash IV              (101203)
- Ultimate Slash        (101204)

// --- SKILL NOTE (I, II, III, IV, V)
[Paladin] 
P: Endurance
S1: Slash I -> ... -> Slash IV -> Ultimate Slash 
S2: Shield I -> ... -> Shield IV -> Ultimate Shield

[Archer]
P: Hawk Eyes
S1: Multiple Shot I -> ... -> Multiple Shot IV -> Ultimate Multiple Shot
S2: Arrow Storm I -> ... -> Arrow Storm IV -> Ultimate Arrow Storm

[Lancer]
P: Determination
S1: Thrust I -> Thrust I -> Ultimate Thrust
S2: Line Rush I -> ... -> Line Rush IV -> Ultimate Line Rush
*/

/*
// --- TODO NOTE
- Impact Critical Hit Pooling 안됨
*/

/*
********************************************************************
- ApplyEffect는 EffectComp의 ActivateEffects에 추가가 되었을 때, Apply할 수 있도록.
- BaseCellObj의 ApplyStat을 호출하면, 재정의된 ApplyFinalStat 메서드를 통해서 값을 설정할 수 있게 된다.
- 쉴드 on할땐 Owner 로컬로 붙이고 off할 땐 다시 PoolingRoot에 붙여줄 것인가? (이것부터 검토)

// --- TODO LIST
- DamageFont 클래스에 Critical Font 스폰하고 있는데 이거 TextFont Class 따로 제작해서 빼야할듯
- TextFont Class에는 폰트 타입을 정할 수 있고 지정한 글자를 작성할 수 있음.
- TextFont를 통한 Critical Font는 objManager를 통해서 생성
- DoTween DoAnimation 기능은 MonoContentsManager로 따로 빼야할듯.

- Paladin Level Up, Level Up Stat, Level Up Skill
-->> 하지만 이 부분을 하려면 먼저 기획이 어느정도 확실해야할듯.

- Critical Font
- 아쳐 머즐이펙트, 발바닥 Dust(랜서, 위자드 포함)
- Related FindPathMethods in Creature.cs -> in BaseCellObject (아직 미완인 것 같긴 함)

// --- NOTE

// --- SKILLS NOTE
[Paladin] Hmmm...
P: Second Wind (E)
S1: Double Slash(C) -> Triple Slash (E)
S2: Shield(C) -> Heaven's Shield (E)

- Instant,    // --- DotBase
- Buff,       // --- BuffBase
- Debuff,     // --- BuffBase
- Dot,        // --- DotBase
- Infinite,   // --- BuffBase
- Knockback,  // --- CCBase
- Airborne,   // --- CCBase
- Freeze,     // --- CCBase
- Stun,       // --- CCBase
- Pull,       // --- CCBase
- 결국 Skill, Effect 주도(중심)의 게임
- Heroes + Monsters + Skill + Effect만 무한정 찍어낼 수 있으면 싸구려 게임(3300)으로써 중간 이상은 갈 수 있을지도.
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