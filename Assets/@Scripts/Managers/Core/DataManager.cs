using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        public Dictionary<int, Data.HeroData> HeroDataDict { get; private set; } = new Dictionary<int, Data.HeroData>();
        public Dictionary<int, Data.HeroSpriteData> HeroSpriteDataDict { get; private set; } = new Dictionary<int, Data.HeroSpriteData>();
        public Dictionary<int, Data.MonsterData> MonsterDataDict { get; private set; } = new Dictionary<int, Data.MonsterData>();
        public Dictionary<int, Data.BirdSpriteData> BirdSpriteDataDict { get; private set; } = new Dictionary<int, Data.BirdSpriteData>();

        public void Init()
        {
            HeroDataDict = LoadJson<Data.HeroDataLoader, int, Data.HeroData>(ReadOnly.String.HeroData).MakeDict();
            HeroSpriteDataDict = LoadJson<Data.HeroSpriteDataLoader, int, Data.HeroSpriteData>(ReadOnly.String.HeroSpriteData).MakeDict();
            MonsterDataDict = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>(ReadOnly.String.MonsterData).MakeDict();
            BirdSpriteDataDict = LoadJson<Data.BirdSpriteDataLoader, int, Data.BirdSpriteData>(ReadOnly.String.BirdSpriteData).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
    }
}
