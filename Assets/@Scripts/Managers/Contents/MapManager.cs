using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class MapManager
    {
        public GameObject Map { get; private set; } = null;
        public string MapName { get; private set; } = null;
        public Grid CellGrid { get; private set; } = null;

        // cell의 어떤 좌표에 어떠한 물체가 존재하는지
        private Dictionary<Vector3Int, BaseObject> _cells = new Dictionary<Vector3Int, BaseObject>();
    }
}
