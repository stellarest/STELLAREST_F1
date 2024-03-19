using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1.Data
{
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
            foreach (TestData test in tests)
                dict.Add(test.Level, test);

            return dict;
        }
    }

    [Serializable]
    public class HeroData
    {
        public int DataID;
        public string DescriptionTextID;
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
            foreach (HeroData hero in Heroes)
                dict.Add(hero.DataID, hero);

            return dict;
        }
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
    public class HeroSpriteData
    {
        public int DataID;
        public string Tag;
        public string SkinColor;
        public HeroSpriteData_Head Head;
        public HeroSpriteData_UpperBody UpperBody;
        public HeroSpriteData_LowerBody LowerBody;        
    }

    public class HeroSpriteDataLoader : ILoader<int, HeroSpriteData>
    {
        public List<HeroSpriteData> HeroesSprites = new List<HeroSpriteData>();
        public Dictionary<int, HeroSpriteData> MakeDict()
        {
            Dictionary<int, HeroSpriteData> dict = new Dictionary<int, HeroSpriteData>();
            foreach (HeroSpriteData heroSprite in HeroesSprites)
                dict.Add(heroSprite.DataID, heroSprite);
            
            return dict;
        }
    }
}