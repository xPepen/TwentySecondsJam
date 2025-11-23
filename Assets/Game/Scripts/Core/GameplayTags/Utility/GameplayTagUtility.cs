
using System.Text.RegularExpressions;

namespace GameplayTags.Utility
{
    public static class GameplayTagUtility
    {
        private static readonly Regex GameplayTagRegexValidator = new (@"^(?!\.)(?!.*\.$)(?!.*\.\.)[A-Za-z.]+$");
        public static readonly string InvalidGameplayTagName = @"\DirtyGameplayTag\b";
        
        
        // FNV-1a hash (fast + stable across runs)
        public static int GetStableHash(System.ReadOnlySpan<char> text)
        {
            unchecked
            {
                const int fnvPrime = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < text.Length; i++)
                {
                    hash ^= text[i];
                    hash *= fnvPrime;
                }

                // Always positive & deterministic
                return hash & int.MaxValue;
            }
        }
        
        public static bool IsValidTagName(string tagName)
        {
            if (tagName == null)
            {
                return false;
            }
            
            bool invalidFlag = Regex.IsMatch(tagName, InvalidGameplayTagName);

            return GameplayTagRegexValidator.IsMatch(tagName) && !invalidFlag;
        }
    }
}