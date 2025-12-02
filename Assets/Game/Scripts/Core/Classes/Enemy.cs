using System.Collections;
using Game.Scripts.Core.Pool;
using Game.Scripts.Runtime.Ability.Class;
using Game.Scripts.Runtime.Damageable;
using Game.Scripts.Runtime.Entity;
using GameplayTags;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core.Classes
{
    [System.Serializable]
    public struct ExperienceReward
    {
        [field: SerializeField] public GameObject ExperienceOrbPrefab { get; private set; }
        [field: SerializeField] public int ExperienceAmount { get; private set; }
        [field: SerializeField] public int MinNumOfOrbs { get; private set; }
        [field: SerializeField] public int MaxNumOfOrbs { get; private set; }
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class Enemy : Character
    {
        public StateMachine.StateMachine StateMachine { get; private set; }
        
        [SerializeField] private ExperienceReward ExperienceReward;
        public Rigidbody RigidBody { get; private set; }
        public NavMeshAgent NavMeshAgent { get; private set; }

        private Collider _collider;

        private Coroutine _onSpawnExperienceCoroutine;

        private GameObject KilledBy;

        bool HasDropExperienceYet = false;

        protected override void OnAwake()
        {
            base.OnAwake();
            
            _collider = GetComponent<Collider>();
            RigidBody = GetComponent<Rigidbody>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            StateMachine = GetComponent<StateMachine.StateMachine>();
        }

        protected override void OnStart()
        {
            //we need to initialize before we start any logic that might depend on it
            OnInitialize();
            base.OnStart();
            StateMachine = GetComponent<StateMachine.StateMachine>();
            
            StateMachine.Init(this);
        }

        public override float GetHeight()
        {
            return _collider.bounds.size.y;
        }

        public override void OnDeadFinish()
        {
            if (transform.parent)
            {
                if (!HasDropExperienceYet)
                {
                    StartCoroutine(OnSpawnExperience());
                    HasDropExperienceYet = true;
                }


                Destroy(transform.parent.gameObject, 2f);
                return;
            }

            Destroy(gameObject);
        }


        public override void ApplyDamage(Actor investigator, float damageAmount, DamageHandle damageHandle)
        {
            base.ApplyDamage(investigator, damageAmount, damageHandle);

            if (IsDead())
            {
                KilledBy = investigator.gameObject;
                _collider.enabled = false;
                RigidBody.isKinematic = true;
                RigidBody.useGravity = false;
            }
        }

        protected virtual IEnumerator OnSpawnExperience()
        {
            int numOfOrbs = Random.Range(ExperienceReward.MinNumOfOrbs, ExperienceReward.MaxNumOfOrbs);

            //Get attractor
            AbilityAttractorComponent abilityAttractor = KilledBy.gameObject.GetComponent<AbilityAttractorComponent>();

            PoolMaster poolContainer = Subsystem.Get<PoolMaster>();

            for (int i = 0; i < numOfOrbs; i++)
            {
                var pos = transform.position;
                pos.y += (GetHeight() * 0.5f);

                // Wait before spawning the next orb
                yield return new WaitForSeconds(0.05f);

                Experience experienceRef =
                    poolContainer.GetItemFromPoolAsComponent<Experience>(GameplayTag.RequestTag("Object.Experience"),
                        pos, Quaternion.identity);

                if (KilledBy)
                {
                    if (abilityAttractor)
                    {
                        // abilityAttractor.AddAttractedBody(experienceRef.Rigidbody);

                        void OnOrbTaken(GameObject orb)
                        {
                            // typeof(Experience).Name;
                            // abilityAttractor.RemoveAttractedBody(experienceRef.Rigidbody);
                            poolContainer.PoolEvent_ReleaseItem(GameplayTag.RequestTag("Object.Experience"))
                                .RemoveListener(OnOrbTaken);
                        }

                        poolContainer.PoolEvent_ReleaseItem(GameplayTag.RequestTag("Object.Experience"))
                            .AddListener(OnOrbTaken);
                    }


                    if (experienceRef)
                    {
                        float angle = (360f / numOfOrbs) * i;
                        Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

                        experienceRef.Launch(dir, 3f, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}