#nullable enable

using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.ProjectileSystem.Systems;

namespace Game.AbilitySystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ProjectileSystemGroup))]
    public class AbilitySystemGroup : AbstractSystemGroup
    {
    }
}