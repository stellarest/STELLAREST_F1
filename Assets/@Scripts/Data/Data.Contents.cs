using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1.Data
{
    public class BaseData
    {
        public int DataID;
        public string PrefabLabel;
        public string IconLabel;
        public string NameTextID;
        public string DescriptionTextID;
        public string Dev_NameTextID;
        public string Dev_DescriptionTextID;
    }

    public class CreatureData : BaseData
    {
        public string AIClassName;
        public string AnimatorLabel;
        public float ColliderRadius;
        public Vector2 ColliderOffset;
        public float MaxHealth;
        public float MinDamage;
        public float MaxDamage;
        public float AttackRate;
        public float MovementSpeed;
        public int Skill_A_ID;
        public int Skill_B_TemplateID;
        public int Skill_C_TemplateID;
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

    [Serializable]
    public class MonsterSpriteData : BaseData
    {
    }

    [Serializable]
    public class MonsterBirdSpriteData : MonsterSpriteData
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

    public class MonsterBirdSpriteDataLoader : ILoader<int, MonsterBirdSpriteData>
    {
        public List<MonsterBirdSpriteData> MonsterBirdsSprites = new List<MonsterBirdSpriteData>();

        public Dictionary<int, MonsterBirdSpriteData> MakeDict()
        {
            Dictionary<int, MonsterBirdSpriteData> dict = new Dictionary<int, MonsterBirdSpriteData>();
            foreach (MonsterBirdSpriteData data in MonsterBirdsSprites)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    [Serializable]
    public class MonsterQuadrupedsSpriteData : MonsterSpriteData
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

    public class MonsterQuadrupedsSpriteDataLoader : ILoader<int, MonsterQuadrupedsSpriteData>
    {
        public List<MonsterQuadrupedsSpriteData> MonsterQuadrupedsSprites = new List<MonsterQuadrupedsSpriteData>();

        public Dictionary<int, MonsterQuadrupedsSpriteData> MakeDict()
        {
            Dictionary<int, MonsterQuadrupedsSpriteData> dict = new Dictionary<int, MonsterQuadrupedsSpriteData>();
            foreach (MonsterQuadrupedsSpriteData data in MonsterQuadrupedsSprites)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    [Serializable]
    public class EnvData : BaseData
    {
        public EEnvType EnvType;
        public string AnimatorLabel;
        public float MaxHealth;
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

    [Serializable]
    public class EnvTreeSpriteData
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

    public class EnvTreeSpriteDataLoader : ILoader<int, EnvTreeSpriteData>
    {
        public List<EnvTreeSpriteData> EnvTreesSprites = new List<EnvTreeSpriteData>();

        public Dictionary<int, EnvTreeSpriteData> MakeDict()
        {
            Dictionary<int, EnvTreeSpriteData> dict = new Dictionary<int, EnvTreeSpriteData>();
            foreach (EnvTreeSpriteData data in EnvTreesSprites)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    [Serializable]
    public class EnvRockSpriteData
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

    public class RockSpriteDataLoader : ILoader<int, EnvRockSpriteData>
    {
        public List<EnvRockSpriteData> EnvRocksSprites = new List<EnvRockSpriteData>();

        public Dictionary<int, EnvRockSpriteData> MakeDict()
        {
            Dictionary<int, EnvRockSpriteData> dict = new Dictionary<int, EnvRockSpriteData>();
            foreach (EnvRockSpriteData data in EnvRocksSprites)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

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
        public int[] OnCreateEffectIDs;            // UnlockSkill로 AddComponent로 새롭게 생성 되었을 때, 최초 한 번
        public int[] OnSkillEnterEffectIDs;        // OnSkillEnterState
        public int[] OnSkillCallbackEffectIDs;     // OnSkillCallback 
        public int[] OnSkillExitEffectIDs;         // OnSkillExitState
        public int[] OnHitEffectIDs;               // OnDamaged, etc..
    }

    [Serializable]
    public class HeroSkillData : SkillData
    {
    }

    public class HeroSkillDataLoader : ILoader<int, HeroSkillData>
    {
        public List<HeroSkillData> HeroesSkills = new List<HeroSkillData>();

        public Dictionary<int, HeroSkillData> MakeDict()
        {
            Dictionary<int, HeroSkillData> dict = new Dictionary<int, HeroSkillData>();
            foreach (HeroSkillData data in HeroesSkills)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    [Serializable]
    public class MonsterSkillData : SkillData
    {
    }

    public class MonsterSkillDataLoader : ILoader<int, MonsterSkillData>
    {
        public List<MonsterSkillData> MonstersSkills = new List<MonsterSkillData>();

        public Dictionary<int, MonsterSkillData> MakeDict()
        {
            Dictionary<int, MonsterSkillData> dict = new Dictionary<int, MonsterSkillData>();
            foreach (MonsterSkillData data in MonstersSkills)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    [Serializable]
    public class ProjectileData : BaseData
    {
        public bool RotateToTarget;
        public bool IncludePrevDamagedTargets;
        public int PenetrationCount;
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

    [Serializable]
    public class EffectData : BaseData
    {
        public bool IsLoop;
        public float AddAmount;
        public float AddPercent;
        public float AddPercentMulti;
        public float Period;
        public float Duration;
        //public EEffectType EffectType;
        public string EffectType; // EEffectType이 자주 바뀌어서 일단 string으로 변경.
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

    [Serializable]
    public class HeroEffectData : EffectData
    {
    }

    public class HeroEffectDataLoader : ILoader<int, HeroEffectData>
    {
        public List<HeroEffectData> HeroEffects = new List<HeroEffectData>();

        public Dictionary<int, HeroEffectData> MakeDict()
        {
            Dictionary<int, HeroEffectData> dict = new Dictionary<int, HeroEffectData>();
            foreach (HeroEffectData data in HeroEffects)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    [Serializable]
    public class MonsterEffectData : EffectData
    {
    }

    public class MonsterEffectDataLoader : ILoader<int, MonsterEffectData>
    {
        public List<MonsterEffectData> MonsterEffects = new List<MonsterEffectData>();

        public Dictionary<int, MonsterEffectData> MakeDict()
        {
            Dictionary<int, MonsterEffectData> dict = new Dictionary<int, MonsterEffectData>();
            foreach (MonsterEffectData data in MonsterEffects)
                dict.Add(data.DataID, data);

            return dict;
        }
    }

    [Serializable]
    public class EnvEffectData : EffectData
    {
    }

    public class EnvEffectDataLoader : ILoader<int, EnvEffectData>
    {
        public List<EnvEffectData> EnvEffects = new List<EnvEffectData>();

        public Dictionary<int, EnvEffectData> MakeDict()
        {
            Dictionary<int, EnvEffectData> dict = new Dictionary<int, EnvEffectData>();
            foreach (EnvEffectData data in EnvEffects)
                dict.Add(data.DataID, data);

            return dict;
        }
    }
}