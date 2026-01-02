#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Arch.Core.Extensions;
using Game.AbilitySystem.Settings;
using Game.Common.Components;
using Game.Components;
using Game.LocomotionSystem.Components;
using Game.ProjectileSystem.Components;
using Game.ProjectileSystem.Settings;
using Game.Utils;
using UnityEngine;

namespace Game.AbilitySystem.Abilities
{
    [Serializable]
    public class AreaProjectileAbility : IAbility
    {
        private static readonly QueryDescription _enemyquery = new QueryDescription()
            .WithAll<Position, ProjectileCollider, Fraction>()
            .WithNone<PlayerTag>();
        
        private AreaProjectileAbilitySettings _abilitySettings;

        public AreaProjectileAbility(AreaProjectileAbilitySettings abilitySettings)
        {
            _abilitySettings = abilitySettings;
        }

        public void Prepare(InstancePool instancePool)
        {
            if (_abilitySettings.ProjectileSettings.Prefab != null)
            {
                instancePool.Register(_abilitySettings.ProjectileSettings.Prefab,
                    _abilitySettings.ProjectileSettings.PoolSize);
            }
        }

        public bool CanActivate(World world, Entity ownerEntity)
        {
            var playerEntity = world.GetPlayerSingleton();
            return playerEntity != ownerEntity
                ? IsPlayerInHitRadius(ownerEntity, playerEntity, _abilitySettings.ProjectileSettings)
                : IsEnemyInHitRadius(world, playerEntity, _abilitySettings.ProjectileSettings);
        }

        public void Activate(World world, CommandBuffer commandBuffer, Entity ownerEntity)
        {
            if (!ownerEntity.TryGet<Position>(out var position))
            {
                Debug.LogError($"Can't find required component in ability owner (ComponentType={nameof(Position)})");
                return;
            }

            if (!ownerEntity.TryGet<Size>(out var size))
            {
                Debug.LogError($"Can't find required component in ability owner (ComponentType={nameof(Size)})");
                return;
            }

            if (!ownerEntity.TryGet<Fraction>(out var fraction))
            {
                Debug.LogError($"Can't find required component in ability owner (ComponentType={nameof(Fraction)})");
                return;
            }
            
            var startPosition = position.Value;
            startPosition.y += size.Value.y * 0.5f;

            var entity = AbilityUtils.SpawnProjectile(world, commandBuffer, _abilitySettings.ProjectileSettings);
            commandBuffer.Add(entity, new Position { Value = startPosition });
            commandBuffer.Add(entity, new Rotation { Value = Quaternion.identity });
            commandBuffer.Add(entity, new LocomotionState
            {
                Speed = _abilitySettings.ProjectileSettings.Speed,
                Direction = Vector3.zero
            });
            commandBuffer.Add(entity, fraction);
        }

        private static bool IsPlayerInHitRadius(
            Entity ownerEntity, 
            Entity playerEntity, 
            ProjectileSettings projectileSettings)
        {
            if (!ownerEntity.TryGet<Fraction>(out var ownerFraction)
                || !playerEntity.TryGet<Fraction>(out var playerFraction)
                || (ownerFraction.EnemiesMask & playerFraction.AlliesMask) == 0)
            {
                return false;
            }
            

            if (!ownerEntity.TryGet<Position>(out var ownerPosition)
                || !playerEntity.TryGet<Position>(out var playerPosition)
                || !playerEntity.TryGet<ProjectileCollider>(out var playerCollider))
            {
                return false;
            }

            var distance = Vector3.Distance(ownerPosition.Value, playerPosition.Value);
            return distance < projectileSettings.HitRadius + playerCollider.Radius;
        }

        private static bool IsEnemyInHitRadius(
            World world, 
            Entity playerEntity, 
            ProjectileSettings projectileSettings)
        {
            if (!playerEntity.TryGet<Position>(out var playerPosition)
                || !playerEntity.TryGet<Fraction>(out var playerFraction))
            {
                return false;
            }

            var result = false;
            world.Query(_enemyquery,
                (ref Position position, ref ProjectileCollider projectileCollider, ref Fraction fraction) =>
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
                if (distance < projectileSettings.HitRadius + projectileCollider.Radius)
                {
                    result = true;
                }
            });

            return result;
        }
    }
}