#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Settings;
using UnityEngine;

namespace Game.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class CameraUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _playerQuery = new QueryDescription()
            .WithAll<PlayerControl>();

        private GameSettings _gameSettings = null!;
        private GameCamera _gameCamera = null!;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            var gameSettings = ServiceLocator.Get<GameSettings>();
            if (gameSettings == null)
            {
                Debug.LogError($"Can't find character settings");
                return;
            }

            var gameCamera = ServiceLocator.Get<GameCamera>();
            if (gameCamera == null)
            {
                Debug.LogError($"Can't find game camera instance");
                return;
            }
            
            _gameSettings = gameSettings;
            _gameCamera = gameCamera;
            _initialized = true;
        }
        
        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }

            World.Query(_playerQuery, (ref Position position) =>
            {
                _gameCamera.SetTransform(position.Value + _gameSettings.CameraSettings.Offset,
                    Quaternion.Euler(_gameSettings.CameraSettings.Angle));
            });
        }
    }
}