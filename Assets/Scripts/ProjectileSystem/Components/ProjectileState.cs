using Game.Common;

namespace Game.ProjectileSystem.Components
{
    public struct ProjectileState
    {
        public float MaxDistance;
        public float PassedDistance;
        public float HitRadius;

        public EntityHandle LastHitEntity;
        public bool DestroyOnHit;
    }
}