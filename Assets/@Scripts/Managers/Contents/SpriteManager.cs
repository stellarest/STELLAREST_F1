using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using STELLAREST_F1.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class SpriteManager
    {
        public Dictionary<EEnvType, Sprite[]> HeroCollectEnvWeaponSpritesDict { get; private set; } = null;

        public void Init()
        {
            if (HeroCollectEnvWeaponSpritesDict == null)
            {
                HeroCollectEnvWeaponSpritesDict = new Dictionary<EEnvType, Sprite[]>();
                Sprite[] envWeaponSprites = new Sprite[(int)ECollectEnvRarity.Max];
                HeroCollectEnvWeaponSpritesDict.Add(EEnvType.Tree, envWeaponSprites);
                envWeaponSprites[(int)ECollectEnvRarity.Common] = Managers.Resource.Load<Sprite>(ReadOnly.Util.WoodcutterAxe_Common_SP);
                envWeaponSprites[(int)ECollectEnvRarity.Elite] = Managers.Resource.Load<Sprite>(ReadOnly.Util.WoodcutterAxe_Elite_SP);

                envWeaponSprites = new Sprite[(int)ECollectEnvRarity.Max];
                HeroCollectEnvWeaponSpritesDict.Add(EEnvType.Rock, envWeaponSprites);
                envWeaponSprites[(int)ECollectEnvRarity.Common] = Managers.Resource.Load<Sprite>(ReadOnly.Util.Pickaxe_Common_SP);
                envWeaponSprites[(int)ECollectEnvRarity.Elite] = Managers.Resource.Load<Sprite>(ReadOnly.Util.Pickaxe_Elite_SP);
            }
        }

        public void SetInfo(int dataID, BaseObject target)
        {
            switch (target.ObjectType)
            {
                case EObjectType.Hero:
                    SetSpritesSet(dataID, (target as Hero).HeroBody);
                    break;

                case EObjectType.Monster:
                    SetSpritesSet(dataID, (target as Monster).MonsterBody);
                    break;

                case EObjectType.Env:
                    SetSpritesSet(dataID, target as Env);
                    break;

                case EObjectType.Projectile:
                    SetSpritesSet(dataID, target as Projectile);
                    break;
            }
        }

        #region Init - Hero Sprites
        private void SetSpritesSet(int dataID, HeroBody heroBody)
        {
            HeroSpriteData heroSpriteData = Managers.Data.HeroSpriteDataDict[dataID];
            SetSkinSprites(heroSpriteData, heroBody);
            SetBodySprites(heroSpriteData, heroBody, EHeroBodyParts.Head);
            SetBodySprites(heroSpriteData, heroBody, EHeroBodyParts.UpperBody);
            SetBodySprites(heroSpriteData, heroBody, EHeroBodyParts.LowerBody);
            SetBodySprites(heroSpriteData, heroBody, EHeroBodyParts.Weapon);
            heroBody.SetFace();
        }

        private void SetSkinSprites(HeroSpriteData heroSpriteData, HeroBody heroBody)
        {
            HeroSpriteData_Skin skinSpriteData = heroSpriteData.Skin;
            if (ColorUtility.TryParseHtmlString(skinSpriteData.SkinColor, out Color skinColor) == false)
            {
                Debug.LogError($"{nameof(SetSkinSprites)}, Input: \"{skinSpriteData.SkinColor}\" ");
                return;
            }

            SpriteRenderer spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.HeadSkin);
            Sprite sprite = Managers.Resource.Load<Sprite>(skinSpriteData.Head);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Appearance.Add(spr);
            }
            
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Ears);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.Ears);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Appearance.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.TorsoSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.Torso);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmLSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.ArmL);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmLSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.ForearmL);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandLSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.HandL);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmRSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.ArmR);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmRSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.ForearmR);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandRSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.HandR);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.FingerSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.Finger);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.PelvisSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.Pelvis);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinLSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.Shin);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegLSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.Leg);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinRSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.Shin);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }

            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegRSkin);
            sprite = Managers.Resource.Load<Sprite>(skinSpriteData.Leg);
            if (spr != null && sprite != null)
            {
                spr.sprite = sprite;
                spr.color = skinColor;
                heroBody.Skin.Add(spr);
            }
        } 

        private void SetBodySprites(Data.HeroSpriteData heroSpriteData, HeroBody heroBody, EHeroBodyParts parts)
        {
            Sprite sprite = null;
            SpriteRenderer spr = null;
            Color color = Color.white;
            if (parts == EHeroBodyParts.Head)
            {
                Data.HeroSpriteData_Head head = Managers.Data.HeroSpriteDataDict[heroSpriteData.DataID].Head;
                sprite = Managers.Resource.Load<Sprite>(head.Hair);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Hair);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.HairColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }


                // --- HeroFace: Eyebrows
                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows);
                heroBody.Appearance.Add(spr);
                // sprite = Managers.Resource.Load<Sprite>(head.Eyebrows[(int)EHeroEmoji.Idle]);
                // if (sprite != null)
                // {
                //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows);
                //     spr.sprite = sprite;
                //     if (ColorUtility.TryParseHtmlString(head.EyebrowsColors[(int)EHeroEmoji.Idle], out color))
                //         spr.color = color;

                //     heroBody.Appearance.Add(spr);
                // }


                // --- HeroFace: Eyes
                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes);
                heroBody.Appearance.Add(spr);
                // sprite = Managers.Resource.Load<Sprite>(head.Eyes[(int)EHeroEmoji.Idle]);
                // if (sprite != null)
                // {
                //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes);
                //     spr.sprite = sprite;
                //     if (ColorUtility.TryParseHtmlString(head.EyesColors[(int)EHeroEmoji.Idle], out color))
                //         spr.color = color;

                //     heroBody.Appearance.Add(spr);
                // }


                // --- HeroFace: Mouth
                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth);
                heroBody.Appearance.Add(spr);
                // sprite = Managers.Resource.Load<Sprite>(head.Mouth[(int)EHeroEmoji.Idle]);
                // if (sprite != null)
                // {
                //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth);
                //     spr.sprite = sprite;
                //     if (ColorUtility.TryParseHtmlString(head.MouthColors[(int)EHeroEmoji.Idle], out color))
                //         spr.color = color;
                    
                //     heroBody.Appearance.Add(spr);
                // }


                sprite = Managers.Resource.Load<Sprite>(head.Beard);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Beard);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.BeardColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(head.Earrings);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Earrings);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.EarringsColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(head.Mask);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mask);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.MaskColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(head.Glasses);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Glasses);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.GlassesColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(head.Helmet);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Helmet);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.HelmetColor, out color))
                        spr.color = color;
                }
            }
            else if (parts == EHeroBodyParts.UpperBody)
            {
                Data.HeroSpriteData_UpperBody upperBody = Managers.Data.HeroSpriteDataDict[heroSpriteData.DataID].UpperBody;
                sprite = Managers.Resource.Load<Sprite>(upperBody.Torso);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Torso);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.TorsoColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.Cape);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Cape);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.CapeColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ArmL);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmL);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmLColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmL);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmL);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmLColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.HandL);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandL);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.HandLColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.Finger);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Finger);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.FingerColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ArmR);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmR);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmRColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmR);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmR);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmRColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.SleeveR);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.SleeveR);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.SleeveRColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(upperBody.HandR);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandR);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.HandRColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }
            }
            else if (parts == EHeroBodyParts.LowerBody)
            {
                Data.HeroSpriteData_LowerBody lowerBody = Managers.Data.HeroSpriteDataDict[heroSpriteData.DataID].LowerBody;
                sprite = Managers.Resource.Load<Sprite>(lowerBody.Pelvis);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.Pelvis);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.PelvisColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.LegL);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegL);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegLColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinL);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinL);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinLColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.LegR);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegR);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegRColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }

                sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinR);
                if (sprite != null)
                {
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinR);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinRColor, out color))
                        spr.color = color;

                    heroBody.Appearance.Add(spr);
                }
            }
            else // --- WEAPON
            {
                List<SpriteRenderer> leftWeaponSPRs = new List<SpriteRenderer>();
                List<SpriteRenderer> rightWeaponSPRs = new List<SpriteRenderer>();
                HeroSpriteData_Weapon weapon = Managers.Data.HeroSpriteDataDict[heroSpriteData.DataID].Weapon;

                // --- WEAPON LEFT
                Transform weaponL = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponL);
                Transform weaponLTrRoot = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponLChildGroup);
                sprite = Managers.Resource.Load<Sprite>(weapon.LWeapon);
                if (sprite != null)
                {
                    weaponL.localScale = weapon.LWeaponLocalScale;
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroWeapon.WeaponL);
                    heroBody.SetLeftWeapon(spr.transform, weapon.LWeaponLocalScale);
                    spr.sprite = sprite;
                    spr.sortingOrder = weapon.LWeaponSorting;
                    spr.flipX = weapon.LWeaponFlipX;
                    spr.flipY = weapon.LWeaponFlipY;
                    leftWeaponSPRs.Add(spr);
                    heroBody.Appearance.Add(spr);
                    heroBody.GetComponent<Transform>(EHeroWeapon.WeaponLSocket).localPosition = weapon.LWeaponFireSocketLocalPosition;

                    if (weapon.LWeaponChilds.Length != 0)
                    {
                        int childLength = weapon.LWeaponChilds.Length;
                        for (int i = 0; i < childLength; ++i)
                        {
                            Sprite childSprite = Managers.Resource.Load<Sprite>(weapon.LWeaponChilds[i]);
                            if (childSprite != null)
                            {
                                Transform childParent = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponLChildGroup); // added
                                SpriteRenderer childSPR = childParent.GetChild(i).GetComponent<SpriteRenderer>(); // fixed

                                childSPR.sprite = childSprite;
                                childSPR.transform.localPosition = weapon.LWeaponChildsLocalPositions[i]; // Added
                                childSPR.sortingOrder = weapon.LWeaponChildSortings[i];
                                childSPR.flipX = weapon.LWeaponChildFlipXs[i];
                                childSPR.flipY = weapon.LWeaponChildFlipYs[i];
                                leftWeaponSPRs.Add(childSPR);
                                heroBody.Appearance.Add(childSPR);
                            }
                        }

                        // L: 사용하지 않는 나머지 자식은 비활성화 (애니메이션으로 제어할 경우 제외)
                        // Archer는 3번 무기를 사용하지 않지만, 나중에 진화할 때 필요할 수도 있어서 애니메이션에서는
                        // L,R Root 자체를 껏켜하는중(*** 현재 애니메이션 제어에서 동작 안되는중 ***)
                        for (int i = 0; i < weaponLTrRoot.childCount; ++i)
                        {
                            GameObject child = weaponLTrRoot.GetChild(i).gameObject;
                            if (child.GetComponent<SpriteRenderer>().sprite == null)
                                child.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        // L: 자식 무기가 아무것도 없으면 루트 자체를 비활성화
                        // 비어있는 무기 오브젝트 자체를 비활성화하지 않는다. 맨주먹 소켓을 사용할 수도 있기 때문.
                        weaponLTrRoot.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // L: 만약에 무기가 없는 캐릭터라면.. 전부 비활성화
                    weaponLTrRoot.gameObject.SetActive(false);
                }

                // --- WEAPON RIGHT
                Transform weaponR = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponR);
                Transform weaponRTrRoot = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponRChildGroup);
                sprite = Managers.Resource.Load<Sprite>(weapon.RWeapon);
                if (sprite != null)
                {
                    weaponR.localScale = weapon.RWeaponLocalScale;
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroWeapon.WeaponR);
                    heroBody.SetRightWeapon(spr.transform, weapon.RWeaponLocalScale);
                    spr.sprite = sprite;
                    spr.sortingOrder = weapon.RWeaponSorting;
                    spr.flipX = weapon.RWeaponFlipX;
                    spr.flipY = weapon.RWeaponFlipY;
                    rightWeaponSPRs.Add(spr);
                    heroBody.Appearance.Add(spr);
                    heroBody.GetComponent<Transform>(EHeroWeapon.WeaponRSocket).localPosition = weapon.RWeaponFireSocketLocalPosition;

                    if (weapon.RWeaponChilds.Length != 0)
                    {
                        int childLength = weapon.RWeaponChilds.Length;
                        for (int i = 0; i < childLength; ++i)
                        {
                            Sprite childSprite = Managers.Resource.Load<Sprite>(weapon.RWeaponChilds[i]);
                            if (childSprite != null)
                            {
                                Transform childParent = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponRChildGroup); // added
                                SpriteRenderer childSPR = childParent.GetChild(i).GetComponent<SpriteRenderer>();
                                
                                childSPR.sprite = childSprite;
                                childSPR.transform.localPosition = weapon.RWeaponChildsLocalPositions[i]; // Added
                                childSPR.sortingOrder = weapon.RWeaponChildSortings[i];
                                childSPR.flipX = weapon.RWeaponChildFlipXs[i];
                                childSPR.flipY = weapon.RWeaponChildFlipYs[i];
                                rightWeaponSPRs.Add(childSPR);
                                heroBody.Appearance.Add(childSPR);
                            }
                        }

                        // R: 사용하지 않는 나머지 자식은 비활성화 (애니메이션으로 제어할 경우 제외)
                        // Archer는 3번 무기를 사용하지 않지만, 나중에 진화할 때 필요할 수도 있어서 애니메이션에서는
                        // L,R Root 자체를 껏켜하는중(*** 현재 애니메이션 제어에서 동작 안되는중 ***)
                        for (int i = 0; i < weaponRTrRoot.childCount; ++i)
                        {
                            GameObject child = weaponRTrRoot.GetChild(i).gameObject;
                            if (child.GetComponent<SpriteRenderer>().sprite == null)
                                child.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        // R: 자식 무기가 아무것도 없으면 루트 자체를 비활성화
                        // 비어있는 무기 오브젝트 자체를 비활성화하지 않는다. 맨주먹 소켓을 사용할 수도 있기 때문.
                        weaponRTrRoot.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // R: 만약에 무기가 없는 캐릭터라면 루트 자체를 비활성화
                    weaponRTrRoot.gameObject.SetActive(false);
                }

                heroBody.SetDefaultWeaponSprites(leftWeaponSPRs.ToArray(), rightWeaponSPRs.ToArray());
            }
        }
        #endregion

        #region Init - Monster Sprites
        private void SetSpritesSet(int dataID, MonsterBody monsterBody)
        {
            MonsterData monsterData = Managers.Data.MonsterDataDict[dataID];
            //EMonsterType monsterType = Util.GetEnumFromString<EMonsterType>(monsterData.Type);
            switch (monsterData.MonsterType)
            {
                case EMonsterType.Bird:
                    SetBodySprites(dataID, Managers.Data.BirdSpriteDataDict[dataID], monsterBody);
                    break;

                case EMonsterType.Quadrupeds:
                    SetBodySprites(dataID, Managers.Data.QuadrupedsSpriteDataDict[dataID], monsterBody);
                    break;
            }
        }

        private void SetBodySprites(int dataID, Data.BirdSpriteData birdSpriteData, MonsterBody monsterBody)
        {
            if (ColorUtility.TryParseHtmlString(birdSpriteData.SkinColor, out Color color) == false)
                return;

            // Bird - Body
            Transform tr = null;
            SpriteRenderer spr = null;
            Sprite sprite = Managers.Resource.Load<Sprite>(birdSpriteData.Body);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EBirdBodyParts.Body);
                tr.transform.localPosition = birdSpriteData.BodyPosition;
                if (monsterBody.Size == EMonsterSize.Small)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Util.MonsterSize_Small, ReadOnly.Util.MonsterSize_Small, 1);
                else if (monsterBody.Size == EMonsterSize.Medium)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Util.MonsterSize_Medium, ReadOnly.Util.MonsterSize_Medium, 1);
                else if (monsterBody.Size == EMonsterSize.Large)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Util.MonsterSize_Large, ReadOnly.Util.MonsterSize_Large, 1);

                spr = monsterBody.GetComponent<SpriteRenderer>(EBirdBodyParts.Body);
                spr.sprite = sprite;
                spr.sortingOrder = birdSpriteData.BodySortingOrder;
                spr.color = color;
            }

            // Bird - Head
            monsterBody.SetFace(EMonsterEmoji.Default, birdSpriteData.Heads[(int)EMonsterEmoji.Default]);
            monsterBody.SetFace(EMonsterEmoji.Angry, birdSpriteData.Heads[(int)EMonsterEmoji.Angry]);
            monsterBody.SetFace(EMonsterEmoji.Dead, birdSpriteData.Heads[(int)EMonsterEmoji.Dead]);

            sprite = Managers.Resource.Load<Sprite>(birdSpriteData.Heads[(int)EMonsterEmoji.Default]);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EBirdBodyParts.Head);
                tr.transform.localPosition = birdSpriteData.HeadPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EBirdBodyParts.Head);
                spr.sprite = sprite;
                spr.sortingOrder = birdSpriteData.HeadSortingOrder;
                spr.color = color;
            }

            // Bird - Wing
            sprite = Managers.Resource.Load<Sprite>(birdSpriteData.Wing);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EBirdBodyParts.Wing);
                tr.transform.localPosition = birdSpriteData.WingPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EBirdBodyParts.Wing);
                spr.sprite = sprite;
                spr.sortingOrder = birdSpriteData.WingSortingOrder;
                spr.color = color;
            }

            // Bird - LegL
            sprite = Managers.Resource.Load<Sprite>(birdSpriteData.LegL);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EBirdBodyParts.LegL);
                tr.transform.localPosition = birdSpriteData.LegLPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EBirdBodyParts.LegL);
                spr.sprite = sprite;
                spr.sortingOrder = birdSpriteData.LegLSortingOrder;
                spr.color = color;
            }

            // Bird - LegR
            sprite = Managers.Resource.Load<Sprite>(birdSpriteData.LegR);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EBirdBodyParts.LegR);
                tr.transform.localPosition = birdSpriteData.LegRPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EBirdBodyParts.LegR);
                spr.sprite = sprite;
                spr.sortingOrder = birdSpriteData.LegRSortingOrder;
                spr.color = color;
            }

            // Bird - Tail
            sprite = Managers.Resource.Load<Sprite>(birdSpriteData.Tail);
            tr = monsterBody.GetComponent<Transform>(EBirdBodyParts.Tail);
            if (sprite != null)
            {
                tr.transform.localPosition = birdSpriteData.TailPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EBirdBodyParts.Tail);
                spr.sprite = sprite;
                spr.sortingOrder = birdSpriteData.TailSortingOrder;
                spr.color = color;
            }
            else
            {
                tr.gameObject.SetActive(false);
            }
        }

        private void SetBodySprites(int dataID, Data.QuadrupedsSpriteData quadrupedsSpriteData, MonsterBody monsterBody)
        {
            if (ColorUtility.TryParseHtmlString(quadrupedsSpriteData.SkinColor, out Color color) == false)
                return;

            // Quadrupeds - Body
            Transform tr = null;
            SpriteRenderer spr = null;
            Sprite sprite = Managers.Resource.Load<Sprite>(quadrupedsSpriteData.Body);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EQuadrupedsParts.Body);
                tr.transform.localPosition = quadrupedsSpriteData.BodyPosition;
                if (monsterBody.Size == EMonsterSize.Small)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Util.MonsterSize_Small, ReadOnly.Util.MonsterSize_Small, 1);
                else if (monsterBody.Size == EMonsterSize.Medium)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Util.MonsterSize_Medium, ReadOnly.Util.MonsterSize_Medium, 1);
                else if (monsterBody.Size == EMonsterSize.Large)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Util.MonsterSize_Large, ReadOnly.Util.MonsterSize_Large, 1);

                spr = monsterBody.GetComponent<SpriteRenderer>(EQuadrupedsParts.Body);
                spr.sprite = sprite;
                spr.sortingOrder = quadrupedsSpriteData.BodySortingOrder;
                spr.color = color;
            }

            // Quadrupeds - Head
            monsterBody.SetFace(EMonsterEmoji.Default, quadrupedsSpriteData.Heads[(int)EMonsterEmoji.Default]);
            monsterBody.SetFace(EMonsterEmoji.Angry, quadrupedsSpriteData.Heads[(int)EMonsterEmoji.Angry]);
            monsterBody.SetFace(EMonsterEmoji.Dead, quadrupedsSpriteData.Heads[(int)EMonsterEmoji.Dead]);

            sprite = Managers.Resource.Load<Sprite>(quadrupedsSpriteData.Heads[(int)EMonsterEmoji.Default]);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EQuadrupedsParts.Head);
                tr.transform.localPosition = quadrupedsSpriteData.HeadPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EQuadrupedsParts.Head);
                spr.sprite = sprite;
                spr.sortingOrder = quadrupedsSpriteData.HeadSortingOrder;
                spr.color = color;
            }

            // Quadrupeds - LegFrontL
            sprite = Managers.Resource.Load<Sprite>(quadrupedsSpriteData.LegFrontL);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EQuadrupedsParts.LegFrontL);
                tr.transform.localPosition = quadrupedsSpriteData.LegFrontLPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EQuadrupedsParts.LegFrontL);
                spr.sprite = sprite;
                spr.sortingOrder = quadrupedsSpriteData.LegFrontLSortingOrder;
                spr.color = color;
            }

            // Quadrupeds - LegFrontR
            sprite = Managers.Resource.Load<Sprite>(quadrupedsSpriteData.LegFrontR);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EQuadrupedsParts.LegFrontR);
                tr.transform.localPosition = quadrupedsSpriteData.LegFrontRPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EQuadrupedsParts.LegFrontR);
                spr.sprite = sprite;
                spr.sortingOrder = quadrupedsSpriteData.LegFrontRSortingOrder;
                spr.color = color;
            }

            // Quadrupeds - LegBackL
            sprite = Managers.Resource.Load<Sprite>(quadrupedsSpriteData.LegBackL);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EQuadrupedsParts.LegBackL);
                tr.transform.localPosition = quadrupedsSpriteData.LegBackLPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EQuadrupedsParts.LegBackL);
                spr.sprite = sprite;
                spr.sortingOrder = quadrupedsSpriteData.LegBackLSortingOrder;
                spr.color = color;
            }

            // Quadrupeds - LegBackR
            sprite = Managers.Resource.Load<Sprite>(quadrupedsSpriteData.LegBackR);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EQuadrupedsParts.LegBackR);
                tr.transform.localPosition = quadrupedsSpriteData.LegBackRPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EQuadrupedsParts.LegBackR);
                spr.sprite = sprite;
                spr.sortingOrder = quadrupedsSpriteData.LegBackRSortingOrder;
                spr.color = color;
            }

            // Quadrupeds - Tail
            sprite = Managers.Resource.Load<Sprite>(quadrupedsSpriteData.Tail);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EQuadrupedsParts.Tail);
                tr.transform.localPosition = quadrupedsSpriteData.TailPosition;
                spr = monsterBody.GetComponent<SpriteRenderer>(EQuadrupedsParts.Tail);
                spr.sprite = sprite;
                spr.sortingOrder = quadrupedsSpriteData.TailSortingOrder;
                spr.color = color;
            }
        }
        #endregion

        #region Init - Env Sprites
        private void SetSpritesSet(int dataID, Env env)
        {
            Data.EnvData envData = Managers.Data.EnvDataDict[dataID];
            //EEnvType envType = Util.GetEnumFromString<EEnvType>(envData.Type);
            switch (envData.EnvType)
            {
                case EEnvType.Tree:
                    SetBodySprites(dataID, Managers.Data.TreeSpriteDataDict[dataID], env);
                    break;

                case EEnvType.Rock:
                    SetBodySprites(dataID, Managers.Data.RockSpriteDataDict[dataID], env);
                    break;
            }
        }

        private void SetBodySprites(int dataID, Data.TreeSpriteData treeSpriteData, Env env)
        {
            Transform tr = null;
            SpriteRenderer spr = null;
            // Trunk
            Sprite sprite = Managers.Resource.Load<Sprite>(treeSpriteData.Trunk);
            if (sprite != null)
            {
                tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ETreeBody_Trunk, true, true);
                tr.localPosition = treeSpriteData.TrunkPosition;
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = sprite;
            }

            // Patch
            sprite = Managers.Resource.Load<Sprite>(treeSpriteData.Patch);
            if (sprite != null)
            {
                tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ETreeBody_Patch, true, true);
                tr.localPosition = treeSpriteData.PatchPosition;
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = sprite;
            }

            // Stump
            sprite = Managers.Resource.Load<Sprite>(treeSpriteData.Stump);
            if (sprite != null)
            {
                tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ETreeBody_Stump, true, true);
                tr.localPosition = treeSpriteData.StumpPosition;
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = sprite;
            }

            // EndParticles
            tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.EBody_EndParticle, true, true);
            Material endParticleMat = Managers.Resource.Load<Material>(treeSpriteData.EndParticleMaterial);
            if (endParticleMat != null)
            {
                tr.localPosition = Vector3.up * 2f;
                ParticleSystemRenderer pr = tr.GetComponent<ParticleSystemRenderer>();
                pr.material = endParticleMat;
            }
            else
            {
                // Remove Particle System
                ParticleSystem ps = tr.GetComponent<ParticleSystem>();
                UnityEngine.Component.DestroyImmediate(ps);
            }
            tr.gameObject.SetActive(false);

            tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ETreeBody_Fruits, true, true);
            sprite = Managers.Resource.Load<Sprite>(treeSpriteData.Fruits);
            if (sprite != null)
            {
                for (int i = 0; i < treeSpriteData.FruitsPositions.Length; ++i)
                {
                    tr.GetChild(i).localScale = treeSpriteData.FruitsScales[i];
                    tr.GetChild(i).localPosition = treeSpriteData.FruitsPositions[i];
                    Quaternion localRot = Quaternion.Euler(treeSpriteData.FruitsRotations[i].x,
                                                            treeSpriteData.FruitsRotations[i].y,
                                                            treeSpriteData.FruitsRotations[i].z);
                    tr.GetChild(i).localRotation = localRot;

                    spr = tr.GetChild(i).GetComponent<SpriteRenderer>();
                    spr.sprite = sprite;
                }
            }
            // else
            // {
            //     for (int i = 0; i < tr.childCount; ++i)
            //         tr.GetChild(i).gameObject.SetActive(false);
            // }

            sprite = Managers.Resource.Load<Sprite>(ReadOnly.Util.Shadow_SP);
            if (sprite != null)
            {
                tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.EBody_Shadow, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = sprite;
            }
        }

        private void SetBodySprites(int dataID, Data.RockSpriteData rockSpriteData, Env env)
        {
            Transform tr = null;
            SpriteRenderer spr = null;
            ParticleSystem ps = null;
            ParticleSystemRenderer pr = null;

            string rock = ReadOnly.Util.ERock_Rock_SP;
            string empty = ReadOnly.Util.ERock_Empty_SP;
            Material rockMat = Managers.Resource.Load<Material>(ReadOnly.Util.ERock_Mat);

            Sprite sprite = Managers.Resource.Load<Sprite>(rock);
            if (sprite != null)
            {
                tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ERock_Rock, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = sprite;
                spr.material = rockMat;
            }

            sprite = Managers.Resource.Load<Sprite>(empty);
            if (sprite != null)
            {
                tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ERock_Empty, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = sprite;
                spr.material = rockMat;
                tr.gameObject.SetActive(false);
            }

            sprite = Managers.Resource.Load<Sprite>(rockSpriteData.Ore);
            if (sprite != null)
            {
                tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ERock_Ore, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = sprite;
                spr.material = rockMat;
            }

            sprite = Managers.Resource.Load<Sprite>(rockSpriteData.OreShadow);
            if (sprite != null)
            {
                tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ERock_OreShadow, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = sprite;
                spr.material = rockMat;
            }

            string colorCode = rockSpriteData.OreLightColor;
            tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ERock_OreLight, true, true);
            if (string.IsNullOrEmpty(colorCode) == false)
            {
                if (ColorUtility.TryParseHtmlString(colorCode, out Color oreLightColor))
                {
                    sprite = Managers.Resource.Load<Sprite>(ReadOnly.Util.Light_SP);
                    spr = tr.GetComponent<SpriteRenderer>();
                    spr.sprite = sprite;
                    spr.color = new Color(oreLightColor.r, oreLightColor.g, oreLightColor.b, spr.color.a);
                }
            }
            else
                tr.gameObject.SetActive(false);

            tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.ERock_OreParticle, true, true);
            colorCode = rockSpriteData.OreParticleColor;
            if (string.IsNullOrEmpty(colorCode) == false)
            {
                if (ColorUtility.TryParseHtmlString(colorCode, out Color oreParticleColor))
                {
                    ps = tr.GetComponent<ParticleSystem>();
                    var main = ps.main;
                    main.startColor = oreParticleColor;
                    main.maxParticles = rockSpriteData.OreMaxParticleCount;
                }

                // *** Rock이 아닌, 다른 객체에서 Glow를 건드리게 되면 복사 생성으로 변경 ***
                pr = tr.GetComponent<ParticleSystemRenderer>();
                pr.material =  Managers.Resource.Load<Material>(ReadOnly.Util.Glow_Mat);
            }
            else
                tr.gameObject.SetActive(false);

            tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.EBody_EndParticle, true, true);
            pr = tr.GetComponent<ParticleSystemRenderer>();
            pr.material = rockMat;
            tr.gameObject.SetActive(false);

            // Spots
            for (int i = 0; i < rockSpriteData.Spots.Length; ++i)
            {
                string name = ReadOnly.Util.ERock_Spot + (i + 1).ToString();
                tr = Util.FindChild<Transform>(env.gameObject, name, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = Managers.Resource.Load<Sprite>(rockSpriteData.Spots[i]);
                spr.flipX = rockSpriteData.SpotsFlipXs[i];
            }

            // Fragments
            for (int i = 0; i < rockSpriteData.Fragments.Length; ++i)
            {
                string name = ReadOnly.Util.ERock_Fragment + (i + 1).ToString();
                tr = Util.FindChild<Transform>(env.gameObject, name, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = Managers.Resource.Load<Sprite>(rockSpriteData.Fragments[i]);
                spr.flipX = rockSpriteData.FragmentsFlipXs[i];
            }

            // Shadow
            sprite = Managers.Resource.Load<Sprite>(ReadOnly.Util.Shadow_SP);
            if (sprite != null)
            {
                tr = Util.FindChild<Transform>(env.gameObject, ReadOnly.Util.EBody_Shadow, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.sprite = sprite;
            }
        }
        #endregion

        #region Init - Projectile Sprites
        private void SetSpritesSet(int dataID, Projectile projectile)
        {
            Data.ProjectileData projectileData = Managers.Data.ProjectileDataDict[dataID];
            SpriteRenderer spr = projectile.GetComponent<SpriteRenderer>();
            Sprite sprite = Managers.Resource.Load<Sprite>(projectileData.Body);
            if (sprite != null)
            {
                spr.sprite = sprite;
                if (ColorUtility.TryParseHtmlString(projectileData.BodyColor, out Color bodyColor))
                    spr.color = bodyColor;

                // 이미 BaseObject의 SortingGroup으로 layer 정렬 중임
                //spr.sortingOrder = ReadOnly.Numeric.SortingLayer_Projectile;
            }
        }
        #endregion

        #region Change Sprite Set
        public void ChangeSpriteSet(int newDataID, BaseObject owner)
        {
            switch (owner.ObjectType)
            {
                case EObjectType.Hero:
                    Hero hero = owner as Hero;
                    HeroBody heroBody = hero.HeroBody;
                    HeroSpriteData heroCurrentSpriteData = Managers.Data.HeroSpriteDataDict[owner.DataTemplateID];
                    HeroSpriteData heroNewSpriteData = Managers.Data.HeroSpriteDataDict[newDataID];

                    heroBody.Face.ChangeHeroFaceSet(heroNewSpriteData);
                    //ChangeBodyColor(heroBody, heroCurrentSpriteData, heroNewSpriteData);
                    ChangeSkinSprites(heroBody, heroNewSpriteData);
                    ChangeBodySprites(heroBody, heroNewSpriteData, EHeroBodyParts.Head);
                    ChangeBodySprites(heroBody, heroNewSpriteData, EHeroBodyParts.UpperBody);
                    ChangeBodySprites(heroBody, heroNewSpriteData, EHeroBodyParts.LowerBody);
                    ChangeBodySprites(heroBody, heroNewSpriteData, EHeroBodyParts.Weapon);
                    break;
            }
        }

        private void ChangeSkinSprites(HeroBody heroBody, HeroSpriteData heroNewSpriteData)
        {
            HeroSpriteData_Skin newSkinData = heroNewSpriteData.Skin;
            if (ColorUtility.TryParseHtmlString(newSkinData.SkinColor, out Color newSkinColor) == false)
                return;

            // --- Head
            SpriteRenderer spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.HeadSkin);
            Sprite sprite = Managers.Resource.Load<Sprite>(newSkinData.Head);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- Ears
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Ears);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.Ears);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- Torso
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.TorsoSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.Torso);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- ArmL
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmLSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.ArmL);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- ForearmL
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmLSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.ArmL);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- HandL
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandLSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.HandL);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- ArmR
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmRSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.ArmR);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- ForearmR
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmRSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.ForearmR);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- HandR
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandRSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.HandR);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- Finger
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Finger);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.Finger);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- Pelvis
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.PelvisSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.Pelvis);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- ShinL
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinLSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.Shin);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- LegL
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegLSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.Leg);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- ShinR
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinRSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.Shin);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;

            // --- LegR
            spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegRSkin);
            sprite = Managers.Resource.Load<Sprite>(newSkinData.Leg);
            if (sprite != null)
            {
                spr.sprite = sprite;
                spr.color = newSkinColor;
            }
            else
                spr.sprite = null;
        }

        private void ChangeBodySprites(HeroBody heroBody, HeroSpriteData heroNewSpriteData, EHeroBodyParts heroBodyParts)
        {
            Sprite sprite = null;
            SpriteRenderer spr = null;
            Color color = Color.white;
            if (heroBodyParts == EHeroBodyParts.Head)
            {
                HeroSpriteData_Head head = Managers.Data.HeroSpriteDataDict[heroNewSpriteData.DataID].Head;
                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Hair);
                sprite = Managers.Resource.Load<Sprite>(head.Hair);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.HairColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;


                // --- HeroFace: Eyebrows
                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows);
                sprite = Managers.Resource.Load<Sprite>(head.Eyebrows[(int)EHeroEmoji.Idle]);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.EyebrowsColors[(int)EHeroEmoji.Idle], out color))
                        spr.color = color;
                }


                // --- HeroFace: Eyes
                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes);
                sprite = Managers.Resource.Load<Sprite>(head.Eyes[(int)EHeroEmoji.Idle]);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.EyesColors[(int)EHeroEmoji.Idle], out color))
                        spr.color = color;
                }


                // --- HeroFace: Mouth
                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth);
                sprite = Managers.Resource.Load<Sprite>(head.Mouth[(int)EHeroEmoji.Idle]);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.MouthColors[(int)EHeroEmoji.Idle], out color))
                        spr.color = color;
                }


                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Beard);
                sprite = Managers.Resource.Load<Sprite>(head.Beard);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.BeardColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Earrings);
                sprite = Managers.Resource.Load<Sprite>(head.Earrings);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.EarringsColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mask);
                sprite = Managers.Resource.Load<Sprite>(head.Mask);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.MaskColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Glasses);
                sprite = Managers.Resource.Load<Sprite>(head.Glasses);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.GlassesColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Helmet);
                sprite = Managers.Resource.Load<Sprite>(head.Helmet);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.HelmetColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;
            }
            else if (heroBodyParts == EHeroBodyParts.UpperBody)
            {
                HeroSpriteData_UpperBody upperBody = Managers.Data.HeroSpriteDataDict[heroNewSpriteData.DataID].UpperBody;
                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Torso);
                sprite = Managers.Resource.Load<Sprite>(upperBody.Torso);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.TorsoColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Cape);
                sprite = Managers.Resource.Load<Sprite>(upperBody.Cape);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.CapeColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmL);
                sprite = Managers.Resource.Load<Sprite>(upperBody.ArmL);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmLColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmL);
                sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmL);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmLColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandL);
                sprite = Managers.Resource.Load<Sprite>(upperBody.HandL);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.HandLColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.Finger);
                sprite = Managers.Resource.Load<Sprite>(upperBody.Finger);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.FingerColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmR);
                sprite = Managers.Resource.Load<Sprite>(upperBody.ArmR);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmRColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmR);
                sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmR);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmRColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.SleeveR);
                sprite = Managers.Resource.Load<Sprite>(upperBody.SleeveR);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.SleeveRColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandR);
                sprite = Managers.Resource.Load<Sprite>(upperBody.HandR);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(upperBody.HandRColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;
            }
            else if (heroBodyParts == EHeroBodyParts.LowerBody)
            {
                HeroSpriteData_LowerBody lowerBody = Managers.Data.HeroSpriteDataDict[heroNewSpriteData.DataID].LowerBody;
                spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.Pelvis);
                sprite = Managers.Resource.Load<Sprite>(lowerBody.Pelvis);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.PelvisColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegL);
                sprite = Managers.Resource.Load<Sprite>(lowerBody.LegL);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegLColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinL);
                sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinL);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinLColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegR);
                sprite = Managers.Resource.Load<Sprite>(lowerBody.LegR);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegRColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;

                spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinR);
                sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinR);
                if (sprite != null)
                {
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinRColor, out color))
                        spr.color = color;
                }
                else
                    spr.sprite = null;
            }
            else // --- WEAPON
            {
                List<SpriteRenderer> leftWeaponSPRs = new List<SpriteRenderer>();
                List<SpriteRenderer> rightWeaponSPRs = new List<SpriteRenderer>();
                HeroSpriteData_Weapon weapon = Managers.Data.HeroSpriteDataDict[heroNewSpriteData.DataID].Weapon;

                // --- LEFT WEAPON
                Transform weaponL = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponL);
                Transform weaponLTrRoot = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponLChildGroup);
                sprite = Managers.Resource.Load<Sprite>(weapon.LWeapon);
                if (sprite != null)
                {
                    weaponL.localScale = weapon.LWeaponLocalScale;
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroWeapon.WeaponL);
                    heroBody.SetLeftWeapon(spr.transform, weapon.LWeaponLocalScale);
                    spr.sprite = sprite;
                    spr.sortingOrder = weapon.LWeaponSorting;
                    spr.flipX = weapon.LWeaponFlipX;
                    spr.flipY = weapon.LWeaponFlipY;
                    leftWeaponSPRs.Add(spr);
                    heroBody.GetComponent<Transform>(EHeroWeapon.WeaponLSocket).localPosition = weapon.LWeaponFireSocketLocalPosition;

                    if (weapon.LWeaponChilds.Length != 0)
                    {
                        int childLength = weapon.LWeaponChilds.Length;
                        for (int i = 0; i < childLength; ++i)
                        {
                            Sprite childSprite = Managers.Resource.Load<Sprite>(weapon.LWeaponChilds[i]);
                            if (childSprite != null)
                            {
                                Transform childParent = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponLChildGroup); // added
                                SpriteRenderer childSPR = childParent.GetChild(i).GetComponent<SpriteRenderer>(); // fixed

                                childSPR.sprite = childSprite;
                                childSPR.transform.localPosition = weapon.LWeaponChildsLocalPositions[i]; // Added
                                childSPR.sortingOrder = weapon.LWeaponChildSortings[i];
                                childSPR.flipX = weapon.LWeaponChildFlipXs[i];
                                childSPR.flipY = weapon.LWeaponChildFlipYs[i];
                                leftWeaponSPRs.Add(childSPR);
                            }
                        }

                        // L: 사용하지 않는 나머지 자식은 비활성화 (애니메이션으로 제어할 경우 제외)
                        // Archer는 3번 무기를 사용하지 않지만, 나중에 진화할 때 필요할 수도 있어서 애니메이션에서는
                        // L,R Root 자체를 껏켜하는중(*** 현재 애니메이션 제어에서 동작 안되는중 ***)
                        for (int i = 0; i < weaponLTrRoot.childCount; ++i)
                        {
                            GameObject child = weaponLTrRoot.GetChild(i).gameObject;
                            if (child.GetComponent<SpriteRenderer>().sprite == null)
                                child.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        // L: 자식 무기가 아무것도 없으면 루트 자체를 비활성화
                        // 비어있는 무기 오브젝트 자체를 비활성화하지 않는다. 맨주먹 소켓을 사용할 수도 있기 때문.
                        weaponLTrRoot.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // L: 만약에 무기가 없는 캐릭터라면.. 전부 비활성화
                    weaponLTrRoot.gameObject.SetActive(false);
                }

                // --- WEAPON RIGHT
                Transform weaponR = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponR);
                Transform weaponRTrRoot = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponRChildGroup);
                sprite = Managers.Resource.Load<Sprite>(weapon.RWeapon);
                if (sprite != null)
                {
                    weaponR.localScale = weapon.RWeaponLocalScale;
                    spr = heroBody.GetComponent<SpriteRenderer>(EHeroWeapon.WeaponR);
                    heroBody.SetRightWeapon(spr.transform, weapon.RWeaponLocalScale);
                    spr.sprite = sprite;
                    spr.sortingOrder = weapon.RWeaponSorting;
                    spr.flipX = weapon.RWeaponFlipX;
                    spr.flipY = weapon.RWeaponFlipY;
                    rightWeaponSPRs.Add(spr);
                    heroBody.GetComponent<Transform>(EHeroWeapon.WeaponRSocket).localPosition = weapon.RWeaponFireSocketLocalPosition;

                    if (weapon.RWeaponChilds.Length != 0)
                    {
                        int childLength = weapon.RWeaponChilds.Length;
                        for (int i = 0; i < childLength; ++i)
                        {
                            Sprite childSprite = Managers.Resource.Load<Sprite>(weapon.RWeaponChilds[i]);
                            if (childSprite != null)
                            {
                                Transform childParent = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponRChildGroup); // added
                                SpriteRenderer childSPR = childParent.GetChild(i).GetComponent<SpriteRenderer>();
                                
                                childSPR.sprite = childSprite;
                                childSPR.transform.localPosition = weapon.RWeaponChildsLocalPositions[i]; // Added
                                childSPR.sortingOrder = weapon.RWeaponChildSortings[i];
                                childSPR.flipX = weapon.RWeaponChildFlipXs[i];
                                childSPR.flipY = weapon.RWeaponChildFlipYs[i];
                                rightWeaponSPRs.Add(childSPR);
                            }
                        }

                        // R: 사용하지 않는 나머지 자식은 비활성화 (애니메이션으로 제어할 경우 제외)
                        // Archer는 3번 무기를 사용하지 않지만, 나중에 진화할 때 필요할 수도 있어서 애니메이션에서는
                        // L,R Root 자체를 껏켜하는중(*** 현재 애니메이션 제어에서 동작 안되는중 ***)
                        for (int i = 0; i < weaponRTrRoot.childCount; ++i)
                        {
                            GameObject child = weaponRTrRoot.GetChild(i).gameObject;
                            if (child.GetComponent<SpriteRenderer>().sprite == null)
                                child.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        // R: 자식 무기가 아무것도 없으면 루트 자체를 비활성화
                        // 비어있는 무기 오브젝트 자체를 비활성화하지 않는다. 맨주먹 소켓을 사용할 수도 있기 때문.
                        weaponRTrRoot.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // R: 만약에 무기가 없는 캐릭터라면 루트 자체를 비활성화
                    weaponRTrRoot.gameObject.SetActive(false);
                }

                heroBody.ChangeDefaultWeaponSprites(leftWeaponSPRs.ToArray(), rightWeaponSPRs.ToArray());
            }
        }
        #endregion
    }
}

/*
[ PREV REF ]
// private void SetBodyType(EHeroBodyType bodyType, Data.HeroSpriteData heroData, HeroBody heroBody)
        // {
        //     if (ColorUtility.TryParseHtmlString(heroData.SkinColor, out Color color) == false)
        //         return;

        //     Sprite sprite = null;
        //     SpriteRenderer spr = null;
        //     if (bodyType == EHeroBodyType.Human)
        //     {
        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Head_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.HeadSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Appearance.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Ears_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Ears);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Appearance.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Torso_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.TorsoSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_ArmL_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmLSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_ArmR_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmRSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_ForearmL_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmLSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_ForearmR_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmRSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_HandL_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandLSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_HandR_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandRSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Finger_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.FingerSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Pelvis_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.PelvisSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Shin_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinLSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);

        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinRSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }

        //         sprite = Managers.Resource.Load<Sprite>(ReadOnly.String.HBody_HumanType_Leg_SP);
        //         if (sprite != null)
        //         {
        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegLSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);

        //             spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegRSkin);
        //             spr.sprite = sprite;
        //             spr.color = color;
        //             heroBody.Skin.Add(spr);
        //         }
        //     }
        // }

        // private void ChangeBodyColor(HeroBody heroBody, HeroSpriteData heroCurrentSpriteData, HeroSpriteData heroNewSpriteData)
        // {
        //     if (ColorUtility.TryParseHtmlString(heroCurrentSpriteData.SkinColor, out Color currentSkinColor) == false)
        //         return;

        //     if (ColorUtility.TryParseHtmlString(heroNewSpriteData.SkinColor, out Color newSkinColor) == false)
        //         return;

        //     if (currentSkinColor == newSkinColor)
        //     {
        //         Debug.Log($"<color=yellow>Already Same Skin Color, {currentSkinColor} == {newSkinColor}</color>");
        //         return;
        //     }

        //     // --- Change Body Color(BodyType 자체를 바꾸는 것은 제외, ex. Human to Skeleton은 불가능)
        //     SpriteRenderer spr = null;
        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.HeadSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Ears);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.TorsoSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmLSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ArmRSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmLSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.ForearmRSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandLSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.HandRSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroUpperBody.FingerSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.PelvisSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinLSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.ShinRSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegLSkin);
        //     spr.color = newSkinColor;

        //     spr = heroBody.GetComponent<SpriteRenderer>(EHeroLowerBody.LegRSkin);
        //     spr.color = newSkinColor;
        // }
*/