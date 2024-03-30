using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1.Data
{
    public class BaseData
    {
        public int DataID;
        public string PrefabLabel;
        public string AnimatorLabel;
        public string DescriptionTextID;
        public string Rarity;
        public float ColliderRadius;
        public float MaxHp;
        public float Atk;
        public float AtkRange;
        public float MovementSpeed;
    }

    [Serializable]
    public class HeroData : BaseData
    {
        public string Type;
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
        public string Type;
        public string SkinColor;
        public HeroSpriteData_Head Head;
        public HeroSpriteData_UpperBody UpperBody;
        public HeroSpriteData_LowerBody LowerBody;
        public HeroSpriteData_Weapon Weapon;
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

        public string[] Mouth;
        public string[] MouthColors;

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
        public string LeftWeapon;
        public string LeftColor;
        public List<string> LeftWeaponAttachments;

        public string RightWeapon;
        public string RightColor;
        public List<string> RightWeaponAttachments;
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
    public class MonsterData : BaseData
    {
        public string Type;
        public string Size;
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
    public class MonsterSpriteData
    {
        public int DataID;
        public string Type;
        public string SkinColor;
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
}