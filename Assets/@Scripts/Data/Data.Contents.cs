using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1.Data
{
    public class BaseData
    {
        public int DataID;
        public string PrefabLabel;
        public string NameTextID;
        public string DescriptionTextID;
        public string IconImage;
    }

    // 101000 ~ ...
    public class CreatureData : BaseData
    {
        public ECreatureRarity CreatureRarity;
        public string AIClassName;
        public string AnimatorLabel;
        public float ColliderRadius;
        public int Skill_Attack_ID;
        public int Skill_A_ID;
        public int Skill_B_ID;
    }

    [Serializable]
    public class HeroData : CreatureData
    {
    }
    public class HeroDataLoader : ILoader<int, HeroData>
    {
        public List<HeroData> Heroes = new List<HeroData>();
        public Dictionary<int, HeroData> MakeDict()
        {
            Dictionary<int, HeroData> dict = new Dictionary<int, HeroData>();
            foreach (HeroData data in Heroes)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    #region Hero Sprite Data
    [Serializable]
    public class HeroSpriteData
    {
        public int DataID;
        public string Tag;
        public HeroSpriteData_Skin Skin;
        public HeroSpriteData_Head Head;
        public HeroSpriteData_UpperBody UpperBody;
        public HeroSpriteData_LowerBody LowerBody;
        public HeroSpriteData_Weapon Weapon;
    }

    [Serializable]
    public class HeroSpriteData_Skin
    {
        public string SkinColor;
        public string Head;
        public string Ears;
        public string Torso;
        public string ArmL;
        public string ForearmL;
        public string HandL;
        public string ArmR;
        public string ForearmR;
        public string HandR;
        public string Finger;
        public string Pelvis;
        public string Shin;
        public string Leg;
    }

    [SerializeField]
    public class HeroSpriteData_Head
    {
        public string Hair;
        public string HairColor;

        public string[] Eyebrows;
        public string[] EyebrowsColors;

        public string[] Eyes;
        public string[] EyesColors;

        public string[] Mouth; // --> Mouths
        public string[] MouthColors; // --> MouthsColors
 
        public string Beard;
        public string BeardColor;

        public string Earrings;
        public string EarringsColor;

        public string Mask;
        public string MaskColor;

        public string Glasses;
        public string GlassesColor;

        public string Helmet;
        public string HelmetColor;
    }

    [Serializable]
    public class HeroSpriteData_UpperBody
    {
        public string Torso;
        public string TorsoColor;

        public string Cape;
        public string CapeColor;

        public string ArmL;
        public string ArmLColor;

        public string ForearmL;
        public string ForearmLColor;

        public string HandL;
        public string HandLColor;

        public string Finger;
        public string FingerColor;

        public string ArmR;
        public string ArmRColor;

        public string ForearmR;
        public string ForearmRColor;

        public string SleeveR;
        public string SleeveRColor;

        public string HandR;
        public string HandRColor;
    }

    [Serializable]
    public class HeroSpriteData_LowerBody
    {
        public string Pelvis;
        public string PelvisColor;

        public string LegL;
        public string LegLColor;

        public string ShinL;
        public string ShinLColor;

        public string LegR;
        public string LegRColor;

        public string ShinR;
        public string ShinRColor;
    }

    [Serializable]
    public class HeroSpriteData_Weapon
    {
        public string LWeapon;
        public Vector3 LWeaponLocalScale;
        public int LWeaponSorting;
        public bool LWeaponFlipX;
        public bool LWeaponFlipY;
        public Vector3 LWeaponFireSocketLocalPosition;

        public string[] LWeaponChilds;
        public Vector3[] LWeaponChildsLocalPositions;
        public Vector3[] LWeaponChildsLocalScales;
        public int[] LWeaponChildSortings;
        public bool[] LWeaponChildFlipXs;
        public bool[] LWeaponChildFlipYs;

        public string RWeapon;
        public Vector3 RWeaponLocalScale;
        public int RWeaponSorting;
        public bool RWeaponFlipX;
        public bool RWeaponFlipY;
        public Vector3 RWeaponFireSocketLocalPosition;

        public string[] RWeaponChilds;
        public Vector3[] RWeaponChildsLocalPositions;
        public Vector3[] RWeaponChildsLocalScales;
        public int[] RWeaponChildSortings;
        public bool[] RWeaponChildFlipXs;
        public bool[] RWeaponChildFlipYs;
    }

    public class HeroSpriteDataLoader : ILoader<int, HeroSpriteData>
    {
        public List<HeroSpriteData> HeroesSprites = new List<HeroSpriteData>();
        public Dictionary<int, HeroSpriteData> MakeDict()
        {
            Dictionary<int, HeroSpriteData> dict = new Dictionary<int, HeroSpriteData>();
            foreach (HeroSpriteData data in HeroesSprites)
                dict.Add(data.DataID, data);

            return dict;
        }
    }
    #endregion

    [Serializable]
    public class MonsterData : CreatureData
    {
        public EMonsterType MonsterType;
        public EObjectSize MonsterSize;
        public int DropItemID;
    }

    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> Monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData data in Monsters)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    #region Monster Sprite Data
    [Serializable]
    public class MonsterSpriteData : BaseData
    {
    }

    [Serializable]
    public class BirdSpriteData : MonsterSpriteData
    {
        public string Body;
        public Vector3 BodyPosition;
        public int BodySortingOrder;

        public string[] Heads;
        public Vector3 HeadPosition;
        public int HeadSortingOrder;

        public string Wing;
        public Vector3 WingPosition;
        public int WingSortingOrder;

        public string LegL;
        public Vector3 LegLPosition;
        public int LegLSortingOrder;

        public string LegR;
        public Vector3 LegRPosition;
        public int LegRSortingOrder;

        public string Tail;
        public Vector3 TailPosition;
        public int TailSortingOrder;
    }

    public class BirdSpriteDataLoader : ILoader<int, BirdSpriteData>
    {
        public List<BirdSpriteData> BirdsSprites = new List<BirdSpriteData>();

        public Dictionary<int, BirdSpriteData> MakeDict()
        {
            Dictionary<int, BirdSpriteData> dict = new Dictionary<int, BirdSpriteData>();
            foreach (BirdSpriteData data in BirdsSprites)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    [Serializable]
    public class QuadrupedsSpriteData : MonsterSpriteData
    {
        public string Body;
        public Vector3 BodyPosition;
        public int BodySortingOrder;

        public string[] Heads;
        public Vector3 HeadPosition;
        public int HeadSortingOrder;

        public string LegFrontL;
        public Vector3 LegFrontLPosition;
        public int LegFrontLSortingOrder;

        public string LegFrontR;
        public Vector3 LegFrontRPosition;
        public int LegFrontRSortingOrder;

        public string LegBackL;
        public Vector3 LegBackLPosition;
        public int LegBackLSortingOrder;

        public string LegBackR;
        public Vector3 LegBackRPosition;
        public int LegBackRSortingOrder;

        public string Tail;
        public Vector3 TailPosition;
        public int TailSortingOrder;
    }

    public class QuadrupedsSpriteDataLoader : ILoader<int, QuadrupedsSpriteData>
    {
        public List<QuadrupedsSpriteData> QuadrupedsSprites = new List<QuadrupedsSpriteData>();

        public Dictionary<int, QuadrupedsSpriteData> MakeDict()
        {
            Dictionary<int, QuadrupedsSpriteData> dict = new Dictionary<int, QuadrupedsSpriteData>();
            foreach (QuadrupedsSpriteData data in QuadrupedsSprites)
                dict.Add(data.DataID, data);

            return dict;
        }
    }
    #endregion

    [Serializable]
    public class EnvData : BaseData
    {
        public EEnvType EnvType;
        public string AnimatorLabel;
        public int DropItemID;
    }

    public class EnvDataLoader : ILoader<int, EnvData>
    {
        public List<EnvData> Envs = new List<EnvData>();

        public Dictionary<int, EnvData> MakeDict()
        {
            Dictionary<int, EnvData> dict = new Dictionary<int, EnvData>();
            foreach (EnvData data in Envs)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    #region Env Sprite Data
    [Serializable]
    public class TreeSpriteData
    {
        public int DataID;
        public string Tag;

        public string Trunk;
        public Vector3 TrunkPosition;

        public string Patch;
        public Vector3 PatchPosition;

        public string Stump;
        public Vector3 StumpPosition;

        public string Fruits;
        public Vector3[] FruitsScales;
        public Vector3[] FruitsPositions;
        public Vector3[] FruitsRotations;

        public string EndParticleMaterial;
    }

    public class TreeSpriteDataLoader : ILoader<int, TreeSpriteData>
    {
        public List<TreeSpriteData> TreesSprites = new List<TreeSpriteData>();

        public Dictionary<int, TreeSpriteData> MakeDict()
        {
            Dictionary<int, TreeSpriteData> dict = new Dictionary<int, TreeSpriteData>();
            foreach (TreeSpriteData data in TreesSprites)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    [Serializable]
    public class RockSpriteData
    {
        public int DataID;
        public string Tag;

        public string Ore;
        public string OreShadow;
        public string OreLightColor;
        public string OreParticleColor;
        public int OreMaxParticleCount;

        public string[] Spots;
        public bool[] SpotsFlipXs;

        public string[] Fragments;
        public bool[] FragmentsFlipXs;
    }

    public class RockSpriteDataLoader : ILoader<int, RockSpriteData>
    {
        public List<RockSpriteData> RocksSprites = new List<RockSpriteData>();

        public Dictionary<int, RockSpriteData> MakeDict()
        {
            Dictionary<int, RockSpriteData> dict = new Dictionary<int, RockSpriteData>();
            foreach (RockSpriteData data in RocksSprites)
                dict.Add(data.DataID, data);

            return dict;
        }
    }
    #endregion


    [Serializable]
    public class StatData : BaseData
    {
        public float MaxHp;
        public float MinAtk;
        public float MaxAtk;
        public float CriticalRate;
        public float DodgeRate;
        public float MovementSpeed;
    }

    public class StatDataLoader : ILoader<int, StatData>
    {
        public List<StatData> Stats = new List<StatData>();

        public Dictionary<int, StatData> MakeDict()
        {
            Dictionary<int, StatData> dict = new Dictionary<int, StatData>();
            foreach (StatData data in Stats)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    // 201000 ~ ...
    [Serializable]
    public class SkillData : BaseData
    {
        public ESkillType SkillType;
		public string ClassName;
        public string AttachmentPoint;
        public int InvokeRange; // --- 시전 조건
        public ESkillTargetRange TargetRange; // --- 시전 범위
        public int TargetDistance;
        public int ProjectileID;
		public float CoolTime;
        public int[] EffectIDs;
    }

    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> Skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData data in Skills)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    // 201000 ~ ...
    [Serializable]
    public class ProjectileData : BaseData
    {
        public bool RotateToTarget;
        public float ProjectileSpeed;
        public float ProjectileLifeTime;
        public EObjectSize ProjectileSize;
        public EAnimationCurveType ProjectileCurveType;
        public EProjectileMotionType ProjectileMotionType;
    }

    public class ProjectileDataLoader : ILoader<int, ProjectileData>
    {
        public List<ProjectileData> Projectiles = new List<ProjectileData>();

        public Dictionary<int, ProjectileData> MakeDict()
        {
            Dictionary<int, ProjectileData> dict = new Dictionary<int, ProjectileData>();
            foreach (ProjectileData data in Projectiles)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    // 301000 ~ ...
    [Serializable]
    public class EffectData : BaseData
    {
        public bool IsLoop;
        public float Amount;
        public float PercentAdd;
        public float PercentMulti;
        public float Duration;
        public float Period;
        public EEffectType EffectType;
        public EObjectSize EffectSize;
        public EEffectSpawnType EffectSpawnType;
    }

    public class EffectDataLoader : ILoader<int, EffectData>
    {
        public List<EffectData> Effects = new List<EffectData>();

        public Dictionary<int, EffectData> MakeDict()
        {
            Dictionary<int, EffectData> dict = new Dictionary<int,EffectData>();
            foreach (EffectData data in Effects)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    // [Serializable]
    // public class ProjectileInfoData 
    // {
    //     public int DataID;
    //     public string Tag;
    //     public string BodyColor;
    //     public string Body;
    // }

    // public class ProjectileInfoDataLoader : ILoader<int, ProjectileInfoData>
    // {
    //     public List<ProjectileInfoData> ProjectileInfos = new List<ProjectileInfoData>();

    //     public Dictionary<int, ProjectileInfoData> MakeDict()
    //     {
    //         Dictionary<int, ProjectileInfoData> dict = new Dictionary<int, ProjectileInfoData>();
    //         foreach (ProjectileInfoData data in ProjectileInfos)
    //             dict.Add(data.DataID, data);

    //         return dict;
    //     }
    // }
}