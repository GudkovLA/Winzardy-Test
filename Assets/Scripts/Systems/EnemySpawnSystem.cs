#nullable enable

using Arch.Core;
using Game.AbilitySystem;
using Game.CharacterSystem;
using Game.CharacterSystem.Settings;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.ResourceSystem;
using Game.ResourceSystem.Components;
using Game.Settings;
using UnityEngine;

namespace Game.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class EnemySpawnSystem : AbstractSystem
    {
        private GameSettings _gameSettings = null!;
        private EnemySettings _enemySettings = null!;
        private GameLevel _gameLevel = null!;
        private GameCamera _gameCamera = null!;
        private AbilityManager _abilityManager = null!;
        private ResourcesManager _resourcesManager = null!;
        
        private float _timeCounter;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _gameSettings)
                || !ServiceLocator.TryGet(out _enemySettings)
                || !ServiceLocator.TryGet(out _gameLevel)
                || !ServiceLocator.TryGet(out _gameCamera)
                || !ServiceLocator.TryGet(out _abilityManager)
                || !ServiceLocator.TryGet(out _resourcesManager))
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
            
            _timeCounter += Time.deltaTime;
            if (_timeCounter < _gameSettings.EnemySpawnTimeout)
            {
                return;
            }

            if (_enemySettings.Character.Prefab == null)
            {
                Debug.LogError($"Character prefab is not defined");
                return;
            }

            _timeCounter -= _gameSettings.EnemySpawnTimeout;
            
            var center = _gameCamera.Camera.transform.position;
            center.y = _gameLevel.Root.transform.position.y;

            var positon = center + new Vector3(Random.value * 20, 0, Random.value * 20);
            var lookDirection = (center - positon).normalized;
            var rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            var entity = CharacterUtils.SpawnCharacter(
                World, 
                _enemySettings,
                _abilityManager,
                commandBuffer,
                positon,
                rotation);

            if (entity == Entity.Null)
            {
                return;
            }

            if (_resourcesManager.TryGetDroppedResource(_enemySettings.GetInstanceID(), out var resourceId))
            {
                commandBuffer.Add(entity, new ResourceSpawner { ResourceId = resourceId });
            }
        }
    }
}