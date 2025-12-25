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

            var characterSettings = ServiceLocator.Get<CharacterSettings>();
            if (characterSettings == null)
            {
                Debug.LogError($"Can't find character settings");
                return;
            }

            var gameLevel = ServiceLocator.Get<GameLevel>();
            if (gameLevel == null)
            {
                Debug.LogError($"Can't find game level instance");
                return;
            }
            
            _characterSettings =  characterSettings;
            _gameLevel = gameLevel;
            _initialized = true;
        }
        
        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }
            
            var playersCount = World.CountEntities(_playerQuery);
            if (playersCount >= KMaxCharactersCount)
            {
                return;
            }

            if (_instantiateOperation != null)
            {
                if (!_instantiateOperation.isDone)
                {
                    return;
                }

                if (_instantiateOperation.Result != null && _instantiateOperation.Result.Length == 1)
                {
                    CreateCharacterEntity(_instantiateOperation.Result[0]);
                }
                else
                {
                    Debug.LogError($"Can't spawn player due of instance incompatibility");
                }
                
                _instantiateOperation.completed -= OnInstantiateComplete;
                _instantiateOperation = null;
                return;
            }
            
            _instantiateOperation = Object.InstantiateAsync(_characterSettings.Prefab, _gameLevel.Root);
            _instantiateOperation.completed += OnInstantiateComplete;
        }

        private void CreateCharacterEntity(GameObject gameObject)
        {
            var entity = Context.World.Create();
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            commandBuffer.Add(entity, new Position());
            commandBuffer.Add(entity, new Rotation());
            commandBuffer.Add(entity, new PlayerControl());
            commandBuffer.Add(entity, new HealthState());
            commandBuffer.Add(entity, new TransformLink { Transform = gameObject.transform });
        }

        private void OnInstantiateComplete(AsyncOperation operation)
        {
            if (!Disposed)
            {
                return;
            }
            
            if (operation is not AsyncInstantiateOperation instantiateOperation)
            {
                Debug.LogError($"Operation is not {nameof(AsyncInstantiateOperation)}");
                return;
            }

            Debug.LogError($"System already been disposed {nameof(PlayerSpawnSystem)}");

            foreach (var gameObject in instantiateOperation.Result)
            {
                Object.Destroy(gameObject);
            }
        }
    }
}