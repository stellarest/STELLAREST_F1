using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STELLAREST_F1.Data;
using static STELLAREST_F1.Define;
using System;

namespace STELLAREST_F1
{
    [System.Serializable]
    public class MonsterBody : CreatureBody
    {
        #region Background
        public Monster Owner { get; private set; } = null;
        public EMonsterType MonsterType { get; private set; } = EMonsterType.None;
        private Sprite[] _heads = null;
        private Dictionary<EBirdBody, BodyContainer> _birdBodyDict = null;
        private Dictionary<EQuadrupedsBody, BodyContainer> _quadrupedsBodyDict = null;

        [SerializeField] private EMonsterEmoji _monsterEmoji = EMonsterEmoji.None;
        public EMonsterEmoji MonsterEmoji
        {
            get => _monsterEmoji;
            set
            {
                if (_monsterEmoji != value)
                {
                    _monsterEmoji = value;
                    switch (value)
                    {
                        case EMonsterEmoji.Normal:
                            {
                                if (MonsterType == EMonsterType.Bird)
                                    _birdBodyDict[EBirdBody.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Normal];
                                else if (MonsterType == EMonsterType.Quadrupeds)
                                    _quadrupedsBodyDict[EQuadrupedsBody.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Normal];
                            }
                            break;

                        case EMonsterEmoji.Angry:
                            {
                                if (MonsterType == EMonsterType.Bird)
                                    _birdBodyDict[EBirdBody.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Angry];
                                else if (MonsterType == EMonsterType.Quadrupeds)
                                    _quadrupedsBodyDict[EQuadrupedsBody.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Angry];
                            }
                            break;

                        case EMonsterEmoji.Dead:
                            {
                                if (MonsterType == EMonsterType.Bird)
                                    _birdBodyDict[EBirdBody.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Dead];
                                else if (MonsterType == EMonsterType.Quadrupeds)
                                    _quadrupedsBodyDict[EQuadrupedsBody.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Dead];
                            }
                            break;
                    }
                }
            }
        }

        public BodyContainer GetContainer(EBirdBody bodyParts) => _birdBodyDict[bodyParts];
        public BodyContainer GetContainer(EQuadrupedsBody bodyParts) => _quadrupedsBodyDict[bodyParts];

        protected override void ApplyDefaultMat_Alpha(float alphaValue)
        {
            ApplyDefaultMat_Alpha(MonsterType, alphaValue);
        }

        private void ApplyDefaultMat_Alpha(EMonsterType monsterType, float alphaValue)
        {
            switch (monsterType)
            {
                case EMonsterType.Bird:
                    {
                        foreach (var container in _birdBodyDict.Values)
                        {
                            if (container.SPR == null)
                                continue;

                            container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                            Color matColor = container.MatPropertyBlock.GetColor(_matDefaultColor);
                            matColor = new Color(matColor.r, matColor.g, matColor.b, alphaValue);
                            container.MatPropertyBlock.SetColor(_matStrongTintColor, matColor);
                            container.SPR.SetPropertyBlock(container.MatPropertyBlock);
                        }
                    }
                    break;

                case EMonsterType.Quadrupeds:
                    {
                        foreach (var container in _quadrupedsBodyDict.Values)
                        {
                            if (container.SPR == null)
                                continue;

                            container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                            Color matColor = container.MatPropertyBlock.GetColor(_matDefaultColor);
                            matColor = new Color(matColor.r, matColor.g, matColor.b, alphaValue);        
                            container.MatPropertyBlock.SetColor(_matStrongTintColor, matColor);
                            container.SPR.SetPropertyBlock(container.MatPropertyBlock);
                        }
                    }
                    break;
            }
        }

        protected override void ApplyStrongTintMat_Color(Color desiredColor)
        {
            ApplyStrongTintMat_Color(MonsterType, desiredColor);
        }

        private void ApplyStrongTintMat_Color(EMonsterType monsterType, Color desiredColor)
        {
            switch (monsterType)
            {
                case EMonsterType.Bird:
                    {
                        foreach (var container in _birdBodyDict.Values)
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
                    break;

                case EMonsterType.Quadrupeds:
                    {
                        foreach (var container in _quadrupedsBodyDict.Values)
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
                    break;
            }
        }

        public override void ResetMaterialsAndColors()
        {
            ResetMaterialsAndColors(MonsterType);
        }

        private void ResetMaterialsAndColors(EMonsterType monsterType)
        {
            switch (monsterType)
            {
                case EMonsterType.Bird:
                    {
                        foreach (var container in _birdBodyDict.Values)
                        {
                            if (container.SPR == null)
                                continue;

                            container.SPR.material = _matDefault;
                            container.SPR.color = container.DefaultSPRColor;
                            container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                            container.MatPropertyBlock.SetColor(_matStrongTintColor, container.DefaultMatColor);
                            container.SPR.SetPropertyBlock(container.MatPropertyBlock);
                        }
                    }
                    break;

                case EMonsterType.Quadrupeds:
                    {
                        foreach (var container in _quadrupedsBodyDict.Values)
                        {
                            if (container.SPR == null)
                                continue;

                            container.SPR.material = _matDefault;
                            container.SPR.color = container.DefaultSPRColor;
                            container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                            container.MatPropertyBlock.SetColor(_matStrongTintColor, container.DefaultMatColor);
                            container.SPR.SetPropertyBlock(container.MatPropertyBlock);
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Core
        public override void InitialSetInfo(int dataID, BaseObject owner)
        {
            Owner = owner as Monster;
            MonsterData monsterData = Managers.Data.MonsterDataDict[dataID];
            MonsterType = monsterData.MonsterType;
            _heads = new Sprite[(int)EMonsterEmoji.Max];
            InitBody(Managers.Data.MonsterDataDict[dataID], dataID);
        }

        private void InitBody(MonsterData monsterData, int dataID)
        {
            switch (monsterData.MonsterType)
            {
                case EMonsterType.Bird:
                    {
                        // --- Bird
                        _birdBodyDict = new Dictionary<EBirdBody, BodyContainer>();
                        BirdSpriteData bird = Managers.Data.BirdSpriteDataDict[dataID];

                        // --- Body
                        string tag = Util.GetStringFromEnum(EBirdBody.Body);
                        Transform tr = Owner.MonsterAnim.transform;
                        tr.transform.localPosition = bird.BodyPosition;
                        switch (monsterData.MonsterSize)
                        {
                            // --- Use Preset Size
                            case EObjectSize.None:
                                break;

                            case EObjectSize.VerySmall:
                                Owner.transform.localScale = new Vector3(0.25F, 0.25F, 1);
                                break;

                            case EObjectSize.Small:
                                Owner.transform.localScale = new Vector3(0.5F, 0.5F, 1);
                                break;

                            case EObjectSize.Medium:
                                Owner.transform.localScale = new Vector3(1F, 1F, 1);
                                break;

                            case EObjectSize.Large:
                                Owner.transform.localScale = new Vector3(2F, 2F, 1);
                                break;

                            case EObjectSize.VeryLarge:
                                Owner.transform.localScale = new Vector3(3F, 3F, 1);
                                break;
                        }

                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        Sprite sprite = Managers.Resource.Load<Sprite>(bird.Body);
                        if (sprite != null)
                            spr.sprite = sprite;

                        _birdBodyDict[EBirdBody.Body] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        MaterialPropertyBlock matPB = _birdBodyDict[EBirdBody.Body].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _birdBodyDict[EBirdBody.Body].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Heads Emoji(if it has invalid value, error)
                        _heads = new Sprite[(int)EMonsterEmoji.Max];
                        for (int i = 0; i < _heads.Length; ++i)
                        {
                            _heads[i] = Managers.Resource.Load<Sprite>(bird.Heads[i]);
                            if (_heads[i] == null)
                            {
                                Debug.LogError($"{nameof(InitBody)}, {bird.Heads[i]}");
                                Debug.Break();
                            }
                        }

                        tag = Util.GetStringFromEnum(EBirdBody.Head);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        if (_heads[(int)EMonsterEmoji.Normal] != null)
                        {
                            tr.localPosition = bird.HeadPosition;
                            spr.sprite = _heads[(int)EMonsterEmoji.Normal];
                            spr.sortingOrder = bird.HeadSortingOrder;
                        }

                        _birdBodyDict[EBirdBody.Head] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _birdBodyDict[EBirdBody.Head].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _birdBodyDict[EBirdBody.Head].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Wing
                        tag = Util.GetStringFromEnum(EBirdBody.Wing);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(bird.Wing);
                        if (sprite != null)
                        {
                            tr.localPosition = bird.WingPosition;
                            spr.sprite = sprite;
                            spr.sortingOrder = bird.WingSortingOrder;
                        }

                        _birdBodyDict[EBirdBody.Wing] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _birdBodyDict[EBirdBody.Wing].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _birdBodyDict[EBirdBody.Wing].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- LegL
                        tag = Util.GetStringFromEnum(EBirdBody.LegL);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(bird.LegL);
                        if (sprite != null)
                        {
                            tr.localPosition = bird.LegLPosition;
                            spr.sprite = sprite;
                            spr.sortingOrder = bird.LegLSortingOrder;
                        }

                        _birdBodyDict[EBirdBody.LegL] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _birdBodyDict[EBirdBody.LegL].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _birdBodyDict[EBirdBody.LegL].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- LegR
                        tag = Util.GetStringFromEnum(EBirdBody.LegR);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(bird.LegR);
                        if (sprite != null)
                        {
                            tr.localPosition = bird.LegRPosition;
                            spr.sprite = sprite;
                            spr.sortingOrder = bird.LegRSortingOrder;
                        }

                        _birdBodyDict[EBirdBody.LegR] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _birdBodyDict[EBirdBody.LegR].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _birdBodyDict[EBirdBody.LegR].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Tail
                        tag = Util.GetStringFromEnum(EBirdBody.Tail);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(bird.Tail);
                        if (sprite != null)
                        {
                            tr.localPosition = bird.TailPosition;
                            spr.sprite = sprite;
                            spr.sortingOrder = bird.TailSortingOrder;
                        }

                        _birdBodyDict[EBirdBody.Tail] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _birdBodyDict[EBirdBody.Tail].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _birdBodyDict[EBirdBody.Tail].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);
                    }
                    break;

                case EMonsterType.Quadrupeds:
                    {
                        // --- Quadrupeds
                        _quadrupedsBodyDict = new Dictionary<EQuadrupedsBody, BodyContainer>();
                        QuadrupedsSpriteData quadrupeds = Managers.Data.QuadrupedsSpriteDataDict[dataID];

                        // --- Body
                        string tag = Util.GetStringFromEnum(EQuadrupedsBody.Body);
                        Transform tr = Owner.MonsterAnim.transform;
                        tr.localPosition = quadrupeds.BodyPosition;
                        switch (monsterData.MonsterSize)
                        {
                            // --- Use Preset Size
                            case EObjectSize.None:
                                break;

                            case EObjectSize.VerySmall:
                                Owner.transform.localScale = new Vector3(0.25F, 0.25F, 1);
                                break;

                            case EObjectSize.Small:
                                Owner.transform.localScale = new Vector3(0.5F, 0.5F, 1);
                                break;

                            case EObjectSize.Medium:
                                Owner.transform.localScale = new Vector3(1F, 1F, 1);
                                break;

                            case EObjectSize.Large:
                                Owner.transform.localScale = new Vector3(2F, 2F, 1);
                                break;

                            case EObjectSize.VeryLarge:
                                Owner.transform.localScale = new Vector3(3F, 3F, 1);
                                break;
                        }

                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        Sprite sprite = Managers.Resource.Load<Sprite>(quadrupeds.Body);
                        if (sprite != null)
                            spr.sprite = sprite;

                        _quadrupedsBodyDict[EQuadrupedsBody.Body] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        MaterialPropertyBlock matPB = _quadrupedsBodyDict[EQuadrupedsBody.Body].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _quadrupedsBodyDict[EQuadrupedsBody.Body].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Heads Emoji(if it has invalid value, error)
                        _heads = new Sprite[(int)EMonsterEmoji.Max];
                        for (int i = 0; i < _heads.Length; ++i)
                        {
                            _heads[i] = Managers.Resource.Load<Sprite>(quadrupeds.Heads[i]);
                            if (_heads[i] == null)
                            {
                                Debug.LogError($"{nameof(InitBody)}, {quadrupeds.Heads[i]}");
                                Debug.Break();
                            }
                        }

                        tag = Util.GetStringFromEnum(EQuadrupedsBody.Head);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        if (_heads[(int)EMonsterEmoji.Normal] != null)
                        {
                            tr.localPosition = quadrupeds.HeadPosition;
                            spr.sprite = _heads[(int)EMonsterEmoji.Normal];
                            spr.sortingOrder = quadrupeds.HeadSortingOrder;
                        }

                        _quadrupedsBodyDict[EQuadrupedsBody.Head] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _quadrupedsBodyDict[EQuadrupedsBody.Head].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _quadrupedsBodyDict[EQuadrupedsBody.Head].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- LegFrontL
                        tag = Util.GetStringFromEnum(EQuadrupedsBody.LegFrontL);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(quadrupeds.LegFrontL);
                        if (sprite != null)
                        {
                            tr.localPosition = quadrupeds.LegFrontLPosition;
                            spr.sprite = sprite;
                            spr.sortingOrder = quadrupeds.LegFrontLSortingOrder;
                        }

                        _quadrupedsBodyDict[EQuadrupedsBody.LegFrontL] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _quadrupedsBodyDict[EQuadrupedsBody.LegFrontL].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _quadrupedsBodyDict[EQuadrupedsBody.LegFrontL].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- LegFrontR
                        tag = Util.GetStringFromEnum(EQuadrupedsBody.LegFrontR);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(quadrupeds.LegFrontR);
                        if (sprite != null)
                        {
                            tr.localPosition = quadrupeds.LegFrontRPosition;
                            spr.sprite = sprite;
                            spr.sortingOrder = quadrupeds.LegFrontRSortingOrder;
                        }

                        _quadrupedsBodyDict[EQuadrupedsBody.LegFrontR] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _quadrupedsBodyDict[EQuadrupedsBody.LegFrontR].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _quadrupedsBodyDict[EQuadrupedsBody.LegFrontR].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- LegBackL
                        tag = Util.GetStringFromEnum(EQuadrupedsBody.LegBackL);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(quadrupeds.LegBackL);
                        if (sprite != null)
                        {
                            tr.localPosition = quadrupeds.LegBackLPosition;
                            spr.sprite = sprite;
                            spr.sortingOrder = quadrupeds.LegBackLSortingOrder;
                        }

                        _quadrupedsBodyDict[EQuadrupedsBody.LegBackL] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _quadrupedsBodyDict[EQuadrupedsBody.LegBackL].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _quadrupedsBodyDict[EQuadrupedsBody.LegBackL].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- LegBackR
                        tag = Util.GetStringFromEnum(EQuadrupedsBody.LegBackR);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(quadrupeds.LegBackR);
                        if (sprite != null)
                        {
                            tr.localPosition = quadrupeds.LegBackRPosition;
                            spr.sprite = sprite;
                            spr.sortingOrder = quadrupeds.LegBackRSortingOrder;
                        }

                        _quadrupedsBodyDict[EQuadrupedsBody.LegBackR] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _quadrupedsBodyDict[EQuadrupedsBody.LegBackR].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _quadrupedsBodyDict[EQuadrupedsBody.LegBackR].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Tail
                        tag = Util.GetStringFromEnum(EQuadrupedsBody.Tail);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(quadrupeds.Tail);
                        if (sprite != null)
                        {
                            tr.localPosition = quadrupeds.TailPosition;
                            spr.sprite = sprite;
                            spr.sortingOrder = quadrupeds.TailSortingOrder;
                        }

                        _quadrupedsBodyDict[EQuadrupedsBody.Tail] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _quadrupedsBodyDict[EQuadrupedsBody.Tail].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _quadrupedsBodyDict[EQuadrupedsBody.Tail].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);
                    }
                    break;
            }
        }
        #endregion

        #region Coroutines
        // protected override IEnumerator CoHurtFlashEffect(bool isCritical = false)
        // {
        //     if (isCritical == false)
        //         ApplyStrongTintMat_Color(Color.white);
        //     else
        //         ApplyStrongTintMat_Color(Color.red);
        //     yield return new WaitForSeconds(0.1f);
        //     ResetMaterialsAndColors();
        // }
        #endregion
    }
}
