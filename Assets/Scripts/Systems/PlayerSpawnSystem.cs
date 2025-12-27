#nullable enable

using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Settings;

namespace Game.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class PlayerSpawnSystem : AbstractSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet<CharacterSettings>(out var characterSettings)
                || !ServiceLocator.TryGet<GameLevel>(out var gameLevel))
            {
                return;
            }

            var entity = Context.World.Create();
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            commandBuffer.Add(entity, new Position { Value = gameLevel.StartPosition });
            commandBuffer.Add(entity, new Rotation { Value = gameLevel.StartRotation });
            commandBuffer.Add(entity, new PrefabId { Value = characterSettings.Prefab.GetInstanceID() });
            commandBuffer.Add(entity, new HealthState
            {
                MaxHealth = characterSettings.MaxHealth,
                Health = characterSettings.MaxHealth
            });
            commandBuffer.Add(entity, new CoinCollector
            {
                CollectRadius = characterSettings.CoinsCollectRadius
            });
            commandBuffer.Add(entity, new PlayerControl());
        }
    }
}