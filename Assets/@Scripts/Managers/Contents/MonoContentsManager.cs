using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // Animation Curve, VFX...
    // --- 임시로 박아놓는 모노 매니저
    public class MonoContentsManager : MonoBehaviour
    {
        private TMP_FontAsset[] _fontAssets = null;
        public static MonoContentsManager Instance = null;
        [SerializeField] private AnimationCurve[] _curves = new AnimationCurve[(int)EAnimationCurveType.Max];

        // --- Color Red
        public Color BrightRed { get; } = new Color(r: 238.0f / 255.0f, g: 75.0f / 255.0f, b: 43.0f / 255.0f, a: 1.0f);

        // --- Color Blue
        public Color BrightBlue { get; } = new Color(r: 0.0f, g: 150.0f / 255.0f, b: 1.0f, a: 1.0f);
        public Color NeonBlue { get; } = new Color(r: 31.0f / 255.0f, g: 81.0f / 255.0f, b: 1.0f, a: 1.0f);

        public AnimationCurve Curve(EAnimationCurveType curveType)
        {
            if (curveType == EAnimationCurveType.None || curveType == EAnimationCurveType.Max)
                return _curves[(int)EAnimationCurveType.Linear];

            return _curves[(int)curveType];
        }
        private void Awake()
        {
            Instance = this;
        }

        private const string c_FontMapleBold = "FontMapleBold";
        private const string c_FontComic = "FontComic";
        public void Init()
        {
            _fontAssets = new TMP_FontAsset[(int)EFontAssetType.Max];
            _fontAssets[(int)EFontAssetType.MapleBold] = Managers.Resource.Load<TMP_FontAsset>(c_FontMapleBold);
            _fontAssets[(int)EFontAssetType.Comic] = Managers.Resource.Load<TMP_FontAsset>(c_FontComic);
        }

        public TMP_FontAsset GetFontAsset(EFontAssetType fontAssetType)
            => _fontAssets[(int)fontAssetType];

        #region PREV
        // public void PlayDOTWeenEndAnimation(GameObject go, RectTransform rectTr, EFontAnimationType endAnimType, 
        //     System.Action endCallback = null)
        // {
        //     switch (endAnimType)
        //     {
        //         case EFontAnimationType.EndGoingUp:
        //             EndGoingUp(go, rectTr, endCallback);
        //             break;

        //         case EFontAnimationType.EndSmaller:
        //             EndSmaller(go, rectTr, endCallback);
        //             break;

        //         case EFontAnimationType.EndFalling:
        //             EndFalling(go, rectTr, endCallback);
        //             break;

        //         case EFontAnimationType.EndFallingShake:
        //             EndFallingShake(go, rectTr, endCallback);
        //             break;

        //         case EFontAnimationType.EndBouncingLeftUp:
        //             EndBouncingLeftUp(go, rectTr, endCallback);
        //             break;

        //         case EFontAnimationType.EndBouncingRightUp:
        //             EndBouncingRightUp(go, rectTr, endCallback);
        //             break;
        //     }
        // }

        // private void EndGoingUp(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        // {
        //     Sequence seq = DOTween.Sequence();
        //     if (rectTr == null)
        //     {
        //         Transform tr = go.transform;
        //         seq.Append(rectTr.DOScale(endValue: 1.3f, duration: 0.1f)).SetEase(Ease.Linear)
        //            .Join(rectTr.DOMove(endValue: tr.position + Vector3.up, duration: 0.2f)).SetEase(Ease.Linear)
        //            .Join(rectTr.GetComponent<TMP_Text>().DOFade(endValue: 0, duration: 0.5f)).SetEase(Ease.InQuint)
        //            .OnComplete(() =>
        //             {
        //                 endCallback?.Invoke();
        //             });
        //     }
        //     else
        //     {
        //         seq.Append(rectTr.DOScale(endValue: 1.3f, duration: 0.1f)).SetEase(Ease.Linear)
        //            .Join(rectTr.DOMove(endValue: rectTr.position + Vector3.up, duration: 0.2f)).SetEase(Ease.Linear)
        //            .Join(rectTr.GetComponent<TMP_Text>().DOFade(endValue: 0, duration: 0.5f)).SetEase(Ease.InQuint)
        //            .OnComplete(() =>
        //            {
        //                endCallback?.Invoke();
        //            });
        //     }

        //     seq.Play();
        // }

        // private void EndSmaller(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        // {
        //     Sequence seq = DOTween.Sequence();
        //     if (rectTr == null)
        //     {
        //         Transform tr = go.transform;
        //         tr.localScale = Vector3.one * 0.1f;

        //         seq.Append(tr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack))
        //            .Join(tr.DOMove(tr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack));

        //         seq.Append(tr.DOMove(tr.position, 0.65f).SetEase(Ease.InBack))
        //            .Join(tr.DOScale(0f, 0.4f).SetEase(Ease.InQuint))

        //            .Append(tr.GetComponent<TMP_Text>().DOFade(0, 0.4f).SetEase(Ease.Linear).SetDelay(0.2f));

        //         seq.OnComplete(() =>
        //         {
        //             endCallback?.Invoke();
        //         });
        //     }
        //     else
        //     {
        //         rectTr.localScale = Vector3.one * 0.1f;

        //         seq.Append(rectTr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack))
        //            .Join(rectTr.DOMove(rectTr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack));

        //         seq.Append(rectTr.DOMove(rectTr.position, 0.65f).SetEase(Ease.InBack))
        //            .Join(rectTr.DOScale(0f, 0.4f).SetEase(Ease.InQuint))

        //            .Append(rectTr.GetComponent<TMP_Text>().DOFade(0, 0.4f).SetEase(Ease.Linear).SetDelay(0.2f));

        //         seq.OnComplete(() =>
        //         {
        //             endCallback?.Invoke();
        //         });
        //     }

        //     seq.Play();
        // }

        // private void EndFalling(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        // {
        //     Sequence seq = DOTween.Sequence();
        //     if (rectTr == null)
        //     {
        //         Transform tr = go.transform;
        //         tr.localScale = Vector3.one * 0.1f;

        //         seq.Append(tr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack))
        //            .Join(tr.DOMove(tr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack));

        //         seq.Append(tr.DOMove(tr.position - Vector3.up * 0.5f, 0.3f).SetEase(Ease.InExpo))
        //            .Join(tr.DOScale(0f, 0.3f).SetEase(Ease.InExpo))
        //            .Join(GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.Linear));

        //         seq.OnComplete(() =>
        //         {
        //             endCallback?.Invoke();
        //         });
        //     }
        //     else
        //     {
        //         rectTr.localScale = Vector3.one * 0.1f;

        //         seq.Append(rectTr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack))
        //            .Join(rectTr.DOMove(rectTr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack));

        //         seq.Append(rectTr.DOMove(rectTr.position - Vector3.up * 0.5f, 0.3f).SetEase(Ease.InExpo))
        //            .Join(rectTr.DOScale(0f, 0.3f).SetEase(Ease.InExpo))
        //            .Join(rectTr.GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.Linear));

        //         seq.OnComplete(() =>
        //         {
        //             endCallback?.Invoke();
        //         });
        //     }

        //     seq.Play();
        // }

        // private void EndFallingShake(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        // {
        //     Sequence seq = DOTween.Sequence();
        //     if (rectTr == null)
        //     {
        //         Transform tr = go.transform;
        //         tr.localScale = Vector3.one * 0.1f;

        //         seq.Append(tr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack))
        //            .Join(tr.DOMove(tr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack));

        //         seq.Append(tr.DOMove(tr.position - Vector3.up * 0.5f, 0.3f).SetEase(Ease.InExpo))
        //            .Join(tr.DOScale(0f, 0.3f).SetEase(Ease.InExpo))
        //            .Join(GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.Linear));

        //         seq.OnComplete(() =>
        //         {
        //             endCallback?.Invoke();
        //         });
        //     }
        //     else
        //     {
        //         rectTr.localScale = Vector3.one * 0.1f;

        //         seq.Append(rectTr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack))
        //            .Append(rectTr.DOShakePosition(duration: 0.3f, strength: Vector3.right, vibrato: 10, randomness: 0, snapping: false, fadeOut: false))
        //            .Join(rectTr.DOMove(rectTr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack));

        //         seq.Append(rectTr.DOMove(rectTr.position - Vector3.up * 0.5f, 0.3f).SetEase(Ease.InExpo))
        //            .Join(rectTr.DOScale(0f, 0.3f).SetEase(Ease.InExpo))
        //            .Join(rectTr.GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.Linear));

        //         seq.OnComplete(() =>
        //         {
        //             endCallback?.Invoke();
        //         });
        //     }

        //     seq.Play();
        // }

        // private void EndBouncingLeftUp(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        // {
        //     Sequence seq = DOTween.Sequence();
        //     float randomUpDuration = Random.Range(0.3f, 0.5f);
        //     float randomDownDuration = Random.Range(0.2f, 0.4f);
        //     if (rectTr == null)
        //     {
        //         Transform tr = go.transform;
        //         tr.localScale = Vector3.one * 0.1f;

        //         Vector3[] pathPoints = new Vector3[]
        //         {
        //             tr.position,
        //             tr.position + new Vector3(-1.0f, 2.0f, 0f),
        //             tr.position + new Vector3(-2.0f, -0.5f, 0f)
        //         };

        //         seq.Append(tr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc))
        //            .Join(tr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc));

        //         seq.Append(tr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc))
        //            .Join(tr.GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear));

        //         seq.OnComplete(() =>
        //         {
        //              endCallback?.Invoke();
        //         });
        //     }
        //     else
        //     {
        //         rectTr.localScale = Vector3.one * 0.1f;

        //         Vector3[] pathPoints = new Vector3[]
        //         {
        //             rectTr.position,
        //             rectTr.position + new Vector3(-1.0f, 2.0f, 0f),
        //             rectTr.position + new Vector3(-2.0f, -0.5f, 0f)
        //         };

        //         seq.Append(rectTr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc))
        //            .Join(rectTr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc));

        //         seq.Append(rectTr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc))
        //            .Join(rectTr.GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear));

        //         seq.OnComplete(() =>
        //         {
        //              endCallback?.Invoke();
        //         });
        //     }

        //     seq.Play();
        // }

        // private void EndBouncingRightUp(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        // {
        //     Sequence seq = DOTween.Sequence();
        //     float randomUpDuration = Random.Range(0.3f, 0.5f);
        //     float randomDownDuration = Random.Range(0.2f, 0.4f);
        //     if (rectTr == null)
        //     {
        //         Transform tr = go.transform;
        //         tr.localScale = Vector3.one * 0.1f;

        //         Vector3[] pathPoints = new Vector3[]
        //         {
        //             tr.position,
        //             tr.position + new Vector3(1.0f, 2.0f, 0f),
        //             tr.position + new Vector3(2.0f, -0.5f, 0f)
        //         };

        //         seq.Append(tr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc))
        //            .Join(tr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc));

        //         seq.Append(tr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc))
        //            .Join(tr.GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear));

        //         seq.OnComplete(() =>
        //         {
        //              endCallback?.Invoke();
        //         });
        //     }
        //     else
        //     {
        //         rectTr.localScale = Vector3.one * 0.1f;

        //         Vector3[] pathPoints = new Vector3[]
        //         {
        //             rectTr.position,
        //             rectTr.position + new Vector3(1.0f, 2.0f, 0f),
        //             rectTr.position + new Vector3(2.0f, -0.5f, 0f)
        //         };

        //         seq.Append(rectTr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc))
        //            .Join(rectTr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc));

        //         seq.Append(rectTr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc))
        //            .Join(rectTr.GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear));

        //         seq.OnComplete(() =>
        //         {
        //              endCallback?.Invoke();
        //         });
        //     }

        //     seq.Play();
        // }
        #endregion
    }
}

// private const int _tryMaxSpawnCount = 999;
// public Vector3Int Spawn(Vector3 spawnPos)
// {
//     Vector3Int cellSpawnPos = Managers.Map.WorldToCell(spawnPos);

//     int randMin = 0;
//     int randMax = 0;
//     while (Managers.Map.CanMove(cellSpawnPos) == false)
//     {
//         int x = Random.Range(--randMin, ++randMax);
//         int y = Random.Range(--randMin, ++randMax);
//         //cellSpawnPos = Managers.Map.WorldToCell()
//     }

//     return cellSpawnPos;
// }