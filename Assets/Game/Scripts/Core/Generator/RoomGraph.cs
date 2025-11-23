using System.Collections.Generic;

namespace Game.Scripts.Core.Generator
{
    public class RoomGraph
    {
        public List<Room> Rooms { get; private set; }
        public Dictionary<Room, List<Room>> Neighbors { get; private set; }

        public RoomGraph(List<Room> rooms, List<(Room, Room)> connections)
        {
            Rooms = rooms;
            Neighbors = new Dictionary<Room, List<Room>>();

            foreach (var r in rooms)
                Neighbors[r] = new List<Room>();

            foreach (var (roomA, roomB) in connections)
            {
                if (!Neighbors[roomA].Contains(roomB)) Neighbors[roomA].Add(roomB);
                if (!Neighbors[roomB].Contains(roomA)) Neighbors[roomB].Add(roomA);
            }
        }
    }
}