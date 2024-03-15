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

        #region Roots
        public Transform GetRoot(string name)
        {
            GameObject root = GameObject.Find(name);
            if (root == null)
                root = new GameObject { name = name };
            
            return root.transform;
        }

        public Transform HeroRoot => GetRoot(ReadOnly.String.HeroRoot);
        public Transform MonsterRoot => GetRoot(ReadOnly.String.MonsterRoot);
        #endregion

        public T Spawn<T>(Vector3 position) where T : BaseObject
        {
            // 얼마든지 자유롭게 변경가능.
            // 스폰은 매프레임마다 하는게 아니기 때문에 조금 비효율적이라고 해도 크게 상관 없다고 판단할수도 있음.
            string prefabName = typeof(T).Name;
            GameObject go = Managers.Resource.Instantiate(prefabName);
            go.transform.position = position;

            BaseObject bo = go.GetComponent<BaseObject>();
            if (bo.ObjectType == EObjectType.Creature)
            {
                Creature creature = bo.GetComponent<Creature>();
                switch (creature.CreatureType)
                {
                    case ECreatureType.Hero:
                        creature.transform.SetParent(HeroRoot);
                        Heroes.Add(creature as Hero);
                        break;

                    case ECreatureType.Monster:
                        creature.transform.SetParent(MonsterRoot);
                        Monsters.Add(creature as Monster);
                        break;
                }
            }
            else if (bo.ObjectType == EObjectType.Projectile)
            {
            }
            else if (bo.ObjectType == EObjectType.Env)
            {
            }

            return bo as T;
        }

        public void Despawn<T>(T obj) where T : BaseObject
        {
            EObjectType objectType = obj.ObjectType;
            if (objectType == EObjectType.Creature)
            {
                Creature creature = obj.GetComponent<Creature>();
                switch (creature.CreatureType)
                {
                    case ECreatureType.Hero:
                        Heroes.Remove(creature as Hero);
                        break;

                    case ECreatureType.Monster:
                        Monsters.Remove(creature as Monster);
                        break;
                }
            }
            else if (objectType == EObjectType.Projectile)
            {
            }
            else if (objectType == EObjectType.Env)
            {
            }

            Managers.Resource.Destroy(obj.gameObject);
        }
    }
}
