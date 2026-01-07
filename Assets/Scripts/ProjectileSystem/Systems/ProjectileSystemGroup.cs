#nullable enable

using Game.Common.Systems;
using Game.Common.Systems.Attributes;

namespace Game.ProjectileSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ProjectileSystemGroup : AbstractSystemGroup
    {
    }
}