using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Debug = UnityEngine.Debug;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using TMPro;
using static STELLAREST_F1.Define;
using Unity.VisualScripting;

#if UNITY_EDITOR
namespace STELLAREST_F1
{
    public class DevShortCut : EditorWindow
    {
        // Mac: %(Command) #(Shift) E
        [MenuItem("Tools/ClearLog %#E")]
        public static void ClearLog()
            => Dev.ClearLog();

        [MenuItem("Tools/SimpleLog %#H")]
        public static void TestPrint()
        {
            int[] numbers = { 1, 2, 3, 4, 5 };
            var plusNums = (from n in numbers
                           select n + 1).ToList();

            string result = string.Join(separator: ", ", plusNums.Select(n => n % 2 == 0 ? 
                                                        $"<color=red>{n}</color>" : $"<color=white>{n}</color>"));
            Dev.Log(result);

            string strA = $"<color=red>show</color>";
            string strB = $" me ";
            string strC = $"<color=white>the money</color>";
            string ret = strA + strB + strC;
            Dev.Log($"Ret: {ret}");
        }
    }

    public class DevInputTag
    {
        public DevInputTag(object obj, object tag)
        {
            Obj = obj;
            Tag = tag;
        }

        public object Obj { get; } = null;
        public object Tag { get; } = null;
        public bool Toggle { get; set; } = false;
    }

    [System.Serializable]
    public class DevCellObj
    {
        public Vector3Int CellPos = Vector3Int.zero;
        public BaseCellObject BaseCellObj = null;
    }

    public class DevMono : MonoBehaviour
    {
        [field: SerializeField] public List<DevCellObj> DevCellObjs { get; private set; }= new List<DevCellObj>();

        private IEnumerator Start()
        {
            while (true)
            {
                foreach (var pair in Managers.Map.Cells)
                {
                    DevCellObj devCellObj = DevCellObjs.Find(n => n.BaseCellObj == pair.Value);
                    if (devCellObj != null)
                        devCellObj.CellPos = pair.Key;
                }
                
                yield return null;
            }
        }

        private void Update()
        {
            // --- Input_F8
            if (Dev.Input(EInput.Input_F8, obj: nameof(Dev), tag: $"{nameof(Dev.PrintInputTagInfo)}"))
                Dev.PrintInputTagInfo();

            // --- Input_F1
            if (Dev.Input(EInput.Input_F1, obj: nameof(Dev), tag: $"{nameof(Dev.PrintObjsOnTheCells)}"))
                Dev.PrintObjsOnTheCells();

            // --- Input_F2
            Dev.Input(EInput.Input_F2, obj: nameof(Dev), tag: $"{nameof(Dev.ShowCellPosText)}", trueCase: () => 
            {
                Dev.ShowCellPosText(show: true);
                Dev.Log($"{nameof(Dev.ShowCellPosText)}, ON", highlight: true);
            }, falseCase: () => 
            {
                Dev.ShowCellPosText(show: false);
                Dev.Log($"{nameof(Dev.ShowCellPosText)}, OFF");
            }, startFlagCase: true);

            // --- Input_F3
            Dev.Input(EInput.Input_F3, obj: nameof(Dev), tag: $"{nameof(Dev.ShowTiles)}", trueCase: () => 
            {
                Dev.ShowTiles(true);
                Dev.Log($"{nameof(Dev.ShowTiles)}, ON", highlight: true);
            }, falseCase: () => 
            {
                Dev.ShowTiles(false);
                Dev.Log($"{nameof(Dev.ShowTiles)}, OFF");
            }, startFlagCase: false);
        }
    }


    public static class Dev
    {
        private static DevMono _devMono = null;

        public static DevMono AddDevMono(GameObject go)
        {
            _devMono = _devMono == null ? go.GetOrAddComponent<DevMono>() : _devMono;
            return _devMono;
        }

        public static void DestroyDevMono(GameObject go)
        {
            DevMono devMono = go.GetComponent<DevMono>();
            if (devMono == null)
                return;

            UnityEngine.Object.Destroy(devMono);
        }

        public static void AddDevCellObj(DevCellObj devCellObj)
        {
            if (_devMono == null)
            {
                LogError($"Dev::", $"{_devMono == null}", $"{nameof(devCellObj)}");
                return;
            }

            _devMono.DevCellObjs.Add(devCellObj);
        }

        public static void RemoveDevCellObj(BaseCellObject baseCellObj)
        {
            if (_devMono == null)
            {
                LogError($"Dev::", $"{nameof(RemoveDevCellObj)}");
                return;
            }

            DevCellObj devCellObj = _devMono.DevCellObjs.Find(n => n.BaseCellObj == baseCellObj);
            _devMono.DevCellObjs.Remove(devCellObj);
        }

        // #FF6666
        // #A3E635
        [Conditional("UNITY_EDITOR")]
        public static void Log(object log, bool highlight = false)
        {
            if (highlight)
                Debug.Log($"<color=#FF6666>\"</color><color=#A3E635>{log}</color><color=#FF6666>\"</color>");
            else
                Debug.Log($"{log}");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object obj, object method)
        {
            Debug.LogWarning($"<color=#00FF22>[!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object obj, object method, object log)
        {
            Debug.LogWarning($"<color=#00FF22>[!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>\n<color=white>→ </color><color=#FFD966>{log}</color>");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(object obj, object method)
        {
            Debug.LogError($"<color=red>[!!!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>");
            Debug.Break();
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(object obj, object method, object log)
        {
            Debug.LogError($"<color=red>[!!!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>\n<color=white>→ </color><color=#F44336>{log}</color>");
            Debug.Break();
        }

        [Conditional("UNITY_EDITOR")]
        public static void ClearLog(bool showClearLog = true)
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);

            if (showClearLog)
                Log("... Clear ...");
        }

        public static GameObject SpawnCircleObj(Vector3Int spawnCellPos)
        {
            GameObject obj = Managers.Resource.Instantiate("TestCircleObject");
            obj.transform.position = Managers.Map.CellToCenterWorld(spawnCellPos);
            return obj;
        }

        #region Input
        private static Dictionary<EInput, DevInputTag> _inputTagDict = new Dictionary<EInput, DevInputTag>();
        public static bool Input(EInput eInput, object obj, object tag)
        {
            bool input = UnityEngine.Input.GetKeyDown(GetDevInputKey(eInput));
            if (input)
            {
                if (_inputTagDict.ContainsKey(eInput) == false)
                    _inputTagDict.Add(eInput, new DevInputTag(obj: obj, tag));
            }

            return input;
        }

        public static void Input(EInput eInput, object obj, object tag, Action trueCase, Action falseCase, bool startFlagCase = true)
        {
            bool input = UnityEngine.Input.GetKeyDown(GetDevInputKey(eInput));
            if (input)
            {
                if (_inputTagDict.ContainsKey(eInput) == false)
                {
                    _inputTagDict.Add(eInput, new DevInputTag(obj: obj, tag));
                    if (startFlagCase == false)
                        _inputTagDict[eInput].Toggle = true;
                }

                DevInputTag value = _inputTagDict[eInput];
                value.Toggle = !value.Toggle;
                if (value.Toggle)
                    trueCase?.Invoke();
                else
                    falseCase?.Invoke();
            }
        }
        
        #region Dev Input Tags (0 ~ 9, F1 ~ F8)
        public static void PrintInputTagInfo()
        {
            ClearLog(showClearLog: false);
            List<string> inputs = new List<string>();
            for (int i = 0; i < (int)EInput.Max; ++i)
            {
                string input = ((EInput)i).ToString();
                if (_inputTagDict.ContainsKey((EInput)i))
                {
                    DevInputTag value = _inputTagDict[(EInput)i];
                    // #DFEFEA
                    //inputs.Add($"<color=red>*{input}</color><color=#FFC300>({value.Obj}, {value.Tag})</color>\n");
                    inputs.Add($"<color=red>*{input}(</color><color=#DFEFEA>obj: </color><color=#FFC300>{value.Obj}</color>, <color=#DFEFEA>tag: </color><color=#80FF00>{value.Tag}</color><color=red>)</color>\n");
                }
                else
                    inputs.Add($"{input}\n");
            }

            string result = string.Join(separator: "", values: inputs);
            Log($"<color=red>* registered</color><color=white>,</color> * empty\n→ <color=white>[</color> \n{result} <color=white>]</color>");
        }

        public static void PrintObjsOnTheCells()
        {
            ClearLog(showClearLog: false);
            Log("===== Cells Pair =====");

            bool isOnTheCellObjFlag = false;
            foreach (var pair in Managers.Map.Cells)
            {
                if (pair.Value != null)
                {
                    Log($"({pair.Key}, {pair.Value.gameObject.name}", highlight: true);
                    isOnTheCellObjFlag = true;
                }
            }

            if (isOnTheCellObjFlag == false)
                Log("=== None of Objs on the Cells ===");
        }

        private static GameObject _cellPosTextRoot = null;
        public static void ShowCellPosText(bool show)
        {
            if (_cellPosTextRoot == null)
            {
                _cellPosTextRoot = new GameObject { name = "@CellPos" };
                MakeCellPosText(_cellPosTextRoot);
            }

            _cellPosTextRoot.SetActive(show);
        }

        private static void MakeCellPosText(GameObject cellPosTextRoot)
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

            SortingGroup sg = cellPosTextRoot.AddComponent<SortingGroup>();
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
                    cell.transform.SetParent(cellPosTextRoot.transform);
                }
            }

        }

        public static void ShowTiles(bool show)
        {
            GameObject map = GameObject.Find("@Map_SummerForestField_Test2");
            GameObject tile = null;
            tile = Util.FindChild(map, "Tilemap_Collision", true, true);
            if (tile != null)
                tile.SetActive(show);

            tile = Util.FindChild(map, "Tilemap_Object", true, true);
            if (tile != null)
                tile.SetActive(show);
        }
        #endregion

        private static UnityEngine.KeyCode GetDevInputKey(EInput eInput)
        {
            return eInput switch
            {
                EInput.Input_0 => KeyCode.Alpha0, EInput.Input_1 => KeyCode.Alpha1,
                EInput.Input_2 => KeyCode.Alpha2, EInput.Input_3 => KeyCode.Alpha3,
                EInput.Input_4 => KeyCode.Alpha4, EInput.Input_5 => KeyCode.Alpha5,
                EInput.Input_6 => KeyCode.Alpha6, EInput.Input_7 => KeyCode.Alpha7,
                EInput.Input_8 => KeyCode.Alpha8, EInput.Input_9 => KeyCode.Alpha9,

                EInput.Input_F1 => KeyCode.F1, EInput.Input_F2 => KeyCode.F2,
                EInput.Input_F3 => KeyCode.F3, EInput.Input_F4 => KeyCode.F4,
                EInput.Input_F5 => KeyCode.F5, EInput.Input_F6 => KeyCode.F6,
                EInput.Input_F7 => KeyCode.F7, EInput.Input_F8 => KeyCode.F8,

                _ => throw new ArgumentOutOfRangeException($"{nameof(Dev)}::{nameof(GetDevInputKey)}", $"\nInvalid: {eInput}")
            };
        }
        #endregion
    }
}
#endif

/*
    [Conditional("UNITY_EDITOR")]
    public static void Log(object log, bool highlight = false)
    {
        if (highlight)
            Debug.Log($"<color=#FF6666>\"</color><color=#A3E635>[<color=#FF6666>*</color>]: {log}</color><color=#FF6666>\"</color>");
        else
            Debug.Log($"{log}");
    }

    // deprecated: DevManager
    

*/