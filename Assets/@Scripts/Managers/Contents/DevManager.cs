using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

#if UNITY_EDITOR
namespace STELLAREST_F1
{
    public class DevManager : MonoBehaviour
    {
        public static DevManager Instance = null;
        
        private void Awake()
        {
            Instance = this;
            ReplaceMode = EReplaceHeroesMode.FocusingOnLeader;
        }

        [field: SerializeField] public EReplaceHeroesMode ReplaceMode { get; set; } = EReplaceHeroesMode.FocusingOnLeader;
        private Hero _leader = null;
        private void OnDrawGizmos()
        {
            if (ReplaceMode == EReplaceHeroesMode.FollowBaseCamp)
                return;

            float testDist = (float)ReplaceMode;

            if (Managers.Object.Heroes.Count != 0 && _leader == null)
            {
                foreach (var hero in Managers.Object.Heroes)
                {
                    if (hero.Leader)
                    {
                        _leader = hero;
                        break;
                    }
                }
            }

            int memberCount = Managers.Object.Heroes.Count - 1;
            if (memberCount == 0)
                return;

            // 근데 사실 이렇게 할 필요도 없긴한데
            if (_leader != null)
            {
                for (int i = 0; i < memberCount; ++i)
                {
                    float degree = 360f * i / memberCount;
                    degree = Mathf.PI / 180f * degree;
                    float x = _leader.transform.position.x + Mathf.Cos(degree) * testDist;
                    float y = _leader.transform.position.y + Mathf.Sin(degree) * testDist;

                    Vector3Int cellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                    //Vector3Int cellPos = new Vector3Int(1, 0, 0); // A* Test
                    Vector3 worldCenterPos = Managers.Map.CenteredCellToWorld(cellPos);

                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(worldCenterPos, radius: 0.5f);
                }
            }
        }
    }
}
#endif