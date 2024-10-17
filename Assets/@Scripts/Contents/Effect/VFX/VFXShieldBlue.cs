using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1;
using UnityEngine;

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
    }

    public override void EnterEffect()
    {
        base.EnterEffect();
    }
}
