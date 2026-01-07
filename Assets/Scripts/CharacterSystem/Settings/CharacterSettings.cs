#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem.Components;
using Game.AbilitySystem.Settings;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.Common.Components;
using Game.DamageSystem.Components;
using Game.LocomotionSystem.Components;
using Game.PresentationSystem;
using Game.PresentationSystem.Components;
using Game.ProjectileSystem.Components;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterSettings), menuName = "Assets/Character Settings")]
    [Serializable]
    public class CharacterSettings : ScriptableObject, IDisposable, IPoolable
    {
        public GameObject? Prefab;
        public int PoolSize;
        
        public Vector3 Size;
        public float Health;
        public float Speed;

        public FractionMask Fraction; 
        public FractionMask Enemies; 

        public AbstractAbilitySettings[] Abilities = null!;
        
        private ProjectileCollider? _projectileColliderCache;

        public void Dispose()
        {
        }

        public virtual void Prepare(InstancePool instancePool)
        {
            if (Prefab != null)
            {
                instancePool.Register(Prefab, PoolSize);
            }

            foreach (var abilitySettings in Abilities)
            {
                abilitySettings.Prepare(instancePool);
            }
        }

        public virtual void Initialize(World world, CommandBuffer commandBuffer, Entity entity)
        {
            if (Prefab == null)
            {
                throw new Exception("Character prefab is not defined");
            }
            
            commandBuffer.Add(entity, new Size { Value = Size });
            commandBuffer.Add(entity, new PrefabId { Value = Prefab.GetInstanceID() });
            commandBuffer.Add(entity, new LocomotionData { MaxSpeed = Speed });
            commandBuffer.Add(entity, new LocomotionState { Speed = Speed });
            commandBuffer.Add(entity, new HealthState
            {
                MaxHealth = Health,
                Health = Health
            });
           
            commandBuffer.Add(entity, GetProjectileCollider());
            
            commandBuffer.Add(entity, new Fraction
            {
                AlliesMask = Fraction,
                EnemiesMask = Enemies
            });
            
            foreach (var abilitySettings in Abilities)
            {
                var abilityEntity = world.Create();
                commandBuffer.Add(abilityEntity, new Ability
                {
                    OwnerEntity = new EntityHandle(entity),
                    LastActivateTime = Time.realtimeSinceStartup
                });
                
                abilitySettings.Initialize(commandBuffer, abilityEntity);
            }
        }

        private ProjectileCollider GetProjectileCollider()
        {
            if (_projectileColliderCache != null)
            {
                return _projectileColliderCache.Value;
            }
            
            if (Prefab != null 
                && Prefab.TryGetComponent<CapsuleCollider>(out var capsuleCollider))
            {
                _projectileColliderCache = new ProjectileCollider { Radius = capsuleCollider.radius };
                return _projectileColliderCache.Value;
            }

            return default;
        }
    }
}