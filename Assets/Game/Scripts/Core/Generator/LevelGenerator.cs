using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core.Generator
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private LevelGenConfig config;
        
        private RoomLayoutGenerator _layoutGenerator;
        private RoomMeshGenerator _meshGenerator;
        private RoomConnectionGenerator _connectionGenerator;

        private List<Room> _rooms;
        
        public void SetConfig(LevelGenConfig conf) => this.config = conf;
        public List<Room> GetRooms() => _rooms;

        [ContextMenu("Generate")]
        public async void Generate()
        {
            if (!config) return;
            
            await GenerateRoutine();
        }

        private async Task GenerateRoutine()
        {
            Clear();
            Random.InitState(config.Seed);

            // Initialize Generators
            _layoutGenerator = new RoomLayoutGenerator(config);
            _meshGenerator = new RoomMeshGenerator(config);
            _connectionGenerator = new RoomConnectionGenerator(config);

            var levelData = _layoutGenerator.GenerateLevel();
            await Task.Yield(); 

            var graph = new RoomGraph(levelData.Rooms, levelData.Connections);
            
            _meshGenerator.Generate(graph);
            await Task.Yield();

            _connectionGenerator.ConnectRooms(graph);
            await Task.Yield();
            
            _rooms = levelData.Rooms;

            if (config.BakeNavMesh)
            {
                await BakeIndividualNavMeshes();
            }
        }

        private async Task BakeIndividualNavMeshes()
        {
            int yieldCounter = 0;

            foreach (var room in _rooms)
            {
                if (room.RoomObject != null)
                {
                    BakeObject(room.RoomObject);
                }
                
                if (++yieldCounter % 5 == 0) await Task.Yield();
            }
        }

        private void BakeObject(GameObject obj)
        {
            NavMeshSurface surface = obj.AddComponent<NavMeshSurface>();

            surface.collectObjects = CollectObjects.Children; 
            surface.center = obj.transform.position; 
            surface.BuildNavMesh(); 
        }

        void Clear()
        {
            foreach (Transform child in transform) DestroyImmediate(child.gameObject);
            
            GameObject roomsContainer = GameObject.Find("Rooms");
            if (roomsContainer) DestroyImmediate(roomsContainer);

            GameObject corridorsContainer = GameObject.Find("Corridors");
            if (corridorsContainer) DestroyImmediate(corridorsContainer);
            
            GameObject navObj = GameObject.Find("NavMesh_Surface");
            if(navObj) DestroyImmediate(navObj);
        }

        private void Start()
        {
            Generate();
        }
    }
}