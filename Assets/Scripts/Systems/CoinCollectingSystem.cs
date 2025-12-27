#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class CoinCollectingSystem : AbstractSystem
    {
        private static readonly QueryDescription _coinsQuery = new QueryDescription()
            .WithAll<Position, Coin>()
            .WithNone<Destroy>();

        private static readonly QueryDescription _collectorQuery = new QueryDescription()
            .WithAll<Position, CoinCollector>()
            .WithNone<Destroy>();

        protected override void OnUpdate()
        {
            using var _ = ListPool<CoinData>.Get(out var coinsData);
            World.Query(_coinsQuery,
                (Entity entity, ref Position position) =>
                {
                    coinsData.Add(new CoinData
                    {
                        Entity = entity,
                        Position = position.Value
                    });
                });

            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
                
            // TODO: Heavy operation, possible to optimize with burst
            World.Query(_collectorQuery, 
                (ref Position position, ref CoinCollector CoinCollector) =>
                {
                    for (var i = coinsData.Count - 1; i >= 0; i--)
                    {
                        var delta = position.Value - coinsData[i].Position;
                        delta.y = 0;
                        
                        if (delta.magnitude > CoinCollector.CollectRadius)
                        {
                            continue;
                        }

                        CoinCollector.CoinsAmount += 1;

                        commandBuffer.Add(coinsData[i].Entity, new Destroy());
                        coinsData.RemoveAt(i);
                    }
                });
        }

        private struct CoinData
        {
            public Entity Entity;
            public Vector3 Position;
        }
    }
}