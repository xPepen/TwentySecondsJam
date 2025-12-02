using System;
using System.Collections.Generic;
using Game.Scripts.Core.Classes;
using UnityEngine;

namespace Game.Scripts.Runtime.LightAndShadow
{
    public class EnviroObject : MonoBehaviour, IMatColor
    {
        private static readonly int BaseColor = Shader.PropertyToID(ColorParam);
        private MeshRenderer _mr;
        private MaterialPropertyBlock _block;

        const string ColorParam = "_BaseColor";

        private void Awake()
        {
            _mr = gameObject.GetInternalComponent<MeshRenderer>();
            _block = new();
        }

        private void Start()
        {
            Subsystem.Get<LightAndShadowSubsystem>().Add(this);
        }

        private void OnDestroy()
        {
            LightAndShadowSubsystem subsystem = Subsystem.Get<LightAndShadowSubsystem>();
            if (subsystem)
            {
                subsystem.Remove(this);
            }
        }

        public void SetColor(Color color)
        {
            if (!_mr)
            {
                _mr = this.gameObject.GetInternalComponent<MeshRenderer>();
                if (!_mr)
                {
                    Debug.Log("No mesh renderer found ->" + gameObject.name, this);
                    return;
                }
            }

            for (int i = 0; i < _mr.materials.Length; i++)
            {
                var currColor = GetCurrentColor(i);
                var finalColor = currColor == Color.white ? Color.black : Color.white;
                SetColor(finalColor, i);
            }
        }

        private void SetColor(Color color, int subMeshIndex)
        {
            _mr.GetPropertyBlock(_block, subMeshIndex);
            _block.SetColor(BaseColor, color);
            _mr.SetPropertyBlock(_block, subMeshIndex);
        }

        public Color GetCurrentColor(int subMeshIndex)
        {
            _mr.GetPropertyBlock(_block, subMeshIndex);

            if (_block.HasColor(BaseColor))
            {
                return _block.GetColor(BaseColor);
            }

            return _mr.sharedMaterials[subMeshIndex].GetColor(BaseColor);
        }

        public void Init(List<Color> colors)
        {
            for (var i = 0; i < colors.Count; i++)
            {
                SetColor(colors[i], i);
            }
        }
    }
}