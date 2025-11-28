using System;
using System.Collections.Generic;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.GAS.VariableSet
{
    [System.Serializable]
    public class GameplayVariableSetGeneric<Tag, Value>
    {
        [field: SerializeField] public Tag VariableTag { get; set; }
        [field: SerializeField] public Value VariableValue { get; set; }
    }

    [CreateAssetMenu(fileName = "GameplayPropertySet", menuName = "Game/GameplayPropertySet", order = 0)]
    public class GameplayVariableSet : ScriptableObject
    {
        [SerializeField] private List<GameplayVariableSetGeneric<GameplayTag, float>> FloatVariables = new();
        public IReadOnlyList<GameplayVariableSetGeneric<GameplayTag, float>> Get => FloatVariables;

        public void Init(Action<GameplayTag, float> onInitializedVariable)
        {
            foreach (var variable in FloatVariables)
            {
                onInitializedVariable?.Invoke(variable.VariableTag, variable.VariableValue);
            }
        }
    }
}