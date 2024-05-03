using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using System.IO;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
using Newtonsoft.Json;
#endif

namespace STELLAREST_F1
{
    public class MapEditor : EditorWindow
    {
#if UNITY_EDITOR
        // Mac: %(Command) #(Shift) K
        [MenuItem("Tools/GenerateMap %#K")]
        private static void GenerateMap()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            if (gameObjects.Length == 0)
            {
                Debug.LogWarning($"Please select map object before.");
                return;
            }

            foreach (GameObject go in gameObjects)
            {
                Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);
                if (tm == null)
                {
                    Debug.LogWarning("Failed to get Tilemap component on \"Tilemap_Collision\" object.");
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
                                if (tile.name.Contains(Define.ReadOnly.String.Tile_CanGo))
                                    writer.Write(Define.ReadOnly.Character.Map_Tool_Write_CanGo_1); 
                                else if (tile.name.Contains(Define.ReadOnly.String.Tile_SemiBlock))
                                    writer.Write(Define.ReadOnly.Character.Map_Tool_Write_SemiBlock_2); 
                                else if (tile.name.Contains(Define.ReadOnly.String.Tile_Block))
                                   writer.Write(Define.ReadOnly.Character.Map_Tool_Write_Block_0);
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
