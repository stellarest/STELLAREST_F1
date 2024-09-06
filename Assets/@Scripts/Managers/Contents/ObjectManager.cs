using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using STELLAREST_F1.Data;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class ObjectManager
    {
        public List<Hero> Heroes { get; } = new List<Hero>();
        public List<Monster> Monsters { get; } = new List<Monster>();
        public HashSet<Env> Envs { get; } = new HashSet<Env>();
        public HeroLeaderController HeroLeaderController { get; private set; } = null;
        public CameraController CameraController { get; set; } = null;

        private Transform GetRoot(string name)
        {
            GameObject root = GameObject.Find(name);
            if (root == null)
                root = new GameObject { name = name };

            return root.transform;
        }
        public Transform HeroRoot => GetRoot(ReadOnly.Util.HeroPoolingRootName);
        public Transform MonsterRoot => GetRoot(ReadOnly.Util.MonsterPoolingRootName);
        public Transform EnvRoot => GetRoot(ReadOnly.Util.EnvPoolingRootName);
        public Transform ProjectileRoot => GetRoot(ReadOnly.Util.ProjectilePoolingRootName);
        public Transform DamageFontRoot => GetRoot(ReadOnly.Util.DamageFontPoolingRootName);
        public Transform EffectRoot => GetRoot(ReadOnly.Util.EffectPoolingRootName);

        private bool IsCellObject(EObjectType objectType)
            => objectType == EObjectType.Hero || objectType == EObjectType.Monster || objectType == EObjectType.Env;

        public Projectile SpawnProjectile(Vector3 spawnPos, int dataID)
        {
            ProjectileData data = Managers.Data.ProjectileDataDict[dataID];
            GameObject go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: ProjectileRoot, poolingID: Util.GetPoolingID(EObjectType.Projectile, dataID));
            if (go == null)
            {
                Debug.LogError($"{nameof(SpawnBaseObject)}, {nameof(EObjectType.Projectile)}, Input: \"{dataID}\"");
                return null;
            }

            Projectile projectile = go.GetComponent<Projectile>();
            // projectile.Owner = owner.GetComponent<Creature>();
            projectile.SetInfo(dataID, spawnPos);
            return projectile;
        }

        public T SpawnBaseObject<T>(EObjectType objectType, Vector3 spawnPos, int dataID = -1, BaseObject owner = null) where T : BaseObject
        {
            Vector3Int cellSpawnPos = Vector3Int.zero;
            if (IsCellObject(objectType))
            {
                cellSpawnPos = Util.MakeSpawnPosition(spawnPos);
                if (Managers.Map.CanMove(cellSpawnPos) == false)
                {
                    Debug.LogWarning($"Failed Spawn Cell Object: {nameof(Util.MakeSpawnPosition)}, {objectType}");
                    return null;
                }

                // --- If success making spawn position
                spawnPos = Managers.Map.CellToCenteredWorld(cellSpawnPos);
            }

            GameObject go = null;
            switch (objectType)
            {
                case EObjectType.Hero:
                    {
                        HeroData data = Managers.Data.HeroDataDict[dataID];
                        go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: HeroRoot, poolingID: Util.GetPoolingID(EObjectType.Hero, dataID));
                        if (go == null)
                        {
                            Debug.LogError($"{nameof(SpawnBaseObject)}, {nameof(EObjectType.Hero)}, Input: \"{dataID}\"");
                            return null;
                        }

                        Hero hero = go.GetComponent<Hero>();
                        hero.SetInfo(dataID, spawnPos);
                        Heroes.Add(hero);

#if UNITY_EDITOR
                        CellObject cellObj = new CellObject
                        {
                            CellPos = cellSpawnPos,
                            CellObj = hero
                        };
                        DevManager.Instance.CellObjs.Add(cellObj);
#endif
                        return hero as T;
                    }

                case EObjectType.Monster:
                    {
                        MonsterData data = Managers.Data.MonsterDataDict[dataID];
                        go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: MonsterRoot, poolingID:  Util.GetPoolingID(EObjectType.Monster, dataID));
                        if (go == null)
                        {
                            Debug.LogError($"{nameof(SpawnBaseObject)}, {nameof(EObjectType.Monster)}, Input: \"{dataID}\"");
                            return null;
                        }

                        Monster monster = go.GetComponent<Monster>();
                        monster.SetInfo(dataID, spawnPos);
                        Monsters.Add(monster);

#if UNITY_EDITOR
                        CellObject cellObj = new CellObject
                        {
                            CellPos = cellSpawnPos,
                            CellObj = monster
                        };
                        DevManager.Instance.CellObjs.Add(cellObj);
#endif
                        return monster as T;
                    }

                case EObjectType.Env:
                    {
                        EnvData data = Managers.Data.EnvDataDict[dataID];
                        go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: EnvRoot, poolingID:  Util.GetPoolingID(EObjectType.Env, dataID));
                        if (go == null)
                        {
                            Debug.LogError($"{nameof(SpawnBaseObject)}, {nameof(EObjectType.Env)}, Input: \"{dataID}\"");
                            return null;
                        }

                        Env env = go.GetComponent<Env>();
                        env.SetInfo(dataID, spawnPos);
                        Envs.Add(env);

#if UNITY_EDITOR
                        CellObject cellObj = new CellObject
                        {
                            CellPos = cellSpawnPos,
                            CellObj = env
                        };
                        DevManager.Instance.CellObjs.Add(cellObj);
#endif
                        return env as T;
                    }

                // --- TODO,,,Projectile, Effect
                case EObjectType.Projectile:
                    {
                        ProjectileData data = Managers.Data.ProjectileDataDict[dataID];
                        go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: ProjectileRoot, poolingID:  Util.GetPoolingID(EObjectType.Projectile, dataID));
                        if (go == null)
                        {
                            Debug.LogError($"{nameof(SpawnBaseObject)}, {nameof(EObjectType.Projectile)}, Input: \"{dataID}\"");
                            return null;
                        }

                        Projectile projectile = go.GetComponent<Projectile>();
                        projectile.Owner = owner.GetComponent<Creature>();
                        projectile.SetInfo(dataID, spawnPos);
                        return projectile as T;
                    }

                // --- TEMP
                case EObjectType.Effect:
                    {
                        EffectData data = Managers.Data.EffectDataDict[dataID];
                        go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: EffectRoot, poolingID: Util.GetPoolingID(EObjectType.Effect, dataID));
                        if (go == null)
                        {
                            Debug.LogError($"{nameof(SpawnBaseObject)}, {nameof(EObjectType.Effect)}, Input: \"{dataID}\"");
                            return null;
                        }

                        EffectBase effect = go.GetComponent<EffectBase>();
                        effect.Owner = owner.GetComponent<BaseCellObject>();
                        effect.SetInfo(dataID, spawnPos);
                        return effect as T;
                    }
            }

            return null;
        }

        public HeroLeaderController SpawnHeroLeaderController()
        {
            if (HeroLeaderController == null)
            {
                GameObject go = Managers.Resource.Instantiate(ReadOnly.Prefabs.PFName_LeaderController);
                go.name = $"@{go.name}";
                HeroLeaderController = go.GetComponent<HeroLeaderController>();
            }

            return HeroLeaderController;
        }

        public void ShowDamageFont(Vector2 position, float damage, bool isCritical = false)
        {
            int poolingID = ReadOnly.DataAndPoolingID.DNPID_DamageFont;
            string prefabName = ReadOnly.Prefabs.PFName_DamageFont;
            GameObject go = Managers.Resource.Instantiate(prefabName, parent: DamageFontRoot, poolingID: poolingID); // 풀링 되려나??
            DamageFont dmgFont = go.GetComponent<DamageFont>();
            dmgFont.SetInfo(position, damage, isCritical);
        }

        public List<T> FindTargets<T>(BaseObject owner, int range, bool isAlly = false) where T : BaseObject
        {
            List<T> targets = new List<T>();
            EObjectType ownerType = owner.ObjectType;
            Vector3 from = owner.transform.position;

            EObjectType targetType = Util.GetTargetType(ownerType, isAlly);
            if (targetType == EObjectType.Monster)
            {
                List<Monster> monsters = Managers.Map.GatherObjects<Monster>(from, range, range);
                for (int i = 0; i < monsters.Count; ++i)
                    targets.Add(monsters[i] as T);
            }
            else if (targetType == EObjectType.Hero)
            {
                List<Hero> heroes = Managers.Map.GatherObjects<Hero>(from, range, range);
                for (int i = 0; i < heroes.Count; ++i)
                    targets.Add(heroes[i] as T);
            }

            return targets;
        }

        // public T Spawn<T>(EObjectType objectType, int dataID = -1) where T : BaseObject
        // {
        //     GameObject go = null;
        //     switch (objectType)
        //     {
        //         case EObjectType.Hero:
        //             {
        //                 Data.HeroData data = Managers.Data.HeroDataDict[dataID];
        //                 go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: HeroRoot, poolingID: dataID);
        //                 if (go == null)
        //                 {
        //                     Debug.LogError($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{dataID}\"");
        //                     return null;
        //                 }
        //                 Hero hero = go.GetComponent<Hero>();
        //                 hero.SetInfo(dataID);
        //                 Heroes.Add(hero);
        //                 return hero as T;
        //             }

        //         case EObjectType.Monster:
        //             {
        //                 Data.MonsterData data = Managers.Data.MonsterDataDict[dataID];
        //                 go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: MonsterRoot, poolingID: dataID);
        //                 if (go == null)
        //                 {
        //                     Debug.LogError($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{dataID}\"");
        //                     return null;
        //                 }
        //                 Monster monster = go.GetComponent<Monster>();
        //                 monster.SetInfo(dataID);
        //                 Monsters.Add(monster);
        //                 return monster as T;
        //             }
        //         case EObjectType.Env:
        //             {
        //                 Data.EnvData data = Managers.Data.EnvDataDict[dataID];
        //                 go = Managers.Resource.Instantiate(data.PrefabLabel, parent: EnvRoot, poolingID: dataID);
        //                 if (go == null)
        //                 {
        //                     Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{dataID}\"");
        //                     return null;
        //                 }

        //                 Env env = go.GetComponent<Env>();
        //                 env.SetInfo(dataID);
        //                 Envs.Add(env);
        //                 return env as T;
        //             }
        //         case EObjectType.Projectile:
        //             {
        //                 Data.ProjectileData data = Managers.Data.ProjectileDataDict[dataID];
        //                 go = Managers.Resource.Instantiate(data.PrefabLabel, parent: ProjectileRoot, poolingID: data.DataID);
        //                 if (go == null)
        //                 {
        //                     Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{data.PrefabLabel}\"");
        //                     return null;
        //                 }

        //                 Projectile projectile = go.GetComponent<Projectile>();
        //                 Projectiles.Add(projectile);
        //                 return projectile as T;
        //             }

        //         case EObjectType.LeaderController:
        //             {
        //                 go = Managers.Resource.Instantiate(ReadOnly.Prefabs.PFName_LeaderController);
        //                 if (go == null)
        //                 {
        //                     Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{EObjectType.LeaderController}\"");
        //                     return null;
        //                 }
        //                 go.name = $"@{go.name}";
        //                 HeroLeaderController = go.GetComponent<HeroLeaderController>();
        //                 return HeroLeaderController as T;
        //             }

        //         case EObjectType.Effect:
        //             {
        //                 EffectData data = Managers.Data.EffectDataDict[dataID];
        //                 go = Managers.Resource.Instantiate(data.PrefabLabel, parent: EffectRoot, poolingID: dataID);
        //                 if (go == null)
        //                 {
        //                     Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{data.PrefabLabel}\"");
        //                     return null;
        //                 }

        //                 return go.GetComponent<EffectBase>() as T;
        //             }

        //         default:
        //             return null;
        //     }
        // }

        public void Despawn<T>(T obj, int dataID) where T : BaseObject
        {
            switch (obj.ObjectType)
            {
                case EObjectType.Hero:
                    Heroes.Remove(obj as Hero);
                    Managers.Map.RemoveCellObject(obj as Hero);
                    Managers.Resource.Destroy(go: obj.gameObject, poolingID: Util.GetPoolingID(EObjectType.Hero, dataID));
                    break;

                case EObjectType.Monster:
                    Monsters.Remove(obj as Monster);
                    Managers.Map.RemoveCellObject(obj as Monster);
                    Managers.Resource.Destroy(go: obj.gameObject, poolingID: Util.GetPoolingID(EObjectType.Monster, dataID));
                    break;

                case EObjectType.Env:
                    Envs.Remove(obj as Env);
                    Managers.Map.RemoveCellObject(obj as Env);
                    Managers.Resource.Destroy(go: obj.gameObject, poolingID: Util.GetPoolingID(EObjectType.Env, dataID));
                    break;

                case EObjectType.Projectile:
                    Managers.Resource.Destroy(go: obj.gameObject, poolingID: Util.GetPoolingID(EObjectType.Projectile, dataID));
                    break;

                case EObjectType.Effect:
                    Managers.Resource.Destroy(go: obj.gameObject, Util.GetPoolingID(EObjectType.Effect, dataID));
                    break;
            }


#if UNITY_EDITOR
            CellObject cellObj = DevManager.Instance.CellObjs.Find(n => n.CellObj == obj);
            DevManager.Instance.CellObjs.Remove(cellObj);
#endif
        }
    }
}

// public List<T> FindCircleRangeTargets<T>(Vector3 from, float range, EObjectType ownerType, bool isAlly = false) where T : BaseObject
// {
//     List<T> targets = new List<T>();
//     List<T> rets = new List<T>();

//     EObjectType targetType = Util.GetTargetType(ownerType, isAlly);
//     if (targetType == EObjectType.Monster)
//     {
//         List<Monster> monsters = Managers.Map.GatherObjects<Monster>(from, range, range);
//         for (int i = 0; i < monsters.Count; ++i)
//             targets.Add(monsters[i] as T); 
//     }
//     else if (targetType == EObjectType.Hero)
//     {
//         List<Hero> heroes = Managers.Map.GatherObjects<Hero>(from, range, range);
//         for (int i = 0; i < heroes.Count; ++i)
//             targets.Add(heroes[i] as T);
//     }

//     for (int i = 0; i < targets.Count; ++i)
//     {
//         Vector3 targetPos = targets[i].transform.position;
//         float distSQR = (targetPos - from).sqrMagnitude;
//         if (distSQR < range * range)
//             rets.Add(targets[i]);
//     }

//     return rets;
// }

// --- 프로젝타일 컨테이너 제거 예정
// public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>(); // 이것도 안들고있어도 될 것같긴한데

// --- 임시
// public HashSet<EffectBase> Effects { get; } = new HashSet<EffectBase>();
// private Transform _leaderMark = null;
// public Transform LeaderMark
// {
//     get
//     {
//         if (_leaderMark == null)
//         {
//             _leaderMark = Managers.Resource.Instantiate(ReadOnly.Prefabs.PFName_LeaderController).transform;
//             _leaderMark.GetComponent<SpriteRenderer>().sortingOrder = 101;
//         }

//         return _leaderMark;
//     }
// }