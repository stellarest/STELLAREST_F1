using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STELLAREST_F1.Data;
using static STELLAREST_F1.Define;
using Unity.VisualScripting.Dependencies.Sqlite;

namespace STELLAREST_F1
{
    public class HeroBody : CreatureBody
    {
        #region Background
        private Dictionary<EHeroBody, BodyContainer[]> _heroBodyDict = new Dictionary<EHeroBody, BodyContainer[]>();

        public Sprite[] Eyebrows { get; private set; } = null;
        public Color[] EyebrowsColors { get; private set; } = null;

        public Sprite[] Eyes { get; private set; } = null;
        public Color[] EyesColors { get; private set; } = null;

        public Sprite[] Mouths { get; private set; } = null;
        public Color[] MouthsColors { get; private set; } = null;

        public Hero Owner { get; private set; } = null;
        private Material _matDefaultEyes = null;

        // --- 제거 예정
        public List<SpriteRenderer> Skin { get; } = new List<SpriteRenderer>();
        public List<SpriteRenderer> Appearance { get; } = new List<SpriteRenderer>();
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

        private void ApplyHeroStrongTint(Color desiredColor)
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

        public void ResetHeroMaterialsAndColors()
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
        public override bool SetInfo(BaseObject owner, int dataID)
        {
            Owner = owner as Hero;
            if (Owner == null)
                return false;

            _matDefaultEyes = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_EyesPaint);
            InitBody(Managers.Data.HeroSpriteDataDict[dataID]);
            return true;
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

            if (ColorUtility.TryParseHtmlString(heroSpriteData.Head.HairColor, out Color hairColor))
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
                Eyebrows[i] = Managers.Resource.Load<Sprite>(heroSpriteData.Head.Eyebrows[i]);
                if (Eyebrows[i] == null)
                {
                    Debug.LogError($"{nameof(InitBody)}, {heroSpriteData.Head.Eyebrows[i]}");
                    Debug.Break();
                }

                string eyebrowsColor = heroSpriteData.Head.EyebrowsColors[i];
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
                Eyes[i] = Managers.Resource.Load<Sprite>(heroSpriteData.Head.Eyes[i]);
                if (Eyes[i] == null)
                {
                    Debug.LogError($"{nameof(InitBody)}, {heroSpriteData.Head.Eyes[i]}");
                    Debug.Break();
                }

                string eyesColors = heroSpriteData.Head.EyesColors[i];
                if (ColorUtility.TryParseHtmlString(eyesColors, out Color color))
                    EyesColors[i] = color;
                else
                {
                    Debug.LogError($"{nameof(InitBody)}, {heroSpriteData.Head.EyesColors[i]}");
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
                                                defaultSPRMat: _matDefault, defaultSPRColor: EyesColors[(int)EHeroEmoji.Idle],
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
                Mouths[i] = Managers.Resource.Load<Sprite>(heroSpriteData.Head.Mouth[i]);
                if (Mouths[i] == null)
                {
                    Debug.LogError($"{nameof(InitBody)}, {heroSpriteData.Head.Mouth[i]}");
                    Debug.Break();
                }

                string mouthColor = heroSpriteData.Head.MouthColors[i];
                if (ColorUtility.TryParseHtmlString(mouthColor, out Color color))
                    MouthsColors[i] = color;
                else
                {
                    Debug.LogError($"{nameof(InitBody)}, {heroSpriteData.Head.MouthColors[i]}");
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
                spr.sortingOrder = weapon.LWeaponSorting;
                spr.flipX = weapon.LWeaponFlipX;
                spr.flipY = weapon.LWeaponFlipY;
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
                        spr.sortingOrder = weapon.LWeaponChildSortings[0];
                        spr.flipX = weapon.LWeaponChildFlipXs[0];
                        spr.flipY = weapon.LWeaponChildFlipYs[0];

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
                            spr.sortingOrder = weapon.LWeaponChildSortings[1];
                            spr.flipX = weapon.LWeaponChildFlipXs[1];
                            spr.flipY = weapon.LWeaponChildFlipYs[1];
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
                            spr.sortingOrder = weapon.LWeaponChildSortings[2];
                            spr.flipX = weapon.LWeaponChildFlipXs[2];
                            spr.flipY = weapon.LWeaponChildFlipYs[2];
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
                spr.sortingOrder = weapon.RWeaponSorting;
                spr.flipX = weapon.RWeaponFlipX;
                spr.flipY = weapon.RWeaponFlipY;
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
                        spr.sortingOrder = weapon.RWeaponChildSortings[0];
                        spr.flipX = weapon.RWeaponChildFlipXs[0];
                        spr.flipY = weapon.RWeaponChildFlipYs[0];

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
                            spr.sortingOrder = weapon.RWeaponChildSortings[1];
                            spr.flipX = weapon.RWeaponChildFlipXs[1];
                            spr.flipY = weapon.RWeaponChildFlipYs[1];
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
                            spr.sortingOrder = weapon.RWeaponChildSortings[2];
                            spr.flipX = weapon.RWeaponChildFlipXs[2];
                            spr.flipY = weapon.RWeaponChildFlipYs[2];
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
        #endregion

        #region Coroutines
        protected override IEnumerator CoHurtFlashEffect()
        {
            ApplyHeroStrongTint(Color.white);
            yield return new WaitForSeconds(0.1f);
            ResetHeroMaterialsAndColors();
        }
        #endregion

        public Transform LeftWeapon { get; private set; } = null;
        public Vector3 LeftWeaponLocalScale { get; private set; } = Vector3.zero;
        public void SetLeftWeapon(Transform leftWeapon, Vector3 leftWeaponLocalScale)
        {
            LeftWeapon = leftWeapon;
            LeftWeaponLocalScale = leftWeaponLocalScale;
        }

        public Transform RightWeapon { get; private set; } = null;
        public Vector3 RightWeaponLocalScale { get; private set; } = Vector3.zero;
        public void SetRightWeapon(Transform rightWeapon, Vector3 rightWeaponLocalScale)
        {
            RightWeapon = rightWeapon;
            RightWeaponLocalScale = rightWeaponLocalScale;
        }

        private SpriteRenderer[] _leftWeaponSPRs = null;
        private Sprite[] _defaultLeftWeaponSPs = null;

        private SpriteRenderer[] _rightWeaponSPRs = null;
        private Sprite[] _defaultRightWeaponSPs = null;

        public void SetDefaultWeaponSprites(SpriteRenderer[] leftWeaponSPRs, SpriteRenderer[] rightWeaponSPRs)
        {
            _leftWeaponSPRs = leftWeaponSPRs;
            _defaultLeftWeaponSPs = new Sprite[leftWeaponSPRs.Length];
            for (int i = 0; i < leftWeaponSPRs.Length; ++i)
                _defaultLeftWeaponSPs[i] = leftWeaponSPRs[i].sprite;

            _rightWeaponSPRs = rightWeaponSPRs;
            _defaultRightWeaponSPs = new Sprite[rightWeaponSPRs.Length];
            for (int i = 0; i < rightWeaponSPRs.Length; ++i)
                _defaultRightWeaponSPs[i] = rightWeaponSPRs[i].sprite;
        }

        public void ChangeDefaultWeaponSprites(SpriteRenderer[] leftWeaponSPRs, SpriteRenderer[] rightWeaponSPRs)
        {
            for (int i = 0; i < _leftWeaponSPRs.Length; ++i)
                _leftWeaponSPRs[i] = null;
            for (int i = 0; i < _defaultLeftWeaponSPs.Length; ++i)
                _defaultLeftWeaponSPs[i] = null;

            for (int i = 0; i < _rightWeaponSPRs.Length; ++i)
                _rightWeaponSPRs[i] = null;
            for (int i = 0; i < _defaultRightWeaponSPs.Length; ++i)
                _defaultRightWeaponSPs[i] = null;

            _leftWeaponSPRs = leftWeaponSPRs;
            _defaultLeftWeaponSPs = new Sprite[leftWeaponSPRs.Length];
            for (int i = 0; i < leftWeaponSPRs.Length; ++i)
                _defaultLeftWeaponSPs[i] = leftWeaponSPRs[i].sprite;

            _rightWeaponSPRs = rightWeaponSPRs;
            _defaultRightWeaponSPs = new Sprite[rightWeaponSPRs.Length];
            for (int i = 0; i < rightWeaponSPRs.Length; ++i)
                _defaultRightWeaponSPs[i] = rightWeaponSPRs[i].sprite;
        }

        // public void DefaultWeapon()
        // {
        //     //ReleaseWeapon();
        //     // for (int i = 0; i < _leftWeaponSPRs.Length; ++i)
        //     //     _leftWeaponSPRs[i].sprite = _defaultLeftWeaponSPs[i];

        //     // for (int i = 0; i < _rightWeaponSPRs.Length; ++i)
        //     //     _rightWeaponSPRs[i].sprite = _defaultRightWeaponSPs[i];
        // }

        private void ReleaseWeapon()
        {
            // for (int i = 0; i < _leftWeaponSPRs.Length; ++i)
            //     _leftWeaponSPRs[i].sprite = null;

            // for (int i = 0; i < _rightWeaponSPRs.Length; ++i)
            //     _rightWeaponSPRs[i].sprite = null;
        }

        public void HeroStateWeaponType(EHeroStateWeaponType heroStateWeaponType)
        {
            ReleaseWeapon();
            switch (heroStateWeaponType)
            {
                case EHeroStateWeaponType.Default:
                    {
                        if (LeftWeapon != null)
                            LeftWeapon.localScale = LeftWeaponLocalScale;
                        for (int i = 0; i < _leftWeaponSPRs.Length; ++i)
                            _leftWeaponSPRs[i].sprite = _defaultLeftWeaponSPs[i];

                        if (RightWeapon != null)
                            RightWeapon.localScale = RightWeaponLocalScale;
                        for (int i = 0; i < _rightWeaponSPRs.Length; ++i)
                            _rightWeaponSPRs[i].sprite = _defaultRightWeaponSPs[i];
                    }
                    break;

                case EHeroStateWeaponType.EnvTree:
                    RightWeapon.localScale = Vector3.one;
                    if (Owner.CreatureRarity == ECreatureRarity.Common)
                        _rightWeaponSPRs[0].sprite = Managers.Sprite.HeroCollectEnvWeaponSpritesDict[EEnvType.Tree][(int)ECollectEnvRarity.Common];
                    else
                        _rightWeaponSPRs[0].sprite = Managers.Sprite.HeroCollectEnvWeaponSpritesDict[EEnvType.Tree][(int)ECollectEnvRarity.Elite];
                    break;

                case EHeroStateWeaponType.EnvRock:
                    RightWeapon.localScale = Vector3.one;
                    if (Owner.CreatureRarity == ECreatureRarity.Common)
                        _rightWeaponSPRs[0].sprite = Managers.Sprite.HeroCollectEnvWeaponSpritesDict[EEnvType.Rock][(int)ECollectEnvRarity.Common];
                    else
                        _rightWeaponSPRs[0].sprite = Managers.Sprite.HeroCollectEnvWeaponSpritesDict[EEnvType.Rock][(int)ECollectEnvRarity.Elite];
                    break;
            }
        }
    }
}
