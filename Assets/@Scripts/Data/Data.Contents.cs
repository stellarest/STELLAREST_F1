using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1.Data
{
    #region Test Data
    [Serializable]
    public class TestData
    {
        public int Level;
        public int Exp;
        public List<int> Skills;
        public float Speed;
        public string Name;
    }

    [Serializable]
    public class TestDataLoader : ILoader<int, TestData>
    {
        public List<TestData> tests = new List<TestData>();
        public Dictionary<int, TestData> MakeDict()
        {
            Dictionary<int, TestData> dict = new Dictionary<int, TestData>();
            foreach (TestData data in tests)
                dict.Add(data.Level, data);

            return dict;
        }
    }
    #endregion

    [Serializable]
    public class HeroData
    {
        public int DataID;
        public string DescriptionTextID;
        public string AnimatorTextID;
        public string Rarity;
        public float MaxHp;
        public float Atk;
        public float AtkRange;
        public float MovementSpeed;
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
        public string BodyType;
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

        public string Eyebrows;
        public string EyebrowsColor;

        public string Eyes;
        public string EyesColor;

        public string Mouth;
        public string MouthColor;

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
        public Vector3 LeftScale;
        public Vector3 LeftPosition;
        public Vector3 LeftRotation;
        public List<string> LeftWeaponAttachments;

        public string RightWeapon;
        public string RightColor;
        public Vector3 RightScale;
        public Vector3 RightPosition;
        public Vector3 RightRotation;
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
    public class HeroAnimationData
    {
        public int DataID;
        public string Tag;
        public string AnimatorTextID;
    }

    public class HeroAnimationDataLoader : ILoader<int, HeroAnimationData>
    {
        public List<HeroAnimationData> HeroAnimations = new List<HeroAnimationData>();

        public Dictionary<int, HeroAnimationData> MakeDict()
        {
            Dictionary<int, HeroAnimationData> dict = new Dictionary<int, HeroAnimationData>();
            foreach (HeroAnimationData data in HeroAnimations)
                dict.Add(data.DataID, data);

            return dict;
        }
    }
}