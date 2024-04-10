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
        public HeroCamp Camp { get; private set; } = null;

        #region Roots
        public Transform GetRoot(string name)
        {
            GameObject root = GameObject.Find(name);
            if (root == null)
                root = new GameObject { name = name };

            return root.transform;
        }

        public Transform HeroRoot => GetRoot(ReadOnly.String.HeroRootName);
        public Transform MonsterRoot => GetRoot(ReadOnly.String.MonsterRootName);
        public Transform EnvRoot => GetRoot(ReadOnly.String.EnvRootName);
        #endregion

        public T Spawn<T>(Vector3 position, EObjectType spawnObjectType, int dataID = -1) where T : BaseObject
        {
            GameObject go = null;
            switch (spawnObjectType)
            {
                case EObjectType.Hero:
                    {
                        Data.HeroData data = Managers.Data.HeroDataDict[dataID];
                        go = Managers.Resource.Instantiate(data.PrefabLabel);
                        if (go ==null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{data.PrefabLabel}\"");
                            return null;
                        }

                        go.transform.position = position;
                        go.transform.SetParent(HeroRoot);
                        Hero hero = go.GetComponent<Hero>();
                        hero.SetInfo(dataID);
                        Heroes.Add(hero);
                        return hero as T;
                    }

                case EObjectType.Monster:
                    {
                        Data.MonsterData data = Managers.Data.MonsterDataDict[dataID];
                        go = Managers.Resource.Instantiate(data.PrefabLabel);
                        if (go ==null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{data.PrefabLabel}\"");
                            return null;
                        }

                        go.transform.position = position;
                        go.transform.SetParent(MonsterRoot);
                        Monster monster = go.GetComponent<Monster>();
                        monster.SetInfo(dataID);
                        Monsters.Add(monster);
                        return monster as T;
                    }

                case EObjectType.Env:
                    {
                        Data.EnvData data = Managers.Data.EnvDataDict[dataID];
                        go = Managers.Resource.Instantiate(data.PrefabLabel);
                        if (go == null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{data.PrefabLabel}\"");
                            return null;
                        }

                        go.transform.position = position;
                        go.transform.SetParent(EnvRoot);
                        Env env = go.GetComponent<Env>();
                        env.SetInfo(dataID);
                        Envs.Add(env);
                        return env as T;
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

        public void Despawn<T>(T obj) where T : BaseObject
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

                case EObjectType.HeroCamp:
                    Camp = null;
                    break;
            }

            Managers.Resource.Destroy(obj.gameObject);
        }
    }
}
