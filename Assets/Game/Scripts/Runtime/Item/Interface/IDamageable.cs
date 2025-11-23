using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Runtime.Item.Interface
{
    public interface IDamageable
    {
        public void TakeDamage(GameObject Investigator,float damage, GameplayTag DamageType);
    }
}