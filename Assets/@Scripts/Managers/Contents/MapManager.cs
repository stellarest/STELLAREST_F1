using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MapManager
    {
        public GameObject Map { get; private set; } = null;
        public string MapName { get; private set; } = null;
        public Grid CellGrid { get; private set; } = null;
        private Dictionary<Vector3Int, BaseObject> _cells = new Dictionary<Vector3Int, BaseObject>();

        private int _minX = 0;
        private int _maxX = 0;
        private int _minY = 0;
        private int _maxY = 0;

        // World를 먼저 Cell로 바꾸고
        public Vector3Int WorldToCell(Vector3 worldPos) => CellGrid.WorldToCell(worldPos);
        // Cell을 다시 World로 바꾸는데 CenterCell 기준의 월드로 바꿈
        public Vector3 CenteredCellToWorld(Vector3Int cellPos) => CellGrid.GetCellCenterWorld(cellPos);
        private ECellCollisionType[,] _cellCollisionType = null;

        public void LoadMap(string mapName)
        {
            DestroyMap();

            GameObject map = Managers.Resource.Instantiate(mapName);
            map.transform.position = Vector3.zero;
            map.name = $"@Map_{mapName}";

            Map = map;
            MapName = mapName;
            CellGrid = map.GetComponent<Grid>();

            ParseCollisionData(map, mapName);
        }

        private void ParseCollisionData(GameObject map, string mapName, string tileMap = "Tilemap_Collision")
        {
            // GameObject collision = Util.FindChild(map, tileMap, true);
            // if (collision != null)
            //     collision.SetActive(false);

            TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}_Collision");
            StringReader stringReader = new StringReader(txt.text); // StringReader, 파일 입출력(System.IO)

            _minX = int.Parse(stringReader.ReadLine());
            _maxX = int.Parse(stringReader.ReadLine());
            _minY = int.Parse(stringReader.ReadLine());
            _maxY = int.Parse(stringReader.ReadLine());

            int xCount = _maxX - _minX;
            int yCount = _maxY - _minY;

            _cellCollisionType = new ECellCollisionType[yCount, xCount];
            stringReader.ReadLine(); // 개행
            for (int y = 0; y < yCount; ++y)
            {
                string line = stringReader.ReadLine();
                for (int x = 0; x < xCount; ++x)
                {
                    switch (line[x])
                    {
                        case ReadOnly.Character.Map_Tool_Block_0:
                            _cellCollisionType[y, x] = ECellCollisionType.Block;
                            break;

                        case ReadOnly.Character.Map_Tool_CanMove_1:
                            _cellCollisionType[y, x] = ECellCollisionType.CanMove;
                            break;

                        case ReadOnly.Character.Map_Tool_SemiBlock_2:
                            _cellCollisionType[y, x] = ECellCollisionType.SemiBlock;
                            break;
                    }
                }
            }
        }

        public bool MoveTo(Creature creature, Vector3 position, bool forceMove = false)
            => MoveTo(creature, Managers.Map.WorldToCell(position), forceMove);

        public bool MoveTo(Creature creature, Vector3Int cellPos, bool forceMove = false)
        {
            if (CanMove(cellPos) == false)
                return false;

            RemoveObject(creature);
            AddObject(creature, cellPos);
            creature.SetCellPos(cellPos, forceMove);
            return true;
        }

        #region Helpers
        public BaseObject GetObject(Vector3Int cellPos)
            => _cells.TryGetValue(cellPos, out BaseObject value) ? value : null;

        public BaseObject GetObject(Vector3 worldPos)
        {
            Vector3Int cellPos = WorldToCell(worldPos);
            return GetObject(cellPos);
        }

        public bool RemoveObject(BaseObject obj)
        {
            BaseObject prev = GetObject(obj.CellPos);

            // 지우려고하는데 obj 자기 자신이 아니라면
            if (prev != obj)
                return false;

            _cells[obj.CellPos] = null;
            return true;
        }

        public bool AddObject(BaseObject obj, Vector3Int cellPos)
        {
            if (CanMove(cellPos) == false)
            {
                Debug.LogWarning($"AddObject Failed: {nameof(CanMove)}");
                return false;
            }

            BaseObject prev = GetObject(cellPos);
            if (prev != null) // TEMP
            {
                Debug.LogWarning($"AddObject Failed: {nameof(GetObject)}");
                return false;
            }

            _cells[cellPos] = obj;
            return true;
        }

        public bool CanMove(Vector3 worldPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
            => CanMove(WorldToCell(worldPos), ignoreObjects, ignoreSemiWall);

        public bool CanMove(Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
        {
            int x = cellPos.x - _minX;
            int y = _maxY - cellPos.y - 1;

            if (x < 0 || x >= _cellCollisionType.GetLength(1) || y < 0 || y >= _cellCollisionType.GetLength(0))
            {
                //Debug.Log($"Tile[{x}][{y}]: {_cellCollisionType[y, x]}");
                return false;
            }

            if (ignoreObjects == false)
            {
                BaseObject obj = GetObject(cellPos);
                if (obj != null)
                    return false;
            }

            if (_cellCollisionType[y, x] == ECellCollisionType.CanMove)
                return true;

            if (ignoreSemiWall && _cellCollisionType[y, x] == ECellCollisionType.SemiBlock)
                return true;

            return false;
        }

        public void DestroyMap()
        {
            ClearObjects();

            if (Map != null)
                Managers.Resource.Destroy(Map);
        }

        public void ClearObjects()
            => _cells.Clear();
        #endregion

        // 원거리 캐릭터가 뒤로 가게 하는것도 조정해보기
        #region A* PathFinding
        public struct PQNode : IComparable<PQNode>
        {
            public int H; // SQR DISTANCE
            public Vector3Int CellPos;
            public int Depth;

            public int CompareTo(PQNode other)
            {
                if (H == other.H)
                    return 0;

                return H < other.H ? 1 : -1;
            }
        }

        private List<Vector3Int> _delta = new List<Vector3Int>()
        {
            // U기준, 시계 방향
            new Vector3Int(0, 1, 0), // U
            new Vector3Int(1, 1, 0), // UR
            new Vector3Int(1, 0, 0), // R
            new Vector3Int(1, -1, 0), // DR
            new Vector3Int(0, -1, 0), // D
            new Vector3Int(-1, -1, 0), // LD
            new Vector3Int(-1, 0, 0), // L
            new Vector3Int(-1, 1, 0) // LU
        };

        // 모바일에서 꽤 무거운 작업이라 0.1초씩 코루틴으로 돌리던지 바꿔야될수도있음.
        // 그리고 몬스터는 maxDepth를 크게 줄 이유가 없긴함.
        // 또한, 현재 캐릭터가 무조건 8방향으로만 움직이기 때문에 지금처럼 CampDest가 중간에 끼면 와리가리할 수 있는 문제가 발생할 수 있음.
        // 하지만 이제 길찾기를 완성하게 됨으로써 알아서 둘러싸게 됨.
        public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, int maxDepth = 10)
        {
            Dictionary<Vector3Int, int> best = new Dictionary<Vector3Int, int>(); // key: pos = value: huristic

            // key: 오픈셋 인접노드 위치, value: 부모(이 부모를 통해서 key가 추가가 되었다)
            // parent[오픈셋다음위치] = 부모위치
            Dictionary<Vector3Int, Vector3Int> parent = new Dictionary<Vector3Int, Vector3Int>();

            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>(); // OpenList
            Vector3Int pos = startCellPos;
            Vector3Int dest = destCellPos;

            Vector3Int closestCellPos = startCellPos;
            int closestH = (dest - pos).sqrMagnitude; // Dest Heuristic

            {
                // Start
                int h = (dest - pos).sqrMagnitude;
                pq.Push(new PQNode() { H = h, CellPos = pos, Depth = 1 });
                parent[pos] = pos; // 자기 자신인 것이고
                best[pos] = h;
            }

            while (pq.Count > 0)
            {
                PQNode node = pq.Pop(); // 제일 좋은 후보
                pos = node.CellPos;

                if (pos == dest)
                    break;

                if (node.Depth >= maxDepth) // 무한으로 깊게 들어가진 않음.
                    break;

                // 원래는 여기에 ClosedSet을 넣는 부분
                // closedSet.Add(node) 뭐 이런식으로 되는 거고 // 기록을 하고. 역추적용이고.

                // 상하좌우 검색, 예약
                foreach (Vector3Int delta in _delta)
                {
                    Vector3Int next = pos + delta;
                    if (CanMove(next) == false) // 갈 수 없는 곳이면 오픈셋에 넣지 않는다.
                        continue;

                    int h = (dest - next).sqrMagnitude; // 인접노드에서 도착점에 대한 Heuristic 계산
                    // ***** 
                    // 오픈셋에서 빼고, 인접한 노드를 다시 OpenSet에 넣으려고 하는데 이미 오픈셋에 노드가 들어가 있을수가 있음
                    // 그 전 루프에서 pop으로 밸 때 인접한 노드를 추가했기 때문. 휴리스틱 값이 달라질 수 있기 때문에 
                    // 더 좋은 휴리스틱이 있으면 그것으로 교체해주는것임
                    // *****
                    if (best.ContainsKey(next) == false)
                        best[next] = int.MaxValue;

                    if (best[next] <= h)
                        continue;

                    // pop할 때 마다 pos가 달라지기때문에 휴리스틱도 달라질 수도 있음.
                    best[next] = h;

                    // Enqueue를 하면서 가장 좋은 값이 위로 올라옴. 그러니까 휴리스틱.
                    pq.Push(new PQNode() { H = h, CellPos = next, Depth = node.Depth + 1 });
                    parent[next] = pos; // ***** Next위치의 부모는 pos가 된다 *****
                       
                    if (closestH > h) // 이거 무조건 있어야함. 이게 없으면 만약 다른 녀석이 차지하면 막 난리날듯.
                    {
                        closestH = h;
                        closestCellPos = next;
                    }
                }
            }

            // RetracePath(startNode, targetNode);
            if (parent.ContainsKey(dest) == false)
                return TraceCellPath(parent, closestCellPos);

            return TraceCellPath(parent, dest);
        }

        private List<Vector3Int> TraceCellPath(Dictionary<Vector3Int, Vector3Int> parent, Vector3Int dest)
        {
            List<Vector3Int> cells = new List<Vector3Int>();
            if (parent.ContainsKey(dest) == false)
                return cells;

            /*
                (ex) Input
                1. parent[0, 0(시작)] = [0, 0]
                2. parent[1, -1] = [0, 0]
                3. parent[2, -1(도착)] = [1, -1]
            */

            Vector3Int now = dest;
            while (parent[now] != now)
            {
                /*
                    cells[0] = (2, -1)
                    cells[1] = (1, -1)
                */
                cells.Add(now);
                now = parent[now];
            }

            // cells[2] = (0, 0)
            cells.Add(now);
            cells.Reverse();
            return cells;
        }
        #endregion

#if UNITY_EDITOR
        public void CheckOnTile(Creature creature)
        {
            Debug.Log($"WorldPos: {creature.transform.position}");
            Vector3Int currentPos = Managers.Map.WorldToCell(creature.transform.position);
            int x = currentPos.x - _minX;
            int y = _maxY - currentPos.y - 1;
            if (x < 0 || x >= _cellCollisionType.GetLength(1) || y < 0 || y >= _cellCollisionType.GetLength(0))
            {
                Debug.LogWarning($"### Out of index ###");
                return;
            }

            Debug.Log($"xMin: {_minX}, xMax: {_maxX}, yMin: {_minY}, yMax: {_maxY}");
            Debug.Log($"Cell[{currentPos.x}][{currentPos.y}] | Tile[{x}][{y}]: {_cellCollisionType[y, x]}");
        }

        public void CheckOnTile(Vector3 worldPos)
        {
            // xMin: -6, xMax: 7, yMin: -8, yMax: 5
            // CellPos -6, 4 -> [0, 0]
            Vector3Int cellPos = CellGrid.WorldToCell(worldPos);
            int x = cellPos.x - _minX; // 이건 좌 - 우 이렇게 적용이 되지만
            int y = _maxY - cellPos.y - 1; // y는 x처럼 적용하면 안됨. 적용 불가. 정사각 기준

            if (x < 0 || x >= _cellCollisionType.GetLength(1) || y < 0 || y >= _cellCollisionType.GetLength(0))
            {
                Debug.LogWarning($"### Out of index ### | x: {x}, y: {y}");
                return;
            }

            Debug.Log($"CellPos: ({cellPos.x}, {cellPos.y})");
            Debug.Log($"Tile[{x}][{y}]: {_cellCollisionType[y, x]}");
        }
#endif
    }
}

/*
         // PREV - ORIGIN
        // public bool CanMove(Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
        // {
        //     if (cellPos.x < _minX || cellPos.x >= _maxX)
        //     {
        //         Debug.LogWarning("Out of X");
        //         return false;
        //     }
        //     if (cellPos.y < _minY || cellPos.y >= _maxY)
        //     {
        //         Debug.LogWarning("Out of Y");
        //         return false;
        //     }

        //     if (ignoreObjects == false)
        //     {
        //         // 이미 해당 위치에 오브젝트가 있는지
        //         BaseObject obj = GetObject(cellPos);
        //         if (obj != null)
        //         {
        //             Debug.LogWarning("WTF");
        //             return false;
        //         }
        //     }

        //     int x = cellPos.x - _minX;
        //     int y = _maxY - cellPos.y - 1;
        //     ECellCollisionType type = _cellCollisionType[y, x];
        //     if (type == ECellCollisionType.CanMove)
        //         return true;

        //     if (ignoreSemiWall && type == ECellCollisionType.SemiBlock)
        //         return true;

        //     return false;
        // }

     public (float dmgResult, bool isCritical) TakeDamage(CreatureController target, CreatureController attacker, SkillBase from)
        {
            // DODGE CHANCE OF TARGET
            if (UnityEngine.Random.Range(0f, 1f) <= target.Stat.DodgeChance)
                return (-1f, false);
            
            // DAMAGE FROM ATTACKER
            float armor = target.Stat.Armor;
            bool isCritical = false;
            float dmgSkill = UnityEngine.Random.Range(from.Data.MinDamage, from.Data.MaxDamage);
            float dmgResult = dmgSkill + (dmgSkill * attacker.Stat.DamageUpRate);
            if (UnityEngine.Random.Range(0f, 1f) <= attacker.Stat.CriticalChance || target[CrowdControlType.Targeted])
            {
                isCritical = true;
                float criticalRatio = UnityEngine.Random.Range(MIN_CRITICAL_RATIO, MAX_CRITICAL_RATIO);
                dmgResult = dmgResult + (dmgResult * criticalRatio);
            }

            dmgResult = dmgResult - (dmgResult * armor);
            return (dmgResult, isCritical);
        }

*/