#nullable enable

using Arch.Core;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.ResourceSystem;
using Game.ResourceSystem.Components;

namespace Game.UiSystem.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class HudUpdateSystem : AbstractSystem
    {
        private readonly QueryDescription _playerInfoQuery = new QueryDescription()
            .WithAll<HealthState, ResourceCollector>()
            .WithNone<Destroy>();

        private GameUi _gameUi = null!;
        private ResourcesRegistry _resourcesRegistry = null!;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _gameUi)
                || !ServiceLocator.TryGet(out _resourcesRegistry))
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
                (ref HealthState healthState) =>
                {
                    _gameUi.HudController.SetHealthAmount(healthState.Health, healthState.MaxHealth);
                });

            var i = 0;
            foreach (var resourceData in _resourcesRegistry.GetResources())
            {
                var resourceView = _gameUi.HudController.GetResourceView(i);
                resourceView.SetIcon(resourceData.Settings.Icon);
                resourceView.SetAmount(resourceData.Amount);
                i++;
            }
        }
    }
}