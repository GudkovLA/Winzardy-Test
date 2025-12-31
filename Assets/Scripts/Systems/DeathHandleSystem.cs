#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;

namespace Game.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class DeathHandleSystem : AbstractSystem
    {
        private static readonly QueryDescription _unitsQuery = new QueryDescription()
            .WithAll<HealthState>()
            .WithNone<Destroy, IsDeadTag>();

        protected override void OnUpdate()
        {
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            
            World.Query(_unitsQuery,
                (Entity entity, ref HealthState healthState) =>
                {
                    if (healthState.Health > 0)
                    {
                        return;
                    }

                    healthState.Health = 0;
                    commandBuffer.Add(entity, new IsDeadTag());
                });
        }
    }
}