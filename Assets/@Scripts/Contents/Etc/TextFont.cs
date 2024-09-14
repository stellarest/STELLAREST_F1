using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // --- TextFont: InGame Font
    // --- TextFontUI: UI Font
    public class TextFont : InitBase
    {
        private RectTransform _rectTr = null;
        private TextMeshPro _text = null;
        private TMP_Text _tmpText = null; // --- For DoTween.DOFade
        private SortingGroup _sortingGroup = null;
        private float _initialFontSize = 0.0f;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _rectTr = GetComponent<RectTransform>();
            _text = GetComponent<TextMeshPro>();
            _tmpText = GetComponent<TMP_Text>();
            _initialFontSize = _text.fontSize;
            _sortingGroup = GetComponent<SortingGroup>();
            _sortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_DamageFont;
            return true;
        }

        #region Show Text
        public void ShowTextFont(Vector3 position, string text, float textSize, Color textColor, 
            EFontAssetType fontAssetType, EFontAnimationType fontAnimType)
        {
            _text.alpha = 1;
            _text.fontSize = textSize;
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
            PlayFontAnimation(fontAnimType: fontAnimType, endCallback: () =>
            {
                Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
            });
        }

        public void ShowTextFont(Vector3 position, string text, float textSize, string textColorCode,
            EFontAssetType fontAssetType, EFontAnimationType fontAnimType)
        {
            _text.alpha = 1;
            _text.fontSize = textSize;

            if (ColorUtility.TryParseHtmlString(textColorCode, out Color textColor))
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
            PlayFontAnimation(fontAnimType: fontAnimType, endCallback: () =>
            {
                Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
            });
        }
        #endregion

        #region Show Damage
        public void ShowDamageFont(Vector3 position, float damage, Color textColor, bool isCritical = false,
                        EFontSignType fontSignType = EFontSignType.None,
                        EFontAnimationType fontAnimType = EFontAnimationType.EndGoingUp)
        {
            damage = Mathf.Abs(Mathf.Round(damage));
            _text.alpha = 1;
            _text.fontSize = _initialFontSize;
            if (isCritical)
            {
                Debug.Log("<color=red>CRITICAL FONT</color>");
                _text.font = Managers.MonoContents.GetFontAsset(EFontAssetType.Comic);
                _text.fontSize *= 1 + 0.5f; // +50%
            }
            else
            {
                Debug.Log("<color=white>NORMAL FONT</color>");
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

            PlayFontAnimation(fontAnimType: fontAnimType, endCallback: () => {
                Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
            });
        }

        public void ShowDamageFont(Vector3 position, float damage, string textColorCode, bool isCritical = false,
                        EFontSignType fontSignType = EFontSignType.None,
                        EFontAnimationType fontAnimType = EFontAnimationType.EndGoingUp)
        {
            damage = Mathf.Abs(Mathf.Round(damage));
            _text.alpha = 1;
            _text.fontSize = _initialFontSize;
            if (isCritical)
            {
                Debug.Log("<color=red>CRITICAL FONT</color>");
                _text.font = Managers.MonoContents.GetFontAsset(EFontAssetType.Comic);
                _text.fontSize *= 1 + 0.5f; // +50%
            }
            else
            {
                Debug.Log("<color=white>NORMAL FONT</color>");
                _text.font = Managers.MonoContents.GetFontAsset(EFontAssetType.MapleBold);
            }

            if (ColorUtility.TryParseHtmlString(textColorCode, out Color textColor))
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

            PlayFontAnimation(fontAnimType: fontAnimType, endCallback: () => {
                Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
            });
        }
        #endregion

        #region Font Animation
        private void PlayFontAnimation(EFontAnimationType fontAnimType, System.Action endCallback = null)
        {
            switch (fontAnimType)
            {
                case EFontAnimationType.EndGoingUp:
                    FontAnimEndGoingUp(endCallback);
                    break;

                case EFontAnimationType.EndSmaller:
                    FontAnimEndSmaller(endCallback);
                    break;

                case EFontAnimationType.EndFalling:
                    FontAnimEndFalling(endCallback);
                    break;

                case EFontAnimationType.EndFallingShake:
                    FontAnimEndFallingShake(endCallback);
                    break;

                case EFontAnimationType.EndBouncingLeftUp:
                    FontAnimEndBouncingLeftUp(endCallback);
                    break;

                case EFontAnimationType.EndBouncingRightUp:
                    FontAnimEndBouncingRightUp(endCallback);
                    break;
            }
        }

        private void FontAnimEndGoingUp(System.Action endCallback = null)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(_rectTr.DOScale(endValue: 1.3f, duration: 0.1f)).SetEase(Ease.Linear)
                  .Join(_rectTr.DOMove(endValue: _rectTr.position + Vector3.up, duration: 0.2f)).SetEase(Ease.Linear)
                  .Join(_tmpText.DOFade(endValue: 0, duration: 0.5f)).SetEase(Ease.InQuint)
                  .OnComplete(() =>
                  {
                      endCallback?.Invoke();
                  });

            seq.Play();
        }

        private void FontAnimEndSmaller(System.Action endCallback = null)
        {
            _rectTr.localScale = Vector3.one * 0.1f;

            Sequence seq = DOTween.Sequence();
            seq.Append(_rectTr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack))
               .Join(_rectTr.DOMove(_rectTr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack));

            seq.Append(_rectTr.DOMove(_rectTr.position, 0.65f).SetEase(Ease.InBack))
               .Join(_rectTr.DOScale(0f, 0.4f).SetEase(Ease.InQuint))
               .Append(_tmpText.DOFade(0, 0.4f).SetEase(Ease.Linear).SetDelay(0.2f));

            seq.OnComplete(() =>
            {
                endCallback?.Invoke();
            });

            seq.Play();
        }

        private void FontAnimEndFalling(System.Action endCallback = null)
        {
            _rectTr.localScale = Vector3.one * 0.1f;

            Sequence seq = DOTween.Sequence();
            seq.Append(_rectTr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack))
               .Join(_rectTr.DOMove(_rectTr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack));

            seq.Append(_rectTr.DOMove(_rectTr.position - Vector3.up * 0.5f, 0.3f).SetEase(Ease.InExpo))
               .Join(_rectTr.DOScale(0f, 0.3f).SetEase(Ease.InExpo))
               .Join(_tmpText.DOFade(0, 0.3f).SetEase(Ease.Linear));

            seq.OnComplete(() =>
            {
                endCallback?.Invoke();
            });

            seq.Play();
        }

        private void FontAnimEndFallingShake(System.Action endCallback = null)
        {
            _rectTr.localScale = Vector3.one * 0.1f;

            Sequence seq = DOTween.Sequence();
            seq.Append(_rectTr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack))
               .Append(_rectTr.DOShakePosition(duration: 0.3f, strength: Vector3.right, vibrato: 10, randomness: 0, snapping: false, fadeOut: false))
               .Join(_rectTr.DOMove(_rectTr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack));

            seq.Append(_rectTr.DOMove(_rectTr.position - Vector3.up * 0.5f, 0.3f).SetEase(Ease.InExpo))
               .Join(_rectTr.DOScale(0f, 0.3f).SetEase(Ease.InExpo))
               .Join(_tmpText.DOFade(0, 0.3f).SetEase(Ease.Linear));

            seq.OnComplete(() =>
            {
                endCallback?.Invoke();
            });

            seq.Play();
        }

        private void FontAnimEndBouncingLeftUp(System.Action endCallback = null)
        {
            float randomUpDuration = Random.Range(0.3f, 0.5f);
            float randomDownDuration = Random.Range(0.2f, 0.4f);
            _rectTr.localScale = Vector3.one * 0.1f;
            Vector3[] pathPoints = new Vector3[]
            {
                    _rectTr.position,
                    _rectTr.position + new Vector3(-1.0f, 2.0f, 0f),
                    _rectTr.position + new Vector3(-2.0f, -0.5f, 0f)
            };

            Sequence seq = DOTween.Sequence();

            seq.Append(_rectTr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc))
               .Join(_rectTr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc));

            seq.Append(_rectTr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc))
               .Join(_tmpText.DOFade(0, randomDownDuration).SetEase(Ease.Linear));

            seq.OnComplete(() =>
            {
                endCallback?.Invoke();
            });

            seq.Play();
        }

        private void FontAnimEndBouncingRightUp(System.Action endCallback = null)
        {
            float randomUpDuration = Random.Range(0.3f, 0.5f);
            float randomDownDuration = Random.Range(0.2f, 0.4f);
            _rectTr.localScale = Vector3.one * 0.1f;
            Vector3[] pathPoints = new Vector3[]
            {
                    _rectTr.position,
                    _rectTr.position + new Vector3(1.0f, 2.0f, 0f),
                    _rectTr.position + new Vector3(2.0f, -0.5f, 0f)
            };

            Sequence seq = DOTween.Sequence();

            seq.Append(_rectTr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc))
               .Join(_rectTr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc));

            seq.Append(_rectTr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc))
               .Join(_tmpText.DOFade(0, randomDownDuration).SetEase(Ease.Linear));

            seq.OnComplete(() =>
            {
                endCallback?.Invoke();
            });

            seq.Play();
        }
        #endregion
    }
}
