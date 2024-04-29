using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using Unity.VisualScripting;
using UnityEngine;
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
            if (Input.GetKeyDown(KeyCode.Q))
            {
                int randSpawnId = ReadOnly.Numeric.DataID_Hero_Paladin;
                // if (65f >= UnityEngine.Random.Range(0f, 100f))
                //     randSpawnId += 10;

                Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -1f, 1f);
                Hero hero = Managers.Object.Spawn<Hero>(spawnPos, EObjectType.Hero, randSpawnId);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -5f, 5f);
                Monster mon = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Chicken);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -7f, 7f);
                Env env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, GetRandEnvTree);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -10f, 10f);
                Env env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, GetRandEnvRock);
            }
        }

        private void Test()
        {
            UI_Joystick joystick = Managers.UI.ShowBaseUI<UI_Joystick>();
            HeroCamp camp = Managers.Object.Spawn<HeroCamp>(Vector3.zero, EObjectType.HeroCamp);
            CameraController cam = Camera.main.GetComponent<CameraController>();
            cam.Target = camp;

            {
                Vector3 spawnPos = new Vector3(camp.transform.position.x - 5f, camp.transform.position.y - 5f, 0f);
                Hero hero = Managers.Object.Spawn<Hero>(spawnPos, EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);

            }
        }

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
