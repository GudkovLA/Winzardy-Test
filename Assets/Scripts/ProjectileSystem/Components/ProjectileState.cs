using Game.Common;
using UnityEngine;

namespace Game.ProjectileSystem.Components
{
    public struct ProjectileState
    {
        public float MaxDistance;
        public float PassedDistance;
        public float HitRadius;

        public EntityHandle LastHitEntity;
        public bool DestroyOnHit;
        
        // TODO: Separate shared component
        public Vector3 Direction;
        public float Speed;
    }
}