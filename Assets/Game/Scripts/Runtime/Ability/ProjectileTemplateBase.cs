using Game.Scripts.GAS.Ability;
using UnityEngine;

namespace Game.Scripts.Runtime.Ability
{
    public abstract class ProjectileTemplateBase : AbilityTemplate
    {
        [SerializeField] protected GameObject ProjectilePrefab;
        [SerializeField] protected float NumberOfProjectiles = 1;
    }
}