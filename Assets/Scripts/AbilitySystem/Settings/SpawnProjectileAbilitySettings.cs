#nullable enable

using System;
using Arch.Core;
using Game.AbilitySystem.Components;
using Game.Common;
using Game.PresentationSystem;
using Game.ProjectileSystem.Settings;
using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    [CreateAssetMenu(fileName = nameof(SpawnProjectileAbilitySettings), 
        menuName = "Assets/Abilities/Spawn Projectile Ability Settings")]
    [Serializable]
    public class SpawnProjectileAbilitySettings : AbstractAbilitySettings
    {
        public ProjectileSettings ProjectileSettings = null!;
        public SpawnDirectionType SpawnDirection;
        public int ProjectilesAmountPerActivation;
        
        public override void Prepare(InstancePool instancePool)
        {
            base.Prepare(instancePool);
            ProjectileSettings.Prepare(instancePool);
        }
        
        public override void Build(Entity entity, BuildContext context)
        {
            base.Build(entity, context);
            
            ProjectileSettings.Build(entity, context);

            context.CommandBuffer.Add(entity, new AbilitySpawnProjectile
            {
                SpawnDirection = SpawnDirection,
                ProjectilesAmount = ProjectilesAmountPerActivation,
            });
        }
    }
}