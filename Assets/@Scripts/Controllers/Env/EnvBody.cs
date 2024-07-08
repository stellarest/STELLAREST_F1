using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STELLAREST_F1.Data;
using static STELLAREST_F1.Define;
using System.ComponentModel;
using UnityEditor;

namespace STELLAREST_F1
{
    public class EnvBody : BaseBody
    {
        #region Background
        public Env Owner { get; private set; } = null;
        public EEnvType EnvType { get; private set; } = EEnvType.None;
        private Dictionary<ETreeBody, BodyContainer> _treeBodyDict = null;
        private Dictionary<ERockBody, BodyContainer> _rockBodyDict = null;

        public BodyContainer GetContainer(ETreeBody treeBody) => _treeBodyDict[treeBody];
        public BodyContainer GetContainer(ERockBody rockBody) => _rockBodyDict[rockBody];

        private void ApplyEnvStrongTint(EEnvType envType, Color desiredColor)
        {
            switch (envType)
            {
                case EEnvType.Tree:
                    ApplyTreeStrongTint(desiredColor);
                    break;

                case EEnvType.Rock:
                    ApplyRockStrongTint(desiredColor);
                    break;
            }
        }

        private void ApplyTreeStrongTint(Color desiredColor)
        {
            foreach (var container in _treeBodyDict.Values)
            {
                if (container.SPR == null)
                    continue;

                if (container == GetContainer(ETreeBody.Shadow))
                    continue;

                container.SPR.material = _matStrongTint;
                container.SPR.color = desiredColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matStrongTintColor, desiredColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }
        }

        private void ApplyRockStrongTint(Color desiredColor)
        {
            foreach (var container in _rockBodyDict.Values)
            {
                if (container.SPR == null)
                    continue;

                if (container == GetContainer(ERockBody.Shadow))
                    continue;

                container.SPR.material = _matStrongTint;
                container.SPR.color = desiredColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matStrongTintColor, desiredColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }
        }

        public void ResetEnvMaterialsAndColors(EEnvType envType)
        {
            switch (envType)
            {
                case EEnvType.Tree:
                    ResetTreeMaterialsAndColors();
                    break;

                case EEnvType.Rock:
                    ResetRockMaterialsAndColors();
                    break;
            }
        }

        private void ResetTreeMaterialsAndColors()
        {
            foreach (var container in _treeBodyDict.Values)
            {
                if (container.SPR == null)
                    continue;

                if (container == GetContainer(ETreeBody.Shadow))
                    continue;

                container.SPR.material = _matDefault;
                container.SPR.color = container.DefaultSPRColor;
                container.SPR.GetPropertyBlock(container.MatPropertyBlock);
                container.MatPropertyBlock.SetColor(_matDefaultColor, container.DefaultMatColor);
                container.SPR.SetPropertyBlock(container.MatPropertyBlock);
            }
        }

        private void ResetRockMaterialsAndColors()
        {
            foreach (var container in _rockBodyDict.Values)
            {
                if (container.SPR == null)
                    continue;

                if (container == GetContainer(ERockBody.Shadow))
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
            this.Owner = owner as Env;
            if (Owner == null)
                return false;

            EnvData envData = Managers.Data.EnvDataDict[dataID];
            EnvType = envData.EnvType;
            InitBody(Managers.Data.EnvDataDict[dataID], dataID);
            return true;
        }

        private void InitBody(EnvData envData, int dataID)
        {
            switch (envData.EnvType)
            {
                case EEnvType.Tree:
                    {
                        // --- Tree
                        _treeBodyDict = new Dictionary<ETreeBody, BodyContainer>();
                        TreeSpriteData tree = Managers.Data.TreeSpriteDataDict[dataID];

                        // --- Trunk
                        string tag = Util.GetStringFromEnum(ETreeBody.Trunk);
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        Sprite sprite = Managers.Resource.Load<Sprite>(tree.Trunk);
                        if (sprite != null)
                        {
                            tr.localPosition = tree.TrunkPosition;
                            spr.sprite = sprite;
                        }

                        _treeBodyDict[ETreeBody.Trunk] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        MaterialPropertyBlock matPB = _treeBodyDict[ETreeBody.Trunk].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _treeBodyDict[ETreeBody.Trunk].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Patch
                        tag = Util.GetStringFromEnum(ETreeBody.Patch);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(tree.Patch);
                        if (sprite != null)
                        {
                            tr.localPosition = tree.PatchPosition;
                            spr.sprite = sprite;
                        }

                        _treeBodyDict[ETreeBody.Patch] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _treeBodyDict[ETreeBody.Patch].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _treeBodyDict[ETreeBody.Patch].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Stump
                        tag = Util.GetStringFromEnum(ETreeBody.Stump);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(tree.Stump);
                        if (sprite != null)
                        {
                            tr.localPosition = tree.StumpPosition;
                            spr.sprite = sprite;
                        }

                        _treeBodyDict[ETreeBody.Stump] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _treeBodyDict[ETreeBody.Stump].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _treeBodyDict[ETreeBody.Stump].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- EndParticle
                        tag = Util.GetStringFromEnum(ETreeBody.EndParticle);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        tr.localPosition = Vector3.up * 2f;
                        Material endParticleMat = Managers.Resource.Load<Material>(tree.EndParticleMaterial);
                        if (endParticleMat != null)
                        {
                            ParticleSystemRenderer pr = tr.GetComponent<ParticleSystemRenderer>();
                             pr.material = endParticleMat;
                        }
                        else
                        {
                            ParticleSystem ps = tr.GetComponent<ParticleSystem>();
                            UnityEngine.Component.DestroyImmediate(ps);
                        }

                        _treeBodyDict[ETreeBody.EndParticle] = new BodyContainer(tag: tag, tr: tr, spr: null,
                                                defaultSPRMat: null, defaultSPRColor: Color.white,
                                                defaultMatColor: Color.white, matPB: null);

                        // --- Fruits_ChildsRoot
                        tag = Util.GetStringFromEnum(ETreeBody.Fruits_ChildsRoot);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);

                        _treeBodyDict[ETreeBody.Fruits_ChildsRoot] = new BodyContainer(tag: tag, tr: tr, spr: null,
                                                defaultSPRMat: null, defaultSPRColor: Color.white,
                                                defaultMatColor: Color.white, matPB: null);

                        // --- Fruits_Child_01
                        tag = Util.GetStringFromEnum(ETreeBody.Fruits_Child_01);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(tree.Fruits);
                        if (sprite != null)
                        {
                            tr.localScale = tree.FruitsScales[0];
                            tr.localPosition = tree.FruitsPositions[0];
                            tr.localRotation = Quaternion.Euler(tree.FruitsRotations[0].x, 
                                            tree.FruitsRotations[0].y, tree.FruitsRotations[0].z);
                            spr.sprite = sprite;
                        }

                        _treeBodyDict[ETreeBody.Fruits_Child_01] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _treeBodyDict[ETreeBody.Fruits_Child_01].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _treeBodyDict[ETreeBody.Fruits_Child_01].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Fruits_Child_02
                        tag = Util.GetStringFromEnum(ETreeBody.Fruits_Child_02);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(tree.Fruits);
                        if (sprite != null)
                        {
                            tr.localScale = tree.FruitsScales[1];
                            tr.localPosition = tree.FruitsPositions[1];
                            tr.localRotation = Quaternion.Euler(tree.FruitsRotations[1].x, 
                                            tree.FruitsRotations[1].y, tree.FruitsRotations[1].z);
                            spr.sprite = sprite;
                        }

                        _treeBodyDict[ETreeBody.Fruits_Child_02] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _treeBodyDict[ETreeBody.Fruits_Child_02].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _treeBodyDict[ETreeBody.Fruits_Child_02].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Fruits_Child_03
                        tag = Util.GetStringFromEnum(ETreeBody.Fruits_Child_03);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(tree.Fruits);
                        if (sprite != null)
                        {
                            tr.localScale = tree.FruitsScales[2];
                            tr.localPosition = tree.FruitsPositions[2];
                            tr.localRotation = Quaternion.Euler(tree.FruitsRotations[2].x, 
                                            tree.FruitsRotations[2].y, tree.FruitsRotations[2].z);
                            spr.sprite = sprite;
                        }

                        _treeBodyDict[ETreeBody.Fruits_Child_03] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _treeBodyDict[ETreeBody.Fruits_Child_03].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _treeBodyDict[ETreeBody.Fruits_Child_03].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Shadow
                        tag = Util.GetStringFromEnum(ETreeBody.Shadow);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(ReadOnly.Util.Shadow_SP);
                        if (sprite != null)
                        {
                            tr.localPosition = tree.StumpPosition;
                            spr.sprite = sprite;
                        }

                        _treeBodyDict[ETreeBody.Shadow] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _treeBodyDict[ETreeBody.Shadow].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _treeBodyDict[ETreeBody.Shadow].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);
                    }
                    break;

                case EEnvType.Rock:
                    {
                        // --- Rock
                        _rockBodyDict = new Dictionary<ERockBody, BodyContainer>();
                        RockSpriteData rock = Managers.Data.RockSpriteDataDict[dataID];

                        // --- Rock(Body)
                        // Material matRockFragments = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_RockFragments);
                        // Material matGlow = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_Glow);

                        // --- Rock(Body)
                        string tag = Util.GetStringFromEnum(ERockBody.Rock);
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        Sprite sprite = Managers.Resource.Load<Sprite>(ReadOnly.Util.EnvRock_Rock_SP);
                        if (sprite != null)
                            spr.sprite = sprite;

                       _rockBodyDict[ERockBody.Rock] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        MaterialPropertyBlock matPB = _rockBodyDict[ERockBody.Rock].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Rock].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Empty
                        tag = Util.GetStringFromEnum(ERockBody.Empty);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(ReadOnly.Util.EnvRoc_Empty_SP);
                        if (sprite != null)
                            spr.sprite = sprite;

                        _rockBodyDict[ERockBody.Empty] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.Empty].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Empty].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Ore
                        tag = Util.GetStringFromEnum(ERockBody.Ore);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(rock.Ore);
                        if (sprite != null)
                            spr.sprite = sprite;

                        _rockBodyDict[ERockBody.Ore] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.Ore].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Ore].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- OreShadow
                        tag = Util.GetStringFromEnum(ERockBody.OreShadow);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(rock.OreShadow);
                        if (sprite != null)
                            spr.sprite = sprite;

                        _rockBodyDict[ERockBody.OreShadow] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.OreShadow].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.OreShadow].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- OreLight
                        tag = Util.GetStringFromEnum(ERockBody.OreLight);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        
                        if (string.IsNullOrEmpty(rock.OreLightColor) == false)
                        {
                            if (ColorUtility.TryParseHtmlString(rock.OreLightColor, out Color oreLightColor))
                            {
                                spr.color = new Color(oreLightColor.r, oreLightColor.g, oreLightColor.b, spr.color.a);
                                sprite = Managers.Resource.Load<Sprite>(ReadOnly.Util.Light_SP);
                                if (sprite != null)
                                    spr.sprite = sprite;
                            }
                        }
                        
                        _rockBodyDict[ERockBody.OreLight] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: new Color(spr.color.r, spr.color.g, spr.color.b, 1f), matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.OreLight].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.OreLight].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        if (sprite == null)
                            tr.gameObject.SetActive(false);

                        // --- OreParticle
                        tag = Util.GetStringFromEnum(ERockBody.OreParticle);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        bool isEmptyOreParticleColor = false;

                        if (string.IsNullOrEmpty(rock.OreLightColor) == false)
                        {
                            if (ColorUtility.TryParseHtmlString(rock.OreParticleColor, out Color oreParticleColor))
                            {
                                ParticleSystem ps = tr.GetComponent<ParticleSystem>();
                                var main = ps.main;
                                main.startColor = oreParticleColor;
                                main.maxParticles = rock.OreMaxParticleCount;
                            }

                            ParticleSystemRenderer pr = tr.GetComponent<ParticleSystemRenderer>();
                            pr.material = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_Glow);
                        }
                        else
                            isEmptyOreParticleColor = true;

                        _rockBodyDict[ERockBody.OreParticle] = new BodyContainer(tag: tag, tr: tr, spr: null,
                                                defaultSPRMat: null, defaultSPRColor: Color.white,
                                                defaultMatColor: Color.white, matPB: null);

                        if (isEmptyOreParticleColor)
                            tr.gameObject.SetActive(false);

                        // --- EndParticle
                        tag = Util.GetStringFromEnum(ERockBody.EndParticle);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        ParticleSystemRenderer endPr = tr.GetComponent<ParticleSystemRenderer>();
                        endPr.material = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_RockFragments);

                        _rockBodyDict[ERockBody.EndParticle] = new BodyContainer(tag: tag, tr: tr, spr: null,
                                                defaultSPRMat: null, defaultSPRColor: Color.white,
                                                defaultMatColor: Color.white, matPB: null);

                        // --- Spot1
                        tag = Util.GetStringFromEnum(ERockBody.Spot1);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(rock.Spots[0]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            spr.flipX = rock.SpotsFlipXs[0];
                        }

                        _rockBodyDict[ERockBody.Spot1] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.Spot1].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Spot1].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Spot2
                        tag = Util.GetStringFromEnum(ERockBody.Spot2);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(rock.Spots[1]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            spr.flipX = rock.SpotsFlipXs[1];
                        }

                        _rockBodyDict[ERockBody.Spot2] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.Spot2].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Spot2].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Spot3
                        tag = Util.GetStringFromEnum(ERockBody.Spot3);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(rock.Spots[2]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            spr.flipX = rock.SpotsFlipXs[2];
                        }

                        _rockBodyDict[ERockBody.Spot3] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.Spot3].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Spot3].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Fragment1
                        tag = Util.GetStringFromEnum(ERockBody.Fragment1);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(rock.Fragments[0]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            spr.flipX = rock.FragmentsFlipXs[0];
                        }

                        _rockBodyDict[ERockBody.Fragment1] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.Fragment1].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Fragment1].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Fragment2
                        tag = Util.GetStringFromEnum(ERockBody.Fragment2);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(rock.Fragments[1]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            spr.flipX = rock.FragmentsFlipXs[1];
                        }

                        _rockBodyDict[ERockBody.Fragment2] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.Fragment2].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Fragment2].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Fragment3
                        tag = Util.GetStringFromEnum(ERockBody.Fragment3);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(rock.Fragments[2]);
                        if (sprite != null)
                        {
                            spr.sprite = sprite;
                            spr.flipX = rock.FragmentsFlipXs[2];
                        }

                        _rockBodyDict[ERockBody.Fragment3] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.Fragment3].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Fragment3].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);

                        // --- Shadow
                        tag = Util.GetStringFromEnum(ERockBody.Shadow);
                        tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        spr.material = _matDefault;
                        sprite = Managers.Resource.Load<Sprite>(ReadOnly.Util.Shadow_SP);
                        if (sprite != null)
                            spr.sprite = sprite;

                        _rockBodyDict[ERockBody.Shadow] = new BodyContainer(tag: tag, tr: tr, spr: spr,
                                                defaultSPRMat: _matDefault, defaultSPRColor: spr.color,
                                                defaultMatColor: Color.white, matPB: new MaterialPropertyBlock());

                        matPB = _rockBodyDict[ERockBody.Shadow].MatPropertyBlock;
                        spr.GetPropertyBlock(matPB);
                        matPB.SetColor(_matDefaultColor, _rockBodyDict[ERockBody.Shadow].DefaultMatColor);
                        spr.SetPropertyBlock(matPB);
                    }
                    break;
            }
        }
        #endregion

        #region  Coroutines
        protected override IEnumerator CoHurtFlashEffect()
        {
            ApplyEnvStrongTint(Owner.EnvType, Color.white);
            yield return new WaitForSeconds(0.1f);
            ResetEnvMaterialsAndColors(Owner.EnvType);
        }
        #endregion
    }
}
