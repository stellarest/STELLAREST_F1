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

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.Q))
            // {
            //     int randSpawnId = ReadOnly.Numeric.DataID_Hero_Paladin;
            //     // if (65f >= UnityEngine.Random.Range(0f, 100f))
            //     //     randSpawnId += 10;

            //     Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -1f, 1f);
            //     Hero hero = Managers.Object.Spawn<Hero>(spawnPos, EObjectType.Hero, randSpawnId);
            // }

            // if (Input.GetKeyDown(KeyCode.W))
            // {
            //     Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -5f, 5f);
            //     Monster mon = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Chicken);
            // }

            // if (Input.GetKeyDown(KeyCode.E))
            // {
            //     Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -7f, 7f);
            //     Env env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, GetRandEnvTree);
            // }

            // if (Input.GetKeyDown(KeyCode.R))
            // {
            //     Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -10f, 10f);
            //     Env env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, GetRandEnvRock);
            // }
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

                // HeroCamp camp = Managers.Object.Spawn<HeroCamp>(EObjectType.HeroCamp);
                // camp.SetCellPos(randPos, forceMove: true);
                HeroLeaderController leaderController = Managers.Object.SetHeroLeaderController();

                CameraController cam = Camera.main.GetComponent<CameraController>();
                Managers.Object.CameraController = cam;

                Hero firstHero = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);
                // Leader는 Map.Move를 하지 않는다. 하면 cells에 Add가 되버려서 못가는 곳으로 인식됨.
                // Leader는 주변 Cell에 갈 수 있는지 없는지만 체크함녀 된다.
                
                firstHero.transform.position = Managers.Map.WorldToCell(Vector3.zero);
                //camp.Leader = hero;
                //cam.Target = hero;
                // Managers.Map.MoveTo(hero, randPos, forceMove: true);
                
                // // A* Test
                // Managers.Map.MoveTo(hero, new Vector3Int(-1, 0, 0), forceMove: true);

                // Env env = Managers.Object.Spawn<Env>(EObjectType.Env, ReadOnly.Numeric.DataID_Env_AshTree);
                // env.transform.position = Vector3.zero;
                // 최대 맵 배치 동료 개수 : 7명 - (리더1, 팔로워6), 또는 9명(리더1, 팔로워8)
                int memberCount = 0;
                int memberMaxCount = 6;
                while (memberCount < memberMaxCount)
                {
                    randPos = new Vector3Int(Random.Range(-4, 4), Random.Range(-4, 4), 0);
                    if (Managers.Map.CanMove(randPos) == false)
                        continue;
                        
                    memberCount++;
                    Hero hero = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);
                    hero.gameObject.name += $"___{memberCount.ToString()}";
                    Managers.Map.MoveTo(hero, randPos, forceMove: true);
                    // A* Test
                    // Managers.Map.MoveTo(hero, new Vector3Int(-1, -1, 0), forceMove: true);
                }

                leaderController.Leader = firstHero;
                leaderController.MembersTemp = Managers.Object.Heroes;
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
