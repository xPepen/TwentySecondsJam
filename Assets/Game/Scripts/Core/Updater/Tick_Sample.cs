using UnityEngine;

namespace Game.Scripts.Core
{
    public class Tick_Sample : MonoBehaviour, ITickerComponent
    {
        [SerializeField] private TickPriority TickPriority;
        [SerializeField] private TickType TickType;

        public TickType GetTickType()
        {
            return TickType;
        }

        private void OnEnable()
        {
            Ticker.Register(this);
        }

        private void OnDisable()
        {
            Ticker.UnRegister(this);
        }

        public TickPriority GetTickPriority()
        {
            return TickPriority;
        }

        public void Tick(TickType tickType, float deltaTime)
        {
            if (tickType == TickType.FixedUpdate)
            {
                print("FixedUpdate");
            }
            else if (tickType == TickType.Update)
            {
                print("Update");
            }
        }


        public bool ShouldTick()
        {
            return true;
        }
    }
}