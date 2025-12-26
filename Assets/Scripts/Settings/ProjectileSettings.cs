#nullable enable

using System;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = nameof(ProjectileSettings), menuName = "Assets/Projectile Settings")]
    [Serializable]
    public class ProjectileSettings : ScriptableObject, IDisposable
    {
        public GameObject Prefab;
        public float FireTimeout;
        public float Speed;
        public float MaxDistance;
        public float HitDistance;
 
        public void Dispose()
        {
        }
    }
}