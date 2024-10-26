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
using UnityEngine.AI;

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

    private static HashSet<EEffectType> s_effectStatTypes = new HashSet<EEffectType>
        {
            EEffectType.MainStat_MaxHealth,
            EEffectType.MainStat_Damage,
            EEffectType.MainStat_AttackRate,
            EEffectType.MainStat_MovementSpeed,

            EEffectType.SubStat_Shield,
            EEffectType.SubStat_BonusHealth,
            EEffectType.SubStat_Armor,
            EEffectType.SubStat_Critical,
            EEffectType.SubStat_Dodge,
            EEffectType.SubStat_Luck,
            EEffectType.SubStat_InvincibleBlockCountPerWave
        };

        public static bool IsEffectStatType(EEffectType effectType)
            => s_effectStatTypes.Contains(effectType);

        public static int GlobalEffectID(EConstInteger constInt)
        {
            return constInt switch
            {
                EConstInteger.VFX_ImpactHit => 900000,
                EConstInteger.VFX_ImpactCriticalHit => 900001,
                EConstInteger.VFX_ImpactFire => 900002,
                EConstInteger.VFX_ImpactShockwave => 900003,
                EConstInteger.VFX_TeleportRed => 900020,
                EConstInteger.VFX_TeleportGreen => 900021,
                EConstInteger.VFX_TeleportBlue => 900022,
                EConstInteger.VFX_TeleportPurple => 900023,
                EConstInteger.VFX_Dust => 990000,
                EConstInteger.VFX_OnDeadSkull => 990001,
                EConstInteger.VFX_EvolutionGlow => 990002,
                _ => throw new ArgumentOutOfRangeException(nameof(GlobalEffectID), $"\nInvalid value: {constInt}")
            };
        }

        public static int TextFontID(EConstInteger constInt)
        {
            return constInt switch
            {
                EConstInteger.TextFont_Damage => 109,
                EConstInteger.TextFont_Text => 110,
                _ => throw new ArgumentOutOfRangeException(nameof(TextFontID), $"\nInvalid value: {constInt}")
            };
        }

        public static int HeroDataID(EConstInteger constInt)
        {
            return constInt switch
            {
                EConstInteger.Hero_Paladin => 101000,
                EConstInteger.Hero_Archer => 102000,
                EConstInteger.Hero_Lancer => 103000,
                EConstInteger.Hero_Wizard => 104000,
                EConstInteger.Hero_Assassin => 105000,
                EConstInteger.Hero_Gunner => 106000,
                EConstInteger.Hero_Trickster => 107000,
                EConstInteger.Hero_Druid => 108000,
                EConstInteger.Hero_Barbarian => 109000,
                EConstInteger.Hero_Ninja => 110000,
                EConstInteger.Hero_PhantomKnight => 111000,
                EConstInteger.Hero_FrostWeaver => 112000,
                EConstInteger.Hero_Queen => 113000,
                EConstInteger.Hero_Hunter => 114000,
                EConstInteger.Hero_Gladiator => 115000,
                EConstInteger.Hero_Priest => 116000,
                EConstInteger.Hero_Berserker => 117000,
                EConstInteger.Hero_Witch => 118000,
                EConstInteger.Hero_DragonKnight => 119000,
                EConstInteger.Hero_Alchemist => 120000,
                _ => throw new ArgumentOutOfRangeException(nameof(HeroDataID), $"\nInvalid value: {constInt}")
            };
        }

        public static int MonsterDataID(EConstInteger constInt)
        {
            return constInt switch
            {
                EConstInteger.Monster_Chicken => 101000,
                EConstInteger.Monster_Turkey => 101001,
                EConstInteger.Monster_Bunny => 101002,
                EConstInteger.Monster_Pug => 101003,
                _ => throw new ArgumentOutOfRangeException(nameof(MonsterDataID), $"\nInvalid value: {constInt}")
            };
        }

        /*
            // public static readonly int DNPID_Env_AshTree = 101000;
                // public static readonly int DNPID_Env_BlackOakTree = 101001;
                // public static readonly int DNPID_Env_GreenAppleTree = 101002;
                // public static readonly int DNPID_Env_IvyTree = 101003;
                // public static readonly int DNPID_Env_ManticoreTree = 101004;
                // public static readonly int DNPID_Env_MapleTree = 101005;
                // public static readonly int DNPID_Env_OakTree = 101006;
                // public static readonly int DNPID_Env_RedAppleTree = 101007;
                // public static readonly int DNPID_Env_RedSandalTree = 101008;
                // public static readonly int DNPID_Env_WillowTree = 101009;
                // public static readonly int DNPID_Env_YewTree = 101010;
                // public static readonly int DNPID_Env_CopperRock = 101011;
                // public static readonly int DNPID_Env_GoldRock = 101012;
                // public static readonly int DNPID_Env_IronRock = 101013;
                // public static readonly int DNPID_Env_LimestoneRock = 101014;
                // public static readonly int DNPID_Env_SilverRock = 101015;
                // public static readonly int DNPID_Env_StoneRock = 101016;
                // public static readonly int DNPID_Env_TinRock = 101017;
                // public static readonly int DNPID_Env_WhetstoneRock = 101018;
                // public static readonly int DNPID_Env_ZincRock = 101019;
        */

        public static int EnvDataID(EConstInteger constInt)
        {
            return constInt switch
            {
                EConstInteger.Env_AshTree => 101000,
                EConstInteger.Env_BlackOakTree => 101001,
                EConstInteger.Env_GreenAppleTree => 101002,
                EConstInteger.Env_IvyTree => 101003,
                EConstInteger.Env_ManticoreTree => 101004,
                EConstInteger.Env_MapleTree => 101005,
                EConstInteger.Env_OakTree => 101006,
                EConstInteger.Env_RedAppleTree => 101007,
                EConstInteger.Env_RedSandalTree => 101008,
                EConstInteger.Env_WillowTree => 101009,
                EConstInteger.Env_YewTree => 101010,
                EConstInteger.Env_CopperRock => 101011,
                EConstInteger.Env_GoldRock => 101012,
                EConstInteger.Env_IronRock => 101013,
                EConstInteger.Env_LimestoneRock => 101014,
                EConstInteger.Env_SilverRock => 101015,
                EConstInteger.Env_StoneRock => 101016,
                EConstInteger.Env_TinRock => 101017,
                EConstInteger.Env_WhetstoneRock => 101018,
                EConstInteger.Env_ZincRock => 101019,
                _ => throw new ArgumentOutOfRangeException(nameof(EnvDataID), $"\nInvalid value: {constInt}")
            };
        }

        public static float MinFloat(EConstFloat constFloat)
        {
            return constFloat switch
            {
                EConstFloat.AttackRate => 1.0F,
                EConstFloat.AttackAnimRate => 0.85F,
                EConstFloat.Armor => 0.0F,
                EConstFloat.MovementSpeed => 1.0F,
                EConstFloat.MovementAnimSpeed => 1.0F,
                _ => throw new ArgumentOutOfRangeException(nameof(MinFloat), $"\nInvalid value: {constFloat}")
            };
        }

        public static float MaxFloat(EConstFloat constFloat)
        {
            return constFloat switch
            {
                EConstFloat.AttackRate => 2.0F,
                EConstFloat.AttackAnimRate => 1.25F,
                EConstFloat.Armor => 0.85F,
                EConstFloat.Luck => 0.6F,
                EConstFloat.MovementSpeed => 12.0F,
                EConstFloat.MovementAnimSpeed => 2.0F,
                 _ => throw new ArgumentOutOfRangeException(nameof(MaxFloat), $"\nInvalid value: {constFloat}")
            };
        }

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
