#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.CharacterSystem.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.LocomotionSystem.Components;
using Game.Utils;
using UnityEngine;

namespace Game.CharacterSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlayerControlSystem : AbstractSystem
    {
        private GameInput _gameInput = null!;
        private bool _initialized;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            if (!ServiceLocator.TryGet(out _gameInput))
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

            var moveInput = _gameInput.Player.Move.ReadValue<Vector2>();

            var playerEntity = World.GetPlayerSingleton();
            if (playerEntity == Entity.Null)
            {
                Debug.LogError("Player entity not found");
                _initialized = false;
                return;
            }

            if (!playerEntity.TryGet<LocomotionState>(out var locomotionState))
            {
                Debug.LogError("Player entity in not initialized");
                _initialized = false;
                return;
            }
            
            locomotionState.Direction = !playerEntity.Has<IsDeadTag>()
                ? new Vector3(moveInput.x, 0, moveInput.y)
                : Vector3.zero;

            playerEntity.Set(locomotionState);
        }
    }
}