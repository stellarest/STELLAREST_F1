using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MonoContentsManager : MonoBehaviour
    {
        public static MonoContentsManager Instance = null;
        
        [SerializeField] private AnimationCurve[] _curves = new AnimationCurve[(int)EAnimationCurveType.Max];
        public AnimationCurve Curve(EAnimationCurveType curveType)
        {
            if (curveType == EAnimationCurveType.None || curveType == EAnimationCurveType.Max)
                return _curves[(int)EAnimationCurveType.Linear];

            return _curves[(int)curveType];
        }

        private void Awake()
        {
            Instance = this;
        }

    }
}
