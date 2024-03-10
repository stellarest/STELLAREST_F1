using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_F1
{
    public class UI_Popup : UI_Base
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Managers.UI.SetCanvas(gameObject, true);
            return true;
        }
    }
}
