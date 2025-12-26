#nullable enable

using Arch.Core;
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
            .WithAll<Position, Enemy>()
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
                (Entity entity, ref Position position) =>
                {
                    for (var i = projectileData.Count - 1; i >= 0; i--)
                    {
                        var delta = position.Value - projectileData[i].Position;
                        delta.y = 0;
                        
                        if (delta.magnitude > projectileData[i].Projectile.HitDistance)
                        {
                            continue;
                        }

                        commandBuffer.Add(projectileData[i].Entity, new Destroy());
                        commandBuffer.Add(entity, new Destroy());
                        commandBuffer.Add(projectileData[i].Entity, new Destroy());
                        commandBuffer.Add(entity, new Destroy());
                        projectileData.RemoveAt(i);
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