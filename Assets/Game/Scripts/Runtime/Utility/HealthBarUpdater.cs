using Game.Scripts.Core.HUD;
using Game.Scripts.Core.TimerManager;
using Game.Scripts.Runtime.GameRule;
using Game.Scripts.UI.Item;
using UnityEngine;

namespace Game.Scripts.Runtime.Utility
{
    //todo :add a canvas manager 
    //set the progress bar to the manager canvas
    //keep a ref on the health bar 
    //update the healthbar transform manually
    //when the owner die we could pool it

    //why canvas has a cost so one big canvas should be  better 
    //idea : canvas should follow the player so we dont need to render what off screen
    public class HealthBarUpdater : MonoBehaviour
    {
        // [SerializeField] private bool shouldLookAtCamera = true;
        // [SerializeField] private bool shouldIgnoreX = false;
        // [SerializeField] private bool shouldIgnoreY = false;
        // [SerializeField] private float ZOffSetPosition;

        private ProgressBar _progressBar;
        private Character _character;
        private Camera _camera;

        private float _health;
        private float _maxHealth;

        TimerHandle RendererStatusTimerHandle = new();
        Vector3 _progressBarPosition = new();

        private bool _isRendered;

        private void Awake()
        {
            _character = GetInternalComponent<Character>();
            _progressBar = GetInternalComponent<ProgressBar>();
            _character.OnStartEvent.AddListener(InitHealthBar);
            _camera = Camera.main;
        }

        private Camera GetCamera()
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }

            return _camera ? _camera : null;
        }


        private T GetInternalComponent<T>() where T : UnityEngine.Component
        {
            T component = this.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            component = GetComponentInChildren<T>();

            if (component != null)
            {
                return component;
            }

            component = GetComponentInParent<T>();
            if (component != null)
            {
                return component;
            }

            return null;
        }

        private void CheckRendererStatus()
        {
            var lastRenderedState = _isRendered;
            _isRendered = _character.IsRender(_camera);

            if (!_character)
            {
                return;
            }

            if (_progressBar)
            {
                if (!_progressBar.GetParent().activeSelf && _isRendered)
                {
                    if (_progressBar)
                    {
                        Vector3 pos = _character.transform.position;
                        _progressBarPosition.Set(pos.x, pos.y + _character.GetHeight() * 0.8f, pos.z);

                        var transformParent = _progressBar.GetParent().transform;
                        transformParent.position = _progressBarPosition;
                        transformParent.LookAt(_camera.transform, Vector3.up);
                    }
                }

                _progressBar.GetParent().SetActive(_isRendered);

                if (!lastRenderedState && _isRendered)
                {
                    _progressBar.SetProgress(_health / _maxHealth);
                }
            }
        }

        private void LateUpdate()
        {
            if (!_character)
            {
                return;
            }

            if (!_character.IsRender(_camera))
            {
                return;
            }

            if (_progressBar)
            {
                Vector3 pos = _character.transform.position;


                _progressBarPosition.Set(pos.x, pos.y + _character.GetHeight() * 0.8f, pos.z /*+ ZOffSetPosition*/);

                var transformParent = _progressBar.GetParent().transform;

                transformParent.position = _progressBarPosition;
                
                Camera cam = GetCamera();
                if (cam)
                {
                    transformParent.LookAt(_camera.transform, Vector3.up);
                }
            }
        }

        private void InitHealthBar()
        {
            //health binding 
            var health = _character.GameplayAbilityComponent.GetVariable(GlobalGameplayTags.Health);
            _health = health.Value;
            health.AddListener(SetHealth);

            //max health binding
            var maxHealth = _character.GameplayAbilityComponent.GetVariable(GlobalGameplayTags.MaxHealth);

            _maxHealth = maxHealth.Value;
            maxHealth.AddListener(SetMaxHealth);

            if (!_progressBar)
            {
                _progressBar = GetInternalComponent<ProgressBar>();
            }

            _progressBar.SetProgress(_health / _maxHealth);

            Subsystem.Get<CanvasWorldSubsystem>().AddChild(_progressBar.GetParent());
            TimerManager.SetTimer(RendererStatusTimerHandle, CheckRendererStatus, 0.25f, true);
        }

        void SetHealth(float oldValue, float newValue)
        {
            if (!_progressBar)
            {
                return;
            }

            if (newValue <= 0)
            {
                TimerManager.ClearTimer(RendererStatusTimerHandle);
                Subsystem.Get<CanvasWorldSubsystem>().RemoveChild(_progressBar.GetParent());

                //todo : later will be pooled in a global pool system, maybe inside world space canvas would make sense
                Destroy(_progressBar.GetParent(), 0.25f);
                return;
            }

            _health = newValue;
            UpdateHealthBar();
        }

        void SetMaxHealth(float oldValue, float newValue)
        {
            _maxHealth = newValue;
            UpdateHealthBar();
        }


        private void UpdateHealthBar()
        {
            if (_progressBar)
            {
                _progressBar.SetProgress(_health / _maxHealth);
            }
        }
    }
}