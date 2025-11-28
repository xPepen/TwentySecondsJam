using Game.Scripts.Core.Classes;
using UnityEngine;

namespace Game.Scripts.Core.Generator.Interface
{
    public class MoveDoor : ICollisionBehaviourHandle
    {
        public void OnTriggerEnter(GameObject investigator, Collider other)
        {
            if (!other.gameObject.IsA<PlayerCharacter>())
            {
                return;
            }

            if (investigator.IsA<Door>(out var obj))
            {
                obj.Open();
            }

            Debug.Log("Oh..You.Trigger.with.Player" + investigator);
        }

        public void OnTriggerExit(GameObject investigator, Collider other)
        {
            if (!other.gameObject.IsA<PlayerCharacter>())
            {
                return;
            }

            if (investigator.IsA<Door>(out var obj))
            {
                obj.Close();
            }

            Debug.Log("Oh..You.Trigger.with.Player" + investigator);
        }
    }
}