using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    // Animation Curve, VFX...
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

        private int _trySpawnCount = 0;
        private const int _tryMaxSpawnCount = 999;
        public Vector3Int Spawn(Vector3 spawnPos)
        {
            Vector3Int cellSpawnPos = Managers.Map.WorldToCell(spawnPos);

            int randMin = 0;
            int randMax = 0;
            while (Managers.Map.CanMove(cellSpawnPos) == false)
            {
                int x = Random.Range(--randMin, ++randMax);
                int y = Random.Range(--randMin, ++randMax);
                //cellSpawnPos = Managers.Map.WorldToCell()
            }

            return cellSpawnPos;
        }
    }
}
