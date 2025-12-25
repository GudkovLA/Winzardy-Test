#nullable enable

using Game.Common.Systems;
using Game.Common.Systems.Attributes;

namespace Game.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class HudUpdateSystem : AbstractSystem
    {
        
    }
}