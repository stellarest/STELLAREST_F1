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
        public void SetInfo(int dataID, BaseObject target)
        {
            switch (target.ObjectType)
            {
                case EObjectType.Hero:
                    SetSpritesSet(dataID, (target as Hero).HeroBody);
                    break;
            }
        }

        public void SetSkinColor(Data.HeroSpriteData data, HeroBody heroBody)
        {
            if (ColorUtility.TryParseHtmlString(data.SkinColor, out Color skinColor) == false)
                return;

            SpriteRenderer[] skin = heroBody.Skin;
            for (int i = 0; i < skin.Length; ++i)
                skin[i].color = skinColor;
        }

        private void SetSpritesSet(int dataID, HeroBody heroBody)
        {
            Data.HeroSpriteData data = Managers.Data.HeroesSpritesDict[dataID];
            SetSkinColor(data, heroBody);
            SetBodySprites(data, heroBody, EHeroBodyParts.Head);
            SetBodySprites(data, heroBody, EHeroBodyParts.UpperBody);
            SetBodySprites(data, heroBody, EHeroBodyParts.LowerBody);
        }

        private void SetBodySprites(Data.HeroSpriteData data, HeroBody heroBody, EHeroBodyParts parts)
        {
            Sprite sprite = null;
            Sprite clone = null;
            Color color = Color.white;
            if (parts == EHeroBodyParts.Head)
            {
                Data.HeroSpriteData_Head head = Managers.Data.HeroesSpritesDict[data.DataID].Head;
                sprite = Managers.Resource.Load<Sprite>(head.Hair);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Hair).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(head.HairColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Hair).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Eyebrows);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(head.EyebrowsColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Eyes);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(head.EyesColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Mouth);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(head.MouthColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Beard);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Beard).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(head.BeardColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Beard).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Earrings);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Earrings).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(head.EarringsColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Earrings).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Mask);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mask).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(head.MaskColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mask).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Glasses);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Glasses).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(head.GlassesColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Glasses).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Helmet);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Helmet).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(head.HelmetColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Helmet).color = color;
                }
            }
            else if (parts == EHeroBodyParts.UpperBody)
            {
                Data.HeroSpriteData_UpperBody upperBody = Managers.Data.HeroesSpritesDict[data.DataID].UpperBody;
                sprite = Managers.Resource.Load<Sprite>(upperBody.Torso);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Torso).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.TorsoColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Torso).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.Cape);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Cape).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.CapeColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Cape).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ArmL);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmL).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmL);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmL).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.HandL);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandL).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.HandLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.Finger);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Finger).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.FingerColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Finger).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ArmR);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmR).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmR).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmR);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmR).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmR).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.SleeveR);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.SleeveR).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.SleeveRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.SleeveR).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.HandR);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandR).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(upperBody.HandRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandR).color = color;
                }
            }
            else
            {
                Data.HeroSpriteData_LowerBody lowerBody = Managers.Data.HeroesSpritesDict[data.DataID].LowerBody;
                sprite = Managers.Resource.Load<Sprite>(lowerBody.Pelvis);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.Pelvis).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(lowerBody.PelvisColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.Pelvis).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.LegL);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegL).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinL);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinL).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.LegR);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegR).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegR).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinR);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinR).sprite = clone;
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinR).color = color;
                }
            }
        }
    }
}
