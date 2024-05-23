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
            if (Input.GetKeyDown(KeyCode.P))
                ShowCellPosText();

            if (Input.GetKeyDown("0"))
            {
                Debug.Log("===== Key Value Pair =====");
                foreach (var pair in Managers.Map.Cells)
                {
                    if (pair.Value != null)
                        Debug.Log($"({pair.Key}, {pair.Value}");
                }
            }

            if (Input.GetKeyDown("1"))
                ChangeRandomHeroLeader();

            if (Input.GetKeyDown("2"))
                Managers.Object.HeroLeaderController.SetJustFollowClosely();

            if (Input.GetKeyDown("3"))
                Managers.Object.HeroLeaderController.SetNarrowFormation();

            if (Input.GetKeyDown("4"))
                Managers.Object.HeroLeaderController.SetWideFormation();

            if (Input.GetKeyDown("5"))
                Managers.Object.HeroLeaderController.SetPatrolFree();

            if (Input.GetKeyDown("6"))
                Managers.Object.HeroLeaderController.ShuffleMembersPosition();
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
                    GameObject cell = new GameObject { name = $"{x}, {y}" };
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
            if (Managers.Object.Heroes.Count == 1)
                return;

            int randIdx = UnityEngine.Random.Range(0, Managers.Object.Heroes.Count);
            Hero newHeroLeader = Managers.Object.Heroes[randIdx];
            Hero currentLeader = Managers.Object.HeroLeaderController.Leader;
            while (newHeroLeader == currentLeader)
            {
                randIdx = UnityEngine.Random.Range(0, Managers.Object.Heroes.Count);
                newHeroLeader = Managers.Object.Heroes[randIdx];
            }

            Managers.Object.HeroLeaderController.Leader = newHeroLeader;
        }

        // public Hero Leader { get; set; } = null; // TMEP
        // public EHeroLeaderChaseMode LeaderChaseMode = EHeroLeaderChaseMode.JustFollowClosely;
        // private void OnDrawGizmos()
        // {
        //     if (Leader == null)
        //         return;

        //     if (LeaderChaseMode == EHeroLeaderChaseMode.JustFollowClosely)
        //         return;

        //     float distance = (float)LeaderChaseMode;
        //     int memberCount = Managers.Object.Heroes.Count - 1;
        //     for (int i = 0; i < memberCount; ++i)
        //     {
        //         float degree = 360f * i / memberCount;
        //         degree = Mathf.PI / 180f * degree;
        //         float x = Leader.transform.position.x + Mathf.Cos(degree) * distance;
        //         float y = Leader.transform.position.y + Mathf.Sin(degree) * distance;

        //         Vector3Int cellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
        //         //Vector3Int cellPos = new Vector3Int(1, 0, 0); // A* Test
        //         Vector3 worldCenterPos = Managers.Map.CenteredCellToWorld(cellPos);

        //         Gizmos.color = Color.red;
        //         Gizmos.DrawSphere(worldCenterPos, radius: 0.5f);
        //     }
        // }

        // public IEnumerator CoUpdateReplacePosition()
        // {
        //     while (true)
        //     {
        //         List<Hero> members = new List<Hero>();
        //         foreach (var hero in Managers.Object.Heroes)
        //         {
        //             if (hero.IsLeader == false)
        //                 members.Add(hero);
        //         }

        //         float _replaceHeroesDistance_Test = DevManager.Instance.TestReplaceDistance;
        //         Hero leader = Managers.Object.HeroLeaderController.Leader;
        //         for (int i = 0; i < members.Count; ++i)
        //         {
        //             float degree = 360f * i / members.Count;
        //             degree = Mathf.PI / 180f * degree;
        //             float x = leader.transform.position.x + Mathf.Cos(degree) * _replaceHeroesDistance_Test;
        //             float y = leader.transform.position.y + Mathf.Sin(degree) * _replaceHeroesDistance_Test;

        //             Vector3Int cellPos = Managers.Map.WorldToCell(new Vector3(x, y, 0));
        //             //members[i].ReplaceHero(cellPos);
        //             members[i].CoUpdateReplaceDestPosition(cellPos);
        //             yield return null;
        //         }
        //     }
        // }
    }
}
#endif