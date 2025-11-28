using Game.Scripts.Core.Generator.Interface;
using UnityEngine;

namespace Game.Scripts.Core.Generator
{
    public class Door : MonoBehaviour
    {
        [field: Header("Behaviours")]
        [field: SerializeField]
        public TNRD.SerializableInterface<ICollisionBehaviourHandle> OnTriggerWithDoor { get; private set; }

        [SerializeField] private GameObject DoorObject; 
        public Room RoomA;
        public Room RoomB;

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


        public void SetDoorHandle(ICollisionBehaviourHandle handle)
        {
            OnTriggerWithDoor.Value = handle;
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerWithDoor.Value.OnTriggerEnter(gameObject, other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            OnTriggerWithDoor.Value.OnTriggerExit(gameObject, other);
        }
    }
}