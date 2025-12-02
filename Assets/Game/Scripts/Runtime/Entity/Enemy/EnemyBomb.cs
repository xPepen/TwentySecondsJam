using GameplayTags;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Runtime.Entity.Enemy
{
    public class EnemyBomb : Core.Classes.Enemy
    {
        protected override void OnStart()
        {
            base.OnStart();
            // GetComponent<NavMeshAgent>().SetDestination(new Vector3());
            
        }

        private float Counter = 0;
        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (Animator)
            {
                Animator.SetFloat(Velocity, RigidBody.angularVelocity.sqrMagnitude, 0.01f, UnityEngine.Time.fixedDeltaTime);
            }

            Counter += deltaTime;
            if (Counter > 3)
            {
                GameplayAbilityComponent.PerformAbility(GameplayTag.RequestTag("Ability.Ratchet"));
                Counter = 0;
            }
        }

        protected override void OnHealthChanged(float oldValue, float newValue)
        {
            base.OnHealthChanged(oldValue, newValue);

            if (IsAlive())
            {
                Animator.SetTrigger(DamagedTrigger);
                
            }
            if(!IsAlive())
            {
                Animator.SetTrigger(AutoDestroyTrigger);
            }
        }
        

        private static readonly string DamagedTrigger = "Damaged";
        private static readonly string AutoDestroyTrigger = "AutoDestroy";
        private static readonly string Velocity = "Velocity";
    }
}