#nullable enable

using Arch.Core;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.ProjectileSystem.Components;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.ProjectileSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileUpdateSystem))]
    public class ProjectileHitDetectionSystem : AbstractSystem
    {
        private static readonly QueryDescription _projectileQuery = new QueryDescription()
            .WithAll<Position, ProjectileState, Fraction>()
            .WithNone<Destroy>();

        private static readonly QueryDescription _targetQuery = new QueryDescription()
            .WithAll<Position, ProjectileCollider, Fraction>()
            .WithNone<Destroy, IsDeadTag>();

        protected override void OnUpdate()
        {
            using var _ = ListPool<ProjectileData>.Get(out var projectileData);
            World.Query(_projectileQuery,
                (Entity entity, ref Position position, ref ProjectileState projectileState, ref Fraction fraction) =>
                {
                    projectileData.Add(new ProjectileData
                    {
                        Entity = entity,
                        Position = position.Value,
                        ProjectileState = projectileState,
                        Fraction = fraction
                    });
                });

            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
                
            // TODO: Heavy operation, possible to optimize with burst
            World.Query(_targetQuery, 
                (Entity entity, ref Position position, ref ProjectileCollider projectileCollider, ref Fraction fraction) =>
                {
                    for (var i = projectileData.Count - 1; i >= 0; i--)
                    {
                        var projectileState = projectileData[i].ProjectileState;
                        var projectileFraction = projectileData[i].Fraction;
                        if ((projectileFraction.EnemiesMask & fraction.AlliesMask) == 0)
                        {
                            continue;
                        }
                        
                        var delta = position.Value - projectileData[i].Position;
                        delta.y = 0;
                        
                        if (delta.magnitude > projectileState.HitRadius + projectileCollider.Radius)
                        {
                            continue;
                        }

                        var hitEntity = Context.World.Create();
                        commandBuffer.Add(hitEntity, new ProjectileHit
                        {
                            ProjectileEntity = new EntityHandle(projectileData[i].Entity),
                            TargetEntity = new EntityHandle(entity)
                        });

                        if (projectileState.DestroyOnHit)
                        {
                            commandBuffer.Add(projectileData[i].Entity, new Destroy());
                            projectileData.RemoveAt(i);
                        }
                    }
                });
        }

        private struct ProjectileData
        {
            public Entity Entity;
            public Vector3 Position;
            public ProjectileState ProjectileState;
            public Fraction Fraction;
        }
    }
}