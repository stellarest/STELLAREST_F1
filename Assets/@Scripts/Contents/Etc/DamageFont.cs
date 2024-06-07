using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class DamageFont : BaseObject
    {
        private TextMeshPro _damageText = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            _damageText = GetComponent<TextMeshPro>();
            RigidBody.simulated = false;
            Collider.enabled = false;
            SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_Projectile;
            return true;
        }

        public void SetInfo(Vector2 pos, float damage = 0f, bool isCritical = false)
        {
            transform.position = pos;
            Color textColor = Color.red;
            if (damage < 0f)
            {
                if (ColorUtility.TryParseHtmlString("#4EEE6F", out textColor))
                    _damageText.color = textColor;
            }
            else if (isCritical)
            {
                if (ColorUtility.TryParseHtmlString("#EFAD00", out textColor))
                    _damageText.color = textColor;
            }

            _damageText.text = $"{Mathf.Abs(damage)}";
            _damageText.alpha = 1;

            DoAnimation();
        }

        private void DoAnimation()
        {
            Sequence seq = DOTween.Sequence();

            //transform.localScale = Vector3.one * 0.5f;
            // 딜레이가 생기네
            seq.Append(transform.DOScale(endValue: 1.3f, duration: 0.1f)).SetEase(Ease.Linear)
           .Join(transform.DOMove(endValue: transform.position + Vector3.up, duration: 0.2f)).SetEase(Ease.Linear)

           //.Append(transform.DOScale(endValue: 1.0f, duration: 0.1f).SetEase(Ease.InOutBounce))
           .Join(transform.GetComponent<TMP_Text>().DOFade(endValue: 0, duration: 0.5f)).SetEase(Ease.InQuint)

           .OnComplete(() =>
           {
               Managers.Resource.Destroy(gameObject, poolingID: ReadOnly.DataAndPoolingID.DNPID_DamageFont);
           });

            seq.Play();
        }
    }
}

