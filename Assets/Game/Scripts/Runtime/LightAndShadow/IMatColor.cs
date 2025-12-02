using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Runtime.LightAndShadow
{
    public interface IMatColor
    {
        public void SetColor(Color color);
        public void Init(List<Color> colors);
    }
}