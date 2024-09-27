using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class BuffDamageReduction : BuffBase
    {
        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EffectBuffType = EEffectBuffType.DamageReduction;
        }

        public override void EnterShowEffect()
        {
            base.EnterShowEffect();
            Debug.Log($"<color=magenta>!! {nameof(EnterShowEffect)} - {Dev_NameTextID}, {DataTemplateID}</color>");
        }

        public override void ExitShowEffect()
        {
            base.ExitShowEffect();
            transform.SetParent(Managers.Object.EffectRoot); // Destroy해도 될것같긴한데 일단 이렇게
        }
    }
}
