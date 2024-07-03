using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BodyContainer
    {
        public BodyContainer(string tag, Transform tr, SpriteRenderer spr, Material defaultMat)
        {
            this.Tag = tag;
            this.TR = tr;
            this.SPR = spr;
            this.DefaultMat = defaultMat;
        }

        public string Tag { get; private set; } = null;
        public Transform TR { get; private set; } = null;
        public SpriteRenderer SPR { get; private set; } = null;
        public Material DefaultMat { get; private set; } = null;

        // --- TODO
        public Color DefaultColor { get; private set; } = Color.white;
        public void SetColor(Color color) => DefaultColor = color;
    }

    public class BaseBody : InitBase
    {
        protected Material _matDefault = null;
        protected Material _matStrongTint = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _matDefault = Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_Default);
            _matStrongTint =  Managers.Resource.Load<Material>(ReadOnly.Materials.Mat_StrongTint);

            return true;
        }

        public void StartCoHurtFlashEffect()
        {
            StartCoroutine(CoHurtFlashEffect());
        }

        protected virtual IEnumerator CoHurtFlashEffect() { yield return null; }
    }
}
