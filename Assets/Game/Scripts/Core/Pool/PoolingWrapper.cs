using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Game.Scripts.Core.Pool
{
    [System.Serializable]
    public abstract class PoolingWrapper<T> where T : class, new()
    {
        [field: SerializeField] public UnityEvent<T> OnGet { get; set; }
        [field: SerializeField] public UnityEvent<T> OnRelease { get; set; }
        [field: SerializeField] public int DefaultSize { get; set; }
        [field: SerializeField] public int MaxSize { get; set; }

        private ObjectPool<T> _experiencePool;

        public void Init()
        {
            Init(DefaultSize, MaxSize);
        }

        public virtual void Init(int defaultSize, int maxSize)
        {
            DefaultSize = defaultSize;
            MaxSize = maxSize;

            if (defaultSize < 0 || maxSize < 0)
            {
                Debug.LogError("DefaultSize | MaxSize must be non-negative. Setting both to 10.");
                DefaultSize = 10;
                MaxSize = 10;
            }

            if (MaxSize < DefaultSize)
            {
                MaxSize = DefaultSize;
                Debug.LogError("MaxSize cannot be less than DefaultSize. Setting MaxSize to DefaultSize.");
            }

            _experiencePool = new ObjectPool<T>(
                CreateItem,
                GetItem,
                ReleaseItem,
                DestroyItem,
                false,
                DefaultSize,
                MaxSize
            );
        }

        public T Get()
        {
            T item = _experiencePool.Get();
            OnGet?.Invoke(item);
            return item;
        }

        public void Release(T item)
        {
            OnRelease?.Invoke(item);
            _experiencePool.Release(item);
        }

        protected abstract T CreateItem();

        protected abstract void GetItem(T item);

        protected abstract void ReleaseItem(T item);

        protected abstract void DestroyItem(T item);
    }
}