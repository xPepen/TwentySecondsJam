using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private GameObject DoorObject;
        public Room RoomA;
        public Room RoomB;

        public bool IsLocked { get; private set; }

        public void Lock()
        {
            IsLocked = true;
        }

        public void UnLock()
        {
            IsLocked = false;
        }

        public void Initialize(Room roomA, Room roomB)
        {
            this.RoomA = roomA;
            this.RoomB = roomB;
            this.name = $"Door_to_{roomA.Rect.center}_and_{roomB.Rect.center}";
        }

        public void Open()
        {
            var v = DoorObject.transform.position;
            v.y += 200f;
            DoorObject.transform.position = v;
        }

        public void Close()
        {
            var v = DoorObject.transform.position;
            v.y -= 200f;
            DoorObject.transform.position = v;
        }


        private void OnTriggerEnter(Collider other)
        {
            Open();
        }

        private void OnTriggerExit(Collider other)
        {
            Close();
        }
    }
}