using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public static class Extension
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
            => Util.GetOrAddComponent<T>(go);

        public static void BindEvent(this GameObject go, Action<PointerEventData> action = null, EUIEvent evtType = EUIEvent.PointerClick)
            => UI_Base.BindEvent(go, action, evtType);

        public static bool IsValid(this GameObject go)
            => go != null && go.activeSelf;

        public static void DestroyChild(this GameObject go)
        {
            foreach (Transform child in go.transform)
                Managers.Resource.Destroy(child.gameObject);
        }

        public static void TranslateEx(this Transform transform, Vector3 dir)
        {
            BaseObject bo = transform.gameObject.GetComponent<BaseObject>();
            if (bo != null)
                bo.TranslateEx(dir);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]); // swap
            }
        }
    }
}
