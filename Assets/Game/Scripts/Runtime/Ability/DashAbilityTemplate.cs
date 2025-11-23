using Game.Scripts.GAS.Ability;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Runtime.Ability
{
    
    [CreateAssetMenu(menuName = "GameplayAbility/DashAbility", fileName = "NewAbilityTemplate")]
    public class DashAbilityTemplate : AbilityTemplate
    {
        //[]feedbakccue
        protected override void SetInternalAbilityTag()
        {
            abilityTag = GameplayTag.CreateTag("Ability.Dash");
        }

        void InitFeedBack(AbilityBase ability)
        {
            // var owner = ability.GetComponent<MovementComponent>();
            //owner.getdashaction += feedbackcue.play();
        }

        public override void OnAbilityStart(AbilityBase ability)
        {
            var owner = ability.GetComponent<MovementComponent>();
            if (owner)
            {
                owner.Dash();
            }
        }
    }
}