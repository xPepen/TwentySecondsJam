using System;
using Game.Scripts.Core.TimerManager;
using Game.Scripts.Runtime.Ability;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.GAS.Ability
{
    public class AbilityBase
    {
        //root owner of the ability component 80% of the time it will be a Mono
        private object _owner;

        //Mean the SO will have access to the instanced ability
        private Action<AbilityBase> _onAbilityStart;

        private Action<float> _onAbilityCooldownUpdate;

        private float _abilityRate;
        private float _abilityCooldownCounter;

        public AbilityType AbilityType { get; private set; }

        private bool _showAbilityLog;

        public GameplayTag AbilityTag { get; private set; }


        public virtual void Init(AbilityTemplate abilityTemplate, object owner)
        {
            if (!abilityTemplate)
            {
                Debug.LogError("AbilityTemplate was not found");
                return;
            }

            _showAbilityLog = abilityTemplate.ShowAbilityLog;


            if (owner == null)
            {
                if (_showAbilityLog) Debug.LogError("Owner is null");
                return;
            }

            _owner = owner;
            //Ability Initialization
            AbilityType = abilityTemplate.AbilityType;
            bool hasCooldown = AbilityType == AbilityType.Rate;

            _abilityRate = hasCooldown ? abilityTemplate.DefaultRate : -1f;
            _onAbilityStart = abilityTemplate.OnAbilityStart;
            AbilityTag = abilityTemplate.GetAbilityTag();
        }

        //todo: add new ability type here, note : passive will start enabled in the init funcntion ()
        public void StartAbility()
        {
            if (!CanPerformAbility())
            {
                return;
            }

            switch (AbilityType)
            {
                case AbilityType.Rate:
                {
                    if (IsAbilityReady())
                    {
                        _onAbilityStart?.Invoke(this);
                        //flag to start the cooldown
                        if (_showAbilityLog) Debug.Log("Ability Started");
                    }

                    break;
                }
                //do we need a king of channeling loop state here?
                case AbilityType.Channeled:
                    _onAbilityStart?.Invoke(this);
                    if (_showAbilityLog) Debug.Log("Ability Started");

                    break;
                case AbilityType.Instant:
                    _onAbilityStart?.Invoke(this);
                    if (_showAbilityLog) Debug.Log("Ability Started");

                    break;
            }
        }

        public virtual void EndAbility()
        {
            _abilityCooldownCounter = 0.001f;
        }

        public float GetAbilityRate()
        {
            return _abilityRate;
        }

        public void SetAbilityRate(float value, bool bOverride = false)
        {
            if (bOverride)
            {
                _abilityRate = value;
                return;
            }

            _abilityRate += value;
        }


        public bool HasCooldown()
        {
            return _abilityRate > 0.0000f;
        }

        public virtual bool IsAbilityReady()
        {
            return _abilityCooldownCounter == 0;
        }

        public virtual bool CanPerformAbility()
        {
            return true;
        }

        public float GetAbilityRemainingTime()
        {
            return Math.Max(0, _abilityRate - _abilityCooldownCounter);
        }

        public void UpdateAbility(float deltaTime)
        {
            if (!IsAbilityReady())
            {
                _abilityCooldownCounter += deltaTime;
                float timeRemaining = GetAbilityRemainingTime();

                _onAbilityCooldownUpdate?.Invoke(timeRemaining);
                if (_abilityCooldownCounter >= _abilityRate)
                {
                    _abilityCooldownCounter = 0;
                }
            }
        }

        public T GetOwner<T>() where T : class
        {
            if (_owner is T owner)
            {
                return owner;
            }

            return null;
        }

        public object GetOwner()
        {
            return _owner;
        }

        public bool IsOwnerValid()
        {
            return _owner != null;
        }

        public T GetComponent<T>() where T : class
        {
            if (_owner is Component component)
            {
                return component.GetComponent<T>();
            }

            return null;
        }
        
        public virtual void ClearAbility()
        {
            _owner = null;
        }
    }

    public class RateAbility : AbilityBase
    {
        private readonly TimerHandle _rateTimerIntervalHandle = new();
        public TimerHandle GetRateTimerIntervalHandle() => _rateTimerIntervalHandle;

        public override bool CanPerformAbility()
        {
            return !TimerManager.TimerExist(_rateTimerIntervalHandle);
        }

        public override void ClearAbility()
        {
            base.ClearAbility();
            TimerManager.ClearTimer(GetRateTimerIntervalHandle());
        }
    }
    
    public class PassiveAbility : AbilityBase
    {
        public PassiveType PassiveType { get; private set; }

        public bool IsEnabled { get; private set; }

        public float Duration { get; private set; }


        private readonly TimerHandle _passiveTimerIntervalHandle = new();
        private readonly TimerHandle _passiveAbilityDurationHandle = new();

        private Action<PassiveAbility> _onEnableAbility;
        private Action<PassiveAbility> _onDisableAbility;


        public TimerHandle GetTimerIntervalHandle() => _passiveTimerIntervalHandle;


        public override void Init(AbilityTemplate abilityTemplate, object owner)
        {
            base.Init(abilityTemplate, owner);

            if (abilityTemplate is PassiveAbilityTemplate passiveAbility)
            {
                _onEnableAbility = passiveAbility.OnEnableAbility;
                _onDisableAbility = passiveAbility.OnDisableAbility;

                PassiveType = passiveAbility.PassiveType;

                if (passiveAbility.ShouldStartEnabled)
                {
                    abilityTemplate.OnAbilityStart(this);
                }
            }
        }

        public override bool CanPerformAbility()
        {
            return !IsEnabled;
        }

        public virtual void OnEnableAbility()
        {
            if (IsEnabled)
            {
                return;
            }

            IsEnabled = true;
            _onEnableAbility?.Invoke(this);


            if (PassiveType == PassiveType.Infinite)
            {
                return;
            }

            if (TimerManager.TimerExist(_passiveAbilityDurationHandle))
            {
                TimerManager.ClearTimer(_passiveAbilityDurationHandle);
            }

            TimerManager.SetTimer(_passiveAbilityDurationHandle, OnDisableAbility, Duration, false);
        }

        public virtual void OnDisableAbility()
        {
            //already disabled
            if (!IsEnabled)
            {
                return;
            }
            
            IsEnabled = false;
            _onDisableAbility?.Invoke(this);
            
            if (PassiveType == PassiveType.Infinite)
            {
                return;
            }

            if (TimerManager.TimerExist(_passiveAbilityDurationHandle))
            {
                TimerManager.ClearTimer(_passiveAbilityDurationHandle);
            }
        }

        public void UpdatePassiveDuration(float newDuration)
        {
            if (PassiveType == PassiveType.Infinite)
            {
                return;
            }

            Duration = newDuration;
        }
    }
}