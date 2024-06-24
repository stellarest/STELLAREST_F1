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
            Common,
            Elite,
            Special,
            Boss
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
            Max = Quadrupeds + 1
        }

        public enum EMonsterSize
        {
            None = -1,
            Small,
            Medium,
            Large
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
            LeaderController,
            Max = LeaderController + 1
        }

        public enum ECreatureAIState // -- > ECreatureAnimMachineState
        {
            Idle,
            Move,
            Dead,
            Max = Dead + 1
        }

        // public enum ECreatureUpperAnimState // --> ECreatureUpperAnimState
        // {
        //     None = -1,
        //     UA_Idle,
        //     UA_MoveStart,
        //     UA_Move,
        //     UA_MoveEnd,
        //     UA_Skill_Attack,
        //     Max
        // }

        // public enum ECreatureLowerAnimState // --> ECreatureLowerAnimState
        // {
        //     None = -1,
        //     LA_Idle,
        //     LA_Move,
        //     Max
        // }

        // public enum ECreatureMoveState
        // {
        //     None,
        //     //MoveToTarget,
        //     Move,
        //     ForceMove,
        // }

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
            Default,
            Angry,
            Dead,
            Max = Dead + 1
        }

        public enum EHeroBodyParts
        {
            Head,
            UpperBody,
            LowerBody,
            Weapon,
        }

        public enum EHeroHead
        {
            None = -1,
            HeadSkin,
            Hair,
            Eyes,
            Eyebrows,
            Mouth,
            Ears,
            Earrings,
            Beard,
            Mask,
            Glasses,
            Helmet,
            Max = Helmet + 1
        }

        public enum EHeroUpperBody
        {
            None = -1,
            TorsoSkin,
            Torso,
            Cape,
            ArmLSkin,
            ArmL,
            ForearmLSkin,
            ForearmL,
            HandLSkin,
            HandL,
            FingerSkin,
            Finger,
            ArmRSkin,
            ArmR,
            ForearmRSkin,
            ForearmR,
            SleeveR,
            HandRSkin,
            HandR,
            Max = HandR + 1
        }

        public enum EHeroLowerBody
        {
            None = -1,
            PelvisSkin,
            Pelvis,
            LegLSkin,
            LegL,
            ShinLSkin,
            ShinL,
            LegRSkin,
            LegR,
            ShinRSkin,
            ShinR,
            Max = ShinR + 1
        }

        public enum EHeroWeapon
        {
            None = -1,
            WeaponL,
            WeaponLSocket,
            WeaponLChildGroup,
            WeaponR,
            WeaponRSocket,
            WeaponRChildGroup,
            Max = WeaponRChildGroup + 1
        }

        public enum EBirdBodyParts
        {
            None = -1,
            Body,
            Head,
            Wing,
            LegL,
            LegR,
            Tail,
            Max = Tail + 1
        }

        public enum EQuadrupedsParts
        {
            None = -1,
            Body,
            Head,
            LegFrontL,
            LegFrontR,
            LegBackL,
            LegBackR,
            Tail,
            Max = Tail + 1
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
            OnDamaged,
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

        public enum EColliderSize
        {
            Small,
            Default,
            Large
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

        // ####################################################
        // Util.GetTypeFromClassName에 반드시 타입 추가
        public enum EClassName
        {
            MeleeAttack,
            RangedAttack,
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
            StraightMotion,
            ParabolaMotion,
            Max = ParabolaMotion + 1
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

        // // 제거 예정. FollowLeader 모드만 제대로 만들어놓으면 될듯.
        // public enum EHeroMemberChaseMode
        // {
        //     FollowLeader, // --- Default, Skill_A,B 제외 Drag시 리더 추적, 리더가 공격하면 리더를 따라서 타겟을 찾고 공격. 리더가 채집하면 채집함. 이렇게?
        //     EngageTarget, // --- 리더가 이동하지 않고 있을 때 타겟 우선 (제거할 수도 있음. 필요 없는듯.)
        //     // --- 아니면 진짜 극단적으로 리더가 이동해도 타겟만 쫓아가는 방식으로. 이거 고민.
        // }

        public enum EProjectileSize
        {
            Small,
            Medium,
            Large
        }

        public enum EEffectType
        {
            None,
            Instant,
            Buff,
            DeBuff,
            Dot,
            Airborne,
            Knockback,
            Freeze,
            Stun,
        }

        public enum EEffectSpawnType
        {
            None,
            Skill,// 지속시간이 있는 기본적인 이펙트 
            External, // 외부(장판스킬)에서 이펙트를 관리(지속시간에 영향을 받지않음
        }

        public enum EEffectClearType
        {
            None,
            TimeOut,
            ClearSKill,
            TriggerOutAoE,
            EndOfCC,
            Disable,
        }

        public enum EHeroStateWeaponType
        {
            Default,
            EnvTree,
            EnvRock
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
            public static class DataAndPoolingID
            {
                // --- Damage Font
                public static readonly int DNPID_DamageFont = 109;
                // --- Heroes
                public static readonly int DNPID_Hero_Paladin = 101000;
                public static readonly int DNPID_Hero_Archer = 101010;
                public static readonly int DNPID_Hero_Wizard = 101020;
                public static readonly int DNPID_Hero_Lancer = 101030;
                public static readonly int DNPID_Hero_Gunner = 101040;
                public static readonly int DNPID_Hero_Assassin = 101050;
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
            }

            public static class Prefabs
            {
                public static readonly string PFName_DamageFont = "DamageFont";
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
                public static readonly string Lower_Move = "Lower_Move";

                // --- Params
                public static readonly string IsMoving = "IsMoving";
                public static readonly string CanSkill = "CanSkill";
                public static readonly string OnSkill_A = "OnSkill_A";
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
                public static readonly int SLOrder_UI = 99;
                public static readonly int SLOrder_DamageFont = 100;
            }
            
            public static class Character
            {
                public const char Map_Tool_Block_0 = '0';
                public const char Map_Tool_CanMove_1 = '1';
                public const char Map_Tool_SemiBlock_2 = '2';
            }

            public static class String
            {
                // Datas
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

                // Others Prefab
                public static readonly string Managers = "@Managers";
                public static readonly string UI_Root = "@UI_Root";
                public static readonly string EventSystem = "@EventSystem";
                public static readonly string PreLoad = "PreLoad";
                public static readonly string BaseMap = "BaseMap";
                public static readonly string Hero = "Hero";
                public static readonly string AnimBody = "AnimationBody";
                public static readonly string HeroPoolingRootName = "@Pool_Heroes";
                public static readonly string MonsterPoolingRootName = "@Pool_Monsters";
                public static readonly string EnvPoolingRootName = "@Pool_Envs";
                public static readonly string ProjectilePoolingRootName = "@Pool_Projectiles";
                public static readonly string DamageFontPoolingRootName = "@Pool_DamageFonts";
                public static readonly string UI_Joystick = "UI_Joystick";
                public static readonly string AnimationBody = "AnimationBody";
                public static readonly string HeroCamp = "HeroCamp";
                public static readonly string LeaderController = "LeaderController";
                //public static readonly string SummerForest_Field_Temp = "SummerForest_Field_Temp";

                // Sprite - Collect Equipment
                public static readonly string Pickaxe_Common_SP = "Pickaxe_Common.sprite";
                public static readonly string Pickaxe_Elite_SP = "Pickaxe_Elite.sprite";
                public static readonly string WoodcutterAxe_Common_SP = "WoodcutterAxe_Common.sprite";
                public static readonly string WoodcutterAxe_Elite_SP = "WoodcutterAxe_Elite.sprite";

                // Sprite - Hero Human Body Type
                public static readonly string HBody_HumanType_Head_SP = "Human_Head.sprite";
                public static readonly string HBody_HumanType_Ears_SP = "Human_Ears.sprite";
                public static readonly string HBody_HumanType_Torso_SP = "Human_Torso.sprite";
                public static readonly string HBody_HumanType_ArmL_SP = "Human_ArmL.sprite";
                public static readonly string HBody_HumanType_ArmR_SP = "Human_ArmR.sprite";
                public static readonly string HBody_HumanType_ForearmL_SP = "Human_ForearmL.sprite";
                public static readonly string HBody_HumanType_ForearmR_SP = "Human_ForearmR.sprite";
                public static readonly string HBody_HumanType_HandL_SP = "Human_HandL.sprite";
                public static readonly string HBody_HumanType_HandR_SP = "Human_HandR.sprite";
                public static readonly string HBody_HumanType_Finger_SP = "Human_Finger.sprite";
                public static readonly string HBody_HumanType_Pelvis_SP = "Human_Pelvis.sprite";
                public static readonly string HBody_HumanType_Shin_SP = "Human_Shin.sprite";
                public static readonly string HBody_HumanType_Leg_SP = "Human_Leg.sprite";

                // Sprite & Material - Env Rock
                public static readonly string ERock_Rock_SP = "StoneRock_Rock.sprite";
                public static readonly string ERock_Empty_SP = "StoneRock_Empty.sprite";
                public static readonly string ERock_Mat = "RockFragments";

                // Sprite - Common
                public static readonly string Shadow_SP = "Shadow.sprite";
                public static readonly string Light_SP = "Light.sprite";

                // Material - Common
                public static readonly string Glow_Mat = "Glow";

                // Hero Armored Body
                public static readonly string HBody_HeadSkin = "Head";
                public static readonly string HBody_Hair = "Hair";
                public static readonly string HBody_Eyes = "Eyes";
                public static readonly string HBody_Eyebrows = "Eyebrows";
                public static readonly string HBody_Mouth = "Mouth";
                public static readonly string HBody_Ears = "Ears";
                public static readonly string HBody_Earrings = "Earrings";
                public static readonly string HBody_Beard = "Beard";
                public static readonly string HBody_Mask = "Mask";
                public static readonly string HBody_Glasses = "Glasses";
                public static readonly string HBody_Helmet = "Helmet";

                public static readonly string HBody_TorsoSkin = "Torso";
                public static readonly string HBody_Torso = "Torso_Armor";
                public static readonly string HBody_Cape = "Cape_Armor";
                public static readonly string HBody_ArmLSkin = "ArmL";
                public static readonly string HBody_ArmL = "ArmL_Armor";
                public static readonly string HBody_ForearmLSkin = "ForearmL";
                public static readonly string HBody_ForearmL = "ForearmL_Armor";
                public static readonly string HBody_HandLSkin = "HandL";
                public static readonly string HBody_HandL = "HandL_Armor";
                public static readonly string HBody_FingerSkin = "Finger";
                public static readonly string HBody_Finger = "Finger_Armor";

                // Hero Armored WeaponL
                public static readonly string HBody_WeaponL = "WeaponL_Armor";
                // public static readonly string HBody_WeaponLFireSocket = "WeaponL_FireSocket";
                // public static readonly string HBody_WeaponLChildsGroup = "WeaponL_Armor_Childs";
                // public static readonly string HBody_WeaponLAttachment1 = "WeaponLAttachment1_Armor";
                // public static readonly string HBody_WeaponLAttachment2 = "WeaponLAttachment2_Armor";
                // public static readonly string HBody_WeaponLAttachment3 = "WeaponLAttachment3_Armor";

                public static readonly string HBody_ArmRSkin = "ArmR";
                public static readonly string HBody_ArmR = "ArmR_Armor";
                public static readonly string HBody_ForearmRSkin = "ForearmR";
                public static readonly string HBody_ForearmR = "ForearmR_Armor";
                public static readonly string HBody_SleeveR = "SleeveR_Armor";
                public static readonly string HBody_HandRSkin = "HandR";
                public static readonly string HBody_HandR = "HandR_Armor";

                // Hero Armored WeaponR
                public static readonly string HBody_WeaponR = "WeaponR_Armor";
                // public static readonly string HBody_WeaponRAttachment1 = "WeaponRAttachment1_Armor";
                // public static readonly string HBody_WeaponRAttachment2 = "WeaponRAttachment2_Armor";
                // public static readonly string HBody_WeaponRAttachment3 = "WeaponRAttachment3_Armor";

                public static readonly string HBody_PelvisSkin = "Pelvis";
                public static readonly string HBody_Pelvis = "Pelvis_Armor";
                public static readonly string HBody_LegLSkin = "LegL";
                public static readonly string HBody_LegL = "LegL_Armor";
                public static readonly string HBody_ShinLSkin = "ShinL";
                public static readonly string HBody_ShinL = "ShinL_Armor";
                public static readonly string HBody_LegRSkin = "LegR";
                public static readonly string HBody_LegR = "LegR_Armor";
                public static readonly string HBody_ShinRSkin = "ShinR";
                public static readonly string HBody_ShinR = "ShinR_Armor";

                // Monster Body
                public static readonly string MBody_Head = "Head";
                public static readonly string MBody_Wing = "Wing";
                public static readonly string MBody_LegL = "LegL";
                public static readonly string MBody_LegFrontL = "LegFrontL";
                public static readonly string MBody_LegBackL = "LegBackL";
                public static readonly string MBody_LegR = "LegR";
                public static readonly string MBody_LegFrontR = "LegFrontR";
                public static readonly string MBody_LegBackR = "LegBackR";
                public static readonly string MBody_Tail = "Tail";

                // Env Body
                public static readonly string ETreeBody_Trunk = "Trunk";
                public static readonly string ETreeBody_Patch = "Patch";
                public static readonly string ETreeBody_Stump = "Stump";
                public static readonly string ETreeBody_Fruits = "Fruits";

                public static readonly string ERock_Rock = "Rock";
                public static readonly string ERock_Empty = "Empty";
                public static readonly string ERock_Ore = "Ore";
                public static readonly string ERock_OreShadow = "OreShadow";
                public static readonly string ERock_OreLight = "OreLight";
                public static readonly string ERock_OreParticle = "OreParticle";
                public static readonly string ERock_Spot = "Spot";
                public static readonly string ERock_Fragment = "Fragment";

                public static readonly string EBody_EndParticle = "EndParticle";
                public static readonly string EBody_Shadow = "Shadow";

                // Animation Params
                public static readonly string AnimParam_Idle = "Idle";
                public static readonly string AnimParam_Move = "Move";
                public static readonly string AnimParam_Skill_Attack = "Skill_Attack";
                public static readonly string AnimParam_Skill_A = "Skill_A";
                public static readonly string AnimParam_Skill_B = "Skill_B";
                public static readonly string AnimParam_CollectEnv = "CollectEnv";
                public static readonly string AnimParam_Dead = "Dead";

                // Write Tile
                public static readonly string Tile_CanMove = "_CanMove";
                public static readonly string Tile_SemiBlock = "_SemiBlock";
                public static readonly string Tile_Block = "_Block";

                // Map
                public static readonly string Tilemap_Collision = "Tilemap_Collision";
            }

            public static class Numeric
            {
                // [ INTEGER ]
                // public static readonly int SortingLayer_Base = 0;
                // public static readonly int SortingLayer_Projectile = 10;
                // public static readonly int SortingLayer_VFX = 20;
                // public static readonly int SortingLayer_HeroCamp = 90;

                // public static readonly int SortingOrder_SpellIndicator = 200;
                // public static readonly int SortingOrder_Creature = 300;
                // public static readonly int SortingOrder_Env = 300;
                // public static readonly int SortingOrder_Projectile = 310;
                // public static readonly int SortingOrder_SkillEffect = 310;
                // public static readonly int SortingOrder_DamageFont = 410;
                // //public static readonly int SortingOrder_Weapon = 200;
                // public static readonly int SortingOrder_Weapon = 320;

                // ID - Monsters

                // ID - Envs Trees & Rocks

                // MISC
                public static readonly int RockElementsCount = 3;
                public static readonly int MaxActiveSkillsCount = 2;
                public static readonly int HeroDefaultMoveDepth = 10; // default: 5 -> 10
                public static readonly int HeroMaxMoveDepth = 100;
                public static readonly int CreatureWarpMoveDepth = 50;
                public static readonly int MonsterDefaultMoveDepth = 5; // default: 3 -> 5       
                public static readonly int MaxCanPingPongConditionCount = 20;

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
                public static readonly float SearchFindTargetTick = 0.25F;
                //public static readonly float SearchFindTargetTick = 0.5F;

                public static readonly float CamOrthoSize = 12.0F; 
                public static readonly float JoystickFocusMinDist = -0.18F;
                public static readonly float JoystickFocusMaxDist = 0.18F;

                public static readonly float MonsterSize_Small = 0.5F;
                public static readonly float MonsterSize_Medium = 0.8F;
                public static readonly float MonsterSize_Large = 1.2F;

                public static readonly float HeroDefaultScanRange = 8.0F; // 오리지날 6F, 일단 6칸
                public static readonly float MonsterDefaultScanRange = 6.0F; // 상하좌우 한칸 기준, 대각선X
                public static readonly float Temp_StopDistance = 1.25F;
                
                // Dead Fade Out Time
                public static readonly float StartDeadFadeOutTime = 0.85F;
                public static readonly float DesiredDeadFadeOutEndTime = 1.0F;

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

