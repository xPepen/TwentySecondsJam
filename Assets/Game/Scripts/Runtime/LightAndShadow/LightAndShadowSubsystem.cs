using System.Collections.Generic;
using Game.Scripts.Core.Classes;
using UnityEngine;

namespace Game.Scripts.Runtime.LightAndShadow
{
    public class LightAndShadowSubsystem : MonoBehaviour
    {
        private readonly HashSet<IMatColor> _enviroObjects = new HashSet<IMatColor>();

        private void Awake()
        {
            this.Bind();
        }

        public void Add(IMatColor enviroObject) => _enviroObjects.Add(enviroObject);
        public void Remove(IMatColor enviroObject) => _enviroObjects.Add(enviroObject);

        public void Subscribe(GameObject go)
        {
            if (go.IsA<IMatColor>(out var matColor))
            {
                Add(matColor);
            }
            else
            {
                IMatColor newMatColor = go.AddComponent<EnviroObject>();
                Add(newMatColor);
            }
        }


        public void SetColor(Color nColor)
        {
            foreach (var enviroObject in _enviroObjects)
            {
                enviroObject.SetColor(nColor);
            }
        }

        public void Clear()
        {
            _enviroObjects.Clear();
        }

        private Color Current_Color;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Current_Color = Current_Color == Color.black ? Color.white : Color.black;
                SetColor(Current_Color);
            }
        }


        private void OnDestroy()
        {
            Subsystem.UnBind<LightAndShadowSubsystem>();
        }
    }
}