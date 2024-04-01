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
            }
        }

        #region Init - Hero Sprites
        private void SetSpritesSet(int dataID, HeroBody heroBody)
        {
            Data.HeroData heroData = Managers.Data.HeroDataDict[dataID];
            Data.HeroSpriteData heroSpriteData = Managers.Data.HeroSpriteDataDict[dataID];
            EHeroBodyType bodyType = Util.GetEnumFromString<EHeroBodyType>(heroData.BodyType);

            SetBodyType(bodyType, heroSpriteData, heroBody);
            SetBodySprites(heroSpriteData, heroBody, EHeroBodyParts.Head);
            SetBodySprites(heroSpriteData, heroBody, EHeroBodyParts.UpperBody);
            SetBodySprites(heroSpriteData, heroBody, EHeroBodyParts.LowerBody);
            SetBodySprites(heroSpriteData, heroBody, EHeroBodyParts.Weapon, ReadOnly.Numeric.SortingOrder_Weapon);
            heroBody.SetFace();
        }

        private void SetBodyType(EHeroBodyType bodyType, Data.HeroSpriteData heroData, HeroBody heroBody)
        {
            if (ColorUtility.TryParseHtmlString(heroData.SkinColor, out Color color) == false)
                return;

            Sprite sprite = null;
            if (bodyType == EHeroBodyType.Human)
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

        private void SetBodySprites(Data.HeroSpriteData heroSpriteData, HeroBody heroBody, EHeroBodyParts parts, int sortingOrder = 0)
        {
            Sprite sprite = null;
            Color color = Color.white;
            if (parts == EHeroBodyParts.Head)
            {
                Data.HeroSpriteData_Head head = Managers.Data.HeroSpriteDataDict[heroSpriteData.DataID].Head;
                sprite = Managers.Resource.Load<Sprite>(head.Hair);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Hair).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.HairColor, out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Hair).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Eyebrows[(int)EHeroEmoji.Default]);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.EyebrowsColors[(int)EHeroEmoji.Default], out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Eyes[(int)EHeroEmoji.Default]);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.EyesColors[(int)EHeroEmoji.Default], out color))
                        heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).color = color;
                }

                sprite = Managers.Resource.Load<Sprite>(head.Mouth[(int)EHeroEmoji.Default]);
                if (sprite != null)
                {
                    heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(head.MouthColors[(int)EHeroEmoji.Default], out color))
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
                Data.HeroSpriteData_UpperBody upperBody = Managers.Data.HeroSpriteDataDict[heroSpriteData.DataID].UpperBody;
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
                Data.HeroSpriteData_LowerBody lowerBody = Managers.Data.HeroSpriteDataDict[heroSpriteData.DataID].LowerBody;
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
                Data.HeroSpriteData_Weapon weapon = Managers.Data.HeroSpriteDataDict[heroSpriteData.DataID].Weapon;

                // Weapon - Left
                Transform weaponL = heroBody.GetComponent<Transform>(EHeroWeapon.WeaponL);
                sprite = Managers.Resource.Load<Sprite>(weapon.LeftWeapon);
                if (sprite != null)
                {
                    SpriteRenderer spr = heroBody.GetComponent<SpriteRenderer>(EHeroWeapon.WeaponL);
                    spr.sprite = sprite;
                    if (ColorUtility.TryParseHtmlString(weapon.LeftColor, out color))
                        spr.color = color;

                    // Vector3 scale = weapon.LeftScale;
                    // Vector3 position = weapon.LeftPosition;
                    // Vector3 rotation = weapon.LeftRotation;

                    // weaponL.localScale = new Vector3(scale.x, scale.y, scale.z);
                    // weaponL.localPosition = new Vector3(position.x, position.y, position.z);
                    // weaponL.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
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

                    // Vector3 scale = weapon.RightScale;
                    // Vector3 position = weapon.RightPosition;
                    // Vector3 rotation = weapon.RightRotation;

                    // weaponR.localScale = new Vector3(scale.x, scale.y, scale.z);
                    // weaponR.localPosition = new Vector3(position.x, position.y, position.z);
                    // weaponR.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
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
        #endregion

        #region Init - Monster Sprites
        private void SetSpritesSet(int dataID, MonsterBody monsterBody)
        {
            Data.MonsterData monsterData = Managers.Data.MonsterDataDict[dataID];
            EMonsterType type = Util.GetEnumFromString<EMonsterType>(monsterData.Type);
            switch (type)
            {
                case EMonsterType.Bird:
                    SetBodyType(dataID, Managers.Data.BirdSpriteDataDict[dataID], monsterBody);
                    break;

                case EMonsterType.Quadrupeds:
                    SetBodyType(dataID, Managers.Data.QuadrupedsSpriteDataDict[dataID], monsterBody);
                    break;
            }

            
        }

        private void SetBodyType(int dataID, Data.BirdSpriteData birdSpriteData, MonsterBody monsterBody)
        {
            if (ColorUtility.TryParseHtmlString(birdSpriteData.SkinColor, out Color color) == false)
                return;

            // Bird - Body
            Transform tr = null;
            SpriteRenderer spr =null;
            Sprite sprite = Managers.Resource.Load<Sprite>(birdSpriteData.Body);
            if (sprite != null)
            {
                tr = monsterBody.GetComponent<Transform>(EBirdBodyParts.Body);
                tr.transform.localPosition = birdSpriteData.BodyPosition;
                if (monsterBody.Size == EMonsterSize.Small)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Numeric.MonsterSize_Small, ReadOnly.Numeric.MonsterSize_Small, 1);
                else if (monsterBody.Size == EMonsterSize.Medium)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Numeric.MonsterSize_Medium, ReadOnly.Numeric.MonsterSize_Medium, 1);
                else if (monsterBody.Size == EMonsterSize.Large)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Numeric.MonsterSize_Large, ReadOnly.Numeric.MonsterSize_Large, 1);

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

        private void SetBodyType(int dataID, Data.QuadrupedsSpriteData quadrupedsSpriteData, MonsterBody monsterBody)
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
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Numeric.MonsterSize_Small, ReadOnly.Numeric.MonsterSize_Small, 1);
                else if (monsterBody.Size == EMonsterSize.Medium)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Numeric.MonsterSize_Medium, ReadOnly.Numeric.MonsterSize_Medium, 1);
                else if (monsterBody.Size == EMonsterSize.Large)
                    monsterBody.GetOwner<Monster>().transform.localScale = new Vector3(ReadOnly.Numeric.MonsterSize_Large, ReadOnly.Numeric.MonsterSize_Large, 1);

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
    }
}
