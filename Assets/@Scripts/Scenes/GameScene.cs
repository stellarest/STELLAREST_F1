using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class GameScene : BaseScene
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            SceneType = Define.EScene.GameScene;
            Debug.Log($"ENTERED GAME SCENE !!");

            GameObject map = Managers.Resource.Instantiate(ReadOnly.String.BaseMap);
            map.transform.position = Vector3.zero;
            map.name = $"@{ReadOnly.String.BaseMap}";

            Hero hero = Managers.Resource.Instantiate(ReadOnly.String.Hero).GetComponent<Hero>();
            hero.transform.position = Vector3.zero;

            return true;
        }

        public override void Clear() { }
    }
}
