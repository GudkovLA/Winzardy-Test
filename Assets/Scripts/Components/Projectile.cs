using UnityEngine;

namespace Game.Components
{
    public struct Projectile
    {
        public Vector3 Direction;
        public float Speed;
        public float Damage;
        public float HitDistance;
        public float MaxDistance;
        public float PassedDistance;
    }
}