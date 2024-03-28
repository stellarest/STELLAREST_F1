using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroBody : CreatureBody
    {
        public HeroBody(Hero owner, int dataID) : base(owner, dataID)
        {
            InitBody(EHeroBodyParts.Head, (int)EHeroHead.Max);
            InitBody(EHeroBodyParts.UpperBody, (int)EHeroUpperBody.Max);
            InitBody(EHeroBodyParts.LowerBody, (int)EHeroLowerBody.Max);
            InitBody(EHeroBodyParts.Weapon, (int)EHeroLowerBody.Max);
        }

        #region Hero Face
        public class HeroFace
        {
            public HeroFace(HeroBody heroBody)
            {
                _heroBody = heroBody;
                Data.HeroSpriteData heroSpriteData = Managers.Data.HeroSpriteDataDict[heroBody.TemplateID];

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

            private HeroBody _heroBody = null;

            private Sprite[] _eyebrowsSprites = null;
            private Color[] _eyebrowsColors = null;

            private Sprite[] _eyesSprites = null;
            private Color[] _eyesColors = null;

            private Sprite[] _mouthSprites = null;
            private Color[] _mouthColors = null;

            public void SetEmoji(EHeroEmoji heroEmoji)
            {
                switch (heroEmoji)
                {
                    case EHeroEmoji.Default:
                        {
                            // Eyebrows - Default
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).sprite = _eyebrowsSprites[(int)EHeroEmoji.Default];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).color = _eyebrowsColors[(int)EHeroEmoji.Default];

                            // Eyes - Default
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).sprite = _eyesSprites[(int)EHeroEmoji.Default];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).color = _eyesColors[(int)EHeroEmoji.Default];

                            // Mouth - Default
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).sprite = _mouthSprites[(int)EHeroEmoji.Default];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).color = _mouthColors[(int)EHeroEmoji.Default];
                        }
                        break;

                    case EHeroEmoji.Combat:
                        {
                            // Eyebrows - Combat
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).sprite = _eyebrowsSprites[(int)EHeroEmoji.Combat];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).color = _eyebrowsColors[(int)EHeroEmoji.Combat];

                            // Eyes - Combat
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).sprite = _eyesSprites[(int)EHeroEmoji.Combat];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).color = _eyesColors[(int)EHeroEmoji.Combat];

                            // Mouth - Combat
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).sprite = _mouthSprites[(int)EHeroEmoji.Combat];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).color = _mouthColors[(int)EHeroEmoji.Combat];
                        }
                        break;

                    case EHeroEmoji.Sick:
                        {
                            // Eyebrows - Sick
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).sprite = _eyebrowsSprites[(int)EHeroEmoji.Sick];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).color = _eyebrowsColors[(int)EHeroEmoji.Sick];

                            // Eyes - Sick
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).sprite = _eyesSprites[(int)EHeroEmoji.Sick];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).color = _eyesColors[(int)EHeroEmoji.Sick];

                            // Mouth - Sick
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).sprite = _mouthSprites[(int)EHeroEmoji.Sick];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).color = _mouthColors[(int)EHeroEmoji.Sick];
                        }
                        break;

                    case EHeroEmoji.Dead:
                        {
                            // Eyebrows - Dead
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).sprite = _eyebrowsSprites[(int)EHeroEmoji.Dead];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyebrows).color = _eyebrowsColors[(int)EHeroEmoji.Dead];

                            // Eyes - Dead
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).sprite = _eyesSprites[(int)EHeroEmoji.Dead];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Eyes).color = _eyesColors[(int)EHeroEmoji.Dead];

                            // Mouth - Dead
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).sprite = _mouthSprites[(int)EHeroEmoji.Dead];
                            _heroBody.GetComponent<SpriteRenderer>(EHeroHead.Mouth).color = _mouthColors[(int)EHeroEmoji.Dead];
                        }
                        break;
                }
            }
        }
        #endregion

        private Dictionary<EHeroBodyParts, Container[]> _bodyDict = new Dictionary<EHeroBodyParts, Container[]>();
        public HeroFace Face { get; private set; } = null;
        public void SetEmoji(EHeroEmoji emoji) => Face?.SetEmoji(emoji);

        public void SetFace()
        {
            if (Face == null)
                Face = new HeroFace(this);
        }

        private void InitBody(EHeroBodyParts bodyParts, int length)
        {
            switch (bodyParts)
            {
                case EHeroBodyParts.Head:
                    {
                        Container[] containers = new Container[length];
                        _bodyDict.Add(EHeroBodyParts.Head, containers);

                        string tag = ReadOnly.String.HBody_HeadSkin;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_HeadSkin, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.HeadSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Hair;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Hair, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Hair] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Eyes;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Eyes, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Eyes] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Eyebrows;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Eyebrows, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Eyebrows] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Mouth;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Mouth, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Mouth] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Beard;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Beard, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Beard] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Earrings;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Earrings, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Earrings] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Mask;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Mask, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Mask] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Glasses;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Glasses, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Glasses] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Helmet;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Helmet, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Helmet] = new Container(tag, tr, spr);
                    }
                    break;

                case EHeroBodyParts.UpperBody:
                    {
                        Container[] containers = new Container[length];
                        _bodyDict.Add(EHeroBodyParts.UpperBody, containers);

                        string tag = ReadOnly.String.HBody_TorsoSkin;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_TorsoSkin, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.TorsoSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Torso;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Torso, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.Torso] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Cape;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Cape, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.Cape] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ArmLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ArmLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ArmLSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ArmL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ArmL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ArmL] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ForearmLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ForearmLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ForearmLSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ForearmL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ForearmL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ForearmL] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_HandLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_HandLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.HandLSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_HandL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_HandL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.HandL] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_FingerSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_FingerSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.FingerSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Finger;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Finger, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.Finger] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ArmRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ArmRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ArmRSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ArmR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ArmR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ArmR] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ForearmRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ForearmRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ForearmRSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ForearmR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ForearmR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ForearmR] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_SleeveR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_SleeveR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.SleeveR] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_HandRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_HandRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.HandRSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_HandR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_HandR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.HandR] = new Container(tag, tr, spr);
                    }
                    break;

                case EHeroBodyParts.LowerBody:
                    {
                        Container[] containers = new Container[length];
                        _bodyDict.Add(EHeroBodyParts.LowerBody, containers);

                        string tag = ReadOnly.String.HBody_PelvisSkin;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_PelvisSkin, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.PelvisSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Pelvis;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Pelvis, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.Pelvis] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_LegLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_LegLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.LegLSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_LegL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_LegL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.LegL] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ShinLSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ShinLSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.ShinLSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ShinL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ShinL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.ShinL] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_LegRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_LegRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.LegRSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_LegR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_LegR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.LegR] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ShinRSkin;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ShinRSkin, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.ShinRSkin] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ShinR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ShinR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.ShinR] = new Container(tag, tr, spr);
                    }
                    break;
                case EHeroBodyParts.Weapon:
                    {
                        Container[] containers = new Container[length];
                        _bodyDict.Add(EHeroBodyParts.Weapon, containers);

                        string tag = ReadOnly.String.HBody_WeaponL;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_WeaponL, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroWeapon.WeaponL] = new Container(tag, tr, spr);

                        tag = ReadOnly.String.HBody_WeaponR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_WeaponR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroWeapon.WeaponR] = new Container(tag, tr, spr);
                    }
                    break;
            }
        }

        public T GetComponent<T>(EHeroHead findTarget) where T : UnityEngine.Component
        {
            if (findTarget == EHeroHead.None || findTarget == EHeroHead.Max)
                return null;

            if (_bodyDict.TryGetValue(EHeroBodyParts.Head, out Container[] containers) == false)
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

            if (_bodyDict.TryGetValue(EHeroBodyParts.UpperBody, out Container[] containers) == false)
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

            if (_bodyDict.TryGetValue(EHeroBodyParts.LowerBody, out Container[] containers) == false)
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

            if (_bodyDict.TryGetValue(EHeroBodyParts.Weapon, out Container[] containers) == false)
                return null;

            Type type = typeof(T);
            if (type == typeof(Transform))
                return containers[(int)findTarget].TR as T;
            else if (type == typeof(SpriteRenderer))
                return containers[(int)findTarget].SPR as T;

            return null;
        }
    }
}
