#nullable enable

using System;
using Game.AbilitySystem.Abilities;
using Game.ProjectileSystem.Settings;
using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    [CreateAssetMenu(fileName = nameof(AreaProjectileAbilitySettings), 
        menuName = "Assets/Abilities/Area Projectile Ability Settings")]
    [Serializable]
    public class AreaProjectileAbilitySettings : AbstractAbilitySettings
    {
        public ProjectileSettings ProjectileSettings;

        public override IAbility Instantiate()
        {
            return new AreaProjectileAbility(this);
        }
    }
}