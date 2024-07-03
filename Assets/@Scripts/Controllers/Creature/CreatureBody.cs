using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Lumin;

namespace STELLAREST_F1
{ 
    public class CreatureBody : BaseBody
    {
        public int DataTemplateID { get; protected set; } = -1;
        public virtual Vector3 GetFirePosition() => Vector3.zero;
    }
}
