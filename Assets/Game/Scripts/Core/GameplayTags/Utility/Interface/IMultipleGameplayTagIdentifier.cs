namespace GameplayTags.Utility
{
    public interface IMultipleGameplayTagIdentifier
    {
        public void AddTag(GameplayTags.GameplayTag tagToAdd);
        public void RemoveTag(GameplayTags.GameplayTag tagToRemove);
        public GameplayTags.GameplayTagContainer GetMultipleTags();
    }
}