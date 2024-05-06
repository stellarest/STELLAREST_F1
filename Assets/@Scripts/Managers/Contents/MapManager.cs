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

        // cell의 어떤 좌표에 어떠한 물체가 존재하는지(CellPos, BaseObject)
        private Dictionary<Vector3Int, BaseObject> _cells = new Dictionary<Vector3Int, BaseObject>();

        // 이걸 벗어난 영역을 갈 수 없다고 생각하면 된다는데. 이건 내가 타일 찍는 방식을 바꿔버려서 일단 지켜봐야 할듯.
        // 근데 맞을걸? 일단 맵 하나만 생각하면 되므로
        private int _minX = 0;
        private int _maxX = 0;
        private int _minY = 0;
        private int _maxY = 0;

        public Vector3Int WorldToCell(Vector3 worldPos) => CellGrid.WorldToCell(worldPos);
        //public Vector3 CellToWorld(Vector3Int cellPos) => CellGrid.CellToWorld(cellPos);
        public Vector3 CellToWorld(Vector3Int cellPos) => CellGrid.GetCellCenterWorld(cellPos);

        // Tile_Collision에 칠해준 영역 
        // 어차피 촘촘하게 쫘악 그려줬으므로 굳이 set같은 컨테이너 말고 2차원 배열로 가고 있음.
        // 단순히 벽이 있느냐 없느냐를 체크하기 위한 용도
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
            
            TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}_Collision"); // --> 확인 필요
            StringReader stringReader = new StringReader(txt.text); // 파일 입출력(System.IO)

            _minX = int.Parse(stringReader.ReadLine());
            _maxX = int.Parse(stringReader.ReadLine());         
            _minY = int.Parse(stringReader.ReadLine());
            _maxY = int.Parse(stringReader.ReadLine());

            // int xCount = _maxX - _minX + 1; // 7 + 6 + 1 = 14  (+1 부분 빼야할수도 있음)
            // int yCount = _maxY - _minY + 1; // 5 + 9 + 1 = 15 (+1 부분 빼야할수도 있음)
            int xCount = _maxX - _minX;
            int yCount = _maxY - _minY;

            //_cellCollision = new ECellCollisionType[xCount, yCount];
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

            Debug.Log("Success: Load Tilemap Collision Data.");
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
            // 먼저 해당 Cell 위치에 물체가 있는지를 판별한다.
            BaseObject prev = GetObject(obj.CellPos);

            // 그런데 이런저런 이유로 아직 obj가 cell에 배치가 되지 않았는데 이 함수가 실행이 되거나,
            // 또는, 다른 오브젝트가 있다면 무시된다.
            if (prev != obj)
                return false;

            // 인자로 들어온 obj를 삭제하려고 하는데, 그 위치에 진짜로 obj가 있는지를 체크해서
            // 진짜로 그 위치에 obj가 있었으면 null로 밀어버림   
            _cells[obj.CellPos] = null;
            return true;
        }

        public bool AddObject(BaseObject obj, Vector3Int cellPos)
        {
            // 먼저 해당 위치의 Cell에 갈 수 있는지?
            if (CanMove(cellPos) == false)
            {
                Debug.LogWarning($"AddObject Failed.");
                return false;
            }

            // 이미 누군가 그 위치를 점령하고 있는가?
            BaseObject prev = GetObject(cellPos);
            if (prev != obj)
            {
                Debug.LogWarning($"AddObject Failed.");
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
            Debug.Log($"Tile[{x}][{y}]: {_cellCollisionType[y, x]}");

            // if (x < 0 || x >= _cellCollisionType.GetLength(1) || y < 0 || y >= _cellCollisionType.GetLength(0))
            // 애초에 처음부터 Out of range Exception 방지
            if (x < 0 || x >= _cellCollisionType.GetLength(1) || y < 0 || y >= _cellCollisionType.GetLength(0))
            {
                //Util.LogError($"{nameof(MapManager)}, {nameof(CanMove)}, Out of range - x:{x}, y:{y}");
                Debug.LogWarning($"{nameof(MapManager)}, {nameof(CanMove)}, Out of range - x:{x}, y:{y}");
                return false;
            }

            if (ignoreObjects == false) // default: false
            {
                BaseObject obj = GetObject(cellPos);
                if (obj != null)
                {
                    Debug.Log($"<color=brown>Obj({obj.name}) is on tile before.</magenta>");
                    return false;
                }
            }

            if (_cellCollisionType[y, x] == ECellCollisionType.CanMove)
            {
                Debug.Log("<color=cyan>CanMove.</color>");
                return true;
            }

            if (ignoreSemiWall && _cellCollisionType[y, x] == ECellCollisionType.SemiBlock)
            {
                Debug.Log("<color=brown>On Semi Wall.</color>");
                return false;
            }

            Debug.Log("<color=magenta>On Black.</color>");
            return false;
        }

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

        public void ClearObjects()
            => _cells.Clear();

        #endregion

#if UNITY_EDITOR
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