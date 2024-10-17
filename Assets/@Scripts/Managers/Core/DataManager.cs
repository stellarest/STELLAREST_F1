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
        public Dictionary<int, HeroSkillData> HeroSkillDataDict { get; private set; } = new Dictionary<int, HeroSkillData>();
        public Dictionary<int, HeroEffectData> HeroEffectDataDict { get; private set; } = new Dictionary<int, HeroEffectData>();
        #endregion
       
        #region Monsters
        public Dictionary<int, MonsterData> MonsterDataDict { get; private set; } = new Dictionary<int, MonsterData>();
        public Dictionary<int, MonsterBirdSpriteData> MonsterBirdSpriteDataDict { get; private set; } = new Dictionary<int, MonsterBirdSpriteData>();
        public Dictionary<int, MonsterQuadrupedsSpriteData> MonsterQuadrupedSpriteDataDict { get; private set; } = new Dictionary<int, MonsterQuadrupedsSpriteData>();
        public Dictionary<int, MonsterSkillData> MonsterSkillDataDict { get; private set; } = new Dictionary<int, MonsterSkillData>();
        public Dictionary<int, MonsterEffectData> MonsterEffectDataDict { get; private set; } = new Dictionary<int, MonsterEffectData>();
        #endregion

        #region Envs
        public Dictionary<int, EnvData> EnvDataDict { get; private set; } = new Dictionary<int, EnvData>();
        public Dictionary<int, EnvTreeSpriteData> EnvTreeSpriteDataDict { get; private set; } = new Dictionary<int, EnvTreeSpriteData>();
        public Dictionary<int, EnvRockSpriteData> RockSpriteDataDict { get; private set; } = new Dictionary<int, EnvRockSpriteData>();
        public Dictionary<int, EnvEffectData> EnvEffectDataDict { get; private set; } = new Dictionary<int, EnvEffectData>();
        #endregion

        #region Global
        public Dictionary<int, EffectData> EffectDataDict { get; private set; } = new Dictionary<int, EffectData>();
        public Dictionary<int, ProjectileData> ProjectileDataDict { get; private set; } = new Dictionary<int, ProjectileData>();
        #endregion

        #region Items
        #endregion

        public void Init()
        {
            // --- Heroes Data
            HeroDataDict = LoadJson<HeroDataLoader, int, HeroData>(ReadOnly.DataSet.HeroData).MakeDict();
            HeroSpriteDataDict = LoadJson<HeroSpriteDataLoader, int, HeroSpriteData>(ReadOnly.DataSet.HeroSpriteData).MakeDict();
            HeroSkillDataDict = LoadJson<HeroSkillDataLoader, int, HeroSkillData>(ReadOnly.DataSet.HeroSkillData).MakeDict();
            HeroEffectDataDict = LoadJson<HeroEffectDataLoader, int, HeroEffectData>(ReadOnly.DataSet.HeroEffectData).MakeDict();

            // --- Monsters Data
            MonsterDataDict = LoadJson<MonsterDataLoader, int, MonsterData>(ReadOnly.DataSet.MonsterData).MakeDict();
            MonsterBirdSpriteDataDict = LoadJson<MonsterBirdSpriteDataLoader, int, MonsterBirdSpriteData>(ReadOnly.DataSet.MonsterBirdSpriteData).MakeDict();
            MonsterQuadrupedSpriteDataDict = LoadJson<MonsterQuadrupedsSpriteDataLoader, int, MonsterQuadrupedsSpriteData>(ReadOnly.DataSet.MonsterQuadrupedSpriteData).MakeDict();
            MonsterSkillDataDict = LoadJson<MonsterSkillDataLoader, int, MonsterSkillData>(ReadOnly.DataSet.MonsterSkillData).MakeDict();
            MonsterEffectDataDict = LoadJson<MonsterEffectDataLoader, int, MonsterEffectData>(ReadOnly.DataSet.MonsterEffectData).MakeDict();

            // --- Envs Data
            EnvDataDict = LoadJson<EnvDataLoader, int, EnvData>(ReadOnly.DataSet.EnvData).MakeDict();
            EnvTreeSpriteDataDict = LoadJson<EnvTreeSpriteDataLoader, int, EnvTreeSpriteData>(ReadOnly.DataSet.EnvTreeSpriteData).MakeDict();
            RockSpriteDataDict = LoadJson<RockSpriteDataLoader, int, EnvRockSpriteData>(ReadOnly.DataSet.EnvRockSpriteData).MakeDict();
            EnvEffectDataDict = LoadJson<EnvEffectDataLoader, int, EnvEffectData>(ReadOnly.DataSet.EnvEffectData).MakeDict();

            // --- Global Data
            EffectDataDict = LoadJson<EffectDataLoader, int, EffectData>(ReadOnly.DataSet.EffectData).MakeDict();
            ProjectileDataDict = LoadJson<ProjectileDataLoader, int, ProjectileData>(ReadOnly.DataSet.ProjectileData).MakeDict();
            // EffectDataDict = LoadJson<EffectDataLoader, int, EffectData>(ReadOnly.DataSet.EffectData).MakeDict();
            
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
    }
}
