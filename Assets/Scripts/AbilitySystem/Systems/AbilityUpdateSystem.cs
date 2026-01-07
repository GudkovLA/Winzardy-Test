#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.AbilitySystem.Components;
using Game.CharacterSystem.Components;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;

namespace Game.AbilitySystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class AbilityUpdateSystem : AbstractSystem
    {
        private readonly QueryDescription _abilitiesQuery = new QueryDescription()
            .WithAll<Ability, AbilityCooldown>()
            .WithNone<Destroy, AbilityReadyTag>();

        protected override void OnUpdate()
        {
            var commandBuffer = GetOrCreateCommandBuffer();
            World.Query(_abilitiesQuery,
                (Entity entity, ref Ability ability, ref AbilityCooldown abilityCooldown) =>
                {
                    if (ability.LastActivateTime + abilityCooldown.Duration > Context.Time)
                    {
                        return;
                    }

                    if (!ability.OwnerEntity.IsValid())
                    {
                        commandBuffer.Destroy(entity);
                        return;
                    }
                    
                    var ownerEntity = ability.OwnerEntity.Value;
                    if (ownerEntity.Has<IsDeadTag>())
                    {
                        return;
                    }
                    
                    commandBuffer.Add(entity, new AbilityReadyTag());
                });
        }
    }
}