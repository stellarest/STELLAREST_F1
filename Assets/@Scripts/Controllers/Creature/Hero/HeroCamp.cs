using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // 캠프는 그냥 리더를 쫓아가는 것?
    // 단점은 피벗과 포인터가 전부 히어로에 붙어야 한다는 것인데.
    // 아니면 왓다갓다하면되지 뭐. 리더 마크랑.
    // 이거를 하면 장점이 새미월같은거 안찍어도 됨. 어차피 리더로 움직이는거라.
    // 내가 봤을때 리더는 아예 길찾기 금지. 이게 맞음...
    public class HeroCamp : BaseObject
    {
        public float CampMovementSpeed { get; set; } = 5.0f;
        public Transform Pivot { get; private set; } = null;
        private SpriteRenderer _pivotSPR = null;

        public Transform Pointer { get; private set; } = null; // Prev: Destination
        private SpriteRenderer _pointerSPR = null;

        [Header("CampMoveDir")]
        [SerializeField]
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

                Managers.Object.CameraController.Target = value;
                // Managers.Game.SetHeroLeader(Managers.Object.LeaderMark, value);
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

        // 리더도 길찾기 해야함.
        private Queue<Vector3Int> _leaderPath = new Queue<Vector3Int>();
        private void Update()
        {
            if (Managers.Map == null)
                return;

            Vector3 dir = _movementDirection * CampMovementSpeed * Time.deltaTime;
            Vector3 newPos = transform.position + dir;
            transform.position = newPos;
            
            if (Managers.Map.CanMove(newPos, ignoreObjects: true, ignoreSemiWall: true) == false)
                return;

            // 이거 자체를 Leader로 움직여야하고..
            // 정확하게 하려면 Arrow 자체를 리더 머리 위에 붙여야함.
            // 그리고 컨트롤러가 움직이면 Arrow도 회전해야하고.
            // 흠...
            // transform.position = newPos;
            // Vector3Int destPos = Managers.Map.WorldToCell(newPos);
            // List<Vector3Int> path = Managers.Map.FindPath(startCellPos: Leader.CellPos, destCellPos: destPos, maxDepth:ReadOnly.Numeric.HeroDefaultMoveDepth);
            // if (path.Count < 2)
            //     return;

            // Leader.FindPathAndMoveToCellPos(destPos: newPos, maxDepth: ReadOnly.Numeric.HeroDefaultMoveDepth);
            // 속도에 따라서 한 칸씩 움직여야 할 것 같은데.
            //Managers.Map.MoveTo(Leader, transform.position);
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