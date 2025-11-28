using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Scripts.Core.Generator.Interface;
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

        [SerializeField] private GameObject Enemy;

        private void Awake()
        {
            this.Bind();
        }

        private void Start()
        {
            Generate();
        }

        private void OnDestroy()
        {
            Subsystem.UnBind<LevelGenerator>();
        }

        public void SetConfig(LevelGenConfig conf) => this.config = conf;

        public List<Room> GetRooms() => _rooms;

        public async Task Generate()
        {
            if (!config)
            {
                Debug.Log("LevelGenConfig is null, can't generate level", this);
                return;
            }

            await GenerateRoutine();

            if (config.BakeNavMesh)
            {
                await AddNavMeshNavigation();
            }

            foreach (Room r in GetRooms())
            {
                r.AddObjectAtRandomPos(Enemy, (GameObject obj) =>obj.SetActive(false));
            }
        }

        private async Task GenerateRoutine()
        {
            Clear();
            Random.InitState(config.Seed);

            // Initialize Generators
            _layoutGenerator = new RoomLayoutGenerator(config);
            _meshGenerator = new RoomMeshGenerator(config);
            _connectionGenerator = new RoomConnectionGenerator(config);
            //
            var levelData = _layoutGenerator.GenerateLevel();

            var graph = new RoomGraph(levelData.Rooms, levelData.Connections);

            _meshGenerator.Generate(graph);
            await Task.Yield();

            _connectionGenerator.ConnectRooms(graph);
            await Task.Yield();

            _rooms = levelData.Rooms;
        }

        private async Task AddNavMeshNavigation()
        {
            int yieldCounter = 0;

            foreach (var room in _rooms)
            {
                if (room == null)
                {
                    Debug.LogError("Room is null, can't bake navmesh", this);
                    break;
                }

                room?.AddNavMesh();

                if (++yieldCounter % 5 == 0) await Task.Yield();
            }
        }


        void Clear()
        {
            foreach (Transform child in transform) DestroyImmediate(child.gameObject);

            GameObject roomsContainer = GameObject.Find("Rooms");
            if (roomsContainer) DestroyImmediate(roomsContainer);

            GameObject corridorsContainer = GameObject.Find("Corridors");
            if (corridorsContainer) DestroyImmediate(corridorsContainer);

            GameObject navObj = GameObject.Find("NavMesh_Surface");
            if (navObj) DestroyImmediate(navObj);
        }
    }
}