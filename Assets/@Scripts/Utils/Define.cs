
namespace STELLAREST_F1
{
    public static class Define
    {
        // --- 독립적인 게임 패키지
        public enum EGamePackages
        {
            ElitePack,                // 3,500 KRW
            PremiumPack,              // 12,000 KRW
            Max = PremiumPack + 1
        }

        public enum EHeroGrade
        {
            Default,
            Max
        }

        public enum EGameGrade
        {
            Common,         // white
            Rare,           // green
            Unique,         // purple
            Elite           // yellow
        }

        public enum EMonsterType
        {
            None = -1,
            Bird,
            Quadrupeds,
            //Max = Quadrupeds + 1
        }

        public enum EObjectSize
        {
            None = -1,
            VerySmall,
            Small,
            Medium,
            Large,
            VeryLarge,
            UsePreset,
        }

        public enum EScene
        {
            Unknown,
            TitleScene,
            GameScene
        }

        public enum EUIEvent
        {
            PointerClick,
            PointerDown,
            PointerUp,
            Drag
        }

        public enum EJoystickState
        {
            PointerDown,
            PointerUp,
            Drag
        }

        public enum ESound
        {
            Bgm,
            Effect,
            Max
        }

        public enum EObjectType
        {
            None = -1,
            Hero,
            Monster,
            Env,
            Projectile,
            Effect,
            Max = Effect + 1
        }

        public enum ECreatureAIState // --> ECreatureAnimMachineState
        {
            Manually, // ---> Leader
            Idle,
            Move,
            Dead,
            Max = Dead + 1
        }

        public enum ELookAtDirection
        {
            Left = -1,
            Right = 1,
        }

        public enum EHeroEmoji
        {
            None = -1,
            Idle,
            Move,
            Skill_A,
            Skill_B,
            Skill_C,
            CollectEnv,
            Sick,
            Dead,
            Max = Dead + 1
        }

        public enum EMonsterEmoji
        {
            None = -1,
            Normal,
            Angry,
            Dead,
            Max = Dead + 1
        }

        public enum EHeroBody
        {
            Head,
            UpperBody,
            LowerBody,
            Weapon,
        }

        public enum EHeroBody_Head
        {
            Head,
            Hair,
            Eyebrows,
            Eyes,
            Mouth,
            Ears,
            Earrings,
            Beard,
            Mask,
            Glasses,
            Helmet,
            Max = Helmet + 1
        }

        public enum EHeroBody_Upper
        {
            Torso,
            Torso_Armor,
            Cape_Armor,
            ArmL,
            ArmL_Armor,
            ForearmL,
            ForearmL_Armor,
            HandL,
            HandL_Armor,
            Finger,
            Finger_Armor,
            ArmR,
            ArmR_Armor,
            ForearmR,
            ForearmR_Armor,
            SleeveR_Armor,
            HandR,
            HandR_Armor,
            Max = HandR_Armor + 1
        }

        public enum EHeroBody_Lower
        {
            Pelvis,
            Pelvis_Armor,
            LegL,
            LegL_Armor,
            ShinL,
            ShinL_Armor,
            LegR,
            LegR_Armor,
            ShinR,
            ShinR_Armor,
            Max = ShinR_Armor + 1
        }

        public enum EHeroBody_Weapon
        {
            WeaponL_Armor,
            WeaponL_FireSocket,
            WeaponL_ChildsRoot,
            WeaponL_Armor_Child01,
            WeaponL_Armor_Child02,
            WeaponL_Armor_Child03,
            WeaponR_Armor,
            WeaponR_FireSocket,
            WeaponR_ChildsRoot,
            WeaponR_Armor_Child01,
            WeaponR_Armor_Child02,
            WeaponR_Armor_Child03,
            Max = WeaponR_Armor_Child03 + 1
        }

        public enum EHeroWeapons
        {
            WeaponL_Armor,
            WeaponL_Armor_Child01,
            WeaponL_Armor_Child02,
            WeaponL_Armor_Child03,
            WeaponR_Armor,
            WeaponR_Armor_Child01,
            WeaponR_Armor_Child02,
            WeaponR_Armor_Child03,
            Max = WeaponR_Armor_Child03 + 1
        }

        public enum EBirdBody
        {
            Body,
            Head,
            Wing,
            LegL,
            LegR,
            Tail,
        }

        public enum EQuadrupedsBody
        {
            Body,
            Head,
            LegFrontL,
            LegFrontR,
            LegBackL,
            LegBackR,
            Tail,
        }

        public enum ETreeBody
        {
            Trunk,
            Patch,
            Stump,
            EndParticle,
            Fruits_ChildsRoot,
            Fruits_Child_01,
            Fruits_Child_02,
            Fruits_Child_03,
            Shadow,
        }

        public enum ERockBody
        {
            Rock,
            Empty,
            Ore,
            OreShadow,
            OreLight,
            OreParticle,
            EndParticle,
            Spot1,
            Spot2,
            Spot3,
            Fragment1,
            Fragment2,
            Fragment3,
            Shadow,
        }

        public enum EEnvType
        {
            None = -1,
            Tree,
            Rock,
            Max = Rock + 1
        }

        public enum EEnvState
        {
            None,
            Idle,
            Dead,
        }

        public enum ELayer
        {
            None = -1,
            Default = 0,
            Transparent = 1,
            IgnoreRaycast = 2,
            Dummy1 = 3,
            Water = 4,
            UI = 5,
            Hero = 6,
            Monster = 7,
            Env = 8,
            Obstacle = 9,
            Projectile = 10,
            Max,
        }

        public enum EAnimationCurveType
        {
            None = -1,
            Linear,
            Ease_In,      // 천천히 시작, 후반에 속도 증가
            Ease_Out,     // 빠르게 시작, 후반에 속도 감소
            Ease_In_Out,   // 천천히 시작, 중간에 속도 증가, 후반에 속도 감소
            Max = Ease_In_Out
        }

        public enum ESkillType
        {
            None = -1,
            Skill_A,        // Default
            Skill_B,        // Active_1
            Skill_C,        // Active_2
            Max = Skill_C + 1
        }

        public enum ESkillElementType
        {
            None = -1,
            Fire,
            Ice,
            Poision,
        }

        // ********************************************************************************
        // Melee는 완료했으므로 일단 이거 세개(Single, Half, Around) Projectile 부분 정의 완료하기
        // 그 다음에 모양을 더 추가하던지 (예시, XShaped)
        public enum ESkillTargetRange
        {
            None = -1,
            Single = 1,
            Half,
            Around
        }
        // ********************************************************************************
        public enum ETargetDirection
        {
            Horizontal,
            VerticalUp,     // --- Added Up, Down
            VerticalDown,   // --- Added Up, Down
            DiagonalUp,     // --- Added Up, Down
            DiagonalDown,   // --- Added Up, Down
            Max = DiagonalDown + 1
        }


        // ####################################################
        // Util.GetTypeFromClassName에 반드시 타입 추가
        public enum EClassName
        {
            DefaultSkillBase,
            ActiveSkillBase,
            Projectile,
            StraightMotion,
            ParabolaMotion,
            BodyAttack,
            CreatureAI,
            HeroAI,
            MeleeHeroAI,
            RangedHeroAI,
            MonsterAI,
        }

        public enum EProjectileMotionType
        {
            None = -1,
            Straight = 1,
            Parabola = 2,
        }

        public enum EWeaponChildIndex
        {
            Socket = 0,
            ChildGroup = 1
        }

        // public enum EAttachmentPoint
        // {
        //     None = -1,
        //     WeaponL,
        //     WeaponLSocket,
        //     WeaponR,
        //     WeaponRSocket,
        //     Max = WeaponRSocket + 1
        // }

        public enum ECellCollisionType
        {
            CanMove,
            SemiBlock,
            Block,
        }

        public enum EFindPathResult
        {
            None = -1,
            Fail_LerpCell,
            // Fail_NoPath,
            Fail_MoveTo,
            Fail_ForceMove,
            Success
        }

        public enum EHeroMemberFormationMode
        {
            FollowLeaderClosely,
            NarrowFormation = 3,
            WideFormation = 5,
            RandomFormation,
            ForceStop
        }

        // --- Main
        // Health:
        // Damage:
        // MoveSpeed: 동료들은 Leader의 스피드를 따라간다.
        // AttackSpeed: 
        // 

        // --- Sub
        // BonueHealth (Not Showing in UI)
        // Armor
        // Critical Rate
        // Dodge
        public enum EEffectType
        {
            None = -1,
            
            // VFX
            VFX_Base,
            VFX_WindBlade,
            VFX_ShieldBlue,
            VFX_BonusHealth, // --- TEMP

            // BUFF BASE STATS
            Buff_BaseStat_MaxHealth,
            Buff_BaseStat_Damage,
            Buff_BaseStat_AttackRate,
            Buff_BaseStat_MovementSpeed,

            // BUFF SUB STATS
            // 둘중에 하나라면 어쩔...?
            Buff_SubStat_BonusHealth, // 그 다음에는 이거 (초록색 -> 노란색 체력바)
            Buff_SubStat_BonusHealthShield, // 이것부터 처리하고 (회색 또는 하늘색 체력바)
            Buff_SubStat_Armor,
            Buff_SubStat_CriticalRate,
            Buff_SubStat_DodgeRate,
            Buff_SubStat_Luck,
            Buff_SubStat_InvincibleBlockCountPerWave,

            // DEBUFF BASE STATS
            Debuff_BaseStat_Example01,

            // DEBUFF SUB STATS
            Debuff_SubStat_Example01,

            // DOT
            Dot_Example01,

            // CC
            CC_Airborne,
            CC_Knockback,
            CC_Freeze,
            CC_Stun,
            CC_Pull,

            Max = CC_Pull + 1
        }

        public enum EEffectSpawnType
        {
            None = -1,
            SetParentOwner = 1,
            SkillFromOwner = 2,
            SkillFromTarget = 3
        }

        public enum EEffectClearType
        {
            TimeOut,
            ByCondition,
        }

        public enum EStatModType
        {
            AddAmount,
            AddPercent,
            AddPercentMulti
        }

        public enum EHeroWeaponType
        {
            Default,
            CollectTree,
            CollectRock
        }

        // new
        public enum ECreatureAnimState
        {
            // Upper
            None = -1,

            Upper_Idle,
            Upper_Move,
            Upper_SkillA,
            Upper_SkillB,
            Upper_SkillC,
            Upper_CollectEnv,
            // --- 나중에 Sick, Healthy 추가
            Upper_Dead,
            Max = Upper_Dead + 1

            // --- Prev
            // Lower
            // Lower_Idle,
            // Lower_Move,
            //Max = Lower_Move + 1
        }

        // --- Font: InGame Font
        // --- FontUI: UI Font
        public enum EFontAnimationType
        {
            EndGoingUp,
            EndSmaller,
            EndFalling,
            EndFallingShake,
            EndBouncingLeftUp,
            EndBouncingRightUp,
            EndTestAnim,
        }

        public enum EFontSignType
        {
            None,
            Plus,
            Minus
        }

        public enum EFontAssetType
        {
            MapleBold,
            Comic,
            Max,
        }

        public enum EGlobalEffectID
        {
            ImpactHit = 900000,
            ImpactCriticalHit = 900001,
            ImpactFire = 900002,
            ImpactShockwave = 900003,

            TeleportRed = 900020,
            TeleportGreen = 900021,
            TeleportBlue = 900022,
            TeleportPurple = 900023,
            
            Dust = 990000,
            OnDeadSkull = 990001,
            EvolutionGlow = 990002
        }

        public enum EGlobalProjectileID
        {
        }

        // ####################################################
        public static class ReadOnly
        {
            public static class ColorCode
            {
                public static readonly string BrightRed = "#EE4B2B";
                public static readonly string BrightBlue = "##0096FF";
                public static readonly string BrightGreen = "#AAFF00";
            }

            public static class Prefabs
            {
                public static readonly string PFName_LeaderController = "LeaderController";
                public static readonly string PFName_TextFontBase = "TextFontBase";

                // public static readonly string PFName_DamageFont = "DamageFont";
                // public static readonly string PFName_CriticalFont = "CriticalFont";
                public static readonly string PFName_EffectBase = "EffectBase";
            }

            public static class Materials
            {
                public static readonly string Mat_Default = "Default.mat";
                public static readonly string Mat_EyesPaint = "EyesPaint.mat";
                public static readonly string Mat_StrongTint = "StrongTint.mat";

                public static readonly string Mat_RockFragments = "RockFragments.mat";
                public static readonly string Mat_Glow = "Glow.mat";
            }

            public static class DataSet
            {
                // --- Heroes Data
                public static readonly string HeroData = "HeroData";
                public static readonly string HeroSpriteData = "HeroSpriteData";
                public static readonly string HeroStatData = "HeroStatData";
                public static readonly string HeroSkillData = "HeroSkillData";
                public static readonly string HeroEffectData = "HeroEffectData";

                // --- Monsters Data
                public static readonly string MonsterData = "MonsterData";
                public static readonly string MonsterBirdSpriteData = "MonsterBirdSpriteData";
                public static readonly string MonsterQuadrupedSpriteData = "MonsterQuadrupedSpriteData";
                public static readonly string MonsterStatData = "MonsterStatData";
                public static readonly string MonsterSkillData = "MonsterSkillData";
                public static readonly string MonsterEffectData = "MonsterEffectData";

                // --- Envs Data
                public static readonly string EnvData = "EnvData";
                public static readonly string EnvTreeSpriteData = "EnvTreeSpriteData";
                public static readonly string EnvRockSpriteData = "EnvRockSpriteData";
                public static readonly string EnvEffectData = "EnvEffectData";

                // --- Global Data
                public static readonly string EffectData = "EffectData";
                public static readonly string ProjectileData = "ProjectileData";
            }

            public static class DataAndPoolingID
            {
                // --- Damage Font
                public static readonly int DNPID_DamageFont = 109;
                public static readonly int DNPID_TextFont = 110;

                public static readonly int DNPID_Hero_Paladin = 101000;
                public static readonly int DNPID_Hero_Archer = 102000;

                public static readonly int DNPID_Hero_Lancer = 103000;
                public static readonly int DNPID_Hero_Wizard = 104000;

                public static readonly int DNPID_Hero_Assassin = 105000;
                public static readonly int DNPID_Hero_Gunner = 106000;

                public static readonly int DNPID_Hero_Trickster = 107000;
                public static readonly int DNPID_Hero_Druid = 108000;

                public static readonly int DNPID_Hero_Barbarian = 109000;
                public static readonly int DNPID_Hero_Ninja = 110000;

                public static readonly int DNPID_Hero_PhantomKnight = 111000;
                public static readonly int DNPID_Hero_FrostWeaver = 112000;

                public static readonly int DNPID_Hero_Queen = 113000;
                public static readonly int DNPID_Hero_Hunter = 114000;

                public static readonly int DNPID_Hero_Gladiator = 115000;
                public static readonly int DNPID_Hero_Priest = 116000;

                public static readonly int DNPID_Hero_Berserker = 117000;
                public static readonly int DNPID_Hero_Witch = 118000;

                public static readonly int DNPID_Hero_DragonKnight = 119000;
                public static readonly int DNPID_Hero_Alchemist = 120000;
                // ---------------------------------------------------------------------- // 20 Heroes
                // + Monk
                // + Blood Mage

                // + Black Smith
                // + Elemental Master

                // + Guardian
                // + 

                // + Skeleton King
                // + Pirate (Common: Melee, Elite: Ranged)

                // + Mutant
                // + Necromancer
                // ---------------------------------------------------------------------- // 30 Heroes

                // --- Monsters
                public static readonly int DNPID_Monster_Chicken = 101000;
                public static readonly int DNPID_Monster_Turkey = 101001;
                public static readonly int DNPID_Monster_Bunny = 101002;
                public static readonly int DNPID_Monster_Pug = 101003;

                // --- Envs
                public static readonly int DNPID_Env_AshTree = 101000;
                public static readonly int DNPID_Env_BlackOakTree = 101001;
                public static readonly int DNPID_Env_GreenAppleTree = 101002;
                public static readonly int DNPID_Env_IvyTree = 101003;
                public static readonly int DNPID_Env_ManticoreTree = 101004;
                public static readonly int DNPID_Env_MapleTree = 101005;
                public static readonly int DNPID_Env_OakTree = 101006;
                public static readonly int DNPID_Env_RedAppleTree = 101007;
                public static readonly int DNPID_Env_RedSandalTree = 101008;
                public static readonly int DNPID_Env_WillowTree = 101009;
                public static readonly int DNPID_Env_YewTree = 101010;
                public static readonly int DNPID_Env_CopperRock = 101011;
                public static readonly int DNPID_Env_GoldRock = 101012;
                public static readonly int DNPID_Env_IronRock = 101013;
                public static readonly int DNPID_Env_LimestoneRock = 101014;
                public static readonly int DNPID_Env_SilverRock = 101015;
                public static readonly int DNPID_Env_StoneRock = 101016;
                public static readonly int DNPID_Env_TinRock = 101017;
                public static readonly int DNPID_Env_WhetstoneRock = 101018;
                public static readonly int DNPID_Env_ZincRock = 101019;

                // --- Effects(VFX): DEPRECIATED
                // public static readonly int DNPID_Effect_Global_VFX_ImpactHit = 800000;
                // public static readonly int DNPID_Effect_Global_VFX_ImpactCriticalHit = 800001;
                // public static readonly int DNPID_Effect_Global_VFX_ImpactFire = 800002;
                // public static readonly int DNPID_Effect_Global_VFX_ImpactShockwave = 800003;
                // public static readonly int DNPID_Effect_Global_VFX_TeleportRed = 800010;     // --- ENV ROCK
                // public static readonly int DNPID_Effect_Global_VFX_TeleportGreen = 800011;   // --- ENV TREE
                // public static readonly int DNPID_Effect_Global_VFX_TeleportBlue = 800012;    // --- HERO + PET(Allies, Etc)
                // public static readonly int DNPID_Effect_Global_VFX_TeleportPurple = 800013;  // --- MONSTER
                // public static readonly int DNPID_Effect_Global_VFX_Dust = 900000;
                // public static readonly int DNPID_Effect_Global_VFX_OnDeadSkull = 900001;
                // public static readonly int DNPID_Effect_Global_VFX_EvolutionGlow = 900002;
            }

            public static class AnimationParams
            {
                // --- Upper Anim States (New)
                public static readonly string Upper_Idle = "Upper_Idle";
                public static readonly string Upper_Move = "Upper_Move";
                public static readonly string Upper_SkillA = "Upper_SkillA";
                public static readonly string Upper_SkillB = "Upper_SkillB";
                public static readonly string Upper_SkillC = "Upper_SkillC";
                public static readonly string Upper_CollectEnv = "Upper_CollectEnv";
                public static readonly string Upper_Dead = "Upper_Dead";
                public static readonly string Lower_Idle = "Lower_Idle";
                public static readonly string Lower_Move = "Lower_Move";

                // --- Params
                public static readonly string IsMoving = "IsMoving";
                public static readonly string CanSkill = "CanSkill";
                public static readonly string OnSkillA = "OnSkillA";
                public static readonly string OnSkillB = "OnSkillB";
                public static readonly string OnSkillC = "OnSkillC";
                public static readonly string OnCollectEnv = "OnCollectEnv";
                public static readonly string OnDead = "OnDead";
                public static readonly string AttackRate = "AttackRate";
            }

            public static class SortingLayers
            {
                public static readonly string SLName_BaseObject = "BaseObject";
                public static readonly int SLOrder_Terrain = 0;
                public static readonly int SLOrder_Deco = 10;
                public static readonly int SLOrder_BaseObject = 20;
                public static readonly int SLOrder_Projectile = 30;
                public static readonly int SLOrder_UI = 90;
                public static readonly int SLOrder_Effect = 100;
                public static readonly int SLOrder_DamageFont = 200;
            }

            public static class Util
            {
                public const char Map_Tool_Block_0 = '0';
                public const char Map_Tool_CanMove_1 = '1';
                public const char Map_Tool_SemiBlock_2 = '2';

                public static readonly string Managers = "@Managers";
                public static readonly string UI_Root = "@UI_Root";
                public static readonly string EventSystem = "@EventSystem";
                public static readonly string HeroPoolingRootName = "@Pool_Heroes";
                public static readonly string MonsterPoolingRootName = "@Pool_Monsters";
                public static readonly string EnvPoolingRootName = "@Pool_Envs";
                public static readonly string ProjectilePoolingRootName = "@Pool_Projectiles";
                //public static readonly string DamageFontPoolingRootName = "@Pool_DamageFonts";
                public static readonly string TextFontPoolingRootName = "@Pool_TextFonts";
                public static readonly string EffectPoolingRootName = "@Pool_Effects";

                public static readonly string PreLoad = "PreLoad";
                public static readonly string AnimBody = "AnimationBody";
                public static readonly string AnimationBody = "AnimationBody";

                // --- Sprites
                public static readonly string Pickaxe_Default_SP = "Pickaxe_Default.sprite";
                public static readonly string Pickaxe_Max_SP = "Pickaxe_Max.sprite";
                public static readonly string WoodcutterAxe_Default_SP = "WoodcutterAxe_Default.sprite";
                public static readonly string WoodcutterAxe_Max_SP = "WoodcutterAxe_Max.sprite";
                public static readonly string Shadow_SP = "Shadow.sprite";

                public static readonly string EnvRock_Rock_SP = "StoneRock_Rock.sprite";
                public static readonly string EnvRoc_Empty_SP = "StoneRock_Empty.sprite";

                public static readonly string Light_SP = "Light.sprite";

                // // Env Body
                // public static readonly string ETreeBody_Trunk = "Trunk";
                // public static readonly string ETreeBody_Patch = "Patch";
                // public static readonly string ETreeBody_Stump = "Stump";
                // public static readonly string ETreeBody_Fruits = "Fruits";

                // public static readonly string ERock_Rock = "Rock";
                // public static readonly string ERock_Empty = "Empty";
                // public static readonly string ERock_Ore = "Ore";
                // public static readonly string ERock_OreShadow = "OreShadow";
                // public static readonly string ERock_OreLight = "OreLight";
                // public static readonly string ERock_OreParticle = "OreParticle";
                // public static readonly string ERock_Spot = "Spot";
                // public static readonly string ERock_Fragment = "Fragment";

                // public static readonly string EBody_EndParticle = "EndParticle";
                // public static readonly string EBody_Shadow = "Shadow";

                // Write Tile
                public static readonly string Tile_CanMove = "_CanMove";
                public static readonly string Tile_SemiBlock = "_SemiBlock";
                public static readonly string Tile_Block = "_Block";

                // Map
                public static readonly string Tilemap_Collision = "Tilemap_Collision";

                // [ INTEGER ]
                public static readonly int HeroMaxLevel = 7;
                public static readonly int EliteHeroMaxLevel = 8;
                public static readonly int CanTryMaxSpawnCount = 999;

                public static readonly int RockElementsCount = 3;
                public static readonly int MaxActiveSkillsCount = 2;
                public static readonly int HeroDefaultMoveDepth = 20; // default: 5 -> 10 -> 20
                public static readonly int HeroMaxMoveDepth = 100;
                public static readonly int CreatureWarpMoveDepth = 50;
                public static readonly int MonsterDefaultMoveDepth = 20; // default: 3 -> 5 -> 20       
                public static readonly int MaxCanPingPongConditionCount = 20;

                public static readonly int ObjectScanRange = 6; // --- 대각선 상관없이 6칸

                public static readonly int ScanEnemyRange = 6;
                public static readonly int ScanAllyRange = ScanEnemyRange / 2;

                // --- [ FLOATING ]
                public static readonly float CoForceWaitTime = 2.5F;
                public static readonly float CheckFarFromHeroesLeaderTick = 1.0F;
                public static readonly float CheckFarFromHeroesLeaderDistanceForWarp = 30.0F; // 15F(15칸, 상하좌우 기준) -> 30칸
                //public static readonly float WaitMovementDistanceSQRFromLeader = 2.4F;
                public static readonly float MaxArmor = 0.8F;

                public static readonly float MinSecPatrolPingPong = 1.0F;
                public static readonly float MaxSecPatrolPingPong = 2.0F;

                public static readonly float WaitHeroesForceStopWarpSeconds = 30.0F;

                // -- [ HERO LEADER CONTROLLER ]
                public static readonly float DesiredCanChangeLeaderTime = 3F;

                // -- [ MONSTER ]
                public static readonly float MinSecWaitSearchTargetForSettingAggroFromRange = 1.0F;
                public static readonly float MaxSecWaitSearchTargetForSettingAggroFromRange = 2.0F;

                // -- [ MISC ]
                public static readonly float ObjectScanTick = 0.1F;
                //public static readonly float FindTargetTick = 0.5F;
                //public static readonly float SearchFindTargetTick = 0.5F;

                public static readonly float CamOrthoSize = 12.0F;
                public static readonly float JoystickFocusMinDist = -0.18F;
                public static readonly float JoystickFocusMaxDist = 0.18F;

                public static readonly float HeroDefaultScanRange = 8.0F; // 오리지날 6F, 일단 6칸
                public static readonly float MonsterDefaultScanRange = 6.0F; // 상하좌우 한칸 기준, 대각선X

                public static readonly float Temp_StopDistance = 1.25F;

                // Dead Fade Out Time
                public static readonly float StartDeadFadeOutTime = 0.85F; // --- PREV
                public static readonly float DesiredDeadFadeOutEndTime = 1.0F; // --- PREV

                public static readonly float DesiredEndFadeInTime = 0.5F;
                public static readonly float DesiredStartFadeOutTime = 2.0F;
                public static readonly float DesiredEndFadeOutTime = 1.0F;

                public static readonly float MaxMovementSpeedMultiplier = 2.0F;
                public static readonly float MaxDistanceForMovementSpeed = 8.0F;

                // CameraController
                public static readonly float CamDesiredMoveToTargetTime = 0.75f;

                // STILL NOT TESTED ##########
                public static readonly float CalcValueMaxDistanceMultiplier = 2.0F;

                // ##############################
                public static readonly float ProjectileLifeTime = 10.0F;
            }
        }
    }
}

