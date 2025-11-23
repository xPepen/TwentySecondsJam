using Game.Scripts.Core.Classes;
using GameplayTags;
using Gas.Component;
using UnityEngine;

namespace Game.Scripts.Runtime.Utility
{
    public enum ApplyValueType
    {
        Add,
        Remove,
        Multiply,
        Divide
    }

    [CreateAssetMenu(menuName = "Game/Utility/ApplyValueToEntity", fileName = "ApplyValueToEntity")]
    public class ApplyValueToEntity : ScriptableObject
    {
        [SerializeField] private float ValueAmount = 10f;
        [SerializeField] private int PlayerIndex = 0;
        [SerializeField] GameplayTag PropertyToApplyValue;
        [SerializeField] private ApplyValueType HowToApplyValue = ApplyValueType.Remove;

        public void ApplyValue(Character character)
        {
            Character playerCharacter = character == null ? GameInstance.GetPlayerByIndex(PlayerIndex).PlayerCharacter : character;
            if (playerCharacter)
            {
                GameplayAbilityComponent gameplayAbilityComponent = playerCharacter.GameplayAbilityComponent;
                if (gameplayAbilityComponent)
                {
                    var variable = gameplayAbilityComponent.GetVariable(PropertyToApplyValue);
                    if (variable != null)
                    {
                        switch (HowToApplyValue)
                        {
                            case ApplyValueType.Add:
                                gameplayAbilityComponent.SetVariableAmountChange(PropertyToApplyValue, ValueAmount);
                                break;
                            case ApplyValueType.Remove:
                                gameplayAbilityComponent.SetVariableAmountChange(PropertyToApplyValue, -ValueAmount);
                                break;
                            case ApplyValueType.Multiply:
                                gameplayAbilityComponent.SetVariableAmountChange(PropertyToApplyValue,
                                    variable.Value * (ValueAmount - 1));
                                break;
                            case ApplyValueType.Divide:
                                if (ValueAmount != 0)
                                {
                                    gameplayAbilityComponent.SetVariableAmountChange(PropertyToApplyValue,
                                        variable.Value * (1 - 1 / ValueAmount));
                                }
                                else
                                {
                                    Debug.LogWarning("Cannot divide by zero.");
                                }

                                break;
                        }
                    }
                    else
                    {
                        Debug.Log($"Player does not have the property: {PropertyToApplyValue.name}");
                    }
                }
                else
                {
                    Debug.Log("Player does not have a GameplayAbilityComponent.");
                }
            }
        }
    }
}