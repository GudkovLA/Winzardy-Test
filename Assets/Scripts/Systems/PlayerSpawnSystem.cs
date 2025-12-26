#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Settings;
using UnityEngine;

namespace Game.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class PlayerSpawnSystem : AbstractSystem
    {
        private const int KMaxCharactersCount = 1;

        private static readonly QueryDescription _playerQuery = new QueryDescription()
            .WithAll<PlayerControl>();
        
        private CharacterSettings _characterSettings = null!;
        private GameLevel _gameLevel = null!;
        private bool _initialized;
        
        private AsyncInstantiateOperation<GameObject>? _instantiateOperation;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _characterSettings)
                || !ServiceLocator.TryGet(out _gameLevel))
            {
                return;
            }

            _initialized = true;
        }
        
        protected override void OnUpdate()
        {
            var playersCount = World.CountEntities(_playerQuery);
            if (playersCount >= KMaxCharactersCount)
            {
                return;
            }

            var entity = Context.World.Create();
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            commandBuffer.Add(entity, new Position { Value = _gameLevel.StartPosition });
            commandBuffer.Add(entity, new Rotation { Value = _gameLevel.StartRotation });
            commandBuffer.Add(entity, new PrefabId { Value = _characterSettings.Prefab.GetInstanceID() });
            commandBuffer.Add(entity, new HealthState());
            commandBuffer.Add(entity, new PlayerControl());
        }
    }
}