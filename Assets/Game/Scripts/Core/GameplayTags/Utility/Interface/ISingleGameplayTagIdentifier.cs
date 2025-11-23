namespace GameplayTags.Utility
{
    public interface ISingleGameplayTagIdentifier
    {
        public void SetTag(GameplayTags.GameplayTag tag);
        public void ClearTag();
        public GameplayTags.GameplayTag GetTag();
    }
}