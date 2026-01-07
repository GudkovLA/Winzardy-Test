#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.ProjectileSystem;
using Game.ProjectileSystem.Components;
using UnityEngine;

namespace Game.DamageSystem.Systems
{
    [UpdateInGroup(typeof(DamageHandleSystemGroup))]
    public class DamageHandleSystem : AbstractSystem
    {
        private readonly QueryDescription _projectileHitQuery = new QueryDescription()
            .WithAll<ProjectileContact>()
            .WithNone<Destroy>();

        protected override void OnUpdate()
        {
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
                    commandBuffer.Add(targetEntity, new DamageHitTag());
                });
        }
    }
}