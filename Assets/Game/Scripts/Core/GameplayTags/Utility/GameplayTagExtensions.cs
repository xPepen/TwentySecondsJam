using UnityEngine;

namespace GameplayTags.Utility
{
    public static class GameplayTagExtensions
    {
        private static T GetComponentInternal<T>(GameObject @object, bool searchInChild = false, bool searchInParent = false) where T : class
        {
            if (@object.TryGetComponent<T>(out var result))
            {
                return result;
            }

            if (searchInChild)
            {
                result = @object.GetComponentInChildren<T>();
                if (result != null)
                {
                    return result;
                }
            }

            if (searchInParent)
            {
                result = @object.GetComponentInParent<T>();
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static bool HasTags(this GameObject obj, GameObject other, GameplayTags.GameplayTagContainer tagContainer, bool searchInChild = false, bool searchInParent = false)
        {
            var interfaceClass = GetComponentInternal<IMultipleGameplayTagIdentifier>(other, searchInChild, searchInParent);
            if (interfaceClass != null && interfaceClass.GetMultipleTags() != null)
            {
                return interfaceClass.GetMultipleTags().MatchAllTags(tagContainer);
            }

            return false;
        }

        public static bool HasTag(this GameObject obj,GameplayTags.GameplayTag tag, bool searchInChild = false, bool searchInParent = false)
        {
            var interfaceClass = GetComponentInternal<ISingleGameplayTagIdentifier>(obj, searchInChild, searchInParent);
            if (interfaceClass != null && interfaceClass.GetTag())
            {
                return interfaceClass.GetTag().IsEqualToTag(tag);
            }
            return false;
        }
    }
}