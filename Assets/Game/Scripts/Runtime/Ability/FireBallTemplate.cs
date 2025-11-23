using System;
using Game.Scripts.Core.Classes;
using Game.Scripts.Core.Pool;
using Game.Scripts.Core.TimerManager;
using Game.Scripts.Core.Utility;
using Game.Scripts.GAS.Ability;
using Game.Scripts.Runtime.SpellAbility;
using Game.Scripts.Runtime.Weapon.Base;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Runtime.Ability
{
    [CreateAssetMenu(menuName = "GameplayAbility/FireBallAbility", fileName = "FireBallAbility")]
    public class FireBallTemplate : ProjectileTemplateBase
    {
        protected override void SetInternalAbilityTag()
        {
            abilityTag = GameplayTag.CreateTag("Ability.FireBall");
        }

        public override void OnAbilityStart(AbilityBase ability)
        {
            if (ProjectilePrefab)
            {
                Character actor = ability.GetOwner<Character>();
                float rot = 0;

                for (int i = 0; i < NumberOfProjectiles; i++)
                {
                    Action SpawnObject = () =>
                    {
                        Vector3 pos = actor.transform.position;
                        var position = new Vector3(pos.x, pos.y + actor.GetHeight(), pos.z);

                        if (actor.GameplayAbilityComponent)
                        {
                            Vector3 feedbackPos = ability.GetOwner<IWeaponCarried>().GetWeapon()
                                .GetSocketTransform("").position;
                            actor.GameplayAbilityComponent.ApplyFeedBack(GameplayTag.RequestTag("Action.Shoot.Default"),
                                feedbackPos);
                        }

                        ProjectileBase projectile = Subsystem.Get<PoolMaster>()
                            .GetItemFromPoolAsComponent<ProjectileBase>(GameplayTag.RequestTag("Object.Ratchet"),
                                position + actor.transform.transform.forward, Quaternion.identity);


                        if (projectile)
                        {
                            rot += 40;
                            var dir = actor.transform.forward + Quaternion.Euler(0, rot, 0) * Vector3.right * 0.1f;
                            projectile.Init(actor).Launch(dir);
                        }
                    };


                    TimerHandle x = new();
                    TimerManager.SetTimer(x, SpawnObject, i * 0.2f, false);
                }

                ability.EndAbility();
            }
        }
    }
}