#nullable enable

using Arch.Buffer;
using Arch.Core;
using Game.Components;
using Game.DamageSystem.Components;
using Game.ProjectileSystem.Components;
using Game.ProjectileSystem.Settings;
using UnityEngine;

namespace Game.AbilitySystem.Abilities
{
    public class AbilityUtils
    {
        public static Entity SpawnProjectile(
            World world, 
            CommandBuffer commandBuffer, 
            ProjectileSettings projectileSettings,
            // TODO: Must be part of another (Locomotion?) component
            Vector3 direction)
        {
            var entity = world.Create();

            // TODO: Can be created by settings
            commandBuffer.Add(entity, new ProjectileState
            {
                Direction = direction,
                Speed = projectileSettings.Speed,
                MaxDistance = projectileSettings.MaxDistance,
                HitRadius = projectileSettings.HitRadius,
                DestroyOnHit = projectileSettings.DestroyOnHit
            });

            commandBuffer.Add(entity, new Damage
            {
                Amount = projectileSettings.Damage
            });

            if (projectileSettings.Prefab != null)
            {
                commandBuffer.Add(entity, new PrefabId { Value = projectileSettings.Prefab.GetInstanceID() });
            }
            
            return entity;
        }
    }
}