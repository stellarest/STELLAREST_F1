using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Burst;
using UnityEngine;
using Debug = UnityEngine.Debug;
using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;
using UnityEngine.UIElements;

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
                    Debug.LogError($"{nameof(Util)}, {nameof(GetTypeFromClassName)}, Input : \"{className}, Please check Define.EClassName\"");
                    Debug.Break();
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
                if (_trySpawnCount++ >= ReadOnly.Util.CanTryMaxSpawnCount)
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

        private static HashSet<EEffectType> EffectBuffStats = new HashSet<EEffectType>
        {
            EEffectType.BuffStat_MaxHealth,
            EEffectType.BuffStat_BonusHealth,
            EEffectType.BuffStat_Shield,
            EEffectType.BuffStat_Damage,
            EEffectType.BuffStat_Armor,
            EEffectType.BuffStat_Critical,
            EEffectType.BuffStat_Dodge,
            EEffectType.BuffStat_Luck,
            EEffectType.BuffStat_InvincibleBlockCountPerWave

        };

        public static bool IsEffectBuffStat(EEffectType effectType)
            => EffectBuffStats.Contains(effectType);
       
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

        /*
            public enum EGlobalEffectID
        {
            ImpactHit = 900000,
            ImpactCriticalHit = 900001,
            ImpactFire = 900002,
            ImpactShockwave = 900003,

            TeleportRed = 900020,
            TeleportGreen = 900021,
            TeleportBlue = 900022,
            TeleportPurple = 900023,
            
            Dust = 990000,
            OnDeadSkull = 990001,
            EvolutionGlow = 990002
        }
        */

        private static readonly int GlobalEffect_VFX_ImpactHit = (int)EGlobalEffectID.ImpactHit;
        private static readonly int GlobalEffect_VFX_ImpactCriticalHit = (int)EGlobalEffectID.ImpactCriticalHit;
        private static readonly int GlobalEffect_VFX_ImpactFire = (int)EGlobalEffectID.ImpactFire;
        private static readonly int GlobalEffect_VFX_ImpactShockwave = (int)EGlobalEffectID.ImpactShockwave;

        private static readonly int GlobalEffect_VFX_TeleportRed = (int)EGlobalEffectID.TeleportRed;
        private static readonly int GlobalEffect_VFX_TeleportGreen = (int)EGlobalEffectID.TeleportGreen;
        private static readonly int GlobalEffect_VFX_TeleportBlue = (int)EGlobalEffectID.TeleportBlue;
        private static readonly int GlobalEffect_VFX_TeleportPurple = (int)EGlobalEffectID.TeleportPurple;

        private static readonly int GlobalEffect_VFX_Dust = (int)EGlobalEffectID.Dust;
        private static readonly int GlobalEffect_VFX_OnDeadSkull = (int)EGlobalEffectID.OnDeadSkull;
        private static readonly int GlobalEffect_VFX_EvolutionGlow = (int)EGlobalEffectID.EvolutionGlow;
        public static int GlobalDataID(EGlobalEffectID effectID)
        {
            return effectID switch
            {
                EGlobalEffectID.ImpactHit => GlobalEffect_VFX_ImpactHit,
                EGlobalEffectID.ImpactCriticalHit => GlobalEffect_VFX_ImpactCriticalHit,
                EGlobalEffectID.ImpactFire => GlobalEffect_VFX_ImpactFire,
                EGlobalEffectID.ImpactShockwave => GlobalEffect_VFX_ImpactShockwave,
                EGlobalEffectID.TeleportRed => GlobalEffect_VFX_TeleportRed,
                EGlobalEffectID.TeleportGreen => GlobalEffect_VFX_TeleportGreen,
                EGlobalEffectID.TeleportBlue => GlobalEffect_VFX_TeleportBlue,
                EGlobalEffectID.TeleportPurple => GlobalEffect_VFX_TeleportPurple,
                EGlobalEffectID.Dust => GlobalEffect_VFX_Dust,
                EGlobalEffectID.OnDeadSkull => GlobalEffect_VFX_OnDeadSkull,
                EGlobalEffectID.EvolutionGlow => GlobalEffect_VFX_EvolutionGlow,
                _ => throw new ArgumentOutOfRangeException(nameof(effectID), $"Invalid value type: {effectID}")
            };
        }

        public static int GlobalDataID(EGlobalProjectileID projectileID)
        {
            return -1;
        }

        public static bool IsCreatureType(BaseCellObject obj)
            => obj != null && obj.ObjectType == EObjectType.Hero || obj.ObjectType == EObjectType.Monster ? true : false;

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

#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        public static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
            Debug.Log("### CLEAR ###");
        }

        public static GameObject SpawnTestObject(Vector3Int spawnCellPos)
        {
            GameObject obj = Managers.Resource.Instantiate("TestCircleObject");
            obj.transform.position = Managers.Map.CellToCenterWorld(spawnCellPos);
            return obj;
        }
#endif
    }
}
