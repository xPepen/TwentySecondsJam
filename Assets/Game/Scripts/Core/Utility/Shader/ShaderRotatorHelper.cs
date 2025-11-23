using Game.Scripts.Core.Classes;
using UnityEngine;

namespace Game.Scripts.Core.Utility.Shader
{
    public class ShaderRotatorHelper : MonoBehaviour
    {
        [SerializeField] private float RotationSpeed = 10f;
        private Renderer Renderer;
        private MaterialPropertyBlock _mpb;

        private float _angle;

        private void Awake()
        {
            Renderer = this.GetInternalComponent<Renderer>(gameObject);
            _mpb = new MaterialPropertyBlock();
        }

        protected void Update()
        {
            float deltaTime = Time.deltaTime;
            _angle += RotationSpeed * deltaTime;

            Renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(RotationID, _angle);
            Renderer.SetPropertyBlock(_mpb);
        }

        private static readonly int RotationID = UnityEngine.Shader.PropertyToID("_Rotation");
    }
}