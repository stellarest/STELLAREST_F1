using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BuffShield : BuffBase
    {
        private const string c_hitBurst = "Hit_Burst";
        private ParticleSystem[] _onShields = null;
        private ParticleSystem[] _offShields = null;
        private ParticleSystem _hitBurst = null;
        private Vector3 _onShieldsLocalPos = Vector3.zero;
        private Vector3 _onShieldsLocalScale = Vector3.one;
        private Vector3 _offShieldsLocalScale = Vector3.one;

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EffectBuffType = EEffectBuffType.BonusHealthShield;

            _onShields = transform.GetChild(0).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _onShields.Length; ++i)
            {
                _onShields[i].gameObject.SetActive(false);
                if (_onShields[i].gameObject.name.Contains(c_hitBurst))
                    _hitBurst = _onShields[i].gameObject.GetComponent<ParticleSystem>();
            }

            _offShields = transform.GetChild(1).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            _onShieldsLocalPos = Vector3.up * 0.75f;
            _onShieldsLocalScale = new Vector3(0.8f, 1f, 0.8f);
            _offShieldsLocalScale = Vector3.one * 1.2f;
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            // --- DEFENSE
            for (int i = 0; i < _onShields.Length; ++i)
                _onShields[i].gameObject.SetActive(false);

            for (int i = 0; i < _offShields.Length; ++i)
                _offShields[i].gameObject.SetActive(false);

            base.EnterInGame(spawnPos);
        }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
            if (Owner.BonusHealth == 0.0f)
            {
                Debug.Log("<color=cyan>ZERO HEALTH OF SHIELD</color>");
                ExitShowEffect();
                return;
            }
        }

        public override void EnterShowEffect()
        {
            base.EnterShowEffect();

            // --- ON SHIELDS
            Debug.Log("<color=white>SHIELD - ON</color>");
            for (int i = 0; i < _offShields.Length; ++i)
                _offShields[i].gameObject.SetActive(false);

            //transform.SetParent(Owner.transform);
            transform.localPosition = _onShieldsLocalPos;
            transform.localScale = _onShieldsLocalScale;
            for (int i = 0; i < _onShields.Length; ++i)
            {
                _onShields[i].gameObject.SetActive(true);
                _onShields[i].Play();
            }
        }

        public override void OnShowEffect()
            => _hitBurst?.Play();

        public override void ExitShowEffect()
        {
            // --- OFF SHIELDS
            base.ExitShowEffect();

            for (int i = 0; i < _onShields.Length; ++i)
                _onShields[i].gameObject.SetActive(false);

            transform.localScale = _offShieldsLocalScale;
            for (int i = 0; i < _offShields.Length; ++i)
            {
                _offShields[i].gameObject.SetActive(true);
                _offShields[i].Play();
            }

            StartCoroutine(CoRemoveShield());
        }

        private IEnumerator CoRemoveShield()
        {
            yield return new WaitForSeconds(2.0F);
            
            for (int i = 0; i < _offShields.Length; ++i)
                _offShields[i].gameObject.SetActive(false);

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.SetParent(Managers.Object.EffectRoot);
            ClearEffect(EEffectClearType.Disable);
            _skill.LockCoolTimeSkill = false;
        }
    }
}

