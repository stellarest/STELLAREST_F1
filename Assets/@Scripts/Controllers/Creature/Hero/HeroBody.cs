using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using STELLAREST_F1.Data;
using Unity.Properties;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    [System.Serializable]
    public class HeroFace
    {
        public HeroFace(HeroBody heroBody)
        {
            // Cache SPRs
            _eyebrowsSPR = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows);
            _eyesSPR = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes);
            _mouthSPR = heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth);

            Data.HeroSpriteData heroSpriteData = Managers.Data.HeroSpriteDataDict[heroBody.DataTemplateID];

            // Eyebrows
            _eyebrowsSprites = new Sprite[(int)EHeroEmoji.Max];
            _eyebrowsColors = new Color[(int)EHeroEmoji.Max];
            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                _eyebrowsSprites[i] = Managers.Resource.Load<Sprite>(heroSpriteData.Head.Eyebrows[i]);
                string eyebrowsColor = heroSpriteData.Head.EyebrowsColors[i];
                if (ColorUtility.TryParseHtmlString(eyebrowsColor, out Color color))
                    _eyebrowsColors[i] = color;
            }

            // Eyes
            _eyesSprites = new Sprite[(int)EHeroEmoji.Max];
            _eyesColors = new Color[(int)EHeroEmoji.Max];
            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                _eyesSprites[i] = Managers.Resource.Load<Sprite>(heroSpriteData.Head.Eyes[i]);
                string eyesColor = heroSpriteData.Head.EyesColors[i];
                if (ColorUtility.TryParseHtmlString(eyesColor, out Color color))
                    _eyesColors[i] = color;
            }

            // Mouth
            _mouthSprites = new Sprite[(int)EHeroEmoji.Max];
            _mouthColors = new Color[(int)EHeroEmoji.Max];
            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                _mouthSprites[i] = Managers.Resource.Load<Sprite>(heroSpriteData.Head.Mouth[i]);
                string mouthColor = heroSpriteData.Head.MouthColors[i];
                if (ColorUtility.TryParseHtmlString(mouthColor, out Color color))
                    _mouthColors[i] = color;
            }
        }

        public void ChangeHeroFaceSet(HeroSpriteData heroNewSpriteData)
        {
            // --- Eyebrows
            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                _eyebrowsSprites[i] = Managers.Resource.Load<Sprite>(heroNewSpriteData.Head.Eyebrows[i]);
                string eyebrowsColor = heroNewSpriteData.Head.EyebrowsColors[i];
                if (ColorUtility.TryParseHtmlString(eyebrowsColor, out Color color))
                    _eyebrowsColors[i] = color;
            }

            // --- Eyes
            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                _eyesSprites[i] = Managers.Resource.Load<Sprite>(heroNewSpriteData.Head.Eyes[i]);
                string eyesColor = heroNewSpriteData.Head.EyesColors[i];
                if (ColorUtility.TryParseHtmlString(eyesColor, out Color color))
                    _eyesColors[i] = color;
            }

            // --- Mouth
            for (int i = 0; i < (int)EHeroEmoji.Max; ++i)
            {
                _mouthSprites[i] = Managers.Resource.Load<Sprite>(heroNewSpriteData.Head.Mouth[i]);
                string mouthColor = heroNewSpriteData.Head.MouthColors[i];
                if (ColorUtility.TryParseHtmlString(mouthColor, out Color color))
                    _mouthColors[i] = color;
            }
        }

        // Cache
        private SpriteRenderer _eyebrowsSPR = null;
        private SpriteRenderer _eyesSPR = null;
        private SpriteRenderer _mouthSPR = null;

        private Sprite[] _eyebrowsSprites = null;
        private Color[] _eyebrowsColors = null;

        private Sprite[] _eyesSprites = null;
        private Color[] _eyesColors = null;

        private Sprite[] _mouthSprites = null;
        private Color[] _mouthColors = null;

        [SerializeField] private EHeroEmoji _heroEmoji = EHeroEmoji.None;
        public EHeroEmoji HeroEmoji
        {
            get => _heroEmoji;
            set
            {
                if (_heroEmoji != value)
                {
                    switch (value)
                    {
                        case EHeroEmoji.Idle:
                            {
                                _eyebrowsSPR.sprite = _eyebrowsSprites[(int)EHeroEmoji.Idle];
                                _eyebrowsSPR.color = _eyebrowsColors[(int)EHeroEmoji.Idle];

                                _eyesSPR.sprite = _eyesSprites[(int)EHeroEmoji.Idle];
                                _eyesSPR.color = _eyesColors[(int)EHeroEmoji.Idle];

                                _mouthSPR.sprite = _mouthSprites[(int)EHeroEmoji.Idle];
                                _mouthSPR.color = _mouthColors[(int)EHeroEmoji.Idle];
                            }
                            break;

                        case EHeroEmoji.Move:
                            {
                                _eyebrowsSPR.sprite = _eyebrowsSprites[(int)EHeroEmoji.Move];
                                _eyebrowsSPR.color = _eyebrowsColors[(int)EHeroEmoji.Move];

                                _eyesSPR.sprite = _eyesSprites[(int)EHeroEmoji.Move];
                                _eyesSPR.color = _eyesColors[(int)EHeroEmoji.Move];

                                _mouthSPR.sprite = _mouthSprites[(int)EHeroEmoji.Move];
                                _mouthSPR.color = _mouthColors[(int)EHeroEmoji.Move];
                            }
                            break;

                        case EHeroEmoji.Skill_A:
                            {
                                _eyebrowsSPR.sprite = _eyebrowsSprites[(int)EHeroEmoji.Skill_A];
                                _eyebrowsSPR.color = _eyebrowsColors[(int)EHeroEmoji.Skill_A];

                                _eyesSPR.sprite = _eyesSprites[(int)EHeroEmoji.Skill_A];
                                _eyesSPR.color = _eyesColors[(int)EHeroEmoji.Skill_A];

                                _mouthSPR.sprite = _mouthSprites[(int)EHeroEmoji.Skill_A];
                                _mouthSPR.color = _mouthColors[(int)EHeroEmoji.Skill_A];
                            }
                            break;

                        case EHeroEmoji.Skill_B:
                            {
                                _eyebrowsSPR.sprite = _eyebrowsSprites[(int)EHeroEmoji.Skill_B];
                                _eyebrowsSPR.color = _eyebrowsColors[(int)EHeroEmoji.Skill_B];

                                _eyesSPR.sprite = _eyesSprites[(int)EHeroEmoji.Skill_B];
                                _eyesSPR.color = _eyesColors[(int)EHeroEmoji.Skill_B];

                                _mouthSPR.sprite = _mouthSprites[(int)EHeroEmoji.Skill_B];
                                _mouthSPR.color = _mouthColors[(int)EHeroEmoji.Skill_B];
                            }
                            break;

                        case EHeroEmoji.Skill_C:
                            {
                                _eyebrowsSPR.sprite = _eyebrowsSprites[(int)EHeroEmoji.Skill_C];
                                _eyebrowsSPR.color = _eyebrowsColors[(int)EHeroEmoji.Skill_C];

                                _eyesSPR.sprite = _eyesSprites[(int)EHeroEmoji.Skill_C];
                                _eyesSPR.color = _eyesColors[(int)EHeroEmoji.Skill_C];

                                _mouthSPR.sprite = _mouthSprites[(int)EHeroEmoji.Skill_C];
                                _mouthSPR.color = _mouthColors[(int)EHeroEmoji.Skill_C];
                            }
                            break;

                        case EHeroEmoji.CollectEnv:
                            {
                                _eyebrowsSPR.sprite = _eyebrowsSprites[(int)EHeroEmoji.CollectEnv];
                                _eyebrowsSPR.color = _eyebrowsColors[(int)EHeroEmoji.CollectEnv];

                                _eyesSPR.sprite = _eyesSprites[(int)EHeroEmoji.CollectEnv];
                                _eyesSPR.color = _eyesColors[(int)EHeroEmoji.CollectEnv];

                                _mouthSPR.sprite = _mouthSprites[(int)EHeroEmoji.CollectEnv];
                                _mouthSPR.color = _mouthColors[(int)EHeroEmoji.CollectEnv];
                            }
                            break;

                        case EHeroEmoji.Sick:
                            {
                                _eyebrowsSPR.sprite = _eyebrowsSprites[(int)EHeroEmoji.Sick];
                                _eyebrowsSPR.color = _eyebrowsColors[(int)EHeroEmoji.Sick];

                                _eyesSPR.sprite = _eyesSprites[(int)EHeroEmoji.Sick];
                                _eyesSPR.color = _eyesColors[(int)EHeroEmoji.Sick];

                                _mouthSPR.sprite = _mouthSprites[(int)EHeroEmoji.Sick];
                                _mouthSPR.color = _mouthColors[(int)EHeroEmoji.Sick];
                            }
                            break;

                        case EHeroEmoji.Dead:
                            {
                                _eyebrowsSPR.sprite = _eyebrowsSprites[(int)EHeroEmoji.Dead];
                                _eyebrowsSPR.color = _eyebrowsColors[(int)EHeroEmoji.Dead];

                                _eyesSPR.sprite = _eyesSprites[(int)EHeroEmoji.Dead];
                                _eyesSPR.color = _eyesColors[(int)EHeroEmoji.Dead];

                                _mouthSPR.sprite = _mouthSprites[(int)EHeroEmoji.Dead];
                                _mouthSPR.color = _mouthColors[(int)EHeroEmoji.Dead];
                            }
                            break;
                    }
                }
                _heroEmoji = value;
            }
        }
    }

    public class HeroBody : CreatureBody
    {
        // public HeroBody(Hero owner, int dataID) : base(owner, dataID)
        // {
        //     InitBody(EHeroBodyParts.Head, (int)EHeroHead.Max);
        //     InitBody(EHeroBodyParts.UpperBody, (int)EHeroUpperBody.Max);
        //     InitBody(EHeroBodyParts.LowerBody, (int)EHeroLowerBody.Max);
        //     InitBody(EHeroBodyParts.Weapon, (int)EHeroWeapon.Max); // NEED FIX
        // }
        public override bool SetInfo(BaseObject owner, int dataID)
        {
            _matEyes = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_EyesPaint);
            this.Owner = owner as Hero;
            DataTemplateID = dataID;
            InitBody(EHeroBodyParts.Head, (int)EHeroHead.Max);
            InitBody(EHeroBodyParts.UpperBody, (int)EHeroUpperBody.Max);
            InitBody(EHeroBodyParts.LowerBody, (int)EHeroLowerBody.Max);
            InitBody(EHeroBodyParts.Weapon, (int)EHeroWeapon.Max);
            Face = new HeroFace(this);

            return true;
        }

        protected override IEnumerator CoHurtFlashEffect()
        {
            Debug.Log("<color=yellow>START</color>");

            for (int i = 0; i < Skin.Count; ++i)
                Skin[i].material = _matStrongTint;

            for (int i = 0; i < Appearance.Count; ++i)
                Appearance[i].material = _matStrongTint;

            this.GetComponent<SpriteRenderer>(EHeroHead.Eyes).material = null;

            yield return new WaitForSeconds(20f);

            for (int i = 0; i < Skin.Count; ++i)
                Skin[i].material = _matDefault;

            for (int i = 0; i < Appearance.Count; ++i)
                Appearance[i].material = _matDefault;

            this.GetComponent<SpriteRenderer>(EHeroHead.Eyes).material = _matEyes;

            Debug.Log("<color=yellow>END</color>");
        }

        public Hero Owner { get; private set; } = null;
        private Dictionary<EHeroBodyParts, BodyContainer[]> _bodyDict = new Dictionary<EHeroBodyParts, BodyContainer[]>();
        [field: SerializeField] public HeroFace Face { get; private set; } = null;
        //public void SetEmoji(EHeroEmoji emoji) => Face?.SetEmoji(emoji);
        private Material _matEyes = null;
        public EHeroEmoji HeroEmoji
        {
            get => Face.HeroEmoji;
            set => Face.HeroEmoji = value;
        }

        public List<SpriteRenderer> Skin { get; } = new List<SpriteRenderer>();
        public List<SpriteRenderer> Appearance { get; } = new List<SpriteRenderer>();

        private void InitBody(EHeroBodyParts bodyParts, int length)
        {
            //Material matDefault = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_Default);
            //Material matEyesPaint = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_EyesPaint);

            switch (bodyParts)
            {
                case EHeroBodyParts.Head:
                    {
                        BodyContainer[] containers = new BodyContainer[length];
                        _bodyDict.Add(EHeroBodyParts.Head, containers);

                        string tag = ReadOnly.Util.HBody_HeadSkin;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_HeadSkin, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.HeadSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Hair;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Hair, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.Hair] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Eyes;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Eyes, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matEyes;
                        containers[(int)EHeroHead.Eyes] = new BodyContainer(tag, tr, spr, _matEyes);

                        tag = ReadOnly.Util.HBody_Eyebrows;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Eyebrows, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.Eyebrows] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Mouth;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Mouth, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.Mouth] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Ears;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Ears, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.Ears] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Earrings;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Earrings, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.Earrings] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Beard;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Beard, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.Beard] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Mask;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Mask, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.Mask] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Glasses;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Glasses, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.Glasses] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Helmet;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Helmet, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroHead.Helmet] = new BodyContainer(tag, tr, spr, _matDefault);
                    }
                    break;

                case EHeroBodyParts.UpperBody:
                    {
                        BodyContainer[] containers = new BodyContainer[length];
                        _bodyDict.Add(EHeroBodyParts.UpperBody, containers);

                        string tag = ReadOnly.Util.HBody_TorsoSkin;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_TorsoSkin, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.TorsoSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Torso;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Torso, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.Torso] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Cape;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Cape, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.Cape] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ArmLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ArmLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.ArmLSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ArmL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ArmL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.ArmL] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ForearmLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ForearmLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.ForearmLSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ForearmL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ForearmL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.ForearmL] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_HandLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_HandLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.HandLSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_HandL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_HandL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.HandL] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_FingerSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_FingerSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.FingerSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Finger;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Finger, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.Finger] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ArmRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ArmRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.ArmRSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ArmR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ArmR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.ArmR] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ForearmRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ForearmRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.ForearmRSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ForearmR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ForearmR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.ForearmR] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_SleeveR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_SleeveR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.SleeveR] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_HandRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_HandRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.HandRSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_HandR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_HandR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroUpperBody.HandR] = new BodyContainer(tag, tr, spr, _matDefault);
                    }
                    break;

                case EHeroBodyParts.LowerBody:
                    {
                        BodyContainer[] containers = new BodyContainer[length];
                        _bodyDict.Add(EHeroBodyParts.LowerBody, containers);

                        string tag = ReadOnly.Util.HBody_PelvisSkin;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_PelvisSkin, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.PelvisSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_Pelvis;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_Pelvis, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.Pelvis] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_LegLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_LegLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.LegLSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_LegL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_LegL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.LegL] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ShinLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ShinLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.ShinLSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ShinL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ShinL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.ShinL] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_LegRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_LegRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.LegRSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_LegR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_LegR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.LegR] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ShinRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ShinRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.ShinRSkin] = new BodyContainer(tag, tr, spr, _matDefault);

                        tag = ReadOnly.Util.HBody_ShinR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_ShinR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroLowerBody.ShinR] = new BodyContainer(tag, tr, spr, _matDefault);
                    }
                    break;
                case EHeroBodyParts.Weapon:
                    {
                        BodyContainer[] containers = new BodyContainer[length];
                        _bodyDict.Add(EHeroBodyParts.Weapon, containers);

                        // --- Left Weapon
                        string tag = ReadOnly.Util.HBody_WeaponL;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_WeaponL, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroWeapon.WeaponL] = new BodyContainer(tag, tr, spr, _matDefault);

                        containers[(int)EHeroWeapon.WeaponLSocket] = new BodyContainer(tag: null,
                                                tr: tr.GetChild((int)EWeaponChildIndex.Socket),
                                                spr: null,
                                                defaultMat: null);
                        containers[(int)EHeroWeapon.WeaponLChildGroup] = new BodyContainer(tag: null,
                                                tr: tr.GetChild((int)EWeaponChildIndex.ChildGroup),
                                                spr: null,
                                                defaultMat: null);
                        Transform childGroup = tr.GetChild((int)EWeaponChildIndex.ChildGroup);
                        for (int i = 0; i < childGroup.childCount; ++i)
                            childGroup.GetChild(i).GetComponent<SpriteRenderer>().material = _matDefault;;

                        // --- Right Weapon
                        tag = ReadOnly.Util.HBody_WeaponR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.HBody_WeaponR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        containers[(int)EHeroWeapon.WeaponR] = new BodyContainer(tag, tr, spr, _matDefault);

                        containers[(int)EHeroWeapon.WeaponRSocket] = new BodyContainer(tag: null,
                                                tr: tr.GetChild((int)EWeaponChildIndex.Socket),
                                                spr: null,
                                                defaultMat: null);
                        containers[(int)EHeroWeapon.WeaponRChildGroup] = new BodyContainer(tag: null,
                                                tr: tr.GetChild((int)EWeaponChildIndex.ChildGroup),
                                                spr: null,
                                                defaultMat: null);
                        childGroup = tr.GetChild((int)EWeaponChildIndex.ChildGroup);
                        for (int i = 0; i < childGroup.childCount; ++i)
                            childGroup.GetChild(i).GetComponent<SpriteRenderer>().material = _matDefault; ;
                    }
                    break;
            }
        }

        public T GetComponent<T>(EHeroHead findTarget) where T : UnityEngine.Component
        {
            if (findTarget == EHeroHead.None || findTarget == EHeroHead.Max)
                return null;

            if (_bodyDict.TryGetValue(EHeroBodyParts.Head, out BodyContainer[] containers) == false)
                return null;

            Type type = typeof(T);
            if (type == typeof(Transform))
                return containers[(int)findTarget].TR as T;
            else if (type == typeof(SpriteRenderer))
                return containers[(int)findTarget].SPR as T;

            return null;
        }

        public T GetComponent<T>(EHeroUpperBody findTarget) where T : UnityEngine.Component
        {
            if (findTarget == EHeroUpperBody.None || findTarget == EHeroUpperBody.Max)
                return null;

            if (_bodyDict.TryGetValue(EHeroBodyParts.UpperBody, out BodyContainer[] containers) == false)
                return null;

            Type type = typeof(T);
            if (type == typeof(Transform))
                return containers[(int)findTarget].TR as T;

            if (type == typeof(SpriteRenderer))
                return containers[(int)findTarget].SPR as T;

            return null;
        }

        public T GetComponent<T>(EHeroLowerBody findTarget) where T : UnityEngine.Component
        {
            if (findTarget == EHeroLowerBody.None || findTarget == EHeroLowerBody.Max)
                return null;

            if (_bodyDict.TryGetValue(EHeroBodyParts.LowerBody, out BodyContainer[] containers) == false)
                return null;

            Type type = typeof(T);
            if (type == typeof(Transform))
                return containers[(int)findTarget].TR as T;
            else if (type == typeof(SpriteRenderer))
                return containers[(int)findTarget].SPR as T;

            return null;
        }

        public T GetComponent<T>(EHeroWeapon findTarget) where T : UnityEngine.Component
        {
            if (findTarget == EHeroWeapon.None || findTarget == EHeroWeapon.Max)
                return null;

            if (_bodyDict.TryGetValue(EHeroBodyParts.Weapon, out BodyContainer[] containers) == false)
                return null;

            Type type = typeof(T);
            if (type == typeof(Transform))
                return containers[(int)findTarget].TR as T;
            else if (type == typeof(SpriteRenderer))
                return containers[(int)findTarget].SPR as T;

            return null;
        }

        // public void SetFace()
        // {
        //     if (Face == null)
        //         Face = new HeroFace(this);
        // }

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
            for (int i = 0; i < _leftWeaponSPRs.Length; ++i)
                _leftWeaponSPRs[i].sprite = null;

            for (int i = 0; i < _rightWeaponSPRs.Length; ++i)
                _rightWeaponSPRs[i].sprite = null;
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

        // --- TEMP
        public override Vector3 GetFirePosition() => base.GetFirePosition();
    }
}
