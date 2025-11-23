using GameplayTags;
using Gas.Component;
using UnityEngine;

namespace Game.Scripts.Runtime.Utility
{
    public class ApplyValueToEntityMono : MonoBehaviour
    {
        [SerializeField] private float ValueAmount = 10f;
        [SerializeField] private GameObject obj;
        [SerializeField] GameplayTag PropertyToApplyValue;
        [SerializeField] private ApplyValueType HowToApplyValue = ApplyValueType.Remove;

        public void ApplyValue()
        {
            if (!obj)
            {
                Debug.Log("No character found to apply value to");
                return;
            }

            Character character = obj.GetComponent<Character>();
            if (!character)
            {
                character = obj.GetComponentInChildren<Character>();
            }
            
            if (character)
            {
                GameplayAbilityComponent gameplayAbilityComponent = character.GameplayAbilityComponent;
                if (gameplayAbilityComponent)
                {
                    var variable = gameplayAbilityComponent.GetVariable(PropertyToApplyValue);
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
                    Debug.Log("Entity does not have a GameplayAbilityComponent.");
                }
            }
        }
    }
}