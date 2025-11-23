using System.Collections.Generic;
using GameplayTags;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Core.Pool
{
    public class PoolMaster : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnInitialized;
        
        private readonly Dictionary<int, GameObjectPool> _poolsContainer = new();

        private void Awake()
        {
            this.Bind();
        }

        private void Start()
        {
            OnInitialized?.Invoke();
            AudioSource x;
        }

        public void Clear() => _poolsContainer.Clear();

        public void AddPool(PooledGameObjectParamSO poolParamSO)
        {
            PooledGameObjectParam poolParam = new()
            {
                Prefab = poolParamSO.Prefab,
                DefaultSize = poolParamSO.DefaultSize,
                MaxSize = poolParamSO.MaxSize,
                ShouldGroupObject = poolParamSO.ShouldGroupObject
            };
            AddPool(poolParamSO.UniquePoolTag, poolParam);
        }

        public void AddPool(GameplayTag uniquePoolTag, PooledGameObjectParam poolParam)
        {
            if (_poolsContainer.ContainsKey(uniquePoolTag))
            {
                Debug.LogError($"Pool with tag {uniquePoolTag} already exists, can't initialize GameObjectPool", this);
                return;
            }

            if (!GameplayTag.IsValid(uniquePoolTag))
            {
                Debug.LogError("uniquePoolTag is null, can't initialize GameObjectPool", this);
                return;
            }

            if (!poolParam.Prefab)
            {
                Debug.LogError("Prefab is null, can't initialize GameObjectPool", this);
                return;
            }

            var poolInstance = new GameObjectPool
            {
                //Parent Params
                DefaultSize = poolParam.DefaultSize,
                MaxSize = poolParam.MaxSize,

                //GameObject Params
                Prefab = poolParam.Prefab,
                ShouldGroupObject = poolParam.ShouldGroupObject
            };


            poolInstance.Init();

            _poolsContainer.Add(uniquePoolTag, poolInstance);
        }

        public void RemovePool(GameplayTag uniquePoolTag)
        {
            if (!_poolsContainer.Remove(uniquePoolTag))
            {
                Debug.LogError($"Pool with tag {uniquePoolTag} does not exist, can't remove GameObjectPool", this);
            }
        }

        public GameObject GetItemFromPool(GameplayTag uniquePoolTag)
        {
            if (_poolsContainer.TryGetValue(uniquePoolTag, out var pool))
            {
                return pool.Get();
            }

            Debug.LogError($"Pool with tag {uniquePoolTag} does not exist, can't get GameObjectPool", this);
            return null;
        }

        public T GetItemFromPoolAsComponent<T>(GameplayTag uniquePoolTag, Vector3 position = default,
            Quaternion rotation = default)
            where T : Component
        {
            if (_poolsContainer.TryGetValue(uniquePoolTag, out var pool))
            {
                T component = pool.GetAsComponent<T>();
                component.transform.position = position;
                component.transform.rotation = rotation;

                return component;
            }

            return null;
        }


        public void ReleaseItemToPool(GameplayTag uniquePoolTag, GameObject item)
        {
            if (_poolsContainer.TryGetValue(uniquePoolTag, out var pool))
            {
                pool.Release(item);
            }
        }

        public UnityEvent<GameObject> PoolEvent_ReleaseItem(GameplayTag uniquePoolTag)
        {
            return _poolsContainer.TryGetValue(uniquePoolTag, out var pool) ? pool.OnRelease : null;
        }

        public UnityEvent<GameObject> PoolEvent_GetItem(GameplayTag uniquePoolTag)
        {
            if (_poolsContainer.TryGetValue(uniquePoolTag, out var pool))
            {
                return pool.OnRelease;
            }

            Debug.Log($"Pool with tag {uniquePoolTag} does not exist, can't get Event", this);
            return null;
        }

        private void OnDestroy()
        {
            Clear();
            Subsystem.UnBind<PoolMaster>();
        }
    }
}