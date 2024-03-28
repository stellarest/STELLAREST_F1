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
            
            Monster chicken = Managers.Object.Spawn<Monster>(Vector3.one, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Chicken);
            //Monster bunny = Managers.Object.Spawn<Monster>(Vector3.one, EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Bunny);

            // Monster turkey = Managers.Object.Spawn<Monster>(new Vector3(chicken.transform.position.x + 1, chicken.transform.position.y + 1, 0), EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Turkey);
            // Monster pug = Managers.Object.Spawn<Monster>(new Vector3(turkey.transform.position.x + 1, turkey.transform.position.y + 1, 0), EObjectType.Monster, ReadOnly.Numeric.DataID_Monster_Pug);
            // Addressables.LoadAssetAsync<T>
            // Sprite armor = Managers.Resource.Load<Sprite>("BanditLightArmor_Torso.sprite");
            // Sprite armor_cloned = UnityEngine.Component.Instantiate(armor);
            // armor_cloned.name = "Cloned_Armor_Test";
            // hero.Torso_Armor_Temp.GetComponent<SpriteRenderer>().sprite = armor_cloned;
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
