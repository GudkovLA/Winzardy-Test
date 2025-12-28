#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;

namespace Game.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class HudUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _coinCollectorQuery = new QueryDescription()
            .WithAll<CoinCollector>()
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
            
            World.Query(_coinCollectorQuery,
                (ref CoinCollector coinCollector) =>
                {
                    _gameUi.HudController.SetCoinsAmount(coinCollector.CoinsAmount);
                });
        }
    }
}