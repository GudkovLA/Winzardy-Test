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
    [UpdateInGroup(typeof(DamageHandleSystemGroup))]
    [UpdateAfter(typeof(DamageHandleSystem))]
    public class DeathHandleSystem : AbstractSystem
    {
        private readonly QueryDescription _unitsQuery = new QueryDescription()
            .WithAll<HealthState, DamageHitTag>()
            .WithNone<Destroy, DeathState>();

        private readonly QueryDescription _deadUnitsQuery = new QueryDescription()
            .WithAll<DeathState>()
            .WithNone<Destroy, PlayerTag>();

        protected override void OnUpdate()
        {
            var time = Context.Time;
            var commandBuffer = GetOrCreateCommandBuffer();
            World.Query(_unitsQuery,
                (Entity entity, ref HealthState healthState) =>
                {
                    if (healthState.Health > 0)
                    {
                        return;
                    }

                    healthState.Health = 0;
                    commandBuffer.Add(entity, new DeathState { DeathTime = time });

                    if (!entity.Has<DontDestroyOnDeath>())
                    {
                        commandBuffer.Add(entity, new Destroy());
                    }
                });
             
            World.Query(_deadUnitsQuery, 
                (Entity entity, ref DeathState deathState) =>
                {
                    if (deathState.DeathTime + deathState.Duration < time)
                    {
                        commandBuffer.Add(entity, new Destroy());
                    }
                });
        }
    }
}