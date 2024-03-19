using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class SpriteManager
    {
        public void InitSprites(int dataID, Transform[] targets, EObjectType objType)
        {
            switch (objType)
            {
                case EObjectType.Hero:
                    //InitHeroSprites(dataID, targets);
                    break;
            }
        }

        // private void InitHeroSprites(int dataID, Transform[] targets)
        // {
        //     Data.HeroSpriteData sprites = Managers.Data.HeroesSpritesDict[dataID];
        //     Type type = typeof(Data.HeroSpriteData);
        //     FieldInfo[] fields = type.GetFields();
        // }

        // private void InitHeroSprites(int dataID, Transform[] targets)
        // {
        //     Data.HeroSpriteData sprites = Managers.Data.HeroesSpritesDict[dataID];
            
            
        //     Type type = typeof(Data.HeroSpriteData);
        //     FieldInfo[] fields = type.GetFields();
        //     for (int i = 0; i < fields.Length; ++i)
        //     {
        //         if (fields[i].FieldType != typeof(string) || fields[i].Name.Contains("Tag"))
        //             continue;

        //         string fieldValue = (string)fields[i].GetValue(sprites);
        //         if (string.IsNullOrEmpty(fieldValue))
        //             continue;

        //         string fieldName = fields[i].Name;
        //         EHeroBodyParts bodyPart = Util.GetEnumFromString<EHeroBodyParts>(fieldName);
        //         Sprite sprite = Managers.Resource.Load<Sprite>(fieldValue);
        //         targets[(int)bodyPart].GetComponent<SpriteRenderer>().sprite = sprite;
        //     }
        // }
    }
}
