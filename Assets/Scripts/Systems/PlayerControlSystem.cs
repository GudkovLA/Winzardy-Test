#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Settings;
using Game.Utils;
using UnityEngine;

namespace Game.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlayerControlSystem : AbstractSystem
    {
        private CharacterSettings _characterSettings = null!;
        private GameInput _gameInput = null!;
        private bool _initialized;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            if (!ServiceLocator.TryGet(out _characterSettings)
                || !ServiceLocator.TryGet(out _gameInput))
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

            var deltaTime = Context.DeltaTime;
            var moveInput = _gameInput.Player.Move.ReadValue<Vector2>();
            var moveSpeed = _characterSettings.Speed * deltaTime;

            var playerEntity = World.GetPlayerSingleton();
            if (playerEntity != Entity.Null
                && !playerEntity.Has<IsDeadTag>()
                && playerEntity.TryGet<Position>(out var playerPosition))
            {
                playerPosition.Value += new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
                playerEntity.Set(playerPosition);
            }
        }
    }
}