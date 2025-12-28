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
            .WithNone<Destroy, CoinCaptured>();

        private static readonly QueryDescription _capturedCoinsQuery = new QueryDescription()
            .WithAll<Position, CoinCaptured>()
            .WithNone<Destroy>();

        private static readonly QueryDescription _collectorQuery = new QueryDescription()
            .WithAll<Position, CoinCollector, InstanceLink>()
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

            using var __ = ListPool<CoinCapturedData>.Get(out var coinsCapturedData);
            World.Query(_capturedCoinsQuery,
                (Entity entity, ref Position position, ref CoinCaptured coinCaptured) =>
                {
                    coinsCapturedData.Add(new CoinCapturedData
                    {
                        Entity = entity,
                        Position = position.Value,
                        CoinCaptured = coinCaptured
                    });
                });

            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
                
            // TODO: Heavy operation, possible to optimize with burst
            World.Query(_collectorQuery, 
                (ref Position position, ref CoinCollector coinCollector) =>
                {
                    for (var i = coinsData.Count - 1; i >= 0; i--)
                    {
                        var delta = position.Value - coinsData[i].Position;
                        delta.y = 0;
                        
                        if (delta.magnitude > coinCollector.CollectRadius)
                        {
                            continue;
                        }

                        commandBuffer.Add(coinsData[i].Entity, new CoinCaptured
                        {
                            Acceleration = 0.2f
                        });
                    }
                    
                    for (var i = coinsCapturedData.Count - 1; i >= 0; i--)
                    {
                        var coinEntity = coinsCapturedData[i].Entity;
                        var coinPosition = coinsCapturedData[i].Position;
                        var coinCaptured = coinsCapturedData[i].CoinCaptured;
                        
                        var delta = position.Value - coinPosition;
                        if (delta.magnitude < coinCaptured.Speed)
                        {
                            coinCollector.CoinsAmount += 1;
                            commandBuffer.Set(coinEntity, new Position { Value = position.Value });
                            commandBuffer.Add(coinEntity, new Destroy());
                        }
                        else
                        {
                            coinPosition += delta.normalized * coinCaptured.Speed;
                            coinCaptured.Speed += Time.deltaTime * coinCaptured.Acceleration;
                            commandBuffer.Set(coinEntity, new Position { Value = coinPosition });
                            commandBuffer.Set(coinEntity, coinCaptured);
                        }
                    }
                });
        }

        private struct CoinData
        {
            public Entity Entity;
            public Vector3 Position;
        }
        
        private struct CoinCaptured
        {
            public float Speed;
            public float Acceleration;
        }
        
        private struct CoinCapturedData
        {
            public Entity Entity;
            public Vector3 Position;
            public CoinCaptured CoinCaptured;
        }
    }
}