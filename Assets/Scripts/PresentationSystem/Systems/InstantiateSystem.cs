#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.PresentationSystem.Components;

namespace Game.PresentationSystem.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class InstantiateSystem : AbstractSystem
    {
        private readonly QueryDescription _instanceQuery = new QueryDescription()
            .WithAll<PrefabId>()
            .WithNone<InstanceLink, InvisibleTag>();

        private InstanceFactory _instanceFactory = null!;
        private GameLevel _gameLevel = null!;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _instanceFactory)
                || !ServiceLocator.TryGet(out _gameLevel))
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
            World.Query(_instanceQuery, 
                (Entity entity, ref PrefabId prefabId) =>
                {
                    if (entity.TryGet<Position>(out var position)
                        && entity.TryGet<Rotation>(out var rotation))
                    {
                        commandBuffer.Add(entity, new InstanceLink
                        {
                            Instance = _instanceFactory.Create(prefabId.Value,
                                _gameLevel.Root, 
                                position.Value, 
                                rotation.Value)
                        });
                        return;
                    }

                    commandBuffer.Add(entity,
                        new InstanceLink
                        {
                            Instance = _instanceFactory.Create(prefabId.Value, _gameLevel.Root)
                        });
                });
        }
    }
}