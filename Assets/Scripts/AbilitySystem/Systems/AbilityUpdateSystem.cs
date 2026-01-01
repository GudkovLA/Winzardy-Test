#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.AbilitySystem.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;

namespace Game.AbilitySystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class AbilityUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _abilitiesQuery = new QueryDescription()
            .WithAll<AbilityState>()
            .WithNone<Destroy>();

        private AbilityManager _abilityManager = null!;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _abilityManager))
            {
                return;
            }

            _initialized = true;
        }

        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }
            
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            
            World.Query(_abilitiesQuery,
                (Entity entity, ref AbilityState abilityState) =>
                {
                    if (abilityState.LastActivateTime + abilityState.ActivateTimeout > Context.Time)
                    {
                        return;
                    }

                    if (!abilityState.OwnerEntity.IsValid())
                    {
                        commandBuffer.Destroy(entity);
                        return;
                    }
                    
                    var ownerEntity = abilityState.OwnerEntity.Value;
                    if (ownerEntity.Has<IsDeadTag>())
                    {
                        return;
                    }

                    if (!_abilityManager.CanActivateAbility(abilityState.AbilityId, ownerEntity))
                    {
                        return;
                    }
                    
                    _abilityManager.ActivateAbility(abilityState.AbilityId, commandBuffer, ownerEntity);
                    abilityState.LastActivateTime = Context.Time;
                });
        }
    }
}