using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CameraController : InitBase
    {
        private BaseObject _target = null;
        public BaseObject Target
        {
            get => _target;
            set => _target = value;
        }
        
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Camera.main.orthographicSize = ReadOnly.Numeric.CamOrthoSize;
            return true;
        }

        private void LateUpdate()
        {
            if (_target == null)
                return;

            // TEMP
            //Vector3 toTargetPos = new Vector3(_target.transform.position.x,  _target.transform.position.y + 1.3f, -10f);
            Vector3 toTargetPos = new Vector3(_target.transform.position.x,  _target.transform.position.y, -10f);
            transform.position = toTargetPos;
        }
    }
}

