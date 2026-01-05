#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.Components;
using Game.DamageSystem.Components;
using Game.LocomotionSystem.Components;
using Game.ProjectileSystem.Components;
using UnityEngine;

namespace Game.ProjectileSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(ProjectileSettings), menuName = "Assets/Projectile Settings")]
    [Serializable]
    public class ProjectileSettings : ScriptableObject, IDisposable
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

        public void Initialize(CommandBuffer commandBuffer, Entity entity)
        {
            commandBuffer.Add(entity, new ProjectileData
            {
                MaxDistance = MaxDistance,
                HitRadius = HitRadius,
                DestroyOnHit = DestroyOnHit
            });

            commandBuffer.Add(entity, new Damage { Amount = Damage });
            commandBuffer.Add(entity, new LocomotionData { MaxSpeed = Speed });

            if (Prefab != null)
            {
                commandBuffer.Add(entity, new PrefabId { Value = Prefab.GetInstanceID() });
            }            
        }
    }
}