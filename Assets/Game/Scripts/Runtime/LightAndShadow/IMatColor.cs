namespace Game.Scripts.Runtime.LightAndShadow
{
    public enum ColorType
    {
        Black,
        White
    }
    public interface IMatColor
    {
        public void SetColor(ColorType type);
    }
}