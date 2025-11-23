using Game.Scripts.Core.Pool;
using Game.Scripts.Core.TimerManager;
using Game.Scripts.GAS.Ability;
using Game.Scripts.Runtime.SpellAbility;
using Game.Scripts.Runtime.Weapon.Base;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Runtime.Ability
{
    [CreateAssetMenu(menuName = "GameplayAbility/RatchetAbilityTemplate", fileName = "RatchetAbilityTemplate")]
    public class RatchetAbilityTemplate : ProjectileTemplateBase
    {
        protected override void SetInternalAbilityTag()
        {
            abilityTag = GameplayTag.CreateTag("Ability.Ratchet");
        }

        public override void OnAbilityStart(AbilityBase ability)
        {
            if (ProjectilePrefab)
            {
                RateAbility RateAbility = ability as RateAbility;

                Character actor = ability.GetOwner<Character>();
                float rot = 0;
                int numberOfProjectiles = 0;
                int maxProjectiles = 3;

                void ShootRatchet()
                {
                    Vector3 pos = actor.transform.position;
                    var position = new Vector3(pos.x, pos.y + actor.GetHeight() * 0.5f, pos.z);

                    if (actor.GameplayAbilityComponent)
                    {
                        Vector3 feedbackPos = pos;
                        IWeaponCarried IWeaponCarried = ability.GetOwner<IWeaponCarried>();
                        if (IWeaponCarried != null)
                        {
                            feedbackPos = IWeaponCarried.GetWeapon()
                                .GetSocketTransform("").position;
                        }

                        actor.GameplayAbilityComponent.ApplyFeedBack(GameplayTag.RequestTag("Action.Shoot.Default"),
                            feedbackPos);
                    }

                    ProjectileBase projectile = Subsystem.Get<PoolMaster>()
                        .GetItemFromPoolAsComponent<ProjectileBase>(GameplayTag.RequestTag("Object.Ratchet"),
                            position + actor.transform.transform.forward, Quaternion.identity);


                    if (projectile)
                    {
                        rot += 40.0f;
                        Vector3 dir = actor.transform.forward + Quaternion.Euler(0, rot, 0) * Vector3.right * 0.1f;
                        projectile.Init(actor).Launch(dir);
                    }

                    numberOfProjectiles++;

                    if (numberOfProjectiles >= maxProjectiles)
                    {
                        TimerManager.ClearTimer(RateAbility.GetRateTimerIntervalHandle());
                        ability.EndAbility();
                    }
                }

                //trust me bro
                TimerManager.SetTimer(RateAbility!.GetRateTimerIntervalHandle(), ShootRatchet, 0.2f, true);
            }
        }
    }
}