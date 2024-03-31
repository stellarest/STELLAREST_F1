using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace STELLAREST_F1
{
    public static class Define
    {
        public enum ECreatureRarity
        {
            Common,
            Epic
        }

        public enum EHeroType
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
            None,
            Hero,
            Monster,
            Projectile,
            Env
        }

        public enum ECreatureState
        {
            None,
            Idle,
            Move,
            Attack,
            Skill,
            OnDamaged,
            Dead
        }

        public enum LookAtDirection
        {
            Left = -1,
            Right = 1,
        }

        public enum EHeroEmoji
        {
            None = -1,
            Default,
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
            Beard,
            Earrings,
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

                public static readonly string Managers = "@Managers";
                public static readonly string UI_Root = "@UI_Root";
                public static readonly string EventSystem = "@EventSystem";
                public static readonly string PreLoad = "PreLoad";
                public static readonly string BaseMap = "BaseMap";
                public static readonly string Hero = "Hero";
                public static readonly string AnimBody = "AnimationBody";
                public static readonly string HeroRootName = "@Heroes";
                public static readonly string MonsterRootName = "@Monsters";
                public static readonly string UI_Joystick = "UI_Joystick";
                public static readonly string AnimationBody = "AnimationBody";

                // public static readonly string Anim_Idle = "Idle";
                // public static readonly string Anim_Attack = "Attack";
                // public static readonly string Anim_Skill_A = "Skill_A"; // 교체해야됨
                // public static readonly string Anim_Skill_B = "Skill_B"; // 교체해야됨
                // public static readonly string Anim_Move = "Move";
                // public static readonly string Anim_Dead = "Dead";

                // Hero - Human Body Type
                public static readonly string HBody_HumanType_Head = "Human_Head.sprite";
                public static readonly string HBody_HumanType_Torso = "Human_Torso.sprite";
                public static readonly string HBody_HumanType_ArmL = "Human_ArmL.sprite";
                public static readonly string HBody_HumanType_ArmR = "Human_ArmR.sprite";
                public static readonly string HBody_HumanType_ForearmL = "Human_ForearmL.sprite";
                public static readonly string HBody_HumanType_ForearmR = "Human_ForearmR.sprite";
                public static readonly string HBody_HumanType_HandL = "Human_HandL.sprite";
                public static readonly string HBody_HumanType_HandR = "Human_HandR.sprite";
                public static readonly string HBody_HumanType_Finger = "Human_Finger.sprite";
                public static readonly string HBody_HumanType_Pelvis = "Human_Pelvis.sprite";
                public static readonly string HBody_HumanType_Shin = "Human_Shin.sprite";
                public static readonly string HBody_HumanType_Leg = "Human_Leg.sprite";

                // Hero Face - Sick
                public static readonly string HFace_Eyebrows_Sick = "Hero_Eyebrows_Sick.sprite";
                public static readonly string HFace_Eyes_Sick = "HeroEyes_Sick.sprite";
                public static readonly string HFace_Mouth_Sick = "HeroMouth_Sick.sprite";

                // Hero Face - Dead
                public static readonly string HFace_Eyes_Dead = "HeroEyes_Dead.sprite";
                public static readonly string HFace_Eyes_DeadColor = "#00C8FF";
                public static readonly string HFace_Mouth_Dead = "HeroMouth_Dead.sprite";

                // Monster Head
                public static readonly string MBody_Chicken_Head_Default = "Chicken_Head_Default.sprite";
                public static readonly string MBody_Chicken_Head_Angry = "Chicken_Head_Angry.sprite";
                public static readonly string MBody_Chicken_Head_Dead = "Chicken_Head_Dead.sprite";
                public static readonly string MBody_Chicken_Body = "Chicken_Body.sprite";
                public static readonly string MBody_Chicken_Wing = "Chicken_Wing.sprite";
                public static readonly string MBody_Chicken_Leg = "Chicken_Leg.sprite";

                // Hero Armored Body
                public static readonly string HBody_HeadSkin = "Head";
                public static readonly string HBody_Hair = "Hair";
                public static readonly string HBody_Eyes = "Eyes";
                public static readonly string HBody_Eyebrows = "Eyebrows";
                public static readonly string HBody_Mouth = "Mouth";
                public static readonly string HBody_Beard = "Beard";
                public static readonly string HBody_Earrings = "Earrings";
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

                // Animation Params
                public static readonly string AnimParam_Idle = "Idle";
                public static readonly string AnimParam_Move = "Move";
                public static readonly string AnimParam_Attack = "Attack";
                public static readonly string AnimParam_Skill_A = "Skill_A";
                public static readonly string AnimParam_Skill_B = "Skill_B";
                public static readonly string AnimParam_Dead = "Dead";
            }

            public static class Numeric
            {
                public static readonly int TemplateID_Hero_Temp = 201000; // TEMP
                public static readonly int SortingOrder_SpellIndicator = 200;
                public static readonly int SortingOrder_Creature = 300;
                public static readonly int SortingOrder_Env = 300;
                public static readonly int SortingOrder_Projectile = 310;
                public static readonly int SortingOrder_SkillEffect = 310;
                public static readonly int SortingOrder_DamageFont = 410;
                //public static readonly int SortingOrder_Weapon = 200;
                public static readonly int SortingOrder_Weapon = 320;

                // ID - Hero
                public static readonly int DataID_Hero_Lancer = 101000;

                // ID - Monster
                public static readonly int DataID_Monster_Chicken = 201000;
                public static readonly int DataID_Monster_Turkey = 201001;
                public static readonly int DataID_Monster_Bunny = 201020;
                public static readonly int DataID_Monster_Pug = 201021;

                public static readonly float CamOrthoSize = 12F; 
                public static readonly float JoystickFocusMinDist = -0.18F;
                public static readonly float JoystickFocusMaxDist = 0.18F;

                public static readonly float MonsterSize_Small = 0.5f;
                public static readonly float MonsterSize_Medium = 0.8f;
                public static readonly float MonsterSize_Large = 1.2f;

            }
        }
    }
}

