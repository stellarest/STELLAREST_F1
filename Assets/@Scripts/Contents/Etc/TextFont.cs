using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class TextFont : InitBase
    {
        private RectTransform _rectTransform = null;
        private TextMeshPro _text = null;
        private SortingGroup _sortingGroup = null;
        private float _initialFontSize = 0.0f;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _rectTransform = GetComponent<RectTransform>();
            _text = GetComponent<TextMeshPro>();
            _initialFontSize = _text.fontSize;
            _sortingGroup = GetComponent<SortingGroup>();
            _sortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_DamageFont;
            return true;
        }

        public void ShowTextFont(Vector3 position, string text, float textSize, Color textColor, EFontAssetType fontAssetType, EFontAnimationType fontAnimType)
        {
            _text.alpha = 1;
            _text.fontSize = _initialFontSize;
            _text.color = textColor;
            switch (fontAssetType)
            {
                case EFontAssetType.MapleBold:
                    _text.font = Managers.MonoContents.GetFontAsset(EFontAssetType.MapleBold);
                    break;

                case EFontAssetType.Comic:
                    _text.font = Managers.MonoContents.GetFontAsset(EFontAssetType.Comic);
                    break;
            }

            _text.text = text;
            transform.position = position;
            Managers.MonoContents.PlayInGameFontAnimation(this.gameObject, _rectTransform, fontAnimType, () => {
                 Managers.Resource.Destroy(_rectTransform.gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
            });
        }

        public void ShowDamageFont(Vector3 position, float damage, Color textColor, bool isCritical = false,
                        EFontSignType fontSignType = EFontSignType.None,
                        EFontAnimationType fontAnimType = EFontAnimationType.GoingUp)
        {
            damage = Mathf.Abs(Mathf.Round(damage));
            _text.alpha = 1;
            _text.fontSize = _initialFontSize;
            if (isCritical)
            {
                Debug.Log("<color=red>CRITICAL</color>");
                _text.font = Managers.MonoContents.GetFontAsset(EFontAssetType.Comic);
                _text.fontSize *= 1 + 0.8f; // +80%
            }
            else
            {
                Debug.Log("<color=white>NORMAL</color>");
                _text.font = Managers.MonoContents.GetFontAsset(EFontAssetType.MapleBold);
            }

            _text.color = textColor;
            transform.position = position;
            switch (fontSignType)
            {
                case EFontSignType.Plus:
                    _text.text = $"+{damage}";
                    break;

                case EFontSignType.Minus:
                    _text.text = $"-{damage}";
                    break;

                default:
                    _text.text = $"{damage}";
                    break;
            }

            Managers.MonoContents.PlayInGameFontAnimation(this.gameObject, _rectTransform, fontAnimType, () => 
            {
                Managers.Resource.Destroy(_rectTransform.gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
            });
        }

        // public void ShowDamageFont(Vector3 position, float damage, string colorCode, bool isCritical = false,
        //                 EFontSignType fontSignType = EFontSignType.None,
        //                 EFontAnimationType fontAnimType = EFontAnimationType.GoingUp)
        // {
        //     damage = Mathf.Abs(Mathf.Round(damage));
        //     _text.fontSize = _initialFontSize;
        //     if (isCritical)
        //     {
        //         Debug.Log("<color=red>CRITICAL</color>");
        //         _text.font = Managers.MonoContents.GetFontAsset(EFontAssetType.Comic);
        //         _text.fontSize *= 1 + 0.6f;
        //     }
        //     else
        //     {
        //         Debug.Log("<color=white>NORMAL</color>");
        //         _text.font = Managers.MonoContents.GetFontAsset(EFontAssetType.MapleBold);
        //     }

        //     Color textColor = Color.white;
        //     if (ColorUtility.TryParseHtmlString(colorCode, out textColor))
        //         _text.color = textColor;
        //     transform.position = position;
        //     switch (fontSignType)
        //     {
        //         case EFontSignType.Plus:
        //             _text.text = $"+{damage}";
        //             break;

        //         case EFontSignType.Minus:
        //             _text.text = $"-{damage}";
        //             break;

        //         default:
        //             _text.text = $"{damage}";
        //             break;
        //     }

        //     _text.alpha = 1;
        //     Managers.MonoContents.PlayInGameFontAnimation(this.gameObject, _rectTransform, fontAnimType);
        // }
    }
}
