using Game.Scripts.GAS.Ability;
using GameplayTags;
using Gas.Component;
using UnityEngine;

namespace Game.Scripts.Runtime.Ability
{
    [CreateAssetMenu(menuName = "GameplayAbility/PlayerJumpAbility", fileName = "NewAbilityTemplate")]
    public class JumpAbilityTemplate : AbilityTemplate
    {
        private void OnEnable()
        {
        }

        protected override void SetInternalAbilityTag()
        {
            abilityTag = GameplayTag.CreateTag("Ability.Jump");
        }

        public override void OnAbilityStart(AbilityBase ability)
        {
            var owner = ability.GetComponent<MovementComponent>();
            if (!owner.CanJump())
            {
                Debug.Log("Cannot perform jump ability");
                return;
            }

            GameplayAbilityComponent gameplayAbility = ability.GetComponent<IGameplayAbilityComponent>().GetGameplayAbilityComponent();
            if (owner)
            {
                owner.Jump();
            }

            if (gameplayAbility)
            {
                gameplayAbility.ApplyFeedBack(GameplayTag.RequestTag("Action.Jump"), owner.transform.position);
            }
        }
    }
}