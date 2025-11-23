using System.Collections.Generic;
using Game.Scripts.GAS.VariableSet;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Core.StateMachine
{
    [System.Serializable]
    public class StateMachine : MonoBehaviour
    {
        private UnityEngine.Object _owner;

        private Dictionary<int, StateObjectBase> _states = new();
        public StateObjectBase CurrentState { get; private set; }


        private void Update()
        {
            if (ShouldUpdate(StateUpdateType.Update | StateUpdateType.All))
            {
                CurrentState.OnUpdate(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (ShouldUpdate(StateUpdateType.FixedUpdate | StateUpdateType.All))
            {
                CurrentState.OnFixedUpdate(Time.deltaTime);
            }
        }

        private bool ShouldUpdate(StateUpdateType targetType)
        {
            if (CurrentState == null)
            {
                return false;
            }

            return (CurrentState.StateUpdateType & targetType) != 0;
        }

        public void Init(UnityEngine.Object owner, List<GameplayVariableSetGeneric<GameplayTag, StateObjectBase>> stateObjects = null)
        {
            _owner = owner;

            if (stateObjects == null) return;
            
            foreach (var stateObject in stateObjects)
            {
                AddState(stateObject.VariableTag, stateObject.VariableValue);
            }
        }

        public void SwitchState(GameplayTag gameplay)
        {
            if (_states.TryGetValue(gameplay.GetID(), out var state))
            {
                CurrentState?.OnExit();

                CurrentState = state;
                CurrentState.OnEnter();
            }
        }


        public void AddState(GameplayTag gameplay, StateObjectBase state)
        {
            if (!GameplayTag.IsValid(gameplay) || state == null)
            {
                return;
            }

            if (_states.TryAdd(gameplay.GetID(), state))
            {
                state.Init(_owner, SwitchState);
            }
        }

        public void RemoveState(GameplayTag gameplay)
        {
            RemoveState(gameplay.GetID());
        }

        public void RemoveState(int tagID)
        {
            if (!GameplayTag.IsValid(tagID))
            {
                return;
            }

            if (_states.Remove(tagID, out var state))
            {
                state.OnClear();
            }
        }

        public void ClearStates()
        {
            foreach (var state in _states.Values)
            {
                state.OnClear();
            }

            //dump all states
            _states.Clear();
        }
    }
}