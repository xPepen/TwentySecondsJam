using UnityEngine;

namespace Game.Scripts.Runtime.Weapon.Base
{
    public enum AttackType
    {
        None,
        Melee,
        Ranged,
        Magic
    }
    
    public interface IDamageCauser
    {
        public void Attack();
        public bool CanAttack();
    }
}