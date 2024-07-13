using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Tilemaps;
using static STELLAREST_F1.Define;
using UnityEditor.Timeline;
using STELLAREST_F1.Data;

#if UNITY_EDITOR
using UnityEditor;
using Newtonsoft.Json;
#endif

namespace STELLAREST_F1
{
    public class MapEditor : EditorWindow
    {
#if UNITY_EDITOR
        [MenuItem("Tools/PrintTest %#H")]
        private static void PrintTest()
        {
            Debug.Log(Mathf.Acos(0.99f) * Mathf.Rad2Deg);

            // List<int> myNums = new List<int>();
            // for (int i = 1; i <= 5; ++i)
            // {
            //     myNums.Add(i);
            // }

            // PrintNums(myNums);

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
        }

        public static void PrintNums(IEnumerable<int> nums)
        {
            Debug.Log("PrintNums");
            foreach (var num in nums)
            {
                Debug.Log($"Num: {num}");
            }
            Debug.Log("End PrintNums");
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
                Tilemap tm = Util.FindChild<Tilemap>(go, ReadOnly.Util.Tilemap_Collision, true);
                if (tm == null)
                {
                    Debug.LogWarning($"Failed to get Tilemap component on \"{ReadOnly.Util.Tilemap_Collision}\" object.");
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
                                if (tile.name.Contains(ReadOnly.Util.Tile_CanMove))
                                    writer.Write(ReadOnly.Util.Map_Tool_CanMove_1);  // CanGo -> CanMove로 이름 변경할 것
                                else if (tile.name.Contains(ReadOnly.Util.Tile_SemiBlock))
                                    writer.Write(ReadOnly.Util.Map_Tool_SemiBlock_2);
                                else if (tile.name.Contains(ReadOnly.Util.Tile_Block))
                                    writer.Write(ReadOnly.Util.Map_Tool_Block_0);
                            }
                        }
                        writer.WriteLine();
                    }
                }
            }

            Debug.Log("Map Collision Generation Complete");
        }

        [MenuItem("Tools/Create Object Tile %#o")]
        public static void CreateObjectTile()
        {
            #region Monster Tile
            Dictionary<int, MonsterData> monsterDataDict = LoadJson<MonsterDataLoader, int, MonsterData>(ReadOnly.DataSet.MonsterData).MakeDict();
            foreach (var data in monsterDataDict.Values)
            {
                string name = $"{data.DataID}_{data.DescriptionTextID}";;
                string assetPath = Path.Combine("Assets/@Resources/TileMaps/Dev/Monsters", $"{name}.asset");
                if (assetPath == "")
                {
                    Debug.LogWarning("AssetPath is empty.");
                    continue;
                }

                CustomTile customTile = AssetDatabase.LoadAssetAtPath<CustomTile>(assetPath);
                if (customTile != null)
                {
                    // 아이콘의 이미지가 변경되었을 경우
                    string spriteName = data.IconImage.Replace(".sprite", "");
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/Creature/Monsters/Icons/{spriteName}.png");
                    if (sprite == null)
                    {
                        Debug.LogWarning($"Failed to find Sprite - {data.DescriptionTextID}");
                        continue;
                    }

                    customTile.DataID = data.DataID;
                    customTile.Name = data.DescriptionTextID;
                    customTile.ObjectType = EObjectType.Monster;
                    customTile.sprite = sprite;
                    EditorUtility.SetDirty(customTile);;
                    Debug.Log($"{nameof(CreateObjectTile)}, Completed SetDirty - {data.DescriptionTextID}");
                }
                else
                {
                    string spriteName = data.IconImage.Replace(".sprite", "");
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/Creature/Monsters/Icons/{spriteName}.png");
                    if (sprite == null)
                    {
                        Debug.LogWarning($"Failed to find Sprite - {data.DescriptionTextID}");
                        continue;
                    }

                    CustomTile newCustomTile = ScriptableObject.CreateInstance<CustomTile>();
                    newCustomTile.DataID = data.DataID;
                    newCustomTile.Name = data.DescriptionTextID;
                    newCustomTile.ObjectType = EObjectType.Monster;
                    newCustomTile.sprite = sprite;
                    AssetDatabase.CreateAsset(newCustomTile, assetPath);
                    Debug.Log($"{nameof(CreateObjectTile)}, Completed CreateAsset - {data.DescriptionTextID}");
                }
            }
            #endregion

            // 나무, 바위 몸뚱아리 분리 때문에 생략
            // #region Env Tile
            // Dictionary<int, EnvData> envDataDict = LoadJson<EnvDataLoader, int, EnvData>(ReadOnly.String.EnvData).MakeDict();
            // foreach (var data in envDataDict.Values)
            // {
            //     string name = $"{data.DataID}_{data.DescriptionTextID}"; ;
            //     string assetPath = Path.Combine("Assets/@Resources/TileMaps/Dev/Envs", $"{name}.asset");
            //     if (assetPath == "")
            //     {
            //         Debug.LogWarning("AssetPath is empty.");
            //         continue;
            //     }

            //     CustomTile customTile = AssetDatabase.LoadAssetAtPath<CustomTile>(assetPath);
            //     if (customTile != null)
            //     {
            //         // 아이콘의 이미지가 변경되었을 경우
            //         string spriteName = data.IconImage.Replace(".sprite", "");
            //         Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/Env/Icons/{spriteName}.png");
            //         if (sprite == null)
            //         {
            //             Debug.LogWarning($"Failed to find Sprite - {data.DescriptionTextID}");
            //             continue;
            //         }

            //         customTile.DataID = data.DataID;
            //         customTile.Name = data.DescriptionTextID;
            //         customTile.ObjectType = EObjectType.Env;
            //         customTile.sprite = sprite;
            //         EditorUtility.SetDirty(customTile); ;
            //         Debug.Log($"{nameof(CreateObjectTile)}, Completed SetDirty - {data.DescriptionTextID}");
            //     }
            //     else
            //     {
            //         string spriteName = data.IconImage.Replace(".sprite", "");
            //         Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/Env/Icons/{spriteName}.png");
            //         if (sprite == null)
            //         {
            //             Debug.LogWarning($"Failed to find Sprite - {data.DescriptionTextID}");
            //             continue;
            //         }

            //         CustomTile newCustomTile = ScriptableObject.CreateInstance<CustomTile>();
            //         newCustomTile.DataID = data.DataID;
            //         newCustomTile.Name = data.DescriptionTextID;
            //         newCustomTile.ObjectType = EObjectType.Env;
            //         newCustomTile.sprite = sprite;
            //         AssetDatabase.CreateAsset(newCustomTile, assetPath);
            //         Debug.Log($"{nameof(CreateObjectTile)}, Completed CreateAsset - {data.DescriptionTextID}");
            //     }
            // }
            // #endregion

            AssetDatabase.SaveAssets();
        }

        private static T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/@Resources/Data/{path}.json");
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
#endif
    }
}


            // Dictionary<int, BirdSpriteData> birdSpriteDataDict = LoadJson<BirdSpriteDataLoader, int, BirdSpriteData>(ReadOnly.String.BirdSpriteData).MakeDict();
            // foreach (var data in birdSpriteDataDict.Values)
            // {
            //     string name = $"{data.DataID}_{data.DescriptionTextID}";
            //     string path = Path.Combine("Assets/@Resources/TileMaps/Dev/Monsters", $"{name}.Asset");

            //     if (path == "")
            //         continue;

            //     CustomTile existingTile = AssetDatabase.LoadAssetAtPath<CustomTile>(path);
            //     if (existingTile != null)
            //     {
            //         /*
            //             TODO = Sprite 교체
            //         */

            //         existingTile.Name = name;
            //         existingTile.DataID = data.DataID;
            //         existingTile.ObjectType = EObjectType.Monster;

            //         EditorUtility.SetDirty(existingTile);
            //         Debug.Log($"{nameof(CreateObjectTile)}, Completed - SetDirty.");
            //     }
            //     else
            //     {
            //         string spriteName = data.IconImage.Replace(".sprite", "");
            //         Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/TileMaps/Dev/Monsters/{spriteName}.png");
            //         if (spr == null)
            //         {
            //             Debug.LogError($"{nameof(CreateObjectTile)}, Failed to load {data.DescriptionTextID} Sprite.");
            //             return;
            //         }

            //         CustomTile customTile = ScriptableObject.CreateInstance<CustomTile>();
            //         customTile.Name = data.DescriptionTextID;
            //         customTile.DataID = data.DataID;
            //         customTile.ObjectType = EObjectType.Monster;

            //         customTile.sprite = spr;
            //         AssetDatabase.CreateAsset(customTile, path);
            //         Debug.Log($"{nameof(CreateObjectTile)}, Completed - Create New Asset.");
            //     }
            // }