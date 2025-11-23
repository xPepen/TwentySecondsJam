using Game.Scripts.Core.Classes;
using Game.Scripts.Runtime.PlayerCore;
using Game.Scripts.UI.Item;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Core.HUD
{
    public class HUDCore : MonoBehaviour
    {
        [SerializeField] private StackableUI _stackableUI;
        
        [SerializeField]private UnityEvent OnInitializeEvent;
        private ProgressBar _healthBar;

        public PlayerController OwningPlayer { get; private set; }
        public PlayerCharacter OwningCharacter => OwningPlayer ? OwningPlayer.PlayerCharacter : null;
        

        private void Awake()
        {
            this.Bind();


            var canvas = GetComponent<Canvas>();
            _stackableUI = new StackableUI(canvas);
        }

        public void OnInitialize(PlayerController pc)
        {
            if (!pc)
            {
                Debug.LogError("PlayerController is null, cannot initialize HUD");
                return;
            }

            OwningPlayer = pc;
            OnInitializeEvent?.Invoke();
        }
        
        public void OnClear()
        {
            if (!OwningPlayer)
            {
                Debug.LogError("OwningPlayer is null, cannot Clear HUD");
                return;
            }

            var window = _stackableUI.Peek();
            while (window != null)
            {
                
                Pop();
                window = _stackableUI.Peek();
            }
        }

        private void OnDestroy()
        {
            Subsystem.UnBind<HUDCore>();
        }
        
        public void CreateAndPushWindow(GameObject prefab)
        {
            if (!prefab)
            {
                Debug.LogError("Prefab is null, cannot push HUD",this);
                return;
            }
            var window = _stackableUI.CreateWindow(prefab);
            window.Init(OwningPlayer);
            _stackableUI.Push(window);
        }

        public IUserInterfaceWindow CreateWindow(GameObject prefab)
        {
            return _stackableUI.CreateWindow(prefab);
        }

        public void Push(IUserInterfaceWindow window)
        {
            _stackableUI.Push(window);
        }

        public void Pop()
        {
            _stackableUI.Pop();
        }

        public IUserInterfaceWindow PeekInterface()
        {
            return _stackableUI.Peek();
        }

        public GameObject PeekWidget()
        {
            if (_stackableUI.Peek() == null)
            {
                return null;
            }

            if (_stackableUI.Peek() is MonoBehaviour mono)
            {
                return mono.gameObject;
            }

            return null;
        }
    }
}