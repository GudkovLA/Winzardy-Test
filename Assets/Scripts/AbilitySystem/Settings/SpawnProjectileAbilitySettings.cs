#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem.Components;
using Game.ProjectileSystem.Settings;
using Game.Utils;
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
        
        public override void Initialize(CommandBuffer commandBuffer, Entity entity)
        {
            base.Initialize(commandBuffer, entity);
            
            ProjectileSettings.Initialize(commandBuffer, entity);

            commandBuffer.Add(entity, new AbilitySpawnProjectile
            {
                SpawnDirection = SpawnDirection,
                ProjectilesAmount = ProjectilesAmountPerActivation,
            });
        }
    }
}