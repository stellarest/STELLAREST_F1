using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class EffectVFXSwing : EffectBase
    {
        private const float c_movementSpeed = 4.0F;

        public override void ApplyEffect()
        {
            base.ApplyEffect();

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
        }

        // --- 눈속임 용도, 충돌x
        private void LateUpdate()
        {
            transform.position += _enteredDir.normalized * c_movementSpeed * Time.deltaTime;
        }
    }
}
