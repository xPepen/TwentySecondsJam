using System;
using Game.Scripts.Core.Classes;
using Game.Scripts.Core.HUD;
using Game.Scripts.Runtime.GameRule;
using Gas.Component;
using Gas.Variable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Runtime.PlayerCore
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(InputComponent))]
    public class PlayerController : Controller
    {
        public PlayerCharacter PlayerCharacter { get; private set; }
        public HUDCore HUD { get; private set; }

        public PlayerInput PlayerInput { get; private set; }
        public InputComponent InputComponent { get; private set; }
        public GameplayAbilityComponent GetAbilityComponent => PlayerCharacter?.GetGameplayAbilityComponent();

        public Action<float,float> OnHealthChangedEvent;
        public Action<float,float> OnMaxHealthChangedEvent;
        
        public Action<float,float> OnExperienceChangedEvent;
        public Action<float,float> OnMaxExperienceChangedEvent;

        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            InputComponent = GetComponent<InputComponent>();
        }

        public void OnInitialize()
        {
            GameInstance.RegisterPlayer(this);
        }
 
        public void PossesPlayer(PlayerCharacter newPlayerCharacter)
        {
            if (PlayerCharacter && !newPlayerCharacter)
            {
                UnPossesPlayer();
                return;
            }

            PlayerCharacter = newPlayerCharacter;
            PlayerCharacter.OnPossessed(this);

            FloatVariable health = PlayerCharacter.GetVariable(GlobalGameplayTags.Health);
            health.AddListener(OnHealthChanged);
            
            FloatVariable maxHealth = PlayerCharacter.GetVariable(GlobalGameplayTags.MaxHealth);
            maxHealth.AddListener(OnMaxHealthChanged);
            
            
            FloatVariable experience = PlayerCharacter.GetVariable(GlobalGameplayTags.Experience);
            experience.AddListener(OnExperienceChanged);
            
            FloatVariable maxExperience = PlayerCharacter.GetVariable(GlobalGameplayTags.MaxExperience);
            maxExperience.AddListener(OnMaxExperienceChanged);
        }

        public void UnPossesPlayer()
        {
            if (PlayerCharacter)
            {
                PlayerCharacter.UnPossessed();

                //to be verify not sure if needed function call : IsAlive()
                if (PlayerCharacter)
                {
                    FloatVariable health = PlayerCharacter.GetVariable(GlobalGameplayTags.Health);
                    health.RemoveListener(OnHealthChanged);
            
                    FloatVariable maxHealth = PlayerCharacter.GetVariable(GlobalGameplayTags.MaxHealth);
                    maxHealth.RemoveListener(OnMaxHealthChanged);    
                    
                    FloatVariable experience = PlayerCharacter.GetVariable(GlobalGameplayTags.Experience);
                    experience.RemoveListener(OnExperienceChanged);
            
                    FloatVariable maxExperience = PlayerCharacter.GetVariable(GlobalGameplayTags.MaxExperience);
                    maxExperience.RemoveListener(OnMaxExperienceChanged);
                }
                PlayerCharacter = null;
            }
        }

        public void SetHUD(HUDCore newHUDCore)
        {
            if (!newHUDCore)
            {
                Debug.Log("The hud is null", this);
                return;
            }

            if (HUD)
            {
                HUD.OnClear();
            }

            HUD = newHUDCore; 
            HUD.OnInitialize(this);
        }

        protected void OnHealthChanged(float oldHealth, float newHealth)
        {
            OnHealthChangedEvent?.Invoke(oldHealth,oldHealth);
        }
        
        protected void OnMaxHealthChanged(float oldHealth, float newHealth)
        {
            OnMaxHealthChangedEvent?.Invoke(oldHealth,oldHealth);
        }
        
        protected void OnExperienceChanged(float oldHealth, float newHealth)
        {
            OnExperienceChangedEvent?.Invoke(oldHealth,oldHealth);
        }
        
        protected void OnMaxExperienceChanged(float oldHealth, float newHealth)
        {
            OnMaxExperienceChangedEvent?.Invoke(oldHealth,oldHealth);
        }


        private void OnDestroy()
        {
            GameInstance.UnregisterPlayer(this);
        }
    }
}