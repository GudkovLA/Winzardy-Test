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
    public class ResourceCaptureSystem : AbstractSystem
    {
        private readonly QueryDescription _coinsQuery = new QueryDescription()
            .WithAll<Position, Resource>()
            .WithNone<Destroy, ResourceCapture>();

        private readonly QueryDescription _capturedCoinsQuery = new QueryDescription()
            .WithAll<Position, ResourceCapture>()
            .WithNone<Destroy>();

        private ResourcesRegistry _resourcesRegistry = null!;
        private float _resourceCaptureAcceleration;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _resourcesRegistry))
            {
                return;
            }

            _resourceCaptureAcceleration = ServiceLocator.TryGet<GameSettings>(out var gameSettings)
                ? gameSettings.ResourceCaptureAcceleration
                : 0.2f;

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

            var commandBuffer = GetOrCreateCommandBuffer(); 
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
                            Acceleration = _resourceCaptureAcceleration
                        });
                    }
                });
                
            World.Query(_capturedCoinsQuery, 
                (Entity entity, ref Position position, ref ResourceCapture resourceCapture) =>
                {
                    var delta = playerPosition.Value - position.Value;
                    if (delta.magnitude < resourceCapture.Speed)
                    {
                        _resourcesRegistry.CollectResource(resourceCapture.ResourceId);
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