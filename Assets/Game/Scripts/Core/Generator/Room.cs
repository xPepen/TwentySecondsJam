using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    public class Room
    {
        public Rect Rect { get; private set; }
        public bool IsSingleDoorRoom { get; set; } = false;

        // References to the generated 3D objects
        public GameObject RoomObject { get; set; }
        public List<GameObject> Doors { get; private set; } = new List<GameObject>(); // List of door references
        public NavMeshSurface NavMeshSurface { get; set; }
        
        public Room(Rect rect)
        {
            this.Rect = rect;
        }
        public Vector2 Center => new Vector2(Rect.x + Rect.width / 2f, Rect.y + Rect.height / 2f);
    }
}