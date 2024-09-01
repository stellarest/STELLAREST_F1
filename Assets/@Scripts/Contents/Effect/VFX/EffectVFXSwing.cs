using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class EffectVFXSwing : EffectBase
    {
        public override void ApplyEffect()
        {
            if (_particles == null || _particleRenderers == null)
                return;

            for (int i = 0; i < _particles.Length; ++i)
            {
                var main = _particles[i].main;
                float angle = Mathf.Atan2(-_enteredDir.normalized.x, _enteredDir.normalized.y) * Mathf.Rad2Deg;
                if (angle < 0)
                    angle += 360f;

                main.startRotation = angle * Mathf.Deg2Rad * -1f;
                _particleRenderers[i].flip = new Vector3(_enteredSignX, 0, 0);
            }

            base.ApplyEffect();
        }
    }
}
