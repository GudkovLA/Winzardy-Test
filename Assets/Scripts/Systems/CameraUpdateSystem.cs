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

            if (!ServiceLocator.TryGet(out _gameSettings)
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

            World.Query(_playerQuery, (ref Position position) =>
            {
                _gameCamera.SetTransform(position.Value + _gameSettings.CameraSettings.Offset,
                    Quaternion.Euler(_gameSettings.CameraSettings.Angle));
            });
        }
    }
}