using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static STELLAREST_F1.Define;
using Object = UnityEngine.Object;

namespace STELLAREST_F1
{
    public abstract class BaseScene : InitBase
    {
        public EScene SceneType { get; protected set; } = EScene.Unknown;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
            if (obj == null)
            {
                GameObject go = new GameObject { name = ReadOnly.Util.EventSystem };
                go.AddComponent<EventSystem>();
                go.AddComponent<StandaloneInputModule>();
            }
            else
                obj.name = ReadOnly.Util.EventSystem;

            return true;
        }

        public abstract void Clear();
    }
}
