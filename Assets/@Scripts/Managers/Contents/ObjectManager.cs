using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class ObjectManager
    {
        public HashSet<Hero> Heroes { get; } = new HashSet<Hero>();
        public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
        public HashSet<Env> Envs { get; } = new HashSet<Env>();
        public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();
        public HeroCamp Camp { get; private set; } = null;

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

        // Need Another Type Spawn Method
        

        public T Spawn<T>(Vector3 position, EObjectType spawnObjectType, int dataID = -1) where T : BaseObject
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

                        go.transform.position = position;
                        Projectile projectile = go.GetComponent<Projectile>();
                        projectile.SetInfo(dataID);
                        Projectiles.Add(projectile);
                        return projectile as T;
                    }

                case EObjectType.HeroCamp:
                    {
                        go = Managers.Resource.Instantiate(ReadOnly.String.HeroCamp);
                        if (go == null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{ReadOnly.String.HeroCamp}\"");
                            return null;
                        }

                        go.transform.position = position;
                        go.name = $"@{go.name}";
                        Camp = go.GetComponent<HeroCamp>();
                        return Camp as T;
                    }
            }

            return null;
        }

        public void Despawn<T>(T obj, int poolingID) where T : BaseObject
        {
            switch (obj.ObjectType)
            {
                case EObjectType.Hero:
                    Heroes.Remove(obj as Hero);
                    break;

                case EObjectType.Monster:
                    Monsters.Remove(obj as Monster);
                    break;

                case EObjectType.Env:
                    Envs.Remove(obj as Env);
                    break;

                case EObjectType.Projectile:
                    Projectiles.Remove(obj as Projectile);
                    break;

                case EObjectType.HeroCamp:
                    Camp = null;
                    break;
            }

            Managers.Resource.Destroy(obj.gameObject, poolingID);
        }
    }
}
