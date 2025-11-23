using Game.Scripts.GAS.Ability;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Runtime.Ability
{
    public abstract class PassiveAbilityTemplate : AbilityTemplate
    {

        [field:SerializeField] public PassiveType PassiveType { get; private set; }
        [field:SerializeField] public bool ShouldStartEnabled { get; private set; } = true;

        [Tooltip("Let default value if the ability is infinite ")]
        [field: SerializeField] public float Duration { get; private set; } = 0.0f;
        
        protected override void SetInternalAbilityTag()
        {
            abilityTag = GameplayTag.CreateTag("Ability.HealthRegen");
        }


        /*
         * Note : Child should not override this function.
         * Use OnEnableAbility and OnDisableAbility instead.
         */
        public override void OnAbilityStart(AbilityBase ability)
        {
            if (ability.GetAbilityRate() <= 0f)
            {
                //default to 1 second rate if invalid rate is set
                ability.SetAbilityRate(0.1f,true);
            }

            if (ability is PassiveAbility passiveAbility)
            {
                if (!passiveAbility.IsEnabled)
                {
                    passiveAbility.OnEnableAbility();
                    return;
                }

                passiveAbility.OnDisableAbility();
            }
        }

        public abstract void OnEnableAbility(PassiveAbility ability);
        public abstract void OnDisableAbility(PassiveAbility ability);
    }
}