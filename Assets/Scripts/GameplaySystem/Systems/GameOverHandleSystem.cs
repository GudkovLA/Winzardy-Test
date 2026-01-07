#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.Utils;

namespace Game.GameplaySystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class GameOverHandleSystem : AbstractSystem
    {
        private GameUi _gameUi = null!;
        private float _deathCameraDuration;
        private bool _initialized;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            if (!ServiceLocator.TryGet(out _gameUi))
            {
                return;
            }

            _deathCameraDuration = ServiceLocator.TryGet<GameSettings>(out var gameSettings)
                ? gameSettings.DeathCameraDuration
                : 0f;
            
            _initialized = true;
        }

        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }

            var time = Context.Time;
            var playerEntity = World.GetPlayerSingleton();
            if (playerEntity != Entity.Null && playerEntity.TryGet<DeathState>(out var deathState))
            {
                if (deathState.DeathTime + deathState.Duration + _deathCameraDuration < time)
                {
                    _gameUi.GameMenuController.ShowGameOverMenu();
                }
            }
        }        
    }
}