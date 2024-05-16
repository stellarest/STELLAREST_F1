using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace STELLAREST_F1
{
    public class CustomTile : Tile
    {
        [Space]
        [Space]
        [Header("For CustomTile")]
        public Define.EObjectType ObjectType;
        public int DataID;
        public string Name;
    }
}
