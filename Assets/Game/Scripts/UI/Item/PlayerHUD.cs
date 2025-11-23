using Game.Scripts.Core.Classes;
using Game.Scripts.Core.HUD;
using Game.Scripts.Runtime.GameRule;
using Game.Scripts.Runtime.PlayerCore;
using GameplayTags;
using Gas.Component;
using UnityEngine;

namespace Game.Scripts.UI.Item
{
    public class PlayerHUD : MonoBehaviour, IUserInterfaceWindow
    {
        [SerializeField] private ProgressBar HealthBar;
        [SerializeField] private ProgressBar ExperienceBar;
        private PlayerController _playerController;
        private PlayerCharacter _PlayerCharacter;

        private float _health;
        private float _maxHealth;
        private float _experience;
        private float _maxExperience;

        public void Init(PlayerController pc)
        {
            _playerController = pc;
            _PlayerCharacter = _playerController.PlayerCharacter;

            if (_playerController)
            {
                _playerController.OnHealthChangedEvent += SetHealth;
                _playerController.OnMaxHealthChangedEvent += SetMaxHealth;

                _playerController.OnExperienceChangedEvent += SetExperience;
                _playerController.OnMaxExperienceChangedEvent += SetMaxExperience;
            }

            if (!_PlayerCharacter)
            {
                Debug.LogError("PlayerCharacter object is null, can't initialize health bar...");
                return;
            }

            GameplayAbilityComponent abilityComponent = _PlayerCharacter.GameplayAbilityComponent;

            if (abilityComponent)
            {
                var health = abilityComponent.GetVariable(GlobalGameplayTags.Health);
                _health = health.Value;

                var maxHealth = abilityComponent.GetVariable(GlobalGameplayTags.MaxHealth);
                _maxHealth = maxHealth.Value;
                
                var experience = abilityComponent.GetVariable(GlobalGameplayTags.Experience);
                _experience = experience.Value;

                var maxExperience = abilityComponent.GetVariable(GlobalGameplayTags.MaxExperience);
                _maxExperience = maxExperience.Value;
                
                
                UpdateProgressBar(HealthBar,_health / _maxHealth);
                UpdateProgressBar(ExperienceBar,_experience / _maxExperience);
            }
        }

        void SetExperience(float oldValue, float newValue)
        {
            _experience = newValue;
            UpdateProgressBar(ExperienceBar, _experience / _maxExperience);
        }

        void SetMaxExperience(float oldValue, float newValue)
        {
            _maxExperience = newValue;
            UpdateProgressBar(ExperienceBar, _experience / _maxExperience);
        }

        void SetHealth(float oldValue, float newValue)
        {
            if (!HealthBar)
            {
                return;
            }

            if (newValue <= 0)
            {
                transform.parent = null;
                Destroy(HealthBar.GetParent(), 3f);
                return;
            }

            _health = newValue;
            UpdateProgressBar(HealthBar, _health / _maxHealth);
        }

        void SetMaxHealth(float oldValue, float newValue)
        {
            _maxHealth = newValue;
            UpdateProgressBar(HealthBar, _health / _maxHealth);
        }


        private void UpdateProgressBar(ProgressBar progressBar, float progress)
        {
            if (progressBar)
            {
                float value = Mathf.Clamp01(progress);
                progressBar.SetProgress(value);
            }
        }

        public void OnPushToStack()
        {
            // print("Push to stack");
            HealthBar.enabled = true;
        }

        public void OnPopFromStack()
        {
            print("Pop from stack");
        }

        public bool ShouldAlwaysBeVisible()
        {
            return true;
        }

        private void OnDestroy()
        {
            if (_playerController)
            {
                _playerController.OnHealthChangedEvent -= SetHealth;
                _playerController.OnMaxHealthChangedEvent -= SetMaxHealth;
            }
        }
    }
}