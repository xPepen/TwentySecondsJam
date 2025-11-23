using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Game.Scripts.Core.HUD
{
    [System.Serializable]
    public class StackableUI
    {
        private readonly Canvas _root;

        private Stack<IUserInterfaceWindow> _UIStack;
        //todo: add pooling
        private PooledObject<IUserInterfaceWindow> Pool;

        [SerializeField] private UnityEvent OnWindowPushed;
        [SerializeField] private UnityEvent OnWindowPopped;
        private GameObject _inactiveContainer;
        public StackableUI(Canvas inRoot)
        {
            _root = inRoot;
            _UIStack = new();
            _inactiveContainer = new GameObject("Inactive_UI_Container");
        }

        public IUserInterfaceWindow CreateWindow(GameObject prefab)
        {
            var interfaceWindow = prefab.GetComponent<IUserInterfaceWindow>();
            if (interfaceWindow == null)
            {
                Debug.LogError($"{prefab.name}: Can't find IUserInterfaceWindow");
                return null;
            }
            
            if (prefab.scene.IsValid() && prefab.scene.name != null)
            {
                return interfaceWindow;
            }

            var newWindow = Object.Instantiate(prefab);
            newWindow.SetActive(false);
            
            return newWindow.GetComponent<IUserInterfaceWindow>();
        }

        public void Push(IUserInterfaceWindow window)
        {
            if (window is not MonoBehaviour behaviour)
            {
                Debug.LogError($"Pushed window : is not from a monoBehaviour");
                return;
            }
            
            if (_UIStack.Count > 0)
            {
                var topWindow = _UIStack.Peek();
                if (!topWindow.ShouldAlwaysBeVisible())
                {
                    topWindow.OnPopFromStack();
                }
            }
            Transform transform = behaviour.transform;
            transform.SetParent(_root.transform, false);
            transform.gameObject.SetActive(true);

            _UIStack.Push(window);
            window.OnPushToStack();

            OnWindowPushed?.Invoke();
        }

        public void Pop()
        {
            if (_UIStack.Count == 0)
            {
                Debug.LogError("UI Stack is empty. Cannot pop.");
                return;
            }

            var topWindow = _UIStack.Peek();
            topWindow.OnPopFromStack();

            Transform transform = ((MonoBehaviour)topWindow).transform;
            transform.SetParent(_inactiveContainer.transform, false);
            transform.gameObject.SetActive(false);
            
            _UIStack.Pop();
            OnWindowPopped?.Invoke();

            if (_UIStack.Count > 0)
            {
                var newTopWindow = _UIStack.Peek();
                newTopWindow.OnPushToStack();
                
                transform = ((MonoBehaviour)newTopWindow).transform;
                
                transform.SetParent(_root.transform, false);
                transform.gameObject.SetActive(true);
            }
        }
        
        public IUserInterfaceWindow Peek()
        {
            if (_UIStack.Count == 0)
            {
                Debug.LogError("UI Stack is empty. Cannot peek.");
                return null;
            }

            return _UIStack.Peek();
        }
    }
}