using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class CameraController : InitBase
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Camera.main.orthographicSize = ReadOnly.Numeric.CamOrthoSize;
            return true;
        }
    }
}

