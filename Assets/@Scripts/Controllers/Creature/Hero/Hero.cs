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

        private HeroBody _heroBody = null;
        public HeroBody HeroBody
        {
            get => _heroBody;
            private set
            {
                _heroBody = value;
                if (CreatureBody == null)
                    CreatureBody = value;
            }
        }

        public HeroAnimation HeroAnim { get; private set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Hero;

            Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;

            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                Refresh();
                return false;
            }

            HeroBody = new HeroBody(this, dataID);
            HeroAnim = CreatureAnim as HeroAnimation;
            HeroAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, target: this);

            HeroData = Managers.Data.HeroDataDict[dataID];
            gameObject.name += $"_{HeroData.DescriptionTextID}";

            Collider.radius = HeroData.ColliderRadius;
            Speed = HeroData.MovementSpeed;
            Refresh();
            
            return true;
        }

        protected override void Refresh()
        {
            base.Refresh();
            Speed = HeroData.MovementSpeed;
            LookAtDir = ELookAtDirection.Right;
        }

        private void Update()
        {
            // EMOJI TEST
            // if (Input.GetKeyDown(KeyCode.Q))
            // {
            //     CreatureState = ECreatureState.Attack;
            // }

            float moveDistPerFrame = Speed * Time.deltaTime;
            transform.TranslateEx(MoveDir * moveDistPerFrame);
        }

        private void OnMoveDirChanged(Vector2 dir)
            => MoveDir = dir;

        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            switch (joystickState)
            {
                case EJoystickState.PointerDown:
                    break;

                case EJoystickState.PointerUp:
                    CreatureState = ECreatureState.Idle;
                    break;

                case EJoystickState.Drag:
                    CreatureState = ECreatureState.Move;
                    break;

                default:
                    break;
            }
        }
    }
}
