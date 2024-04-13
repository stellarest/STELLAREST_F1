using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class AnimationManager
    {
        private AnimationCurve[] _curves = null;
        public AnimationCurve Curve(EAnimationCurveType curveType)
        {
            switch (curveType)
            {
                case EAnimationCurveType.Ease_In:
                    return _curves[(int)EAnimationCurveType.Ease_In];

                case EAnimationCurveType.Ease_Out:
                    return _curves[(int)EAnimationCurveType.Ease_Out];

                case EAnimationCurveType.Ease_In_Out:
                    return _curves[(int)EAnimationCurveType.Ease_In_Out];

                default:
                    return _curves[(int)EAnimationCurveType.Linear];
            }
        }

        public void Init()
        {
            _curves = new AnimationCurve[(int)EAnimationCurveType.Max];
            // Linear
            _curves[(int)EAnimationCurveType.Linear] = new AnimationCurve();
            _curves[(int)EAnimationCurveType.Linear].AddKey(0f, 0f);
            _curves[(int)EAnimationCurveType.Linear].AddKey(1f, 1f);

            // Ease-In
            _curves[(int)EAnimationCurveType.Ease_In] = new AnimationCurve();
            _curves[(int)EAnimationCurveType.Ease_In].AddKey(0f, 0f);
            _curves[(int)EAnimationCurveType.Ease_In].AddKey(1f, 1f);
            Keyframe[] keys = _curves[(int)EAnimationCurveType.Ease_In].keys;
            keys[0].outTangent = 1f; // 첫 번째 키프레임의 아웃 탄젠트를 조정하여 Ease-In 효과 적용
            _curves[(int)EAnimationCurveType.Ease_In].keys = keys;

            // Ease-Out
            _curves[(int)EAnimationCurveType.Ease_Out] = new AnimationCurve();
            _curves[(int)EAnimationCurveType.Ease_Out].AddKey(0f, 0f);
            _curves[(int)EAnimationCurveType.Ease_Out].AddKey(1f, 1f);
            keys = _curves[(int)EAnimationCurveType.Ease_Out].keys;
            keys[1].inTangent = 1f; // 두 번째 키프레임의 인 탄젠트를 조정하여 Ease-Out 효과 적용
            _curves[(int)EAnimationCurveType.Ease_Out].keys = keys;

            // Ease-In-Out
            _curves[(int)EAnimationCurveType.Ease_In_Out] = new AnimationCurve();
            _curves[(int)EAnimationCurveType.Ease_In_Out].AddKey(0f, 0f);
            _curves[(int)EAnimationCurveType.Ease_In_Out].AddKey(1f, 1f);
            keys = _curves[(int)EAnimationCurveType.Ease_In_Out].keys;
            keys[0].outTangent = 1f; // Ease-In 부터 적용
            keys[1].inTangent = 1f; // Ease-Out 적용
            keys[1].time = 0.5f; // 중간 지점 설정
            keys[1].value = 0.5f; // 중간 지점의 값 설정
            _curves[(int)EAnimationCurveType.Ease_In_Out].keys = keys;
        }
    }
}
