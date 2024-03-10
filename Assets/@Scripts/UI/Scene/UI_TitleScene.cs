using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.EventSystems;
using static STELLAREST_F1.Define;
using Object = UnityEngine.Object;

namespace STELLAREST_F1
{
    public class UI_TitleScene : UI_Scene
    {
        public enum GameObjects
        {
            StartImage
        }

        public enum Texts
        {
            DisplayText
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BindObjects(typeof(GameObjects));
            BindTexts(typeof(Texts));
            // GetObject((int)GameObjects.StartImage).BindEvent((evt) => 
            // {
            //     Debug.Log("GO TO GAME SCENE !!");
            //     Managers.Scene.LoadScene(Define.EScene.GameScene);
            // }, Define.EUIEvent.PointerClick);

            GetObject((int)GameObjects.StartImage).BindEvent(action: delegate(PointerEventData evtData)
            {
                Debug.Log("GO TO GAME SCENE !!");
                Managers.Scene.LoadScene(Define.EScene.GameScene);
            }, evtType: EUIEvent.PointerClick);

            GetObject((int)GameObjects.StartImage).SetActive(false);
            GetText((int)Texts.DisplayText).text = $"...";
            StartLoadAsset();

            return true;
        }

        private void StartLoadAsset()
        {
            Managers.Resource.LoadAllAsync<Object>(Const.String.PreLoad, delegate(string key, int count, int totalCount)
            {
                Debug.Log($"Key Loaded : {key}, Current : {count} / Total : {totalCount}");
                if (count == totalCount)
                {
                    // Managers.Data.Init
                    GetObject((int)GameObjects.StartImage).SetActive(true);
                    GetText((int)Texts.DisplayText).text = $"Touch To Start";
                }
            });
        }
    }
}
