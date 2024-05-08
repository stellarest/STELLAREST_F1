using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace STELLAREST_F1
{
    public class ResourceManager
    {
        private Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();
        private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();

        public T Load<T>(string key) where T : UnityEngine.Object
        {
            if (_resources.TryGetValue(key, out UnityEngine.Object resource))
                return resource as T;

            return null;
        }

        public GameObject Instantiate(string key, Transform parent = null, int poolingID = -1)
        {
            GameObject prefab = this.Load<GameObject>(key);
            if (prefab == null)
            {
                //Debug.LogError($"{nameof(ResourceManager)}, {nameof(Instantiate)}, Input : \"{key}\"");
                Util.LogError("{nameof(ResourceManager)}, {nameof(Instantiate)}, Input : \"{key}\"");
                return null;
            }

            // PoolManager
            if (poolingID != -1)
                return Managers.Pool.Pop(prefab, parent, poolingID); // CreatePool

            GameObject go = UnityEngine.Object.Instantiate(prefab);
            go.name = prefab.name;
            return go;
        }

        public void Destroy(GameObject go, int poolingID = -1)
        {
            if (go == null)
                return;

            if (Managers.Pool.Push(go, poolingID))
                return;

            UnityEngine.Object.Destroy(go, Time.deltaTime);
        }

        #region Addressable
        private void LoadAsync<T>(string key, System.Action<T> callback = null) where T : UnityEngine.Object
        {
            // Cache
            if (_resources.TryGetValue(key, out UnityEngine.Object resource))
            {
                callback?.Invoke(resource as T);
                return;
            }

            // BanditLightArmor.sprite -> BanditLightArmor.sprite[BanditLightArmor]
            // texture to sprite : EXPGem_01.sprite -> EXPGem_01.sprite[EXPGem_01]
            string loadKey = key;
            if (key.Contains(".sprite"))
                loadKey = $"{key}[{key.Replace(".sprite", "")}]";

            var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
            asyncOperation.Completed += (op) =>
            {
                _resources.Add(key, op.Result);
                _handles.Add(key, asyncOperation);
                callback?.Invoke(op.Result);
            };
        }

        public void LoadAllAsync<T>(string label, System.Action<string, int, int> callback) where T : UnityEngine.Object
        {
            var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
            opHandle.Completed += (op) =>
            {
                int loadCount = 0;
                int totalCount = op.Result.Count;
                
                foreach (var result in op.Result)
                {
                    if (result.PrimaryKey.Contains(".sprite"))
                    {
                        LoadAsync<Sprite>(result.PrimaryKey, (obj) => 
                        {
                            loadCount++;
                            callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                        });
                    }
                    else
                    {
                        LoadAsync<T>(result.PrimaryKey, (obj) => 
                        {
                            loadCount++;
                            callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                        });
                    }
                }
            };
        }
        #endregion

        public void Clear()
        {
            _resources.Clear();
            foreach (var handle in _handles)
                UnityEngine.AddressableAssets.Addressables.Release(handle);
            _handles.Clear();
        }
    }
}