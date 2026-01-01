#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;

namespace Game.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ProjectileUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _projectileQuery = new QueryDescription()
            .WithAll<Position, Rotation, Projectile>();

        protected override void OnUpdate()
        {
            var deltaTime = Context.DeltaTime;
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            
            World.Query(_projectileQuery, 
                (Entity entity, ref Position position, ref Rotation rotation, ref Projectile projectile) =>
                {
                    position.Value += projectile.Direction * projectile.Speed * deltaTime;
                    projectile.PassedDistance += projectile.Speed * deltaTime;

                    if (projectile.PassedDistance >= projectile.MaxDistance)
                    {
                        commandBuffer.Add(entity, new Destroy());
                    }
                });
        }

    }
}