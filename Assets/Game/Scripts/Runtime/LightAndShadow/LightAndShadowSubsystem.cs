using System;
using System.Collections.Generic;
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
        
        
        public void SetColor(ColorType colortype)
        {
            foreach (var enviroObject in _enviroObjects)
            {
                enviroObject.SetColor(colortype);
            }
        }

        private ColorType x;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                x = x == ColorType.Black ? ColorType.White : ColorType.Black;
                SetColor(x);
            }
        }


        private void OnDestroy()
        {
            Subsystem.UnBind<LightAndShadowSubsystem>();
        }
    }
}