#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Utils;
using UnityEngine;

namespace Game.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class CoinCollectingSystem : AbstractSystem
    {
        private static readonly QueryDescription _coinsQuery = new QueryDescription()
            .WithAll<Position, Coin>()
            .WithNone<Destroy, CoinCaptured>();

        private static readonly QueryDescription _capturedCoinsQuery = new QueryDescription()
            .WithAll<Position, CoinCaptured>()
            .WithNone<Destroy>();

        protected override void OnUpdate()
        {
            var playerEntity = World.GetPlayerSingleton();
            if (playerEntity == Entity.Null
                || !playerEntity.TryGet<Position>(out var playerPosition)
                || !playerEntity.TryGet<CoinCollector>(out var coinCollector))
            {
                return;
            }

            // TODO: Heavy operation, possible to optimize with burst
            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
            World.Query(_coinsQuery,
                (Entity entity, ref Position position) =>
                {
                    var delta = playerPosition.Value - position.Value;
                    delta.y = 0;
                    
                    if (delta.magnitude < coinCollector.CollectRadius)
                    {
                        commandBuffer.Add(entity, new CoinCaptured
                        {
                            // TODO: Make configurable
                            Acceleration = 0.2f
                        });
                    }
                });
                
            // TODO: Heavy operation, possible to optimize with burst
            World.Query(_capturedCoinsQuery, 
                (Entity entity, ref Position position, ref CoinCaptured coinCaptured) =>
                {
                    var delta = playerPosition.Value - position.Value;
                    if (delta.magnitude < coinCaptured.Speed)
                    {
                        coinCollector.CoinsAmount += 1;
                        position.Value += playerPosition.Value;
                        commandBuffer.Add(entity, new Destroy());
                    }
                    else
                    {
                        position.Value += delta.normalized * coinCaptured.Speed;
                        coinCaptured.Speed += Time.deltaTime * coinCaptured.Acceleration;
                    }
                });
            
            playerEntity.Set(coinCollector);
        }

        private struct CoinCaptured
        {
            public float Speed;
            public float Acceleration;
        }
    }
}