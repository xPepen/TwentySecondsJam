using UnityEngine;

namespace Game.Scripts.Core.HUD
{
    public class CanvasWorldSubsystem : MonoBehaviour
    {
        private Canvas _worldCanvas;

        private void Awake()
        {
            this.Bind();
            _worldCanvas = GetComponent<Canvas>();
            _worldCanvas.worldCamera = Camera.main;
            _worldCanvas.renderMode = RenderMode.WorldSpace;
        }

        private void OnDestroy()
        {
            Subsystem.UnBind<CanvasWorldSubsystem>();
        }

        public void AddChild(GameObject widget)
        {
            if (widget)
            {
                widget.transform.SetParent(_worldCanvas.transform);
            }
        }

        public void RemoveChild(GameObject widget)
        {
            // if (widget)
            {
                widget.transform.SetParent(_worldCanvas.transform);
            }
        }
    }
}