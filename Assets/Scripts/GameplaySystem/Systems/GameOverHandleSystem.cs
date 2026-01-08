#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.GameplaySystem.Components;
using Game.Utils;

namespace Game.GameplaySystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class GameOverHandleSystem : AbstractSystem
    {
        private float _deathCameraDuration;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _deathCameraDuration = ServiceLocator.TryGet<GameSettings>(out var gameSettings)
                ? gameSettings.DeathCameraDuration
                : 0f;
        }

        protected override void OnUpdate()
        {
            var time = Context.Time;
            var playerEntity = World.GetPlayerSingleton();
            if (playerEntity != Entity.Null
                && !playerEntity.Has<GameOverTag>()
                && playerEntity.TryGet<DeathState>(out var deathState))
            {
                if (deathState.DeathTime + deathState.Duration + _deathCameraDuration < time)
                {
                    playerEntity.Add(new GameOverTag());
                }
            }
        }        
    }
}