using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Hero : Creature
    {
        private Vector2 _moveDir = Vector2.zero;
        public Transform Torso_Armor_Temp = null;
        private HeroAnimation _heroAnim = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            CreatureType = ECreatureType.Hero;
            CreatureState = ECreatureState.Idle;
            Speed = 5.0f;

            Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;

            if (BaseAnim != null)
                _heroAnim = BaseAnim as HeroAnimation;

            // TEMP
            Torso_Armor_Temp = Util.FindChild<Transform>(gameObject, "Torso_Armor", true, true);
            return true;
        }

        private void Update()
        {
            float distancePerFrame = Speed * Time.deltaTime;
            transform.TranslateEx(_moveDir * distancePerFrame);
        }

        private void OnMoveDirChanged(Vector2 dir)
        {
            _moveDir = dir;
        }

        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            switch (joystickState)
            {
                case EJoystickState.PointerDown:
                    CreatureState = ECreatureState.Move;
                    break;

                case EJoystickState.PointerUp:
                    CreatureState = ECreatureState.Idle;
                    break;

                case EJoystickState.Drag:
                    break;

                default:
                    break;
            }
        }
    }
}
