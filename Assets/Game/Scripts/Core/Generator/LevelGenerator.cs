using System.Collections.Generic;
using System.Threading.Tasks;
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
        private int Seed;


        //just for testing purpose for now should be handle else where 
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
                await NavMeshUtils.AddNavMeshNavigation(GetRooms());
            }

            foreach (Room r in GetRooms())
            {
                r.AddObjectAtRandomPos(Enemy /*(GameObject obj) =>obj.SetActive(false)*/);
            }

            await Task.Yield();
        }

        private async Task GenerateRoutine()
        {
            Clear();
            Random.InitState(GetSeed());

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

        public int GetSeed()
        {
            return config.Seed + GetSaltedSeed("Global");
        }
        
        public int GetSeedWithSalt(string salt)
        {
            if (string.IsNullOrEmpty(salt))
            {
                return GetSeed();
            }
            
            return config.Seed + GetSaltedSeed(salt);
        }
        

        public void GenerateSeed()
        {
            config.Seed = Random.Range(100_000_000, 999_999_999);
            print(config.Seed);
        }

        private int GetSaltedSeed(string salt)
        {
            unchecked // Allows overflow without error, which is desired for hashing
            {
                int saltHash = salt.GetHashCode();
                return (config.Seed * 397) ^ saltHash;
            }
        }


        private void Clear()
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