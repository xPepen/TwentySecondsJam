using System.Collections.Generic;
using Game.Scripts.Core.Classes;
using UnityEngine;

namespace Game.Scripts.Runtime.Ability.Class
{
    [RequireComponent(typeof(SphereCollider))]
    public class AbilityAttractorComponent : MonoBehaviour
    {
        private SphereCollider _sphereCollider;

        private readonly LinkedList<Rigidbody> _attractedBodies = new LinkedList<Rigidbody>();

        private void Awake()
        {
            _sphereCollider = GetComponent<SphereCollider>();
            _sphereCollider.isTrigger = true;
        }

        public void SetAttractorRadius(float newRadius)
        { 
            _sphereCollider.radius = newRadius;
        }

        public void AddAttractedBody(Rigidbody rb)
        {
            if (rb && !_attractedBodies.Contains(rb))
            {
                _attractedBodies.AddLast(rb);
            }
        }
        
        public void RemoveAttractedBody(Rigidbody rb)
        {
            if (rb && _attractedBodies.Contains(rb))
            {
                _attractedBodies.Remove(rb);
            }
        }
        private void OnCollisionEnter(Collision other)
        {
            Rigidbody rb = other.rigidbody;

            if (rb)
            {
                _attractedBodies.AddLast(rb);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            Rigidbody rb = other.rigidbody;
            if (rb)
            {
                _attractedBodies.Remove(rb);
            }
        }

        private void Tick()
        {
            // Define the ray from the object's position and forward direction
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            // Physics.SphereCastAll<Actor>(transform.position, _sphereCollider.radius, Vector3.zero, 1,
            //     LayerMask.GetMask("Default"));
            // {
            //     
            //     Debug.Log("SphereCast hit: " + hit.collider.gameObject.name);
            //     // You can access other hit information like hit.point, hit.normal, etc.
            // }
        }
    }
}