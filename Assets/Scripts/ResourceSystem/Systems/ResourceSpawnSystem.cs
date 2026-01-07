#nullable enable

using Arch.Core;
using Game.CharacterSystem.Components;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.PresentationSystem.Components;
using Game.ResourceSystem.Components;
using UnityEngine;

namespace Game.ResourceSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ResourceSpawnSystem : AbstractSystem
    {
        private readonly QueryDescription _deadEnemyQuery = new QueryDescription()
            .WithAll<Position, ResourceSpawner, IsDeadTag>();
        
        private ResourcesRegistry _resourcesRegistry = null!;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _resourcesRegistry))
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

            var commandBuffer = GetOrCreateCommandBuffer(); 
            World.Query(_deadEnemyQuery, 
                (Entity entity, ref Position position, ref ResourceSpawner resourceSpawner) =>
                {
                    commandBuffer.Remove<ResourceSpawner>(entity);

                    var resourceSettings = _resourcesRegistry.GetResource(resourceSpawner.ResourceId);
                    if (resourceSettings == null || resourceSettings.Prefab == null)
                    {
                        return;
                    }

                    var resource = Context.World.Create();
                    commandBuffer.Add(resource, new Position { Value = position.Value });
                    commandBuffer.Add(resource, new Rotation { Value = Quaternion.identity });
                    commandBuffer.Add(resource, new Resource { ResourceId = resourceSpawner.ResourceId });
                
                    commandBuffer.Add(resource,
                        new PrefabId { Value = resourceSettings.Prefab.GetInstanceID() });
                });
        }        
    }
}