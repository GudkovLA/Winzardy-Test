#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Arch.Core.Extensions;
using Game.AbilitySystem.Settings;
using Game.Common.Components;
using Game.Components;
using Game.LocomotionSystem.Components;
using Game.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.AbilitySystem.Abilities
{
    [Serializable]
    public class RandomDirectionProjectileAbility : IAbility
    {
        private RandomDirectionProjectileAbilitySettings _abilitySettings;

        public RandomDirectionProjectileAbility(RandomDirectionProjectileAbilitySettings abilitySettings)
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
            return true;
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

            for (var i = 0; i < _abilitySettings.ProjectilesAmountPerActivation; i++)
            {
                var angle = Random.value * Mathf.PI * 2;
                var direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

                var entity = AbilityUtils.SpawnProjectile(world, commandBuffer, _abilitySettings.ProjectileSettings);
                commandBuffer.Add(entity, new Position { Value = startPosition });
                commandBuffer.Add(entity, new Rotation { Value = Quaternion.identity });
                commandBuffer.Add(entity, new LocomotionState
                {
                    Speed = _abilitySettings.ProjectileSettings.Speed,
                    Direction = direction
                });
                commandBuffer.Add(entity, fraction);
            }
        }
    }
}