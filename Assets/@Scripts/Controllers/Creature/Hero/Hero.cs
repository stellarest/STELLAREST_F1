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
        //private Vector2 _moveDir = Vector2.zero;

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

        public override void SetInfo(int dataID)
        {
            base.SetInfo(dataID);
            if (HeroAnim == null && HeroBody == null)
            {
                HeroBody = new HeroBody(this, dataID);
                HeroAnim = CreatureAnim as HeroAnimation;
                HeroAnim.SetInfo(dataID, this);
                Managers.Sprite.SetInfo(dataID, target: this);
                SetCreatureFromData(dataID);
            }

            RefreshCreature();
        }

        protected override void SetCreatureFromData(int dataID)
        {
            HeroData = Managers.Data.HeroDataDict[dataID];
            gameObject.name += $"_{HeroData.DescriptionTextID}";
            Collider.radius = HeroData.ColliderRadius;
            Speed = HeroData.MovementSpeed;

            /*
                TODO : Set Hero Stat..
            */
        }

        protected override void RefreshCreature()
        {
            base.RefreshCreature();
            Speed = HeroData.MovementSpeed;
            LookAtDir = ELookAtDirection.Right;
        }

        private void Update()
        {
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
