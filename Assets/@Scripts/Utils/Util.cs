using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

using Debug = UnityEngine.Debug;
using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;

namespace STELLAREST_F1
{
    public static class Util
    {
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();

            return component;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false, bool inActiveTarget = false)
        {
            Transform transform = FindChild<Transform>(go, name, recursive, inActiveTarget);
            if (transform == null)
                return null;

            return transform.gameObject;
        }

        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false, bool inActiveTarget = false) where T : UnityEngine.Object
        {
            if (go == null)
                return null;

            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; ++i)
                {
                    Transform transform = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        T component = transform.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
            }
            else
            {
                foreach (T component in go.GetComponentsInChildren<T>(includeInactive: inActiveTarget))
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }

            return null;
        }

        public static EObjectType GetTargetType(EObjectType ownerType, bool isAlly)
        {
            if (ownerType == EObjectType.Hero)
                return isAlly ? EObjectType.Hero : EObjectType.Monster;
            else if (ownerType == EObjectType.Monster)
                return isAlly ? EObjectType.Monster : EObjectType.Hero;

            return EObjectType.None;
        }

        public static T GetEnumFromString<T>(string value) where T : struct, Enum
        {
            if (System.Enum.TryParse<T>(value, out T enumValue))
                return enumValue;

            return default(T);
        }

        public static string GetStringFromEnum<T>(T enumValue) where T : struct, Enum
            => enumValue.ToString();

        public static Type GetTypeFromClassName(string className)
        {
            EClassName eClassName = GetEnumFromString<EClassName>(className);
            switch (eClassName)
            {
                case EClassName.DefaultSkillBase:
                    return typeof(DefaultSkillBase);

                case EClassName.ActiveSkillBase:
                    return typeof(ActiveSkillBase);

                case EClassName.Projectile:
                    return typeof(Projectile);

                case EClassName.StraightMotion:
                    return typeof(StraightMotion);

                case EClassName.ParabolaMotion:
                    return typeof(ParabolaMotion);

                case EClassName.BodyAttack:
                    return typeof(BodyAttack);

                case EClassName.CreatureAI:
                    return typeof(CreatureAI);

                case EClassName.HeroAI:
                    return typeof(HeroAI);

                case EClassName.MeleeHeroAI:
                    return typeof(MeleeHeroAI);

                case EClassName.RangedHeroAI:
                    return typeof(RangedHeroAI);

                case EClassName.MonsterAI:
                    return typeof(MonsterAI);

                default:
                    Dev.LogError(obj: nameof(Util), method: nameof(GetTypeFromClassName), log: $"Input: {className}, Please check Define.EClassName.");
                    return null;
            }
        }

        // string value -> Enum Type, true : 대소문자 구분 안함.
        public static T ParseEnum<T>(string value)
            => (T)Enum.Parse(typeof(T), value, true);

        public static Vector3Int MakeSpawnPosition(Vector3 spawnPos)
        {
            int _trySpawnCount = 0;
            float randMinPos = -1f;
            float randMaxPos = 1f;
            Vector3Int cellSpawnPos = Managers.Map.WorldToCell(spawnPos);
            while (Managers.Map.CanMove(cellSpawnPos) == false)
            {
                if (_trySpawnCount++ >= 999)
                    return Vector3Int.zero;

                // float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Rad2Deg;
                // float dist = UnityEngine.Random.Range(randMinPos--, randMaxPos++);

                // float x = Mathf.Cos(angle) * dist;
                // float y = Mathf.Sin(angle) * dist;

                float angle = UnityEngine.Random.Range(0f, 360f);
                float rad = angle * Mathf.Deg2Rad;
                float dist = UnityEngine.Random.Range(randMinPos--, randMaxPos++);

                float x = Mathf.Cos(rad) * dist;
                float y = Mathf.Sin(rad) * dist;

                cellSpawnPos = Managers.Map.WorldToCell(spawnPos + new Vector3(x, y, 0));
            }

            return cellSpawnPos;
        }

        public static Vector3 GetRandomQuadPosition(Vector3 from, float cellSize = 1f)
        {
            float offset = cellSize / 4.0f;
            Vector3[] quadCenters = new Vector3[4];
            quadCenters[0] = from + new Vector3(offset, offset, 0);       // --- 1사분면 중앙
            quadCenters[1] = from + new Vector3(-offset, offset, 0);      // --- 2사분면 중앙
            quadCenters[2] = from + new Vector3(-offset, -offset, 0);     // --- 3사분면 중앙
            quadCenters[3] = from + new Vector3(offset, -offset, 0);      // --- 4사분면 중앙
            int randIdx = UnityEngine.Random.Range(0, quadCenters.Length + 1);
            if (randIdx == quadCenters.Length)                             // --- Center
                return from;

            return quadCenters[randIdx];
        }

        // Chase Or Attack
        public static float CalculateValueFromDistance(float value, float maxValue,
                                                       float distanceToTargetSQR, float maxDistanceSQR,
                                                       bool increaseWithDistance = true)
        {
            if (increaseWithDistance)
                value = Mathf.Lerp(value, maxValue, Mathf.Log(distanceToTargetSQR + 0.1f) / Mathf.Log(maxDistanceSQR * maxDistanceSQR + 0.1f));
            else
                value = Mathf.Lerp(maxValue, value, Mathf.Log(distanceToTargetSQR + 0.1f) / Mathf.Log(maxDistanceSQR * maxDistanceSQR + 0.1f));
            return value;
        }

        public static float Distance(Vector3 pos1, Vector3 pos2, bool isSQR = true)
        {
            if (isSQR)
                return (pos1 - pos2).sqrMagnitude;
            else
                return (pos1 - pos2).magnitude;
        }

        // public static float DistanceFromCellCenter(Vector3 worldPos)
        // {
        //     Vector3 cellCenter = Managers.Map.GetCenterWorld(Managers.Map.WorldToCell(worldPos));
        //     float distX = Mathf.Abs(worldPos.x - cellCenter.x);
        //     float distY = Mathf.Abs(worldPos.y - cellCenter.y);
        //     return Mathf.Max(distX, distY);
        // }

        // public static bool IsNearCellCenter(Vector3 worldPos, float threshold = 0.5f)
        //     => DistanceFromCellCenter(worldPos) <= threshold;

        public static bool IsNearCellCenter(BaseObject baseObj, Vector3Int cellPos, float threshold = 0.1f)
        {
            Vector3 center = Managers.Map.CellToCenterWorld(cellPos);
            if ((center - baseObj.transform.position).sqrMagnitude < threshold * threshold)
                return true;

            return false;
        }

        public static int GetPoolingID(EObjectType objType, int dataID)
        {
            if (dataID == -1)
                return -1;

            switch (objType)
            {
                case EObjectType.Hero:
                    dataID = dataID | (1 << 31);
                    break;

                case EObjectType.Monster:
                    dataID = dataID | (1 << 30);
                    break;

                case EObjectType.Env:
                    dataID = dataID | (1 << 29);
                    break;

                case EObjectType.Projectile:
                    dataID = dataID | (1 << 28);
                    break;

                case EObjectType.Effect:
                    dataID = dataID | (1 << 27);
                    break;
            }

            return dataID;
        }

        private static HashSet<EEffectType> s_effectStatTypes = new HashSet<EEffectType>
        {
            EEffectType.MainStat_MaxHealth,
            EEffectType.MainStat_Damage,
            EEffectType.MainStat_AttackRate,
            EEffectType.MainStat_MovementSpeed,

            EEffectType.MainStat_Shield,
            EEffectType.MainStat_BonusHealth,
            EEffectType.MainStat_Armor,
            EEffectType.MainStat_Critical,
            EEffectType.MainStat_Dodge,
            EEffectType.PublicStat_Luck,
            EEffectType.MainStat_InvincibleBlockCountPerWave
        };

        public static bool IsEffectStatType(EEffectType effectType)
            => s_effectStatTypes.Contains(effectType);

        public static bool IsCreatureType(BaseCellObject obj)
            => obj != null && obj.ObjectType == EObjectType.Hero || obj.ObjectType == EObjectType.Monster ? true : false;

        public static CreatureData GetCreatureData(int dataID, Creature owner)
        {
            switch (owner?.ObjectType)
            {
                case EObjectType.Hero:
                    {
                        if (Managers.Data.HeroDataDict.TryGetValue(dataID, out HeroData heroData))
                            return heroData;
                    }
                    break;

                case EObjectType.Monster:
                    {
                        if (Managers.Data.MonsterDataDict.TryGetValue(dataID, out MonsterData monsterData))
                            return monsterData;
                    }
                    break;
            }

            return null;
        }

        public static EffectData GetEffectData(int dataID, BaseCellObject owner)
        {
            // --- Check Global Effect Data First
            if (Managers.Data.EffectDataDict.TryGetValue(dataID, out EffectData globalEffectData))
                return globalEffectData;

            switch (owner?.ObjectType)
            {
                case EObjectType.Hero:
                    {
                        if (Managers.Data.HeroEffectDataDict.TryGetValue(dataID, out HeroEffectData heroEffectData))
                            return heroEffectData;
                    }
                    break;

                case EObjectType.Monster:
                    {
                        if (Managers.Data.MonsterEffectDataDict.TryGetValue(dataID, out MonsterEffectData monsterEffectData))
                            return monsterEffectData;
                    }
                    break;

                case EObjectType.Env:
                    {
                        if (Managers.Data.EnvEffectDataDict.TryGetValue(dataID, out EnvEffectData envEffectData))
                            return envEffectData;
                    }
                    break;
            }

            return null;
        }

        public static SkillData GetSkillData(int dataID, Creature owner)
        {
            switch (owner?.ObjectType)
            {
                case EObjectType.Hero:
                    {
                        if (Managers.Data.HeroSkillDataDict.TryGetValue(dataID, out HeroSkillData heroSkillData))
                            return heroSkillData;
                    }
                    break;

                case EObjectType.Monster:
                    {
                        if (Managers.Data.MonsterSkillDataDict.TryGetValue(dataID, out MonsterSkillData monsterSkillData))
                            return monsterSkillData;
                    }
                    break;
            }

            return null;
        }

        public static T LoadScriptableObject<T>(EString eString) where T : UnityEngine.ScriptableObject
                => Managers.Resource.Load<T>(CString.SObject(eString)) as T;

// #if UNITY_EDITOR
//         [Conditional("UNITY_EDITOR")]
//         public static void ClearLog()
//         {
//             var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
//             var type = assembly.GetType("UnityEditor.LogEntries");
//             var method = type.GetMethod("Clear");
//             method.Invoke(new object(), null);
//             Log("### CLEAR ###");
//         }

//         [Conditional("UNITY_EDITOR")]
//         public static void Log(object log, bool highlight = false)
//         {
//             if (highlight)
//                 Debug.Log($"<color=#FF6666>\"</color><color=#A3E635>[<color=#FF6666>!</color>]: {log}</color><color=#FF6666>\"</color>");
//             else
//                 Debug.Log($"{log}");
//         }

//         [Conditional("UNITY_EDITOR")]
//         public static void LogWarning(object obj, object method)
//         {
//             Debug.LogWarning($"<color=#00FF22>[!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>");
//         }

//         [Conditional("UNITY_EDITOR")]
//         public static void LogWarning(object obj, object method, object log)
//         {
//             Debug.LogWarning($"<color=#00FF22>[!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>\n<color=white>{log}</color>");
//         }

//         [Conditional("UNITY_EDITOR")]
//         public static void LogError(object obj, object method)
//         {
//             Debug.LogError($"<color=red>[!!!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>");
//             Debug.Break();
//         }
        
//         [Conditional("UNITY_EDITOR")]
//         public static void LogError(object obj, object method, object log)
//         {
//             Debug.LogError($"<color=red>[!!!]</color> <color=yellow>{obj}</color><color=white>::</color><color=cyan>{method}</color>\n<color=white>{log}</color>");
//             Debug.Break();
//         }
// #endif
    }

    public static class TileMap
    {
        public const string Tilemap_Collision = "Tilemap_Collision";

        public const string Block = "_Block";
        public const char BlockFlag = '0';

        public const string CanMove = "_CanMove";
        public const char CanMoveFlag = '1';

        public const string SemiBlock = "_SemiBlock";
        public const char SemiBlockFlag = '2';
    }

    // --- Const Int
    public static class CInt
    {
        public static int ID(EInt eInt)
        {
            return eInt switch
            {
                EInt.ID_ImpactHit => 900000,
                EInt.ID_ImpactCriticalHit => 900001,
                EInt.ID_ImpactFire => 900002,
                EInt.ID_ImpactShockwave => 900003,
                EInt.ID_TeleportRed => 900020,
                EInt.ID_TeleportGreen => 900021,
                EInt.ID_TeleportBlue => 900022,
                EInt.ID_TeleportPurple => 900023,
                EInt.ID_Dust => 990000,
                EInt.ID_OnDeadSkull => 990001,
                EInt.ID_EvolutionGlow => 990002,

                EInt.ID_DamageFont => 109,
                EInt.ID_TextFont => 110,

                EInt.ID_Paladin => 101000,
                EInt.ID_Archer => 102000,
                EInt.ID_Lancer => 103000,
                EInt.ID_Wizard => 104000,
                EInt.ID_Assassin => 105000,
                EInt.ID_Gunner => 106000,
                EInt.ID_Trickster => 107000,
                EInt.ID_Druid => 108000,
                EInt.ID_Barbarian => 109000,
                EInt.ID_Ninja => 110000,
                EInt.ID_PhantomKnight => 111000,
                EInt.ID_FrostWeaver => 112000,
                EInt.ID_Queen => 113000,
                EInt.ID_Hunter => 114000,
                EInt.ID_Gladiator => 115000,
                EInt.ID_Priest => 116000,
                EInt.ID_Berserker => 117000,
                EInt.ID_Witch => 118000,
                EInt.ID_DragonKnight => 119000,
                EInt.ID_Alchemist => 120000,

                EInt.ID_Chicken => 201000,
                EInt.ID_Turkey => 202000,
                EInt.ID_Bunny => 203000,
                EInt.ID_Pug => 204000,

                EInt.ID_AshTree => 301000,
                EInt.ID_BlackOakTree => 302000,
                EInt.ID_GreenAppleTree => 303000,
                EInt.ID_IvyTree => 304000,
                EInt.ID_ManticoreTree => 305000,
                EInt.ID_MapleTree => 306000,
                EInt.ID_OakTree => 307000,
                EInt.ID_RedAppleTree => 308000,
                EInt.ID_RedSandalTree => 309000,
                EInt.ID_WillowTree => 310000,
                EInt.ID_YewTree => 311000,
                EInt.ID_CopperRock => 312000,
                EInt.ID_GoldRock => 313000,
                EInt.ID_IronRock => 314000,
                EInt.ID_LimestoneRock => 315000,
                EInt.ID_SilverRock => 316000,
                EInt.ID_StoneRock => 317000,
                EInt.ID_TinRock => 318000,
                EInt.ID_WhetstoneRock => 319000,
                EInt.ID_ZincRock => 320000,
                _ => throw new ArgumentOutOfRangeException($"{nameof(CInt)}::{nameof(ID)}", $"\nInvalid: {eInt}")
            };
        }

        public static int Sorting(EInt eInt)
        {
            return eInt switch
            {
                EInt.Sorting_Terrain => 0,
                EInt.Sorting_Deco => 10,
                EInt.Sorting_WeaponSTrail => 14, // TEMP
                EInt.Sorting_BodySTrail => 15,
                EInt.Sorting_BaseObject => 20,
                EInt.Sorting_Projectile => 30,
                EInt.Sorting_UI => 90,
                EInt.Sorting_Effect => 100,
                EInt.Sorting_DamageFont => 200,
                _ => throw new ArgumentOutOfRangeException($"{nameof(CInt)}::{nameof(Sorting)}", $"\nInvalid: {eInt}")
            };
        }

        public static int CValue(EInt eInt)
        {
            return eInt switch
            {
                EInt.CValue_ScanRange => 6,
                EInt.CValue_HeroMaxLevel => 8,
                EInt.CValue_CreatureMoveDepth => 20,
                EInt.CValue_TryFindingPathMaxCount => 12,
                _ => throw new ArgumentOutOfRangeException($"{nameof(CInt)}::{nameof(CValue)}", $"\nInvalid: {eInt}")
            };
        }
    }

    // --- Const Float
    public static class CFloat
    {
        public static float Min(EFloat eFloat)
        {
            return eFloat switch
            {
                EFloat.Range_AttackRate => 1.0F,
                EFloat.Range_AttackAnimRate => 0.85F,
                EFloat.Range_CollectRate => 1.0F,
                EFloat.Range_CollectAnimRate => 1.0F,
                EFloat.Range_MovementSpeed => 1.0F,
                EFloat.Range_MovementAnimSpeed => 1.0F,
                EFloat.Range_Armor => 0.0F,
                EFloat.Range_Critical => 0.0F,
                EFloat.Range_Dodge => 0.0F,
                EFloat.Range_Luck => 0.0F,
                EFloat.Range_HeroesRandFormationDist => 1.0F,
                EFloat.Range_JoystickFocusDist => -0.18F,
                _ => throw new ArgumentOutOfRangeException($"{nameof(CFloat)}::{nameof(Min)}", $"\nInvalid: {eFloat}")
            };
        }

        public static float Max(EFloat eFloat)
        {
            return eFloat switch
            {
                EFloat.Range_AttackRate => 2.0F,
                EFloat.Range_AttackAnimRate => 1.25F,
                EFloat.Range_CollectRate => 2.0F,
                EFloat.Range_CollectAnimRate => 1.5F,
                EFloat.Range_MovementSpeed => 12.0F,
                EFloat.Range_MovementAnimSpeed => 2.0F,
                EFloat.Range_Armor => 0.85F,
                EFloat.Range_Critical => 1.0F,
                EFloat.Range_Dodge => 0.8F,
                EFloat.Range_Luck => 0.65F,
                EFloat.Range_HeroesRandFormationDist => 2.0F,
                EFloat.Range_JoystickFocusDist => 0.18F,
                _ => throw new ArgumentOutOfRangeException($"{nameof(CFloat)}::{nameof(Max)}", $"\nInvalid: {eFloat}")
            };
        }

        public static float CValue(EFloat eFloat)
        {
            return eFloat switch
            {
                EFloat.CValue_CriticalDamageUpRate => 0.5F,
                EFloat.CValue_ForceWaitTime => 2.5F,
                EFloat.CValue_FarFromLeaderHeroTick => 1.0F,
                EFloat.CValue_ForceWaitTimeForStopWarp => 30.0F,
                EFloat.CValue_ChangeLeaderCoolTime => 5.0F,
                EFloat.CValue_FindTargetsTick => 0.1F,
                EFloat.CValue_CameraOrthoSize => 15.0F,
                EFloat.CValue_MaxMovementSpeedByDist => 8.0F,
                EFloat.CValue_FadeInTime => 0.5F,
                EFloat.CValue_StartFadeOutWaitTime => 2.0F,
                EFloat.CValue_FadeOutTime => 1.0F,
                EFloat.CValue_CameraMoveToTargetTime => 0.75F,
                _ => throw new ArgumentOutOfRangeException($"{nameof(CFloat)}::{nameof(CValue)}", $"\nInvalid: {eFloat}")
            };
        }
    }

    // --- Const String
    public static class CString
    {
        public static string Prefab(EString eString)
        {
            return eString switch
            {
                EString.Prefab_LeaderController => "LeaderController",
                EString.Prefab_TextFontBase => "TextFontBase",
                _ => throw new ArgumentOutOfRangeException($"{nameof(CString)}::{nameof(Prefab)}", $"\nInvalid: {eString}")
            };
        }

        public static string Data(EString eString)
        {
            return eString switch
            {
                EString.Data_Hero => "HeroData",
                EString.Data_HeroSprite => "HeroSpriteData",
                EString.Data_HeroSkill => "HeroSkillData",
                EString.Data_HeroEffect => "HeroEffectData",
                EString.Data_Monster => "MonsterData",
                EString.Data_MonsterBirdSprite => "MonsterBirdSpriteData",
                EString.Data_MonsterQuadrupedSprite => "MonsterQuadrupedSpriteData",
                EString.Data_MonsterSkill => "MonsterSkillData",
                EString.Data_MonsterEffect => "MonsterEffectData",
                EString.Data_Env => "EnvData",
                EString.Data_EnvTreeSprite => "EnvTreeSpriteData",
                EString.Data_EnvRockSprite => "EnvRockSpriteData",
                EString.Data_EnvEffect => "EnvEffectData",
                EString.Data_Effect => "EffectData",
                EString.Data_Projectile => "ProjectileData",
                _ => throw new ArgumentOutOfRangeException($"{nameof(CString)}::{nameof(Data)}", $"\nInvalid: {eString}")
            };
        }

        public static string AnimState(EString eString)
        {
            return eString switch
            {
                EString.AnimState_Upper_Idle => "Upper_Idle",
                EString.AnimState_Upper_Move => "Upper_Move",
                EString.AnimState_Upper_SkillA => "Upper_SkillA",

                EString.AnimState_Upper_SkillB => "Upper_SkillB",
                EString.AnimState_Upper_SkillB_Elite => "Upper_SkillB_Elite",

                EString.AnimState_Upper_SkillC => "Upper_SkillC",
                EString.AnimState_Upper_CollectEnv => "Upper_CollectEnv",
                EString.AnimState_Upper_Dead => "Upper_Dead",
                _ => throw new ArgumentOutOfRangeException($"{nameof(CString)}::{nameof(AnimState)}", $"\nInvalid: {eString}")
            };
        }

        public static string AnimParam(EString eString)
        {
            return eString switch
            {
                EString.AnimParam_IsMoving => "IsMoving",
                EString.AnimParam_IsEliteMax => "IsEliteMax",
                EString.AnimParam_CanSkill => "CanSkill",
                EString.AnimParam_OnSkillA => "OnSkillA",
                EString.AnimParam_OnSkillB => "OnSkillB",
                EString.AnimParam_OnSkillC => "OnSkillC",
                EString.AnimParam_OnCollectEnv => "OnCollectEnv",
                EString.AnimParam_OnDead => "OnDead",
                EString.AnimParam_AttackRate => "AttackRate",
                EString.AnimParam_CollectRate => "CollectRate",
                EString.AnimParam_MovementSpeed => "MovementSpeed",
                _ => throw new ArgumentOutOfRangeException($"{nameof(CString)}::{nameof(AnimParam)}", $"\nInvalid: {eString}")
            };
        }

        public static string Material(EString eString)
        {
            return eString switch
            {
                EString.Mat_Default => "Default.mat",
                EString.Mat_EyesPaint => "EyesPaint.mat",
                EString.Mat_StrongTint => "StrongTint.mat",
                EString.Mat_RockFragments => "RockFragments.mat",
                EString.Mat_Glow => "Glow.mat",
                _ => throw new ArgumentOutOfRangeException($"{nameof(CString)}::{nameof(Material)}", $"\nInvalid: {eString}")
            };
        }

        public static string Sprite(EString eString)
        {
            return eString switch
            {
                EString.Sprite_DefaultWoodcutterAxe => "DefaultWoodcutterAxe.sprite",
                EString.Sprite_MaxWoodcutterAxe => "MaxWoodcutterAxe.sprite",
                EString.Sprite_DefaultPickaxe => "DefaultPickaxe.sprite",
                EString.Sprite_MaxPickaxe => "MaxPickaxe.sprite",
                EString.Sprite_RockBodyFrame => "RockBodyFrame.sprite",
                EString.Sprite_RockEmptyFrame => "RockEmptyFrame.sprite",
                EString.Sprite_Shadow => "Shadow.sprite",
                EString.Sprite_CircleLight => "CircleLight.sprite",
                _ => throw new ArgumentOutOfRangeException($"{nameof(CString)}::{nameof(Sprite)}", $"\nInvalid: {eString}")
            };
        }

        public static string Object(EString eString)
        {
            return eString switch
            {
                EString.Obj_Managers => "@Managers",
                EString.Obj_UIRoot => "@UI_Root",
                EString.Obj_EventSystem => "@EventSystem",
                EString.Obj_HeroesRoot => "@Pool_Heroes",
                EString.Obj_MonstersRoot => "@Pool_Monsters",
                EString.Obj_EnvsRoot => "@Pool_Envs",
                EString.Obj_ProjectilesRoot => "@Pool_Projectiles",
                EString.Obj_TextFontsRoot => "@Pool_TextFonts",
                EString.Obj_EffectsRoot => "@Pool_Effects",
                _ => throw new ArgumentOutOfRangeException($"{nameof(CString)}::{nameof(Object)}", $"\nInvalid: {eString}")
            };
        }

        //--- Scriptable Object
        public static string SObject(EString eString)
        {
            return eString switch
            {
                EString.SO_STrail_Base => "SO_STrail_Base",
                EString.SO_STrail_Illusion => "SO_STrail_Illusion",
                EString.SO_STrail_DarkShadow => "SO_STrail_DarkShadow",
                EString.SO_STrail_DarkBeaten => "SO_STrail_DarkBeaten",
                EString.SO_STrail_Rainbow => "SO_STrail_Rainbow",
                EString.SO_STrail_PastelRainbow => "SO_STrail_PastelRainbow",
                _ => throw new ArgumentOutOfRangeException($"{nameof(CString)}::{nameof(SObject)}", $"\nInvalid: {eString}")
            };
        }

        public static string CValue(EString eString)
        {
            return eString switch
            {
                EString.CValue_BaseObject => "BaseObject",
                _ => throw new ArgumentOutOfRangeException($"{nameof(CString)}::{nameof(CValue)}", $"\nInvalid: {eString}")
            };
        }
    }
}

/*
        // public static T GetEffectComponent<T>(EEffectType effectType) where T : EffectBase
        // {
        //     return effectType switch
        //     {
        //         EEffectType.VFX_Base or EEffectType.VFX_BonusHealth or EEffectType.VFX_ShieldBlue or
        //         EEffectType.VFX_WindBlade
        //             => typeof(VFXBase) as T,

        //         EEffectType.BuffStat_MaxHealth or EEffectType.BuffStat_Damage or EEffectType.BuffStat_AttackRate or
        //         EEffectType.BuffStat_MovementSpeed or EEffectType.BuffStat_Shield or EEffectType.BuffStat_BonusHealth or
        //         EEffectType.BuffStat_Armor or EEffectType.BuffStat_Critical or EEffectType.BuffStat_Dodge or
        //         EEffectType.BuffStat_Luck or EEffectType.BuffStat_InvincibleBlockCountPerWave
        //             => typeof(BuffBase) as T,
                    
        //         _ => throw new ArgumentOutOfRangeException(nameof(effectType), $"Invalid value: {effectType}")
        //     };
        // }
*/