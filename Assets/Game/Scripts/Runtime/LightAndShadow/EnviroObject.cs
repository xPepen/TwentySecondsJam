using UnityEngine;

namespace Game.Scripts.Runtime.LightAndShadow
{
    public class EnviroObject : MonoBehaviour, IMatColor
    {
        private MeshRenderer _mr;
        private MaterialPropertyBlock _block;

        private void Awake()
        {
            _mr = GetComponent<MeshRenderer>();
            _block = new();
        }

        // public void Init()
        // {
        //     foreach (var v in (_mr.sharedMaterials))
        //     {
        //         var block = new MaterialPropertyBlock();
        //
        //         _mr.GetPropertyBlock(block);
        //         block.SetColor("_BaseColor", Color.green); // or "_Color"
        //         _mr.SetPropertyBlock(block);
        //     }
        // }

        public void SetColor(ColorType type)
        {
            Color c = type == ColorType.Black ? Color.black : Color.white;
        
            if (!_mr)
            {
                _mr = GetComponent<MeshRenderer>();
                if (!_mr)
                {
                    Debug.Log("No mesh renderer found", this);
                    return;
                }
            }
        
            for (int i = 0; i < _mr.materials.Length; i++)
            {
                _mr.GetPropertyBlock(_block, i);
                _block.SetColor("_BaseColor", c);
                _mr.SetPropertyBlock(_block, i);
            }
        }
        
        public void SetMultipleColor(ColorType matColorA, ColorType matColorB)
        {
            Color colorA = matColorA == ColorType.Black ? Color.black : Color.white;
            Color colorB = matColorB == ColorType.Black ? Color.black : Color.white;
        
            
            _mr.GetPropertyBlock(_block, 0);
            _block.SetColor("_BaseColor", colorA);
            _mr.SetPropertyBlock(_block, 0);
            
            _mr.GetPropertyBlock(_block, 1);
            _block.SetColor("_BaseColor", colorB);
            _mr.SetPropertyBlock(_block, 1);
        }
    }
}