using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    public class RoomLayoutGenerator
    {
        private LevelGenConfig config;

        private const float BRANCH_CHANCE = 0.5f;
        private const float SINGLE_DOOR_CHANCE = 0.20f;

        public class LevelData
        {
            public List<Room> Rooms;
            public List<(Room, Room)> Connections;
        }

        private Dictionary<Vector2Int, Room> gridRooms = new();
        private Queue<Vector2Int> frontier = new Queue<Vector2Int>();

        private List<(Room, Room)> connections = new List<(Room, Room)>();

        private static readonly Vector2Int[] DIRECTIONS =
        {
            new(1, 0), new(-1, 0), new(0, 1), new(0, -1)
        };

        public RoomLayoutGenerator(LevelGenConfig config)
        {
            this.config = config;
        }

        public LevelData GenerateLevel()
        {
            gridRooms.Clear();
            frontier.Clear();
            connections.Clear();

            Vector2Int start = Vector2Int.zero;
            gridRooms[start] = CreateRoomAtGrid(start);
            frontier.Enqueue(start);

            while (gridRooms.Count < config.RoomCount)
            {
                if (frontier.Count == 0)
                {
                    var keys = gridRooms.Keys.ToList();
                    frontier.Enqueue(keys[Random.Range(0, keys.Count)]);
                }

                var currentPos = frontier.Dequeue();
                Room currentRoom = gridRooms[currentPos];

                if (currentRoom.IsSingleDoorRoom)
                    continue;

                foreach (var dir in DIRECTIONS)
                {
                    if (gridRooms.Count >= config.RoomCount) break;

                    Vector2Int nextPos = currentPos + dir;

                    // If already occupied, skip (Spanning Tree style)
                    if (gridRooms.ContainsKey(nextPos))
                        continue;

                    // Random chance to skip this direction
                    if (Random.value > BRANCH_CHANCE)
                        continue;

                    // Create the new room
                    Room newRoom = CreateRoomAtGrid(nextPos);

                    int neighborCount = CountNeighbors(currentPos);
                    bool parentSingle = (neighborCount <= 1);
                    newRoom.IsSingleDoorRoom = parentSingle && Random.value < SINGLE_DOOR_CHANCE;

                    gridRooms[nextPos] = newRoom;
                    frontier.Enqueue(nextPos);

                    connections.Add((currentRoom, newRoom));
                }
            }

            return new LevelData
            {
                Rooms = new List<Room>(gridRooms.Values),
                Connections = connections
            };
        }

        private Room CreateRoomAtGrid(Vector2Int gridPos)
        {
            Vector2 size = GenerateSize();

            float cellWidth = config.RoomSizeMax.x * config.Spacing;
            float cellHeight = config.RoomSizeMax.y * config.Spacing;

            Vector2 worldCenter = new Vector2(
                gridPos.x * cellWidth,
                gridPos.y * cellHeight
            );

            Rect rect = new Rect(
                worldCenter.x - size.x / 2f,
                worldCenter.y - size.y / 2f,
                size.x,
                size.y
            );

            return new Room(rect);
        }

        private Vector2 GenerateSize()
        {
            return new Vector2(
                Random.Range(config.RoomSizeMin.x, config.RoomSizeMax.x),
                Random.Range(config.RoomSizeMin.y, config.RoomSizeMax.y)
            );
        }

        private int CountNeighbors(Vector2Int cell)
        {
            int n = 0;
            foreach (var d in DIRECTIONS)
                if (gridRooms.ContainsKey(cell + d))
                    n++;
            return n;
        }
    }
}