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

        private void Test()
        {
            UI_Joystick joystick = Managers.UI.ShowBaseUI<UI_Joystick>();
            //Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero, ReadOnly.Numeric.DataID_Lancer);

            Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero, EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Paladin);
            CameraController cam = Camera.main.GetComponent<CameraController>();
            cam.Target = hero;

            // Spawn Monsters
            {
                // Vector3 spawnPos = Util.MakeSpawnPosition(hero, 4f, 8f);
                // Monster mon = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Chicken);

                // spawnPos = Util.MakeSpawnPosition(hero, 4f, 8f);
                // mon = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Turkey);

                // spawnPos = Util.MakeSpawnPosition(hero, 4f, 8f);
                // mon = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Bunny);

                // spawnPos = Util.MakeSpawnPosition(hero, 4f, 8f);
                // mon = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Pug);
            }

            // Spawn Envs
            {
                // Trees
                // Vector3 spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // Env env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_AshTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_BlackOakTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_GreenAppleTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_IvyTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_ManticoreTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_MapleTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_OakTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_RedAppleTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_RedSandalTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_WillowTree);

                // spawnPos = Util.MakeSpawnPosition(hero, -2f, -2f);
                // env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_YewTree);
            }

            {
                // Rocks
                Vector3 spawnPos = Util.MakeSpawnPosition(hero, 1f, 2f);
                Env env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_SilverRock);

                spawnPos = new Vector3(env.transform.position.x + 2f, env.transform.position.y, 0f);
                env = Managers.Object.Spawn<Env>(spawnPos, EObjectType.Env, ReadOnly.Numeric.DataID_Env_GoldRock);
            }
        }

        private void LoadAsset()
        {
            Managers.Resource.LoadAllAsync<Object>(label: ReadOnly.String.PreLoad, callback: delegate (string key, int count, int totalCount)
            {
                Debug.Log($"Key Loaded : {key}, Current : {count} / Total : {totalCount}");
                if (count == totalCount)
                {
                    Managers.Data.Init();
                    Test();
                }
            });
        }

        public override void Clear() { }
    }
}
#endif
