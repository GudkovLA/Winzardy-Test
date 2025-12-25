#nullable enable

using Game.Common.Systems;
using Game.Common.Systems.Attributes;

namespace Game.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class EnemySpawnSystem : AbstractSystem
    {
        // TODO: Use enemy pool
    }
}