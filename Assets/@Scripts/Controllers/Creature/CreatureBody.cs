using System.Collections;
using System.Collections.Generic;
using SpriteTrail;
using UnityEngine;

using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{ 
    public class CreatureBody : BaseBody
    {
        protected void AddCustomWeaponSTrail(EString key, string loadKey)
        {
            if (string.IsNullOrEmpty(loadKey))
                return;

            if (_sTrailPresetDict.ContainsKey(key))
            {
                Dev.LogWarning(obj: nameof(CreatureBody), method: nameof(AddCustomWeaponSTrail), log: $"ContainsKey: {key}");
                return;
            }

            TrailPreset value = Managers.Resource.Load<TrailPreset>(loadKey);
            _sTrailPresetDict.Add(key, value);
        }

        public virtual void EnableSTrail_Weapon(EString eString) { }
        public virtual void DisableSTrail_Weapon() { }
    }
}
