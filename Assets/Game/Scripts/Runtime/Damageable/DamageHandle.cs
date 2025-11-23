using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Runtime.Damageable
{
    [System.Serializable]
    public class DamageHandle
    {
        [field: SerializeField] public GameplayTag VariableAffected { get; private set; }
        [field: SerializeField] public GameplayTag GameplayEffectTag { get; private set; }

        [HideInInspector] public Vector3 HitLocation;

        public DamageHandle()
        {
        }

        public DamageHandle(GameplayTag variableAffected, GameplayTag gameplayEffectTag, Vector3 hitLocation)
        {
            this.VariableAffected = variableAffected;
            this.GameplayEffectTag = gameplayEffectTag;
            this.HitLocation = hitLocation;
        }
    }
}