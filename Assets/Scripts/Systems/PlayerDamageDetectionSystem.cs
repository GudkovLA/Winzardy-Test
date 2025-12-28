#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlayerDamageDetectionSystem : AbstractSystem
    {
        private static readonly QueryDescription _enemyQuery = new QueryDescription()
            .WithAll<Position, Enemy, Damage>()
            .WithNone<Destroy>();

        private static readonly QueryDescription _playerQuery = new QueryDescription()
            .WithAll<Position, PlayerControl, HealthState>()
            .WithNone<Destroy>();
        
        
        protected override void OnUpdate()
        {
            using var _ = ListPool<EnemyData>.Get(out var enemyData);
            World.Query(_enemyQuery,
                (Entity entity, ref Position position, ref Enemy enemy, ref Damage damage) =>
                {
                    enemyData.Add(new EnemyData
                    {
                        Entity = entity,
                        Position = position.Value,
                        Enemy = enemy,
                        Damage = damage
                    });
                });

            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
                
            // TODO: Heavy operation, possible to optimize with burst
            World.Query(_playerQuery, 
                (Entity entity, ref Position position, ref HealthState healthState) =>
                {
                    for (var i = enemyData.Count - 1; i >= 0; i--)
                    {
                        var enemy = enemyData[i].Enemy;
                        if (enemy.LastHitTime + enemy.HitTimeout > Context.Time)
                        {
                            continue;
                        }

                        var delta = position.Value - enemyData[i].Position;
                        delta.y = 0;
                        
                        var enemyDamage = enemyData[i].Damage;
                        if (delta.magnitude > enemyDamage.HitDistance)
                        {
                            continue;
                        }
                 
                        // TODO: looks like a job for different system (in late simulation for example)
                        if (enemyDamage.Amount <= 0)
                        {
                            continue;                            
                        }

                        healthState.Health -= enemyDamage.Amount;
                        healthState.LastHitTime = Context.Time;
                        
                        enemy.LastHitTime = Context.Time;
                        commandBuffer.Set(enemyData[i].Entity, enemy);
                        enemyData.RemoveAt(i);
                    }
                });
        }

        private struct EnemyData
        {
            public Entity Entity;
            public Vector3 Position;
            public Enemy Enemy;
            public Damage Damage;
        }
    }
}