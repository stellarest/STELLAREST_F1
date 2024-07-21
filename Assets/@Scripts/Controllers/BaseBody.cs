using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BodyContainer
    {
        public BodyContainer(string tag, Transform tr, SpriteRenderer spr, Material defaultSPRMat, 
                                Color defaultSPRColor, Color defaultMatColor, MaterialPropertyBlock matPB)
        {
            this.Tag = tag;
            this.TR = tr;
            this.SPR = spr;
            this.DefaultSPRMat = defaultSPRMat;
            this.DefaultSPRColor = defaultSPRColor;
            this.DefaultMatColor = defaultMatColor;
            MatPropertyBlock = matPB;
        }

        public string Tag { get; private set; } = null;
        public Transform TR { get; private set; } = null;
        public SpriteRenderer SPR { get; private set; } = null;
        public Material DefaultSPRMat { get; } = null;
        public Color DefaultSPRColor { get; private set; } = Color.white;
        public Color DefaultMatColor { get; private set; } = Color.white;
        public MaterialPropertyBlock MatPropertyBlock { get; } = null;

        public void ChangeDefaultSPRColor(Color defaultSPRColor) => DefaultSPRColor = defaultSPRColor;
        // public void ChangeDefaultMatColor(Color defaultMatColor) => DefaultMatColor = defaultMatColor;
    }

    public class BaseBody : InitBase
    {
        protected Material _matDefault = null;
        protected const string _matDefaultColor = "_Color";

        protected Material _matStrongTint = null;
        protected const string _matStrongTintColor = "_Color";

        protected MaterialPropertyBlock _matPropertyBlock = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _matDefault = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_Default);
            _matStrongTint =  Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_StrongTint);
            _matPropertyBlock = new MaterialPropertyBlock();

            return true;
        }

        public virtual void InitialSetInfo(int dataID, BaseObject owner) { }
        // --- Mat: Default
        protected virtual void ApplyDefaultMat_Alpha(float alphaValue){ }

        // --- Mat: Strong Tint
        protected virtual void ApplyStrongTintMat_Color(Color desiredColor) { }

        // --- Reset
        public virtual void ResetMaterialsAndColors() { }

        // --- Effect: Hurt Flash
        public void StartCoHurtFlashEffect(bool isCritical = false) => StartCoroutine(CoHurtFlashEffect(isCritical));
        protected virtual IEnumerator CoHurtFlashEffect(bool isCritical = false)
        {
            if (isCritical == false)
                ApplyStrongTintMat_Color(Color.white);
            else
                ApplyStrongTintMat_Color(Color.red);
            yield return new WaitForSeconds(0.1f);
            ResetMaterialsAndColors();
        }

        // --- Effect: Fade In
        public void StartCoFadeInEffect(System.Action startCallback = null, System.Action endCallback = null)
        {
            startCallback?.Invoke();
            StartCoroutine(CoFadeInEffect(endCallback));
        }
        private IEnumerator CoFadeInEffect(System.Action endCallback = null)
        {
            ApplyDefaultMat_Alpha(0f);
            yield return null;
            float delta = 0f;
            float percent = 0f;
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / ReadOnly.Util.DesiredEndFadeInTime;
                ApplyDefaultMat_Alpha(percent);
                yield return null;
            }

            ApplyDefaultMat_Alpha(1f);
            endCallback?.Invoke();
        }

        // --- Effect: Fade Out
        public void StartCoFadeOutEffect(System.Action startCallback = null, System.Action endCallback = null)
        {
            ResetMaterialsAndColors();
            startCallback?.Invoke();
            StartCoroutine(CoFadeOutEffect(endCallback));
        }
        private IEnumerator CoFadeOutEffect(System.Action endCallback = null)
        {
            yield return new WaitForSeconds(ReadOnly.Util.DesiredStartFadeOutTime);

            float delta = 0f;
            float percent = 1f;
            while (percent >= 0f)
            {
                delta += Time.deltaTime;
                percent = 1f - (delta / ReadOnly.Util.DesiredEndFadeOutTime);
                ApplyDefaultMat_Alpha(percent);
                yield return null;
            }

            ApplyDefaultMat_Alpha(0f);
            endCallback?.Invoke();
        }
    }
}
