#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem.Settings;
using Game.Components;
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
            instancePool.Register(_abilitySettings.ProjectileSettings.Prefab, _abilitySettings.ProjectileSettings.PoolSize);
        }

        public void Activate(World world, CommandBuffer commandBuffer, Vector3 ownerPosition)
        {
            for (var i = 0; i < _abilitySettings.ProjectilesAmountPerActivation; i++)
            {
                var angle = Random.value * Mathf.PI * 2;
                var direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

                var entity = world.Create();
                commandBuffer.Add(entity, new Position { Value = ownerPosition });
                commandBuffer.Add(entity, new Rotation { Value = Quaternion.identity });
                commandBuffer.Add(entity,
                    new PrefabId { Value = _abilitySettings.ProjectileSettings.Prefab.GetInstanceID() });

                commandBuffer.Add(entity, new Projectile
                {
                    Direction = direction,
                    Speed = _abilitySettings.ProjectileSettings.Speed,
                    MaxDistance = _abilitySettings.ProjectileSettings.MaxDistance,
                });

                commandBuffer.Add(entity, new Damage
                {
                    Amount = _abilitySettings.ProjectileSettings.Damage,
                    HitDistance = _abilitySettings.ProjectileSettings.HitDistance,
                });
            }
        }
    }
}