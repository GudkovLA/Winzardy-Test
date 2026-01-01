#nullable enable

using Arch.Core;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.ProjectileSystem.Components;

namespace Game.ProjectileSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ProjectileUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _projectileQuery = new QueryDescription()
            .WithAll<Position, Rotation, ProjectileState>();

        protected override void OnUpdate()
        {
            var deltaTime = Context.DeltaTime;
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            
            World.Query(_projectileQuery, 
                (Entity entity, ref Position position, ref Rotation rotation, ref ProjectileState projectileState) =>
                {
                    position.Value += projectileState.Direction * projectileState.Speed * deltaTime;
                    projectileState.PassedDistance += projectileState.Speed * deltaTime;

                    if (projectileState.PassedDistance >= projectileState.MaxDistance)
                    {
                        commandBuffer.Add(entity, new Destroy());
                    }
                });
        }

    }
}