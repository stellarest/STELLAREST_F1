using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using static STELLAREST_F1.Define;

#if UNITY_EDITOR
namespace STELLAREST_F1
{
    public class DevScene : BaseScene
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            SceneType = Define.EScene.Unknown;
            LoadAsset();

            return true;
        }

        private void Test()
        {
            UI_Joystick joystick = Managers.UI.ShowBaseUI<UI_Joystick>();
            Managers.Map.LoadMap("SummerForestField_Test2");
            Managers.Map.Map.transform.position = Vector3.zero;

            {
                // RandPos Test
                int attemptCount = 0;
                Vector3Int randPos = new Vector3Int(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                while (Managers.Map.CanMove(randPos) == false && attemptCount++ < 100)
                {
                    if (attemptCount >= 100)
                    {
                        Debug.LogError("Failed set randPos");
                        Application.Quit();
                    }
                    randPos = new Vector3Int(Random.Range(-3, 3), Random.Range(-3, 3), 0); // Retry
                }

                HeroLeaderController leaderController = Managers.Object.SetHeroLeaderController();
                CameraController cam = Camera.main.GetComponent<CameraController>();
                Managers.Object.CameraController = cam;
                Hero firstHero = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);
                Managers.Map.MoveTo(firstHero, Vector3.zero, stopLerpToCell: true, forceMove: true);
                firstHero.InitialSpawnedCellPos = Vector3Int.zero;

                // 최대 맵 배치 동료 개수 : 7명 - (리더1, 팔로워6), 또는 9명(리더1, 팔로워8)
                // 부대 최대 3개. 5개까지 하고 싶지만 (전체 목표 캐릭터 30명), 캐릭터 만들 수 있는 컨텐츠 오링날듯..
                // 리더 포함, 최대 영웅 30명. 영웅 비활성화 기능 넣기? 안되면 부대 2개로 넣어야지 뭐.
                int memberCount = 0;
                int memberMaxCount = 7;
                while (memberCount < memberMaxCount)
                {
                    randPos = new Vector3Int(Random.Range(-4, 4), Random.Range(-4, 4), 0);
                    if (Managers.Map.CanMove(randPos) == false)
                        continue;
                        
                    memberCount++;
                    Hero hero = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);
                    hero.gameObject.name += $"___{memberCount.ToString()}";
                    Managers.Map.MoveTo(hero, randPos, stopLerpToCell: true, forceMove: true);
                    hero.InitialSpawnedCellPos = randPos;
                    // A* Test
                    // Managers.Map.MoveTo(hero, new Vector3Int(-1, -1, 0), forceMove: true);
                }

                leaderController.Leader = firstHero;
            }

            // Temp - Spawn Env (스폰 데이터 시트로 빼야함)
            // int maxX = Managers.Object.Monsters[0].CellPos.x;
            // int maxY = Managers.Object.Monsters[0].CellPos.y;
            // Env env = Managers.Object.Spawn<Env>(EObjectType.Env, ReadOnly.Numeric.DataID_Env_AshTree);
            // env.SetCellPos(new Vector3Int(Random.Range(maxX - 2, maxX + 2), Random.Range(maxY - 2, maxY + 2)), stopLerpToCell: true, forceMove: true);
            if (Managers.Object.HeroLeaderController == null)
            {
                Debug.LogError("This game absolutely requires at least one Hero Leader Controller.");
                Application.Quit();
            }
        }

        // private IEnumerator CoSpawnHero(Vector3 spawnPos)
        // {
        //     yield return new WaitForSeconds(2f);
        //     Hero hero = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);
        //     Managers.Map.MoveTo(hero, spawnPos, true);
        // }

        private int GetRandEnvTree
            => UnityEngine.Random.Range(ReadOnly.Numeric.DataID_Env_AshTree, ReadOnly.Numeric.DataID_Env_YewTree + 1);

        private int GetRandEnvRock
            => UnityEngine.Random.Range(ReadOnly.Numeric.DataID_Env_CopperRock, ReadOnly.Numeric.DataID_Env_ZincRock + 1);

        private void LoadAsset()
        {
            Managers.Resource.LoadAllAsync<Object>(label: ReadOnly.String.PreLoad, callback: delegate (string key, int count, int totalCount)
            {
                Debug.Log($"Key Loaded : {key}, Current : {count} / Total : {totalCount}");
                if (count == totalCount)
                {
                    Managers.Data.Init();
                    Managers.Sprite.Init();
                    Managers.Animation.Init();
                    Test();
                }
            });
        }

        public override void Clear() { }
    }
}
#endif