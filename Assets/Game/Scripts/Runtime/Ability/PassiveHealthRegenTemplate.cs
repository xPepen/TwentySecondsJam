using Game.Scripts.Core.TimerManager;
using Game.Scripts.GAS.Ability;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Runtime.Ability
{
    [UnityEngine.CreateAssetMenu(menuName = "GameplayAbility/PassiveHealthRegenTemplate",
        fileName = "PassiveHealthRegenTemplate")]
    public class PassiveHealthRegenTemplate : PassiveAbilityTemplate
    {
        
        [SerializeField]private float HealAmount = 5f;
        protected override void SetInternalAbilityTag()
        {
            abilityTag = GameplayTag.CreateTag("Ability.HealthRegen");
        }


        public override void OnEnableAbility(PassiveAbility ability)
        {
            void RegenAction()
            {
                var owner = ability.GetOwner<Character>();
                if (!owner)
                {
                    Debug.Log("No character owner found for health regen.");
                    return;
                }

                if (!owner.GameplayAbilityComponent)
                {
                    Debug.Log($"No gameplay component found for health regen.{owner.name}");
                    return;
                }

                var healthVar = owner.GetCurrentHealth();
                var maxHealthVar = owner.GetMaxHealth();

                if (healthVar >= maxHealthVar)
                {
                    //nothing to heal here
                    if (healthVar > maxHealthVar)
                    {
                        owner.GameplayAbilityComponent.SetVariableOverride(GameplayTag.RequestTag("Health"), maxHealthVar);
                    }
                    return;
                }

                owner.Heal(HealAmount);
            }

            TimerManager.SetTimer(ability.GetTimerIntervalHandle(), RegenAction,  ability.GetAbilityRate(), true);
        }

        public override void OnDisableAbility(PassiveAbility ability)
        {
        }
    }
}