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
            SetBodySprites(data, heroBody, EHeroBodyParts.Weapon);
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
            else if (parts == EHeroBodyParts.LowerBody)
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
            else // Wepon
            {
                Data.HeroSpriteData_Weapon weapon = Managers.Data.HeroesSpritesDict[data.DataID].Weapon;

                // Weapon - Left
                Transform weaponL = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponL);
                sprite = Managers.Resource.Load<Sprite>(weapon.LeftWeapon);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    SpriteRenderer spr = heroBody.GetComponent<SpriteRenderer>(EHeroWeapon.WeaponL);
                    spr.sprite = clone;
                    if (ColorUtility.TryParseHtmlString(weapon.LeftColor, out color))
                        spr.color = color;
                    
                    Vector3 scale = weapon.LeftScale;
                    Vector3 position = weapon.LeftPosition;
                    Vector3 rotation = weapon.LeftRotation;

                    weaponL.localScale = new Vector3(scale.x, scale.y, scale.z);
                    weaponL.localPosition = new Vector3(position.x, position.y, position.z);
                    weaponL.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                    for (int i = 0; i < weaponL.childCount; ++i)
                    {
                        if (weapon.LeftWeaponAttachments[i] != null)
                        {
                            Sprite childSprite = Managers.Resource.Load<Sprite>(weapon.LeftWeaponAttachments[i]);
                            if (childSprite != null)
                            {
                                Sprite childClone = UnityEngine.Object.Instantiate(childSprite);
                                weaponL.GetChild(i).GetComponent<SpriteRenderer>().sprite = childClone;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < weaponL.childCount; ++i)
                        weaponL.GetChild(i).gameObject.SetActive(false);
                    weaponL.gameObject.SetActive(false);
                }

                // Weapon - Right
                Transform weaponR = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponR);
                sprite = Managers.Resource.Load<Sprite>(weapon.RightWeapon);
                if (sprite != null)
                {
                    clone = UnityEngine.Object.Instantiate(sprite);
                    SpriteRenderer spr = heroBody.GetComponent<SpriteRenderer>(EHeroWeapon.WeaponR);
                    spr.sprite = clone;
                    if (ColorUtility.TryParseHtmlString(weapon.RightColor, out color))
                        spr.color = color;
                    
                    Vector3 scale = weapon.RightScale;
                    Vector3 position = weapon.RightPosition;
                    Vector3 rotation = weapon.RightRotation;

                    weaponR.localScale = new Vector3(scale.x, scale.y, scale.z);
                    weaponR.localPosition = new Vector3(position.x, position.y, position.z);
                    weaponR.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                    for (int i = 0; i < weaponR.childCount; ++i)
                    {
                        if (weapon.RightWeaponAttachments.Count != 0)
                        {
                            Sprite childSprite = Managers.Resource.Load<Sprite>(weapon.RightWeaponAttachments[i]);
                            if (childSprite != null)
                            {
                                Sprite childClone = UnityEngine.Object.Instantiate(childSprite);
                                weaponR.GetChild(i).GetComponent<SpriteRenderer>().sprite = childClone;
                            }
                        }
                        else
                            weaponR.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    for (int i = 0; i < weaponR.childCount; ++i)
                        weaponR.GetChild(i).gameObject.SetActive(false);
                    weaponR.gameObject.SetActive(false);
                }
            }
        }
    }
}
