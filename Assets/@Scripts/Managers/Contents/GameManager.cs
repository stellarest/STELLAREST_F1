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
        public event Action<Vector2> OnMoveDirChangedHandler = null;

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
        public event Action<EJoystickState> OnJoystickStateChangedHandler = null;

        
        // TEMP: Replace Heroes
        public float TestDist = 3f;
        public void ReplaceHeroes() // param ex: ReplaceMode
        {
            if (DevManager.Instance.ReplaceMode == EReplaceHeroesMode.FollowBaseCamp)
            {
                Debug.LogWarning($"Failed to ReplaceMode - input: \"{DevManager.Instance.ReplaceMode}\"");
                return;
            }
            TestDist = (float)DevManager.Instance.ReplaceMode;

            Hero leader = null;
            List<Hero> members = new List<Hero>();

            for (int i = 0; i < Managers.Object.Heroes.Count; ++i)
            {
                if (Managers.Object.Heroes[i].Leader)
                {
                    if (Managers.Object.Heroes[i].CreatureState == ECreatureState.Move)
                    {
                        Debug.LogWarning("Failed Replace Heroes, Leader State: Move");
                        return;
                    }

                    leader = Managers.Object.Heroes[i];
                }
                else
                    members.Add(Managers.Object.Heroes[i]);
            }

            if (members.Count == 0)
            {
                Debug.LogWarning($"Failed to ReplaceMode - Member count: \"{members.Count}\"");
                return;
            }

            for (int i = 0; i < members.Count; ++i)
            {
                float degree = 360f * i / members.Count;
                degree = Mathf.PI / 180f * degree;
                float x = leader.transform.position.x + Mathf.Cos(degree) * TestDist;
                float y = leader.transform.position.y + Mathf.Sin(degree) * TestDist;

                Vector3Int cellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                //Vector3Int cellPos = new Vector3Int(1, 0, 0); // A* Test
                members[i].ReplaceHero(cellPos);

                // 99.9999999% PathFind가 한번만 호출되서 그런것임 !!!!!!!!!!
                //members[i].FindPathAndMoveToCellPos(cellPos, 999);
                // Managers.Map.MoveTo(members[i], cellPos);
            }

            Debug.Log("Replace Heroes");
        }
    }
}
