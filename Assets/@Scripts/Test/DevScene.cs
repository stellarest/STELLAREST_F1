using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
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

            Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero, EObjectType.Hero, ReadOnly.Numeric.DataID_Hero_Lancer);
            CameraController cam = Camera.main.GetComponent<CameraController>();
            cam.Target = hero;

            Vector3 spawnPos = Util.MakeSpawnPosition(hero, 15f, 20f);

            {
                Monster chicken = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Chicken);
                // Vector3 spawnPos = Util.MakeSpawnPosition(chicken, 3f, 5f);
                // Monster turkey = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Turkey);

                // spawnPos = Util.MakeSpawnPosition(chicken, 3f, 5f);
                // Monster bunny = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Bunny);

                // spawnPos = Util.MakeSpawnPosition(chicken, 3f, 5f);
                // Monster pug = Managers.Object.Spawn<Monster>(spawnPos, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Pug);
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
