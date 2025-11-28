using Game.Scripts.Core.Generator.Interface;
using Game.Scripts.Runtime.LightAndShadow;
using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    [CreateAssetMenu(menuName = "Level/LevelGenConfig")]
    public class LevelGenConfig : ScriptableObject
    {
        [field: Header("Generation Settings")]
        [field: SerializeField]
        public int Seed { get; private set; } = 12345;

        [Range(0.0f, 1.0f)]
        [field: SerializeField]
        public float BranchChance { get; private set; } = 0.5f;

        [Range(0f, 1f)]
        [field: SerializeField]
        public float SingleDoorChance { get; private set; } = 0.2f;

        [field: SerializeField] public int RoomCount { get; private set; } = 20;
        [field: SerializeField] public Vector2 RoomSizeMin { get; private set; } = new Vector2(4, 4);
        [field: SerializeField] public Vector2 RoomSizeMax { get; private set; } = new Vector2(12, 12);
        [field: SerializeField] public float Spacing { get; private set; } = 2f;
        [field: SerializeField] public float DoorHeight { get; private set; } = 2f;

        [field: Header("Visuals")]
        [field: SerializeField]
        public Material FloorMaterial { get; private set; }

        [field: SerializeField] public ColorType InitialFloorColor { get; private set; } = ColorType.White;
        [field: SerializeField] public Material WallMaterial { get; private set; }
        [field: SerializeField] public ColorType InitialWallColor { get; private set; } = ColorType.Black;
        [field: SerializeField] public float WallHeight { get; private set; } = 3.5f;
        [field: SerializeField] public float WallThickness { get; private set; } = 0.5f;
        [field: SerializeField] public float DoorWidth { get; private set; } = 2.0f;


        [field: Header("Prefabs")]
        [field: SerializeField]
        public GameObject DoorPrefab { get; private set; }

        [field: Header("AI Navigation")]
        [field: SerializeField]
        public bool BakeNavMesh { get; private set; } = true;
    }
}