using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using Debug = UnityEngine.Debug;
using UnityEngine;
using UnityEditor;

using static STELLAREST_F1.Define;
using Unity.VisualScripting;

#if UNITY_EDITOR
namespace STELLAREST_F1
{
    public class DevEditor : EditorWindow
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

    public static class Dev
    {
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
            bool input = UnityEngine.Input.GetKeyDown(GetKey(eInput));
            if (input)
            {
                if (_inputTagDict.ContainsKey(eInput) == false)
                    _inputTagDict.Add(eInput, new DevInputTag(obj: obj, tag));
            }

            return input;
        }

        public static void Input(EInput eInput, object obj, object tag, Action trueCase, Action falseCase)
        {
            bool input = UnityEngine.Input.GetKeyDown(GetKey(eInput));
            if (input)
            {
                if (_inputTagDict.ContainsKey(eInput) == false)
                    _inputTagDict.Add(eInput, new DevInputTag(obj: obj, tag));

                DevInputTag value = _inputTagDict[eInput];
                value.Toggle = !value.Toggle;
                if (value.Toggle)
                    trueCase?.Invoke();
                else
                    falseCase?.Invoke();
            }
        }
        
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

        private static UnityEngine.KeyCode GetKey(EInput eInput)
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

                _ => throw new ArgumentOutOfRangeException($"{nameof(Dev)}::{nameof(GetKey)}", $"\nInvalid: {eInput}")
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
*/