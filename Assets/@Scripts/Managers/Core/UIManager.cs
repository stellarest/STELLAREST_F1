using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class UIManager
    {
        private int _order = 10;
        private Stack<UI_Popup> _popupUIStack = new Stack<UI_Popup>();

        private UI_Scene _sceneUI = null;
        public UI_Scene SceneUI
        {
            get => _sceneUI;
            set => _sceneUI = value;
        }

        public GameObject Root
        {
            get
            {
                GameObject root = GameObject.Find(Const.String.UI_Root);
                if (root == null)
                    root = new GameObject { name = Const.String.UI_Root };

                return root;
            }
        }

        public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
        {
            Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.overrideSorting = true;
            }

            CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
            if (cs != null)
            {
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = new Vector2(1080f, 1920f);
            }

            go.GetOrAddComponent<GraphicRaycaster>();

            if (sort)
                canvas.sortingOrder = _order++;
            else
                canvas.sortingOrder = sortOrder;
        }

        public T GetSceneUI<T>() where T : UI_Base
            => _sceneUI as T;

        public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = Managers.Resource.Instantiate($"{name}");
            if (parent != null)
                go.transform.SetParent(parent);

            Canvas canvas = go.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            return Util.GetOrAddComponent<T>(go);
        }

        public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UI_Base
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = Managers.Resource.Instantiate(name, parent, pooling);
            go.transform.SetParent(parent);

            return Util.GetOrAddComponent<T>(go);
        }

        public T ShowBaseUI<T>(string name = null) where T : UI_Base
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = Managers.Resource.Instantiate(name);
            T baseUI = Util.GetOrAddComponent<T>(go);
            go.transform.SetParent(Root.transform);

            return baseUI;
        }

        public T ShowSceneUI<T>(string name = null) where T : UI_Scene
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = Managers.Resource.Instantiate(name);
            T sceneUI = Util.GetOrAddComponent<T>(go);
            _sceneUI = sceneUI;
            go.transform.SetParent(Root.transform);
            
            return sceneUI;
        }

        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            GameObject go = Managers.Resource.Instantiate(name);
            T popupUI = Util.GetOrAddComponent<T>(go);
            _popupUIStack.Push(popupUI);
            go.transform.SetParent(Root.transform);

            return popupUI;
        }

        public void ClosePopupUI(UI_Popup popup)
        {
            if (_popupUIStack.Count == 0)
                return;

            if (_popupUIStack.Peek() != popup)
            {
                Debug.LogWarning($"{nameof(UIManager)}, {nameof(ClosePopupUI)}, Input : \"{popup.name}\"");
                return;
            }

            ClosePopupUI();
        }

        public void ClosePopupUI()
        {
            if (_popupUIStack.Count == 0)
                return;

            UI_Popup popupUI = _popupUIStack.Pop();
            Managers.Resource.Destroy(popupUI.gameObject);
            _order--;
        }

        public void CloseAllPopupUI()
        {
            while (_popupUIStack.Count > 0)
                ClosePopupUI();
        }

        public int GetPopupUICount => _popupUIStack.Count;

        public void Clear()
        {
            CloseAllPopupUI();
            _sceneUI = null;
        }
    }
}
