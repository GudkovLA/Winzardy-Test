#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.ResourceSystem.Components;
using Game.Utils;
using UnityEngine;

namespace Game.ResourceSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ResourceCollectingSystem : AbstractSystem
    {
        private static readonly QueryDescription _coinsQuery = new QueryDescription()
            .WithAll<Position, Resource>()
            .WithNone<Destroy, ResourceCapture>();

        private static readonly QueryDescription _capturedCoinsQuery = new QueryDescription()
            .WithAll<Position, ResourceCapture>()
            .WithNone<Destroy>();

        private ResourcesManager _resourcesManager = null!;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _resourcesManager))
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
            
            var playerEntity = World.GetPlayerSingleton();
            if (playerEntity == Entity.Null
                || !playerEntity.TryGet<Position>(out var playerPosition)
                || !playerEntity.TryGet<ResourceCollector>(out var coinCollector))
            {
                return;
            }

            // TODO: Heavy operation, possible to optimize with burst
            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
            World.Query(_coinsQuery,
                (Entity entity, ref Position position, ref Resource resource) =>
                {
                    var delta = playerPosition.Value - position.Value;
                    delta.y = 0;
                    
                    if (delta.magnitude < coinCollector.CollectRadius)
                    {
                        commandBuffer.Add(entity, new ResourceCapture
                        {
                            ResourceId = resource.ResourceId,
                            // TODO: Make configurable
                            Acceleration = 0.2f
                        });
                    }
                });
                
            // TODO: Heavy operation, possible to optimize with burst
            World.Query(_capturedCoinsQuery, 
                (Entity entity, ref Position position, ref ResourceCapture resourceCapture) =>
                {
                    var delta = playerPosition.Value - position.Value;
                    if (delta.magnitude < resourceCapture.Speed)
                    {
                        _resourcesManager.CollectResource(resourceCapture.ResourceId);
                        position.Value += playerPosition.Value;
                        commandBuffer.Add(entity, new Destroy());
                    }
                    else
                    {
                        position.Value += delta.normalized * resourceCapture.Speed;
                        resourceCapture.Speed += Time.deltaTime * resourceCapture.Acceleration;
                    }
                });
        }

        private struct ResourceCapture
        {
            public int ResourceId;
            public float Speed;
            public float Acceleration;
        }
    }
}