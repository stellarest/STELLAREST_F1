using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class SceneManagerEx
    {
        public BaseScene CurrentScene => GameObject.FindObjectOfType<BaseScene>();

        public void LoadScene(EScene sceneType)
            => SceneManager.LoadScene(GetSceneName(sceneType));

        private string GetSceneName(EScene sceneType)
            => System.Enum.GetName(typeof(EScene), sceneType);

        public void Clear() { }
    }
}
