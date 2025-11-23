using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Core.Classes
{
    public class Actor : Braviour
    {
        public Renderer Renderer { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            Renderer = GetComponent<Renderer>();
            if (!Renderer)
            {
                Renderer = GetComponentInChildren<Renderer>();
            }
        }

        public bool IsRender(Camera activeCamera)
        {
            if (!activeCamera)
            {
                return Renderer.isVisible; // simplest case
            }

            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(activeCamera),
                Renderer.bounds);
        }
    }
}