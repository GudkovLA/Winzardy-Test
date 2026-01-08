using Game.Common;

namespace Game.AbilitySystem.Components
{
    public struct Ability
    {
        public EntityHandle OwnerEntity;
        public float LastActivateTime;
    }
}