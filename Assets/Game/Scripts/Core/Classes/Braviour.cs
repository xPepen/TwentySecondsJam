using System;
using Game.Scripts.Core;
using GameplayTags;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class CollisionEvent : UnityEvent<Collision>
{
}

[System.Serializable]
public class ColliderEvent : UnityEvent<Collider>
{
}


public abstract class Braviour : MonoBehaviour, ITickerComponent
{
    [System.Serializable]
    public class EventsGroup
    {
        [Header("Lifecycle")] public UnityEvent OnAwakeEvent;
        public UnityEvent OnStartEvent;
        public UnityEvent OnUpdateEvent;
        public UnityEvent OnFixedUpdateEvent;
        public UnityEvent OnlateUpdateEvent;
        public UnityEvent OnDestroyEvent;
        public UnityEvent CustomInitializeEvent;

        [Header("Physics 2D")] public CollisionEvent OnCollisionEnterEvent;
        public CollisionEvent OnCollisionExitEvent;

        public ColliderEvent OnTriggerEnterEvent;
        public ColliderEvent OnTriggerExitEvent;
    }

    [System.Serializable]
    public class TickerBehaviour
    {
        [field: SerializeField] public TickPriority TickPriority { get; set; }
        [field: SerializeField] public TickType TickType { get; set; }
        [field: SerializeField] public bool TickEnabled { get; set; }
    }

    [SerializeField] private TickerBehaviour tickerBehaviour = new();
    [SerializeField] protected EventsGroup events = new();
    [SerializeField] private GameplayTagContainer _tagContainer;

    // --- Public Getters ---
    public UnityEvent OnAwakeEvent => events.OnAwakeEvent;
    public UnityEvent OnStartEvent => events.OnStartEvent;
    public UnityEvent OnUpdateEvent => events.OnUpdateEvent;
    public UnityEvent OnFixedUpdateEvent => events.OnFixedUpdateEvent;
    public UnityEvent OnLateUpdateEvent => events.OnlateUpdateEvent;
    public UnityEvent OnDestroyEvent => events.OnDestroyEvent;
    public UnityEvent CustomInitializeEvent => events.CustomInitializeEvent;

    public CollisionEvent OnCollisionEnterEvent => events.OnCollisionEnterEvent;
    public CollisionEvent OnCollisionExitEvent => events.OnCollisionExitEvent;
    public ColliderEvent OnTriggerEnterEvent => events.OnTriggerEnterEvent;
    public ColliderEvent OnTriggerExitEvent => events.OnTriggerExitEvent;

    public GameplayTagContainer TagContainer => _tagContainer;


    public bool HasTag(GameplayTag gameplayTag)
    {
        return _tagContainer.MatchTag(gameplayTag);
    }

    // --- MonoBehaviour Callbacks ---
    protected void Awake()
    {
        OnAwake();
    }

    protected void Start()
    {
        OnStart();
    }
    protected void Update() { }

    protected void FixedUpdate() { }

    private void LateUpdate() { }

    // --------------------------------------------------------------
    // Tick
    // --------------------------------------------------------------
    public TickType GetTickType()
    {
        return tickerBehaviour.TickType;
    }

    protected virtual void OnEnable()
    {
        if (ShouldTick())
        {
            Ticker.Register(this);
        }
    }

    protected virtual void OnDisable()
    {
        if (ShouldTick())
        {
            Ticker.UnRegister(this);
        }
    }

    public TickPriority GetTickPriority()
    {
        return tickerBehaviour.TickPriority;
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

            case TickType.LateUpdate:
                OnLateUpdate(deltaTime);
                break;
        }
    }


    public void SetTickEnabled(bool active)
    {
        if (!enabled)
        {
            Debug.LogWarning("Can't change tick state when object is disabled");
        }

        //nothing have changed
        if (ShouldTick() == active)
        {
            return;
        }

        tickerBehaviour.TickEnabled = active;

        if (tickerBehaviour.TickEnabled)
        {
            Ticker.Register(this);
            return;
        }

        Ticker.UnRegister(this);
    }

    public bool ShouldTick()
    {
        return tickerBehaviour.TickEnabled;
    }

    private void OnCollisionEnter(Collision other)
    {
        OnCollisionEnterVirtual(other);
    }

    private void OnCollisionExit(Collision other)
    {
        OnCollisionExitVirtual(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterVirtual(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitVirtual(other);
    }

    private void OnDestroy()
    {
        OnDestroyVirtual();
    }

    // --- Virtual Methods for Inheritance ---
    protected virtual void OnAwake()
    {
        OnAwakeEvent?.Invoke();
    }

    protected virtual void OnStart()
    {
        OnStartEvent?.Invoke();
    }

    protected virtual void OnUpdate(float deltaTime)
    {
        OnUpdateEvent?.Invoke();
    }

    protected virtual void OnFixedUpdate(float fixedDeltaTime)
    {
        OnFixedUpdateEvent?.Invoke();
    }

    protected virtual void OnLateUpdate(float deltaTime)
    {
        OnLateUpdateEvent?.Invoke();
    }

    protected virtual void OnCollisionEnterVirtual(Collision other)
    {
        OnCollisionEnterEvent?.Invoke(other);
    }

    protected virtual void OnCollisionExitVirtual(Collision other)
    {
        OnCollisionExitEvent?.Invoke(other);
    }

    protected virtual void OnTriggerEnterVirtual(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    protected virtual void OnTriggerExitVirtual(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }

    protected virtual void OnDestroyVirtual()
    {
        OnDestroyEvent?.Invoke();
    }

    public virtual void OnInitialize()
    {
        CustomInitializeEvent?.Invoke();
    }
}