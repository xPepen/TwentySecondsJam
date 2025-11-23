using System;
using Game.Scripts.Core.Classes;
using Game.Scripts.Core.Pool;
using Game.Scripts.Core.TimerManager;
using GameplayTags;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Runtime.Entity
{
    [RequireComponent(typeof(Rigidbody))]
    public class Experience : Actor
    {
        [SerializeField] private float DefaultExperienceAmount = 10f;

        [Header("Experience Randomization")] [SerializeField]
        private bool IsExperienceRandomize;

        [SerializeField] private float AmountRange = 10f;

        private Rigidbody _rigidbody;

        TimerHandle _IsGroundedHandle = new();

        protected override void OnAwake()
        {
            base.OnAwake();
            if (!GetComponent<Collider>())
            {
                Debug.LogError($"{nameof(Experience)} on {gameObject.name} must have a Collider", this);
            }

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = true;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            TimerManager.SetTimer(_IsGroundedHandle, HasReachedGround, 0.3f, true);
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.WakeUp();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TimerManager.ClearTimer(_IsGroundedHandle);
        }


        public void Launch(Vector3 dir, float launchForce = 5f, ForceMode forceMode = ForceMode.Impulse)
        {
            if (!_rigidbody)
            {
                Debug.LogError($"{nameof(Experience)} on {gameObject.name} must have a Rigidbody", this);
                return;
            }

            var force = dir.normalized * launchForce;
            _rigidbody.AddForce(force, forceMode);
        }


        protected override void OnTriggerEnterVirtual(Collider other)
        {
            base.OnTriggerEnterVirtual(other);

            if (other.gameObject.IsA<PlayerCharacter>(out var player))
            {
                float finalAmount = DefaultExperienceAmount;

                if (IsExperienceRandomize)
                {
                    //we don't want to remove experience so we put 0 as min
                    finalAmount += Random.Range(1, AmountRange);
                }

                player.AddExperience(finalAmount);
                base.OnTriggerEnterVirtual(other);

                Subsystem.Get<PoolMaster>().ReleaseItemToPool(GameplayTag.RequestTag("Object.Experience"), gameObject);
            }
        }

        protected virtual void HasReachedGround()
        {
            if (_rigidbody.linearVelocity.magnitude < 0.1f)
            {
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
                _rigidbody.Sleep();
            }

            TimerManager.ClearTimer(_IsGroundedHandle);
        }
    }
}