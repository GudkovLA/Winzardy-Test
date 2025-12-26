#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Utils;

namespace Game.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class InstantiateSystem : AbstractSystem
    {
        private static readonly QueryDescription _instanceQuery = new QueryDescription()
            .WithAll<PrefabId, Position, Rotation>()
            .WithNone<TransformLink>();

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
            
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            World.Query(_instanceQuery, 
                (Entity entity, ref Position position, ref Rotation rotation, ref PrefabId prefabId) =>
                {
                    var instance = _instanceFactory.Create(prefabId.Value, 
                        _gameLevel.Root, 
                        position.Value, 
                        rotation.Value);
                    
                    commandBuffer.Add(entity, new TransformLink { Transform = instance.transform });
                });
        }
    }
}