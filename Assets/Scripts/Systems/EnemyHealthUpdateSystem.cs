#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Settings;
using Game.Ui;
using UnityEngine;

namespace Game.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class EnemyHealthUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _enemyQuery = new QueryDescription()
            .WithAll<HealthState, Enemy>()
            .WithNone<Destroy, HandledTag>();

        private static readonly QueryDescription _healthViewQuery = new QueryDescription()
            .WithAll<HealthView, InstanceLink>()
            .WithNone<Destroy>();
        
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
            
            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 

            World.Query(_enemyQuery,
                entity =>
                {
                    var healthView = Context.World.Create();
                    commandBuffer.Add(healthView, new HealthView
                    {
                        EntityHandle = new EntityHandle(entity),
                        HealthController = null
                    });
                    commandBuffer.Add(healthView, new PrefabId
                    {
                        Value = _gameSettings.HealthViewPrefab.GetInstanceID()
                    });
                    commandBuffer.Add(entity, new HandledTag());
                });
                
            World.Query(_healthViewQuery, 
                (Entity entity, ref HealthView healthView, ref InstanceLink instanceLink) =>
                {
                    if (!healthView.EntityHandle.IsValid())
                    {
                        commandBuffer.Add(entity, new Destroy());
                        return;
                    }

                    if (healthView.HealthController == null)
                    {
                        var healthController = instanceLink.Instance.GetComponent<HealthController>();
                        if (healthController == null)
                        {
                            Debug.LogError($"Can't find {nameof(HealthController)} in health view entity");
                            return;
                        }

                        healthView.HealthController = healthController;
                        instanceLink.Instance.transform.SetParent(_gameUi.Root);
                    }

                    var enemyEntity = healthView.EntityHandle.Value;
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
                    
                    healthView.HealthController.SetHealth(healthState.Health);
                    instanceLink.Instance.transform.position= _gameUi.GetScreenPosition(position.Value + offset);
                    instanceLink.Instance.SetActive(true);
                });
        }
        
        private struct HealthView
        {
            public EntityHandle EntityHandle;
            public HealthController? HealthController;
        }

        private struct HandledTag
        {
        }
    }
}