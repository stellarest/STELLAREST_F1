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
            // Managers.Map.LoadMap(ReadOnly.String.SummerForest_Field_Temp);
            Managers.Map.LoadMap("TempMap02");
            Managers.Map.Map.transform.position = Vector3.zero;

            {
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

                HeroCamp camp = Managers.Object.Spawn<HeroCamp>(EObjectType.HeroCamp);
                camp.SetCellPos(randPos, forceMove: true);
                CameraController cam = Camera.main.GetComponent<CameraController>();
                cam.Target = camp;

                Hero hero = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);
                Managers.Map.MoveTo(hero, randPos, forceMove: true);

                /*
                        public static readonly int DataID_Hero_Paladin = 101000;
                        public static readonly int DataID_Hero_Archer = 101010;
                        public static readonly int DataID_Hero_Wizard = 101020;
                */

                int randID = 0;
                for (int i = 0; i < 10; ++i)
                {
                    float rand = UnityEngine.Random.Range(0, 100f);
                    if (rand >= 0 && rand <= 50f)
                        randID = ReadOnly.Numeric.DataID_Hero_Paladin;
                    else if (rand > 50f && rand < 75)
                        randID = ReadOnly.Numeric.DataID_Hero_Archer;
                    else
                        randID = ReadOnly.Numeric.DataID_Hero_Wizard;

                    randPos = new Vector3Int(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                    if (Managers.Map.CanMove(randPos) == false)
                        continue;

                    hero = Managers.Object.Spawn<Hero>(EObjectType.Hero, randID);
                    Managers.Map.MoveTo(creature: hero, randPos, forceMove: false);
                }

                // for (int i = 0; i < 10; ++i)
                // {
                //     randPos = new Vector3Int(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                //     if (Managers.Map.CanMove(randPos) == false)
                //         continue;

                //     Hero hero = Managers.Object.Spawn<Hero>(EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);
                //     Managers.Map.MoveTo(hero, randPos, forceMove: true);
                // }
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
