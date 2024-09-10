using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    /*
        [ Note ]
        // --- Off할 때 다시 EnvRoot로 붙여주기
        // --- 그리고 이미 진행중인 상태에서 Skill을 Spawn하지 않는 등을 조정해야함. 쿨타임이 되면 다시 DoSkill을 시도하기 때문
        // Debug.Log($"<color=cyan>{nameof(BuffShield)}, {nameof(EnterInGame)}</color>");
    */

    // --- TEMP
    public class BuffShield : BuffBase
    {
        private const string c_hitBurst = "Hit_Burst";
        private ParticleSystem[] _onShields = null;
        private ParticleSystem[] _offShields = null;
        private ParticleSystem _hitBurst = null; // --- 이거 따로 Hit Effect VFX Simple로 줘야할지?
        private Vector3 _localPosition = Vector3.zero;
        //private Vector3 _localScale = Vector3.zero;
        private Vector3 _localScale = Vector3.zero;


        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EffectBuffType = EEffectBuffType.ShieldHp;
            _onShields = transform.GetChild(0).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _onShields.Length; ++i)
            {
                _onShields[i].gameObject.SetActive(false);
                if (_onShields[i].gameObject.name.Contains(c_hitBurst))
                    _hitBurst = _onShields[i].gameObject.GetComponent<ParticleSystem>();
            }

            _offShields = transform.GetChild(1).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            _localPosition = Vector3.up * 0.75f;
            _localScale = new Vector3(0.8f, 1f, 0.8f);
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            transform.localPosition = _localPosition;
            EnableShield(_onShields, false);
            EnableShield(_offShields, false);
            base.EnterInGame(spawnPos);
        }

        public override void ApplyEffect()
        {
            // --- 쉴드 에너지부터 채워주고
            base.ApplyEffect();

            // --- Show Effect
            // EnableShield(_onShields, true);
        }

        public override void EnterShowEffect()
        {
            transform.localScale = _localScale;
            EnableShield(_onShields, true);
        }

        public override void OnShowEffect()
            => _hitBurst?.Play();

        public override void ExitShowEffect()
        {
            transform.localScale = Vector3.one * 1.2f;
            EnableShield(_onShields, false);
            EnableShield(_offShields, true);
            StartCoroutine(CoRemoveShield());
        }

        private IEnumerator CoRemoveShield()
        {
            yield return new WaitForSeconds(2f);
            ClearEffect(EEffectClearType.TimeOut);
            Debug.Log($"<color=white>ESType: {_skill.SkillType}</color>");
            _skill.LockCoolTimeSkill = false;
            // _skill 한테도 끝났다는 것을 알려준다. (다음 쿨타임을 위해)
        }

        private void EnableShield(ParticleSystem[] shields, bool isOn)
        {
            for (int i = 0; i < shields.Length; ++i)
            {
                if (isOn)
                {
                    // --- On할 때 잠깐 붙여주고
                    transform.SetParent(Owner.transform);
                    // transform.localPosition = new Vector3(0, 0.75f, 0f);
                    // transform.localScale = _localScale;
                    shields[i].gameObject.SetActive(true);
                    shields[i].Play();
                }
                else
                    shields[i].gameObject.SetActive(false);
            }
        }
    }
}

