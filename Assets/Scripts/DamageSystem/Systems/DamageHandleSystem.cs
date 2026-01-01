#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.DamageSystem.Components;
using Game.ProjectileSystem.Components;
using UnityEngine;

namespace Game.DamageSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class DamageHandleSystem : AbstractSystem
    {
        private static readonly QueryDescription _projectileHitQuery = new QueryDescription()
            .WithAll<ProjectileHit>()
            .WithNone<Destroy>();

        protected override void OnUpdate()
        {
            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
                
            World.Query(_projectileHitQuery, 
                (Entity entity, ref ProjectileHit projectileHit) =>
                {
                    commandBuffer.Add(entity, new Destroy());

                    if (!projectileHit.ProjectileEntity.IsValid()
                        || !projectileHit.TargetEntity.IsValid())
                    {
                        return;
                    }
                    
                    var projectileEntity = projectileHit.ProjectileEntity.Value;
                    var targetEntity = projectileHit.TargetEntity.Value;

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
                    
                    healthState.Health = 0;
                    commandBuffer.Add(targetEntity, new Destroy());

                    // Try to spawn a coin
                    // TODO: Drop result can be managed by ResourceDropManager or set drop on spawn
                    if (targetEntity.TryGet<CoinSpawner>(out var coinSpawner)
                        && targetEntity.TryGet<Position>(out var position)
                        && Random.value < coinSpawner.Chance)
                    {
                        var coinEntity = Context.World.Create();
                        commandBuffer.Add(coinEntity, new Position { Value = position.Value });
                        commandBuffer.Add(coinEntity, new Rotation { Value = Quaternion.identity });
                        commandBuffer.Add(coinEntity, new PrefabId { Value = coinSpawner.CoinPrefabId});
                        commandBuffer.Add(coinEntity, new Coin());
                    }
                });
        }
    }
}