#nullable enable

using System;
using UnityEngine;

namespace Game.Settings
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
 
        public void Dispose()
        {
        }
    }
}