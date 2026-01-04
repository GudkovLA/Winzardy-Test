using Game.Common;

namespace Game.ProjectileSystem.Components
{
    public struct ProjectileContact
    {
        public EntityHandle ProjectileEntity;
        public EntityHandle TargetEntity;
        public ProjectileContactPhase ContactPhase;
    }
}