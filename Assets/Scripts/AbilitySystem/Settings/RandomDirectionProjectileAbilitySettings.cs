#nullable enable

using System;
using Game.AbilitySystem.Abilities;
using Game.ProjectileSystem.Settings;
using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    [CreateAssetMenu(fileName = nameof(RandomDirectionProjectileAbilitySettings), 
        menuName = "Assets/Abilities/Random Direction Projectile Ability Settings")]
    [Serializable]
    public class RandomDirectionProjectileAbilitySettings : AbstractAbilitySettings
    {
        public ProjectileSettings ProjectileSettings;
        public int ProjectilesAmountPerActivation;

        public override IAbility Instantiate()
        {
            return new RandomDirectionProjectileAbility(this);
        }
    }
}