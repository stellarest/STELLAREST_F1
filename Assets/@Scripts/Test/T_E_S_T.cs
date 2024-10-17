
/*
// Skill Data ID
AwakeEffectIDs -> OnCreateEffectIDs
EnterStateEffectIDs -> OnSkillEnterEffectIDs
OnStateEffectIDs -> OnSkillCallbackEffectIDs
EndStateEffectIDs -> OnSkillExitEffectIDs
HitEffectIDs -> OnHitEffectIDs

// CreatureStateMachine for SkillBase
OnSkillStateEnter
OnSkillCallback
OnSkillStateExit

- Lv.01
> Passive(Common)

- Lv.02
> Passive(Common)
> Skill_B(Common)

- Lv.03
> Passive(Rare)
> Skill_B(Common)
> Skill_C(Common)

- Lv.04
> Passive(Rare)
> Skill_B(Rare)
> Skill_C(Common)

- Lv.05
> Passive(Unique)
> Skill_B(Rare)
> Skill_C(Rare)

- Lv.06
> Passive(Unique)
> Skill_B(Unique)
> Skill_C(Rare)

- Lv.07
> Passive(Unique)
> Skill_B(Unique)
> Skill_C(Unique)

- Lv.08
> Passive(Elite)
> Skill_B(Elite)
> Skill_C(Elite)

1. 웨이브 종료. (Max Wave: 20)
(웨이브 종료시에 나타나는 성장UI를 에서만 히어로를 성장시킬 수 있음)

2. 아이템 가챠 / 히어로 고용(TODO)
- 100% 보유 시키기, 가챠, 리롤 가능
- 보유하고 있는 목록 보여줌, 보유하고 있는 아이템 판매 가능 (글로벌 아이템)
2-1. 히어로 고용 메뉴 버튼 존재 (선택, TODO)
- 보유하고 있는 히어로 고용, 비용 소모

3. 히어로 레벨업 / 히어로 스킬 정보 (단순한 정보만 표기)
- 레벨업 버튼을 누르면 재화를 소모하고, 가챠 스탯이 생성됨. (반드시 선택) 
- 최대 레벨은 Max.8. Max.8 레벨을 찍으면 Elite(Dev_Code) 형태로 변함.
- 이후에도 계속 스탯을 찍을 수 있지만, Lv.08(Max+)라고만 표기.
- Lv.08에는 스탯을 찍는 대신(진화이므로), V2 스킬로 갈 것 인지, 강력한 궁극기로 갈것인지(미정)

3-1. 히어로 스킬 정보 메뉴 버튼 존재 (선택)
- 스킬 정보 메뉴를 선택하면, 단순히 정보만 보여줌
- 게임 시작시, 이미 Lv.01 패시브는 찍혀있는 상태

(EX) Common(I, white) < Rare(II, green) < Unique(III, pueple) < Elite(IV, yellow)
// --- 만약, 추가적인 외형을 추가한다면 03(Rare), 05(Unique) 부분까지만 필요.
> Lv.01, Lv.03, Lv.05, Lv.08 Passive
> Lv.02, Lv.04, Lv.06, Lv.08 Skill_B
> Lv.03, Lv.05, Lv.07, Lv.08 Skill_C

> Lv.01, Lv.03, Lv.05, Lv.08 Endurance(I, II, III, IV)
> Lv.02, Lv.04, Lv.06, Lv.08 Slash(I, II, III, IV)
> Lv.03, Lv.05, Lv.07, Lv.08 Shield(I, II, III, IV)

        Passive: 101000(Lv.01), 101002(Lv.03), 101004(Lv.05), 101007(Lv.08)
        Skill_A: 101100(Lv.01)
        Skill_B: 101201(Lv.02), 101203(Lv.04), 101205(Lv.06), 102007(Lv.08)
        Skill_C: 101302(Lv.03), 101304(Lv.05), 101306(Lv.07), 103007(Lv.08)

3. 히어로 레벨업(*) - (LevelUp, Skill Info UI 메뉴 별도 표기)
- 현재 진행중엔 웨이브 수에 따른 레벨업 성장 가능 제한 그런건 없음. 성장을 통한 재화에만 한계만 존재함.
- 웨이브 종료했을 때만, 유저가 원하는 히어로 레벨업은 언제든지 가능.
- 히어로 레벨은 n까지 존재. 레벨업(스탯 가챠 뽑기 UI) | 스킬업 UI 따로 존재.
- 레벨업 스탯 가챠 뽑기(UI)는 무한대로 가능(MaxLv.20)

- 최초 히어로는 레벨 1이니까, 최초에 누르면 스탯 가챠가 나타나있음.
- 레벨업시 비용이 소모됨. 레벨업을 누르면, 다시 스탯 가챠가 나타남. 히어로의 레벨은 10까지 존재함.
- Hero Max Lv.10 -> Max.10+ 이걸로 고정.
- 스탯 가챠는 리롤이 가능하고 리롤하면 비용이 소모됨.
*******************************************************
1. 스킬B, C 잡기. (Common -> Rare -> Unique -> Elite)
2. 스킬 Elite까지 다 잡고나서 치킨 사냥 테스트.
// Effect_BuffDamageReduction 이 부분 고민해보기
// 평소에 생각해 볼 건 Slash 2,3,4, Shield 2,3,4 
*******************************************************
패시브 버프 이펙트랑 스킬해결하고, BaseAnim Flip이랑 A* 부분 모조리 Creature쪽으로,,,


// *** Projectile: FindSkill 주석 참고
// Hero Skill 수정(Skill_A, B(Lv.3부터 자동 잠금 해제), C(Lv.5(Max)에서 자동 잠금 해제), Elite Skill
// HeroData, Stat 재설정, DataID에 따른 패시브만 기본적으로 들고있고, 나머지는 레벨업하면서 가챠돌리면서 스탯찍는 방식으로
————————————————————————————

// EffectSpawnType: SetParentOwner 부분 앞으로 정상적인 로직으로 될지 확인하고 (아마 될듯)
// 그리고 Enter, Exit Show Effect 부분 다시 참고해보고, 구조 조금 제대로 다듬고, DamageReductinoRate 적용하기
// EffectComponent::RemoveEffect 참고
// EffectBase::EnterShowEffect(); // --- +PARAM: ON TO THE PARENT(OWNER) ??

// 이전 패시브 버프 적용된거 제거하고 새로운 Value로 적용시키기.
// Seperatae EffectData - GlobalEffectData, HeroEffectData, MonsterEffect ???

// DamageReductionRate가 Infinite일 때랑 버프가 해제될때랑 스탯 적용 / 해제하는거 잡아야할듯.
// ----->>> Infinite Effect (이것부터 할까? 귀찮은데. DamageReductionRate부터 해보기)
// --- Dodge... (ExportHeroes에 DodgeHologram_Export받아 놨음. 이전 2DBase 프로젝트에서 Dodge 부분 참고)
// --- Invincible Count...
// --- Shield - Lightning
// --- Shield - Fire...
// --- Arhcer Muzzle Fire Effect...
// --- EFontAnimationType.EndTestAnim 폰트 애니메이션 추가할 거 남아있음...

// [NOTE]
// --- ObjectDisposedException: SerializedProperty 에러가 발생하면 채찍이 알려준 방법으로 적용해볼것.
// --- 단순한 인스펙터 에러로 추측되긴함.
// --- 유료 풀버전 부터 제작, 출시 -> 기능 제한 무료 버전 출시(+스테이지 오픈시에만 광고)
// --- Common, Uncommon, Rare...: 거기서 거기, Epic, Ultimate: 체감되는 효과
// --- Paladin: Endurance(5%, 12%, 20%, 30%, 40%)

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

// --- NOTE
1. Paladin
2. Archer
3. Lancer
4. Wizard
5. Assassin
6. Gunner
7. Tricster
8. Druid

*/