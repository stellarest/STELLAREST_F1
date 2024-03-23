using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace STELLAREST_F1
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        public Dictionary<int, Data.TestData> TestDict { get; private set; } = new Dictionary<int, Data.TestData>();
        public Dictionary<int, Data.HeroData> HeroDataDict { get; private set; } = new Dictionary<int, Data.HeroData>();
        public Dictionary<int, Data.HeroSpriteData> HeroSpriteDataDict { get; private set; } = new Dictionary<int, Data.HeroSpriteData>();
        public Dictionary<int, Data.HeroAnimationData> HeroAnimationDataDict { get; private set; } = new Dictionary<int, Data.HeroAnimationData>();

        public void Init()
        {
            //TestDict = LoadJson<Data.TestDataLoader, int, Data.TestData>("TestData").MakeDict();
            HeroDataDict = LoadJson<Data.HeroDataLoader, int, Data.HeroData>("HeroData").MakeDict();
            HeroSpriteDataDict = LoadJson<Data.HeroSpriteDataLoader, int, Data.HeroSpriteData>("HeroSpriteData").MakeDict();
            HeroAnimationDataDict = LoadJson<Data.HeroAnimationDataLoader, int, Data.HeroAnimationData>("HeroAnimationData").MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
    }
}
