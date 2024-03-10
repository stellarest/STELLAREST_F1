using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            
            return true;
        }

        public override void Clear() { }
    }
}
