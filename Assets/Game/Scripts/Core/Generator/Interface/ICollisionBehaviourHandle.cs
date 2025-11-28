
using UnityEngine;

namespace Game.Scripts.Core.Generator.Interface
{
    public interface ICollisionBehaviourHandle
    {
        public void OnTriggerEnter(GameObject investigator, Collider other);
        public void OnTriggerExit(GameObject investigator, Collider other);
    }
}