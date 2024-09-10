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
        public void InitFontTypes()
        {
            _fontAssets = new TMP_FontAsset[(int)EFontAssetType.Max];
            _fontAssets[(int)EFontAssetType.MapleBold] = Managers.Resource.Load<TMP_FontAsset>(c_FontMapleBold);
            _fontAssets[(int)EFontAssetType.Comic] = Managers.Resource.Load<TMP_FontAsset>(c_FontComic);
        }

        public TMP_FontAsset GetFontAsset(EFontAssetType fontAssetType)
            => _fontAssets[(int)fontAssetType];

        public void PlayInGameFontAnimation(GameObject go, RectTransform rectTr, EFontAnimationType fontAnimType, System.Action endCallback = null)
        {
            switch (fontAnimType)
            {
                case EFontAnimationType.GoingUp:
                    FontGoingUp(go, rectTr, endCallback);
                    break;

                case EFontAnimationType.Smaller:
                    FontSmaller(go, rectTr, endCallback);
                    break;

                case EFontAnimationType.Falling:
                    FontFalling(go, rectTr, endCallback);
                    break;

                case EFontAnimationType.BouncingLeftUp:
                    FontBouncingLeftUp(go, rectTr, endCallback);
                    break;

                case EFontAnimationType.BouncingRightUp:
                    FontBouncingRightUp(go, rectTr, endCallback);
                    break;
            }
        }

        private void FontGoingUp(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        {
            Sequence seq = DOTween.Sequence();
            if (rectTr == null)
            {
                Transform tr = go.transform;
                seq.Append(rectTr.DOScale(endValue: 1.3f, duration: 0.1f)).SetEase(Ease.Linear)
                   .Join(rectTr.DOMove(endValue: tr.position + Vector3.up, duration: 0.2f)).SetEase(Ease.Linear)
                   .Join(rectTr.GetComponent<TMP_Text>().DOFade(endValue: 0, duration: 0.5f)).SetEase(Ease.InQuint)
                   .OnComplete(() =>
                    {
                        Managers.Resource.Destroy(go, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                    });
            }
            else
            {
                seq.Append(rectTr.DOScale(endValue: 1.3f, duration: 0.1f)).SetEase(Ease.Linear)
               .Join(rectTr.DOMove(endValue: rectTr.position + Vector3.up, duration: 0.2f)).SetEase(Ease.Linear)
               .Join(rectTr.GetComponent<TMP_Text>().DOFade(endValue: 0, duration: 0.5f)).SetEase(Ease.InQuint)
               .OnComplete(() =>
               {
                   // Managers.Resource.Destroy(rectTr.gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                   endCallback?.Invoke();
               });
            }

            seq.Play();
        }

        private void FontSmaller(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        {
            Sequence seq = DOTween.Sequence();
            if (rectTr == null)
            {
                Transform tr = go.transform;
                tr.localScale = Vector3.one * 0.1f;

                seq.Append(tr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack)) // scale 증가 (0.1 -> 1.5)
                   .Join(tr.DOMove(tr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack)); // 위로 살짝 튀는 동작

                seq.Append(tr.DOMove(tr.position, 0.65f).SetEase(Ease.InBack)) // 자연스럽게 아래로 튕겨 내려오는 동작 (0.65초)
                   .Join(tr.DOScale(0f, 0.4f).SetEase(Ease.InQuint)) // scale 감소 (1.5 -> 0, 0.4초)

                   .Append(tr.GetComponent<TMP_Text>().DOFade(0, 0.4f).SetEase(Ease.Linear).SetDelay(0.2f)); // 페이드아웃 (0.4초, 약간 지연 후 시작)

                seq.OnComplete(() =>
                {
                    // Managers.Resource.Destroy(tr.gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                    endCallback?.Invoke();
                });
            }
            else
            {
                rectTr.localScale = Vector3.one * 0.1f;

                seq.Append(rectTr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack)) // scale 증가 (0.1 -> 1.5)
                   .Join(rectTr.DOMove(rectTr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack)); // 위로 살짝 튀는 동작

                seq.Append(rectTr.DOMove(rectTr.position, 0.65f).SetEase(Ease.InBack)) // 자연스럽게 아래로 튕겨 내려오는 동작 (0.65초)
                   .Join(rectTr.DOScale(0f, 0.4f).SetEase(Ease.InQuint)) // scale 감소 (1.5 -> 0, 0.4초)

                   .Append(rectTr.GetComponent<TMP_Text>().DOFade(0, 0.4f).SetEase(Ease.Linear).SetDelay(0.2f)); // 페이드아웃 (0.4초, 약간 지연 후 시작)

                seq.OnComplete(() =>
                {
                    // Managers.Resource.Destroy(rectTr.gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                    endCallback?.Invoke();
                });
            }

            seq.Play();
        }

        private void FontFalling(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        {
            Sequence seq = DOTween.Sequence();
            if (rectTr == null)
            {
                Transform tr = go.transform;
                tr.localScale = Vector3.one * 0.1f;

                seq.Append(tr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack)) // scale 증가 (0.1 -> 1.5)
                   .Join(tr.DOMove(tr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack)); // 위로 살짝 튀는 동작

                seq.Append(tr.DOMove(tr.position - Vector3.up * 0.5f, 0.3f).SetEase(Ease.InExpo)) // 급격히 아래로 떨어짐 (0.3초)
                   .Join(tr.DOScale(0f, 0.3f).SetEase(Ease.InExpo)) // 빠르게 크기 감소 (1.5 -> 0, 0.3초)
                   .Join(GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.Linear)); // 빠르게 페이드아웃 (0.3초)

                seq.OnComplete(() =>
                {
                    // Managers.Resource.Destroy(rectTr.gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                    endCallback?.Invoke();
                });
            }
            else
            {
                rectTr.localScale = Vector3.one * 0.1f;

                seq.Append(rectTr.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack)) // scale 증가 (0.1 -> 1.5)
                   .Join(rectTr.DOMove(rectTr.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack)); // 위로 살짝 튀는 동작

                seq.Append(rectTr.DOMove(rectTr.position - Vector3.up * 0.5f, 0.3f).SetEase(Ease.InExpo)) // 급격히 아래로 떨어짐 (0.3초)
                   .Join(rectTr.DOScale(0f, 0.3f).SetEase(Ease.InExpo)) // 빠르게 크기 감소 (1.5 -> 0, 0.3초)
                   .Join(rectTr.GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.Linear)); // 빠르게 페이드아웃 (0.3초)

                seq.OnComplete(() =>
                {
                    // Managers.Resource.Destroy(rectTr.gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                    endCallback?.Invoke();
                });
            }

            seq.Play();
        }

        private void FontBouncingLeftUp(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        {
            Sequence seq = DOTween.Sequence();
            float randomUpDuration = Random.Range(0.3f, 0.5f);
            float randomDownDuration = Random.Range(0.2f, 0.4f);
            if (rectTr == null)
            {
                Transform tr = go.transform;
                tr.localScale = Vector3.one * 0.1f;

                Vector3[] pathPoints = new Vector3[]
                {
                    tr.position,
                    tr.position + new Vector3(-1.0f, 2.0f, 0f),
                    tr.position + new Vector3(-2.0f, -0.5f, 0f)
                };

                seq.Append(tr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc)) // scale 증가 (0.1 -> 1.5)
                   .Join(tr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc)); // 곡선을 따라 이동

                seq.Append(tr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc)) // 빠르게 크기 감소 (1.5 -> 0)
                   .Join(tr.GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear)); // 페이드아웃

                seq.OnComplete(() =>
                {
                    Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                });
            }
            else
            {
                rectTr.localScale = Vector3.one * 0.1f;

                Vector3[] pathPoints = new Vector3[]
                {
                    rectTr.position,
                    rectTr.position + new Vector3(-1.0f, 2.0f, 0f),
                    rectTr.position + new Vector3(-2.0f, -0.5f, 0f)
                };

                seq.Append(rectTr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc)) // scale 증가 (0.1 -> 1.5)
                   .Join(rectTr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc)); // 곡선을 따라 이동

                seq.Append(rectTr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc)) // 빠르게 크기 감소 (1.5 -> 0)
                   .Join(rectTr.GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear)); // 페이드아웃

                seq.OnComplete(() =>
                {
                    Managers.Resource.Destroy(rectTr.gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                });
            }

            seq.Play();
        }

        private void FontBouncingRightUp(GameObject go, RectTransform rectTr, System.Action endCallback = null)
        {
            Debug.Log("FontBouncingRightUp");
            Sequence seq = DOTween.Sequence();
            float randomUpDuration = Random.Range(0.3f, 0.5f);
            float randomDownDuration = Random.Range(0.2f, 0.4f);
            if (rectTr == null)
            {
                Transform tr = go.transform;
                tr.localScale = Vector3.one * 0.1f;

                Vector3[] pathPoints = new Vector3[]
                {
                    tr.position,
                    tr.position + new Vector3(1.0f, 2.0f, 0f),
                    tr.position + new Vector3(2.0f, -0.5f, 0f)
                };

                seq.Append(tr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc)) // scale 증가 (0.1 -> 1.5)
                   .Join(tr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc)); // 곡선을 따라 이동

                seq.Append(tr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc)) // 빠르게 크기 감소 (1.5 -> 0)
                   .Join(tr.GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear)); // 페이드아웃

                seq.OnComplete(() =>
                {
                    Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                });
            }
            else
            {
                rectTr.localScale = Vector3.one * 0.1f;

                Vector3[] pathPoints = new Vector3[]
                {
                    rectTr.position,
                    rectTr.position + new Vector3(1.0f, 2.0f, 0f),
                    rectTr.position + new Vector3(2.0f, -0.5f, 0f)
                };

                seq.Append(rectTr.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCirc)) // scale 증가 (0.1 -> 1.5)
                   .Join(rectTr.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(Ease.OutCirc)); // 곡선을 따라 이동

                seq.Append(rectTr.DOScale(0f, randomDownDuration).SetEase(Ease.InCirc)) // 빠르게 크기 감소 (1.5 -> 0)
                   .Join(rectTr.GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear)); // 페이드아웃

                seq.OnComplete(() =>
                {
                    Debug.Log("EndFontBouncingRightUp");
                    Managers.Resource.Destroy(rectTr.gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_TextFont);
                });
            }

            seq.Play();
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
    }
}
