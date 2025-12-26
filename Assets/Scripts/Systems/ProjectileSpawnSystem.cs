#nullable enable

using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ProjectileSpawnSystem : AbstractSystem
    {
        private CharacterSettings _characterSettings = null!;
        private GameCamera _gameCamera = null!;
        
        private float _timeCounter;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _characterSettings)
                || !ServiceLocator.TryGet(out _gameCamera))
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
            if (_timeCounter < _characterSettings.Projectile.FireTimeout)
            {
                return;
            }

            _timeCounter -= _characterSettings.Projectile.FireTimeout;

            // TODO: Need to be used player position, not the camera
            var positon = _gameCamera.Camera.transform.position;
            positon.y = 1f;

            var angle = Random.value * Mathf.PI * 2;
            var direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            
            var entity = Context.World.Create();
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            commandBuffer.Add(entity, new Position { Value = positon });
            commandBuffer.Add(entity, new Rotation { Value = Quaternion.identity });
            commandBuffer.Add(entity, new PrefabId { Value = _characterSettings.Projectile.Prefab.GetInstanceID() });
            commandBuffer.Add(entity, new Projectile
            {
                Direction = direction,
                Speed = _characterSettings.Projectile.Speed,
                HitDistance = _characterSettings.Projectile.HitDistance,
                MaxDistance = _characterSettings.Projectile.MaxDistance
            });
        }
    }
}