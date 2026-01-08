#nullable enable

using System;
using Arch.Core;
using Game.Common;
using Game.DamageSystem.Components;
using Game.LocomotionSystem.Components;
using Game.PresentationSystem;
using Game.PresentationSystem.Components;
using Game.ProjectileSystem.Components;
using UnityEngine;

namespace Game.ProjectileSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(ProjectileSettings), menuName = "Assets/Projectile Settings")]
    [Serializable]
    public class ProjectileSettings : ScriptableObject, IDisposable, IPoolable, IEntityBuilder
    { 
        public GameObject? Prefab;
        public int PoolSize;

        public float Speed;
        public float Damage;
        public float MaxDistance;
        public float HitRadius;
        public bool DestroyOnHit;
        
        public void Dispose()
        {
        }

        public void Prepare(InstancePool instancePool)
        {
            if (Prefab != null)
            {
                instancePool.Register(Prefab, PoolSize);
            }
        }

        public void Build(Entity entity, BuildContext context)
        {
            context.CommandBuffer.Add(entity, new ProjectileData
            {
                MaxDistance = MaxDistance,
                HitRadius = HitRadius,
                DestroyOnHit = DestroyOnHit
            });

            context.CommandBuffer.Add(entity, new Damage { Amount = Damage });
            context.CommandBuffer.Add(entity, new LocomotionData { MaxSpeed = Speed });

            if (Prefab != null)
            {
                context.CommandBuffer.Add(entity, new PrefabId { Value = Prefab.GetInstanceID() });
            }            
        }
    }
}