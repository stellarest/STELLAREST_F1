using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace STELLAREST_F1
{
    internal class Pool
    {
        public Pool(GameObject prefab, Transform parent)
        {
            _prefab = prefab;
            Root = parent;
            _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
        }

        private GameObject _prefab = null;
        private IObjectPool<GameObject> _pool = null;

        private Transform _root = null;
        public Transform Root
        {
            get => _root;
            private set => _root = _root == null ? value : _root;
        }

        // 사용후 반납
        public void Push(GameObject go)
        {
            if (go.activeSelf)
                _pool.Release(go);
        }

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
        //private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
        private Dictionary<int, Pool> _pools = new Dictionary<int, Pool>();

        public bool Push(GameObject go, int poolingID)
        {
            // if (_pools.ContainsKey(go.name) == false)
            //     return false;

            if (_pools.ContainsKey(poolingID) == false)
                return false;

            _pools[poolingID].Push(go);
            return true;
        }

        public GameObject Pop(GameObject prefab, Transform parent, int poolingID)
        {
            if (_pools.ContainsKey(poolingID) == false)
                CreatePool(prefab, parent, poolingID);

            //return _pools[prefab.name].Pop();
            return _pools[poolingID].Pop();
        }

        public void Clear() => _pools.Clear();

        private void CreatePool(GameObject original, Transform parent, int poolingID)
        {
            //_pools.Add(original.name, new Pool(original, parent));
            _pools.Add(poolingID, new Pool(original, parent));
        }
    }
}
