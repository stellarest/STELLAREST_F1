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

        private void SetSpritesSet(int dataID, HeroBody heroBody)
        {
            Data.HeroSpriteData data = Managers.Data.HeroSpriteDataDict[dataID];
            SetSkin(data, heroBody);
            SetBodySprites(data, heroBody, EHeroBodyParts.Head);
            SetBodySprites(data, heroBody, EHeroBodyParts.UpperBody);
            SetBodySprites(data, heroBody, EHeroBodyParts.LowerBody);
            SetBodySprites(data, heroBody, EHeroBodyParts.Weapon, ReadOnly.Numeric.SortingOrder_Weapon);
            heroBody.SetFace();
        }

        private void SetSkin(Data.HeroSpriteData data, HeroBody heroBody)
        {
            if (ColorUtility.TryParseHtmlString(data.SkinColor, out Color color) == false)
                return;

            Sprite sprite = null;
            EHeroBodyType type = Util.GetEnumFromString<EHeroBodyType>(data.BodyType);
            if (type == EHeroBodyType.Human)
            {
                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Head);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.HeadSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.HeadSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Torso);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.TorsoSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.TorsoSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_ArmL);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmLSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmLSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_ArmR);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmRSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmRSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_ForearmL);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmLSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmLSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_ForearmR);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmRSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmRSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_HandL);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandLSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandLSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_HandR);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandRSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandRSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Finger);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.FingerSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.FingerSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Pelvis);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.PelvisSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.PelvisSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Shin);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinLSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinLSkin).color = color;

                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinRSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinRSkin).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Leg);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegLSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegLSkin).color = color;

                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegRSkin).sprite = sprite;
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegRSkin).color = color;
                }
            }
        }

        private void SetBodySprites(Data.HeroSpriteData data, HeroBody heroBody, EHeroBodyParts parts, int sortingOrder = 0)
        {
            Sprite sprite = null;
            Color color = Color.white;
            if (parts == EHeroBodyParts.Head)
            {
                Data.HeroSpriteData_Head head = Managers.Data.HeroSpriteDataDict[data.DataID].Head;
                sprite = Managers.Resource.Load<Sprite>(head.Hair);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Hair).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.HairColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Hair).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Eyebrows);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.EyebrowsColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Eyes);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.EyesColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Mouth);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.MouthColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Beard);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Beard).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.BeardColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Beard).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Earrings);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Earrings).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.EarringsColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Earrings).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Mask);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mask).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.MaskColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mask).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Glasses);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Glasses).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.GlassesColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Glasses).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Helmet);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Helmet).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.HelmetColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Helmet).color = color;
                }
            }
            else if (parts == EHeroBodyParts.UpperBody)
            {
                Data.HeroSpriteData_UpperBody upperBody = Managers.Data.HeroSpriteDataDict[data.DataID].UpperBody;
                sprite = Managers.Resource.Load<Sprite>(upperBody.Torso);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Torso).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.TorsoColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Torso).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.Cape);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Cape).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.CapeColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Cape).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ArmL);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmL).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmL);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmL).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.HandL);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandL).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.HandLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.Finger);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Finger).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.FingerColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Finger).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ArmR);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmR).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmR).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmR);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmR).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmR).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.SleeveR);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.SleeveR).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.SleeveRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.SleeveR).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.HandR);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandR).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.HandRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandR).color = color;
                }
            }
            else if (parts == EHeroBodyParts.LowerBody)
            {
                Data.HeroSpriteData_LowerBody lowerBody = Managers.Data.HeroSpriteDataDict[data.DataID].LowerBody;
                sprite = Managers.Resource.Load<Sprite>(lowerBody.Pelvis);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.Pelvis).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.PelvisColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.Pelvis).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.LegL);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegL).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinL);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinL).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinLColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinL).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.LegR);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegR).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegR).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinR);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinR).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinRColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinR).color = color;
                }
            }
            else // Wepon
            {
                Data.HeroSpriteData_Weapon weapon = Managers.Data.HeroSpriteDataDict[data.DataID].Weapon;

                // Weapon - Left
                Transform weaponL = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponL);
                sprite = Managers.Resource.Load<Sprite>(weapon.LeftWeapon);
                if (sprite != null)
                {
                    SpriteRenderer spr = heroBody.GetComponent<SpriteRenderer>(EHeroWeapon.WeaponL);
                    spr.sprite = sprite;
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
                    SpriteRenderer spr = heroBody.GetComponent<SpriteRenderer>(EHeroWeapon.WeaponR);
                    spr.sprite = sprite;
                    spr.sortingOrder = sortingOrder;
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
