using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.Tilemaps;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class MapManager
    {
        public GameObject Map { get; private set; } = null;
        public string MapName { get; private set; } = null;
        public Grid CellGrid { get; private set; } = null;

        // 현재 사용하고 있는 이 Cell에 쿼드트리를 얹히면 좋다고 함(TODO LIST)
        // 스타크래프트 유닛은 아마 일일이 칸 단위로 찾진 않을것임
        // 갈 수 있는지 큼지막하게 먼저 판단하고 먼저 갈수있는 기본적인것부터 셋팅하는 등의 방법을 썼을 것이라고 함
        private Dictionary<Vector3Int, BaseObject> _cells = new Dictionary<Vector3Int, BaseObject>();
        public Dictionary<Vector3Int, BaseObject> Cells => _cells;

        public int MinX { get; private set; } = 0;
        public int MaxX { get; private set; } = 0;
        public int MinY { get; private set; } = 0;
        public int MaxY { get; private set; } = 0;

        // World를 먼저 Cell로 바꾸고
        public Vector3 CellToWorld(Vector3Int cellPos) => CellGrid.CellToWorld(cellPos);
        public Vector3 CenteredCellToWorld(Vector3Int cellPos) => CellGrid.GetCellCenterWorld(cellPos);
        public Vector3Int WorldToCell(Vector3 worldPos) => CellGrid.WorldToCell(worldPos);
        public Vector3Int CenteredCellPos(Vector3Int cellPos)
        {
            Vector3 center = CenteredCellToWorld(cellPos);
            return WorldToCell(center);
        }

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
            SpawnObjectsByData(map, mapName);
        }

        private void ParseCollisionData(GameObject map, string mapName, string tileMap = "Tilemap_Collision")
        {
            // GameObject collision = Util.FindChild(map, tileMap, true);
            // if (collision != null)
            //     collision.SetActive(false);
            TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}_Collision");
            StringReader stringReader = new StringReader(txt.text); // StringReader, 파일 입출력(System.IO)

            MinX = int.Parse(stringReader.ReadLine());
            MaxX = int.Parse(stringReader.ReadLine());
            MinY = int.Parse(stringReader.ReadLine());
            MaxY = int.Parse(stringReader.ReadLine());

            int xCount = MaxX - MinX;
            int yCount = MaxY - MinY;

            _cellCollisionType = new ECellCollisionType[yCount, xCount];
            stringReader.ReadLine(); // \n
            for (int y = 0; y < yCount; ++y)
            {
                string line = stringReader.ReadLine();
                for (int x = 0; x < xCount; ++x)
                {
                    switch (line[x])
                    {
                        case ReadOnly.Util.Map_Tool_Block_0:
                            _cellCollisionType[y, x] = ECellCollisionType.Block;
                            break;

                        case ReadOnly.Util.Map_Tool_CanMove_1:
                            _cellCollisionType[y, x] = ECellCollisionType.CanMove;
                            break;

                        case ReadOnly.Util.Map_Tool_SemiBlock_2:
                            _cellCollisionType[y, x] = ECellCollisionType.SemiBlock;
                            break;
                    }
                }
            }
        }

        private void SpawnObjectsByData(GameObject map, string mapName, string tilemap = "Tilemap_Object")
        {
            Tilemap tm = Util.FindChild<Tilemap>(map, tilemap, true);
            if (tm != null)
                tm.gameObject.SetActive(false);

            // Debug.Log($"xMin: {tm.cellBounds.xMin}");
            // Debug.Log($"xMax: {tm.cellBounds.xMax}");
            // Debug.Log($"yMin: {tm.cellBounds.yMin}");
            // Debug.Log($"yMax: {tm.cellBounds.yMax}");
            // DBDBD

            for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; --y)
            {
                for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; ++x)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    CustomTile tile = tm.GetTile(cellPos) as CustomTile;
                    if (tile == null)
                        continue;

                    if (tile.ObjectType == EObjectType.Env)
                    {
                    }
                    else if (tile.ObjectType == EObjectType.Monster)
                    {
                        // --- Respawn Spawn Data 필요
                        // --- CHICKEN IN TILE TEST(FIX)
                        Monster monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, tile.DataID);
                        //monster.SetCellPos(cellPos, stopLerpToCell: false, forceMove: true);
                        MoveTo(monster, cellPos, stopLerpToCell: true, forceMove: true);
                        monster.InitialSpawnedCellPos = cellPos;

                        // --- MORE CHICKEN
                        int current = 0;
                        int spawnCount = 2;
                        int attemptSpawnCount = 0;
                        while (attemptSpawnCount < 100)
                        {
                            cellPos = new Vector3Int(UnityEngine.Random.Range(cellPos.x - 5, cellPos.x + 5),
                                                    UnityEngine.Random.Range(cellPos.y - 5, cellPos.y + 5), 0);
                            if (Managers.Map.CanMove(cellPos))
                            {
                                if (spawnCount == current)
                                    break;

                                monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, ReadOnly.DataAndPoolingID.DNPID_Monster_Chicken);
                                MoveTo(monster, cellPos, stopLerpToCell: true, forceMove: true);
                                monster.InitialSpawnedCellPos = cellPos;
                                ++current;
                            }
                            else
                            {
                                ++attemptSpawnCount;
                                Debug.Log("Fail to spawn Turkey");
                            }
                        }

                        // --- TURKEY TEST
                        current = 0;
                        spawnCount = 2;
                        attemptSpawnCount = 0;
                        while (attemptSpawnCount < 100)
                        {
                            cellPos = new Vector3Int(UnityEngine.Random.Range(cellPos.x - 5, cellPos.x + 5),
                                                    UnityEngine.Random.Range(cellPos.y - 5, cellPos.y + 5), 0);
                            if (Managers.Map.CanMove(cellPos))
                            {
                                if (spawnCount == current)
                                    break;

                                monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, ReadOnly.DataAndPoolingID.DNPID_Monster_Turkey);
                                MoveTo(monster, cellPos, stopLerpToCell: true, forceMove: true);
                                monster.InitialSpawnedCellPos = cellPos;
                                ++current;
                            }
                            else
                            {
                                ++attemptSpawnCount;
                                Debug.Log("Fail to spawn Turkey");
                            }
                        }

                        // --- BUNNY TEST
                        current = 0;
                        spawnCount = 2;
                        attemptSpawnCount = 0;
                        while (attemptSpawnCount < 100)
                        {
                            cellPos = new Vector3Int(UnityEngine.Random.Range(cellPos.x - 5, cellPos.x + 5),
                                                    UnityEngine.Random.Range(cellPos.y - 5, cellPos.y + 5), 0);
                            if (Managers.Map.CanMove(cellPos))
                            {
                                if (spawnCount == current)
                                    break;

                                monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, ReadOnly.DataAndPoolingID.DNPID_Monster_Bunny);
                                MoveTo(monster, cellPos, stopLerpToCell: true, forceMove: true);
                                monster.InitialSpawnedCellPos = cellPos;
                                ++current;
                            }
                            else
                            {
                                ++attemptSpawnCount;
                                Debug.Log("Fail to spawn Bunny");
                            }
                        }

                        // --- PUG TEST
                        current = 0;
                        spawnCount = 2;
                        attemptSpawnCount = 0;
                        while (attemptSpawnCount < 100)
                        {
                            cellPos = new Vector3Int(UnityEngine.Random.Range(cellPos.x - 5, cellPos.x + 5),
                                                    UnityEngine.Random.Range(cellPos.y - 5, cellPos.y + 5), 0);
                            if (Managers.Map.CanMove(cellPos))
                            {
                                if (spawnCount == current)
                                    break;

                                monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, ReadOnly.DataAndPoolingID.DNPID_Monster_Pug);
                                MoveTo(monster, cellPos, stopLerpToCell: true, forceMove: true);
                                monster.InitialSpawnedCellPos = cellPos;
                                ++current;
                            }
                            else
                            {
                                ++attemptSpawnCount;
                                Debug.Log("Fail to spawn Pug");
                            }
                        }

                        // // TEST MULTIPLE
                        // int currentMonsterCount = 0;
                        // int maxMonsterCount = UnityEngine.Random.Range(10, 20);
                        // while (currentMonsterCount < maxMonsterCount)
                        // {
                        //     Vector3Int randPos = new Vector3Int
                        //         (UnityEngine.Random.Range(monster.CellPos.x - 10, monster.CellPos.x + 10), 
                        //         UnityEngine.Random.Range(monster.CellPos.x - 10, monster.CellPos.x + 10), 0);

                        //     if (Managers.Map.CanMove(randPos) == false)
                        //         continue;

                        //     currentMonsterCount++;

                        //     int randID = UnityEngine.Random.Range(ReadOnly.DataAndPoolingID.DNPID_Monster_Chicken, 
                        //                                         ReadOnly.DataAndPoolingID.DNPID_Monster_Pug + 1);

                        //     monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, randID);
                        //     MoveTo(monster, randPos, stopLerpToCell: true, forceMove: true);
                        //     monster.InitialSpawnedCellPos = randPos;
                        // }

                        // // TEST: NEAR1
                        // cellPos = new Vector3Int(UnityEngine.Random.Range(cellPos.x + 3, cellPos.x + 5),
                        //                         UnityEngine.Random.Range(cellPos.y + 3, cellPos.y + 5), 0);
                        // if (Managers.Map.CanMove(cellPos))
                        // {
                        //     monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, tile.DataID);
                        //     MoveTo(monster, cellPos, stopLerpToCell: true, forceMove: true);
                        //     monster.InitialSpawnedCellPos = cellPos;
                        // }

                        // // TEST: NEAR2
                        // cellPos = new Vector3Int(UnityEngine.Random.Range(cellPos.x - 3, cellPos.x - 5),
                        //                         UnityEngine.Random.Range(cellPos.y + 3, cellPos.y + 5), 0);
                        // if (Managers.Map.CanMove(cellPos))
                        // {
                        //     monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, tile.DataID);
                        //     MoveTo(monster, cellPos, stopLerpToCell: true, forceMove: true);
                        //     monster.InitialSpawnedCellPos = cellPos;
                        // }

                        // // TEST: NEAR3
                        // cellPos = new Vector3Int(UnityEngine.Random.Range(cellPos.x + 3, cellPos.x + 5),
                        //                         UnityEngine.Random.Range(cellPos.y + 3, cellPos.y + 5), 0);
                        // if (Managers.Map.CanMove(cellPos))
                        // {
                        //     monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, tile.DataID);
                        //     MoveTo(monster, cellPos, stopLerpToCell: true, forceMove: true);
                        //     monster.InitialSpawnedCellPos = cellPos;
                        // }

                        // TEST: 9,5
                        // monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, tile.DataID);
                        // MoveTo(monster, new Vector3Int(9, 5, 0), stopLerpToCell: true, forceMove: true);
                        // monster.InitialSpawnedCellPos = new Vector3Int(9, 5, 0);

                        // TEST2
                        // Vector3Int monPos2 = monster.CellPos;
                        // monPos2 += new Vector3Int(3, 3, 0);
                        // monster = Managers.Object.Spawn<Monster>(EObjectType.Monster, tile.DataID);
                        // MoveTo(monster, monPos2, stopLerpToCell: true, forceMove: true);
                    }
                }
            }
        }

        public void UpdateCellPos()
        {
        }

        public bool MoveTo(Creature creature, Vector3 position, bool stopLerpToCell = false, bool forceMove = false, EObjectType ignoreObjectType = EObjectType.None)
            => MoveTo(creature: creature, cellPos: Managers.Map.WorldToCell(position), stopLerpToCell: stopLerpToCell, forceMove: forceMove, ignoreObjectType);

        public bool MoveTo(Creature creature, Vector3Int cellPos, bool stopLerpToCell = false, bool forceMove = false, EObjectType ignoreObjectType = EObjectType.None)
        {
            if (CanMove(cellPos, ignoreObjectType) == false)
                return false;

            RemoveObject(creature);
            AddObject(creature, cellPos);
            creature.SetCellPos(cellPos, stopLerpToCell, forceMove);
            return true;
        }

        public void WarpTo(Creature creature, Vector3Int destCellPos, Action warpEndCallback = null)
        {
            if (creature.Target.IsValid())
                return;

            if (CanMove(destCellPos) == false)
            {
                List<Vector3Int> path = FindPath(creature.CellPos, destCellPos: destCellPos, ReadOnly.Util.CreatureWarpMoveDepth);
                path.Reverse();
                for (int i = 0; i < path.Count; ++i)
                {
                    if (CanMove(path[i]) == false)
                        continue;

                    destCellPos = path[i]; // Leader 근처에 가장 가까운 위치를 찾는다.
                    break;
                }
            }

            creature.Target = null;
            RemoveObject(creature);
            AddObject(creature, destCellPos);
            creature.SetCellPos(destCellPos, stopLerpToCell: true, forceMove: true);
            warpEndCallback?.Invoke(); // 나중에 이펙트추가되면 여기다가 설정하면 됨.
        }

        public void MoveLeader(Hero leader, Vector3 targetPosition)
        {
            // 다른 애들이랑 중복되면 안됨
            // Vector3Int prevCellPos = WorldToCell(leader.transform.position);
            // if (prevCellPos != targetCellPos)
            // {
            //     // 리더 이거 하면 안될듯.
            //     _cells[prevCellPos] = null;
            //     _cells[targetCellPos] = leader;
            // }

            // Vector3Int targetCellPos = WorldToCell(targetPosition);
            // leader.SetCellPos(cellPos: targetCellPos, stopLerpToCell: true, forceMove: false);
            // leader.transform.position = targetPosition;
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
                return false;

            BaseObject prev = GetObject(cellPos);
            if (prev != null)
                return false;

            _cells[cellPos] = obj;
            return true;
        }

        // 이걸로 전부 교체 예정
        public bool CanMove(Vector3 worldPos, EObjectType ignoreObjectType)
            => CanMove(WorldToCell(worldPos), ignoreObjectType);

        public bool CanMove(Vector3Int cellPos, EObjectType ignoreObjectType)
        {
            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y - 1;
            if (x < 0 || x >= _cellCollisionType.GetLength(1) || y < 0 || y >= _cellCollisionType.GetLength(0))
                return false;

            if (_cellCollisionType[y, x] == ECellCollisionType.SemiBlock)
                return false;

            BaseObject obj = GetObject(cellPos);
            if (ignoreObjectType == EObjectType.None)
            {
                if (obj != null)
                    return false;
            }
            else if (obj != null && obj.ObjectType != ignoreObjectType)
                return false;

            if (_cellCollisionType[y, x] == ECellCollisionType.CanMove)
                return true;

            return false;
        }

        // 이거 제거 예정
        public bool CanMove(Vector3 worldPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
            => CanMove(WorldToCell(worldPos), ignoreObjects, ignoreSemiWall);

        public bool CanMove(Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
        {
            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y - 1;

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

        public List<Vector3Int> DeltaPos { get; }= new List<Vector3Int>()
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
        public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, int maxDepth = 10, EObjectType ignoreObjectType = EObjectType.None)
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
                foreach (Vector3Int delta in DeltaPos)
                {
                    Vector3Int next = pos + delta;
                    // if (CanMove(next, ignoreObjects: ignoreObjects) == false) // 갈 수 없는 곳이면 오픈셋에 넣지 않는다.
                    //     continue;

                    if (CanMove(next, ignoreObjectType) == false) // 갈 수 없는 곳이면 오픈셋에 넣지 않는다.
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
            // 여기일것같음
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
            int x = currentPos.x - MinX;
            int y = MaxY - currentPos.y - 1;
            if (x < 0 || x >= _cellCollisionType.GetLength(1) || y < 0 || y >= _cellCollisionType.GetLength(0))
            {
                Debug.LogWarning($"### Out of index ###");
                return;
            }

            Debug.Log($"xMin: {MinX}, xMax: {MaxX}, yMin: {MinY}, yMax: {MaxY}");
            Debug.Log($"Cell[{currentPos.x}][{currentPos.y}] | Tile[{x}][{y}]: {_cellCollisionType[y, x]}");
        }

        public void CheckOnTile(Vector3 worldPos)
        {
            // xMin: -6, xMax: 7, yMin: -8, yMax: 5
            // CellPos -6, 4 -> [0, 0]
            Vector3Int cellPos = CellGrid.WorldToCell(worldPos);
            int x = cellPos.x - MinX; // 이건 좌 - 우 이렇게 적용이 되지만
            int y = MaxY - cellPos.y - 1; // y는 x처럼 적용하면 안됨. 적용 불가. 정사각 기준

            if (x < 0 || x >= _cellCollisionType.GetLength(1) || y < 0 || y >= _cellCollisionType.GetLength(0))
            {
                Debug.LogWarning($"### Out of index ### | x: {x}, y: {y}");
                return;
            }

            Debug.Log($"CellPos: ({cellPos.x}, {cellPos.y})");
            Debug.Log($"Tile[{x}][{y}]: {_cellCollisionType[y, x]}");
        }

        public void PrintCollisionTile()
        {
            // --- Check Collision Tile Type
            int minX = MinX;
            int maxX = MaxX;
            int minY = MinY;
            int maxY = MaxY;
            for (int i = minX; i < maxX; ++i)
            {
                for (int j = maxY - 1; j >= minY; --j)
                {
                    Vector3Int cellPos = new Vector3Int(i, j);
                    Vector3 cellToWorld = CellToWorld(cellPos);
                    Managers.Map.CheckOnTile(cellToWorld);
                }
            }
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

        // public bool CanMoveDeltaPos(Creature creature, bool ignoreObjects = false)
        // {
        //     Vector3Int currentCellPos = WorldToCell(creature.transform.position);
        //     for (int i = 0; i < DeltaPos.Count; ++i)
        //     {
        //         if (CanMove(currentCellPos + DeltaPos[i], ignoreObjects) == false)
        //             return false;
        //     }

        //     return true;
        // }

*/