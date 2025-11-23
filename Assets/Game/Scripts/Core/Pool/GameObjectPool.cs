using System.Collections.Generic;
using System.Linq;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Core.Pool
{
    [System.Serializable]
    public struct PooledGameObjectParam
    {
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public bool ShouldGroupObject { get; set; }


        [field: SerializeField] public int DefaultSize { get; set; }
        [field: SerializeField] public int MaxSize { get; set; }

        public PooledGameObjectParam(GameObject prefab, bool shouldGroupObject, int defaultSize, int maxSize)
        {
            Prefab = prefab;
            ShouldGroupObject = shouldGroupObject;
            DefaultSize = defaultSize;
            MaxSize = maxSize;
        }
    }

    [System.Serializable]
    public class GameObjectPool : PoolingWrapper<GameObject>
    {
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public bool ShouldGroupObject { get; set; } = true;

        protected GameObject _folder;

        private readonly Dictionary<(int, System.Type), Component> _componentCache = new();

        public override void Init(int defaultSize, int maxSize)
        {
            if (!Prefab)
            {
                Debug.LogError("Prefab is null, can't initialize GameObjectPool");
                return;
            }

            _folder = new GameObject(Prefab.name);

            base.Init(defaultSize, maxSize);
        }

        public T1 GetAsComponent<T1>() where T1 : Component
        {
            var item = Get();

            var key = (item.GetInstanceID(), typeof(T1));

            if (_componentCache.TryGetValue(key, out var cachedComponent))
            {
                return cachedComponent as T1;
            }

            T1 component = item.GetComponent<T1>();

            if (!component)
            {
                component = item.GetComponentInChildren<T1>();
            }

            if (!component)
            {
                component = item.GetComponentInParent<T1>();
            }

            if (component)
            {
                _componentCache.TryAdd(key, component);
            }

            return component;
        }

        protected override GameObject CreateItem()
        {
            GameObject obj = Object.Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            if (ShouldGroupObject)
            {
                obj.transform.parent = _folder.transform;
            }

            return obj;
        }

        protected override void GetItem(GameObject item)
        {
            item.SetActive(true);
        }

        protected override void ReleaseItem(GameObject item)
        {
            item.SetActive(false);
        }

        protected override void DestroyItem(GameObject item)
        {
            ClearCacheForItem(item);
        }

        protected void ClearCacheForItem(GameObject item)
        {
            var id = item.GetInstanceID();

            // Collect matching keys using LINQ
            var keysToRemove = _componentCache
                .Where(kv => kv.Key.Item1 == id)
                .Select(kv => kv.Key)
                .ToList(); // Important: materialize before removing

            foreach (var key in keysToRemove)
            {
                _componentCache.Remove(key);
            }
        }
    }
}