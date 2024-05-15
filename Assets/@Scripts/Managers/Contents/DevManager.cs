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

        public Hero _heroA = null;
        public Hero _heroB = null;

        private void Awake()
        {
            Instance = this;
            ReplaceMode = EReplaceHeroMode.FocusingOnLeader;
            TestReplaceDistance = (float)EReplaceHeroMode.FocusingOnLeader;
        }

        private void Update()
        {
            if (Input.GetKeyDown("1"))
                ChangeRandomHeroLeader();

            if (Input.GetKeyDown("2"))
                Managers.Game.ReplaceHeroes();

            // Me - 상, 하, 좌, 우 : 1 (한칸)
            // Me - 좌상단 우상단 우하단 좌하단 : 2 (대각선)
            // if (Input.GetKeyDown("3"))
            // {
            //     Debug.Log($"sqrDist: {(_heroA.CellPos - _heroB.CellPos).sqrMagnitude}");
            // }
        }

        public void ChangeRandomHeroLeader()
        {
            int randIdx = UnityEngine.Random.Range(0, Managers.Object.Heroes.Count);
            Managers.Object.Camp.Leader = Managers.Object.Heroes[randIdx];
            Debug.Log($"Completed: {nameof(ChangeRandomHeroLeader)}");
        }

        public float TestReplaceDistance = 3f;
        [field: SerializeField] public EReplaceHeroMode ReplaceMode { get; set; } = EReplaceHeroMode.FocusingOnLeader;
        private void OnDrawGizmos()
        {
            if (ReplaceMode == EReplaceHeroMode.FollowBaseCamp)
                return;
            if (Managers.Object.Heroes == null || Managers.Object.Camp == null)
                return;
            if (Managers.Object.Heroes.Count < 2 && Managers.Object.Camp.Leader == null)
                return;

            //float testDist = (float)ReplaceMode;
            Hero leader = Managers.Object.Camp.Leader;
            int memberCount = Managers.Object.Heroes.Count - 1;
            for (int i = 0; i < memberCount; ++i)
            {
                float degree = 360f * i / memberCount;
                degree = Mathf.PI / 180f * degree;
                float x = leader.transform.position.x + Mathf.Cos(degree) * TestReplaceDistance;
                float y = leader.transform.position.y + Mathf.Sin(degree) * TestReplaceDistance;

                Vector3Int cellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
                //Vector3Int cellPos = new Vector3Int(1, 0, 0); // A* Test
                Vector3 worldCenterPos = Managers.Map.CenteredCellToWorld(cellPos);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(worldCenterPos, radius: 0.5f);
            }
        }
    }
}
#endif