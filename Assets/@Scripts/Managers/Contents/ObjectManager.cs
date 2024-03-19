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

        public T Spawn<T>(Vector3 position, int dataID) where T : BaseObject
        {
            // 얼마든지 자유롭게 변경가능. 스폰은 매프레임마다 하는게 아니기 때문에 조금 비효율적이라고 해도 크게 상관 없다고 판단할수도 있음.
            string prefabName = typeof(T).Name;
            GameObject go = Managers.Resource.Instantiate(prefabName);
            go.transform.position = position;

            BaseObject bo = go.GetComponent<BaseObject>();
            switch (bo.ObjectType)
            {
                case EObjectType.Hero:
                    {
                        bo.transform.SetParent(HeroRoot);
                        Hero hero = bo as Hero;
                        hero.SetInfo(dataID);
                        Heroes.Add(hero);
                    }
                    break;

                case EObjectType.Monster:
                    {
                        bo.transform.SetParent(HeroRoot);
                        Monster monster = bo as Monster;
                        monster.SetInfo(dataID);
                        Monsters.Add(monster);
                    }
                    break;
            }

            return bo as T;
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
            }

            Managers.Resource.Destroy(obj.gameObject);
        }
    }
}
