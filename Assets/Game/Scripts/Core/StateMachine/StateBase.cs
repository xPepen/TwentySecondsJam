using System;
using GameplayTags;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Core.StateMachine
{
    [System.Serializable]
    public class StateBase : IStateObject, ITickerComponent
    {
        [field: SerializeField] public TickType TickType { get; private set; }
        [field: SerializeField] public TickPriority TickPriority { get; private set; }

        [Header("Behaviour Events")]
        [field: SerializeField]
        public UnityEvent OnInitialize { get; private set; }

        [field: SerializeField] public UnityEvent OnStateEnter { get; private set; }
        [field: SerializeField] public UnityEvent<float> OnStateUpdate { get; private set; }
        [field: SerializeField] public UnityEvent<float> OnStateFixedUpdate { get; private set; }
        [field: SerializeField] public UnityEvent OnStateExit { get; private set; }
        [field: SerializeField] public UnityEvent OnStateDestroy { get; private set; }


        private UnityEngine.Object _owner;
        private GameplayTag _stateTag;
        private Action<GameplayTag> _switchStateAction;

        public GameplayTag GetStateTag()
        {
            return _stateTag;
        }

        public virtual void Init(UnityEngine.Object owner, Action<GameplayTag> switchState, GameplayTag stateTag)
        {
            _owner = owner;
            _switchStateAction = switchState;
            _stateTag = stateTag;
            OnInitialize?.Invoke();

            if (TickType != TickType.None)
            {
                Ticker.Register(this);
            }
        }

        public virtual void SwitchState(GameplayTag tag)
        {
            _switchStateAction?.Invoke(tag);
        }

        public virtual void OnEnter()
        {
            OnStateEnter?.Invoke();
        }

        public void Tick(TickType tickType, float deltaTime)
        {
            switch (tickType)
            {
                case TickType.Update:
                    OnUpdate(deltaTime);
                    break;
                case TickType.FixedUpdate:
                    OnFixedUpdate(deltaTime);
                    break;
            }
        }

        public virtual void OnUpdate(float deltaTime)
        {
            OnStateUpdate?.Invoke(deltaTime);
        }

        public virtual void OnFixedUpdate(float fixedDeltaTime)
        {
            OnStateFixedUpdate?.Invoke(fixedDeltaTime);
        }


        public virtual void OnExit()
        {
            OnStateExit?.Invoke();
        }

        public void OnDestroy()
        {
            Ticker.UnRegister(this);
            OnStateDestroy?.Invoke();
        }

        public TickType GetTickType()
        {
            return TickType;
        }

        public TickPriority GetTickPriority()
        {
            return TickPriority;
        }

        public bool ShouldTick()
        {
            return GetTickType() != TickType.None;
        }
    }
}