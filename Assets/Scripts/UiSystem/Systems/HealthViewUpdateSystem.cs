#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.PresentationSystem.Components;
using Game.UiSystem.Views;
using UnityEngine;

namespace Game.UiSystem.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class HealthViewUpdateSystem : AbstractSystem
    {
        private readonly QueryDescription _enemyQuery = new QueryDescription()
            .WithAll<HealthState>()
            .WithNone<Destroy, HandledTag, PlayerTag>();

        private readonly QueryDescription _healthViewQuery = new QueryDescription()
            .WithAll<HealthViewState, InstanceLink>()
            .WithNone<Destroy, PlayerTag>();
        
        private GameSettings _gameSettings = null!;
        private GameUi _gameUi = null!;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _gameSettings)
                || !ServiceLocator.TryGet(out _gameUi))
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
            
            var commandBuffer = GetOrCreateCommandBuffer();
            World.Query(_enemyQuery,
                entity =>
                {
                    commandBuffer.Add(entity, new HandledTag());

                    if (_gameSettings.HealthViewPrefab == null)
                    {
                        return;
                    }
                        
                    var healthView = Context.World.Create();
                    commandBuffer.Add(healthView, new HealthViewState
                    {
                        EntityHandle = new EntityHandle(entity),
                        View = null
                    });
                    commandBuffer.Add(healthView, new PrefabId
                    {
                        Value = _gameSettings.HealthViewPrefab.GetInstanceID()
                    });
                });
                
            World.Query(_healthViewQuery, 
                (Entity entity, ref HealthViewState healthViewState, ref InstanceLink instanceLink) =>
                {
                    if (!healthViewState.EntityHandle.IsValid())
                    {
                        commandBuffer.Add(entity, new Destroy());
                        return;
                    }

                    if (healthViewState.View == null)
                    {
                        var healthView = instanceLink.Instance.GetComponent<HealthView>();
                        if (healthView == null)
                        {
                            Debug.LogError($"Can't find {nameof(HealthView)} in health view entity");
                            return;
                        }

                        healthViewState.View = healthView;
                        instanceLink.Instance.transform.SetParent(_gameUi.Root);
                    }

                    var enemyEntity = healthViewState.EntityHandle.Value;
                    if (!enemyEntity.TryGet<HealthState>(out var healthState))
                    {
                        Debug.LogError($"Can't find {nameof(HealthState)} in enemy entity");
                        instanceLink.Instance.SetActive(false);
                        return;
                    }

                    if (!enemyEntity.TryGet<Position>(out var position))
                    {
                        Debug.LogError($"Can't find {nameof(Position)} in enemy entity");
                        instanceLink.Instance.SetActive(false);
                        return;
                    }

                    var size = enemyEntity.Has<Size>()
                        ? enemyEntity.Get<Size>().Value
                        : Vector3.zero;
                    var offset = new Vector3(0, size.y, 0);
                    
                    healthViewState.View.SetHealth(healthState.Health);
                    instanceLink.Instance.transform.position= _gameUi.GetScreenPosition(position.Value + offset);
                    instanceLink.Instance.SetActive(true);
                });
        }
        
        private struct HealthViewState
        {
            public EntityHandle EntityHandle;
            public HealthView? View;
        }

        private struct HandledTag
        {
        }
    }
}