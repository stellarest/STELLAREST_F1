using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static STELLAREST_F1.Define;

#if UNITY_EDITOR
namespace STELLAREST_F1
{
    [System.Serializable]
    public class CellObject
    {
        public Vector3Int CellPos;
        public BaseObject CellObj;
    }

    public class DevManager : MonoBehaviour
    {
        public static DevManager Instance = null;

        private void Awake()
        {
            Instance = this;
        }

        public List<CellObject> CellObjs = new List<CellObject>();
        private IEnumerator Start()
        {
            while (true)
            {
                yield return null;
                foreach (var pair in Managers.Map.Cells)
                {
                    Vector3Int currentPos = pair.Key;
                    BaseObject currentObj = pair.Value;

                    // 오브젝트가 있는지 먼저 찾아본다.
                    CellObject cellObj = CellObjs.Find(n => n.CellObj == currentObj);
                    if (cellObj != null)
                    {
                        // 제거할 필요는 없고, 업데이트만 해주면 될 것 같은데
                        if (cellObj.CellPos != currentPos)
                        {
                            cellObj.CellPos = currentPos;
                            continue;
                        }
                    }
                }

                // yield return null;
                // foreach (var pair in Managers.Map.Cells)
                // {
                //     if (pair.Value != null)
                //     {
                //         Util.Log($"({pair.Key}, {pair.Value.gameObject.name}");
                //     }
                // }
            }
        }

        private void Update()
        {
            if (Dev.Input_J)
            {
                Dev.Log("===== Cells Pair =====");
                foreach (var pair in Managers.Map.Cells)
                {
                    if (pair.Value != null)
                        Dev.Log($"({pair.Key}, {pair.Value.gameObject.name}");
                    else
                        Dev.Log("NONE OF VALUE...");
                }

                // Util.Log($"<color=white>Is GameOver: {Managers.Game.IsGameOver}</color>");
            }

            if (Dev.Input_K)
                ShowCellPosText();

            if (Dev.Input_L)
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
            //sg.sortingLayerName = "BaseObject";
            sg.sortingLayerName = CString.CValue(EString.CValue_BaseObject);
            sg.sortingOrder = 999;

            for (int y = MaxY - 1; y >= MinY; --y)
            {
                for (int x = MinX; x < MaxX; ++x)
                {
                    GameObject cell = new GameObject { name = $"{x}, {y}" };
                    cell.transform.position = Managers.Map.CellToCenterWorld(new Vector3Int(x, y));
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
                GameObject tc = Util.FindChild(map, "Tilemap_Collision", true, true);
                if (tc != null)
                    tc.SetActive(false);

                // GameObject wall = Util.FindChild(map, "Wall", true, true);
                // if (wall != null)
                //     wall.SetActive(false);

                GameObject to = Util.FindChild(map, "Tilemap_Object", true, true);
                if (to != null)
                    to.SetActive(false);
            }
            else
            {
                GameObject tc = Util.FindChild(map, "Tilemap_Collision", true, true);
                if (tc != null)
                    tc.SetActive(true);

                GameObject to = Util.FindChild(map, "Tilemap_Object", true, true);
                if (to != null)
                    to.SetActive(false);
            }

            OnOffTileColliderFlag = !OnOffTileColliderFlag;
        }
    }
}
#endif