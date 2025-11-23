namespace Game.Scripts.Core.Pool
{
    public interface IPooler
    {
        public void OnPush();
        public void OnPop();
    }
}