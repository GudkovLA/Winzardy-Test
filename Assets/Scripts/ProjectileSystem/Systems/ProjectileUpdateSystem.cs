#nullable enable

using Arch.Core;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.LocomotionSystem.Components;
using Game.ProjectileSystem.Components;
using UnityEngine;

namespace Game.ProjectileSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ProjectileUpdateSystem : AbstractSystem
    {
        private readonly QueryDescription _projectileQuery = new QueryDescription()
            .WithAll<Position, ProjectileData, ProjectileState, LocomotionState>();

        protected override void OnUpdate()
        {
            var commandBuffer = GetOrCreateCommandBuffer();
            World.Query(_projectileQuery, 
                (Entity entity, 
                    ref Position position, 
                    ref ProjectileData projectileData,
                    ref ProjectileState projectileState, 
                    ref LocomotionState locomotionState) =>
                {
                    projectileState.PassedDistance += Vector3.Distance(position.Value, locomotionState.LastPosition);
                    if (projectileState.PassedDistance >= projectileData.MaxDistance)
                    {
                        commandBuffer.Add(entity, new Destroy());
                    }
                });
        }
    }
}