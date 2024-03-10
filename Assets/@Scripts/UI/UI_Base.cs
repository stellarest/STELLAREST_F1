using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Image = UnityEngine.UI.Image;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class UI_Base : InitBase
    {
        protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        protected void Bind<T>(Type type) where T : UnityEngine.Object
        {
            string[] names = Enum.GetNames(type);

            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            _objects.Add(typeof(T), objects);
            /*
                EX1) Key : GameObject, Value : Object(StartImage)
                EX2) Key : TMP_Text, Value : Object(DisplayText)
            */

            for (int i = 0; i < names.Length; ++i)
            {
                if (typeof(T) == typeof(GameObject))
                    objects[i] = Util.FindChild(gameObject, names[i], true);
                else
                    objects[i] = Util.FindChild<T>(gameObject, names[i], true);

                if (objects[i] == null)
                    Debug.LogWarning($"{nameof(UI_Base)}, {nameof(Bind)}, Input : \"{names[i]}\"");
            }
        }

        protected void BindObjects(Type type) => Bind<GameObject>(type);
        protected void BindImages(Type type) => Bind<Image>(type);
        protected void BindTexts(Type type) => Bind<TMP_Text>(type);
        protected void BindButtons(Type type) => Bind<Button>(type);
        protected void BindToggles(Type type) => Bind<Toggle>(type);

        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            if (_objects.TryGetValue(typeof(T), out UnityEngine.Object[] objects) == false)
                return null;

            return objects[idx] as T;
        }
        protected GameObject GetObject(int idx) => Get<GameObject>(idx);
        protected Image GetImage(int idx) => Get<Image>(idx);
        protected TMP_Text GetText(int idx) => Get<TMP_Text>(idx);
        protected Button GetButton(int idx) => Get<Button>(idx);
        protected Toggle GetToggle(int idx) => Get<Toggle>(idx);

        public static void BindEvent(GameObject go, Action<PointerEventData> action = null, EUIEvent evtType = EUIEvent.PointerClick)
        {
            UI_EventHandler uiEvtHandler = Util.GetOrAddComponent<UI_EventHandler>(go);
            switch (evtType)
            {
                case EUIEvent.PointerClick:
                    uiEvtHandler.OnPointerClickHandler -= action;
                    uiEvtHandler.OnPointerClickHandler += action;
                    break;

                case EUIEvent.PointerDown:
                    uiEvtHandler.OnPointerDownHandler -= action;
                    uiEvtHandler.OnPointerDownHandler += action;
                    break;

                case EUIEvent.PointerUp:
                    uiEvtHandler.OnPointerUpHandler -= action;
                    uiEvtHandler.OnPointerUpHandler += action;
                    break;

                case EUIEvent.Drag:
                    uiEvtHandler.OnDragHandler -= action;
                    uiEvtHandler.OnDragHandler += action;
                    break;
            }
        }
    }
}
