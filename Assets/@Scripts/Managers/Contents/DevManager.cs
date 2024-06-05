using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                ShowCellPosText();

            if (Input.GetKeyDown("0"))
            {
                // --- Check Current Cells Objects
                Debug.Log("===== Cells Pair =====");
                foreach (var pair in Managers.Map.Cells)
                {
                    if (pair.Value != null)
                        Debug.Log($"({pair.Key}, {pair.Value}");
                }
                Debug.Log($"<color=white>Is GameOver: {Managers.Game.IsGameOver}</color>");
            }

            if (Input.GetKeyDown("1"))
                Managers.Game.ChangeHeroLeader(autoChangeFromDead: false);

            if (Input.GetKeyDown("2"))
                Managers.Object.HeroLeaderController.ChangeFormation_Dev();

            if (Input.GetKeyDown("3"))
                Managers.Object.HeroLeaderController.ShuffleMembersPosition();

            if (Input.GetKeyDown("4"))
                OnOffTileCollider();
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

        bool OnOffTileColliderFlag = false;
        private void OnOffTileCollider()
        {
            GameObject map = GameObject.Find("@Map_SummerForestField_Test2");
            if (OnOffTileColliderFlag == false)
            {
                GameObject wall = Util.FindChild(map, "Wall", true, true);
                if (wall != null)
                {
                    TilemapRenderer tr = wall.GetComponent<TilemapRenderer>();
                    tr.sortingOrder = 100;
                }

                GameObject tc = Util.FindChild(map, "Tilemap_Collision", true, true);
                if (tc != null)
                    tc.SetActive(false);
            }
            else
            {
                GameObject wall = Util.FindChild(map, "Wall", true, true);
                if (wall != null)
                {
                    TilemapRenderer tr = wall.GetComponent<TilemapRenderer>();
                    tr.sortingOrder = 20;
                }

                GameObject tc = Util.FindChild(map, "Tilemap_Collision", true, true);
                if (tc != null)
                    tc.SetActive(true);
            }

            OnOffTileColliderFlag = !OnOffTileColliderFlag;
        }
    }
}
#endif