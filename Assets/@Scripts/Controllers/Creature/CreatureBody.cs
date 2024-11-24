using System;
using System.Collections;
using System.Collections.Generic;
using SpriteTrail;
using UnityEngine;

using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{ 
    public class CreatureBody : BaseBody
    {
        [SerializeField] protected TrailPreset _weaponLSTrailPreset = null;
        [SerializeField] protected TrailPreset _weaponRSTrailPreset = null;
    }
}
