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

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.HeroCamp;

            Collider.includeLayers = 1 << (int)ELayer.Obstacle;
            Collider.excludeLayers = 1 << (int)ELayer.Monster | (1 << (int)ELayer.Hero);

            Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;

            Pivot = transform.GetChild(0).transform;
            Destination = Pivot.GetChild(0).transform;

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
    }
}