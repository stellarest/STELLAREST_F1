using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class ObjectManager
    {
        //public HashSet<Hero> Heroes { get; } = new HashSet<Hero>();
        // TEMP
        public List<Hero> Heroes { get; } = new List<Hero>();
        public List<Monster> Monsters { get; } = new List<Monster>();
        public HashSet<Env> Envs { get; } = new HashSet<Env>();
        public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();
        //public HeroCamp Camp { get; private set; } = null;
        public CameraController CameraController { get; set; } = null;
        public HeroLeaderController HeroLeaderController { get; private set; } = null;
        // public Hero HeroLeader => HeroLeaderController.Leader;


        private Transform _leaderMark = null;
        public Transform LeaderMark
        {
            get
            {
                if (_leaderMark == null)
                {
                    _leaderMark = Managers.Resource.Instantiate(ReadOnly.String.LeaderController).transform;
                    _leaderMark.GetComponent<SpriteRenderer>().sortingOrder = 101;
                }

                return _leaderMark;
            }
        }

        #region Roots
        public Transform GetRoot(string name)
        {
            GameObject root = GameObject.Find(name);
            if (root == null)
                root = new GameObject { name = name };

            return root.transform;
        }

        public Transform HeroRoot => GetRoot(ReadOnly.String.HeroPoolingRootName);
        public Transform MonsterRoot => GetRoot(ReadOnly.String.MonsterPoolingRootName);
        public Transform EnvRoot => GetRoot(ReadOnly.String.EnvPoolingRootName);
        public Transform ProjectileRoot => GetRoot(ReadOnly.String.ProjectilePoolingRootName);
        #endregion

        public HeroLeaderController SetHeroLeaderController()
        {
            if (HeroLeaderController == null)
            {
                GameObject go = Managers.Resource.Instantiate(ReadOnly.String.LeaderController);
                go.name = $"@{go.name}";
                HeroLeaderController = go.GetComponent<HeroLeaderController>();
            }

            return HeroLeaderController;
        }

        // VER2
        public T Spawn<T>(EObjectType objectType, int dataID = -1, BaseObject presetOwner = null) where T : BaseObject
        {
            GameObject go = null;
            switch (objectType)
            {
                case EObjectType.Hero:
                    {
                        Data.HeroData data = Managers.Data.HeroDataDict[dataID];
                        go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: HeroRoot, poolingID: dataID);
                        if (go == null)
                        {
                            Debug.LogError($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{data.PrefabLabel}\"");
                            return null;
                        }
                        Hero hero = go.GetComponent<Hero>();
                        hero.SetInfo(dataID);
                        Heroes.Add(hero);
                        return hero as T;
                    }

                case EObjectType.Monster:
                    {
                        Data.MonsterData data = Managers.Data.MonsterDataDict[dataID];
                        go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: MonsterRoot, poolingID: dataID);
                        if (go == null)
                        {
                            Debug.LogError($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{data.PrefabLabel}\"");
                            return null;
                        }
                        Monster monster = go.GetComponent<Monster>();
                        monster.SetInfo(dataID);
                        Monsters.Add(monster);
                        return monster as T;
                    }
                case EObjectType.Env:
                    {
                        Data.EnvData data = Managers.Data.EnvDataDict[dataID];
                        go = Managers.Resource.Instantiate(data.PrefabLabel, parent: EnvRoot, poolingID: dataID);
                        if (go == null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{data.PrefabLabel}\"");
                            return null;
                        }

                        Env env = go.GetComponent<Env>();
                        env.SetInfo(dataID);
                        Envs.Add(env);
                        return env as T;
                    }
                case EObjectType.Projectile:
                    throw new System.NotImplementedException();

                // case EObjectType.HeroCamp:
                //     {
                //         go = Managers.Resource.Instantiate(ReadOnly.String.HeroCamp);
                //         if (go == null)
                //         {
                //             Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{ReadOnly.String.HeroCamp}\"");
                //             return null;
                //         }
                //         go.name = $"@{go.name}";
                //         Camp = go.GetComponent<HeroCamp>();
                //         return Camp as T;
                //     }

                case EObjectType.LeaderController:
                    {
                        go = Managers.Resource.Instantiate(ReadOnly.String.LeaderController);
                        if (go == null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{ReadOnly.String.HeroCamp}\"");
                            return null;
                        }
                        go.name = $"@{go.name}";
                        HeroLeaderController = go.GetComponent<HeroLeaderController>();
                        return HeroLeaderController as T;
                    }

                default:
                    return null;
            }
        }

        // VER1
        public T Spawn<T>(Vector3 position, EObjectType spawnObjectType, int dataID = -1, BaseObject owner = null) where T : BaseObject
        {
            GameObject go = null;
            switch (spawnObjectType)
            {
                case EObjectType.Hero:
                    {
                        Data.HeroData data = Managers.Data.HeroDataDict[dataID];
                        go = Managers.Resource.Instantiate(key: data.PrefabLabel, parent: HeroRoot, poolingID: data.DataID);
                        if (go ==null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{data.PrefabLabel}\"");
                            return null;
                        }

                        go.transform.position = position;
                        Hero hero = go.GetComponent<Hero>();
                        hero.SetInfo(dataID);
                        Heroes.Add(hero);
                        return hero as T;
                    }

                case EObjectType.Monster:
                    {
                        Data.MonsterData data = Managers.Data.MonsterDataDict[dataID];
                        go = Managers.Resource.Instantiate(data.PrefabLabel, parent: MonsterRoot, poolingID: data.DataID);
                        if (go ==null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{data.PrefabLabel}\"");
                            return null;
                        }

                        go.transform.position = position;
                        Monster monster = go.GetComponent<Monster>();
                        monster.SetInfo(dataID);
                        Monsters.Add(monster);
                        return monster as T;
                    }

                case EObjectType.Env:
                    {
                        Data.EnvData data = Managers.Data.EnvDataDict[dataID];
                        go = Managers.Resource.Instantiate(data.PrefabLabel, parent: EnvRoot, poolingID: data.DataID);
                        if (go == null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{data.PrefabLabel}\"");
                            return null;
                        }

                        go.transform.position = position;
                        Env env = go.GetComponent<Env>();
                        env.SetInfo(dataID);
                        Envs.Add(env);
                        return env as T;
                    }

                case EObjectType.Projectile:
                    {
                        Data.ProjectileData data = Managers.Data.ProjectileDataDict[dataID];
                        go = Managers.Resource.Instantiate(data.PrefabLabel, parent: ProjectileRoot, poolingID: data.DataID);
                        if (go == null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{data.PrefabLabel}\"");
                            return null;
                        }

                        go.transform.position = position; // .....
                        Projectile projectile = go.GetComponent<Projectile>();
                        projectile.SetInfo(owner, dataID); // SetInfo(param int[] dataIDs), 또는 List<int>로 받아도 될 것 같긴 한데...
                        Projectiles.Add(projectile);
                        //go.SetActive(false);
                        return projectile as T;
                    }

                // case EObjectType.HeroCamp:
                //     {
                //         go = Managers.Resource.Instantiate(ReadOnly.String.HeroCamp);
                //         if (go == null)
                //         {
                //             Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{ReadOnly.String.HeroCamp}\"");
                //             return null;
                //         }

                //         go.transform.position = position;
                //         go.name = $"@{go.name}";
                //         Camp = go.GetComponent<HeroCamp>();
                //         return Camp as T;
                //     }
            }

            return null;
        }

        public void Despawn<T>(T obj, int poolingID) where T : BaseObject
        {
            switch (obj.ObjectType)
            {
                case EObjectType.Hero:
                    Heroes.Remove(obj as Hero);
                    Managers.Map.RemoveObject(obj as Hero);
                    break;

                case EObjectType.Monster:
                    Monsters.Remove(obj as Monster);
                    Managers.Map.RemoveObject(obj as Monster);
                    break;

                case EObjectType.Env:
                    Envs.Remove(obj as Env);
                    Managers.Map.RemoveObject(obj as Env);
                    break;

                case EObjectType.Projectile:
                    Projectiles.Remove(obj as Projectile);
                    break;

                case EObjectType.HeroCamp:
                    //Camp = null;
                    break;
            }

            Managers.Resource.Destroy(obj.gameObject, poolingID);
        }
    }
}
