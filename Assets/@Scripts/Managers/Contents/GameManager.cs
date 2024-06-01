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
        public bool IsGameOver => Managers.Object.Heroes.Count == 0 ? true : false;

        public void ChangeHeroLeader()
        {
            HeroLeaderController leaderController = Managers.Object.HeroLeaderController;
            if (leaderController == null)
                return;

            leaderController.StartCoChangeRandomHeroLeader();
        }
    }
}

/*
        public void ReplaceHeroes()
        {
            List<Hero> members = new List<Hero>();
            foreach (var hero in Managers.Object.Heroes)
            {
                if (hero.IsLeader == false)
                    members.Add(hero);
            }

            float _replaceHeroesDistance_Test = DevManager.Instance.TestReplaceDistance;
            if (_replaceHeroesDistance_Test < 1f)
            {
                Debug.LogWarning("### You have to set over \"1f\" distance. ###");
                return;
            }
            
            // Hero leader = Managers.Object.Camp.Leader;
            Hero leader = Managers.Object.HeroLeaderController.Leader;
            for (int i = 0; i < members.Count; ++i)
            {
                float degree = 360f * i / members.Count;
                degree = Mathf.PI / 180f * degree;
                float x = leader.transform.position.x + Mathf.Cos(degree) * _replaceHeroesDistance_Test;
                float y = leader.transform.position.y + Mathf.Sin(degree) * _replaceHeroesDistance_Test;

                Vector3Int cellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                members[i].ReplaceHero(cellPos);
            }
        }
*/