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
        #region Heroes
        public Dictionary<int, Data.HeroData> HeroDataDict { get; private set; } = new Dictionary<int, Data.HeroData>();
        public Dictionary<int, Data.HeroSpriteData> HeroSpriteDataDict { get; private set; } = new Dictionary<int, Data.HeroSpriteData>();
        #endregion
       
        #region Monsters
        public Dictionary<int, Data.MonsterData> MonsterDataDict { get; private set; } = new Dictionary<int, Data.MonsterData>();
        public Dictionary<int, Data.BirdSpriteData> BirdSpriteDataDict { get; private set; } = new Dictionary<int, Data.BirdSpriteData>();
        public Dictionary<int, Data.QuadrupedsSpriteData> QuadrupedsSpriteDataDict { get; private set; } = new Dictionary<int, Data.QuadrupedsSpriteData>();
        #endregion

        #region Envs
        public Dictionary<int, Data.EnvData> EnvDataDict { get; private set; } = new Dictionary<int, Data.EnvData>();
        public Dictionary<int, Data.TreeSpriteData> TreeSpriteDataDict { get; private set; } = new Dictionary<int, Data.TreeSpriteData>();
        public Dictionary<int, Data.RockSpriteData> RockSpriteDataDict { get; private set; } = new Dictionary<int, Data.RockSpriteData>();
        #endregion


        public void Init()
        {
            HeroDataDict = LoadJson<Data.HeroDataLoader, int, Data.HeroData>(ReadOnly.String.HeroData).MakeDict();
            HeroSpriteDataDict = LoadJson<Data.HeroSpriteDataLoader, int, Data.HeroSpriteData>(ReadOnly.String.HeroSpriteData).MakeDict();

            MonsterDataDict = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>(ReadOnly.String.MonsterData).MakeDict();
            BirdSpriteDataDict = LoadJson<Data.BirdSpriteDataLoader, int, Data.BirdSpriteData>(ReadOnly.String.BirdSpriteData).MakeDict();
            QuadrupedsSpriteDataDict = LoadJson<Data.QuadrupedsSpriteDataLoader, int, Data.QuadrupedsSpriteData>(ReadOnly.String.QuadrupedsSpriteData).MakeDict();

            EnvDataDict = LoadJson<Data.EnvDataLoader, int, Data.EnvData>(ReadOnly.String.EnvData).MakeDict();
            TreeSpriteDataDict = LoadJson<Data.TreeSpriteDataLoader, int, Data.TreeSpriteData>(ReadOnly.String.TreeSpriteData).MakeDict();
            RockSpriteDataDict = LoadJson<Data.RockSpriteDataLoader, int, Data.RockSpriteData>(ReadOnly.String.RockSpriteData).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
    }
}
