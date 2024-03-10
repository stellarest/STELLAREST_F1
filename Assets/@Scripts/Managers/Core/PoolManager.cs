using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace STELLAREST_F1
{
    internal class Pool
    {
        public Pool(GameObject prefab)
        {
            _prefab = prefab;
            _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
        }

        private GameObject _prefab = null;
        private IObjectPool<GameObject> _pool = null;

        private Transform _root = null;
        public Transform Root
        {
            get
            {
                if (_root == null)
                {
                    GameObject go = new GameObject { name = $"{_prefab.name}_Pool" };
                    _root = go.transform;
                }

                return _root;
            }
        }

        // 사용후 반납
        public void Push(GameObject go)
        {
            if (go.activeSelf)
                _pool.Release(go);
        }

        // 꺼냄
        public GameObject Pop() => _pool.Get();

        private GameObject OnCreate()
        {
            GameObject go = UnityEngine.GameObject.Instantiate(_prefab);
            go.transform.SetParent(Root);
            go.name = _prefab.name;
            return go;
        }
        private void OnGet(GameObject go) => go.SetActive(true);
        private void OnRelease(GameObject go) => go.SetActive(false);
        private void OnDestroy(GameObject go) => UnityEngine.GameObject.Destroy(go, Time.deltaTime);
    }

    public class PoolManager
    {
        private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

        public bool Push(GameObject go)
        {
            if (_pools.ContainsKey(go.name) == false)
                return false;

            _pools[go.name].Push(go);
            return true;
        }

        public GameObject Pop(GameObject prefab)
        {
            if (_pools.ContainsKey(prefab.name) == false)
                CreatePool(prefab);
            
            return _pools[prefab.name].Pop();
        }

        public void Clear() => _pools.Clear();

        private void CreatePool(GameObject original)
            => _pools.Add(original.name, new Pool(original));
    }
}
