using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using STELLAREST_F1.Data;
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
        public Dictionary<int, HeroData> HeroDataDict { get; private set; } = new Dictionary<int, HeroData>();
        public Dictionary<int, HeroSpriteData> HeroSpriteDataDict { get; private set; } = new Dictionary<int, HeroSpriteData>();
        public Dictionary<int, HeroStatData> HeroStatDataDict { get; private set; } = new Dictionary<int, HeroStatData>();
        #endregion
       
        #region Monsters
        public Dictionary<int, MonsterData> MonsterDataDict { get; private set; } = new Dictionary<int, MonsterData>();
        public Dictionary<int, BirdSpriteData> BirdSpriteDataDict { get; private set; } = new Dictionary<int, BirdSpriteData>();
        public Dictionary<int, QuadrupedsSpriteData> QuadrupedsSpriteDataDict { get; private set; } = new Dictionary<int, QuadrupedsSpriteData>();
        public Dictionary<int, MonsterStatData> MonsterStatDataDict { get; private set; } = new Dictionary<int, MonsterStatData>();
        
        #endregion

        #region Envs
        public Dictionary<int, EnvData> EnvDataDict { get; private set; } = new Dictionary<int, EnvData>();
        public Dictionary<int, TreeSpriteData> TreeSpriteDataDict { get; private set; } = new Dictionary<int, TreeSpriteData>();
        public Dictionary<int, RockSpriteData> RockSpriteDataDict { get; private set; } = new Dictionary<int, RockSpriteData>();
        public Dictionary<int, EnvStatData> EnvStatDataDict { get; private set; } = new Dictionary<int, EnvStatData>();
        #endregion

        // --- TEMP
        // public Dictionary<int, StatData> StatDataDict { get; private set; } = new Dictionary<int, StatData>();
        

        #region Skills
        public Dictionary<int, SkillData> SkillDataDict { get; private set; } = new Dictionary<int, SkillData>();
        #endregion
        #region Projectiles
        public Dictionary<int, ProjectileData> ProjectileDataDict { get; private set; } = new Dictionary<int, ProjectileData>();
        #endregion
        #region Effects
        public Dictionary<int, EffectData> EffectDataDict { get; private set;} = new Dictionary<int, EffectData>();
        #endregion

        public void Init()
        {
            HeroDataDict = LoadJson<HeroDataLoader, int, HeroData>(ReadOnly.DataSet.HeroData).MakeDict();
            HeroSpriteDataDict = LoadJson<HeroSpriteDataLoader, int, HeroSpriteData>(ReadOnly.DataSet.HeroSpriteData).MakeDict();
            HeroStatDataDict = LoadJson<HeroStatDataLoader, int, HeroStatData>(ReadOnly.DataSet.HeroStatData).MakeDict();

            MonsterDataDict = LoadJson<MonsterDataLoader, int, MonsterData>(ReadOnly.DataSet.MonsterData).MakeDict();
            BirdSpriteDataDict = LoadJson<BirdSpriteDataLoader, int, BirdSpriteData>(ReadOnly.DataSet.BirdSpriteData).MakeDict();
            QuadrupedsSpriteDataDict = LoadJson<QuadrupedsSpriteDataLoader, int, QuadrupedsSpriteData>(ReadOnly.DataSet.QuadrupedsSpriteData).MakeDict();
            MonsterStatDataDict = LoadJson<MonsterStatDataLoader, int, MonsterStatData>(ReadOnly.DataSet.MonsterStatData).MakeDict();

            EnvDataDict = LoadJson<EnvDataLoader, int, EnvData>(ReadOnly.DataSet.EnvData).MakeDict();
            TreeSpriteDataDict = LoadJson<TreeSpriteDataLoader, int, TreeSpriteData>(ReadOnly.DataSet.TreeSpriteData).MakeDict();
            RockSpriteDataDict = LoadJson<RockSpriteDataLoader, int, RockSpriteData>(ReadOnly.DataSet.RockSpriteData).MakeDict();
            EnvStatDataDict = LoadJson<EnvStatDataLoader, int, EnvStatData>(ReadOnly.DataSet.EnvStatData).MakeDict();

            // --- TEMP
            // StatDataDict = LoadJson<StatDataLoader, int, StatData>(ReadOnly.DataSet.StatData).MakeDict();

            SkillDataDict = LoadJson<SkillDataLoader, int, SkillData>(ReadOnly.DataSet.SkillData).MakeDict();
            ProjectileDataDict = LoadJson<ProjectileDataLoader, int, ProjectileData>(ReadOnly.DataSet.ProjectileData).MakeDict();
            
            EffectDataDict = LoadJson<EffectDataLoader, int, EffectData>(ReadOnly.DataSet.EffectData).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
    }
}
