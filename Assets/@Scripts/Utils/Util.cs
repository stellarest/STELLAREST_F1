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

        // Util.GlobalEffectID(eInt)
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
                    _ => throw new ArgumentOutOfRangeException($"{nameof(CInt)}::{nameof(ID)}", $"\nInvalid: {eInt}")
                };
            }
        }

        // public static int GlobalEffectID(EInt eInt)
        // {
        //     return eInt switch
        //     {
        //         EInt.VFX_ImpactHit => 900000,
        //         EInt.VFX_ImpactCriticalHit => 900001,
        //         EInt.VFX_ImpactFire => 900002,
        //         EInt.VFX_ImpactShockwave => 900003,
        //         EInt.VFX_TeleportRed => 900020,
        //         EInt.VFX_TeleportGreen => 900021,
        //         EInt.VFX_TeleportBlue => 900022,
        //         EInt.VFX_TeleportPurple => 900023,
        //         EInt.VFX_Dust => 990000,
        //         EInt.VFX_OnDeadSkull => 990001,
        //         EInt.VFX_EvolutionGlow => 990002,
        //         _ => throw new ArgumentOutOfRangeException(nameof(GlobalEffectID), $"\nInvalid value: {eInt}")
        //     };
        // }

        // public static int TextFontID(EInt eInt)
        // {
        //     return eInt switch
        //     {
        //         EInt.TextFont_Damage => 109,
        //         EInt.TextFont_Text => 110,
        //         _ => throw new ArgumentOutOfRangeException(nameof(TextFontID), $"\nInvalid value: {eInt}")
        //     };
        // }

        public static int HeroDataID(EInt eInt)
        {
            return eInt switch
            {
                EInt.Hero_Paladin => 101000,
                EInt.Hero_Archer => 102000,
                EInt.Hero_Lancer => 103000,
                EInt.Hero_Wizard => 104000,
                EInt.Hero_Assassin => 105000,
                EInt.Hero_Gunner => 106000,
                EInt.Hero_Trickster => 107000,
                EInt.Hero_Druid => 108000,
                EInt.Hero_Barbarian => 109000,
                EInt.Hero_Ninja => 110000,
                EInt.Hero_PhantomKnight => 111000,
                EInt.Hero_FrostWeaver => 112000,
                EInt.Hero_Queen => 113000,
                EInt.Hero_Hunter => 114000,
                EInt.Hero_Gladiator => 115000,
                EInt.Hero_Priest => 116000,
                EInt.Hero_Berserker => 117000,
                EInt.Hero_Witch => 118000,
                EInt.Hero_DragonKnight => 119000,
                EInt.Hero_Alchemist => 120000,
                _ => throw new ArgumentOutOfRangeException(nameof(HeroDataID), $"\nInvalid value: {eInt}")
            };
        }

        public static int MonsterDataID(EInt eInt)
        {
            return eInt switch
            {
                EInt.Monster_Chicken => 101000,
                EInt.Monster_Turkey => 101001,
                EInt.Monster_Bunny => 101002,
                EInt.Monster_Pug => 101003,
                _ => throw new ArgumentOutOfRangeException(nameof(MonsterDataID), $"\nInvalid value: {eInt}")
            };
        }

        public static int EnvDataID(EInt eInt)
        {
            return eInt switch
            {
                EInt.Env_AshTree => 101000,
                EInt.Env_BlackOakTree => 101001,
                EInt.Env_GreenAppleTree => 101002,
                EInt.Env_IvyTree => 101003,
                EInt.Env_ManticoreTree => 101004,
                EInt.Env_MapleTree => 101005,
                EInt.Env_OakTree => 101006,
                EInt.Env_RedAppleTree => 101007,
                EInt.Env_RedSandalTree => 101008,
                EInt.Env_WillowTree => 101009,
                EInt.Env_YewTree => 101010,
                EInt.Env_CopperRock => 101011,
                EInt.Env_GoldRock => 101012,
                EInt.Env_IronRock => 101013,
                EInt.Env_LimestoneRock => 101014,
                EInt.Env_SilverRock => 101015,
                EInt.Env_StoneRock => 101016,
                EInt.Env_TinRock => 101017,
                EInt.Env_WhetstoneRock => 101018,
                EInt.Env_ZincRock => 101019,
                _ => throw new ArgumentOutOfRangeException(nameof(EnvDataID), $"\nInvalid value: {eInt}")
            };
        }

        public static int Sorting(EInt eInt)
        {
            return eInt switch
            {
                EInt.Sorting_Terrain => 0,
                EInt.Sorting_Deco => 10,
                EInt.Sorting_BaseObject => 20,
                EInt.Sorting_Projectile => 30,
                EInt.Sorting_UI => 90,
                EInt.Sorting_Effect => 100,
                EInt.Sorting_DamageFont => 200,
                _ => throw new ArgumentOutOfRangeException(nameof(EnvDataID), $"\nInvalid value: {eInt}")
            };
        }

        public static int MinInt(EInt eInt)
        {
            return eInt switch
            {
                EInt.HeroLevel => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(MinInt), $"\nInvalid value: {eInt}")
            };
        }

        public static int MaxInt(EInt eInt)
        {
            return eInt switch
            {
                EInt.HeroLevel => 8,
                _ => throw new ArgumentOutOfRangeException(nameof(MaxInt), $"\nInvalid value: {eInt}")
            };
        }

        public static float Float(EFloat eFloat)
        {
            return eFloat switch
            {
                _ => throw new ArgumentOutOfRangeException(nameof(Float), $"\nInvalid value: {eFloat}")
            };
        }

        public static float MinFloat(EFloat eFloat)
        {
            return eFloat switch
            {
                EFloat.AttackRate => 1.0F,
                EFloat.AttackAnimRate => 0.85F,
                EFloat.CollectRate => 1.0F,
                EFloat.CollectAnimRate => 1.0F,
                EFloat.Armor => 0.0F,
                EFloat.MovementSpeed => 1.0F,
                EFloat.MovementAnimSpeed => 1.0F,
                _ => throw new ArgumentOutOfRangeException(nameof(MinFloat), $"\nInvalid value: {eFloat}")
            };
        }

        public static float MaxFloat(EFloat eFloat)
        {
            return eFloat switch
            {
                EFloat.AttackRate => 2.0F,
                EFloat.AttackAnimRate => 1.25F,
                EFloat.CollectRate => 2.0F,
                EFloat.CollectAnimRate => 1.5F,
                EFloat.Armor => 0.85F,
                EFloat.Luck => 0.6F,
                EFloat.MovementSpeed => 12.0F,
                EFloat.MovementAnimSpeed => 2.0F,
                _ => throw new ArgumentOutOfRangeException(nameof(MaxFloat), $"\nInvalid value: {eFloat}")
            };
        }

        public static string Prefab(EString eString)
        {
            return eString switch
            {
                EString.LeaderController => "LeaderController",
                EString.TextFontBase => "TextFontBase",
                _ => throw new ArgumentOutOfRangeException(nameof(Prefab), $"\nInvalid value: {eString}")
            };
        }

        public static string Data(EString eString)
        {
            return eString switch
            {
                EString.HeroData => "HeroData",
                EString.HeroSpriteData => "HeroSpriteData",
                EString.HeroSkillData => "HeroSkillData",
                EString.HeroEffectData => "HeroEffectData",
                EString.MonsterData => "MonsterData",
                EString.MonsterBirdSpriteData => "MonsterBirdSpriteData",
                EString.MonsterQuadrupedSpriteData => "MonsterQuadrupedSpriteData",
                EString.MonsterSkillData => "MonsterSkillData",
                EString.MonsterEffectData => "MonsterEffectData",
                EString.EnvData => "EnvData",
                EString.EnvTreeSpriteData => "EnvTreeSpriteData",
                EString.EnvRockSpriteData => "EnvRockSpriteData",
                EString.EnvEffectData => "EnvEffectData",
                EString.EffectData => "EffectData",
                EString.ProjectileData => "ProjectileData",
                _ => throw new ArgumentOutOfRangeException(nameof(Data), $"\nInvalid value: {eString}")
            };
        }

        public static string AnimState(EString eString)
        {
            return eString switch
            {
                EString.Upper_Idle => "Upper_Idle",
                EString.Upper_Move => "Upper_Move",
                EString.Upper_SkillA => "Upper_SkillA",
                EString.Upper_SkillB => "Upper_SkillB",
                EString.Upper_SkillC => "Upper_SkillC",
                EString.Upper_CollectEnv => "Upper_CollectEnv",
                EString.Upper_Dead => "Upper_Dead",
                _ => throw new ArgumentOutOfRangeException(nameof(AnimState), $"\nInvalid value: {eString}")
            };
        }

        public static string AnimParam(EString eString)
        {
            return eString switch
            {
                EString.IsMoving => "IsMoving",
                EString.CanSkill => "CanSkill",
                EString.OnSkillA => "OnSkillA",
                EString.OnSkillB => "OnSkillB",
                EString.OnSkillC => "OnSkillC",
                EString.OnCollectEnv => "OnCollectEnv",
                EString.OnDead => "OnDead",
                EString.AttackRate => "AttackRate",
                EString.CollectRate => "CollectRate",
                EString.MovementSpeed => "MovementSpeed",
                _ => throw new ArgumentOutOfRangeException(nameof(AnimParam), $"\nInvalid value: {eString}")
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
                _ => throw new ArgumentOutOfRangeException(nameof(Material), $"\nInvalid value: {eString}")
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
                _ => throw new ArgumentOutOfRangeException(nameof(Sprite), $"\nInvalid value: {eString}")
            };
        }

        /*
            Obj_Managers,
            Obj_UIRoot,
            Obj_EventSystem,
            Obj_HeroesPool,
            Obj_MonstersPool,
            Obj_EnvsPool,
            Obj_ProjectilesPool,
            Obj_TextFontsPool,
            Obj_EffectsPool
        */

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
                _ => throw new ArgumentOutOfRangeException(nameof(Object), $"\nInvalid value: {eString}")
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
