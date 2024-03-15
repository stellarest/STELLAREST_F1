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

        private void Test()
        {
            UI_Joystick joystick = Managers.UI.ShowBaseUI<UI_Joystick>();
        }

        public override void Clear() { }
    }
}
#endif
