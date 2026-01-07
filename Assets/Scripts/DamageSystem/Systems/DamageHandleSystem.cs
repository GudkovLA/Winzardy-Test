#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.PresentationSystem.Components;
using Game.ProjectileSystem;
using Game.ProjectileSystem.Components;
using Game.ResourceSystem;
using Game.ResourceSystem.Components;
using UnityEngine;

namespace Game.DamageSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class DamageHandleSystem : AbstractSystem
    {
        private readonly QueryDescription _projectileHitQuery = new QueryDescription()
            .WithAll<ProjectileContact>()
            .WithNone<Destroy>();
        
        private ResourcesRegistry _resourcesRegistry = null!;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _resourcesRegistry))
            {
                return;
            }

            _initialized = true;
        }
        
        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }

            var commandBuffer = GetOrCreateCommandBuffer(); 
            World.Query(_projectileHitQuery, 
                (Entity entity, ref ProjectileContact projectileContact) =>
                {
                    if (projectileContact.ContactPhase == ProjectileContactPhase.Finish)
                    {
                        commandBuffer.Add(entity, new Destroy());
                        return;
                    }

                    if (!projectileContact.ProjectileEntity.IsValid()
                        || !projectileContact.TargetEntity.IsValid())
                    {
                        commandBuffer.Add(entity, new Destroy());
                        return;
                    }

                    // Handle damage only for start contact phase
                    if (projectileContact.ContactPhase != ProjectileContactPhase.Start)
                    {
                        return;
                    }

                    var projectileEntity = projectileContact.ProjectileEntity.Value;
                    var targetEntity = projectileContact.TargetEntity.Value;

                    if (!projectileEntity.TryGet<Damage>(out var damage))
                    {
                        Debug.LogError($"Can't find required component in projectile entity (ComponentType={nameof(Damage)})");
                        return;
                    }
                    
                    if (!targetEntity.TryGet<HealthState>(out var healthState))
                    {
                        Debug.LogError($"Can't find required component in target entity (ComponentType={nameof(HealthState)})");
                        return;
                    }

                    healthState.Health -= damage.Amount;
                    healthState.LastHitTime = Context.Time;
                    commandBuffer.Set(targetEntity, healthState);

                    if (healthState.Health > 0)
                    {
                        return;
                    }
                    
                    // TODO: Spawn must be a part of ResourceSystem
                    if (targetEntity.TryGet<ResourceSpawner>(out var coinSpawner)
                        && targetEntity.TryGet<Position>(out var position))
                    {
                        var resourceSettings = _resourcesRegistry.GetResource(coinSpawner.ResourceId);
                        if (resourceSettings != null
                            && resourceSettings.Prefab != null)
                        {
                            var coinEntity = Context.World.Create();
                            commandBuffer.Add(coinEntity, new Position { Value = position.Value });
                            commandBuffer.Add(coinEntity, new Rotation { Value = Quaternion.identity });
                            commandBuffer.Add(coinEntity, new Resource { ResourceId = coinSpawner.ResourceId });
                        
                            commandBuffer.Add(coinEntity,
                                new PrefabId { Value = resourceSettings.Prefab.GetInstanceID() });
                        }
                    }
                });
        }
    }
}