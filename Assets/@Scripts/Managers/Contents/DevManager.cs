using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static STELLAREST_F1.Define;

#if UNITY_EDITOR
namespace STELLAREST_F1
{
    public class DevManager : MonoBehaviour
    {
        public static DevManager Instance = null;

        public Hero _heroA = null;
        public Hero _heroB = null;

        public GameObject TestObject = null;
        //public Hero HeroTest = null;
        //public bool Magnitude = true;

        private void Awake()
        {
            Instance = this;
            ReplaceMode = EReplaceHeroMode.FocusingOnLeader;
            TestReplaceDistance = (float)EReplaceHeroMode.FocusingOnLeader;

            // TestObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // TestObject.name = "@@@@@TestSphere@@@@@";
            // SortingGroup sg = TestObject.AddComponent<SortingGroup>();
            // sg.sortingLayerName = "BaseObject";
            // sg.sortingOrder = 101;
            // TestObject.transform.localScale *= 0.5f;
        }

        // [ContextMenu("TestSetPos")]
        // private void TestSetPos()
        // {
        //     TestObject.transform.position = Managers.Map.CenteredCellToWorld(Vector3Int.zero);
        // }

        private void Update()
        {
            // if (HeroTest != null)
            // {
            //     Vector3Int cellPos1 = Managers.Map.WorldToCell(TestObject.transform.position);
            //     Vector3Int cellPos2 = HeroTest.CellPos;
            //     if (Magnitude)
            //     {
            //         Debug.Log($"magnitude: {(cellPos1 - cellPos2).magnitude}");
            //     }
            //     else
            //     {
            //         Debug.Log($"sqrMagnitude: {(cellPos1 - cellPos2).sqrMagnitude}");
            //     }
            // }

            if (Input.GetKeyDown(KeyCode.P))
                ShowCellPosText();

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

        private void ShowCellPosText()
        {
            /*
                MinX: -18, MaxX: 18
                MinY: -24, MaxY: 24
                
                좌상단: -18, 23
                우하단: 17, -24 
            */
            int MinX = Managers.Map.MinX;
            int MaxX = Managers.Map.MaxX;
            int MinY = Managers.Map.MinY;
            int MaxY = Managers.Map.MaxY;

            GameObject root = new GameObject { name = "@CellPos" };
            SortingGroup sg = root.AddComponent<SortingGroup>();
            sg.sortingLayerName = "BaseObject";
            sg.sortingOrder = 999;

            for (int y = MaxY - 1; y >= MinY; --y)
            {
                for (int x = MinX; x < MaxX; ++x)
                {
                    GameObject cell = new GameObject { name = $"{x},{y}" };
                    cell.transform.position = Managers.Map.CenteredCellToWorld(new Vector3Int(x, y));
                    TextMeshPro tmPro = cell.AddComponent<TextMeshPro>();
                    tmPro.fontSize = 3f;
                    tmPro.text = $"{x},{y}";
                    tmPro.alignment = TextAlignmentOptions.Center;
                    tmPro.autoSizeTextContainer = false;

                    cell.transform.SetParent(root.transform);
                }
            }
        }

        public void ChangeRandomHeroLeader()
        {
            int randIdx = UnityEngine.Random.Range(0, Managers.Object.Heroes.Count);
            Managers.Object.LeaderController.Leader = Managers.Object.Heroes[randIdx];
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