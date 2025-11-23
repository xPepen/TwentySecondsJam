using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    public class Room
    {
         public Rect Rect { get; private set; }
        public bool IsSingleDoorRoom { get; set; } = false;

        public Room(Rect rect)
        {
            this.Rect = rect;
        }

        public Vector2 Center => new Vector2(Rect.x + Rect.width / 2f, Rect.y + Rect.height / 2f);

        public void DebugDraw()
        {
            Debug.DrawLine(new Vector3(Rect.xMin, 0, Rect.yMin), new Vector3(Rect.xMax, 0, Rect.yMin), Color.green,
                999);
            Debug.DrawLine(new Vector3(Rect.xMax, 0, Rect.yMin), new Vector3(Rect.xMax, 0, Rect.yMax), Color.green,
                999);
            Debug.DrawLine(new Vector3(Rect.xMax, 0, Rect.yMax), new Vector3(Rect.xMin, 0, Rect.yMax), Color.green,
                999);
            Debug.DrawLine(new Vector3(Rect.xMin, 0, Rect.yMax), new Vector3(Rect.xMin, 0, Rect.yMin), Color.green,
                999);
        }
    }
}