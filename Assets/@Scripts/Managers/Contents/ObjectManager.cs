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
        public List<Hero> Heroes { get; } = new List<Hero>();
        public List<Monster> Monsters { get; } = new List<Monster>();
        public HashSet<Env> Envs { get; } = new HashSet<Env>();
        public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>(); // 이것도 안들고있어도 될 것같긴한데
        
        public CameraController CameraController { get; set; } = null;
        public HeroLeaderController HeroLeaderController { get; private set; } = null;

        private Transform _leaderMark = null;
        public Transform LeaderMark
        {
            get
            {
                if (_leaderMark == null)
                {
                    _leaderMark = Managers.Resource.Instantiate(ReadOnly.Prefabs.PFName_LeaderController).transform;
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

        public Transform HeroRoot => GetRoot(ReadOnly.Util.HeroPoolingRootName);
        public Transform MonsterRoot => GetRoot(ReadOnly.Util.MonsterPoolingRootName);
        public Transform EnvRoot => GetRoot(ReadOnly.Util.EnvPoolingRootName);
        public Transform ProjectileRoot => GetRoot(ReadOnly.Util.ProjectilePoolingRootName);
        public Transform DamageFontRoot => GetRoot(ReadOnly.Util.DamageFontPoolingRootName);
        public Transform EffectRoot => GetRoot(ReadOnly.Util.EffectPoolingRootName);
        #endregion

        public HeroLeaderController SetHeroLeaderController()
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
                            Debug.LogError($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{dataID}\"");
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
                            Debug.LogError($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input: \"{dataID}\"");
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
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{dataID}\"");
                            return null;
                        }

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

                        Projectile projectile = go.GetComponent<Projectile>();
                        Projectiles.Add(projectile);
                        return projectile as T;
                    }

                case EObjectType.LeaderController:
                    {
                        go = Managers.Resource.Instantiate(ReadOnly.Prefabs.PFName_LeaderController);
                        if (go == null)
                        {
                            Debug.LogWarning($"{nameof(ObjectManager)}, {nameof(Spawn)}, Input : \"{EObjectType.LeaderController}\"");
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
            }

            Managers.Resource.Destroy(obj.gameObject, poolingID);
        }
    }
}
