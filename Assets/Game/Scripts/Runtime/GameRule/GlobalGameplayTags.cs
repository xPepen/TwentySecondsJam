using GameplayTags;
using Gas.Component;
using Gas.Variable;

namespace Game.Scripts.Runtime.GameRule
{
    public static class GlobalGameplayTags
    {
        public static readonly GameplayTag Health = GameplayTag.RequestTag("Stat.Health");
        public static readonly GameplayTag MaxHealth = GameplayTag.RequestTag("Stat.MaxHealth");
        
        public static readonly GameplayTag Experience = GameplayTag.RequestTag("Stat.Experience");
        public static readonly GameplayTag MaxExperience = GameplayTag.RequestTag("Stat.MaxExperience");
        
        public static readonly GameplayTag Speed = GameplayTag.RequestTag("Stat.Speed");
        public static readonly GameplayTag MaxSpeed = GameplayTag.RequestTag("Stat.MaxSpeed");
        
        public static FloatVariable GetVariable(GameplayTag tag, GameplayAbilityComponent gameplayAbilityComponent)
        {
            if (gameplayAbilityComponent)
            {
                return gameplayAbilityComponent.GetVariable(tag);
            }
            return null;
        }
            
    }
}