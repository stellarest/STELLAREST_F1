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

        #region Stats
        public Dictionary<int, Data.StatData> StatDataDict { get; private set; } = new Dictionary<int, Data.StatData>();
        #endregion
        #region Skills
        public Dictionary<int, Data.SkillData> SkillDataDict { get; private set; } = new Dictionary<int, Data.SkillData>();
        #endregion
        #region Projectiles
        public Dictionary<int, Data.ProjectileData> ProjectileDataDict { get; private set; } = new Dictionary<int, Data.ProjectileData>();
        #endregion
        #region Effects
        public Dictionary<int, Data.EffectData> EffectDataDict { get; private set;} = new Dictionary<int, Data.EffectData>();
        #endregion

        public void Init()
        {
            HeroDataDict = LoadJson<Data.HeroDataLoader, int, Data.HeroData>(ReadOnly.DataSet.HeroData).MakeDict();
            HeroSpriteDataDict = LoadJson<Data.HeroSpriteDataLoader, int, Data.HeroSpriteData>(ReadOnly.DataSet.HeroSpriteData).MakeDict();

            MonsterDataDict = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>(ReadOnly.DataSet.MonsterData).MakeDict();
            BirdSpriteDataDict = LoadJson<Data.BirdSpriteDataLoader, int, Data.BirdSpriteData>(ReadOnly.DataSet.BirdSpriteData).MakeDict();
            QuadrupedsSpriteDataDict = LoadJson<Data.QuadrupedsSpriteDataLoader, int, Data.QuadrupedsSpriteData>(ReadOnly.DataSet.QuadrupedsSpriteData).MakeDict();

            EnvDataDict = LoadJson<Data.EnvDataLoader, int, Data.EnvData>(ReadOnly.DataSet.EnvData).MakeDict();
            TreeSpriteDataDict = LoadJson<Data.TreeSpriteDataLoader, int, Data.TreeSpriteData>(ReadOnly.DataSet.TreeSpriteData).MakeDict();
            RockSpriteDataDict = LoadJson<Data.RockSpriteDataLoader, int, Data.RockSpriteData>(ReadOnly.DataSet.RockSpriteData).MakeDict();

            StatDataDict = LoadJson<Data.StatDataLoader, int, Data.StatData>(ReadOnly.DataSet.StatData).MakeDict();
            SkillDataDict = LoadJson<Data.SkillDataLoader, int, Data.SkillData>(ReadOnly.DataSet.SkillData).MakeDict();
            ProjectileDataDict = LoadJson<Data.ProjectileDataLoader, int, Data.ProjectileData>(ReadOnly.DataSet.ProjectileData).MakeDict();
            
            EffectDataDict = LoadJson<Data.EffectDataLoader, int, Data.EffectData>(ReadOnly.DataSet.EffectData).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
    }
}
