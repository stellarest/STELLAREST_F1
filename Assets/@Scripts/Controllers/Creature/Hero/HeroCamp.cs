using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroCamp : BaseObject
    {
        public float Speed { get; set; } = 5.0f;
        public Transform Pivot { get; private set; } = null;
        public Transform Destination { get; private set; } = null;

        private SpriteRenderer _circleSPR = null;
        private SpriteRenderer _arrowSPR = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.HeroCamp;

            Collider.includeLayers = 1 << (int)ELayer.Obstacle;
            Collider.excludeLayers = 1 << (int)ELayer.Monster | (1 << (int)ELayer.Hero);

            Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;

            Pivot = transform.GetChild(0).transform;
            Destination = Pivot.GetChild(0).transform;

            _circleSPR = GetComponent<SpriteRenderer>();
            _arrowSPR = Destination.GetComponent<SpriteRenderer>();

            ShowArrowCircle(false);
            SortingGroup.sortingOrder = ReadOnly.Numeric.SortingLayer_HeroCamp;
            return true;
        }

        private void Update()
        {
            transform.Translate(MoveDir * Time.deltaTime * Speed);
        }

        private void OnMoveDirChanged(Vector2 dir)
        {
            MoveDir = dir;
            if (dir != Vector2.zero)
            {
                float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
                Pivot.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            switch (joystickState)
            {
                case EJoystickState.PointerDown:
                    ShowArrowCircle(true);
                    break;

                case EJoystickState.PointerUp:
                    ShowArrowCircle(false);
                    break;
            }
        }

        private void ShowArrowCircle(bool show)
        {
            _circleSPR.enabled = show;
            _arrowSPR.enabled = show;
        }
    }
}