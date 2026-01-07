#nullable enable

using Arch.Core;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;

namespace Game.DamageSystem.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class DamageInitializationSystem : AbstractSystem
    {
        private readonly QueryDescription _damageHitQuery = new QueryDescription()
            .WithAll<DamageHitTag>()
            .WithNone<Destroy>();

        protected override void OnUpdate()
        {
            var commandBuffer = GetOrCreateCommandBuffer();
            World.Query(_damageHitQuery,
                (entity) =>
                {
                    commandBuffer.Remove<DamageHitTag>(entity);
                });
        }        
    }
}