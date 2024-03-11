using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public static class Define
    {
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
            Drag,
        }

        public enum ESound
        {
            Bgm,
            Effect,
            Max
        }

        public static class ReadOnly
        {
            public static class String
            {
                public static readonly string Managers = "@Managers";
                public static readonly string UI_Root = "@UI_Root";
                public static readonly string EventSystem = "@EventSystem";
                public static readonly string PreLoad = "PreLoad";
            }

            public static class Numeric
            {
            }
        }
    }
}

