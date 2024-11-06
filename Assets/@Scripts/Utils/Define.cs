
using System;
using Unity.Burst.CompilerServices;

namespace STELLAREST_F1
{
    public static class Define
    {
        // --- 독립적인 게임 패키지
        // ---> Apple Arcade로 변경
        // public enum EGamePackages
        // {
        //     ElitePack,                // 3,500 KRW
        //     PremiumPack,              // 12,000 KRW
        //     Max = PremiumPack + 1
        // }
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

        public enum EEffectType
        {
            None = -1,
            
            // VFX
            VFX_Base,
            VFX_WindBlade,
            VFX_ShieldBlue,
            VFX_BonusHealth,

            // BUFF STATS
            MainStat_MaxHealth,
            MainStat_Damage,
            MainStat_AttackRate,
            MainStat_MovementSpeed,

            SubStat_Shield,
            SubStat_BonusHealth,
            SubStat_Armor,
            SubStat_Critical,
            SubStat_Dodge,
            SubStat_Luck,
            SubStat_InvincibleBlockCountPerWave,

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
            Manually,
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

        public enum EInt
        {
            // --- GlobalEffectID
            ID_ImpactHit,
            ID_ImpactCriticalHit,
            ID_ImpactFire,
            ID_ImpactShockwave,
            ID_TeleportRed,
            ID_TeleportGreen,
            ID_TeleportBlue,
            ID_TeleportPurple,
            ID_Dust,
            ID_OnDeadSkull,
            ID_EvolutionGlow,

            // --- TextFontID
            ID_DamageFont,
            ID_TextFont,

            // --- HeroDataID
            ID_Paladin,
            ID_Archer,
            ID_Lancer,
            ID_Wizard,
            ID_Assassin,
            ID_Gunner,
            ID_Trickster,
            ID_Druid,
            ID_Barbarian,
            ID_Ninja,
            ID_PhantomKnight,
            ID_FrostWeaver,
            ID_Queen,
            ID_Hunter,
            ID_Gladiator,
            ID_Priest,
            ID_Berserker,
            ID_Witch,
            ID_DragonKnight,
            ID_Alchemist,

            // --- MonsterDataID
            ID_Chicken,
            ID_Turkey,
            ID_Bunny,
            ID_Pug,

            // --- EnvDataID
            ID_AshTree,
            ID_BlackOakTree,
            ID_GreenAppleTree,
            ID_IvyTree,
            ID_ManticoreTree,
            ID_MapleTree,
            ID_OakTree,
            ID_RedAppleTree,
            ID_RedSandalTree,
            ID_WillowTree,
            ID_YewTree,
            ID_CopperRock,
            ID_GoldRock,
            ID_IronRock,
            ID_LimestoneRock,
            ID_SilverRock,
            ID_StoneRock,
            ID_TinRock,
            ID_WhetstoneRock,
            ID_ZincRock,

            // --- Sorting
            Sorting_Terrain,
            Sorting_Deco,
            Sorting_BaseObject,
            Sorting_Projectile,
            Sorting_UI,
            Sorting_Effect,
            Sorting_DamageFont,

            // --- Constant Value
            CValue_ScanRange,
            CValue_HeroMaxLevel,
            CValue_CreatureMoveDepth,
            CValue_TryFindingPathMaxCount,
        }

        public enum EFloat
        {            
            // --- MinMax Range
            Range_AttackRate,
            Range_AttackAnimRate,

            Range_CollectRate,
            Range_CollectAnimRate,

            Range_MovementSpeed,
            Range_MovementAnimSpeed,

            Range_Armor,
            Range_Critical,
            Range_Dodge,
            Range_Luck,

            Range_HeroesRandFormationDist,
            Range_JoystickFocusDist,

            // --- Constant Value
            CValue_CriticalDamageUpRate,
            CValue_ForceWaitTime,
            CValue_FarFromLeaderHeroTick,
            CValue_ForceWaitTimeForStopWarp,
            CValue_ChangeLeaderCoolTime,
            CValue_FindTargetsTick,
            CValue_CameraOrthoSize,
            CValue_MaxMovementSpeedByDist,
            CValue_FadeInTime,
            CValue_StartFadeOutWaitTime,
            CValue_FadeOutTime,
            CValue_CameraMoveToTargetTime,
        }

        public enum EString
        {
            // --- Labels
            Prefab_LeaderController,
            Prefab_TextFontBase,

            // --- Datas
            Data_Hero,
            Data_HeroSprite,
            Data_HeroSkill,
            Data_HeroEffect,

            Data_Monster,
            Data_MonsterBirdSprite,
            Data_MonsterQuadrupedSprite,
            Data_MonsterSkill,
            Data_MonsterEffect,

            Data_Env,
            Data_EnvTreeSprite,
            Data_EnvRockSprite,
            Data_EnvEffect,

            Data_Effect,
            Data_Projectile,

            // --- Anim States
            AnimState_Upper_Idle,
            AnimState_Upper_Move,
            AnimState_Upper_SkillA,
            
            AnimState_Upper_SkillB,
            AnimState_Upper_SkillB_Elite,

            AnimState_Upper_SkillC,
            AnimState_Upper_CollectEnv,
            AnimState_Upper_Dead,

            // --- Anim Params
            AnimParam_IsMoving,
            AnimParam_IsEliteMax,
            AnimParam_CanSkill,
            AnimParam_OnSkillA,
            AnimParam_OnSkillB,
            AnimParam_OnSkillC,
            AnimParam_OnCollectEnv,
            AnimParam_OnDead,
            AnimParam_AttackRate,
            AnimParam_CollectRate,
            AnimParam_MovementSpeed,

            // --- Materials
            Mat_Default,
            Mat_EyesPaint,
            Mat_StrongTint,
            Mat_RockFragments,
            Mat_Glow,

            // --- Sprites
            Sprite_DefaultWoodcutterAxe,
            Sprite_MaxWoodcutterAxe,
            Sprite_DefaultPickaxe,
            Sprite_MaxPickaxe,
            Sprite_RockBodyFrame,
            Sprite_RockEmptyFrame,
            Sprite_Shadow,
            Sprite_CircleLight,

            // --- Objects
            Obj_Managers,
            Obj_UIRoot,
            Obj_EventSystem,
            Obj_HeroesRoot,
            Obj_MonstersRoot,
            Obj_EnvsRoot,
            Obj_ProjectilesRoot,
            Obj_TextFontsRoot,
            Obj_EffectsRoot
        }
    }
}

