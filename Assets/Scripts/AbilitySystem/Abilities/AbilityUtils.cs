#nullable enable

using Arch.Buffer;
using Arch.Core;
using Game.Components;
using Game.DamageSystem.Components;
using Game.LocomotionSystem.Components;
using Game.ProjectileSystem.Components;
using Game.ProjectileSystem.Settings;

namespace Game.AbilitySystem.Abilities
{
    public class AbilityUtils
    {
        public static Entity SpawnProjectile(
            World world, 
            CommandBuffer commandBuffer, 
            ProjectileSettings projectileSettings)
        {
            var entity = world.Create();

            // TODO: Can be created by settings
            commandBuffer.Add(entity, new ProjectileState
            {
                MaxDistance = projectileSettings.MaxDistance,
                HitRadius = projectileSettings.HitRadius,
                DestroyOnHit = projectileSettings.DestroyOnHit
            });

            commandBuffer.Add(entity, new Damage
            {
                Amount = projectileSettings.Damage
            });

            commandBuffer.Add(entity, new IgnoreRotationTag());

            if (projectileSettings.Prefab != null)
            {
                commandBuffer.Add(entity, new PrefabId { Value = projectileSettings.Prefab.GetInstanceID() });
            }
            
            return entity;
        }
    }
}