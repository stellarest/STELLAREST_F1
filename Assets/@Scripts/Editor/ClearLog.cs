using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using System;
using System.Reflection;
using System.Collections;
using System.ComponentModel;


namespace STELLAREST_F1
{
    public class DataTransformer : EditorWindow
    {
#if UNITY_EDITOR
        [MenuItem("Tools/ClearLog %#J")]
        public static void ClearLog()
        {
            Util.ClearLog();
        }
#endif
    }
}
