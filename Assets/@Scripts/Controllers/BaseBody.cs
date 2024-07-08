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
        public void ChangeDefaultMatColor(Color defaultMatColor) => DefaultMatColor = defaultMatColor;
    }

    public class BaseBody : InitBase
    {
        //public int DataTemplateID { get; protected set; } = -1;

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

        public void StartCoHurtFlashEffect()
        {
            StartCoroutine(CoHurtFlashEffect());
        }

        protected virtual IEnumerator CoHurtFlashEffect() { yield return null; }
    }
}
