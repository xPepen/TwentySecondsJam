using System;
using GameplayTags;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Core.StateMachine
{
    [Flags]
    public enum StateUpdateType
    {
        None = 0,
        Update = 1,
        FixedUpdate = 2,
        Delay = 3,
        All = 4
    }

    [System.Serializable]
    public class StateObjectBase : IStateObject
    {
        [Header("Initializer")]
        [field: SerializeField]
        public float UpdateDelayInterval { get; private set; }

        [field: SerializeField] public UnityEvent OnInitialize { get; private set; }
        [field: SerializeField] public UnityEvent OnStateClear { get; private set; }

        public StateUpdateType StateUpdateType { get; private set; }

        [Header("Behaviour Events")]
        [field: SerializeField]
        public UnityEvent OnStateEnter { get; private set; }

        [field: SerializeField] public UnityEvent<float> OnStateUpdate { get; private set; }
        [field: SerializeField] public UnityEvent OnStateExit { get; private set; }

        private UnityEngine.Object _owner;

        private Action<GameplayTag> SwitchStateAction { get; set; }

        public GameplayTag GetStateTag()
        {
            return GameplayTag.RequestTag("State.blabla");
        }

        public void Init(UnityEngine.Object owner, Action<GameplayTag> switchState)
        {
            _owner = owner;
            SwitchStateAction = switchState;
            OnInitialize?.Invoke();
        }

        public void SwitchState(GameplayTag tag)
        {
            SwitchStateAction?.Invoke(tag);
        }

        public void OnEnter()
        {
            OnStateEnter?.Invoke();
            Debug.Log("owner = " + _owner);
        }

        public void OnUpdate(float deltaTime)
        {
            OnStateUpdate?.Invoke(deltaTime);
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
        }

        public void OnUpdateDelay()
        {
        }

        public void OnExit()
        {
            OnStateExit?.Invoke();
        }

        public void OnClear()
        {
            OnStateClear?.Invoke();
        }
    }
}