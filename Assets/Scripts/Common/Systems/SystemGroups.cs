#nullable enable

using Game.Common.Systems.Attributes;

namespace Game.Common.Systems
{
    [DisableAutoCreation]
    public class LoopSystemGroup : AbstractSystemGroup
    {
    }

    [UpdateInGroup(typeof(LoopSystemGroup))]
    public class SimulationSystemGroup : AbstractSystemGroup
    {
    }

    [UpdateInGroup(typeof(LoopSystemGroup))]
    [UpdateAfter(typeof(SimulationSystemGroup))]
    public class LateSimulationSystemGroup : AbstractSystemGroup
    {
    }

    [UpdateInGroup(typeof(LoopSystemGroup))]
    [UpdateBefore(typeof(SimulationSystemGroup))]
    public class InitializationSystemGroup : AbstractSystemGroup
    {
    }

    [UpdateInGroup(typeof(LoopSystemGroup))]
    [UpdateAfter(typeof(LateSimulationSystemGroup))]
    public class CleanupSystemGroup : AbstractSystemGroup
    {
    }

    [UpdateInGroup(typeof(LoopSystemGroup))]
    [UpdateAfter(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(CleanupSystemGroup))]
    public class PresentationSystemGroup : AbstractSystemGroup
    {
    }
}