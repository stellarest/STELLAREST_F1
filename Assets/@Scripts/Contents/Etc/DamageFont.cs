using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class DamageFont : InitBase
    {
        private RectTransform _rectTransform = null;
        private TextMeshPro _damageText = null;
        private SortingGroup _sortingGroup = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _rectTransform = GetComponent<RectTransform>();
            _damageText = GetComponent<TextMeshPro>();
            _sortingGroup = GetComponent<SortingGroup>();
            _sortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_DamageFont;
            return true;
        }

        public void Show(Vector3 position, float damage, Color textColor, bool isCritical = false,
                        EFontSignType fontSignType = EFontSignType.None,
                        EFontOutAnimationType fontOutAnimType = EFontOutAnimationType.OutGoingUp)
        {
            _damageText.color = textColor;
            transform.position = position;
            switch (fontSignType)
            {
                case EFontSignType.Plus:
                    _damageText.text = $"+{Mathf.Abs(damage)}";
                    break;

                case EFontSignType.Minus:
                    _damageText.text = $"-{Mathf.Abs(damage)}";
                    break;

                default:
                    _damageText.text = $"{Mathf.Abs(damage)}";
                    break;
            }
            _damageText.alpha = 1;
            DoOutAnimation(fontOutAnimType);
        }

        public void Show(Vector3 position, float damage, string colorCode, bool isCritical = false,
                        EFontSignType fontSignType = EFontSignType.None,
                        EFontOutAnimationType fontOutAnimType = EFontOutAnimationType.OutGoingUp)
        {
            Color textColor = Color.white;
            if (isCritical)
                textColor = Color.red;
            else if (ColorUtility.TryParseHtmlString(colorCode, out textColor))
                _damageText.color = textColor;
            transform.position = position;
            switch (fontSignType)
            {
                case EFontSignType.Plus:
                    _damageText.text = $"+{Mathf.Abs(damage)}";
                    break;

                case EFontSignType.Minus:
                    _damageText.text = $"-{Mathf.Abs(damage)}";
                    break;

                default:
                    _damageText.text = $"{Mathf.Abs(damage)}";
                    break;
            }
            _damageText.alpha = 1;
            DoOutAnimation(fontOutAnimType);
        }

        // --- 나중에 RectTransform의 객체만 받는 매니저로 구성해야하거나 아니면 MonoContents 매니저에서 실행해도 될듯
        private void DoOutAnimation(EFontOutAnimationType animType)
        {
            switch (animType)
            {
                case EFontOutAnimationType.OutGoingUp:
                    PlayOutGoingUp();
                    break;

                case EFontOutAnimationType.OutSmaller:
                    PlayOutSmaller();
                    break;

                case EFontOutAnimationType.OutFalling:
                    PlayOutFalling();
                    break;

                case EFontOutAnimationType.OutBouncingLeftUp:
                    // PlyaOutBouncingLeftUp();
                    PlayOutBouncingLeftUp_TEST();
                    break;

                case EFontOutAnimationType.OutBouncingRightUp:
                    // PlayOutBouncingRightUp();
                    PlayOutBouncingRightUp_TEST();

                    break;
            }
        }

        // --- PREV ORIGIN
        private void PlayOutGoingUp()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(endValue: 1.3f, duration: 0.1f)).SetEase(Ease.Linear)
           .Join(transform.DOMove(endValue: transform.position + Vector3.up, duration: 0.2f)).SetEase(Ease.Linear)
           .Join(transform.GetComponent<TMP_Text>().DOFade(endValue: 0, duration: 0.5f)).SetEase(Ease.InQuint)

           .OnComplete(() =>
           {
               Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
           });

            seq.Play();
        }


        private void PlayOutSmaller()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.localScale = Vector3.one * 0.1f;

            Sequence seq = DOTween.Sequence();
            seq.Append(_rectTransform.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack)) // scale 증가 (0.1 -> 1.5)
               .Join(_rectTransform.DOMove(_rectTransform.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack)); // 위로 살짝 튀는 동작

            seq.Append(_rectTransform.DOMove(_rectTransform.position, 0.65f).SetEase(Ease.InBack)) // 자연스럽게 아래로 튕겨 내려오는 동작 (0.65초)
               .Join(_rectTransform.DOScale(0f, 0.4f).SetEase(Ease.InQuint)) // scale 감소 (1.5 -> 0, 0.4초)

               .Append(_rectTransform.GetComponent<TMP_Text>().DOFade(0, 0.4f).SetEase(Ease.Linear).SetDelay(0.2f)); // 페이드아웃 (0.4초, 약간 지연 후 시작)

            seq.OnComplete(() =>
            {
                Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
            });

            seq.Play();
        }

        private void PlayOutFalling()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.localScale = Vector3.one * 0.1f;

            Sequence seq = DOTween.Sequence();
            seq.Append(_rectTransform.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack)) // scale 증가 (0.1 -> 1.5)
               .Join(_rectTransform.DOMove(_rectTransform.position + Vector3.up * 1.5f, 0.3f).SetEase(Ease.OutBack)); // 위로 살짝 튀는 동작

            seq.Append(_rectTransform.DOMove(_rectTransform.position - Vector3.up * 0.5f, 0.3f).SetEase(Ease.InExpo)) // 급격히 아래로 떨어짐 (0.3초)
               .Join(_rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InExpo)) // 빠르게 크기 감소 (1.5 -> 0, 0.3초)
               .Join(GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.Linear)); // 빠르게 페이드아웃 (0.3초)

            seq.OnComplete(() =>
            {
                Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
            });

            seq.Play();
        }

        // --- Bouncing Up Left / Right 개선 필요
        // private void PlyaOutBouncingLeftUp()
        // {
        //     // RectTransform 참조 가져오기
        //     _rectTransform = GetComponent<RectTransform>();

        //     // 처음 scale을 매우 작게 설정 (0.1)
        //     _rectTransform.localScale = Vector3.one * 0.1f;

        //     Sequence seq = DOTween.Sequence();

        //     // 랜덤 요소 추가
        //     float randomUpDuration = Random.Range(0.3f, 0.5f); // 올라가는 속도에 약간의 랜덤성 부여 (0.3초 ~ 0.5초)
        //     float randomDownDuration = Random.Range(0.2f, 0.4f); // 내려가는 속도에 약간의 랜덤성 부여 (0.2초 ~ 0.4초)

        //     // 첫 번째 단계: 빠르게 왼쪽 상단으로 튕겨 올라가는 동작 (랜덤 속도)
        //     Vector3 upwardPos = _rectTransform.position + new Vector3(-1.5f, 2f, 0f); // 왼쪽 상단으로 이동할 위치 설정
        //     seq.Append(_rectTransform.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCubic)) // scale 증가 (0.1 -> 1.5), 빠르게 올라감
        //        .Join(_rectTransform.DOMove(upwardPos, randomUpDuration).SetEase(Ease.OutCubic)); // 자연스럽게 튕겨 올라감 (랜덤 시간)

        //     // 두 번째 단계: 자연스럽게 아래로 떨어지면서 속도 증가 (랜덤 속도)
        //     Vector3 downwardPos = upwardPos + new Vector3(-0.5f, -2.5f, 0f); // 왼쪽 아래로 떨어질 위치 설정
        //     seq.Append(_rectTransform.DOMove(downwardPos, randomDownDuration).SetEase(Ease.InCubic)) // 속도가 점점 빨라지며 떨어짐 (랜덤 시간)
        //        .Join(_rectTransform.DOScale(0f, randomDownDuration).SetEase(Ease.InCubic)) // 빠르게 크기 감소 (1.5 -> 0)
        //        .Join(GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear)); // 페이드아웃 (랜덤 시간)

        //     // 애니메이션 완료 후 오브젝트 제거
        //     seq.OnComplete(() =>
        //     {
        //         Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
        //     });

        //     seq.Play();
        // }

        // private void PlayOutBouncingRightUp()
        // {
        //     // RectTransform 참조 가져오기
        //     _rectTransform = GetComponent<RectTransform>();

        //     // 처음 scale을 매우 작게 설정 (0.1)
        //     _rectTransform.localScale = Vector3.one * 0.1f;

        //     Sequence seq = DOTween.Sequence();

        //     // 랜덤 요소 추가
        //     float randomUpDuration = Random.Range(0.3f, 0.5f); // 올라가는 속도에 약간의 랜덤성 부여 (0.3초 ~ 0.5초)
        //     float randomDownDuration = Random.Range(0.2f, 0.4f); // 내려가는 속도에 약간의 랜덤성 부여 (0.2초 ~ 0.4초)

        //     // 첫 번째 단계: 빠르게 오른쪽 상단으로 튕겨 올라가는 동작 (랜덤 속도)
        //     Vector3 upwardPos = _rectTransform.position + new Vector3(1.5f, 2f, 0f); // 오른쪽 상단으로 이동할 위치 설정
        //     seq.Append(_rectTransform.DOScale(1.5f, randomUpDuration).SetEase(Ease.OutCubic)) // scale 증가 (0.1 -> 1.5), 빠르게 올라감
        //        .Join(_rectTransform.DOMove(upwardPos, randomUpDuration).SetEase(Ease.OutCubic)); // 자연스럽게 튕겨 올라감 (랜덤 시간)

        //     // 두 번째 단계: 자연스럽게 아래로 떨어지면서 속도 증가 (랜덤 속도)
        //     Vector3 downwardPos = upwardPos + new Vector3(0.5f, -2.5f, 0f); // 오른쪽 아래로 떨어질 위치 설정
        //     seq.Append(_rectTransform.DOMove(downwardPos, randomDownDuration).SetEase(Ease.InCubic)) // 속도가 점점 빨라지며 떨어짐 (랜덤 시간)
        //        .Join(_rectTransform.DOScale(0f, randomDownDuration).SetEase(Ease.InCubic)) // 빠르게 크기 감소 (1.5 -> 0)
        //        .Join(GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear)); // 페이드아웃 (랜덤 시간)

        //     // 애니메이션 완료 후 오브젝트 제거
        //     seq.OnComplete(() =>
        //     {
        //         Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
        //     });

        //     seq.Play();
        // }

        // --- BouncingUp Test
        private void PlayOutBouncingLeftUp_TEST()
        {
            DG.Tweening.Ease OutTest = Managers.MonoContents.OutTest;
            DG.Tweening.Ease InTest = Managers.MonoContents.InTest;

            // RectTransform 참조 가져오기
            _rectTransform = GetComponent<RectTransform>();

            // 처음 scale을 매우 작게 설정 (0.1)
            _rectTransform.localScale = Vector3.one * 0.1f;

            Sequence seq = DOTween.Sequence();

            // 랜덤 요소 추가
            float randomUpDuration = Random.Range(0.3f, 0.5f); // 올라가는 속도에 약간의 랜덤성 부여 (0.3초 ~ 0.5초)
            float randomDownDuration = Random.Range(0.2f, 0.4f); // 내려가는 속도에 약간의 랜덤성 부여 (0.2초 ~ 0.4초)

            // 경로 지점 설정 (곡선 이동)
            Vector3[] pathPoints = new Vector3[]
            {
                _rectTransform.position, // 시작 위치
                _rectTransform.position + new Vector3(-1.0f, 2.0f, 0f), // 왼쪽 상단 (곡선 중간 위치)
                _rectTransform.position + new Vector3(-2.0f, -0.5f, 0f) // 왼쪽 아래
            };

            // 첫 번째 단계: 곡선을 따라 자연스럽게 이동 (랜덤 속도)
            seq.Append(_rectTransform.DOScale(1.5f, randomUpDuration).SetEase(OutTest)) // scale 증가 (0.1 -> 1.5)
               .Join(_rectTransform.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(OutTest)); // 곡선을 따라 이동

            // 두 번째 단계: 자연스럽게 아래로 떨어지면서 크기 감소 및 페이드아웃
            seq.Append(_rectTransform.DOScale(0f, randomDownDuration).SetEase(InTest)) // 빠르게 크기 감소 (1.5 -> 0)
               .Join(GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear)); // 페이드아웃

            // 애니메이션 완료 후 오브젝트 제거
            seq.OnComplete(() =>
            {
                Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
            });

            seq.Play();
        }

        private void PlayOutBouncingRightUp_TEST()
        {
            DG.Tweening.Ease OutTest = Managers.MonoContents.OutTest;
            DG.Tweening.Ease InTest = Managers.MonoContents.InTest;

            // RectTransform 참조 가져오기
            _rectTransform = GetComponent<RectTransform>();

            // 처음 scale을 매우 작게 설정 (0.1)
            _rectTransform.localScale = Vector3.one * 0.1f;

            Sequence seq = DOTween.Sequence();

            // 랜덤 요소 추가
            float randomUpDuration = Random.Range(0.3f, 0.5f); // 올라가는 속도에 약간의 랜덤성 부여 (0.3초 ~ 0.5초)
            float randomDownDuration = Random.Range(0.2f, 0.4f); // 내려가는 속도에 약간의 랜덤성 부여 (0.2초 ~ 0.4초)

            // 경로 지점 설정 (곡선 이동)
            Vector3[] pathPoints = new Vector3[]
            {
                _rectTransform.position, // 시작 위치
                _rectTransform.position + new Vector3(1.0f, 2.0f, 0f), // 오른쪽 상단 (곡선 중간 위치)
                _rectTransform.position + new Vector3(2.0f, -0.5f, 0f) // 오른쪽 아래
            };

            // 첫 번째 단계: 곡선을 따라 자연스럽게 이동 (랜덤 속도)
            seq.Append(_rectTransform.DOScale(1.5f, randomUpDuration).SetEase(OutTest)) // scale 증가 (0.1 -> 1.5)
               .Join(_rectTransform.DOPath(pathPoints, randomUpDuration + randomDownDuration, PathType.CatmullRom).SetEase(OutTest)); // 곡선을 따라 이동

            // 두 번째 단계: 자연스럽게 아래로 떨어지면서 크기 감소 및 페이드아웃
            seq.Append(_rectTransform.DOScale(0f, randomDownDuration).SetEase(InTest)) // 빠르게 크기 감소 (1.5 -> 0)
               .Join(GetComponent<TMP_Text>().DOFade(0, randomDownDuration).SetEase(Ease.Linear)); // 페이드아웃

            // 애니메이션 완료 후 오브젝트 제거
            seq.OnComplete(() =>
            {
                Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
            });

            seq.Play();
        }
    }
}

