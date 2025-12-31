#nullable enable

using Game.Common;

namespace Game.AbilitySystem.Components
{
    public struct AbilityState
    {
        public int AbilityId;
        public EntityHandle OwnerEntity;
        public float LastActivateTime;
        public float ActivateTimeout;
    }
}