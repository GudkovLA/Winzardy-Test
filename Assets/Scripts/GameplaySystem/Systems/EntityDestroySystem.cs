#nullable enable

using Arch.Core;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;

namespace Game.GameplaySystem.Systems
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public class EntityDestroySystem : AbstractSystem
    {
        private readonly QueryDescription _destroyQuery = new QueryDescription()
            .WithAll<Destroy>();

        protected override void OnUpdate()
        {
            var commandBuffer = GetOrCreateCommandBuffer(); 
            World.Query(_destroyQuery, 
                entity =>
                {
                    commandBuffer.Destroy(entity);
                });
        }        
    }
}