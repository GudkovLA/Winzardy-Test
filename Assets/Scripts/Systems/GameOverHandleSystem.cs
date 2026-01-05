#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.CharacterSystem.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Utils;

namespace Game.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class GameOverHandleSystem : AbstractSystem
    {
        private GameUi _gameUi = null!;
        private bool _initialized;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            if (!ServiceLocator.TryGet(out _gameUi))
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
            if (playerEntity != Entity.Null && playerEntity.Has<IsDeadTag>())
            {
                _gameUi.GameMenuController.ShowGameOverMenu();
            }
        }        
    }
}