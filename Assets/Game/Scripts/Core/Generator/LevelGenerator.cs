using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core.Generator
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private LevelGenConfig config;
        // Fixed Types here:
        private RoomLayoutGenerator layoutGenerator; // Logic
        private RoomMeshGenerator meshGenerator; // 3D Rooms
        private RoomConnectionGenerator connectionGenerator; // 3D Corridors

        public void SetConfig(LevelGenConfig config)
        {
            this.config = config;
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            if (!config) return;
            Clear();

            Random.InitState(config.Seed);

            // Initialize Generators
            layoutGenerator = new RoomLayoutGenerator(config);
            meshGenerator = new RoomMeshGenerator(config);
            // Ensuring config is passed for DoorPrefab access
            connectionGenerator = new RoomConnectionGenerator(config);

            // 1. Generate Logic Data (Rooms & Connection Graph)
            var levelData = layoutGenerator.GenerateLevel();

            // 2. Build Graph (Helper for neighbors)
            var graph = new RoomGraph(levelData.Rooms, levelData.Connections);

            // 3. Generate 3D Meshes for Rooms
            meshGenerator.Generate(graph);

            // 4. Generate Connections (Corridor Meshes and Doors)
            connectionGenerator.ConnectRooms(graph);
        }

        void Clear()
        {
            // Destroy children created by this generator
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }

            // Cleanup loose containers
            GameObject roomsContainer = GameObject.Find("Rooms");
            if (roomsContainer) DestroyImmediate(roomsContainer);

            GameObject corridorsContainer = GameObject.Find("Corridors");
            if (corridorsContainer) DestroyImmediate(corridorsContainer);
        }

        private void Start()
        {
            Generate();
        }
    }
}