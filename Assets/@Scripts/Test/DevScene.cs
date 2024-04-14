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
                Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -1f, 1f);
                Hero hero = Managers.Object.Spawn<Hero>(spawnPos, EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -3f, 3f);
                Monster mon = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Chicken);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Vector3 spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -3f, 3f);
                Env env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_AshTree);

                spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -3f, 3f);
                env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_RedAppleTree);

                spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -3f, 3f);
                env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_CopperRock);

                spawnPos = Util.MakeSpawnPosition(Managers.Object.Camp, -3f, 3f);
                env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_GoldRock);
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

            // Monster mon = Managers.Object.Spawn<Monster>(new Vector3(camp.transform.position.x - 2f, camp.transform.position.y - 2f, 0f),
            //                                              EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Chicken);
        }

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
