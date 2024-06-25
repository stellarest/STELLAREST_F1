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
                // --- HEROES TEST
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
                Hero firstHero = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Paladin);
                Managers.Map.MoveTo(firstHero, Vector3.zero, stopLerpToCell: true, forceMove: true);
                firstHero.InitialSpawnedCellPos = Vector3Int.zero;

                // // Leader:1, Maximum Members:6
                // // Paladin - Archer, Wizard, Lancer, Gunner
                // int memberCount = 0;
                // int memberMaxCount = 1;
                // // int dataID = ReadOnly.DataAndPoolingID.DNPID_Hero_Archer;
                // int dataID = ReadOnly.DataAndPoolingID.DNPID_Hero_Paladin;
                // while (memberCount < memberMaxCount)
                // {
                //     randPos = new Vector3Int(Random.Range(-4, 4), Random.Range(-4, 4), 0);
                //     if (Managers.Map.CanMove(randPos) == false)
                //         continue;

                //     memberCount++;
                //     Hero hero = Managers.Object.Spawn<Hero>(EObjectType.Hero, dataID);
                //     //dataID -= 10;

                //     hero.gameObject.name += $"___{memberCount.ToString()}";
                //     Managers.Map.MoveTo(hero, randPos, stopLerpToCell: true, forceMove: true);
                //     hero.InitialSpawnedCellPos = randPos;
                // }
                // leaderController.Leader = firstHero;

                int paladin = 3;
                int archer = 0;
                int wizard = 0;
                int lancer = 0;
                int gunner = 0;

                int current = 0;
                int total = paladin + archer + wizard + lancer + gunner;
                Hero heroMember = null;
                while (current < total)
                {
                    randPos = new Vector3Int(Random.Range(-4, 4), Random.Range(-4, 4), 0);
                    if (Managers.Map.CanMove(randPos) == false)
                        continue;

                    if (paladin > 0)
                    {
                        paladin--;
                        heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Paladin);
                    }
                    else if (archer > 0)
                    {
                        archer--;
                        heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Archer);
                    }
                    else if (wizard > 0)
                    {
                        wizard--;
                        heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Wizard);
                    }
                    else if (lancer > 0)
                    {
                        lancer--;
                        heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Lancer);
                    }
                    else if (gunner > 0)
                    {
                        gunner--;
                        heroMember = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.DataAndPoolingID.DNPID_Hero_Gunner);
                    }

                    current++;
                    heroMember.gameObject.name += $"___{current.ToString()}";
                    Managers.Map.MoveTo(heroMember, randPos, stopLerpToCell: true, forceMove: true);
                    heroMember.InitialSpawnedCellPos = randPos;
                }

                leaderController.Leader = firstHero;
            }

            {   // --- ENV SINGLE TEST
                // // // // Temp - Spawn Env (스폰 데이터 시트로 빼야함)
                // Env env = Managers.Object.Spawn<Env>(EObjectType.Env, ReadOnly.DataAndPoolingID.DNPID_Env_AshTree);
                // // int x = Managers.Object.Monsters[0].CellPos.x;
                // // int y = Managers.Object.Monsters[0].CellPos.y;
                // //Vector3Int randPos = new Vector3Int(Random.Range(x + 3, x + 5), Random.Range(y + 3, y + 5));
                // Vector3Int randPos = new Vector3Int(-6, 11, 0);
                // env.SetCellPos(cellPos: randPos, stopLerpToCell: true, forceMove: true);
                // env.InitialSpawnedCellPos = randPos;
                // env.UpdateCellPos();
            }

            // {
            //     // --- ENV SPREAD
            //     int envCount = 0;
            //     int envMaxCount = 10;
            //     int minX = Managers.Map.MinX;
            //     int maxX = Managers.Map.MaxX;
            //     int minY = Managers.Map.MinY;
            //     int maxY = Managers.Map.MaxY;
            //     int attempCount = 0;
            //     Vector3Int cellPos = new Vector3Int(Random.Range(minX, maxX), Random.Range(minY, maxY));
            //     while (envCount < envMaxCount)
            //     {
            //         if (attempCount++ > 100)
            //             break;

            //         if (Managers.Map.CanMove(cellPos) == false)
            //         {
            //             cellPos = new Vector3Int(Random.Range(minX, maxX), Random.Range(minY, maxY));
            //             continue;
            //         }

            //         bool envTree = true;
            //         if (Random.Range(0, 100) > 50)
            //             envTree = false;

            //         int spawnDataID = -1;
            //         if (envTree)
            //             spawnDataID = GetRandEnvTree;
            //         else
            //             spawnDataID = GetRandEnvRock;

            //         Env env = Managers.Object.Spawn<Env>(EObjectType.Env, spawnDataID);
            //         env.SetCellPos(cellPos: cellPos, stopLerpToCell: true, forceMove: true);
            //         env.InitialSpawnedCellPos = cellPos;
            //         env.UpdateCellPos();
            //         ++envCount;
            //     }
            // }

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
            => UnityEngine.Random.Range(ReadOnly.DataAndPoolingID.DNPID_Env_AshTree, ReadOnly.DataAndPoolingID.DNPID_Env_YewTree + 1);

        private int GetRandEnvRock
            => UnityEngine.Random.Range(ReadOnly.DataAndPoolingID.DNPID_Env_CopperRock, ReadOnly.DataAndPoolingID.DNPID_Env_ZincRock + 1);

        private void LoadAsset()
        {
            Managers.Resource.LoadAllAsync<Object>(label: ReadOnly.String.PreLoad, callback: delegate (string key, int count, int totalCount)
            {
                Debug.Log($"Key Loaded : {key}, Current : {count} / Total : {totalCount}");
                if (count == totalCount)
                {
                    Managers.Data.Init();
                    Managers.Sprite.Init();
                    //Managers.Animation.Init();
                    Test();
                }
            });
        }

        public override void Clear() { }
    }
}
#endif