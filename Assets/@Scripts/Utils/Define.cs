using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

namespace STELLAREST_F1
{
    public static class Define
    {
        public enum EHeroGrade
        {
            Default,
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
            Creature,
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

        public static class ReadOnly
        {
            public static class String
            {
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
                
                public static readonly string Anim_Idle = "Idle";
                public static readonly string Anim_Attack = "Attack";
                public static readonly string Anim_Skill_A = "Skill_A"; // 교체해야됨
                public static readonly string Anim_Skill_B = "Skill_B"; // 교체해야됨
                public static readonly string Anim_Move = "Move";
                public static readonly string Anim_Dead = "Dead";
            }

            public static class Numeric
            {
                public static readonly float CamOrthoSize = 10F; 
                public static readonly float JoystickFocusMinDist = -0.18F;
                public static readonly float JoystickFocusMaxDist = 0.18F;
            }
        }
    }
}

