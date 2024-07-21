using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace STELLAREST_F1
{
    public static class Define
    {
        public enum ECreatureRarity
        {
            None = -1,
            Common,
            Elite,
            Max = Elite + 1
        }

        public enum ECollectEnvRarity
        {
            Common,
            Elite,
            Max = Elite + 1
        }

        public enum EHeroBodyType
        {
            Human,
            Skeleton1,
            Skeleton2,
            Undead,
            Demon
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
            PresetSize
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

        public enum ECreatureAIState // -- > ECreatureAnimMachineState
        {
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

        public enum EStatModType
        {
            None,
            Add,
            PercentAdd,
            PercentMulti,
            Max
        }

        public enum EAnimationCurveType
        {
            None = -1,
            Linear,
            Ease_In,      // 천천히 시작, 후반에 속도 증가
            Ease_Out,     // 빠르게 시작, 후반에 속도 감소
            Ease_In_Out,   // 천천히 시작, 중간에 속도 증가, 후반에 속도 감소
            Max = Ease_In_Out + 1
        }

        public enum ESkillType
        {
            None = -1,
            Skill_A,
            Skill_B,
            Skill_C,
            Max = Skill_C + 1
        }

        public enum ESkillTargetRange
        {
            None = -1,
            Self,
            Single,
            Half,
            AllAround
        }

        // ####################################################
        // Util.GetTypeFromClassName에 반드시 타입 추가
        public enum EClassName
        {
            DefaultSkill,
            Projectile,
            StraightMotion,
            ParabolaMotion,
            BodyAttack,
            CreatureAI,
            HeroAI,
            MeleeHeroAI,
            RangedHeroAI,
            MonsterAI
        }

        public enum EProjectileMotionType
        {
            None = -1,
            Parabola,
            Straight,
        }

        public enum EWeaponChildIndex
        {
            Socket = 0,
            ChildGroup = 1
        }

        public enum EAttachmentPoint
        {
            None = -1,
            WeaponL,
            WeaponLSocket,
            WeaponR,
            WeaponRSocket,
            Max = WeaponRSocket + 1
        }

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
            Fail_NoPath,
            Fail_MoveTo,
            Fail_ForceMovePingPongObject,
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
            Instant,
            Infinite,
            Buff,
            DeBuff,
            Dot,
            Airborne,
            Knockback,
            Freeze,
            Stun,
            Pull,
        }

        public enum EEffectSpawnType
        {
            None = -1,
            Internal,              // 지속시간이 있는 기본적인 이펙트 
            External,           // 외부(장판스킬)에서 이펙트를 관리(지속시간에 영향을 받지않음)
        }

        public enum EEffectClearType
        {
            VFXTimeOut,
            TimeOut,            // 단순 시간초과로 종료
            ClearSkill,         // 정화 스킬로 인한 종료
            TriggerOutAoE,      // AoE 스킬을 벗어난 종료
            EndOfCC,            // CC가 끝났을 때 종료
            Disable,
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
            Upper_Idle_To_Skill_A,
            Upper_Idle_To_Skill_B,
            Upper_Idle_To_Skill_C,
            Upper_Idle_To_CollectEnv,

            Upper_Move,
            Upper_Move_To_Skill_A,
            Upper_Move_To_Skill_B,
            Upper_Move_To_Skill_C,

            Upper_Dead,
            // Lower
            Lower_Idle,
            Lower_Move,

            Max = Lower_Move + 1
        }

        // ####################################################
        public static class ReadOnly
        {
            public static class Prefabs
            {
                public static readonly string PFName_LeaderController = "LeaderController";
                public static readonly string PFName_DamageFont = "DamageFont";
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
                public static readonly string HeroData = "HeroData";
                public static readonly string HeroSpriteData = "HeroSpriteData";
                public static readonly string MonsterData = "MonsterData";

                public static readonly string BirdSpriteData = "BirdSpriteData";
                public static readonly string QuadrupedsSpriteData = "QuadrupedsSpriteData";
                public static readonly string EnvData = "EnvData";
                public static readonly string TreeSpriteData = "TreeSpriteData";
                public static readonly string RockSpriteData = "RockSpriteData";
                public static readonly string StatData = "StatData";
                public static readonly string SkillData = "SkillData";
                public static readonly string ProjectileData = "ProjectileData";
                public static readonly string EffectData = "EffectData";
            }

            public static class DataAndPoolingID
            {
                // --- Damage Font
                public static readonly int DNPID_DamageFont = 109;

                public static readonly int DNPID_Hero_Paladin = 101000;
                public static readonly int DNPID_Hero_Archer = 101010;

                public static readonly int DNPID_Hero_Lancer = 101020;
                public static readonly int DNPID_Hero_Wizard = 101030;

                public static readonly int DNPID_Hero_Assassin = 101040;
                public static readonly int DNPID_Hero_Gunner = 101050;

                public static readonly int DNPID_Hero_Trickster = 101060;
                public static readonly int DNPID_Hero_Druid = 101070;

                public static readonly int DNPID_Hero_Barbarian = 101080;
                public static readonly int DNPID_Hero_Ninja = 101090;

                public static readonly int DNPID_Hero_PhantomKnight = 101100;
                public static readonly int DNPID_Hero_FrostWeaver = 101110;

                public static readonly int DNPID_Hero_Queen = 101120;
                public static readonly int DNPID_Hero_Hunter = 101130;

                public static readonly int DNPID_Hero_Gladiator = 101140;
                public static readonly int DNPID_Hero_Priest = 101150;

                public static readonly int DNPID_Hero_Berserker = 101160;
                public static readonly int DNPID_Hero_Witch = 101170;

                public static readonly int DNPID_Hero_DragonKnight = 101180;
                public static readonly int DNPID_Hero_Alchemist = 101190;
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
                public static readonly int DNPID_Monster_Chicken = 102000;
                public static readonly int DNPID_Monster_Turkey = 102001;
                public static readonly int DNPID_Monster_Bunny = 102002;
                public static readonly int DNPID_Monster_Pug = 102003;
                // --- Envs
                public static readonly int DNPID_Env_AshTree = 103000;
                public static readonly int DNPID_Env_BlackOakTree = 103001;
                public static readonly int DNPID_Env_GreenAppleTree = 103002;
                public static readonly int DNPID_Env_IvyTree = 103003;
                public static readonly int DNPID_Env_ManticoreTree = 103004;
                public static readonly int DNPID_Env_MapleTree = 103005;
                public static readonly int DNPID_Env_OakTree = 103006;
                public static readonly int DNPID_Env_RedAppleTree = 103007;
                public static readonly int DNPID_Env_RedSandalTree = 103008;
                public static readonly int DNPID_Env_WillowTree = 103009;
                public static readonly int DNPID_Env_YewTree = 103010;
                public static readonly int DNPID_Env_CopperRock = 103011;
                public static readonly int DNPID_Env_GoldRock = 103012;
                public static readonly int DNPID_Env_IronRock = 103013;
                public static readonly int DNPID_Env_LimestoneRock = 103014;
                public static readonly int DNPID_Env_SilverRock = 103015;
                public static readonly int DNPID_Env_StoneRock = 103016;
                public static readonly int DNPID_Env_TinRock = 103017;
                public static readonly int DNPID_Env_WhetstoneRock = 103018;
                public static readonly int DNPID_Env_ZincRock = 103019;

                // --- Effects(VFX)
                public static readonly int DNPID_Effect_TeleportRed = 301001;
                public static readonly int DNPID_Effect_TeleportGreen = 301002;
                public static readonly int DNPID_Effect_TeleportBlue = 301003;
                public static readonly int DNPID_Effect_Dust = 301004;
                public static readonly int DNPID_Effect_OnDeadSkull = 301005;
            }

            public static class AnimationParams
            {
                // --- States for State Machine
                // Upper Layers
                public static readonly string Upper_Idle = "Upper_Idle";
                public static readonly string Upper_Idle_To_Skill_A = "Upper_Idle_To_Skill_A";
                public static readonly string Upper_Idle_To_Skill_B = "Upper_Idle_To_Skill_B";
                public static readonly string Upper_Idle_To_Skill_C = "Upper_Idle_To_Skill_C";
                public static readonly string Upper_Idle_To_CollectEnv = "Upper_Idle_To_CollectEnv";

                public static readonly string Upper_Move = "Upper_Move";
                public static readonly string Upper_Move_To_Skill_A = "Upper_Move_To_Skill_A";
                public static readonly string Upper_Move_To_Skill_B = "Upper_Move_To_Skill_B";
                public static readonly string Upper_Move_To_Skill_C = "Upper_Move_To_Skill_C";

                public static readonly string Upper_Dead = "Upper_Dead";

                // Lower Layers
                public static readonly string Lower_Idle = "Lower_Idle";
                public static readonly string Lower_Idle_To_Skill_A = "Lower_Idle_To_Skill_A";
                public static readonly string Lower_Move = "Lower_Move";

                // --- Params
                public static readonly string IsMoving = "IsMoving";
                public static readonly string CanSkill = "CanSkill";
                public static readonly string OnSkill_A = "OnSkill_A";
                public static readonly string LowerIdleToSkillA = "LowerIdleToSkillA";                 

                public static readonly string OnSkill_B = "OnSkill_B";
                public static readonly string OnSkill_C = "OnSkill_C";
                public static readonly string OnCollectEnv = "OnCollectEnv";
                public static readonly string OnDead = "OnDead";
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
                public static readonly string DamageFontPoolingRootName = "@Pool_DamageFonts";
                public static readonly string EffectPoolingRootName = "@Pool_Effects";

                public static readonly string PreLoad = "PreLoad";
                public static readonly string AnimBody = "AnimationBody";
                public static readonly string AnimationBody = "AnimationBody";

                // --- Sprites
                public static readonly string Pickaxe_Common_SP = "Pickaxe_Common.sprite";
                public static readonly string Pickaxe_Elite_SP = "Pickaxe_Elite.sprite";
                public static readonly string WoodcutterAxe_Common_SP = "WoodcutterAxe_Common.sprite";
                public static readonly string WoodcutterAxe_Elite_SP = "WoodcutterAxe_Elite.sprite";
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
                public static readonly int HeroMaxLevel = 5;
                public static readonly int CanTryMaxSpawnCount = 999;


                public static readonly int RockElementsCount = 3;
                public static readonly int MaxActiveSkillsCount = 2;
                public static readonly int HeroDefaultMoveDepth = 10; // default: 5 -> 10
                public static readonly int HeroMaxMoveDepth = 100;
                public static readonly int CreatureWarpMoveDepth = 50;
                public static readonly int MonsterDefaultMoveDepth = 5; // default: 3 -> 5       
                public static readonly int MaxCanPingPongConditionCount = 20;

                public static readonly int ObjectScanRange = 6; // --- 대각선 상관없이 6칸

                // --- [ FLOATING ]
                // -- [ HERO ]
                public static readonly float CheckFarFromHeroesLeaderTick = 1.0F;
                public static readonly float CheckFarFromHeroesLeaderDistanceForWarp = 30.0F; // 15F(15칸, 상하좌우 기준) -> 30칸
                //public static readonly float WaitMovementDistanceSQRFromLeader = 2.4F;

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

