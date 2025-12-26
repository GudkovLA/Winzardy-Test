#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;

namespace Game.Systems
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public class EntityDestroySystem : AbstractSystem
    {
        private static readonly QueryDescription _destroyQuery = new QueryDescription()
            .WithAll<Destroy>();

        protected override void OnUpdate()
        {
            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
            World.Query(_destroyQuery, 
                entity =>
                {
                    commandBuffer.Destroy(entity);
                });
        }        
    }
}