#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using UnityEngine.Pool;

namespace Game.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class HudUpdateSystem : AbstractSystem
    {
        /*
        private static readonly QueryDescription _palyerQuery = new QueryDescription()
            .WithAll<Position, Projectile>()
            .WithNone<Destroy>();

        private static readonly QueryDescription _enemyQuery = new QueryDescription()
            .WithAll<Position, HealthState, Enemy>()
            .WithNone<Destroy>();
        
        private GameSettings _gameSettings = null!;
        private EnemySettings _enemySettings = null!;
        private GameLevel _gameLevel = null!;
        private GameCamera _gameCamera = null!;
        
        private float _timeCounter;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _gameSettings)
                || !ServiceLocator.TryGet(out _enemySettings)
                || !ServiceLocator.TryGet(out _gameLevel)
                || !ServiceLocator.TryGet(out _gameCamera))
            {
                return;
            }

            _initialized = true;
        }
        
        protected override void OnUpdate()
        {
            using var _ = ListPool<ProjectileData>.Get(out var projectileData);
            World.Query(_projectileQuery,
                (Entity entity, ref Position position, ref Projectile projectile) =>
                {
                    projectileData.Add(new ProjectileData
                    {
                        Entity = entity,
                        Position = position.Value,
                        Projectile = projectile
                    });
                });

            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
                
            // TODO: Heavy operation, possible to optimize with burst
            World.Query(_enemyQuery, 
                (Entity entity, ref Position position, ref HealthState healthState) =>
                {
                    for (var i = projectileData.Count - 1; i >= 0; i--)
                    {
                        var delta = position.Value - projectileData[i].Position;
                        delta.y = 0;
                        
                        if (delta.magnitude > projectileData[i].Projectile.HitDistance)
                        {
                            continue;
                        }
                 
                        // TODO: looks like a job for different system (in late simulation for example)
                        if (projectileData[i].Projectile.Damage <= 0)
                        {
                            continue;                            
                        }
                        
                        healthState.Health -= projectileData[i].Projectile.Damage;
                        healthState.LastHitTime = Context.Time;

                        commandBuffer.Add(projectileData[i].Entity, new Destroy());
                        projectileData.RemoveAt(i);
                        
                        if (healthState.Health <= 0)
                        {
                            healthState.Health = 0;
                            commandBuffer.Add(entity, new Destroy());
                        }
                    }
                });
        }
       */
    }
}