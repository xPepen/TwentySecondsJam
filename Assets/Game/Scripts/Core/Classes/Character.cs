using Game.Scripts.Core.Classes;
using Game.Scripts.GAS.Ability;
using Game.Scripts.GAS.VariableSet;
using Game.Scripts.Runtime.Damageable;
using Game.Scripts.Runtime.GameRule;
using GameplayTags;
using Gas.Component;
using Gas.Variable;
using UnityEngine;

[RequireComponent(typeof(GameplayAbilityComponent))]
public abstract class Character : Actor, IGameplayAbilityComponent, IDamageable
{
    [UnityEngine.HideInInspector] public GameplayAbilityComponent GameplayAbilityComponent { get; private set; }

    [SerializeField] private float InitialMultiplier = 1f;

    [SerializeField] protected GameplayVariableSet InitialPropertySet;

    [field: SerializeField] private AbilityTemplateSet AbilityTemplate;

    [SerializeField] private bool ShouldBeDestroyedOnHealthZero = false;

    public Controller Controller { get; private set; }
    public Animator Animator { get; private set; }

    public FloatVariable GetDamageMultiplier()
    {
        return GameplayAbilityComponent
            ? GameplayAbilityComponent.GetVariable(GameplayTag.RequestTag("DamageMultiplier"))
            : null;
    }
    

    protected override void OnAwake()
    {
        base.OnAwake();
        GameplayAbilityComponent = GetComponent<GameplayAbilityComponent>();
        Animator = GetComponent<Animator>();
    }

    public override void OnInitialize()
    {
        if (GameplayAbilityComponent)
        {
            GameplayAbilityComponent.Init(this, AbilityTemplate, InitialPropertySet);

            var healthVar = GlobalGameplayTags.GetVariable(GlobalGameplayTags.Health, GameplayAbilityComponent);
            healthVar.AddListener(OnHealthChanged);

            GameplayAbilityComponent.AddVariable(GameplayTag.CreateTag("DamageMultiplier"), InitialMultiplier);
        }

        base.OnInitialize();
    }

    public virtual void OnPossessed(Controller newController)
    {
        if (newController)
        {
            Controller = newController;
        }
    }

    public virtual void UnPossessed()
    {
        Controller = null;
    }

    protected virtual void OnHealthChanged(float oldValue, float newValue)
    {
        if (!ShouldBeDestroyedOnHealthZero)
        {
            return;
        }

        if (newValue <= 0)
        {
            if (transform.parent)
            {
                Destroy(transform.parent.gameObject, 0.3f);
                return;
            }

            Destroy(gameObject, 0.3f);
        }
    }

    public abstract void OnDeadFinish();


    // --- Getter Functions ---

    public bool IsDead() => !IsAlive();

    public bool IsAlive()
    {
        if (GameplayAbilityComponent)
        {
            var currentHealthVar = GameplayAbilityComponent.GetVariable(GlobalGameplayTags.Health);
            if (currentHealthVar != null)
            {
                return currentHealthVar > 0;
            }
        }

        //no health data found, can't be alive
        return false;
    }

    public virtual float GetHeight()
    {
        //default value
        return 1f;
    }

    public float GetCurrentHealth()
    {
        if (GameplayAbilityComponent)
        {
            var healthVar = GameplayAbilityComponent.GetVariable(GlobalGameplayTags.Health);
            if (healthVar != null)
            {
                return healthVar;
            }
        }

        return 0f;
    }

    public float GetMaxHealth()
    {
        if (GameplayAbilityComponent)
        {
            var maxHealthVar = GameplayAbilityComponent.GetVariable(GlobalGameplayTags.MaxHealth);
            if (maxHealthVar != null)
            {
                return maxHealthVar;
            }
        }

        return 0f;
    }

    public void Heal(float amount)
    {
        //Mean we can't find health or we are dead
        if ((GetCurrentHealth() == 0) || IsDead())
        {
            return;
        }

        if (GameplayAbilityComponent)
        {
            GameplayAbilityComponent.SetVariableAmountChange(GlobalGameplayTags.Health, amount);
        }
    }

    public FloatVariable GetVariable(GameplayTag tag)
    {
        return GameplayAbilityComponent.GetVariable(tag);
    }


    public GameplayAbilityComponent GetGameplayAbilityComponent()
    {
        return GameplayAbilityComponent;
    }

    public virtual void ApplyDamage(Actor investigator, float damageAmount, DamageHandle damageHandle)
    {
        if (!IsAlive())
        {
            return;
        }

        print("Applying damage: " + damageHandle.VariableAffected + " to " + gameObject.name);
        GameplayAbilityComponent.SetVariableAmountChange(damageHandle.VariableAffected, -damageAmount);
        GameplayAbilityComponent.ApplyFeedBack(damageHandle.GameplayEffectTag, damageHandle.HitLocation);
    }
}