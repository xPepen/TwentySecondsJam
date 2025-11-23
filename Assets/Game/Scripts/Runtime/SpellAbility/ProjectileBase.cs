using Game.Scripts.Core.Classes;
using Game.Scripts.Core.Pool;
using Game.Scripts.Runtime.Damageable;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Runtime.SpellAbility
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBase : Actor
    {
        [SerializeField] private float Damage;
        [SerializeField] private float MovementSpeed;
        [SerializeField] private DamageHandle DamageHandle;

        private Actor _owner;

        //todo : we should have a physical actorclass with base physic core stuff
        public Rigidbody Rigidbody { get; private set; }

        private float _angle;

        protected override void OnAwake()
        {
            base.OnAwake();
            Rigidbody = GetComponent<Rigidbody>();
        }

        public ProjectileBase Init(Actor owner)
        {
            _owner = owner;
            return this;
        }

        public void Launch(Vector3 direction)
        {
            Vector3 finalMovementSpeed = direction.normalized * MovementSpeed;
            Rigidbody.linearVelocity = finalMovementSpeed;

            Rigidbody.AddForce(finalMovementSpeed, ForceMode.VelocityChange);

            transform.rotation = Quaternion.LookRotation(finalMovementSpeed, Vector3.forward);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Rigidbody.WakeUp();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Rigidbody.Sleep();
        }

        protected void Animation(float deltaTime)
        {
            if (enabled)
            {
                transform.Rotate(Vector3.up, 500f * deltaTime, Space.Self);
            }
        }

        protected override void OnTriggerEnterVirtual(Collider other)
        {
            if (other.gameObject == _owner.gameObject)
            {
                //we don't want to damage ourselves
                return;
            }

            if (!_owner)
            {
                print("No owner assigned to projectile !");
                return;
            }

            if (other.gameObject)
            {
                if (other.gameObject.IsA<IDamageable>(out var damageableComponent))
                {
                    float multiplier = 1.0f;
                    Character character = _owner.GetComponent<Character>();

                    if (character)
                    {
                        multiplier = character.GetDamageMultiplier().Value;
                    }

                    DamageHandle.HitLocation = other.transform.position;

                    damageableComponent.ApplyDamage(_owner, Damage * multiplier, DamageHandle);
                    base.OnTriggerEnterVirtual(other);
                }
            }

            Subsystem.Get<PoolMaster>().ReleaseItemToPool(GameplayTag.RequestTag("Object.Ratchet"), gameObject);
        }
    }
}