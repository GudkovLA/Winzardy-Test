#nullable enable

using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.ProjectileSystem.Systems;

namespace Game.DamageSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileSystemGroup))]
    public class DamageHandleSystemGroup : AbstractSystemGroup
    {
    }
}