using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroCamp : BaseObject
    {
        public float CampMovementSpeed { get; set; } = 5.0f;
        public Transform Pivot { get; private set; } = null;
        private SpriteRenderer _pivotSPR = null;

        public Transform Pointer { get; private set; } = null; // Prev: Destination
        private SpriteRenderer _pointerSPR = null;

        private Vector3 _movementDirection = Vector3.zero;

        private Hero _leader = null;
        public Hero Leader
        {
            get => _leader;
            set
            {
                if (_leader == null)
                {
                    _leader = value;
                    _leader.IsLeader = true;
                }
                else if (_leader != value)
                {
                    _leader.IsLeader = false;
                    _leader = value;
                    _leader.IsLeader = true;
                }
                else
                {
                    Debug.Log("Same leader == value");
                    return;
                }

                // 이 부분 너무 많아지면 이벤트 등록해서 해도 될듯
                Managers.Object.CameraController.Target = value;
                Managers.Game.SetHeroLeader(Managers.Object.LeaderMark, value);
            }
        }
        
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
            Pointer = Pivot.GetChild(0).transform;

            _pivotSPR = GetComponent<SpriteRenderer>();
            _pointerSPR = Pointer.GetComponent<SpriteRenderer>();

            ShowArrowCircle(true);
            SortingGroup.sortingOrder = ReadOnly.Numeric.SortingLayer_HeroCamp;
            return true;
        }

        private void Update()
        {
            if (Managers.Map == null)
                return;

            Vector3 dir = _movementDirection * CampMovementSpeed * Time.deltaTime;
            Vector3 newPos = transform.position + dir;
            if (Managers.Map.CanMove(newPos, ignoreObjects: true, ignoreSemiWall: true) == false)
                return;

            transform.position = newPos;
        }

        private void OnMoveDirChanged(Vector2 nDir) // normalized Dir
        {
            _movementDirection = nDir;
            if (nDir != Vector2.zero)
            {
                float angle = Mathf.Atan2(-nDir.x, nDir.y) * Mathf.Rad2Deg;
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
            _pivotSPR.enabled = show;
            _pointerSPR.enabled = show;
        }
    }
}