using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace STELLAREST_F1
{
    public class VFXWindBlade : VFXBase
    {
        protected float _movementSpeed = 0.0f;
        private ParticleSystem _ps = null;
        private ParticleSystemRenderer _psRenderer = null;

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            _movementSpeed = 5.0f;
            _ps = GetComponent<ParticleSystem>();
            _psRenderer = GetComponent<ParticleSystemRenderer>();
        }

        public override void ApplyEffect()
        {
            base.ApplyEffect();

            var main = _ps.main;
            float angle = Mathf.Atan2(-_enteredDir.normalized.x, _enteredDir.normalized.y) * Mathf.Rad2Deg;
            if (angle < 0.0f)
                angle += 360.0f;

            main.startRotation = angle * Mathf.Deg2Rad * -1.0f;
            _psRenderer.flip = new Vector3(_enteredSignX, 0, 0);
        }

        private void LateUpdate()
        {
            transform.position += _enteredDir.normalized * _movementSpeed * Time.deltaTime;
        }
    }
}

/*
    public class VFXWindBlade : VFXBase
    {
        protected float _movementSpeed = 0.0f;
        private ParticleSystem[] _particles = null;
        private ParticleSystemRenderer[] _particleRenderers = null;

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            _movementSpeed = 4.0f;
            _particles = GetComponentsInChildren<ParticleSystem>();
            _particleRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
        }

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

        private void LateUpdate()
        {
            transform.position += _enteredDir.normalized * _movementSpeed * Time.deltaTime;
        }
    }
*/