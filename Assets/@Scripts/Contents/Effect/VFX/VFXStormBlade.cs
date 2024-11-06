using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class VFXStormBlade : VFXBase
    {
        [SerializeField] protected float _movementSpeed = 0.0f;
        [SerializeField] protected bool _flipX = false;

        private ParticleSystem _ps = null;
        private ParticleSystemRenderer _psRenderer = null;

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            //_movementSpeed = 12.0f;
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

            // EnteredSignX = (Owner.LookAtDir == ELookAtDirection.Left) ? 1 : 0;
            if (_flipX)
                _enteredSignX = _enteredSignX == 1 ? -1 : 1;
                
            _psRenderer.flip = new Vector3(_enteredSignX, 0, 0);
        }

        private void LateUpdate()
        {
            transform.position += _enteredDir.normalized * _movementSpeed * Time.deltaTime;
        }
    }
}
