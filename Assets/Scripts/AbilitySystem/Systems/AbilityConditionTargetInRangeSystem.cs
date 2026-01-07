#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.AbilitySystem.Components;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Utils;
using UnityEngine;

namespace Game.AbilitySystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class AbilityConditionTargetInRangeSystem : AbstractSystem
    {
        private static readonly QueryDescription _enemyquery = new QueryDescription()
            .WithAll<Position, Fraction>()
            .WithNone<PlayerTag>();
        
        private static readonly QueryDescription _unblockedAbilitiesQuery = new QueryDescription()
            .WithAll<Ability, AbilityConditionTargetInRange>()
            .WithNone<Destroy, AbilityBlockedTag>();

        private static readonly QueryDescription _blockedAbilitiesQuery = new QueryDescription()
            .WithAll<Ability, AbilityConditionTargetInRange, AbilityBlockedTag>()
            .WithNone<Destroy>();

        private EntityHandle? _playerEntity;

        protected override void OnUpdate()
        {
            var world = Context.World;
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            
            World.Query(_unblockedAbilitiesQuery,
                (Entity entity, ref Ability ability, ref AbilityConditionTargetInRange condition) =>
                {
                    if (!ability.OwnerEntity.IsValid())
                    {
                        commandBuffer.Destroy(entity);
                        return;
                    }
                    
                    var ownerEntity = ability.OwnerEntity.Value;
                    if (!CanActivate(world, ownerEntity, condition.Distance))
                    {
                        commandBuffer.Add(entity, new AbilityBlockedTag());
                    }
                });
            
            World.Query(_blockedAbilitiesQuery,
                (Entity entity, ref Ability ability, ref AbilityConditionTargetInRange condition) =>
                {
                    if (!ability.OwnerEntity.IsValid())
                    {
                        commandBuffer.Destroy(entity);
                        return;
                    }
                    
                    var ownerEntity = ability.OwnerEntity.Value;
                    if (CanActivate(world, ownerEntity, condition.Distance))
                    {
                        commandBuffer.Remove<AbilityBlockedTag>(entity);
                    }
                });
        }
        
        private static bool CanActivate(World world, Entity ownerEntity, float range)
        {
            var playerEntity = world.GetPlayerSingleton();
            return playerEntity != ownerEntity
                ? IsPlayerInRange(ownerEntity, playerEntity, range)
                : IsEnemyInRange(world, playerEntity, range);
        }
        
        private static bool IsPlayerInRange(Entity ownerEntity, Entity playerEntity, float range)
        {
            if (!ownerEntity.TryGet<Fraction>(out var ownerFraction)
                || !playerEntity.TryGet<Fraction>(out var playerFraction)
                || (ownerFraction.EnemiesMask & playerFraction.AlliesMask) == 0)
            {
                return false;
            }
            

            if (!ownerEntity.TryGet<Position>(out var ownerPosition)
                || !playerEntity.TryGet<Position>(out var playerPosition))
            {
                return false;
            }

            var distance = Vector3.Distance(ownerPosition.Value, playerPosition.Value);
            return distance < range;
        }

        private static bool IsEnemyInRange(World world, Entity playerEntity, float range)
        {
            if (!playerEntity.TryGet<Position>(out var playerPosition)
                || !playerEntity.TryGet<Fraction>(out var playerFraction))
            {
                return false;
            }

            var result = false;
            world.Query(_enemyquery,
                (ref Position position, ref Fraction fraction) =>
            {
                if (result)
                {
                    return;
                }

                if ((playerFraction.EnemiesMask & fraction.AlliesMask) == 0)
                {
                    return;
                }
                
                var distance = Vector3.Distance(playerPosition.Value, position.Value);
                if (distance < range)
                {
                    result = true;
                }
            });

            return result;
        }
    }
}