#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileUpdateSystem))]
    public class ProjectileHitDetectionSystem : AbstractSystem
    {
        private static readonly QueryDescription _projectileQuery = new QueryDescription()
            .WithAll<Position, Projectile>()
            .WithNone<Destroy>();

        private static readonly QueryDescription _enemyQuery = new QueryDescription()
            .WithAll<Position, HealthState, Enemy>()
            .WithNone<Destroy>();

        protected override void OnUpdate()
        {
            using var _ = ListPool<ProjectileData>.Get(out var projectileData);
            World.Query(_projectileQuery,
                (Entity entity, ref Position position, ref Projectile projectile) =>
                {
                    projectileData.Add(new ProjectileData
                    {
                        Entity = entity,
                        Position = position.Value,
                        Projectile = projectile
                    });
                });

            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
                
            // TODO: Heavy operation, possible to optimize with burst
            World.Query(_enemyQuery, 
                (Entity entity, ref Position position, ref HealthState healthState) =>
                {
                    for (var i = projectileData.Count - 1; i >= 0; i--)
                    {
                        var delta = position.Value - projectileData[i].Position;
                        delta.y = 0;
                        
                        if (delta.magnitude > projectileData[i].Projectile.HitDistance)
                        {
                            continue;
                        }
                 
                        // TODO: looks like a job for different system (in late simulation for example)
                        if (projectileData[i].Projectile.Damage <= 0)
                        {
                            continue;                            
                        }
                        
                        healthState.Health -= projectileData[i].Projectile.Damage;
                        healthState.LastHitTime = Context.Time;

                        commandBuffer.Add(projectileData[i].Entity, new Destroy());
                        projectileData.RemoveAt(i);
                        
                        if (healthState.Health > 0)
                        {
                            continue;
                        }
                        
                        healthState.Health = 0;
                        commandBuffer.Add(entity, new Destroy());

                        // Try to spawn a coin
                        if (entity.TryGet<CoinSpawner>(out var coinSpawner)
                            && Random.value < coinSpawner.Chance)
                        {
                            var coinEntity = Context.World.Create();
                            commandBuffer.Add(coinEntity, new Position { Value = position.Value });
                            commandBuffer.Add(coinEntity, new Rotation { Value = Quaternion.identity });
                            commandBuffer.Add(coinEntity, new PrefabId { Value = coinSpawner.CoinPrefabId});
                            commandBuffer.Add(coinEntity, new Coin());
                        }
                    }
                });
        }

        private struct ProjectileData
        {
            public Entity Entity;
            public Vector3 Position;
            public Projectile Projectile;
        }
    }
}