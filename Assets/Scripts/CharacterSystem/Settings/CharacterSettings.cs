#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.Common.Components;
using Game.Components;
using Game.DamageSystem.Components;
using Game.LocomotionSystem.Components;
using Game.ProjectileSystem.Components;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterSettings), menuName = "Assets/Character Settings")]
    [Serializable]
    public class CharacterSettings : ScriptableObject, IDisposable
    {
        public GameObject? Prefab;
        
        public Vector3 Size;
        
        public float MaxHealth;
        public float Speed;
        
        private ProjectileCollider? _projectileCollider;

        public void Dispose()
        {
        }

        public void Initialize(Entity entity, CommandBuffer commandBuffer)
        {
            if (Prefab == null)
            {
                throw new Exception("Character prefab is not defined");
            }
            
            commandBuffer.Add(entity, new Size { Value = Size });
            commandBuffer.Add(entity, new LocomotionState { Speed = Speed });
            commandBuffer.Add(entity, new PrefabId { Value = Prefab.GetInstanceID() });
            commandBuffer.Add(entity, new HealthState
            {
                MaxHealth = MaxHealth,
                Health = MaxHealth
            });
            
            commandBuffer.Add(entity, GetProjectileCollider());
        }

        private ProjectileCollider GetProjectileCollider()
        {
            if (_projectileCollider != null)
            {
                return _projectileCollider.Value;
            }
            
            if (Prefab != null 
                && Prefab.TryGetComponent<CapsuleCollider>(out var capsuleCollider))
            {
                _projectileCollider = new ProjectileCollider { Radius = capsuleCollider.radius };
                return _projectileCollider.Value;
            }

            return default;
        }
    }
}