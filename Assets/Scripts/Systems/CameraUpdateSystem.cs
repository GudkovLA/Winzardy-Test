#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Utils;
using UnityEngine;

namespace Game.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class CameraUpdateSystem : AbstractSystem
    {
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

            var playerEntity = World.GetPlayerSingleton();
            if (playerEntity != Entity.Null
                && playerEntity.TryGet<Position>(out var playerPosition))
            {
                _gameCamera.SetTransform(playerPosition.Value + _gameSettings.Camera.Offset,
                    Quaternion.Euler(_gameSettings.Camera.Angle));
            }
        }
    }
}