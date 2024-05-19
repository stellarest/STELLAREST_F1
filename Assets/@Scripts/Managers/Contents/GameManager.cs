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

        public Transform SetHeroLeader(Transform leaderMark, Hero leader) // 이걸 여기서 할 필요가 있을끼???
        {
            if (leader.IsValid() == false)
            {
                Hero nextHero = null;
                for (int i = 0; i < Managers.Object.Heroes.Count; ++i)
                {
                    nextHero = Managers.Object.Heroes[i];
                    if (nextHero.IsValid())
                    {
                        leader = nextHero;
                        break;
                    }
                }

                if (nextHero == null)
                {
                    Debug.LogWarning($"{nameof(GameManager)}, {nameof(SetHeroLeader)}, None of heroes.");
                    return null;
                }
            }

            leaderMark.SetParent(leader.transform);
            leaderMark.transform.localPosition = Vector3.up * 2.5f;
            return leaderMark;
        }

        // TEMP: Replace Heroes
        public float TestDist = 3f;
        public void ReplaceHeroes() // param ex: ReplaceMode
        {
            if (DevManager.Instance.ReplaceMode == EReplaceHeroMode.FollowBaseCamp)
            {
                Debug.LogWarning($"{nameof(GameManager)}, {nameof(ReplaceHeroes)}, type: \"{DevManager.Instance.ReplaceMode}\"");
                return;
            }
            if (Managers.Object.Heroes.Count < 2)
            {
                Debug.LogWarning($"{nameof(GameManager)}, {nameof(ReplaceHeroes)}, count: \"{Managers.Object.Heroes.Count}\"");
                return;
            }
            if (Managers.Object.Camp.Leader == null)
            {
                Debug.LogWarning($"{nameof(GameManager)}, {nameof(ReplaceHeroes)}, null ref: \"{Managers.Object.Camp.Leader}\"");
                return;
            }

            // 여기가 잘못된듯. 둘 다 같은 위치에 가려고해서 겹쳐 보이는 것 같음. 무조건 각각 다른 위치로 줘야함.
            List<Hero> members = new List<Hero>();
            foreach (var hero in Managers.Object.Heroes)
            {
                if (hero.IsLeader == false)
                    members.Add(hero);
            }

            //TestDist = (float)DevManager.Instance.ReplaceMode;
            TestDist = DevManager.Instance.TestReplaceDistance;
            if (TestDist < 1f)
            {
                Debug.LogWarning("### You have to set over \"1f\" distance. ###");
                return;
            }
            
            Hero leader = Managers.Object.Camp.Leader;
            for (int i = 0; i < members.Count; ++i)
            {
                float degree = 360f * i / members.Count;
                degree = Mathf.PI / 180f * degree;
                float x = leader.transform.position.x + Mathf.Cos(degree) * TestDist;
                float y = leader.transform.position.y + Mathf.Sin(degree) * TestDist;

                Vector3Int cellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                //Vector3Int cellPos = new Vector3Int(1, 0, 0); // A* Test

                // ************************************************************
                // 만약 1번 인덱스가 cellPos로 이동하려고 했음.
                // 그리고 2번 인덱스가 다음 cellPos로 이동하려고 했는데
                // 1번 인덱스가 이동하려고 했던 곳이 canMove == false였던 곳이라 이동을 실패해서 다른 길을 찾은것임.
                // 근데 하필 그 길에 2번 인덱스가 가려고 했던 길이었음.
                // 그러면 2번 인덱스가 먼저 선점을 했을텐데 뭐임???
                Debug.Log($"<color=white>CellPos[{i}]: {cellPos}");
                members[i].ReplaceHero(cellPos); ; // Leader 제외하고 전부 이동.

                // 99.9999999% PathFind가 한번만 호출되서 그런것임 !!!!!!!!!!
                //members[i].FindPathAndMoveToCellPos(cellPos, 999);
                // Managers.Map.MoveTo(members[i], cellPos);
            }
        }
    }
}
