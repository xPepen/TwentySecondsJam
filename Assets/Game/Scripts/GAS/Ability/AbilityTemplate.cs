using System.Collections;
using System.Collections.Generic;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.GAS.Ability
{
    public enum AbilityType
    {
        None,
        Instant, /* One frame trip triggering is instant*/
        Channeled, /* Mean we stop performing the attack when no input is trigger?*/
        Rate, /* like attack speed, health regen, dash, etc... */
        Passive, /* will be active on a specific rate for a duration x secs*/
        PassiveInfinite /* will loop infinitely with a rate */
    }
    
    public enum PassiveType
    {
        Duration,
        Infinite,
    }

    [System.Serializable]
    //this suck it should ne a scriptable object 
    public class AbilityTemplateSet : IEnumerable<AbilityTemplate>
    {
        public List<AbilityTemplate> AbilitiesSet = new ();

        public AbilityTemplateSet() { }

        public AbilityTemplateSet(List<AbilityTemplate> newAbilitiesSet)
        {
            AbilitiesSet = newAbilitiesSet ?? new();
        }
        
        public IEnumerator<AbilityTemplate> GetEnumerator() => AbilitiesSet.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // [CreateAssetMenu(menuName = "GameplayAbility/Ability", fileName = "NewAbilityTemplate")]
    public abstract class AbilityTemplate : ScriptableObject
    {
        [field: SerializeField] public AbilityType AbilityType { get; private set; }
        [field: SerializeField] public float DefaultRate { get; protected set; }
        [field: SerializeField] protected GameplayTag abilityTag;
        [field: SerializeField] public bool ShowAbilityLog { get; private set; }


        public virtual GameplayTag GetAbilityTag()
        {
            if (abilityTag == null || abilityTag.GetID() == -1)
            {
                 SetInternalAbilityTag();
            }
            
            return abilityTag;
        }

        protected abstract void SetInternalAbilityTag(); 
        
        public abstract void OnAbilityStart(AbilityBase ability);
    }
}