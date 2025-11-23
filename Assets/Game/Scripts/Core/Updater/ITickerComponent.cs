namespace Game.Scripts.Core
{
    public interface ITickerComponent
    {
        TickType GetTickType();
        TickPriority GetTickPriority();
        public void Tick(TickType tickType, float deltaTime);
        bool ShouldTick();
    }

    [System.Flags]
    [System.Serializable]
    public enum TickType
    {
        None = 0,
        Update = 1 << 0,
        FixedUpdate = 1 << 1,
        LateUpdate = 1 << 2,
        All = Update | FixedUpdate | LateUpdate
    }

    [System.Serializable]
    public enum TickPriority
    {
        High,
        Medium,
        Low
    }
}