#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.CharacterSystem.Components;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;

namespace Game.DamageSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(DamageHandleSystem))]
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

                    if (entity.Has<DontDestroyOnDeath>())
                    {
                        commandBuffer.Add(entity, new IsDeadTag());
                    }
                    else
                    {
                        commandBuffer.Add(entity, new Destroy());
                    }
                });
        }
    }
}