using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STELLAREST_F1.Data;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroBody : CreatureBody
    {
        #region Background
        private Dictionary<EHeroBody, BodyContainer[]> _heroBodyDict = new Dictionary<EHeroBody, BodyContainer[]>();
        private Dictionary<EEnvType, Sprite[]> _envHeroWeaponDict = new Dictionary<EEnvType, Sprite[]>();
        private Sprite[] _defaultHeroWeapons = new Sprite[(int)EHeroWeapons.Max];
        public Vector3[] _defaultHeroWeaponsLocalScales = new Vector3[(int)EHeroWeapons.Max];
        private Vector3 GetWeaponLocalScale(EHeroWeapons weapon)
            => _defaultHeroWeaponsLocalScales[(int)weapon] != null ? _defaultHeroWeaponsLocalScales[(int)weapon] : Vector3.zero;

        public Sprite[] Eyebrows { get; private set; } = null;
        public Color[] EyebrowsColors { get; private set; } = null;

        public Sprite[] Eyes { get; private set; } = null;
        public Color[] EyesColors { get; private set; } = null;

        public Sprite[] Mouths { get; private set; } = null;
        public Color[] MouthsColors { get; private set; } = null;


        public Hero Owner { get; private set; } = null;
        private Material _matDefaultEyes = null;

        // --- 제거 예정
        // public List<SpriteRenderer> Skin { get; } = new List<SpriteRenderer>();
        // public List<SpriteRenderer> Appearance { get; } = new List<SpriteRenderer>();
        // --- 제거 얘정

        public BodyContainer GetContainer(EHeroBody_Head head) => _heroBodyDict[EHeroBody.Head][(int)head];
        public BodyContainer GetContainer(EHeroBody_Upper upperBody) => _heroBodyDict[EHeroBody.UpperBody][(int)upperBody];
        public BodyContainer GetContainer(EHeroBody_Lower lowerBody) => _heroBodyDict[EHeroBody.LowerBody][(int)lowerBody];
        public BodyContainer GetContainer(EHeroBody_Weapon weapon) => _heroBodyDict[EHeroBody.Weapon][(int)weapon];

        private EHeroEmoji _heroEmoji = EHeroEmoji.None;
        public EHeroEmoji HeroEmoji
        {
            get => _heroEmoji;
            set
            {
                if (_heroEmoji != value)
                {
                    _heroEmoji = value;
                    switch (value)
                    {
                        case EHeroEmoji.Idle:
                            {
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.sprite = Eyebrows[(int)EHeroEmoji.Idle];
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.color = EyebrowsColors[(int)EHeroEmoji.Idle];

                                GetContainer(EHeroBody_Head.Eyes).SPR.sprite = Eyes[(int)EHeroEmoji.Idle];
                                GetContainer(EHeroBody_Head.Eyes).SPR.color = EyesColors[(int)EHeroEmoji.Idle];

                                GetContainer(EHeroBody_Head.Mouth).SPR.sprite = Mouths[(int)EHeroEmoji.Idle];
                                GetContainer(EHeroBody_Head.Mouth).SPR.color = MouthsColors[(int)EHeroEmoji.Idle];
                            }
                            break;

                        case EHeroEmoji.Move:
                            {
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.sprite = Eyebrows[(int)EHeroEmoji.Move];
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.color = EyebrowsColors[(int)EHeroEmoji.Move];

                                GetContainer(EHeroBody_Head.Eyes).SPR.sprite = Eyes[(int)EHeroEmoji.Move];
                                GetContainer(EHeroBody_Head.Eyes).SPR.color = EyesColors[(int)EHeroEmoji.Move];

                                GetContainer(EHeroBody_Head.Mouth).SPR.sprite = Mouths[(int)EHeroEmoji.Move];
                                GetContainer(EHeroBody_Head.Mouth).SPR.color = MouthsColors[(int)EHeroEmoji.Move];
                            }
                            break;

                        case EHeroEmoji.Skill_A:
                            {
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.sprite = Eyebrows[(int)EHeroEmoji.Skill_A];
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.color = EyebrowsColors[(int)EHeroEmoji.Skill_A];

                                GetContainer(EHeroBody_Head.Eyes).SPR.sprite = Eyes[(int)EHeroEmoji.Skill_A];
                                GetContainer(EHeroBody_Head.Eyes).SPR.color = EyesColors[(int)EHeroEmoji.Skill_A];

                                GetContainer(EHeroBody_Head.Mouth).SPR.sprite = Mouths[(int)EHeroEmoji.Skill_A];
                                GetContainer(EHeroBody_Head.Mouth).SPR.color = MouthsColors[(int)EHeroEmoji.Skill_A];
                            }
                            break;

                        case EHeroEmoji.Skill_B:
                            {
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.sprite = Eyebrows[(int)EHeroEmoji.Skill_B];
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.color = EyebrowsColors[(int)EHeroEmoji.Skill_B];

                                GetContainer(EHeroBody_Head.Eyes).SPR.sprite = Eyes[(int)EHeroEmoji.Skill_B];
                                GetContainer(EHeroBody_Head.Eyes).SPR.color = EyesColors[(int)EHeroEmoji.Skill_B];

                                GetContainer(EHeroBody_Head.Mouth).SPR.sprite = Mouths[(int)EHeroEmoji.Skill_B];
                                GetContainer(EHeroBody_Head.Mouth).SPR.color = MouthsColors[(int)EHeroEmoji.Skill_B];
                            }
                            break;

                        case EHeroEmoji.Skill_C:
                            {
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.sprite = Eyebrows[(int)EHeroEmoji.Skill_C];
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.color = EyebrowsColors[(int)EHeroEmoji.Skill_C];

                                GetContainer(EHeroBody_Head.Eyes).SPR.sprite = Eyes[(int)EHeroEmoji.Skill_C];
                                GetContainer(EHeroBody_Head.Eyes).SPR.color = EyesColors[(int)EHeroEmoji.Skill_C];

                                GetContainer(EHeroBody_Head.Mouth).SPR.sprite = Mouths[(int)EHeroEmoji.Skill_C];
                                GetContainer(EHeroBody_Head.Mouth).SPR.color = MouthsColors[(int)EHeroEmoji.Skill_C];
                            }
                            break;

                        case EHeroEmoji.CollectEnv:
                            {
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.sprite = Eyebrows[(int)EHeroEmoji.CollectEnv];
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.color = EyebrowsColors[(int)EHeroEmoji.CollectEnv];

                                GetContainer(EHeroBody_Head.Eyes).SPR.sprite = Eyes[(int)EHeroEmoji.CollectEnv];
                                GetContainer(EHeroBody_Head.Eyes).SPR.color = EyesColors[(int)EHeroEmoji.CollectEnv];

                                GetContainer(EHeroBody_Head.Mouth).SPR.sprite = Mouths[(int)EHeroEmoji.CollectEnv];
                                GetContainer(EHeroBody_Head.Mouth).SPR.color = MouthsColors[(int)EHeroEmoji.CollectEnv];
                            }
                            break;

                        case EHeroEmoji.Sick:
                            {
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.sprite = Eyebrows[(int)EHeroEmoji.Sick];
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.color = EyebrowsColors[(int)EHeroEmoji.Sick];

                                GetContainer(EHeroBody_Head.Eyes).SPR.sprite = Eyes[(int)EHeroEmoji.Sick];
                                GetContainer(EHeroBody_Head.Eyes).SPR.color = EyesColors[(int)EHeroEmoji.Sick];

                                GetContainer(EHeroBody_Head.Mouth).SPR.sprite = Mouths[(int)EHeroEmoji.Sick];
                                GetContainer(EHeroBody_Head.Mouth).SPR.color = MouthsColors[(int)EHeroEmoji.Sick];
                            }
                            break;

                        case EHeroEmoji.Dead:
                            {
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.sprite = Eyebrows[(int)EHeroEmoji.Dead];
                                GetContainer(EHeroBody_Head.Eyebrows).SPR.color = EyebrowsColors[(int)EHeroEmoji.Dead];

                                GetContainer(EHeroBody_Head.Eyes).SPR.sprite = Eyes[(int)EHeroEmoji.Dead];
                                GetContainer(EHeroBody_Head.Eyes).SPR.color = EyesColors[(int)EHeroEmoji.Dead];

                                GetContainer(EHeroBody_Head.Mouth).SPR.sprite = Mouths[(int)EHeroEmoji.Dead];
                                GetContainer(EHeroBody_Head.Mouth).SPR.color = MouthsColors[(int)EHeroEmoji.Dead];
                            }
                            break;
                    }
                }
            }
        }

        private void ReleaseWeapon()
        {
            // --- WeaponL
            GetContainer(EHeroBody_Weapon.WeaponL_Armor).SPR.sprite = null;
            GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child01).SPR.sprite = null;
            GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child02).SPR.sprite = null;
            GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child03).SPR.sprite = null;

            // --- WeaponR
            GetContainer(EHeroBody_Weapon.WeaponR_Armor).SPR.sprite = null;
            GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child01).SPR.sprite = null;
            GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child02).SPR.sprite = null;
            GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child03).SPR.sprite = null;
        }

        public void ChangeWeaponType(EHeroWeaponType weaponType)
        {
            ReleaseWeapon();
            switch (weaponType)
            {
                case EHeroWeaponType.Default:
                    {
                        GetContainer(EHeroBody_Weapon.WeaponL_Armor).TR.localScale = GetWeaponLocalScale(EHeroWeapons.WeaponL_Armor);
                        GetContainer(EHeroBody_Weapon.WeaponL_Armor).SPR.sprite = _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor];

                        GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child01).TR.localScale = GetWeaponLocalScale(EHeroWeapons.WeaponL_Armor_Child01);
                        GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child01).SPR.sprite = _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor_Child01];

                        GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child02).TR.localScale = GetWeaponLocalScale(EHeroWeapons.WeaponL_Armor_Child02);
                        GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child02).SPR.sprite = _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor_Child02];

                        GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child03).TR.localScale = GetWeaponLocalScale(EHeroWeapons.WeaponL_Armor_Child03);
                        GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child03).SPR.sprite = _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor_Child03];

                        GetContainer(EHeroBody_Weapon.WeaponR_Armor).TR.localScale = GetWeaponLocalScale(EHeroWeapons.WeaponR_Armor);
                        GetContainer(EHeroBody_Weapon.WeaponR_Armor).SPR.sprite = _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor];

                        GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child01).TR.localScale = GetWeaponLocalScale(EHeroWeapons.WeaponR_Armor_Child01);
                        GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child01).SPR.sprite = _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor_Child01];

                        GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child02).TR.localScale = GetWeaponLocalScale(EHeroWeapons.WeaponR_Armor_Child02);
                        GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child02).SPR.sprite = _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor_Child02];

                        GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child03).TR.localScale = GetWeaponLocalScale(EHeroWeapons.WeaponR_Armor_Child03);
                        GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child03).SPR.sprite = _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor_Child03];
                    }
                    break;

                case EHeroWeaponType.CollectTree:
                    GetContainer(EHeroBody_Weapon.WeaponR_Armor).TR.localScale = Vector3.one;
                    GetContainer(EHeroBody_Weapon.WeaponR_Armor).SPR.sprite = _envHeroWeaponDict[EEnvType.Tree][Owner.IsMaxLevel ? 1 : 0];
                    break;

                case EHeroWeaponType.CollectRock:
                    GetContainer(EHeroBody_Weapon.WeaponR_Armor).TR.localScale = Vector3.one;
                    GetContainer(EHeroBody_Weapon.WeaponR_Armor).SPR.sprite = _envHeroWeaponDict[EEnvType.Rock][Owner.IsMaxLevel ? 1 : 0];
                    break;
            }
        }

        public void ChangeSpriteSet(HeroSpriteData heroSpriteData)
        {
            // --- Release Current
            ReleaseWeapon();
            foreach (var containers in _heroBodyDict.Values)
            {
                foreach (var container in containers)
                {
                    if (container.SPR == null)
                        continue;

                    container.SPR.sprite = null;
                }
            }

            for (int i = 0; i < _defaultHeroWeapons.Length; ++i)
                _defaultHeroWeapons[i] = null;

            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                Eyebrows[i] = null;
                Eyes[i] = null;
                Mouths[i] = null;
            }

            // --- Set New
            HeroSpriteData_Skin skin = heroSpriteData.Skin;
            if (ColorUtility.TryParseHtmlString(skin.SkinColor, out Color skinColor) == false)
            {
                Debug.LogError($"{nameof(ChangeSpriteSet)}, {skin.SkinColor}");
                Debug.Break();
                return;
            }

            // --- Head
            HeroSpriteData_Head head = heroSpriteData.Head;
            foreach (var container in _heroBodyDict[EHeroBody.Head])
            {
                if (container.SPR == null)
                    continue;

                // --- Head(skin)
                if (container == GetContainer(EHeroBody_Head.Head))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.Head);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- Hair
                if (container == GetContainer(EHeroBody_Head.Hair))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(head.Hair);
                    if (ColorUtility.TryParseHtmlString(head.HairColor, out Color hairColor))
                        container.SPR.color = hairColor;
                    container.ChangeDefaultSPRColor(hairColor);
                }

                // --- Eyebrows Emoji(if it has invalid value, error)
                if (container == GetContainer(EHeroBody_Head.Eyebrows))
                {
                    for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
                    {
                        Eyebrows[i] = Managers.Resource.Load<Sprite>(head.Eyebrows[i]);
                        if (Eyebrows[i] == null)
                        {
                            Debug.LogError($"{nameof(ChangeSpriteSet)}, {head.Eyebrows[i]}");
                            Debug.Break();
                        }

                        string eyebrowsColor = head.EyebrowsColors[i];
                        if (ColorUtility.TryParseHtmlString(eyebrowsColor, out Color color))
                            EyebrowsColors[i] = color;
                        else
                        {
                            Debug.LogError($"{nameof(ChangeSpriteSet)}, {head.EyebrowsColors[i]}");
                            Debug.Break();
                        }
                    }

                    container.SPR.sprite = Eyebrows[(int)HeroEmoji];
                    container.SPR.color = EyebrowsColors[(int)HeroEmoji];
                    container.ChangeDefaultSPRColor(EyebrowsColors[(int)EHeroEmoji.Idle]);
                }

                // --- Eyes Emoji(if it has invalid value, error)
                if (container == GetContainer(EHeroBody_Head.Eyes))
                {
                    for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
                    {
                        Eyes[i] = Managers.Resource.Load<Sprite>(head.Eyes[i]);
                        if (Eyes[i] == null)
                        {
                            Debug.LogError($"{nameof(ChangeSpriteSet)}, {head.Eyes[i]}");
                            Debug.Break();
                        }

                        string eyesColor = head.EyesColors[i];
                        if (ColorUtility.TryParseHtmlString(eyesColor, out Color color))
                            EyesColors[i] = color;
                        else
                        {
                            Debug.LogError($"{nameof(ChangeSpriteSet)}, {head.EyesColors[i]}");
                            Debug.Break();
                        }
                    }

                    container.SPR.sprite = Eyes[(int)HeroEmoji];
                    container.SPR.color = EyesColors[(int)HeroEmoji];
                    container.ChangeDefaultSPRColor(EyesColors[(int)EHeroEmoji.Idle]);
                }

                // --- Mouth Emoji(if it has invalid value, error)
                if (container == GetContainer(EHeroBody_Head.Mouth))
                {
                    for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
                    {
                        Mouths[i] = Managers.Resource.Load<Sprite>(head.Mouth[i]);
                        if (Mouths[i] == null)
                        {
                            Debug.LogError($"{nameof(ChangeSpriteSet)}, {head.Mouth[i]}");
                            Debug.Break();
                        }

                        string mouthColor = head.MouthColors[i];
                        if (ColorUtility.TryParseHtmlString(mouthColor, out Color color))
                            MouthsColors[i] = color;
                        else
                        {
                            Debug.LogError($"{nameof(ChangeSpriteSet)}, {head.MouthColors[i]}");
                            Debug.Break();
                        }
                    }

                    container.SPR.sprite = Mouths[(int)HeroEmoji];
                    container.SPR.color = MouthsColors[(int)HeroEmoji];
                    container.ChangeDefaultSPRColor(MouthsColors[(int)EHeroEmoji.Idle]);
                }

                // --- Ears(skin)
                if (container == GetContainer(EHeroBody_Head.Ears))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.Ears);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- Beard
                if (container == GetContainer(EHeroBody_Head.Beard))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(head.Beard);
                    if (ColorUtility.TryParseHtmlString(head.BeardColor, out Color beardColor))
                        container.SPR.color = beardColor;
                    container.ChangeDefaultSPRColor(beardColor);
                }

                // --- Mask
                if (container == GetContainer(EHeroBody_Head.Mask))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(head.Mask);
                    if (ColorUtility.TryParseHtmlString(head.MaskColor, out Color maskColor))
                        container.SPR.color = maskColor;
                    container.ChangeDefaultSPRColor(maskColor);
                }

                // --- Glasses
                if (container == GetContainer(EHeroBody_Head.Glasses))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(head.Glasses);
                    if (ColorUtility.TryParseHtmlString(head.GlassesColor, out Color glassesColor))
                        container.SPR.color = glassesColor;
                    container.ChangeDefaultSPRColor(glassesColor);
                }

                // --- Helmet
                if (container == GetContainer(EHeroBody_Head.Helmet))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(head.Helmet);
                    if (ColorUtility.TryParseHtmlString(head.HelmetColor, out Color helmetColor))
                        container.SPR.color = helmetColor;
                    container.ChangeDefaultSPRColor(helmetColor);
                }
            }

            // --- UpperBody
            HeroSpriteData_UpperBody upperBody = heroSpriteData.UpperBody;
            foreach (var container in _heroBodyDict[EHeroBody.UpperBody])
            {
                // --- Torso(skin)
                if (container == GetContainer(EHeroBody_Upper.Torso))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.Torso);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- Torso_Armor
                if (container == GetContainer(EHeroBody_Upper.Torso_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.Torso);
                    if (ColorUtility.TryParseHtmlString(upperBody.TorsoColor, out Color torsoColor))
                        container.SPR.color = torsoColor;
                    container.ChangeDefaultSPRColor(torsoColor);
                }

                // --- Cape_Armor
                if (container == GetContainer(EHeroBody_Upper.Cape_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.Cape);
                    if (ColorUtility.TryParseHtmlString(upperBody.CapeColor, out Color capeColor))
                        container.SPR.color = capeColor;
                    container.ChangeDefaultSPRColor(capeColor);
                }

                // --- ArmL(skin)
                if (container == GetContainer(EHeroBody_Upper.ArmL))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.ArmL);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- ArmL_Armor
                if (container == GetContainer(EHeroBody_Upper.ArmL_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.ArmL);
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmLColor, out Color armLColor))
                        container.SPR.color = armLColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- ForearmL(skin)
                if (container == GetContainer(EHeroBody_Upper.ForearmL))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.ForearmL);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- ForearmL_Armor
                if (container == GetContainer(EHeroBody_Upper.ForearmL_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmL);
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmLColor, out Color forearmLColor))
                        container.SPR.color = forearmLColor;
                    container.ChangeDefaultSPRColor(forearmLColor);
                }

                // --- HandL(skin)
                if (container == GetContainer(EHeroBody_Upper.HandL))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.HandL);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- HandL_Armor
                if (container == GetContainer(EHeroBody_Upper.HandL_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.HandL);
                    if (ColorUtility.TryParseHtmlString(upperBody.HandLColor, out Color handLColor))
                        container.SPR.color = handLColor;
                    container.ChangeDefaultSPRColor(handLColor);
                }

                // --- Finger(skin)
                if (container == GetContainer(EHeroBody_Upper.Finger))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.Finger);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- Finger_Armor
                if (container == GetContainer(EHeroBody_Upper.Finger_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.Finger);
                    if (ColorUtility.TryParseHtmlString(upperBody.FingerColor, out Color fingerColor))
                        container.SPR.color = fingerColor;
                    container.ChangeDefaultSPRColor(fingerColor);
                }

                // --- ArmR(skin)
                if (container == GetContainer(EHeroBody_Upper.ArmR))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.ArmR);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- ArmR_Armor
                if (container == GetContainer(EHeroBody_Upper.ArmR_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.ArmR);
                    if (ColorUtility.TryParseHtmlString(upperBody.ArmRColor, out Color armRColor))
                        container.SPR.color = armRColor;
                    container.ChangeDefaultSPRColor(armRColor);
                }

                // --- ForearmR(skin)
                if (container == GetContainer(EHeroBody_Upper.ForearmR))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.ForearmR);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- ForearmR_Armor
                if (container == GetContainer(EHeroBody_Upper.ForearmR_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmR);
                    if (ColorUtility.TryParseHtmlString(upperBody.ForearmRColor, out Color forearmRColor))
                        container.SPR.color = forearmRColor;
                    container.ChangeDefaultSPRColor(forearmRColor);
                }

                // --- SleeveR_Armor
                if (container == GetContainer(EHeroBody_Upper.SleeveR_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.SleeveR);
                    if (ColorUtility.TryParseHtmlString(upperBody.SleeveRColor, out Color sleeveRColor))
                        container.SPR.color = sleeveRColor;
                    container.ChangeDefaultSPRColor(sleeveRColor);
                }

                // --- HandR(skin)
                if (container == GetContainer(EHeroBody_Upper.HandR))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.HandR);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- HandR_Armor
                if (container == GetContainer(EHeroBody_Upper.HandR_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(upperBody.HandR);
                    if (ColorUtility.TryParseHtmlString(upperBody.HandRColor, out Color handRColor))
                        container.SPR.color = handRColor;
                    container.ChangeDefaultSPRColor(handRColor);
                }
            }

            // --- LowerBody
            HeroSpriteData_LowerBody lowerBody = heroSpriteData.LowerBody;
            foreach (var container in _heroBodyDict[EHeroBody.LowerBody])
            {
                // --- Pelvis(skin)
                if (container == GetContainer(EHeroBody_Lower.Pelvis))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.Pelvis);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- Pelvis_Armor
                if (container == GetContainer(EHeroBody_Lower.Pelvis_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(lowerBody.Pelvis);
                    if (ColorUtility.TryParseHtmlString(lowerBody.PelvisColor, out Color pelvisColor))
                        container.SPR.color = pelvisColor;
                    container.ChangeDefaultSPRColor(pelvisColor);
                }

                // --- LegL(skin)
                if (container == GetContainer(EHeroBody_Lower.LegL))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.Leg);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- LegL_Armor
                if (container == GetContainer(EHeroBody_Lower.LegL_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(lowerBody.LegL);
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegLColor, out Color legLColor))
                        container.SPR.color = legLColor;
                    container.ChangeDefaultSPRColor(legLColor);
                }

                // --- ShinL(skin)
                if (container == GetContainer(EHeroBody_Lower.ShinL))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.Shin);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- ShinL_Armor
                if (container == GetContainer(EHeroBody_Lower.ShinL_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinL);
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinLColor, out Color shinLColor))
                        container.SPR.color = shinLColor;
                    container.ChangeDefaultSPRColor(shinLColor);
                }

                // --- LegR(skin)
                if (container == GetContainer(EHeroBody_Lower.LegR))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.Leg);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- LegR_Armor
                if (container == GetContainer(EHeroBody_Lower.LegR_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(lowerBody.LegR);
                    if (ColorUtility.TryParseHtmlString(lowerBody.LegRColor, out Color legRColor))
                        container.SPR.color = legRColor;
                    container.ChangeDefaultSPRColor(legRColor);
                }

                // --- ShinR(skin)
                if (container == GetContainer(EHeroBody_Lower.ShinR))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(skin.Shin);
                    container.SPR.color = skinColor;
                    container.ChangeDefaultSPRColor(skinColor);
                }

                // --- ShinR_Armor
                if (container == GetContainer(EHeroBody_Lower.ShinR_Armor))
                {
                    container.SPR.sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinR);
                    if (ColorUtility.TryParseHtmlString(lowerBody.ShinRColor, out Color shinRColor))
                        container.SPR.color = shinRColor;
                    container.ChangeDefaultSPRColor(shinRColor);
                }
            }

            // --- Weapon
            HeroSpriteData_Weapon weapon = heroSpriteData.Weapon;
            foreach (var container in _heroBodyDict[EHeroBody.Weapon])
            {
                // --- WeaponL_Armor
                if (container == GetContainer(EHeroBody_Weapon.WeaponL_Armor))
                {
                    if (string.IsNullOrEmpty(weapon.LWeapon) == false)
                    {
                        Sprite sprite = Managers.Resource.Load<Sprite>(weapon.LWeapon);
                        if (sprite != null)
                        {
                            container.SPR.sprite = sprite;
                            container.TR.localScale = weapon.LWeaponLocalScale;
                            _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponL_Armor] = weapon.LWeaponLocalScale;

                            container.SPR.sortingOrder = weapon.LWeaponSorting;
                            container.SPR.flipX = weapon.LWeaponFlipX;
                            container.SPR.flipY = weapon.LWeaponFlipY;
                            _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor] = sprite;

                            container.TR.gameObject.SetActive(true);
                        }
                    }
                }

                // --- WeaponL_FireSocket(설정만해놓고, WeaponL_Armor가 존재하지 않으면 active false)
                if (container == GetContainer(EHeroBody_Weapon.WeaponL_FireSocket))
                    container.TR.localPosition = weapon.LWeaponFireSocketLocalPosition;

                if (weapon.LWeaponChilds.Length != 0)
                {
                    // --- WeaponL_Armor_Child01
                    if (container == GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child01))
                    {
                        if (string.IsNullOrEmpty(weapon.LWeaponChilds[0]) == false)
                        {
                            Sprite sprite = Managers.Resource.Load<Sprite>(weapon.LWeaponChilds[0]);
                            if (sprite != null)
                            {
                                container.SPR.sprite = sprite;
                                container.TR.localPosition = weapon.LWeaponChildsLocalPositions[0];
                                container.TR.localScale = weapon.LWeaponChildsLocalScales[0];
                                _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponL_Armor_Child01] = weapon.LWeaponChildsLocalScales[0];

                                container.SPR.sortingOrder = weapon.LWeaponChildSortings[0];
                                container.SPR.flipX = weapon.LWeaponChildFlipXs[0];
                                container.SPR.flipY = weapon.LWeaponChildFlipYs[0];
                                _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor_Child01] = sprite;

                                container.TR.gameObject.SetActive(true);
                            }
                        }
                    }

                    // --- WeaponL_Armor_Child02
                    if (container == GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child02))
                    {
                        if (string.IsNullOrEmpty(weapon.LWeaponChilds[1]) == false)
                        {
                            Sprite sprite = Managers.Resource.Load<Sprite>(weapon.LWeaponChilds[1]);
                            if (sprite != null)
                            {
                                container.SPR.sprite = sprite;
                                container.TR.localPosition = weapon.LWeaponChildsLocalPositions[1];
                                container.TR.localScale = weapon.LWeaponChildsLocalScales[1];
                                _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponL_Armor_Child02] = weapon.LWeaponChildsLocalScales[1];

                                container.SPR.sortingOrder = weapon.LWeaponChildSortings[1];
                                container.SPR.flipX = weapon.LWeaponChildFlipXs[1];
                                container.SPR.flipY = weapon.LWeaponChildFlipYs[1];
                                _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor_Child02] = sprite;

                                container.TR.gameObject.SetActive(true);
                            }
                        }
                    }

                    // --- WeaponL_Armor_Child03
                    if (container == GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child03))
                    {
                        if (string.IsNullOrEmpty(weapon.LWeaponChilds[2]) == false)
                        {
                            Sprite sprite = Managers.Resource.Load<Sprite>(weapon.LWeaponChilds[2]);
                            if (sprite != null)
                            {
                                container.SPR.sprite = sprite;
                                container.TR.localPosition = weapon.LWeaponChildsLocalPositions[2];
                                container.TR.localScale = weapon.LWeaponChildsLocalScales[2];
                                _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponL_Armor_Child03] = weapon.LWeaponChildsLocalScales[2];

                                container.SPR.sortingOrder = weapon.LWeaponChildSortings[2];
                                container.SPR.flipX = weapon.LWeaponChildFlipXs[2];
                                container.SPR.flipY = weapon.LWeaponChildFlipYs[2];
                                _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor_Child03] = sprite;

                                container.TR.gameObject.SetActive(true);
                            }
                        }
                    }
                }

                // --- WeaponR_Armor
                if (container == GetContainer(EHeroBody_Weapon.WeaponR_Armor))
                {
                    if (string.IsNullOrEmpty(weapon.RWeapon) == false)
                    {
                        Sprite sprite = Managers.Resource.Load<Sprite>(weapon.RWeapon);
                        if (sprite != null)
                        {
                            container.SPR.sprite = sprite;
                            container.TR.localScale = weapon.RWeaponLocalScale;
                            _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponR_Armor] = weapon.RWeaponLocalScale;

                            container.SPR.sortingOrder = weapon.RWeaponSorting;
                            container.SPR.flipX = weapon.RWeaponFlipX;
                            container.SPR.flipY = weapon.RWeaponFlipY;
                            _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor] = sprite;

                            container.TR.gameObject.SetActive(true);
                        }
                    }
                }

                // --- WeaponR_FireSocket(설정만해놓고, WeaponR_Armor가 존재하지 않으면 active false)
                if (container == GetContainer(EHeroBody_Weapon.WeaponR_FireSocket))
                    container.TR.localPosition = weapon.RWeaponFireSocketLocalPosition;

                if (weapon.RWeaponChilds.Length != 0)
                {
                    // --- WeaponR_Armor_Child01
                    if (container == GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child01))
                    {
                        if (string.IsNullOrEmpty(weapon.RWeaponChilds[0]) == false)
                        {
                            Sprite sprite = Managers.Resource.Load<Sprite>(weapon.RWeaponChilds[0]);
                            if (sprite != null)
                            {
                                container.SPR.sprite = sprite;
                                container.TR.localPosition = weapon.RWeaponChildsLocalPositions[0];
                                container.TR.localScale = weapon.RWeaponChildsLocalScales[0];
                                _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponR_Armor_Child01] = weapon.RWeaponChildsLocalScales[0];

                                container.SPR.sortingOrder = weapon.RWeaponChildSortings[0];
                                container.SPR.flipX = weapon.RWeaponChildFlipXs[0];
                                container.SPR.flipY = weapon.RWeaponChildFlipYs[0];
                                _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor_Child01] = sprite;

                                container.TR.gameObject.SetActive(true);
                            }
                        }
                    }

                    // --- WeaponR_Armor_Child02
                    if (container == GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child02))
                    {
                        if (string.IsNullOrEmpty(weapon.RWeaponChilds[1]) == false)
                        {
                            Sprite sprite = Managers.Resource.Load<Sprite>(weapon.RWeaponChilds[1]);
                            if (sprite != null)
                            {
                                container.SPR.sprite = sprite;
                                container.TR.localPosition = weapon.RWeaponChildsLocalPositions[1];
                                container.TR.localScale = weapon.RWeaponChildsLocalScales[1];
                                _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponR_Armor_Child02] = weapon.RWeaponChildsLocalScales[1];

                                container.SPR.sortingOrder = weapon.RWeaponChildSortings[1];
                                container.SPR.flipX = weapon.RWeaponChildFlipXs[1];
                                container.SPR.flipY = weapon.RWeaponChildFlipYs[1];
                                _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor_Child02] = sprite;

                                container.TR.gameObject.SetActive(true);
                            }
                        }
                    }

                    // --- WeaponR_Armor_Child03
                    if (container == GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child03))
                    {
                        if (string.IsNullOrEmpty(weapon.RWeaponChilds[2]) == false)
                        {
                            Sprite sprite = Managers.Resource.Load<Sprite>(weapon.RWeaponChilds[2]);
                            if (sprite != null)
                            {
                                container.SPR.sprite = sprite;
                                container.TR.localPosition = weapon.RWeaponChildsLocalPositions[2];
                                container.TR.localScale = weapon.RWeaponChildsLocalScales[2];
                                _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponR_Armor_Child03] = weapon.RWeaponChildsLocalScales[2];

                                container.SPR.sortingOrder = weapon.RWeaponChildSortings[2];
                                container.SPR.flipX = weapon.RWeaponChildFlipXs[2];
                                container.SPR.flipY = weapon.RWeaponChildFlipYs[2];
                                _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor_Child03] = sprite;

                                container.TR.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }

            // --- WeaponLs On or Off
            if (GetContainer(EHeroBody_Weapon.WeaponL_Armor).SPR.sprite == null)
                GetContainer(EHeroBody_Weapon.WeaponL_Armor).TR.gameObject.SetActive(false);
            else if (GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child01).SPR.sprite == null)
                GetContainer(EHeroBody_Weapon.WeaponL_ChildsRoot).TR.gameObject.SetActive(false);
            else if (GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child02).SPR.sprite == null)
            {
                GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child02).TR.gameObject.SetActive(false);
                GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child03).TR.gameObject.SetActive(false);
            }
            else if (GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child03).SPR.sprite == null)
                GetContainer(EHeroBody_Weapon.WeaponL_Armor_Child03).TR.gameObject.SetActive(false);

            // --- WeaponRs On or Off
            if (GetContainer(EHeroBody_Weapon.WeaponR_Armor).SPR.sprite == null)
                GetContainer(EHeroBody_Weapon.WeaponR_Armor).TR.gameObject.SetActive(false);
            else if (GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child01).SPR.sprite == null)
                GetContainer(EHeroBody_Weapon.WeaponR_ChildsRoot).TR.gameObject.SetActive(false);
            else if (GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child02).SPR.sprite == null)
            {
                GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child02).TR.gameObject.SetActive(false);
                GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child03).TR.gameObject.SetActive(false);
            }
            else if (GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child03).SPR.sprite == null)
                GetContainer(EHeroBody_Weapon.WeaponR_Armor_Child03).TR.gameObject.SetActive(false);
        }

        protected override void ApplyDefaultMat_Alpha(float alphaValue)
        {
            // --- Head
            foreach (var container in _heroBodyDict[EHeroBody.Head])
            {
                if (container.SPR == null)
                    continue;

                if (container == GetContainer(EHeroBody_Head.Eyes))
                    container.SPR.color = new Color(container.SPR.color.r, container.SPR.color.g, container.SPR.color.b,
                                    alphaValue);
                else
                {
                    container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                    Color matColor = container.MatPropertyBlock.GetColor(_matDefaultColor);
                    matColor = new Color(matColor.r, matColor.g, matColor.b, alphaValue);
                    container.MatPropertyBlock.SetColor(_matDefaultColor, matColor);
                    container.SPR.SetPropertyBlock(container.MatPropertyBlock);
                }
            }

            // --- UpperBody
            foreach (var container in _heroBodyDict[EHeroBody.UpperBody])
            {
                if (container.SPR == null)
                    continue;

                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                Color matColor = container.MatPropertyBlock.GetColor(_matDefaultColor);
                matColor = new Color(matColor.r, matColor.g, matColor.b, alphaValue);
                container.MatPropertyBlock.SetColor(_matDefaultColor, matColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }

            // --- LowerBody
            foreach (var container in _heroBodyDict[EHeroBody.LowerBody])
            {
                if (container.SPR == null)
                    continue;

                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                Color matColor = container.MatPropertyBlock.GetColor(_matDefaultColor);
                matColor = new Color(matColor.r, matColor.g, matColor.b, alphaValue);
                container.MatPropertyBlock.SetColor(_matDefaultColor, matColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }

            // --- Weapon
            foreach (var container in _heroBodyDict[EHeroBody.Weapon])
            {
                if (container.SPR == null)
                    continue;

                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                Color matColor = container.MatPropertyBlock.GetColor(_matDefaultColor);
                matColor = new Color(matColor.r, matColor.g, matColor.b, alphaValue);
                container.MatPropertyBlock.SetColor(_matDefaultColor, matColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }
        }

        protected override void ApplyStrongTintMat_Color(Color desiredColor)
        {
            // --- Head
            foreach (var container in _heroBodyDict[EHeroBody.Head])
            {
                if (container.SPR == null)
                    continue;

                if (container == GetContainer(EHeroBody_Head.Eyes))
                    container.SPR.material = null;
                else
                    container.SPR.material = _matStrongTint;

                container.SPR.color = desiredColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matStrongTintColor, desiredColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }

            // --- UpperBody
            foreach (var container in _heroBodyDict[EHeroBody.UpperBody])
            {
                if (container.SPR == null)
                    continue;

                container.SPR.material = _matStrongTint;
                container.SPR.color = desiredColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matStrongTintColor, desiredColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }

            // --- LowerBody
            foreach (var container in _heroBodyDict[EHeroBody.LowerBody])
            {
                if (container.SPR == null)
                    continue;

                container.SPR.material = _matStrongTint;
                container.SPR.color = desiredColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matStrongTintColor, desiredColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }

            // --- Weapon
            foreach (var container in _heroBodyDict[EHeroBody.Weapon])
            {
                if (container.SPR == null)
                    continue;

                container.SPR.material = _matStrongTint;
                container.SPR.color = desiredColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matStrongTintColor, desiredColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }
        }

        public override void ResetMaterialsAndColors()
        {
            // --- Head
            foreach (var container in _heroBodyDict[EHeroBody.Head])
            {
                if (container.SPR == null)
                    continue;

                if (container == GetContainer(EHeroBody_Head.Eyebrows))
                {
                    container.SPR.material = _matDefault;
                    container.SPR.color = EyebrowsColors[(int)HeroEmoji];
                }
                else if (container == GetContainer(EHeroBody_Head.Eyes))
                {
                    container.SPR.material = _matDefaultEyes;
                    container.SPR.color = EyesColors[(int)HeroEmoji];
                }
                else if (container == GetContainer(EHeroBody_Head.Mouth))
                {
                    container.SPR.material = _matDefault;
                    container.SPR.color = MouthsColors[(int)HeroEmoji];
                }
                else
                {
                    container.SPR.material = _matDefault;
                    container.SPR.color = container.DefaultSPRColor;
                }

                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matDefaultColor, container.DefaultMatColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }

            // --- UpperBody
            foreach (var container in _heroBodyDict[EHeroBody.UpperBody])
            {
                if (container.SPR == null)
                    continue;

                container.SPR.material = _matDefault;
                container.SPR.color = container.DefaultSPRColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matDefaultColor, container.DefaultMatColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }

            // --- LowerBody
            foreach (var container in _heroBodyDict[EHeroBody.LowerBody])
            {
                if (container.SPR == null)
                    continue;

                container.SPR.material = _matDefault;
                container.SPR.color = container.DefaultSPRColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matDefaultColor, container.DefaultMatColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }

            // --- Weapon
            foreach (var container in _heroBodyDict[EHeroBody.Weapon])
            {
                if (container.SPR == null)
                    continue;

                container.SPR.material = _matDefault;
                container.SPR.color = container.DefaultSPRColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matDefaultColor, container.DefaultMatColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }
        }
        #endregion

        #region Core
        public override void InitialSetInfo(int dataID, BaseObject owner)
        {
            Owner = owner as Hero;
            _matDefaultEyes = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_EyesPaint);
            InitBody(Managers.Data.HeroSpriteDataDict[dataID]);
            InitEnvWeapon();
        }

        private void InitBody(HeroSpriteData heroSpriteData)
        {
            // --- Skin Color(if it has invalid value, error)
            HeroSpriteData_Skin skin = heroSpriteData.Skin;
            if (ColorUtility.TryParseHtmlString(skin.SkinColor, out Color skinColor) == false)
            {
                Debug.LogError($"{nameof(InitBody)}, {skin.SkinColor}");
                Debug.Break();
                return;
            }

            // --- Head
            HeroSpriteData_Head head = heroSpriteData.Head;
            BodyContainer[] headContainers = new BodyContainer[(int)EHeroBody_Head.Max];
            _heroBodyDict.Add(EHeroBody.Head, headContainers);

            // --- Head(skin)
            string tag = Util.GetStringFromEnum(EHeroBody_Head.Head);
            Transform tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            Sprite sprite = Managers.Resource.Load<Sprite>(skin.Head);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            headContainers[(int)EHeroBody_Head.Head] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            MaterialPropertyBlock matPB = headContainers[(int)EHeroBody_Head.Head].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Head].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Hair
            tag = Util.GetStringFromEnum(EHeroBody_Head.Hair);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(head.Hair);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(head.HairColor, out Color hairColor))
                spr.color = hairColor;

            headContainers[(int)EHeroBody_Head.Hair] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Hair].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Hair].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Eyebrows Emoji(if it has invalid value, error)
            Eyebrows = new Sprite[(int)EHeroEmoji.Max];
            EyebrowsColors = new Color[(int)EHeroEmoji.Max];
            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                Eyebrows[i] = Managers.Resource.Load<Sprite>(head.Eyebrows[i]);
                if (Eyebrows[i] == null)
                {
                    Debug.LogError($"{nameof(InitBody)}, {heroSpriteData.Head.Eyebrows[i]}");
                    Debug.Break();
                }

                string eyebrowsColor = head.EyebrowsColors[i];
                if (ColorUtility.TryParseHtmlString(eyebrowsColor, out Color color))
                    EyebrowsColors[i] = color;
                else
                {
                    Debug.LogError($"{nameof(InitBody)}, {heroSpriteData.Head.EyebrowsColors[i]}");
                    Debug.Break();
                }
            }

            tag = Util.GetStringFromEnum(EHeroBody_Head.Eyebrows);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            if (Eyebrows[(int)EHeroEmoji.Idle] != null)
                spr.sprite = Eyebrows[(int)EHeroEmoji.Idle];
            spr.color = EyebrowsColors[(int)EHeroEmoji.Idle];

            headContainers[(int)EHeroBody_Head.Eyebrows] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: EyebrowsColors[(int)EHeroEmoji.Idle],
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Eyebrows].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Eyebrows].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Eyes Emoji(if it has invalid value, error)
            Eyes = new Sprite[(int)EHeroEmoji.Max];
            EyesColors = new Color[(int)EHeroEmoji.Max];
            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                Eyes[i] = Managers.Resource.Load<Sprite>(head.Eyes[i]);
                if (Eyes[i] == null)
                {
                    Debug.LogError($"{nameof(InitBody)}, {head.Eyes[i]}");
                    Debug.Break();
                }

                string eyesColors = head.EyesColors[i];
                if (ColorUtility.TryParseHtmlString(eyesColors, out Color color))
                    EyesColors[i] = color;
                else
                {
                    Debug.LogError($"{nameof(InitBody)}, {head.EyesColors[i]}");
                    Debug.Break();
                }
            }

            tag = Util.GetStringFromEnum(EHeroBody_Head.Eyes);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefaultEyes;
            if (Eyes[(int)EHeroEmoji.Idle] != null)
                spr.sprite = Eyes[(int)EHeroEmoji.Idle];
            spr.color = EyesColors[(int)EHeroEmoji.Idle];

            headContainers[(int)EHeroBody_Head.Eyes] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefaultEyes, defaultSPRColor: EyesColors[(int)EHeroEmoji.Idle],
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Eyes].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Eyes].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Mouths Emoji(if it has invalid value, error)
            Mouths = new Sprite[(int)EHeroEmoji.Max];
            MouthsColors = new Color[(int)EHeroEmoji.Max];
            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                Mouths[i] = Managers.Resource.Load<Sprite>(head.Mouth[i]);
                if (Mouths[i] == null)
                {
                    Debug.LogError($"{nameof(InitBody)}, {heroSpriteData.Head.Mouth[i]}");
                    Debug.Break();
                }

                string mouthColor = head.MouthColors[i];
                if (ColorUtility.TryParseHtmlString(mouthColor, out Color color))
                    MouthsColors[i] = color;
                else
                {
                    Debug.LogError($"{nameof(InitBody)}, {head.MouthColors[i]}");
                    Debug.Break();
                }
            }

            tag = Util.GetStringFromEnum(EHeroBody_Head.Mouth);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            if (Mouths[(int)EHeroEmoji.Idle] != null)
                spr.sprite = Mouths[(int)EHeroEmoji.Idle];

            spr.color = MouthsColors[(int)EHeroEmoji.Idle];

            headContainers[(int)EHeroBody_Head.Mouth] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: MouthsColors[(int)EHeroEmoji.Idle],
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Mouth].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Mouth].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Ears(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Head.Ears);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.Ears);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            headContainers[(int)EHeroBody_Head.Ears] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Ears].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Ears].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Earrings
            tag = Util.GetStringFromEnum(EHeroBody_Head.Earrings);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(head.Earrings);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(head.EarringsColor, out Color earringsColor))
                spr.color = earringsColor;

            headContainers[(int)EHeroBody_Head.Earrings] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Earrings].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Earrings].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Beard
            tag = Util.GetStringFromEnum(EHeroBody_Head.Beard);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(head.Beard);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(head.BeardColor, out Color beardColor))
                spr.color = beardColor;

            headContainers[(int)EHeroBody_Head.Beard] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Beard].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Beard].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Mask
            tag = Util.GetStringFromEnum(EHeroBody_Head.Mask);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(head.Mask);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(head.MaskColor, out Color maskColor))
                spr.color = maskColor;

            headContainers[(int)EHeroBody_Head.Mask] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Mask].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Mask].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Glasses
            tag = Util.GetStringFromEnum(EHeroBody_Head.Glasses);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(head.Glasses);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(head.GlassesColor, out Color glassesColor))
                spr.color = glassesColor;

            headContainers[(int)EHeroBody_Head.Glasses] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Glasses].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Glasses].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Helmet
            tag = Util.GetStringFromEnum(EHeroBody_Head.Helmet);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(head.Helmet);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(head.HelmetColor, out Color helmetColor))
                spr.color = helmetColor;

            headContainers[(int)EHeroBody_Head.Helmet] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = headContainers[(int)EHeroBody_Head.Helmet].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, headContainers[(int)EHeroBody_Head.Helmet].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- UpperBody
            HeroSpriteData_UpperBody upperBody = heroSpriteData.UpperBody;
            BodyContainer[] upperBodyContainers = new BodyContainer[(int)EHeroBody_Upper.Max];
            _heroBodyDict.Add(EHeroBody.UpperBody, upperBodyContainers);

            // --- Torso(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Upper.Torso);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.Torso);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            upperBodyContainers[(int)EHeroBody_Upper.Torso] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.Torso].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.Torso].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Torso_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.Torso_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.Torso);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.TorsoColor, out Color torsoColor))
                spr.color = torsoColor;

            upperBodyContainers[(int)EHeroBody_Upper.Torso_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.Torso_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.Torso_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Cape_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.Cape_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.Cape);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.CapeColor, out Color capeColor))
                spr.color = capeColor;

            upperBodyContainers[(int)EHeroBody_Upper.Cape_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.Cape_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.Cape_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ArmL(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Upper.ArmL);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.ArmL);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            upperBodyContainers[(int)EHeroBody_Upper.ArmL] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.ArmL].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.ArmL].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ArmL_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.ArmL_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.ArmL);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.ArmLColor, out Color armLColor))
                spr.color = armLColor;

            upperBodyContainers[(int)EHeroBody_Upper.ArmL_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.ArmL_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.ArmL_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ForearmL(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Upper.ForearmL);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.ForearmL);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            upperBodyContainers[(int)EHeroBody_Upper.ForearmL] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.ForearmL].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.ForearmL].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ForearmL_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.ForearmL_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmL);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.ForearmLColor, out Color forearmLColor))
                spr.color = forearmLColor;

            upperBodyContainers[(int)EHeroBody_Upper.ForearmL_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.ForearmL_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.ForearmL_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- HandL(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Upper.HandL);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.HandL);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            upperBodyContainers[(int)EHeroBody_Upper.HandL] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.HandL].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.HandL].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- HandL_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.HandL_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.HandL);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.HandLColor, out Color handLColor))
                spr.color = handLColor;

            upperBodyContainers[(int)EHeroBody_Upper.HandL_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.HandL_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.HandL_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Finger(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Upper.Finger);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.Finger);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            upperBodyContainers[(int)EHeroBody_Upper.Finger] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.Finger].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.Finger].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Finger_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.Finger_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.Finger);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.FingerColor, out Color fingerColor))
                spr.color = fingerColor;

            upperBodyContainers[(int)EHeroBody_Upper.Finger_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.Finger_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.Finger_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ArmR(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Upper.ArmR);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.ArmR);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            upperBodyContainers[(int)EHeroBody_Upper.ArmR] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.ArmR].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.ArmR].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ArmR_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.ArmR_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.ArmR);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.ArmRColor, out Color armRColor))
                spr.color = armRColor;

            upperBodyContainers[(int)EHeroBody_Upper.ArmR_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.ArmR_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.ArmR_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ForearmR(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Upper.ForearmR);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.ForearmR);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            upperBodyContainers[(int)EHeroBody_Upper.ForearmR] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.ForearmR].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.ForearmR].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ForearmR_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.ForearmR_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.ForearmR);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.ForearmRColor, out Color ForearmRColor))
                spr.color = ForearmRColor;

            upperBodyContainers[(int)EHeroBody_Upper.ForearmR_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.ForearmR_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.ForearmR_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- SleeveR_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.SleeveR_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.SleeveR);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.SleeveRColor, out Color sleeveRColor))
                spr.color = sleeveRColor;

            upperBodyContainers[(int)EHeroBody_Upper.SleeveR_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.SleeveR_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.SleeveR_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- HandR(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Upper.HandR);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.HandR);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            upperBodyContainers[(int)EHeroBody_Upper.HandR] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.HandR].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.HandR].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- HandR_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Upper.HandR_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(upperBody.HandR);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(upperBody.HandRColor, out Color handRColor))
                spr.color = handRColor;

            upperBodyContainers[(int)EHeroBody_Upper.HandR_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = upperBodyContainers[(int)EHeroBody_Upper.HandR_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, upperBodyContainers[(int)EHeroBody_Upper.HandR_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- LowerBody
            HeroSpriteData_LowerBody lowerBody = heroSpriteData.LowerBody;
            BodyContainer[] lowerBodyContainers = new BodyContainer[(int)EHeroBody_Lower.Max];
            _heroBodyDict.Add(EHeroBody.LowerBody, lowerBodyContainers);

            // --- Pelvis(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Lower.Pelvis);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.Pelvis);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            lowerBodyContainers[(int)EHeroBody_Lower.Pelvis] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.Pelvis].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.Pelvis].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Pelvis_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Lower.Pelvis_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(lowerBody.Pelvis);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(lowerBody.PelvisColor, out Color pelvisColor))
                spr.color = pelvisColor;

            lowerBodyContainers[(int)EHeroBody_Lower.Pelvis_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.Pelvis_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.Pelvis_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- LegL(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Lower.LegL);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.Leg);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            lowerBodyContainers[(int)EHeroBody_Lower.LegL] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.LegL].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.LegL].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- LegL_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Lower.LegL_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(lowerBody.LegL);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(lowerBody.LegLColor, out Color legLColor))
                spr.color = legLColor;

            lowerBodyContainers[(int)EHeroBody_Lower.LegL_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.LegL_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.LegL_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ShinL(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Lower.ShinL);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.Shin);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            lowerBodyContainers[(int)EHeroBody_Lower.ShinL] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.ShinL].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.ShinL].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ShinL_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Lower.ShinL_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinL);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(lowerBody.ShinLColor, out Color shinLColor))
                spr.color = shinLColor;

            lowerBodyContainers[(int)EHeroBody_Lower.ShinL_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.ShinL_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.ShinL_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- LegR(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Lower.LegR);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.Leg);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            lowerBodyContainers[(int)EHeroBody_Lower.LegR] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.LegR].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.LegR].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- LegR_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Lower.LegR_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(lowerBody.LegR);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(lowerBody.LegRColor, out Color legRColor))
                spr.color = legRColor;

            lowerBodyContainers[(int)EHeroBody_Lower.LegR_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.LegR_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.LegR_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ShinR(skin)
            tag = Util.GetStringFromEnum(EHeroBody_Lower.ShinR);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(skin.Shin);
            if (sprite != null)
                spr.sprite = sprite;
            spr.color = skinColor;

            lowerBodyContainers[(int)EHeroBody_Lower.ShinR] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: skinColor,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.ShinR].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.ShinR].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- ShinR_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Lower.ShinR_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(lowerBody.ShinR);
            if (sprite != null)
                spr.sprite = sprite;

            if (ColorUtility.TryParseHtmlString(lowerBody.ShinR, out Color shinRColor))
                spr.color = shinRColor;

            lowerBodyContainers[(int)EHeroBody_Lower.ShinR_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = lowerBodyContainers[(int)EHeroBody_Lower.ShinR_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, lowerBodyContainers[(int)EHeroBody_Lower.ShinR_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- Weapon
            HeroSpriteData_Weapon weapon = heroSpriteData.Weapon;
            BodyContainer[] weaponContainers = new BodyContainer[(int)EHeroBody_Weapon.Max];
            _heroBodyDict.Add(EHeroBody.Weapon, weaponContainers);

            for (int i = 0; i < _defaultHeroWeapons.Length; ++i)
                _defaultHeroWeapons[i] = null;

            // --- WeaponL_Armor
            tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponL_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(weapon.LWeapon);
            if (sprite != null)
            {
                spr.sprite = sprite;
                tr.localScale = weapon.LWeaponLocalScale;
                _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponL_Armor] = weapon.LWeaponLocalScale;

                spr.sortingOrder = weapon.LWeaponSorting;
                spr.flipX = weapon.LWeaponFlipX;
                spr.flipY = weapon.LWeaponFlipY;
                _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor] = sprite;
            }

            weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- WeaponL_FireSocket
            tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponL_FireSocket);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            tr.localPosition = weapon.LWeaponFireSocketLocalPosition;

            weaponContainers[(int)EHeroBody_Weapon.WeaponL_FireSocket] = new BodyContainer(tag: tag, tr: tr, spr: null,
                                                            defaultSPRMat: null, defaultSPRColor: Color.white,
                                                            defaultMatColor: Color.white, matPB: null);

            // --- WeaponL_ChildsRoot
            tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponL_ChildsRoot);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);

            weaponContainers[(int)EHeroBody_Weapon.WeaponL_ChildsRoot] = new BodyContainer(tag: tag, tr: tr, spr: null,
                                                            defaultSPRMat: null, defaultSPRColor: Color.white,
                                                            defaultMatColor: Color.white, matPB: null);

            // --- WeaponL_Armor_Child01, Child02, Child03
            tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponL_Armor_Child01);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            if (weapon.LWeaponChilds.Length != 0)
            {
                for (int i = 0; i < weapon.LWeaponChilds.Length; ++i)
                {
                    if (i == 0)
                    {
                        sprite = Managers.Resource.Load<Sprite>(weapon.LWeaponChilds[0]);
                        spr.sprite = sprite;
                        tr.localPosition = weapon.LWeaponChildsLocalPositions[0];
                        tr.localScale = weapon.LWeaponChildsLocalScales[0];
                        _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponL_Armor_Child01] = weapon.LWeaponChildsLocalScales[0];

                        spr.sortingOrder = weapon.LWeaponChildSortings[0];
                        spr.flipX = weapon.LWeaponChildFlipXs[0];
                        spr.flipY = weapon.LWeaponChildFlipYs[0];
                        _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor_Child01] = sprite;

                        weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child01] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child01].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child01].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);
                    }
                    else if (i == 1)
                    {
                        tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponL_Armor_Child02);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(weapon.LWeaponChilds[1]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            tr.localPosition = weapon.LWeaponChildsLocalPositions[1];
                            tr.localScale = weapon.LWeaponChildsLocalScales[1];
                            _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponL_Armor_Child02] = weapon.LWeaponChildsLocalScales[1];

                            spr.sortingOrder = weapon.LWeaponChildSortings[1];
                            spr.flipX = weapon.LWeaponChildFlipXs[1];
                            spr.flipY = weapon.LWeaponChildFlipYs[1];
                            _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor_Child02] = sprite;
                        }

                        weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child02] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child02].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child02].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        if (sprite == null)
                            tr.gameObject.SetActive(false);
                    }
                    else if (i == 2)
                    {
                        tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponL_Armor_Child03);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(weapon.LWeaponChilds[2]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            tr.localPosition = weapon.LWeaponChildsLocalPositions[2];
                            tr.localScale = weapon.LWeaponChildsLocalScales[2];
                            _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponL_Armor_Child03] = weapon.LWeaponChildsLocalScales[2];

                            spr.sortingOrder = weapon.LWeaponChildSortings[2];
                            spr.flipX = weapon.LWeaponChildFlipXs[2];
                            spr.flipY = weapon.LWeaponChildFlipYs[2];
                            _defaultHeroWeapons[(int)EHeroWeapons.WeaponL_Armor_Child03] = sprite;
                        }

                        weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child03] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child03].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child03].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        if (sprite == null)
                            tr.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child01] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child01].MatPropertyBlock;
                spr.GetPropertyBlock(matPB);
                matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child01].DefaultMatColor);
                spr.SetPropertyBlock(matPB);

                tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponL_Armor_Child02);
                tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.material = _matDefault;

                weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child02] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child02].MatPropertyBlock;
                spr.GetPropertyBlock(matPB);
                matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child02].DefaultMatColor);
                spr.SetPropertyBlock(matPB);

                tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponL_Armor_Child03);
                tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.material = _matDefault;

                weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child03] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child03].MatPropertyBlock;
                spr.GetPropertyBlock(matPB);
                matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponL_Armor_Child03].DefaultMatColor);
                spr.SetPropertyBlock(matPB);

                GetContainer(EHeroBody_Weapon.WeaponL_ChildsRoot).TR.gameObject.SetActive(false);
            }

            if (GetContainer(EHeroBody_Weapon.WeaponL_Armor).SPR.sprite == null)
                GetContainer(EHeroBody_Weapon.WeaponL_Armor).TR.gameObject.SetActive(false);

            // --- WeaponR
            tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponR_Armor);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            sprite = Managers.Resource.Load<Sprite>(weapon.RWeapon);
            if (sprite != null)
            {
                spr.sprite = sprite;
                tr.localScale = weapon.RWeaponLocalScale;
                _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponR_Armor] = weapon.RWeaponLocalScale;

                spr.sortingOrder = weapon.RWeaponSorting;
                spr.flipX = weapon.RWeaponFlipX;
                spr.flipY = weapon.RWeaponFlipY;
                _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor] = sprite;
            }

            weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

            matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor].MatPropertyBlock;
            spr.GetPropertyBlock(matPB);
            matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor].DefaultMatColor);
            spr.SetPropertyBlock(matPB);

            // --- WeaponR_FireSocket
            tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponR_FireSocket);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            tr.localPosition = weapon.RWeaponFireSocketLocalPosition;

            weaponContainers[(int)EHeroBody_Weapon.WeaponR_FireSocket] = new BodyContainer(tag: tag, tr: tr, spr: null,
                                                            defaultSPRMat: null, defaultSPRColor: Color.white,
                                                            defaultMatColor: Color.white, matPB: null);

            // --- WeaponR_ChildsRoot
            tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponR_ChildsRoot);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);

            weaponContainers[(int)EHeroBody_Weapon.WeaponR_ChildsRoot] = new BodyContainer(tag: tag, tr: tr, spr: null,
                                                            defaultSPRMat: null, defaultSPRColor: Color.white,
                                                            defaultMatColor: Color.white, matPB: null);

            // --- WeaponR_Armor_Child01, Child02, Child03
            tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponR_Armor_Child01);
            tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
            spr = tr.GetComponent<SpriteRenderer>();
            spr.material = _matDefault;
            if (weapon.RWeaponChilds.Length != 0)
            {
                for (int i = 0; i < weapon.RWeaponChilds.Length; ++i)
                {
                    if (i == 0)
                    {
                        sprite = Managers.Resource.Load<Sprite>(weapon.RWeaponChilds[0]);
                        spr.sprite = sprite;
                        tr.localPosition = weapon.RWeaponChildsLocalPositions[0];
                        tr.localScale = weapon.RWeaponChildsLocalScales[0];
                        _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponR_Armor_Child01] = weapon.RWeaponChildsLocalScales[0];

                        spr.sortingOrder = weapon.RWeaponChildSortings[0];
                        spr.flipX = weapon.RWeaponChildFlipXs[0];
                        spr.flipY = weapon.RWeaponChildFlipYs[0];
                        _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor_Child01] = sprite;

                        weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child01] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child01].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child01].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);
                    }
                    else if (i == 1)
                    {
                        tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponR_Armor_Child02);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(weapon.RWeaponChilds[1]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            tr.localPosition = weapon.RWeaponChildsLocalPositions[1];
                            tr.localScale = weapon.RWeaponChildsLocalScales[1];
                            _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponR_Armor_Child02] = weapon.RWeaponChildsLocalScales[1];

                            spr.sortingOrder = weapon.RWeaponChildSortings[1];
                            spr.flipX = weapon.RWeaponChildFlipXs[1];
                            spr.flipY = weapon.RWeaponChildFlipYs[1];
                            _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor_Child02] = sprite;
                        }

                        weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child02] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child02].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child02].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        if (sprite == null)
                            tr.gameObject.SetActive(false);
                    }
                    else if (i == 2)
                    {
                        tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponR_Armor_Child03);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(weapon.RWeaponChilds[2]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            tr.localPosition = weapon.RWeaponChildsLocalPositions[2];
                            tr.localScale = weapon.RWeaponChildsLocalScales[2];
                            _defaultHeroWeaponsLocalScales[(int)EHeroWeapons.WeaponR_Armor_Child03] = weapon.RWeaponChildsLocalScales[2];

                            spr.sortingOrder = weapon.RWeaponChildSortings[2];
                            spr.flipX = weapon.RWeaponChildFlipXs[2];
                            spr.flipY = weapon.RWeaponChildFlipYs[2];
                            _defaultHeroWeapons[(int)EHeroWeapons.WeaponR_Armor_Child03] = sprite;
                        }

                        weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child03] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child03].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child03].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        if (sprite == null)
                            tr.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child01] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child01].MatPropertyBlock;
                spr.GetPropertyBlock(matPB);
                matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child01].DefaultMatColor);
                spr.SetPropertyBlock(matPB);

                tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponR_Armor_Child02);
                tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.material = _matDefault;

                weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child02] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child02].MatPropertyBlock;
                spr.GetPropertyBlock(matPB);
                matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child02].DefaultMatColor);
                spr.SetPropertyBlock(matPB);

                tag = Util.GetStringFromEnum(EHeroBody_Weapon.WeaponR_Armor_Child03);
                tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                spr = tr.GetComponent<SpriteRenderer>();
                spr.material = _matDefault;

                weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child03] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                            defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                            defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                matPB = weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child03].MatPropertyBlock;
                spr.GetPropertyBlock(matPB);
                matPB.SetColor(_matDefaultColor, weaponContainers[(int)EHeroBody_Weapon.WeaponR_Armor_Child03].DefaultMatColor);
                spr.SetPropertyBlock(matPB);

                GetContainer(EHeroBody_Weapon.WeaponR_ChildsRoot).TR.gameObject.SetActive(false);
            }

            if (GetContainer(EHeroBody_Weapon.WeaponR_Armor).SPR.sprite == null)
                GetContainer(EHeroBody_Weapon.WeaponR_Armor).TR.gameObject.SetActive(false);
        }

        private void InitEnvWeapon()
        {
            Sprite[] envWeapons = new Sprite[(int)EEnvType.Max];
            envWeapons[(int)EHeroGrade.Default] = Managers.Resource.Load<Sprite>(ReadOnly.Util.WoodcutterAxe_Default_SP);
            envWeapons[(int)EHeroGrade.Max] = Managers.Resource.Load<Sprite>(ReadOnly.Util.WoodcutterAxe_Max_SP);
            _envHeroWeaponDict.Add(EEnvType.Tree, envWeapons);

            envWeapons = new Sprite[(int)EEnvType.Max];
            envWeapons[(int)EHeroGrade.Default] = Managers.Resource.Load<Sprite>(ReadOnly.Util.Pickaxe_Default_SP);
            envWeapons[(int)EHeroGrade.Max] = Managers.Resource.Load<Sprite>(ReadOnly.Util.Pickaxe_Max_SP);
            _envHeroWeaponDict.Add(EEnvType.Rock, envWeapons);
        }
        #endregion
    }
}
