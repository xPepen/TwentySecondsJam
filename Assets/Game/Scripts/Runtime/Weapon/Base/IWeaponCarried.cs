namespace Game.Scripts.Runtime.Weapon.Base
{
    public interface IWeaponCarried
    {
        
        //later on this will not exist and be replace by a inventory system
        public WeaponBase GetWeapon();
        public void SetWeapon(WeaponBase newWeapon);
    }
}