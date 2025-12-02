using System.Collections.Generic;
using Game.Scripts.Core.Utility;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Core.StateMachine
{
    [System.Serializable]
    public class StateMachine : MonoBehaviour
    {
        private UnityEngine.Object _owner;


        [SerializeField] private GameplayTag InitialState;

        //only act as initializer container for editor
        [SerializeField] private List<Binder<GameplayTag, StateBase>> stateObjects = new();

        private Dictionary<int, StateBase> _states = new();
        public StateBase CurrentState { get; private set; }

        public void f(float x)
        {
            print(x);
        }
        // private void Update()
        // {
        //     if (ShouldUpdate(TickType.Update | TickType.All))
        //     {
        //         CurrentState.OnUpdate(Time.deltaTime);
        //     }
        // }
        //
        // private void FixedUpdate()
        // {
        //     if (ShouldUpdate(TickType.FixedUpdate | TickType.All))
        //     {
        //         CurrentState.OnFixedUpdate(Time.deltaTime);
        //     }
        // }
        //
        // private bool ShouldUpdate(TickType targetType)
        // {
        //     if (CurrentState == null)
        //     {
        //         return false;
        //     }
        //
        //     return (CurrentState.StateUpdateType & targetType) != 0;
        // }

        public void Init(Object owner)
        {
            foreach (var stateObject in stateObjects)
            {
                AddState(stateObject.ItemA, stateObject.ItemB);
            }

            SwitchState(InitialState);
        }

        public StateBase GetState(GameplayTag gameplay)
        {
            // ReSharper disable once CanSimplifyDictionaryTryGetValueWithGetValueOrDefault
            if (_states.TryGetValue(gameplay.GetID(), out var state))
            {
                return state;
            }

            return null;
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


        public void AddState(GameplayTag gameplay, StateBase state)
        {
            if (!GameplayTag.IsValid(gameplay) || state == null)
            {
                return;
            }

            if (_states.TryAdd(gameplay.GetID(), state))
            {
                state.Init(_owner, SwitchState, gameplay);
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
                state.OnDestroy();
            }
        }

        public void DestroyStates()
        {
            foreach (var state in _states.Values)
            {
                state.OnDestroy();
            }

            _states.Clear();
        }
    }
}