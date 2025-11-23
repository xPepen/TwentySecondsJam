using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ADDRESSABLE_INSTALLED
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
#endif
namespace Game.Scripts.Core.Pool
{
    public class PoolBinder<T>
    {
        public Pooling<T> Pool;
        public GameObject Object;

        public PoolBinder(Pooling<T> pool,  GameObject gameobject)
        {
            Pool = pool;
            Object = gameobject;
        }
    }
    
    public class PoolObject<T>
    {
        public readonly GameObject Instance;
        public T Component;
        public bool IsActive;

        public PoolObject(GameObject instance, T component, bool isActive)
        {
            Instance = instance;
            Component = component;
            IsActive = isActive;
        }
    }

    //How to use it example : // Fast pool for small prefabs
    
    //Normal basic pool 
    // bulletPool.InitSync(bulletPrefab, 200);
    
    // Async pool for heavy objects
    // await enemyPool.InitAsync("EnemyBossAddressable", 10);
    
    [System.Serializable]
    public class Pooling<T>
    {
        private readonly Stack<PoolObject<T>> _pool = new();

        public Action OnPush;
        public Action OnPop;

        // --- SYNC INIT ---
        public void Init(int count, GameObject prefab, Transform parent = null)
        {
            for (int i = 0; i < count; i++)
            {
                var go = UnityEngine.Object.Instantiate(prefab, parent);
                go.SetActive(false);
                var comp = go.GetComponent<T>();
                _pool.Push(new PoolObject<T>(go, comp, false));
            }
        }
#if UNITY_ADDRESSABLE_INSTALLED
        // --- ASYNC INIT (Addressable) ---
        public async Task InitAsync(int count, string addressableKey, Transform parent = null)
        {
            for (int i = 0; i < count; i++)
            {
                var handle = Addressables.InstantiateAsync(addressableKey, parent);
                var go = await handle.Task;
                go.SetActive(false);
                var comp = go.GetComponent<T>();
                _pool.Push(new PoolObject<T>(go, comp, false));
            }
        }
#endif
        // --- POP ---
        public PoolObject<T> Pop(Vector3 position, Quaternion rotation)
        {
            if (_pool.Count == 0)
                throw new InvalidOperationException("Pool empty!");

            var obj = _pool.Pop();
            obj.Instance.transform.SetPositionAndRotation(position, rotation);
            obj.Instance.SetActive(true);
            obj.IsActive = true;
            OnPop?.Invoke();
            return obj;
        }

        // --- PUSH ---
        public void Push(PoolObject<T> obj)
        {
            obj.Instance.SetActive(false);
            obj.IsActive = false;
            _pool.Push(obj);
            OnPush?.Invoke();
        }
    }
}