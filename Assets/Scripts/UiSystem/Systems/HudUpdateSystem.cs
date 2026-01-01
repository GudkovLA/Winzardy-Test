#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.DamageSystem.Components;

namespace Game.UiSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class HudUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _playerInfoQuery = new QueryDescription()
            .WithAll<HealthState, CoinCollector>()
            .WithNone<Destroy>();

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
            
            World.Query(_playerInfoQuery,
                (ref HealthState healthState, ref CoinCollector coinCollector) =>
                {
                    _gameUi.HudController.SetHealthAmount(healthState.Health, healthState.MaxHealth);
                    _gameUi.HudController.SetCoinsAmount(coinCollector.CoinsAmount);
                });
        }
    }
}