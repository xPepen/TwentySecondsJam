using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Core.Pool
{
    [CreateAssetMenu(fileName = "PooledGameObjectParamSO", menuName = "Pooling/PooledGameObjectParamSO", order = 0)]
    public class PooledGameObjectParamSO : ScriptableObject
    {
        [field: SerializeField] public GameplayTag UniquePoolTag { get; set; }
        
        [Header("Prefab Settings")] [SerializeField]
        private GameObject prefab;

        [SerializeField] private bool shouldGroupObject = true;

        [Header("Pool Settings")] [SerializeField]
        private int defaultSize = 10;

        [SerializeField] private int maxSize = 50;


        // Public read-only properties
        public GameObject Prefab => prefab;
        public bool ShouldGroupObject => shouldGroupObject;
        public int DefaultSize => defaultSize;
        public int MaxSize => maxSize;
    }
}