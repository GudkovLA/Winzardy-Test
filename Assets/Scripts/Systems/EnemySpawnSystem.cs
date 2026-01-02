#nullable enable

using Game.AbilitySystem;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.DamageSystem.Components;
using Game.LocomotionSystem.Components;
using Game.ProjectileSystem.Components;
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
        
        private float _timeCounter;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _gameSettings)
                || !ServiceLocator.TryGet(out _enemySettings)
                || !ServiceLocator.TryGet(out _gameLevel)
                || !ServiceLocator.TryGet(out _gameCamera)
                || !ServiceLocator.TryGet(out _abilityManager))
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

            _timeCounter -= _gameSettings.EnemySpawnTimeout;
            
            var center = _gameCamera.Camera.transform.position;
            center.y = _gameLevel.Root.transform.position.y;

            var positon = center + new Vector3(Random.value * 20, 0, Random.value * 20);
            var lookDirection = (center - positon).normalized;
            var rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            
            var entity = Context.World.Create();
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            commandBuffer.Add(entity, new Position { Value = positon });
            commandBuffer.Add(entity, new Rotation { Value = rotation });
            commandBuffer.Add(entity, new Size { Value = _enemySettings.Size });
            commandBuffer.Add(entity, new PrefabId { Value = _enemySettings.Prefab.GetInstanceID() });
            commandBuffer.Add(entity, new CoinSpawner
            {
                Chance = _enemySettings.CoinSettings.DropChance,
                CoinPrefabId = _enemySettings.CoinSettings.Prefab.GetInstanceID()
            });
            commandBuffer.Add(entity, new HealthState
            {
                MaxHealth = _enemySettings.MaxHealth,
                Health = _enemySettings.MaxHealth
            });
            commandBuffer.Add(entity, new LocomotionState
            {
                Speed = _enemySettings.Speed
            });
            commandBuffer.Add(entity, new Fraction
            {
                AlliesMask = _enemySettings.Fraction,
                EnemiesMask = _enemySettings.Enemies
            });
            commandBuffer.Add(entity, new ProjectileCollider
            {
                Radius = _enemySettings.ColliderRadius
            });
            
            foreach (var abilitySettings in _enemySettings.Abilities)
            {
                _abilityManager.CreateAbility(abilitySettings, entity);
            }
        }
    }
}