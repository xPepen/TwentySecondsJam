namespace Game.Scripts.Runtime.Data
{
    internal static class DefaultPlayerSetting
    {
        public static readonly float DefaultMouseSensitivity = 0.5f;
    }

    public static class PlayerSetting
    {
        private static float _mouseSensitivity = DefaultPlayerSetting.DefaultMouseSensitivity;

        public static float Sensitivity
        {
            get => _mouseSensitivity * 100;
            set => _mouseSensitivity = UnityEngine.Mathf.Clamp(value, 0, 1);
        }
    }
}