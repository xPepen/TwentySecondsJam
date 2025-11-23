using GameplayTags;
using UnityEngine;
using UnityEngine.Events;

namespace Gas.Variable
{
//This should be use as a type for any variable that is use in gameplay while on a entity
    [System.Serializable]
    public class FloatVariable
    {
        //tood : allow OnValueChangedEvent?.Invoke(oldValue, _value); on initialization ? 
        [field: SerializeField] private GameplayTag _gameplayTag;
        [field: SerializeField] private float _value;

        [SerializeField, HideInInspector] private float _defaultValue;

        /* float 0 = Old Value, float 1 = NewValue*/
        [field: SerializeField] private UnityEvent<float, float> OnValueChangedEvent = new UnityEvent<float, float>();


        public FloatVariable(GameplayTag gameplayTag)
        {
            _value = 1f;
            _defaultValue = 1f;
            _gameplayTag = gameplayTag;
        }

        public FloatVariable(GameplayTag gameplayTag, float value)
        {
            _value = value;
            _defaultValue = value;
            _gameplayTag = gameplayTag;
        }

        public float Value
        {
            get => _value;
            set
            {
                if (Mathf.Approximately(_value, value))
                    return;

                float oldValue = _value;
                _value = value;

                OnValueChangedEvent?.Invoke(oldValue, _value);
            }
        }

        public GameplayTag Tag
        {
            get => _gameplayTag;

            private set
            {
                if (!GameplayTag.IsValid(value))
                {
                    Debug.LogError($"Tag {value} does not exist");
                    return;
                }

                _gameplayTag = value;
            }
        }

        private void OnValidate()
        {
            Debug.Log("Hello");
        }

        public void AddListener(UnityAction<float, float> listener)
        {
            OnValueChangedEvent.AddListener(listener);
        }

        // Remove listener
        public void RemoveListener(UnityAction<float, float> listener)
        {
            OnValueChangedEvent.RemoveListener(listener);
        }

        public static implicit operator float(FloatVariable variable) => variable._value;

        public void Reset()
        {
            float oldValue = _value;
            Value = _defaultValue;
            OnValueChangedEvent?.Invoke(oldValue, _defaultValue);
        }
    }
}