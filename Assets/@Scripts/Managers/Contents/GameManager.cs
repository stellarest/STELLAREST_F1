using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class GameManager
    {
        private Vector2 _moveDir = Vector2.zero;
        public Vector2 MoveDir
        {
            get => _moveDir;
            set
            {
                _moveDir = value;
                OnMoveDirChangedHandler?.Invoke(value);
            }
        }

        private EJoystickState _joystickState = EJoystickState.PointerUp;
        public EJoystickState JoystickState
        {
            get => _joystickState;
            set
            {
                _joystickState = value;
                OnJoystickStateChangedHandler?.Invoke(value);
            }
        }

        public event Action<Vector2> OnMoveDirChangedHandler = null;
        public event Action<EJoystickState> OnJoystickStateChangedHandler = null;
    }
}
