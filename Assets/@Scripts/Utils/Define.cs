using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace STELLAREST_F1
{
    public static class Define
    {
        public enum EObjectRarity
        {
            None = -1,
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
            HeroCamp,
            Projectile,
            Max = Projectile + 1
        }

        public enum ECreatureState
        {
            None = -1,
            Idle,
            Move,
            Skill_Attack,
            Skill_A,
            Skill_B,
            CollectEnv,
            OnDamaged, // TEMP
            Dead,
            Max = Dead + 1
        }

        public enum ECreatureMoveState
        {
            None,
            TargetToEnemy,
            CollectEnv,
            ReturnToBase,
            ForceMove,
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
            Combat,
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
            WeaponR,
            Max = WeaponR + 1
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

        public static class ReadOnly
        {
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

                // Others
                public static readonly string Managers = "@Managers";
                public static readonly string UI_Root = "@UI_Root";
                public static readonly string EventSystem = "@EventSystem";
                public static readonly string PreLoad = "PreLoad";
                public static readonly string BaseMap = "BaseMap";
                public static readonly string Hero = "Hero";
                public static readonly string AnimBody = "AnimationBody";
                public static readonly string HeroPoolingRootName = "@Pool_Heroes";
                public static readonly string MonsterPoolingRootName = "@Pool_Monsters";
                public static readonly string EnvRootName = "@Envs";
                public static readonly string UI_Joystick = "UI_Joystick";
                public static readonly string AnimationBody = "AnimationBody";
                public static readonly string HeroCamp = "HeroCamp";

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
                public static readonly string HBody_WeaponLAttachment1 = "WeaponLAttachment1_Armor";
                public static readonly string HBody_WeaponLAttachment2 = "WeaponLAttachment2_Armor";
                public static readonly string HBody_WeaponLAttachment3 = "WeaponLAttachment3_Armor";

                public static readonly string HBody_ArmRSkin = "ArmR";
                public static readonly string HBody_ArmR = "ArmR_Armor";
                public static readonly string HBody_ForearmRSkin = "ForearmR";
                public static readonly string HBody_ForearmR = "ForearmR_Armor";
                public static readonly string HBody_SleeveR = "SleeveR_Armor";
                public static readonly string HBody_HandRSkin = "HandR";
                public static readonly string HBody_HandR = "HandR_Armor";

                // Hero Armored WeaponR
                public static readonly string HBody_WeaponR = "WeaponR_Armor";
                public static readonly string HBody_WeaponRAttachment1 = "WeaponRAttachment1_Armor";
                public static readonly string HBody_WeaponRAttachment2 = "WeaponRAttachment2_Armor";
                public static readonly string HBody_WeaponRAttachment3 = "WeaponRAttachment3_Armor";

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
            }

            public static class Numeric
            {
                public static readonly int SortingOrder_SpellIndicator = 200;
                public static readonly int SortingOrder_Creature = 300;
                public static readonly int SortingOrder_Env = 300;
                public static readonly int SortingOrder_Projectile = 310;
                public static readonly int SortingOrder_SkillEffect = 310;
                public static readonly int SortingOrder_DamageFont = 410;
                //public static readonly int SortingOrder_Weapon = 200;
                public static readonly int SortingOrder_Weapon = 320;

                // ID - Heroes
                public static readonly int DataID_Hero_Paladin = 101000;
                public static readonly int DataID_Hero_Archer = 101001;
                public static readonly int DataID_Hero_Wizard = 101002;
                public static readonly int DataID_Hero_Lancer = 101999;

                // ID - Monsters
                public static readonly int DataID_Monster_Chicken = 201000;
                public static readonly int DataID_Monster_Turkey = 201001;
                public static readonly int DataID_Monster_Bunny = 201020;
                public static readonly int DataID_Monster_Pug = 201021;

                // ID - Envs Trees & Rocks
                public static readonly int DataID_Env_AshTree = 301000;
                public static readonly int DataID_Env_BlackOakTree = 301001;
                public static readonly int DataID_Env_GreenAppleTree = 301002;
                public static readonly int DataID_Env_IvyTree = 301003;
                public static readonly int DataID_Env_ManticoreTree = 301004;
                public static readonly int DataID_Env_MapleTree = 301005;
                public static readonly int DataID_Env_OakTree = 301006;
                public static readonly int DataID_Env_RedAppleTree = 301007;
                public static readonly int DataID_Env_RedSandalTree = 301008;
                public static readonly int DataID_Env_WillowTree = 301009;
                public static readonly int DataID_Env_YewTree = 301010;
                public static readonly int DataID_Env_CopperRock = 301011;
                public static readonly int DataID_Env_GoldRock = 301012;
                public static readonly int DataID_Env_IronRock = 301013;
                public static readonly int DataID_Env_LimestoneRock = 301014;
                public static readonly int DataID_Env_SilverRock = 301015;
                public static readonly int DataID_Env_StoneRock = 301016;
                public static readonly int DataID_Env_TinRock = 301017;
                public static readonly int DataID_Env_WhetstoneRock = 301018;
                public static readonly int DataID_Env_ZincRock = 301019;

                public static readonly float CamOrthoSize = 12F; 
                public static readonly float JoystickFocusMinDist = -0.18F;
                public static readonly float JoystickFocusMaxDist = 0.18F;

                public static readonly float MonsterSize_Small = 0.5F;
                public static readonly float MonsterSize_Medium = 0.8F;
                public static readonly float MonsterSize_Large = 1.2F;

                public static readonly float DefaultSearchRange = 8.0F;
                public static readonly float DefaultStopRange = 1.25F;

                // Env
                public static readonly int RockElementsCount = 3;
                
                // Dead Fade Out Time
                public static readonly float StartDeadFadeOutTime = 1.0F;
                public static readonly float DesiredDeadFadeOutEndTime = 2.0F;
            }
        }
    }
}

