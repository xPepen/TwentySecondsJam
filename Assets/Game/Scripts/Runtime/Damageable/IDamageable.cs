using Game.Scripts.Core.Classes;

namespace Game.Scripts.Runtime.Damageable
{
    public interface IDamageable
    {
        public void ApplyDamage(Actor investigator, float damageAmount, DamageHandle damageHandle);
    }

    public static class DamageableExtensions
    {
        public static void ApplyDamage(this IDamageable damageable, Actor investigator, float damageAmount, DamageHandle damageHandle = default)
        {
            damageable.ApplyDamage(investigator, damageAmount, damageHandle);
        }
    }
}