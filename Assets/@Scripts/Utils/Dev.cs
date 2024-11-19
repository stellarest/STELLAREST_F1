using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
namespace STELLAREST_F1
{
    public static class Dev
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(object log, bool highlight = false)
        {
            if (highlight)
                Debug.Log($"<color=#FF6666>\"</color><color=#A3E635>[<color=#FF6666>!</color>]: {log}</color><color=#FF6666>\"</color>");
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
            Debug.LogWarning($"<color=#00FF22>[!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>\n<color=yellow>{log}</color>");
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
            Debug.LogError($"<color=red>[!!!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>\n<color=red>{log}</color>");
            Debug.Break();
        }

        [Conditional("UNITY_EDITOR")]
        public static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
            Log("... Clear ...");
        }

        public static GameObject SpawnCircleObj(Vector3Int spawnCellPos)
        {
            GameObject obj = Managers.Resource.Instantiate("TestCircleObject");
            obj.transform.position = Managers.Map.CellToCenterWorld(spawnCellPos);
            return obj;
        }

        #region Input
        public static bool Input_J
            => UnityEngine.Input.GetKeyDown(KeyCode.J);
        public static bool Input_K
            => UnityEngine.Input.GetKeyDown(KeyCode.K);
        public static bool Input_L
            => UnityEngine.Input.GetKeyDown(KeyCode.L);
        public static bool Input_M
            => UnityEngine.Input.GetKeyDown(KeyCode.M);
        public static bool Input_N
            => UnityEngine.Input.GetKeyDown(KeyCode.N);
        public static bool Input_O
            => UnityEngine.Input.GetKeyDown(KeyCode.T);
        public static bool Input_P
            => UnityEngine.Input.GetKeyDown(KeyCode.P);
        public static bool Input_Q
            => UnityEngine.Input.GetKeyDown(KeyCode.Q);
        public static bool Input_R
            => UnityEngine.Input.GetKeyDown(KeyCode.R);
        public static bool Input_S
            => UnityEngine.Input.GetKeyDown(KeyCode.S);
        public static bool Input_T
            => UnityEngine.Input.GetKeyDown(KeyCode.T);
        public static bool Input_U
            => UnityEngine.Input.GetKeyDown(KeyCode.U);
        #endregion
    }
}
#endif
