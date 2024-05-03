using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Managers : MonoBehaviour
    {
        private static Managers s_instance = null;
        public static Managers Instance
        {
            get
            {
                if (s_isApplicationQuitting == false && s_instance == null)
                {
                    GameObject go = GameObject.Find(ReadOnly.String.Managers);
                    if (go == null)
                    {
                        go = new GameObject { name = ReadOnly.String.Managers };
                        DontDestroyOnLoad(go);
                    }

                    s_instance = go.AddComponent<Managers>();
                }

                return s_instance;
            }
        }

        #region Contents
        private AnimationManager _animation = new AnimationManager();
        public static AnimationManager Animation => Instance?._animation;

        private GameManager _game = new GameManager();
        public static GameManager Game => Instance?._game;

        private MapManager _map = new MapManager();
        public static MapManager Map => Instance?._map;

        private ObjectManager _object = new ObjectManager();
        public static ObjectManager Object => Instance?._object;

        private SpriteManager _sprite = new SpriteManager();
        public static SpriteManager Sprite => Instance?._sprite;
        #endregion

        #region Core
        private DataManager _data = new DataManager();
        public static DataManager Data => Instance?._data;

        private PoolManager _pool = new PoolManager();
        public static PoolManager Pool => Instance?._pool;

        private ResourceManager _resource = new ResourceManager();
        public static ResourceManager Resource => Instance?._resource;

        private SceneManagerEx _scene = new SceneManagerEx();
        public static SceneManagerEx Scene => Instance?._scene;

        private SoundManager _sound = new SoundManager();
        public static SoundManager Sound => Instance?._sound;

        private UIManager _ui = new UIManager();
        public static UIManager UI => Instance?._ui;
        #endregion

        private static bool s_isApplicationQuitting = false;
        private void OnApplicationQuit()
            => s_isApplicationQuitting = true;
    }
}
