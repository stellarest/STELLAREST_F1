using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.Diagnostics;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Hero : Creature
    {
        public Data.HeroData HeroData { get; private set; } = null;
        public HeroBody HeroBody { get; private set; } = null;
        public HeroAnimation HeroAnim { get; private set; } = null;
        private Vector2 _moveDir = Vector2.zero;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Hero;
            CreatureState = ECreatureState.Idle;
            Speed = 5.0f;

            Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;

            if (BaseAnim != null)
                HeroAnim = BaseAnim as HeroAnimation;

            HeroBody = new HeroBody(this);
            return true;
        }

        public override void SetInfo(int dataID)
        {
            base.SetInfo(dataID);
            Managers.Sprite.SetInfo(dataID, this);

            Data.HeroSpriteData data = Managers.Data.HeroesSpritesDict[ReadOnly.Numeric.DataID_Lancer];
            HeroData = Managers.Data.HeroesDict[dataID];
        }

        public EHeroHead FindHeadTest = EHeroHead.None;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
            }

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
