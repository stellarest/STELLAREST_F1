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

        public static bool IsValid(this Transform tr)
            => tr != null && tr.gameObject.activeSelf;

        public static bool IsValid(this BaseObject bo)
        {
            if (bo != null && bo.isActiveAndEnabled)
            {
                switch (bo.ObjectType)
                {
                    case EObjectType.Hero:
                    case EObjectType.Monster:
                        Creature creature = bo as Creature;
                        return creature.CreatureState != ECreatureState.Dead;

                    case EObjectType.Env:
                        Env env = bo as Env;
                        return env.EnvState != EEnvState.Dead;

                    // ##### TODO #####
                    case EObjectType.Projectile:
                        return bo != null && bo.gameObject.activeSelf;
                    
                }
            }

            return false;
        }

        public static void DestroyChild(this GameObject go)
        {
            foreach (Transform child in go.transform)
                Managers.Resource.Destroy(child.gameObject);
        }

        public static void AddLayer(this ref LayerMask mask, Define.ELayer layer)
        {
            mask |= 1 << (int)layer;
        }

        public static void RemoveLayer(this ref LayerMask mask, Define.ELayer layer)
        {
            mask &= ~(1 << (int)layer);
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
