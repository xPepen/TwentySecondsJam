using Game.Scripts.Core.Utility;
using Game.Scripts.GAS.Ability;
using GameplayTags;
using Gas.Component;
using Gas.Variable;
using UnityEngine;

namespace Game.Scripts.Runtime.Weapon.Base
{
    [System.Serializable]
    public enum WeaponAttackBehaviour
    {
        OnPress,
        OnRelease,
        OnHold
    }

    public readonly struct WeaponStats
    {
        public readonly float Damage;
        public readonly float AttackRate;

        public WeaponStats(float damage, float attackRate)
        {
            Damage = damage;
            AttackRate = attackRate;
        }

        public bool IsValid()
        {
            return Damage > 0 && AttackRate > 0;
        }
    }

    //Note : weapon does not have rate, because it is handled by ability system\
    [RequireComponent(typeof(SphereCollider))]
    public abstract class WeaponBase : MonoBehaviour, IDamageCauser, ISocket
    {
        [SerializeField] private AbilityTemplate WeaponAbility;

        // [SerializeField]
        // WeaponAttackBehaviour

        [field: SerializeField] public FloatVariable DamageVariable { get; private set; }

        private GameplayAbilityComponent _gameplayAbilityComponent;
        private Component _owner;
        private GameplayTag _weaponAbilityTag;

        private Transform _socket;

        private SphereCollider _equipCollider;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _equipCollider = GetComponent<SphereCollider>();
            _equipCollider.isTrigger = true;
        }

        public void Init(Component owner, GameplayAbilityComponent gameplayAbilityComponent,
            WeaponStats weaponStats = default)
        {
            _owner = owner;
            _gameplayAbilityComponent = gameplayAbilityComponent;
            _weaponAbilityTag = WeaponAbility.GetAbilityTag();

            if (weaponStats.IsValid())
            {
                DamageVariable.Value = weaponStats.Damage;
            }
        }

        public void Attack()
        {
            if (CanAttack())
            {
                _gameplayAbilityComponent.PerformAbility(_weaponAbilityTag);
            }
        }

        public bool CanAttack()
        {
            return _gameplayAbilityComponent && _gameplayAbilityComponent.CanPerformAbility(_weaponAbilityTag);
        }

        public void OnAttach(Transform root)
        {
            _equipCollider.enabled = false;
            transform.parent = root;
            transform.localPosition = Vector3.zero;

            _spriteRenderer.enabled = false;
            if (!WeaponAbility)
            {
                Debug.Log("No Weapon Ability Assigned", this);
                return;
            }

            if (_gameplayAbilityComponent)
            {
                _gameplayAbilityComponent.AddAbility(WeaponAbility, _owner);
            }
        }

        public void OnDetach()
        {
            _equipCollider.enabled = true;
            _spriteRenderer.enabled = true;
            transform.parent = null;
            if (!WeaponAbility)
            {
                Debug.Log("No Weapon Ability Assigned", this);
                return;
            }

            if (_gameplayAbilityComponent)
            {
                _gameplayAbilityComponent.RemoveAbility(_weaponAbilityTag);
            }
        }

        public Transform GetSocketTransform(string socketName)
        {
            return _socket;
        }

        public void SetSocket(string socketName, Transform socketTransform)
        {
            _socket = socketTransform;
        }
    }
}