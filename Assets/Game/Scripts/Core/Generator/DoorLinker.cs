using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    public class DoorLinker : MonoBehaviour
    {
        // Public fields to hold references to the rooms this door connects
        public Room RoomA;
        public Room RoomB;

        public void Initialize(Room roomA, Room roomB)
        {
            this.RoomA = roomA;
            this.RoomB = roomB;
            
            // For debugging/organization
            this.name = $"Door_to_{roomA.Rect.center}_and_{roomB.Rect.center}";
        }
    }
}