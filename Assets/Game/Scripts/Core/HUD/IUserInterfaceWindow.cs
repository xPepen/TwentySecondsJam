using Game.Scripts.Runtime.PlayerCore;

namespace Game.Scripts.Core.HUD
{
    public interface IUserInterfaceWindow
    {
        public void Init(PlayerController pc);
        public void OnPushToStack();
        public void OnPopFromStack();
        public bool ShouldAlwaysBeVisible();
    }
}