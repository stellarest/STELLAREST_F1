using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    // --- 필요시 상속 받아서 정의
    public class VFXBase : EffectBase
    {
        protected float _movementSpeed = 0f;

        protected override void EnterInGame(Vector3 spawnPos)
        {
            base.EnterInGame(spawnPos);
        }

        // --- 눈속임 용도, 충돌x
        private void LateUpdate()
        {
            transform.position += _enteredDir.normalized * _movementSpeed * Time.deltaTime;
        }
    }
}

