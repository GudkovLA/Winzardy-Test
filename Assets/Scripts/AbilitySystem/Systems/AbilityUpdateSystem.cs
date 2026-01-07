#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.AbilitySystem.Components;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;

namespace Game.AbilitySystem.Systems
{
    [UpdateInGroup(typeof(AbilitySystemGroup))]
    public class AbilityUpdateSystem : AbstractSystem
    {
        private readonly QueryDescription _abilitiesQuery = new QueryDescription()
            .WithAll<Ability, AbilityCooldown>()
            .WithNone<Destroy, AbilityReadyTag>();

        protected override void OnUpdate()
        {
            var time = Context.Time;
            var commandBuffer = GetOrCreateCommandBuffer();
            World.Query(_abilitiesQuery,
                (Entity entity, ref Ability ability, ref AbilityCooldown abilityCooldown) =>
                {
                    if (ability.LastActivateTime + abilityCooldown.Duration > time)
                    {
                        return;
                    }

                    if (!ability.OwnerEntity.IsValid())
                    {
                        commandBuffer.Add(entity, new Destroy());
                        return;
                    }
                    
                    var ownerEntity = ability.OwnerEntity.Value;
                    if (ownerEntity.Has<DeathState>())
                    {
                        if (entity.Has<AbilityReadyTag>())
                        {
                            commandBuffer.Remove<AbilityReadyTag>(entity);
                        }
                        
                        return;
                    }
                    
                    commandBuffer.Add(entity, new AbilityReadyTag());
                });
        }
    }
}