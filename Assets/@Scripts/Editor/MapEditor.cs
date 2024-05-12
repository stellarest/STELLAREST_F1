using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using System.IO;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.Collections;

#if UNITY_EDITOR
using UnityEditor;
using Newtonsoft.Json;
#endif

namespace STELLAREST_F1
{
    public struct NodeTest : IComparable<NodeTest>
    {
        public int H;

        public int CompareTo(NodeTest other)
        {
            if (H == other.H)
                return 0;

            return H < other.H ? 1 : -1;
        }
    }

    public class MapEditor : EditorWindow
    {
#if UNITY_EDITOR
        [MenuItem("Tools/PrintTest %#H")]
        private static void PrintTest()
        {
            // ###
            // int value = 0;
            // int[,] arr = new int[3,5];
            // /*
            //     [0,0]: 0 | [0,1]: 1 | [0,2]: 2 | [0,3]: 3 | [0,4]: 4
            //     [1,0]: 5 | [1,1]: 6 | [1,2]: 7 | [0,3]: 8 | [0,4]: 9
            //     [2,0]: 10 | [2,1]: 11 | [2,2]: 12 | [2,3]: 13 | [2,4]: 14
            // */

            // for (int i = 0; i < arr.GetLength(0); ++i)
            // {
            //     for (int j = 0; j < arr.GetLength(1); ++j)
            //         arr[i, j] = value++;
            // }

            // for (int i = 0; i < arr.GetLength(0); ++i)
            // {
            //     for (int j = 0; j < arr.GetLength(1); ++j)
            //         Debug.Log($"arr[{i}][{j}]: {arr[i, j]}");
            //     Debug.Log("");
            // }

            // ###
            // List<int> lst = new List<int>() { 3, 5, 7, 9 ,12 };
            // int[] arr = lst.Where(n => n <= 7).ToArray();
            // for (int i = 0; i < arr.Length; ++i)
            //     Debug.Log($"arr[{i}]: {arr[i]}");

            // ###
            PriorityQueue<NodeTest> pq = new PriorityQueue<NodeTest>();
            NodeTest node = new NodeTest() { H = 23 };
            pq.Push(node);
            node = new NodeTest() { H = 10 };
            pq.Push(node);

            for (int i = 0; i < pq.Heap.Count; ++i)
                Debug.Log($"pq[{i}]: {pq.Heap[i].H}");
        }

        // Mac: %(Command) #(Shift) K
        [MenuItem("Tools/GenerateMap %#K")]
        private static void GenerateMap()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            if (gameObjects.Length == 0)
            {
                Debug.LogWarning($"Please select map before.");
                return;
            }

            foreach (GameObject go in gameObjects)
            {
                Tilemap tm = Util.FindChild<Tilemap>(go, Define.ReadOnly.String.Tilemap_Collision, true);
                if (tm == null)
                {
                    Debug.LogWarning($"Failed to get Tilemap component on \"{Define.ReadOnly.String.Tilemap_Collision}\" object.");
                    return;
                }
                tm.RefreshAllTiles();
                tm.CompressBounds();

                // xxx_Collision.txt의 파일은 결국 클라랑 서버랑 같이 사용할 데이터 파일이라고 보면 됨.
                using (var writer = File.CreateText($"Assets/@Resources/Data/MapData/{go.gameObject.name}_Collision.txt"))
                {
                    // x,y 최소, 최대 좌표를 적는다.
                    writer.WriteLine(tm.cellBounds.xMin);
                    writer.WriteLine(tm.cellBounds.xMax);
                    writer.WriteLine(tm.cellBounds.yMin);
                    writer.WriteLine(tm.cellBounds.yMax);

                    // 최소, 최대 좌표를 하나씩 순회
                    // Y는 최상단부터, x는 최하단부터 하나씩 순회한다. (load 데이터로 읽어 오듯이)
                    for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
                    {
                        for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
                        {
                            // 죄표에 있는 타일 정보를 가져옴
                            TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                            if (tile != null)
                            {
                                if (tile.name.Contains(Define.ReadOnly.String.Tile_CanMove))
                                    writer.Write(Define.ReadOnly.Character.Map_Tool_CanMove_1);  // CanGo -> CanMove로 이름 변경할 것
                                else if (tile.name.Contains(Define.ReadOnly.String.Tile_SemiBlock))
                                    writer.Write(Define.ReadOnly.Character.Map_Tool_SemiBlock_2); 
                                else if (tile.name.Contains(Define.ReadOnly.String.Tile_Block))
                                   writer.Write(Define.ReadOnly.Character.Map_Tool_Block_0);
                            }
                        }
                        writer.WriteLine();
                    }
                }
            }

            Debug.Log("Map Collision Generation Complete");
        }

        private static T LoadJson<T, Key, value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/@Resources/Data/JsonData/{path}.json");
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
#endif
    }

}
