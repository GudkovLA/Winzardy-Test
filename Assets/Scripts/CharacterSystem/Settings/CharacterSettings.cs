#nullable enable

using System;
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
    public class CharacterSettings : ScriptableObject, IDisposable, IPoolable, IEntityBuilder
    {
        public GameObject? Prefab;
        public int PoolSize;
        
        public float SizeRadius;
        public float SizeHeight;
        public float MaxHealth;
        public float MoveSpeed;
        public float ProjectileColliderRadius;
        public bool IgnoreObstacles;
        public bool DontDestroyOnDeath;

        public FractionMask Fraction; 
        public FractionMask Enemies; 

        public AbstractAbilitySettings[] Abilities = null!;

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

        public virtual void Build(Entity entity, BuildContext context)
        {
            if (Prefab == null)
            {
                throw new Exception("Character prefab is not defined");
            }

            var commandBuffer = context.CommandBuffer;
            commandBuffer.Add(entity, new PrefabId { Value = Prefab.GetInstanceID() });
            commandBuffer.Add(entity, new LocomotionData { MaxSpeed = MoveSpeed });
            commandBuffer.Add(entity, new LocomotionState { Speed = MoveSpeed });
            commandBuffer.Add(entity, new Size
            {
                Radius = SizeRadius,
                Height =  SizeHeight
            });
            commandBuffer.Add(entity, new HealthState
            {
                MaxHealth = MaxHealth,
                Health = MaxHealth
            });
           
            if (IgnoreObstacles)
            {
                commandBuffer.Add(entity, new IgnoreObstaclesTag());
            }

            if (DontDestroyOnDeath)
            {
                commandBuffer.Add(entity, new DontDestroyOnDeath());
            }

            if (ProjectileColliderRadius > 0)
            {
                commandBuffer.Add(entity, new ProjectileCollider { Radius = ProjectileColliderRadius });
            }
            
            commandBuffer.Add(entity, new Fraction
            {
                AlliesMask = Fraction,
                EnemiesMask = Enemies
            });
            
            foreach (var abilitySettings in Abilities)
            {
                var abilityEntity = context.World.Create();
                commandBuffer.Add(abilityEntity, new Ability
                {
                    OwnerEntity = new EntityHandle(entity),
                    LastActivateTime = Time.realtimeSinceStartup
                });
                
                abilitySettings.Build(abilityEntity, context);
            }
        }
    }
}