using UnityEngine;

namespace Game.ProjectileSystem.Components
{
    public struct Projectile
    {
        public Vector3 Direction;
        public float Speed;
        public float MaxDistance;
        public float PassedDistance;
    }
}