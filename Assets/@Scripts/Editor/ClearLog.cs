using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using System;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STELLAREST_F1
{
    public class DataTransformer : EditorWindow
    {
#if UNITY_EDITOR
        // Mac: %(Command) #(Shift) J
        [MenuItem("Tools/ClearLog %#J")]
        public static void ClearLog()
        {
            Util.ClearLog();
        }

        [MenuItem("Tools/SimpleLog %#H")]
        public static void SimpleLog()
        {
            int[] numbers = {1, 2, 3, 4, 5};
            var plusNums = from n in numbers
                           select n + 1;

            foreach (var num in plusNums)
                Debug.Log($"num: {num}");
        }
#endif
    }
}
