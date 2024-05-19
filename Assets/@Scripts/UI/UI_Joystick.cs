using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class UI_Joystick : UI_Base
    {
        private enum GameObjects
        {
            JoystickBG,
            BG_FocusLT,
            BG_FocusRT,
            BG_FocusLB,
            BG_FocusRB,
            JoystickCursor,
        }

        private GameObject _bg = null;
        private GameObject[] _bgFocuses = null;
        private GameObject _cursor = null;
        private float _radius = 0f;
        private Vector2 _touchPos = Vector2.zero;

        private Vector2 _basePos = Vector2.zero;
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BindObjects(typeof(GameObjects));
            _bg = GetObject((int)GameObjects.JoystickBG);
            _basePos = _bg.transform.position;

            _bgFocuses = new GameObject[(int)GameObjects.BG_FocusRB];
            _bgFocuses[GetFocusIndex(GameObjects.BG_FocusLT)] = GetObject((int)GameObjects.BG_FocusLT);
            _bgFocuses[GetFocusIndex(GameObjects.BG_FocusRT)] = GetObject((int)GameObjects.BG_FocusRT);
            _bgFocuses[GetFocusIndex(GameObjects.BG_FocusLB)] = GetObject((int)GameObjects.BG_FocusLB);
            _bgFocuses[GetFocusIndex(GameObjects.BG_FocusRB)] = GetObject((int)GameObjects.BG_FocusRB);
            _cursor = GetObject((int)GameObjects.JoystickCursor);
            _radius = _bg.GetComponent<RectTransform>().sizeDelta.x / 2;

            gameObject.BindEvent(action: OnJoystickPointerDown, evtType: EUIEvent.PointerDown);
            gameObject.BindEvent(action: OnJoystickPointerUp, evtType: EUIEvent.PointerUp);
            gameObject.BindEvent(action: OnJoystickDrag, evtType: EUIEvent.Drag);

            ShowJoystick(true);
            ShowFocus(Vector2.zero);
            return true;
        }

        public void OnJoystickPointerDown(PointerEventData evtData)
        {
            ShowFocus(Vector2.zero);
            _bg.transform.position = evtData.position;
            _cursor.transform.position = evtData.position;
            _touchPos = evtData.position;

            Managers.Game.MoveDir = Vector2.zero;
            Managers.Game.JoystickState = EJoystickState.PointerDown;
        }

        public void OnJoystickPointerUp(PointerEventData evtData)
        {
            ShowFocus(Vector2.zero);
            _bg.transform.position = _basePos;
            _cursor.transform.position = _basePos;

            Managers.Game.MoveDir = Vector2.zero;
            Managers.Game.JoystickState = EJoystickState.PointerUp;
        }

        public void OnJoystickDrag(PointerEventData evtData)
        {
            Vector2 dir = evtData.position - _touchPos;
            float maxDist = _radius * GetViewportScale;
            float moveDist = Mathf.Min(dir.magnitude, maxDist);
            Vector2 nDir = dir.normalized;
            ShowFocus(nDir);

            _cursor.transform.position = _touchPos + nDir * moveDist;
            Managers.Game.MoveDir = nDir;
            Managers.Game.JoystickState = EJoystickState.Drag;
        }

        private void ShowJoystick(bool show)
        {
            _bg.SetActive(show);
            _cursor.SetActive(show);
        }

        private void ShowFocus(Vector2 dir)
        {
            if (dir == Vector2.zero)
            {
                ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLT));
                ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRT));
                ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLB));
                ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRB));
            }
            else
            {
                float x = Mathf.Sign(dir.x);
                float y = Mathf.Sign(dir.y);
                // 좌측
                if (x == -1)
                {
                    // 좌상단
                    if (y == 1)
                    {
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusLT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLB));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRB));
                    }
                    // 좌중앙
                    else if (dir.y >= ReadOnly.Numeric.JoystickFocusMinDist && dir.y <= ReadOnly.Numeric.JoystickFocusMaxDist)
                    {
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusLT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRT));
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusLB));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRB));
                    }
                    // 좌하단
                    else if (y == -1)
                    {
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRT));
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusLB));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRB));
                    }
                }
                // 중앙
                else if (dir.x >= ReadOnly.Numeric.JoystickFocusMinDist && dir.x <= ReadOnly.Numeric.JoystickFocusMaxDist)
                {
                    // 중앙 상단
                    if (y == 1)
                    {
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusLT));
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusRT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLB));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRB));
                    }
                    // 중앙 하단
                    else if (y == -1)
                    {
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRT));
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusLB));
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusRB));
                    }
                }
                // 우측
                else if (x == 1)
                {
                    // 우상단
                    if (y == 1)
                    {
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLT));
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusRT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLB));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRB));
                    }
                    // 우측 중앙
                    else if (dir.y >= ReadOnly.Numeric.JoystickFocusMinDist && dir.y <= ReadOnly.Numeric.JoystickFocusMaxDist)
                    {
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLT));
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusRT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLB));
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusRB));
                    }
                    // 우하단
                    else if (y == -1)
                    {
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusRT));
                        ShowFocus(false, GetFocusIndex(GameObjects.BG_FocusLB));
                        ShowFocus(true, GetFocusIndex(GameObjects.BG_FocusRB));
                    }
                }
            }
        }

        private void ShowFocus(bool show, int idx)
            => _bgFocuses[idx].SetActive(show);

        private float GetViewportScale 
            => Screen.height / 1920;

        private int GetFocusIndex(GameObjects focus)
        {
            if (focus == GameObjects.JoystickBG || focus == GameObjects.JoystickCursor)
            {
                //Debug.LogError($"{nameof(UI_Joystick)}, {nameof(GetFocusIndex)}, Input : \"{focus}\"");
                Util.LogError($"{nameof(UI_Joystick)}, {nameof(GetFocusIndex)}, Input : \"{focus}\"");
                return -1;
            }

            return (int)focus - 1;
        }
    }
}
