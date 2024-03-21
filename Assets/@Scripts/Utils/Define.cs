using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace STELLAREST_F1
{
    public static class Define
    {
        public enum EHeroRarity
        {
            Common,
            Epic
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

        public enum ECreatureType
        {
            None,
            Hero,
            Monster,
            Npc
        }

        public enum ECreatureState
        {
            None,
            Idle,
            Move,
            Attack,
            Skill,
            Dead
        }

        public enum LookAtDirection
        {
            Left = -1,
            Right = 1,
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


        // public enum EHeroBodyParts
        // {
        //     None = -1,
        //     // HEAD
        //     Hair,
        //     Eyes,
        //     Eyebrows,
        //     Mouth,
        //     Beard,
        //     Earrings,
        //     Mask,
        //     Glasses,
        //     Helmet,

        //     // UPPER
        //     Torso,
        //     Cape,
        //     ArmL,
        //     ForearmL,
        //     HandL,
        //     Finger,
        //     ArmR,
        //     ForearmR,
        //     SleeveR,
        //     HandR,

        //     // LOWER
        //     Pelvis,
        //     LegL,
        //     ShinL,
        //     LegR,
        //     ShinR,
        //     Max = 24,
        // }

        public static class ReadOnly
        {
            public static class String
            {
                public static readonly string HeroData = "HeroData";
                public static readonly string HeroSpriteData = "HeroSpriteData";

                public static readonly string Managers = "@Managers";
                public static readonly string UI_Root = "@UI_Root";
                public static readonly string EventSystem = "@EventSystem";
                public static readonly string PreLoad = "PreLoad";
                public static readonly string BaseMap = "BaseMap";
                public static readonly string Hero = "Hero";
                public static readonly string AnimBody = "AnimationBody";
                public static readonly string HeroRoot = "@Heroes";
                public static readonly string MonsterRoot = "@Monsters";
                public static readonly string UI_Joystick = "UI_Joystick";
                public static readonly string AnimationBody = "AnimationBody";
                
                public static readonly string Anim_Idle = "Idle";
                public static readonly string Anim_Attack = "Attack";
                public static readonly string Anim_Skill_A = "Skill_A"; // 교체해야됨
                public static readonly string Anim_Skill_B = "Skill_B"; // 교체해야됨
                public static readonly string Anim_Move = "Move";
                public static readonly string Anim_Dead = "Dead";

                // Hero Body Parts
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

                // TODO : WEAPON L
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

                // TODO : WEAPON R
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
            }

            public static class Numeric
            {
                public static readonly int TemplateID_Hero_Temp = 201000; // TEMP
                public static readonly int SortingLayer_SpellIndicator = 200;
                public static readonly int SortingLayer_Creature = 300;
                public static readonly int SortingLayer_Env = 300;
                public static readonly int SortingLayer_Projectile = 310;
                public static readonly int SortingLayer_SkillEffect = 310;
                public static readonly int SortingLayer_DamageFont = 410;

                public static readonly int DataID_Lancer = 101000;

                public static readonly float CamOrthoSize = 10F; 
                public static readonly float JoystickFocusMinDist = -0.18F;
                public static readonly float JoystickFocusMaxDist = 0.18F;
            }
        }
    }
}

