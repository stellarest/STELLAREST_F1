using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using UnityEngine;
using static STELLAREST_F1.Define;

// --- 버프에서 VFX를 가지고 있어도 괜찮을 것 같다.
public class VFXShieldBlue : VFXBase
{
    private const string c_HitBurst = "Hit_Burst";
    private ParticleSystem[] _onShields = null;
    private ParticleSystem[] _offShields = null;
    private ParticleSystem _hitBurst = null;
    private Vector3 _onShieldsLocalPos = Vector3.zero;
    private Vector3 _onShieldsLocalScale = Vector3.one;
    private Vector3 _offShieldsLocalScale = Vector3.one;

    protected override void InitialSetInfo(int dataID)
    {
        base.InitialSetInfo(dataID);
        _onShields = transform.GetChild(0).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
        for (int i = 0; i < _onShields.Length; ++i)
        {
            _onShields[i].gameObject.SetActive(false);
            if (_onShields[i].gameObject.name.Contains(c_HitBurst))
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
        // 여기서 버프를 생성한다.
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        if (Owner.Shield == 0.0f)
        {
            Debug.LogWarning("Zero value of BonusHealthShield.");
            ExitEffect();
            return;
        }

        for (int i = 0; i < _offShields.Length; ++i)
            _offShields[i].gameObject.SetActive(false);

        transform.localPosition = _onShieldsLocalPos;
        transform.localScale = _onShieldsLocalScale;
        for (int i = 0; i < _onShields.Length; ++i)
        {
            _onShields[i].gameObject.SetActive(true);
            _onShields[i].Play();
        }
    }

    public override void OnShowEffect()
    {
        base.OnShowEffect();
        _hitBurst?.Play();
    }

    public override void ExitEffect()
    {
        base.ExitEffect();
        for (int i = 0; i < _onShields.Length; ++i)
            _onShields[i].gameObject.SetActive(false);

        transform.localScale = _offShieldsLocalScale;
        for (int i = 0; i < _offShields.Length; ++i)
        {
            _offShields[i].gameObject.SetActive(true);
            _offShields[i].Play();
        }
    }

    protected override void OnRemoveSelfByCondition(Action endCallback = null)
    {
        StartCoroutine(CoRemoveShield(endCallback));
    }

    private IEnumerator CoRemoveShield(Action endCallback)
    {
        yield return new WaitForSeconds(2.0F);
        for (int i = 0; i < _offShields.Length; ++i)
            _offShields[i].gameObject.SetActive(false);

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        //_skill.LockCoolTimeSkill = false;

        if (_skill != null)
            _skill.StartManualCoolTime();

        endCallback?.Invoke();
    }
}
