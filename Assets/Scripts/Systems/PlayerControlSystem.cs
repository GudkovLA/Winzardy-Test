#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Settings;
using UnityEngine;

namespace Game.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlayerControlSystem : AbstractSystem
    {
        private static readonly QueryDescription _playerQuery = new QueryDescription()
            .WithAll<PlayerControl>();

        private CharacterSettings _characterSettings = null!;
        private GameInput _gameInput = null!;
        private bool _initialized;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            var characterSettings = ServiceLocator.Get<CharacterSettings>();
            if (characterSettings == null)
            {
                Debug.LogError("Can't find character settings");
                return;
            }
            
            var gameInput = ServiceLocator.Get<GameInput>();
            if (gameInput == null)
            {
                Debug.LogError("Can't find game input");
                return;
            }

            _characterSettings = characterSettings;
            _gameInput = gameInput;
            _initialized = true;
        }

        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }

            var moveInput = _gameInput.Player.Move.ReadValue<Vector2>();
            var moveSpeed = _characterSettings.Speed * Time.deltaTime;
            World.Query(_playerQuery, (ref Position position) =>
            {
                position.Value += new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
            });
        }
    }
}