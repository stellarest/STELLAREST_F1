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

        public void DestroyMap()
        {
            ClearObjects();

            if (Map != null)
                Managers.Resource.Destroy(Map);
        }

        private void ParseCollisionData(GameObject map, string mapName, string tileMap = "Tilemap_Collision")
        {
            // SummerForest_Field_Temp_Collision
            // GameObject collision = Util.FindChild(map, tileMap, true);
            // if (collision != null)
            //     collision.SetActive(false); // 타일을 찍은 부분이 실제로 보이면 안됨.
            
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
            =>  MoveTo(creature, Managers.Map.WorldToCell(position), forceMove);
        
        public bool MoveTo(Creature creature, Vector3Int cellPos, bool forceMove = false)
        {
            if (CanMove(cellPos) == false)
            {
                return false;
            }

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
            //Debug.Log($"Tile[{x}][{y}]: {_cellCollisionType[y, x]}");

            if (x < 0 || x >= _cellCollisionType.GetLength(1) || y < 0 || y >= _cellCollisionType.GetLength(0))
                return false;

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

        public void ClearObjects()
            => _cells.Clear();

        #endregion

#if UNITY_EDITOR
        public void CheckOnTile(Creature creature)
        {
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